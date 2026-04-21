using System;
using System.Linq;
using Server.Context;
using Server.Log;
using Server.System;

namespace Server.Log
{
    public static class ServerUI
    {
        private static readonly object _lock = new object();
        private static int _frameCount;
        private static long _lastUpdate;
        private static int _playerCount;
        private static int _vesselCount;
        private static int _connectionsCount;
        private static double _avgTickLatency;
        private static int _tickOverruns;
        private static long _bytesSentSec;
        private static long _bytesRecvSec;

        public static void Initialize()
        {
            _lastUpdate = ServerContext.ServerClock.ElapsedMilliseconds;
            LunaLog.Info("ServerUI initialized - Press Ctrl+C to shutdown");
        }

        public static void Update()
        {
            var now = ServerContext.ServerClock.ElapsedMilliseconds;
            if (now - _lastUpdate < 1000) return;
            _lastUpdate = now;

            lock (_lock)
            {
                _frameCount++;
                _playerCount = ServerContext.PlayerCount;
                _vesselCount = VesselStoreSystem.CurrentVessels.Count;
                _connectionsCount = ServerContext.Clients.Count;
                _avgTickLatency = ServerTickSystem.InstanceProperty.AverageTickLatencyMs;
                _tickOverruns = (int)ServerTickSystem.InstanceProperty.TickOverruns;
                _bytesSentSec = GetBytesSentPerSec();
                _bytesRecvSec = GetBytesRecvPerSec();
            }
        }

        public static string GetStatusLine()
        {
            lock (_lock)
            {
                var latency = _avgTickLatency > 10 ? $"!{_avgTickLatency:F0}ms" : $"{_avgTickLatency:F1}ms";
                return $"[{DateTime.Now:HH:mm:ss}] P:{_playerCount} V:{_vesselCount} | " +
                       $"Ticks:{_tickOverruns}/{latency} | " +
                       $"Net:{_bytesSentSec/1024}KB/s {_bytesRecvSec/1024}KB/s";
            }
        }

        public static void DrawBox()
        {
            Console.Clear();
            var lines = new[]
            {
                "╔═══════════════════════════════════════════════════════════════╗",
                "║           Luna Multiplayer Server - Running                   ║",
                "╠═══════════════════════════════════════════════════════════════╣",
                "║                                                       ║",
                "║  Players:        [0]         Uptime: [0h 0m]           ║",
                "║  Vessels:        [0]         Connections: [0]              ║",
                "║                                                       ║",
                "╠═══════════════════════════════════════════════════════════════╣",
                "║  Tick Rate:     0 Hz         Latency: 0 ms              ║",
                "║  Bandwidth:     0 KB/s       0 KB/s                    ║",
                "║                                                       ║",
                "╠═══════════════════════════════════════════════════════════════╣",
                "║  Press Ctrl+C to shutdown gracefully                      ║",
                "╚═══════════════════════════════════════════════════════════════╝"
            };
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public static void UpdateDashboard()
        {
            lock (_lock)
            {
                Console.CursorTop = 3;
                Console.CursorLeft = 1;
                Console.Write(new string(' ', 60));

                var playerStr = _playerCount.ToString();
                Console.CursorTop = 3;
                Console.CursorLeft = 10;
                Console.Write(playerStr);

                var vesselStr = _vesselCount.ToString();
                Console.CursorTop = 5;
                Console.CursorLeft = 10;
                Console.Write(vesselStr);

                var connStr = _connectionsCount.ToString();
                Console.CursorTop = 5;
                Console.CursorLeft = 50;
                Console.Write(connStr);
            }
        }

        private static long GetBytesSentPerSec()
        {
            long total = 0;
            foreach (var client in ServerContext.Clients.Values)
            {
                total += client.BytesSent;
            }
            return total / 1;
        }

        private static long GetBytesRecvPerSec()
        {
            long total = 0;
            foreach (var client in ServerContext.Clients.Values)
            {
                total += client.BytesReceived;
            }
            return total / 1;
        }
    }
}