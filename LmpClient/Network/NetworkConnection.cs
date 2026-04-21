using Lidgren.Network;
using LmpClient.Base;
using LmpClient.ModuleStore.Patching;
using LmpClient.Systems.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using LmpCommon.Enums;
using LmpCommon.Message.Base;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using UniLinq;

namespace LmpClient.Network
{
    public class NetworkConnection
    {
        private static readonly object DisconnectLock = new object();
        public static volatile bool ResetRequested;

        // ── Failure tracking (read by ConnectingWindow) ───────────────────────
        /// <summary>Human-readable reason for the last disconnect (empty = clean disconnect).</summary>
        public static volatile string LastFailureReason = "";
        /// <summary>Environment.TickCount at the moment of failure, for linger-display timing.</summary>
        public static volatile int LastFailureTickCount = 0;
        /// <summary>The ClientState we were in when the failure occurred.</summary>
        public static volatile ClientState LastFailedAtState = ClientState.Disconnected;

        // ── Disconnect ────────────────────────────────────────────────────────

        /// <summary>
        /// Disconnects the network system.  Must be called from the main thread when killing threads.
        /// </summary>
        public static void Disconnect(string reason = "unknown")
        {
            lock (DisconnectLock)
            {
                if (MainSystem.NetworkState > ClientState.Disconnected)
                {
                    // Capture state BEFORE changing it so the UI can show which step failed
                    LastFailedAtState    = MainSystem.NetworkState;
                    LastFailureReason    = reason ?? "";
                    LastFailureTickCount = System.Environment.TickCount;

                    //DO NOT set networkstate as disconnected as we are in another thread!
                    MainSystem.NetworkState = ClientState.DisconnectRequested;

                    LunaLog.Log($"[LMP]: Disconnected, reason: {reason}");
                    if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight)
                    {
                        MainSystem.Singleton.ForceQuit = true;
                    }
                    else
                    {
                        //User is in flight so just display a message but don't force them to main menu...
                        NetworkSystem.DisplayDisconnectMessage = true;
                    }

                    MainSystem.Singleton.Status = $"Disconnected: {reason}";

                    // Guard against calling Disconnect on a peer that Lidgren already moved to
                    // Disconnected state (e.g. after an initial handshake timeout).  Calling it
                    // on an already-disconnected peer produces the harmless but confusing warning:
                    //   "Disconnect requested when not connected!"
                    if (NetworkMain.ClientConnection.Status == NetPeerStatus.Running)
                        NetworkMain.ClientConnection.Disconnect(reason);

                    NetworkMain.ClientConnection.Shutdown(reason);
                    NetworkMain.ResetConnectionStaticsAndQueues();
                }
            }
        }

        // ── Connect (hostname overload) ───────────────────────────────────────

        public static void ConnectToServer(string hostname, int port, string password)
        {
            // Clear previous failure before a new attempt
            LastFailureReason = "";

            // ── DNS / IP resolution with full diagnostic logging ──────────────
            IPAddress[] addresses;

            if (IPAddress.TryParse(hostname, out var directIp))
            {
                addresses = new[] { directIp };
                LunaLog.Log($"[LMP]: Target is a direct IP — {hostname}:{port} (UDP)");
            }
            else
            {
                LunaLog.Log($"[LMP]: Resolving hostname '{hostname}' for port {port}...");
                try
                {
                    addresses = Dns.GetHostAddresses(hostname);
                    if (addresses == null || addresses.Length == 0)
                    {
                        var msg = $"Hostname '{hostname}' resolved to no addresses — check for typos or DNS issues";
                        LunaLog.LogError($"[LMP]: {msg}");
                        MainSystem.Singleton.Status = msg;
                        Disconnect(msg);
                        return;
                    }
                    LunaLog.Log($"[LMP]: '{hostname}' resolved to {addresses.Length} address(es): " +
                                $"{string.Join(", ", addresses.Select(a => a.ToString()))}");
                }
                catch (Exception ex)
                {
                    var msg = $"Hostname resolution failed for '{hostname}': {ex.Message}";
                    LunaLog.LogError($"[LMP]: {msg}");
                    MainSystem.Singleton.Status = msg;
                    Disconnect(msg);
                    return;
                }
            }

            var endpoints = addresses.Select(addr => new IPEndPoint(addr, port)).ToArray();
            ConnectToServer(endpoints, password);
        }

        // ── Connect (endpoint overload) ───────────────────────────────────────

        public static void ConnectToServer(IPEndPoint[] endpoints, string password)
        {
            if (MainSystem.NetworkState > ClientState.Disconnected || endpoints == null || endpoints.Length == 0)
                return;

            // Clear any previous failure display when starting a fresh connection attempt
            LastFailureReason = "";

            MainSystem.NetworkState = ClientState.Connecting;

            SystemBase.TaskFactory.StartNew(() =>
            {
                while (!PartModuleRunner.Ready)
                {
                    MainSystem.Singleton.Status = $"Patching part modules (runs on every restart). {PartModuleRunner.GetPercentage()}%";
                    Thread.Sleep(50);
                }

                // Log local network context and Lidgren settings once before any attempt
                LogConnectionPreamble();

                foreach (var endpoint in endpoints)
                {
                    if (endpoint == null)
                        continue;

                    MainSystem.Singleton.Status = $"Connecting to {endpoint.Address}:{endpoint.Port}";
                    LunaLog.Log($"[LMP]: Connecting to {endpoint.Address} port {endpoint.Port}");

                    try
                    {
                        var client = NetworkMain.ClientConnection;

                        if (client.Status == NetPeerStatus.NotRunning)
                        {
                            LunaLog.Log("[LMP]: Starting Lidgren UDP client");
                            client.Start();
                        }

                        while (client.Status != NetPeerStatus.Running)
                        {
                            // Still trying to start up
                            Thread.Sleep(50);
                        }

                        var outMsg = client.CreateMessage(password.GetByteCount());
                        outMsg.Write(password);

                        var conn = client.Connect(endpoint, outMsg);
                        if (conn == null)
                        {
                            // Lidgren says we're already connected — unexpected state
                            LunaLog.LogError($"[LMP]: Invalid connection state — Lidgren returned null connection object");
                            client.Disconnect("Invalid state");
                            break;
                        }
                        client.FlushSendQueue();

                        while (conn.Status == NetConnectionStatus.InitiatedConnect || conn.Status == NetConnectionStatus.None)
                        {
                            // Waiting for handshake to complete or time out
                            Thread.Sleep(50);
                        }

                        if (client.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            LunaLog.Log($"[LMP]: Connected to {endpoint.Address}:{endpoint.Port}");
                            MainSystem.NetworkState = ClientState.Connected;
                            break;
                        }
                        else
                        {
                            // Lidgren has already moved the peer to Disconnected state here.
                            // Do NOT call client.Disconnect() — it's a no-op that prints:
                            //   "[Lidgren WARNING] Disconnect requested when not connected!"
                            LunaLog.Log($"[LMP]: No handshake response from {endpoint.Address}:{endpoint.Port} — running diagnostics...");

                            // Ping the host to distinguish "server down / wrong address" from
                            // "server up but UDP port is firewalled".  Result is stored in
                            // LastFailureReason so both the log and the connecting UI show it.
                            LastFailureReason = DiagnoseTimeout(endpoint);
                        }
                    }
                    catch (Exception e)
                    {
                        NetworkMain.HandleDisconnectException(e);
                    }
                }

                if (MainSystem.NetworkState < ClientState.Connected)
                {
                    // Prefer the diagnostic reason (set above) over the generic fallback
                    var reason = MainSystem.NetworkState == ClientState.Connecting
                        ? (string.IsNullOrEmpty(LastFailureReason)
                            ? "No response from server — check the address/port and server status"
                            : LastFailureReason)
                        : "Cancelled connection";

                    Disconnect(reason);
                }
            });
        }

        // ── Pre-attempt diagnostic logging ────────────────────────────────────

        /// <summary>
        /// Logs local network state and the Lidgren connection settings once per connection attempt.
        /// Gives support staff the context they need without reading the full Lidgren config.
        /// </summary>
        private static void LogConnectionPreamble()
        {
            try
            {
                var localIp = LunaNetUtils.GetOwnInternalIPv4Address();
                LunaLog.Log($"[LMP]: Local IPv4 interface: {localIp}");
            }
            catch (Exception ex)
            {
                LunaLog.Log($"[LMP]: Could not determine local IP: {ex.Message}");
            }

            try
            {
                var s = SettingsSystem.CurrentSettings;
                LunaLog.Log($"[LMP]: Lidgren settings — timeout: {s.TimeoutSeconds}s, " +
                            $"handshake attempts: {s.ConnectionTries}, " +
                            $"retry interval: {s.MsBetweenConnectionTries}ms, " +
                            $"MTU: {s.Mtu} (auto-expand: {s.AutoExpandMtu})");
            }
            catch (Exception ex)
            {
                LunaLog.Log($"[LMP]: Could not read connection settings: {ex.Message}");
            }
        }

        // ── Post-timeout ICMP diagnostics ─────────────────────────────────────

        /// <summary>
        /// After a UDP handshake timeout, sends an ICMP ping to determine whether the host
        /// itself is reachable.  This separates two very different failure causes:
        /// <list type="bullet">
        ///   <item>Ping succeeds → host is alive; UDP port is probably blocked by a firewall or the server is not running.</item>
        ///   <item>Ping fails   → host is unreachable; server may be offline or the address is wrong.</item>
        /// </list>
        /// Returns an actionable human-readable string that is stored in <see cref="LastFailureReason"/>
        /// and shown in both KSP.log and the ConnectingWindow failure display.
        /// </summary>
        private static string DiagnoseTimeout(IPEndPoint endpoint)
        {
            LunaLog.Log($"[LMP]: === Diagnostics for {endpoint.Address}:{endpoint.Port} ===");

            // Log local IP again so it's adjacent to the failure in the log
            try
            {
                var localIp = LunaNetUtils.GetOwnInternalIPv4Address();
                LunaLog.Log($"[LMP]: Local interface: {localIp}");
            }
            catch { /* non-fatal */ }

            // ── ICMP ping ─────────────────────────────────────────────────────
            try
            {
                using (var ping = new Ping())
                {
                    LunaLog.Log($"[LMP]: ICMP ping → {endpoint.Address} (3 s timeout)...");
                    var reply = ping.Send(endpoint.Address, 3000);

                    if (reply.Status == IPStatus.Success)
                    {
                        LunaLog.Log($"[LMP]: ICMP ping SUCCESS — {endpoint.Address} responded in {reply.RoundtripTime} ms. " +
                                    $"Host is reachable; UDP port {endpoint.Port} may be blocked by a firewall or the LMP server is not running.");

                        return $"No UDP response on port {endpoint.Port} — host {endpoint.Address} is reachable " +
                               $"({reply.RoundtripTime} ms ping). Check: server is running, UDP {endpoint.Port} is " +
                               $"open in the server firewall, and router port-forwarding is correct.";
                    }
                    else
                    {
                        LunaLog.Log($"[LMP]: ICMP ping FAILED — {endpoint.Address} status: {reply.Status}. " +
                                    $"Host appears unreachable. Possible causes: wrong address, server offline, ICMP blocked.");

                        return $"No response from {endpoint.Address}:{endpoint.Port} — host also did not respond " +
                               $"to ping ({reply.Status}). Check: the server address is correct, the server machine is online, " +
                               $"and you have internet connectivity.";
                    }
                }
            }
            catch (PlatformNotSupportedException)
            {
                // Unity/Mono on some platforms disallows raw ICMP sockets
                LunaLog.Log($"[LMP]: ICMP ping not available on this platform — skipping ping check.");
                return $"No response from {endpoint.Address}:{endpoint.Port} (UDP). " +
                       $"Could not run ping check on this platform. " +
                       $"Verify the address/port, that the server is running, and that UDP {endpoint.Port} is not firewalled.";
            }
            catch (Exception ex)
            {
                LunaLog.Log($"[LMP]: ICMP ping error: {ex.GetType().Name}: {ex.Message}");
                return $"No response from {endpoint.Address}:{endpoint.Port} (UDP). " +
                       $"Ping check failed: {ex.Message}. " +
                       $"Verify the server address, port, and firewall rules.";
            }
        }
    }
}
