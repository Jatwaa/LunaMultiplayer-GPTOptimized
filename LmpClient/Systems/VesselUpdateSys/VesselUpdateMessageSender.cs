using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselUpdateSys
{
    public class VesselUpdateMessageSender : SubSystem<VesselUpdateSystem>, IMessageSender
    {
        // ── VesselUpdate delta suppression ───────────────────────────────────
        // VesselUpdate messages carry slowly-changing metadata: name, type,
        // situation, body, stage, landed/splashed flags, etc.  These are sent
        // on a routine timer regardless of whether anything changed.  In a
        // stable flight the values are identical every interval — we skip the
        // send when the snapshot is unchanged and rely on a heartbeat so that
        // late-joining clients still receive the current state.
        //
        // Deliberately excluded from the delta check:
        //   • MissionTime / LastUt / DistanceTraveled  — always incrementing;
        //     comparing them would cancel the suppression in normal flight.
        //   • Com                                      — noisy float; not
        //     actionable by other clients in real time.

        private const int ForceSendIntervalMs = 30_000; // heartbeat every 30 s

        private struct UpdateSnapshot
        {
            public string Name, Type, Situation, LandedAt, DisplayLandedAt, BodyName, AutoCleanReason;
            public bool   Landed, Splashed, Persistent, AutoClean, WasControllable;
            public int    Stage;
            public float  LaunchTime;
        }

        // Per-vessel snapshots and heartbeat timestamps.
        private static readonly ConcurrentDictionary<Guid, UpdateSnapshot> _lastSnapshot
            = new ConcurrentDictionary<Guid, UpdateSnapshot>();
        private static readonly ConcurrentDictionary<Guid, DateTime> _lastForceSent
            = new ConcurrentDictionary<Guid, DateTime>();

        // Called by VesselCommon.RemoveVesselFromSystems.
        public static void RemoveVessel(Guid vesselId)
        {
            _lastSnapshot.TryRemove(vesselId, out _);
            _lastForceSent.TryRemove(vesselId, out _);
        }

        // ─────────────────────────────────────────────────────────────────────

        public void SendMessage(IMessageData msg)
        {
            NetworkSender.QueueOutgoingMessage(MessageFactory.CreateNew<VesselCliMsg>(msg));
        }

        public void SendVesselUpdate(Vessel vessel)
        {
            if (vessel == null) return;

            var vesselId = vessel.id;

            var current = new UpdateSnapshot
            {
                Name             = vessel.vesselName        ?? string.Empty,
                Type             = vessel.vesselType.ToString(),
                Situation        = vessel.situation.ToString(),
                Landed           = vessel.Landed,
                LandedAt         = vessel.landedAt          ?? string.Empty,
                DisplayLandedAt  = vessel.displaylandedAt   ?? string.Empty,
                Splashed         = vessel.Splashed,
                Persistent       = vessel.isPersistent,
                AutoClean        = vessel.AutoClean,
                AutoCleanReason  = vessel.AutoCleanReason   ?? string.Empty,
                WasControllable  = vessel.IsControllable,
                Stage            = vessel.currentStage,
                BodyName         = vessel.mainBody?.bodyName ?? string.Empty,
                LaunchTime       = (float)vessel.launchTime,
            };

            // ── Delta check ─────────────────────────────────────────────────
            bool forceHeartbeat = !_lastForceSent.TryGetValue(vesselId, out var lastSent)
                                  || (LunaComputerTime.UtcNow - lastSent).TotalMilliseconds >= ForceSendIntervalMs;

            if (!forceHeartbeat && SnapshotUnchanged(vesselId, current))
                return;

            _lastSnapshot[vesselId]  = current;
            _lastForceSent[vesselId] = LunaComputerTime.UtcNow;

            // ── Build and send ──────────────────────────────────────────────
            var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselUpdateMsgData>();
            msgData.GameTime           = TimeSyncSystem.UniversalTime;
            msgData.VesselId           = vesselId;
            msgData.Name               = vessel.vesselName;
            msgData.Type               = vessel.vesselType.ToString();
            msgData.DistanceTraveled   = vessel.distanceTraveled;

            msgData.Situation          = vessel.situation.ToString();
            msgData.Landed             = vessel.Landed;
            msgData.LandedAt           = vessel.landedAt;
            msgData.DisplayLandedAt    = vessel.displaylandedAt;
            msgData.Splashed           = vessel.Splashed;
            msgData.MissionTime        = vessel.missionTime;
            msgData.LaunchTime         = vessel.launchTime;
            msgData.LastUt             = vessel.lastUT;
            msgData.Persistent         = vessel.isPersistent;
            msgData.RefTransformId     = vessel.referenceTransformId;

            msgData.AutoClean          = vessel.AutoClean;
            msgData.AutoCleanReason    = vessel.AutoCleanReason;
            msgData.WasControllable    = vessel.IsControllable;
            msgData.Stage              = vessel.currentStage;
            msgData.Com[0]             = vessel.localCoM.x;
            msgData.Com[1]             = vessel.localCoM.y;
            msgData.Com[2]             = vessel.localCoM.z;

            msgData.BodyName           = vessel.mainBody != null ? vessel.mainBody.bodyName : string.Empty;

            SendMessage(msgData);
        }

        private static bool SnapshotUnchanged(Guid vesselId, UpdateSnapshot current)
        {
            if (!_lastSnapshot.TryGetValue(vesselId, out var prev))
                return false; // First send for this vessel.

            return current.Name            == prev.Name
                && current.Type            == prev.Type
                && current.Situation       == prev.Situation
                && current.Landed          == prev.Landed
                && current.LandedAt        == prev.LandedAt
                && current.DisplayLandedAt == prev.DisplayLandedAt
                && current.Splashed        == prev.Splashed
                && current.Persistent      == prev.Persistent
                && current.AutoClean       == prev.AutoClean
                && current.AutoCleanReason == prev.AutoCleanReason
                && current.WasControllable == prev.WasControllable
                && current.Stage           == prev.Stage
                && current.BodyName        == prev.BodyName
                && current.LaunchTime      == prev.LaunchTime;
        }
    }
}
