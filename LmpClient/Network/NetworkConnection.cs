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
        /// Set by background threads to request the main thread apply ForceQuit vs DisplayDisconnectMessage.
        /// </summary>
        public static volatile bool PendingDisconnect = false;

        /// <summary>
        /// Set by background threads to signal the main thread should force-quit to main menu.
        /// </summary>
        public static volatile bool PendingDisconnectForceQuit = false;

        /// <summary>
        /// Disconnects the network system.  Safe to call from any thread — Unity API decisions
        /// are deferred to the main thread via <see cref="PendingDisconnect"/>.
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

                    // Defer HighLogic scene checks to the main thread to avoid Mono/Linux deadlocks
                    PendingDisconnect = true;
                    PendingDisconnectForceQuit = false;

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
            MainSystem.Singleton.Status = $"Resolving {hostname}...";

            // Move all blocking work (DNS, hairpin detection, HTTP lookups) off the Unity thread
            SystemBase.TaskFactory.StartNew(() =>
            {
                try
                {
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

                    // NAT hairpin detection (fully local — no external HTTP calls required).
                    bool needsHairpinRedirect = false;

                    foreach (var addr in addresses)
                    {
                        if (!IsPublicIPv4(addr)) continue;

                        // Check 1: external IP check via HTTP (may fail if offline — non-fatal)
                        var ownExternal = LunaNetUtils.GetOwnExternalIpAddress();
                        if (ownExternal != null && Equals(addr, ownExternal))
                        {
                            needsHairpinRedirect = true;
                            break;
                        }

                        // Check 2: we're on a private network AND any LMP-like UDP server is
                        // listening locally
                        if (IsOnPrivateNetwork() && FindLocalServerPort(port) != -1)
                        {
                            needsHairpinRedirect = true;
                            break;
                        }
                    }

                    if (needsHairpinRedirect)
                    {
                        var localPort = FindLocalServerPort(port);
                        if (localPort != -1)
                        {
                            // Server is running on this machine — redirect to loopback.
                            LunaLog.Log($"[LMP]: NAT hairpin detected — redirecting to 127.0.0.1:{localPort} " +
                                        $"(was {string.Join(", ", addresses.Select(a => a.ToString()))}:{port})");
                            addresses = new[] { IPAddress.Loopback };
                            port      = localPort;
                        }
                        else
                        {
                            // Server is NOT on this machine but appears to be on the same LAN
                            // (same external IP).  Check the master-server list for the server's
                            // internal (LAN) endpoint and use that instead of the external IP.
                            var targetExternal = new IPEndPoint(addresses[0], port);
                            var lanEntry = NetworkServerList.Servers.Values
                                .FirstOrDefault(s => s.ExternalEndpoint != null &&
                                                     s.ExternalEndpoint.Address.Equals(targetExternal.Address) &&
                                                     s.ExternalEndpoint.Port == targetExternal.Port);

                            if (lanEntry?.InternalEndpoint != null &&
                                !lanEntry.InternalEndpoint.Address.Equals(IPAddress.Loopback))
                            {
                                LunaLog.Log($"[LMP]: NAT hairpin detected — no local server, but found LAN entry in server list. " +
                                            $"Redirecting to internal endpoint {lanEntry.InternalEndpoint} " +
                                            $"(was {targetExternal})");
                                addresses = new[] { lanEntry.InternalEndpoint.Address };
                                port      = lanEntry.InternalEndpoint.Port;
                            }
                            else
                            {
                                LunaLog.Log($"[LMP]: NAT hairpin suspected (your external IP matches the server's) but " +
                                            $"no local server or LAN entry found — proceeding with original endpoint. " +
                                            $"If you are on the same network as the server, connect via its local IP instead, " +
                                            $"or disconnect from that network to test external connectivity.");
                            }
                        }
                    }

                    var endpoints = addresses.Select(addr => new IPEndPoint(addr, port)).ToArray();
                    ConnectToServerCore(endpoints, password);
                }
                catch (Exception ex)
                {
                    LunaLog.LogError($"[LMP]: Connection setup failed: {ex}");
                    Disconnect($"Connection setup failed: {ex.Message}");
                }
            });
        }

        // ── Connect (endpoint overload) ───────────────────────────────────────

        /// <summary>
        /// Internal core connection logic. Callers must already be on a background thread.
        /// For hostname-based connects, use <see cref="ConnectToServer(string, int, string)"/>.
        /// </summary>
        public static void ConnectToServerCore(IPEndPoint[] endpoints, string password)
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

                int totalAttempts = endpoints.Length;
                LunaLog.Log($"[LMP]: Beginning connection sequence — {totalAttempts} endpoint(s) to try");

                for (int i = 0; i < totalAttempts; i++)
                {
                    var endpoint = endpoints[i];
                    if (endpoint == null)
                    {
                        LunaLog.Log($"[LMP]: Endpoint {i + 1}/{totalAttempts} is null — skipping");
                        continue;
                    }

                    int attemptNumber = i + 1;
                    int progressPercent = (attemptNumber * 100) / totalAttempts;
                    string progressBar = BuildProgressBar(attemptNumber, totalAttempts);

                    MainSystem.Singleton.Status = $"[{progressBar}] Connecting to {endpoint.Address}:{endpoint.Port} ({attemptNumber}/{totalAttempts})";
                    LunaLog.Log($"[LMP]: ── Attempt {attemptNumber}/{totalAttempts} ──────────────────────────");
                    LunaLog.Log($"[LMP]: Target endpoint: {endpoint.Address}:{endpoint.Port} (family: {endpoint.AddressFamily})");
                    LunaLog.Log($"[LMP]: Connection progress: {progressPercent}% ({attemptNumber} of {totalAttempts} endpoints)");

                    try
                    {
                        var client = NetworkMain.ClientConnection;

                        if (client.Status == NetPeerStatus.NotRunning)
                        {
                            LunaLog.Log("[LMP]: Starting Lidgren UDP client");
                            client.Start();
                        }

                        int startupWaitCycles = 0;
                        while (client.Status != NetPeerStatus.Running)
                        {
                            Thread.Sleep(50);
                            startupWaitCycles++;
                            if (startupWaitCycles % 20 == 0) // log every ~1s
                                LunaLog.Log($"[LMP]: Waiting for Lidgren client to start... ({startupWaitCycles * 50}ms)");
                        }
                        if (startupWaitCycles > 0)
                            LunaLog.Log($"[LMP]: Lidgren client started after {startupWaitCycles * 50}ms");

                        var outMsg = client.CreateMessage(password.GetByteCount());
                        outMsg.Write(password);
                        LunaLog.Log($"[LMP]: Sending handshake with {password.GetByteCount()} byte password payload");

                        var conn = client.Connect(endpoint, outMsg);
                        if (conn == null)
                        {
                            LunaLog.LogError($"[LMP]: Invalid connection state — Lidgren returned null connection object on attempt {attemptNumber}/{totalAttempts}");
                            client.Disconnect("Invalid state");
                            break;
                        }
                        LunaLog.Log($"[LMP]: Handshake initiated — Lidgren connection status: {conn.Status}");
                        client.FlushSendQueue();

                        // Wait for Lidgren to resolve the handshake on its own schedule.
                        int handshakeWaitCycles = 0;
                        while (conn.Status == NetConnectionStatus.InitiatedConnect || conn.Status == NetConnectionStatus.None)
                        {
                            Thread.Sleep(50);
                            handshakeWaitCycles++;
                            // Update status periodically so the UI doesn't look frozen
                            if (handshakeWaitCycles % 10 == 0)
                            {
                                float elapsedSec = handshakeWaitCycles * 50 / 1000f;
                                MainSystem.Singleton.Status = $"[{progressBar}] Handshaking with {endpoint.Address}:{endpoint.Port}... ({elapsedSec:F1}s)";
                            }
                        }

                        float totalHandshakeSec = handshakeWaitCycles * 50 / 1000f;
                        LunaLog.Log($"[LMP]: Handshake wait completed after {totalHandshakeSec:F1}s — Lidgren status: {conn.Status}");

                        if (client.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            LunaLog.Log($"[LMP]: ✓ CONNECTED to {endpoint.Address}:{endpoint.Port} on attempt {attemptNumber}/{totalAttempts} (after {totalHandshakeSec:F1}s)");
                            MainSystem.Singleton.Status = $"Connected to {endpoint.Address}:{endpoint.Port}!";
                            MainSystem.NetworkState = ClientState.Connected;
                            break;
                        }

                        // Handshake failed — capture Lidgren stats BEFORE we tear down the
                        // connection so we know whether the server sent us ANY UDP packets.
                        var receivedPackets = conn.Statistics.ReceivedPackets;

                        LunaLog.Log($"[LMP]: ✗ Handshake FAILED to {endpoint.Address}:{endpoint.Port} " +
                                    $"(attempt {attemptNumber}/{totalAttempts}, status={conn.Status}, " +
                                    $"remoteUdpPacketsReceived={receivedPackets}, waited={totalHandshakeSec:F1}s)");

                        client.Disconnect("Initial connection timeout");
                        LastFailureReason = DiagnoseTimeout(endpoint, receivedPackets);

                        // Log remaining endpoints for traceability
                        int remaining = totalAttempts - attemptNumber;
                        if (remaining > 0)
                            LunaLog.Log($"[LMP]: {remaining} endpoint(s) remaining in connection sequence");
                    }
                    catch (Exception e)
                    {
                        LunaLog.LogError($"[LMP]: Exception during connection attempt {attemptNumber}/{totalAttempts} to {endpoint.Address}:{endpoint.Port}: {e.GetType().Name}: {e.Message}");
                        NetworkMain.HandleDisconnectException(e);
                    }
                }

                if (MainSystem.NetworkState < ClientState.Connected)
                {
                    LunaLog.Log($"[LMP]: All {totalAttempts} connection attempt(s) exhausted — moving to disconnect");

                    // Prefer the diagnostic reason (set above) over the generic fallback
                    var reason = MainSystem.NetworkState == ClientState.Connecting
                        ? (string.IsNullOrEmpty(LastFailureReason)
                            ? "No response from server — check the address/port and server status"
                            : LastFailureReason)
                        : "Cancelled connection";

                    LunaLog.Log($"[LMP]: Final disconnect reason: {reason}");
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

        // ── NAT hairpin helpers ───────────────────────────────────────────────

        /// <summary>
        /// Finds the local UDP port an LMP server is actually bound to.
        /// Returns the port number, or -1 if no suitable listener is found.
        /// Strategy: prefer the exact <paramref name="externalPort"/> (NAT 1:1), then the LMP
        /// default 8800.  Never guesses an arbitrary port.
        /// </summary>
        private static int FindLocalServerPort(int externalPort)
        {
            try
            {
                var listeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();

                // Fast path: external port == local port (NAT uses the same port number)
                if (listeners.Any(ep => ep.Port == externalPort))
                    return externalPort;

                // LMP default port
                if (listeners.Any(ep => ep.Port == 8800))
                    return 8800;

                // No known LMP listener found — do not guess an arbitrary port
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>Returns true for globally-routable IPv4 addresses (not loopback, RFC-1918, or link-local).</summary>
        private static bool IsPublicIPv4(IPAddress addr)
        {
            if (addr.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return false;
            if (addr.Equals(IPAddress.Loopback))
                return false;
            var b = addr.GetAddressBytes();
            if (b[0] == 10)                                  return false; // 10/8
            if (b[0] == 172 && b[1] >= 16 && b[1] <= 31)   return false; // 172.16-31/12
            if (b[0] == 192 && b[1] == 168)                 return false; // 192.168/16
            if (b[0] == 169 && b[1] == 254)                 return false; // 169.254/16 link-local
            return true;
        }

        /// <summary>Returns true when our own outbound IPv4 address is in a private range (i.e. we're behind NAT).</summary>
        private static bool IsOnPrivateNetwork()
        {
            var own = LunaNetUtils.GetOwnInternalIPv4Address();
            return own != null && !IsPublicIPv4(own);
        }

        // ── Progress UI helpers ───────────────────────────────────────────────

        /// <summary>
        /// Builds a simple ASCII progress bar (e.g. "[==>    ]" for 1 of 3).
        /// </summary>
        private static string BuildProgressBar(int current, int total)
        {
            const int barWidth = 10;
            if (total <= 0) return new string(' ', barWidth);
            int filled = (current * barWidth) / total;
            if (filled > barWidth) filled = barWidth;
            return new string('=', filled).PadRight(barWidth, ' ');
        }

        // ── Post-timeout ICMP diagnostics ─────────────────────────────────────

        /// <summary>
        /// After a UDP handshake timeout, diagnoses why the connection failed.
        ///
        /// Primary signal: <paramref name="receivedPackets"/> tells us whether the server sent
        /// any UDP packets back at all.  This is far more reliable than ICMP (which is often
        /// blocked by firewalls) because it tests the exact protocol and port we care about.
        ///
        /// <list type="bullet">
        ///   <item>receivedPackets == 0 → server never sent us a single UDP packet.
        ///       Causes: server down, wrong address/port, UDP completely firewalled, or severe packet loss.</item>
        ///   <item>receivedPackets &gt; 0 → server IS reachable over UDP but the handshake still failed.
        ///       Causes: password rejected, version mismatch, server full, NAT punchthrough needed but not completed.</item>
        /// </list>
        ///
        /// ICMP is used only as a secondary hint and is explicitly caveated because many hosts
        /// (cloud VMs, corporate networks, etc.) block it by default.
        ///
        /// Returns an actionable human-readable string stored in <see cref="LastFailureReason"/>
        /// and shown in both KSP.log and the ConnectingWindow failure display.
        /// </summary>
        private static string DiagnoseTimeout(IPEndPoint endpoint, long receivedPackets)
        {
            LunaLog.Log($"[LMP]: === Diagnostics for {endpoint.Address}:{endpoint.Port} ===");

            // ── Primary diagnostic: Lidgren UDP packet count ─────────────────
            if (receivedPackets > 0)
            {
                LunaLog.Log($"[LMP]: Server sent {receivedPackets} UDP packet(s) — host is reachable over UDP, " +
                            $"but the Lidgren handshake did not complete. " +
                            $"Likely causes: password mismatch, incompatible LMP version, server full, " +
                            $"or NAT punchthrough still pending.");

                return $"Server {endpoint.Address}:{endpoint.Port} responded over UDP, but the handshake failed. " +
                       $"Check: password is correct, your LMP version matches the server, " +
                       $"and the server is not full. If connecting over the internet, ensure NAT punchthrough completed.";
            }

            // receivedPackets == 0: server never sent us anything over UDP.
            // This is the same symptom for "server down", "wrong port", and "UDP firewalled".
            LunaLog.Log($"[LMP]: Server sent 0 UDP packets — no response at all on {endpoint.Address}:{endpoint.Port}. " +
                        $"This usually means the server is down, the address/port is wrong, or UDP is blocked by a firewall.");

            // ── Secondary hint: ICMP ping (heavily caveated) ──────────────────
            try
            {
                using (var ping = new Ping())
                {
                    LunaLog.Log($"[LMP]: ICMP ping → {endpoint.Address} (3 s timeout)...");
                    var reply = ping.Send(endpoint.Address, 3000);

                    if (reply.Status == IPStatus.Success)
                    {
                        LunaLog.Log($"[LMP]: ICMP ping SUCCESS ({reply.RoundtripTime} ms). " +
                                    $"Host is alive but did not reply over UDP port {endpoint.Port}. " +
                                    $"The LMP server is probably not running or UDP {endpoint.Port} is firewalled.");

                        return $"No UDP response from {endpoint.Address}:{endpoint.Port}. " +
                               $"Host is reachable ({reply.RoundtripTime} ms ICMP ping), but the LMP server did not respond over UDP. " +
                               $"Most likely: the server is not running, or UDP port {endpoint.Port} is blocked by a firewall/router.";
                    }
                    else
                    {
                        LunaLog.Log($"[LMP]: ICMP ping got no reply ({reply.Status}). " +
                                    $"NOTE: many firewalls block ICMP — this does NOT prove the server is down. " +
                                    $"The UDP timeout itself is the authoritative failure signal.");

                        return $"No UDP response from {endpoint.Address}:{endpoint.Port}. " +
                               $"(ICMP ping also failed with {reply.Status}, but many firewalls block ICMP so this is inconclusive.) " +
                               $"Most likely: the server is not running, the address/port is wrong, or UDP {endpoint.Port} is blocked.";
                    }
                }
            }
            catch (PlatformNotSupportedException)
            {
                LunaLog.Log($"[LMP]: ICMP ping not available on this platform — relying on UDP timeout only.");
            }
            catch (InvalidOperationException)
            {
                LunaLog.Log($"[LMP]: ICMP ping unavailable in this runtime context — relying on UDP timeout only.");
            }
            catch (Exception ex)
            {
                LunaLog.Log($"[LMP]: ICMP ping error ({ex.GetType().Name}) — ignoring.");
            }

            // Fallback when ICMP is unavailable or inconclusive
            return $"No UDP response from {endpoint.Address}:{endpoint.Port}. " +
                   $"The server may be offline, the address/port may be wrong, or UDP {endpoint.Port} may be blocked by a firewall.";
        }
    }
}
