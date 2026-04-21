using Server.Client;
using Server.Context;
using Server.System;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Log
{
    /// <summary>
    /// Detects server-side lag spikes and appends detailed diagnostic snapshots
    /// to logs/LagReport.log. Reports are throttled to at most one every
    /// <see cref="CooldownSeconds"/> seconds so the file never floods.
    /// </summary>
    public static class LagReporter
    {
        // ── Thresholds (tunable) ─────────────────────────────────────────────
        /// <summary>Single message processing time (ms) that triggers a report.</summary>
        public const double MessageThresholdMs = 500;
        /// <summary>Vessel sync duration (ms) that triggers a report.</summary>
        public const double VesselSyncThresholdMs = 300;
        /// <summary>GC heap growth (MB in one monitor cycle) that triggers a report.</summary>
        private const long MemorySpikeMb = 200;
        /// <summary>Minimum seconds between consecutive reports.</summary>
        private const int CooldownSeconds = 10;
        /// <summary>How often the background monitor polls (ms).</summary>
        private const int MonitorIntervalMs = 5000;

        // ── State ────────────────────────────────────────────────────────────
        private static readonly string LagReportPath =
            Path.Combine(LunaLog.LogFolder, "LagReport.log");

        /// <summary>UTC ticks of the last written report (Interlocked-guarded).</summary>
        private static long _lastReportTicks;

        /// <summary>Rolling window of recent message-processing durations (ms).</summary>
        private static readonly ConcurrentQueue<double> RecentMsgMs =
            new ConcurrentQueue<double>();
        private const int MaxSamples = 200;

        // ── Public recording API ─────────────────────────────────────────────

        /// <summary>
        /// Called after processing each client message.
        /// Records timing in the rolling window and triggers a report when slow.
        /// </summary>
        public static void RecordMessageTime(double elapsedMs, string msgType, string playerName)
        {
            Enqueue(elapsedMs);
            if (elapsedMs >= MessageThresholdMs)
                WriteReport($"Slow message — {msgType} from '{playerName}' took {elapsedMs:F1} ms");
        }

        /// <summary>
        /// Called after each vessel-sync pass.
        /// Triggers a report when the sync took longer than <see cref="VesselSyncThresholdMs"/>.
        /// </summary>
        public static void RecordVesselSync(double elapsedMs, int sentCount, int totalVessels, string playerName)
        {
            if (elapsedMs >= VesselSyncThresholdMs && sentCount > 0)
                WriteReport($"Slow vessel sync — sent {sentCount}/{totalVessels} vessels to '{playerName}' in {elapsedMs:F1} ms");
        }

        // ── Background monitor ───────────────────────────────────────────────

        /// <summary>
        /// Long-running task that periodically samples overall server health and
        /// triggers a report when memory spikes abnormally.
        /// Start this from <see cref="MainServer"/> alongside the other tasks.
        /// </summary>
        public static async Task RunMonitorAsync(CancellationToken token)
        {
            var highWaterMb = 0L;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(MonitorIntervalMs, token).ConfigureAwait(false);

                    var memMb = GC.GetTotalMemory(false) / 1024 / 1024;
                    if (memMb - highWaterMb > MemorySpikeMb)
                        WriteReport($"Memory spike — GC heap grew to {memMb} MB (+{memMb - highWaterMb} MB since last check)");

                    if (memMb > highWaterMb)
                        highWaterMb = memMb;
                }
                catch (OperationCanceledException) { break; }
                catch { /* never let the monitor crash */ }
            }
        }

        // ── Core writer ──────────────────────────────────────────────────────

        /// <summary>
        /// Builds a full diagnostic snapshot and appends it to LagReport.log.
        /// Respects the cooldown — concurrent callers that arrive within the
        /// cooldown window are silently dropped.
        /// </summary>
        public static void WriteReport(string trigger)
        {
            var now = DateTime.UtcNow;
            var nowTicks = now.Ticks;

            // Atomic cooldown gate
            var last = Interlocked.Read(ref _lastReportTicks);
            if (nowTicks - last < TimeSpan.FromSeconds(CooldownSeconds).Ticks) return;
            if (Interlocked.CompareExchange(ref _lastReportTicks, nowTicks, last) != last) return;

            try
            {
                const string Sep = "══════════════════════════════════════════════════════════════════════════";
                var sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine($"╔{Sep}╗");
                sb.AppendLine($"║  SERVER LAG REPORT   [{now:yyyy-MM-dd HH:mm:ss.fff} UTC]");
                sb.AppendLine($"║  Trigger             : {trigger}");
                sb.AppendLine($"╠{Sep}╣");

                // Server vitals
                var uptime = TimeSpan.FromMilliseconds(ServerContext.ServerClock.ElapsedMilliseconds);
                sb.AppendLine($"║  Uptime       : {(int)uptime.TotalHours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}");
                sb.AppendLine($"║  Memory (GC)  : {GC.GetTotalMemory(false) / 1024 / 1024} MB");
                sb.AppendLine($"║  Players      : {ServerContext.PlayerCount} authenticated  /  {ServerContext.Clients.Count} connected");
                sb.AppendLine($"║  Vessels      : {VesselStoreSystem.CurrentVessels.Count} stored");

                // Per-player detail
                var clients = ServerContext.Clients.Values.ToArray();
                if (clients.Length > 0)
                {
                    sb.AppendLine($"╠{Sep}╣");
                    sb.AppendLine("║  Players:");
                    foreach (var c in clients)
                    {
                        var pingMs = c.Connection != null
                            ? c.Connection.AverageRoundtripTime * 1000d
                            : -1d;
                        sb.AppendLine(
                            $"║    {c.PlayerName,-22} " +
                            $"auth={c.Authenticated,-5}  " +
                            $"subspace={c.Subspace,-4}  " +
                            $"ping={pingMs,6:F0} ms");
                    }
                }

                // Message timing statistics
                var samples = RecentMsgMs.ToArray();
                if (samples.Length > 0)
                {
                    Array.Sort(samples);
                    var avg = samples.Average();
                    var max = samples[samples.Length - 1];
                    var p95 = samples[Math.Max(0, (int)(samples.Length * 0.95) - 1)];
                    sb.AppendLine($"╠{Sep}╣");
                    sb.AppendLine($"║  Message times (last {samples.Length} msgs):");
                    sb.AppendLine($"║    avg={avg,7:F1} ms   p95={p95,7:F1} ms   max={max,7:F1} ms");
                }

                // Vessel list (capped)
                var vessels = VesselStoreSystem.CurrentVessels.ToArray();
                if (vessels.Length > 0)
                {
                    sb.AppendLine($"╠{Sep}╣");
                    sb.AppendLine("║  Stored vessel IDs:");
                    foreach (var v in vessels.Take(30))
                        sb.AppendLine($"║    {v.Key}");
                    if (vessels.Length > 30)
                        sb.AppendLine($"║    … and {vessels.Length - 30} more");
                }

                sb.AppendLine($"╚{Sep}╝");

                File.AppendAllText(LagReportPath, sb.ToString());
                LunaLog.Warning($"[LagReport] {trigger} — full snapshot written to {LagReportPath}");
            }
            catch
            {
                // Never let the reporter crash the server
            }
        }

        // ── Helper ───────────────────────────────────────────────────────────

        private static void Enqueue(double ms)
        {
            RecentMsgMs.Enqueue(ms);
            while (RecentMsgMs.Count > MaxSamples)
                RecentMsgMs.TryDequeue(out _);
        }
    }
}
