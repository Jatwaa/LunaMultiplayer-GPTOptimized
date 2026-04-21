using LmpClient.Base;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LmpClient.Systems.LagReportSys
{
    /// <summary>
    /// Monitors Unity frame times every update and writes a diagnostic snapshot
    /// to LagReport.log (KSP root folder) when:
    ///   • A single frame takes longer than 1/5 s  (below 5 FPS — severe spike), or
    ///   • FPS stays below <see cref="SustainedFpsThreshold"/> for more than
    ///     <see cref="SustainedDurationSec"/> seconds.
    ///
    /// Reports are throttled to at most one every <see cref="CooldownSec"/> seconds.
    /// The system is always enabled — it runs regardless of connection state so
    /// lag that occurs before or after a session is also captured.
    /// </summary>
    public class LagReportSystem : System<LagReportSystem>
    {
        public override string SystemName { get; } = nameof(LagReportSystem);
        protected override bool AlwaysEnabled { get; } = true;

        // ── Config ───────────────────────────────────────────────────────────
        /// <summary>Sustained FPS below this triggers a report after <see cref="SustainedDurationSec"/>.</summary>
        private const float SustainedFpsThreshold = 20f;
        /// <summary>A single frame below this FPS triggers an immediate report.</summary>
        private const float InstantFpsThreshold = 5f;
        /// <summary>Seconds of sustained low-FPS before a report fires.</summary>
        private const float SustainedDurationSec = 1f;
        /// <summary>Minimum seconds between consecutive reports.</summary>
        private const float CooldownSec = 15f;
        /// <summary>Rolling frame-time buffer size (frames).</summary>
        private const int BufferSize = 120;

        // ── State ────────────────────────────────────────────────────────────
        private readonly float[] _frameTimes = new float[BufferSize];
        private int _frameIdx;
        private int _frameFilled;

        private float _lagDurationSec;
        private float _lastReportTime = float.MinValue;

        private static readonly string LagReportPath =
            Path.Combine(KSPUtil.ApplicationRootPath, "LagReport.log");

        // ── System lifecycle ─────────────────────────────────────────────────
        protected override void OnEnabled()
        {
            base.OnEnabled();
            // IntervalInMs = 0 → runs every Update frame
            SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, CheckFrameTime));
        }

        // ── Per-frame check ──────────────────────────────────────────────────
        private void CheckFrameTime()
        {
            var dt  = Time.unscaledDeltaTime;
            var fps = dt > 0f ? 1f / dt : 9999f;

            // Record into rolling buffer
            _frameTimes[_frameIdx] = dt;
            _frameIdx = (_frameIdx + 1) % BufferSize;
            if (_frameFilled < BufferSize) _frameFilled++;

            // Immediate severe spike
            if (fps < InstantFpsThreshold)
            {
                TryWriteReport(
                    $"Frame spike — {fps:F1} FPS  ({dt * 1000f:F0} ms frame time)");
                _lagDurationSec = 0f;
                return;
            }

            // Sustained lag accumulator
            if (fps < SustainedFpsThreshold)
            {
                _lagDurationSec += dt;
                if (_lagDurationSec >= SustainedDurationSec)
                {
                    TryWriteReport(
                        $"Sustained lag — {GetAvgFps():F1} FPS avg over {_lagDurationSec:F1}s " +
                        $"(threshold: {SustainedFpsThreshold} FPS)");
                    _lagDurationSec = 0f;
                }
            }
            else
            {
                _lagDurationSec = 0f;
            }
        }

        // ── Report writer ─────────────────────────────────────────────────────
        private void TryWriteReport(string trigger)
        {
            var now = Time.unscaledTime;
            if (now - _lastReportTime < CooldownSec) return;
            _lastReportTime = now;

            // Snapshot main-thread state immediately, then write on a background thread
            var snapshot = BuildSnapshot(trigger);
            Task.Run(() =>
            {
                try { File.AppendAllText(LagReportPath, snapshot); }
                catch { /* ignore write errors — never crash the game */ }
            });

            LunaLog.LogWarning($"[LMP LagReport] {trigger} — snapshot written to LagReport.log");
        }

        private string BuildSnapshot(string trigger)
        {
            var now = DateTime.UtcNow;
            const string Sep = "══════════════════════════════════════════════════════════════════════════";
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"╔{Sep}╗");
            sb.AppendLine($"║  CLIENT LAG REPORT   [{now:yyyy-MM-dd HH:mm:ss.fff} UTC]");
            sb.AppendLine($"║  Trigger             : {trigger}");
            sb.AppendLine($"╠{Sep}╣");

            // Scene & client state
            sb.AppendLine($"║  Scene          : {HighLogic.LoadedScene}");
            sb.AppendLine($"║  Network state  : {MainSystem.NetworkState}");
            sb.AppendLine($"║  LMP status     : {MainSystem.Singleton?.Status ?? "—"}");
            sb.AppendLine($"║  Memory (GC)    : {GC.GetTotalMemory(false) / 1024 / 1024} MB");

            // Rolling frame-time statistics
            if (_frameFilled > 0)
            {
                // Copy the filled portion of the circular buffer in chronological order
                var count = _frameFilled;
                var copy  = new float[count];
                for (var i = 0; i < count; i++)
                    copy[i] = _frameTimes[(_frameIdx - count + i + BufferSize) % BufferSize];

                Array.Sort(copy); // ascending by frame-time (slowest = last)
                var avgMs  = Average(copy) * 1000f;
                var worstMs = copy[count - 1] * 1000f;
                var p5Ms   = copy[Math.Max(0, (int)(count * 0.95f))] * 1000f; // worst 5%
                var minFps = worstMs > 0f ? 1000f / worstMs : 0f;
                var maxFps = copy[0] > 0f ? 1f / copy[0] : 0f;

                sb.AppendLine($"╠{Sep}╣");
                sb.AppendLine($"║  Frame times (last {count} frames):");
                sb.AppendLine($"║    avg={avgMs,6:F1} ms   worst 5%={p5Ms,6:F1} ms   " +
                              $"min FPS={minFps,5:F1}   max FPS={maxFps,5:F1}");
            }

            // Vessel info — only meaningful in flight/tracking scenes
            var scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT || scene == GameScenes.TRACKSTATION)
            {
                try
                {
                    var vessels = FlightGlobals.Vessels;
                    var count   = vessels?.Count ?? 0;
                    var loaded  = 0;
                    if (vessels != null)
                        foreach (var v in vessels)
                            if (v != null && v.loaded) loaded++;

                    sb.AppendLine($"╠{Sep}╣");
                    sb.AppendLine($"║  FlightGlobals vessels  : {count} total  /  {loaded} loaded in physics");

                    if (vessels != null && count > 0 && count <= 30)
                    {
                        sb.AppendLine("║  Vessel list:");
                        foreach (var v in vessels)
                            if (v != null)
                                sb.AppendLine($"║    [{(v.loaded ? "LOAD" : "pack")}] {v.id}  {v.vesselName}");
                    }
                }
                catch { /* may fail mid-scene-switch */ }
            }

            sb.AppendLine($"╚{Sep}╝");
            return sb.ToString();
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private float GetAvgFps()
        {
            if (_frameFilled == 0) return 0f;
            var sum = 0f;
            for (var i = 0; i < _frameFilled; i++) sum += _frameTimes[i];
            var avgDt = sum / _frameFilled;
            return avgDt > 0f ? 1f / avgDt : 0f;
        }

        private static float Average(float[] arr)
        {
            var s = 0f;
            foreach (var v in arr) s += v;
            return s / arr.Length;
        }
    }
}
