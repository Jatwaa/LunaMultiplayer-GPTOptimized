using Server.Context;
using Server.Events;
using Server.Log;
using Server.Settings.Structures;
using Server.Web.Structures;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Web
{
    /// <summary>
    /// HTTP server that exposes JSON APIs and static pages for the LMP dashboard.
    ///
    /// Endpoints
    ///   GET  /                  → index.html
    ///   GET  /map  /map.html    → map.html
    ///   GET  /api/telemetry     → TelemetryData JSON  (player list + count)
    ///   GET  /api/chat          → ChatData JSON        (message history)
    ///   POST /api/chat          → body: text=…         (broadcast to all players)
    ///   GET  /api/map           → MapData JSON
    ///   GET  /telemetry.json    → same as /api/telemetry but as a static-file path
    ///                             (written to disk each refresh cycle as well)
    ///   GET  /chat.json         → latest chat written to disk each refresh cycle
    /// </summary>
    public static class WebServer
    {
        // ── Shared data stores ───────────────────────────────────────────────
        public static readonly ServerInformation ServerInformation = new ServerInformation();
        public static readonly MapData            MapData           = new MapData();
        public static readonly ChatData           ChatData          = new ChatData();
        public static readonly TelemetryData      TelemetryData     = new TelemetryData();

        // ── HTTP listener ────────────────────────────────────────────────────
        private static HttpListener? _listener;

        private static readonly JsonSerializerOptions _jsonOpts =
            new JsonSerializerOptions { WriteIndented = false };

        // ── Static pages directory ───────────────────────────────────────────
        private static string PagesDir =>
            Path.Combine(AppContext.BaseDirectory, "Web", "Pages");

        static WebServer() => ExitEvent.ServerClosing += StopWebServer;

        // ────────────────────────────────────────────────────────────────────
        // Start / Stop
        // ────────────────────────────────────────────────────────────────────

        public static void StartWebServer()
        {
            if (!WebsiteSettings.SettingsStore.EnableWebsite) return;

            var port = WebsiteSettings.SettingsStore.Port;

            try
            {
                _listener = new HttpListener();

                // Prefer binding to all interfaces (http://+) so the map is
                // reachable from other machines on the network.  Windows requires
                // either an elevated process or a one-time netsh ACL entry:
                //   netsh http add urlacl url=http://+:8900/ user=Everyone
                // If that is not present we silently fall back to localhost-only.
                bool bound = false;
                foreach (var (prefix, localOnly) in new[]
                {
                    ($"http://+:{port}/",         false),
                    ($"http://localhost:{port}/",  true)
                })
                {
                    try
                    {
                        _listener = new HttpListener();
                        _listener.Prefixes.Add(prefix);
                        _listener.Start();

                        if (localOnly)
                        {
                            LunaLog.Normal($"Web server started (localhost only) → http://localhost:{port}/");
                            LunaLog.Normal($"  To allow external access run once as admin:");
                            LunaLog.Normal($"  netsh http add urlacl url=http://+:{port}/ user=Everyone");
                        }
                        else
                        {
                            LunaLog.Normal($"Web server started (all interfaces) → http://+:{port}/");
                        }

                        bound = true;
                        break;
                    }
                    catch (HttpListenerException)
                    {
                        try { _listener?.Close(); } catch { }
                    }
                }

                if (!bound)
                {
                    LunaLog.Error($"Web server could not start on port {port}. Is the port already in use?");
                    return;
                }

                _ = AcceptLoopAsync();
            }
            catch (Exception e)
            {
                LunaLog.Error($"Could not start web server. Details: {e}");
            }
        }

        public static void StopWebServer()
        {
            if (!WebsiteSettings.SettingsStore.EnableWebsite) return;
            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch { }
        }

        // ────────────────────────────────────────────────────────────────────
        // Periodic refresh (called from ServerContext main loop)
        // ────────────────────────────────────────────────────────────────────

        public static async Task RefreshWebServerInformationAsync()
        {
            if (!WebsiteSettings.SettingsStore.EnableWebsite) return;

            while (ServerContext.ServerRunning)
            {
                ServerInformation.Refresh();
                MapData.Refresh();
                ChatData.Refresh();
                TelemetryData.Refresh();

                // Write JSON snapshots to disk so map.html can read them
                // as plain static files (no CORS / API dependency).
                WriteToDisk("telemetry.json", TelemetryData);
                WriteToDisk("chat.json",      ChatData);

                await Task.Delay(WebsiteSettings.SettingsStore.RefreshIntervalMs);
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Accept loop
        // ────────────────────────────────────────────────────────────────────

        private static async Task AcceptLoopAsync()
        {
            while (_listener?.IsListening == true)
            {
                try
                {
                    var ctx = await _listener.GetContextAsync().ConfigureAwait(false);
                    _ = HandleAsync(ctx);
                }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    LunaLog.Warning($"Web accept error: {ex.Message}");
                }
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Request dispatch
        // ────────────────────────────────────────────────────────────────────

        private static async Task HandleAsync(HttpListenerContext ctx)
        {
            try
            {
                // Add CORS so browser-based pages on other origins can call us
                ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");
                ctx.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                var method = ctx.Request.HttpMethod.ToUpperInvariant();
                var path   = (ctx.Request.Url?.AbsolutePath ?? "/")
                             .Trim('/').ToLowerInvariant();

                // ── CORS pre-flight ──────────────────────────────────────────
                if (method == "OPTIONS")
                {
                    ctx.Response.StatusCode = 204;
                    ctx.Response.Close();
                    return;
                }

                // ── API routes ───────────────────────────────────────────────
                switch (path)
                {
                    // GET /api/telemetry  or  /telemetry.json
                    case "api/telemetry":
                    case "telemetry.json":
                        TelemetryData.Refresh();
                        await SendJsonAsync(ctx, TelemetryData);
                        return;

                    // GET /api/chat
                    case "api/chat":
                    case "chat.json":
                        if (method == "POST")
                        {
                            await HandleChatPostAsync(ctx);
                        }
                        else
                        {
                            ChatData.Refresh();
                            await SendJsonAsync(ctx, ChatData);
                        }
                        return;

                    // GET /api/map
                    case "api/map":
                        MapData.Refresh();
                        await SendJsonAsync(ctx, MapData);
                        return;

                    // GET /  root  → index.html
                    case "":
                    case "index.html":
                        await ServeFileAsync(ctx, "index.html");
                        return;

                    // GET /map  /map/  /map.html  → map.html
                    case "map":
                    case "map/":
                    case "map.html":
                        await ServeFileAsync(ctx, "map.html");
                        return;
                }

                // ── Static file fallback ─────────────────────────────────────
                // Allows serving telemetry.json / chat.json written to Pages dir.
                var filePath = Path.GetFullPath(
                    Path.Combine(PagesDir, path.Replace('/', Path.DirectorySeparatorChar)));

                // Security: ensure the resolved path is inside PagesDir
                if (!filePath.StartsWith(PagesDir, StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.Close();
                    return;
                }

                if (File.Exists(filePath))
                {
                    await ServeFileAsync(ctx, path);
                }
                else
                {
                    ctx.Response.StatusCode = 404;
                    ctx.Response.Close();
                }
            }
            catch (Exception ex)
            {
                LunaLog.Warning($"Web request error: {ex.Message}");
                try { ctx.Response.StatusCode = 500; ctx.Response.Close(); } catch { }
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // POST /api/chat handler
        // ────────────────────────────────────────────────────────────────────

        private static async Task HandleChatPostAsync(HttpListenerContext ctx)
        {
            string body;
            using (var reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8, leaveOpen: true))
                body = await reader.ReadToEndAsync().ConfigureAwait(false);

            var text = ParseFormValue(body, "text");
            if (!string.IsNullOrWhiteSpace(text))
            {
                ChatData.AddMessage("[Server]", text);
                ChatData.Broadcast("[Server]", text);

                // Immediately refresh so the response includes the new message
                ChatData.Refresh();
            }

            await SendJsonAsync(ctx, ChatData);
        }

        // ────────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────────

        private static async Task SendJsonAsync(HttpListenerContext ctx, object data)
        {
            var json  = JsonSerializer.Serialize(data, _jsonOpts);
            var bytes = Encoding.UTF8.GetBytes(json);
            ctx.Response.ContentType     = "application/json; charset=utf-8";
            ctx.Response.StatusCode      = 200;
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes).ConfigureAwait(false);
            ctx.Response.Close();
        }

        private static async Task ServeFileAsync(HttpListenerContext ctx, string relativePath)
        {
            var filePath = Path.Combine(PagesDir,
                relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(filePath))
            {
                ctx.Response.StatusCode = 404;
                ctx.Response.Close();
                return;
            }

            var bytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
            ctx.Response.ContentType     = GetContentType(relativePath);
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes).ConfigureAwait(false);
            ctx.Response.Close();
        }

        /// <summary>Write an object as JSON to the Pages directory (best-effort).</summary>
        private static void WriteToDisk(string filename, object data)
        {
            try
            {
                var path = Path.Combine(PagesDir, filename);
                var json = JsonSerializer.Serialize(data, _jsonOpts);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch { /* non-fatal */ }
        }

        private static string GetContentType(string path) =>
            Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".html" => "text/html; charset=utf-8",
                ".js"   => "application/javascript; charset=utf-8",
                ".css"  => "text/css; charset=utf-8",
                ".json" => "application/json; charset=utf-8",
                ".png"  => "image/png",
                ".ico"  => "image/x-icon",
                _       => "application/octet-stream"
            };

        private static string ParseFormValue(string body, string key)
        {
            // Parse application/x-www-form-urlencoded body
            foreach (var part in body.Split('&'))
            {
                var eq = part.IndexOf('=');
                if (eq < 0) continue;
                var k = Uri.UnescapeDataString(part[..eq].Trim());
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return Uri.UnescapeDataString(part[(eq + 1)..].Replace('+', ' '));
            }
            return string.Empty;
        }
    }
}
