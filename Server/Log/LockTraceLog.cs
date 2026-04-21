using LmpCommon.Locks;
using Server.Context;
using Server.System;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server.Log
{
    /// <summary>
    /// Dedicated lock-event tracer.  Every acquire/release/fail/steal is written to
    /// logs/LockTrace_&lt;timestamp&gt;.log with full context:
    ///   - Timestamp (UTC ms precision)
    ///   - Event type  (ACQUIRE | RELEASE | FAIL_ACQUIRE | FAIL_RELEASE | STEAL | PROXIMITY_ENTER | PROXIMITY_EXIT)
    ///   - Player name
    ///   - Lock type + vessel ID (or kerbal name)
    ///   - Vessel snapshot: name, sit, landedAt, lat/lon/alt (from the store if available)
    ///   - Full lock table for that vessel at the moment of the event
    ///   - Total connected/online players
    /// </summary>
    public static class LockTraceLog
    {
        // ── File ─────────────────────────────────────────────────────────────
        private static readonly string TraceFile = Path.Combine(
            LunaLog.LogFolder,
            $"LockTrace_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.log");

        private static readonly object _fileLock = new object();

        // ── Column widths ─────────────────────────────────────────────────────
        private const int W_TS     = 17;
        private const int W_EVT    = 16;
        private const int W_PLAYER = 18;
        private const int W_TYPE   = 14;
        private const int W_VESSEL = 38;

        // ── Initialiser ───────────────────────────────────────────────────────
        static LockTraceLog()
        {
            WriteHeader();
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>A lock was successfully granted.</summary>
        public static void Acquire(string player, LockDefinition lockDef, bool wasStolen)
            => Write(wasStolen ? "STEAL" : "ACQUIRE", player, lockDef);

        /// <summary>A lock acquire request was denied.</summary>
        public static void FailAcquire(string player, LockDefinition lockDef, string currentHolder)
            => Write("FAIL_ACQUIRE", player, lockDef, extra: $"holder={currentHolder}");

        /// <summary>A lock was successfully released.</summary>
        public static void Release(string player, LockDefinition lockDef)
            => Write("RELEASE", player, lockDef);

        /// <summary>A lock release request failed (player did not own it).</summary>
        public static void FailRelease(string player, LockDefinition lockDef, string currentHolder)
            => Write("FAIL_RELEASE", player, lockDef, extra: $"holder={currentHolder}");

        /// <summary>
        /// A player's vessel has entered physics loading range of another vessel.
        /// eventType = PROXIMITY_ENTER or PROXIMITY_EXIT
        /// </summary>
        public static void ProximityChange(string eventType, string movingPlayer, Guid movingVessel,
                                           string nearPlayer, Guid nearVessel, float distanceMetres)
        {
            var sb = new StringBuilder();
            AppendTimestamp(sb);
            AppendCol(sb, eventType,     W_EVT);
            AppendCol(sb, movingPlayer,  W_PLAYER);
            AppendCol(sb, "---",         W_TYPE);
            AppendCol(sb, movingVessel.ToString()[..8] + "→" + nearVessel.ToString()[..8], W_VESSEL);

            sb.Append($" dist={distanceMetres:F0}m  near={nearPlayer}");

            // Vessel snapshot for the NEAR vessel (this is what gets loaded into physics)
            AppendVesselSnapshot(sb, nearVessel, "  near-vessel");

            // Current lock holders for both vessels
            AppendVesselLocks(sb, movingVessel, "  moving");
            AppendVesselLocks(sb, nearVessel,   "  near");

            AppendPlayerCount(sb);
            AppendLine(sb.ToString());
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static void Write(string eventType, string player, LockDefinition lockDef, string extra = null)
        {
            var sb = new StringBuilder();
            AppendTimestamp(sb);
            AppendCol(sb, eventType,               W_EVT);
            AppendCol(sb, player,                  W_PLAYER);
            AppendCol(sb, lockDef.Type.ToString(), W_TYPE);

            // Vessel or kerbal identifier
            var id = lockDef.VesselId != Guid.Empty
                ? lockDef.VesselId.ToString()
                : !string.IsNullOrEmpty(lockDef.KerbalName)
                    ? $"kerbal:{lockDef.KerbalName}"
                    : "(global)";
            AppendCol(sb, id, W_VESSEL);

            if (!string.IsNullOrEmpty(extra))
                sb.Append($" [{extra}]");

            // Vessel context
            if (lockDef.VesselId != Guid.Empty)
            {
                AppendVesselSnapshot(sb, lockDef.VesselId, "  vessel");
                AppendVesselLocks(sb, lockDef.VesselId, "  locks");
            }

            AppendPlayerCount(sb);
            AppendLine(sb.ToString());
        }

        /// <summary>Appends name/sit/landedAt/lat/lon/alt for the vessel from the live store.</summary>
        private static void AppendVesselSnapshot(StringBuilder sb, Guid vesselId, string prefix)
        {
            if (!VesselStoreSystem.CurrentVessels.TryGetValue(vesselId, out var v))
            {
                sb.Append($"{prefix}=(not in store)");
                return;
            }

            var name     = v.Fields.GetSingle("name")?.Value     ?? "?";
            var sit      = v.Fields.GetSingle("sit")?.Value      ?? "?";
            var landedAt = v.Fields.GetSingle("landedAt")?.Value ?? "";
            var lat      = v.Fields.GetSingle("lat")?.Value      ?? "?";
            var lon      = v.Fields.GetSingle("lon")?.Value      ?? "?";
            var alt      = v.Fields.GetSingle("alt")?.Value      ?? "?";
            var type     = v.Fields.GetSingle("type")?.Value     ?? "?";

            sb.Append($"{prefix}=[name=\"{Truncate(name,24)}\" type={type} sit={sit}");
            if (!string.IsNullOrWhiteSpace(landedAt)) sb.Append($" landedAt={landedAt}");
            if (sit != "ORBITING")
                sb.Append($" lat={TrimCoord(lat)} lon={TrimCoord(lon)} alt={TrimCoord(alt)}m");
            sb.Append(']');
        }

        /// <summary>Appends the full lock table for a vessel (Control/Update/UnloadedUpdate).</summary>
        private static void AppendVesselLocks(StringBuilder sb, Guid vesselId, string prefix)
        {
            if (vesselId == Guid.Empty) return;

            var ctrl   = LockSystem.LockQuery.GetControlLock(vesselId);
            var upd    = LockSystem.LockQuery.GetUpdateLock(vesselId);
            var unload = LockSystem.LockQuery.GetUnloadedUpdateLock(vesselId);

            sb.Append($"{prefix}=[ctrl={ctrl?.PlayerName ?? "none"}" +
                      $" upd={upd?.PlayerName ?? "none"}" +
                      $" unload={unload?.PlayerName ?? "none"}]");
        }

        private static void AppendPlayerCount(StringBuilder sb)
        {
            var online    = ServerContext.Clients.Values.Count(c => c.Authenticated);
            var connected = ServerContext.Clients.Count;
            sb.Append($"  players={online}/{connected}");
        }

        private static void AppendTimestamp(StringBuilder sb)
            => AppendCol(sb, $"[{DateTime.UtcNow:HH:mm:ss.fff}]", W_TS);

        private static void AppendCol(StringBuilder sb, string value, int width)
        {
            sb.Append((value ?? "").PadRight(width));
            sb.Append(' ');
        }

        private static string Truncate(string s, int max)
            => s.Length <= max ? s : s[..max] + "…";

        private static string TrimCoord(string s)
        {
            if (double.TryParse(s, global::System.Globalization.NumberStyles.Float,
                    global::System.Globalization.CultureInfo.InvariantCulture, out var d))
                return d.ToString("F2");
            return s;
        }

        private static void WriteHeader()
        {
            var header = new StringBuilder();
            header.AppendLine("═══════════════════════════════════════════════════════════════════════════════════════════════");
            header.AppendLine($"  LMP Lock Trace  —  started {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            header.AppendLine("═══════════════════════════════════════════════════════════════════════════════════════════════");
            header.AppendLine();
            header.AppendLine("  Events:");
            header.AppendLine("    ACQUIRE        Lock granted to requesting player");
            header.AppendLine("    STEAL          Lock forcibly transferred (prior holder loses it)");
            header.AppendLine("    FAIL_ACQUIRE   Lock request denied (another player holds it)");
            header.AppendLine("    RELEASE        Lock cleanly released");
            header.AppendLine("    FAIL_RELEASE   Release failed (player did not own the lock)");
            header.AppendLine("    PROXIMITY_ENTER  Vessel entered physics range of another vessel");
            header.AppendLine("    PROXIMITY_EXIT   Vessel left physics range of another vessel");
            header.AppendLine();
            header.Append("  Columns: ".PadRight(W_TS + 1));
            header.Append("TIMESTAMP".PadRight(W_TS + 1));
            header.Append("EVENT".PadRight(W_EVT + 1));
            header.Append("PLAYER".PadRight(W_PLAYER + 1));
            header.Append("LOCK_TYPE".PadRight(W_TYPE + 1));
            header.AppendLine("VESSEL_ID  [context...]");
            header.AppendLine();
            AppendLine(header.ToString());
        }

        private static void AppendLine(string line)
        {
            lock (_fileLock)
            {
                try { File.AppendAllText(TraceFile, line + Environment.NewLine, Encoding.UTF8); }
                catch { /* non-fatal — never crash the server for a trace file */ }
            }
        }
    }
}
