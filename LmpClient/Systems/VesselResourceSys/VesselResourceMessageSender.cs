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
using System.Collections.Generic;

namespace LmpClient.Systems.VesselResourceSys
{
    public class VesselResourceMessageSender : SubSystem<VesselResourceSystem>, IMessageSender
    {
        // ── Resource delta suppression ────────────────────────────────────────
        // Resources are polled every 2.5 s.  When a vessel has no active resource
        // consumption or generation — engines off, electric motors idle, solar
        // panels in shadow, reaction wheels saturated, etc. — the snapshot is
        // bitwise-identical every tick.  Without this guard we were sending a full
        // resource dump (all parts × all resources) 24 times a minute for no reason.
        //
        // The check is resource-agnostic: it fires for ANY consumer — rocket
        // engines, electric motors, wheels, reaction wheels, fuel cells, drills,
        // solar panels, lights, science instruments.  If the amount changed by more
        // than ResourceEpsilon OR a flow-state toggle occurred, the message is sent.
        //
        // Epsilon is set to 0.005 units — safely below any real consumption rate
        // observable over 2.5 s (electric wheel drain ≈ 5–25 EC/s → 12.5–62.5 EC
        // per interval; solar trickle ≈ 0.3 EC/s → 0.75 EC per interval; both are
        // orders of magnitude above the threshold).
        //
        // A heartbeat every ForceSendIntervalMs ensures late-joining clients still
        // receive the current state even when nothing has changed.

        private const double ResourceEpsilon     = 0.005;   // ~0.005 units — safely below any real drain
        private const int    ForceSendIntervalMs = 30_000;  // heartbeat every 30 s

        // Per-vessel last-sent snapshot: (partFlightId, resourceName) → (amount, flowState)
        // Tuple field names are intentionally omitted here — net472 doesn't preserve them
        // through generic dictionaries; access via Item1/Item2 in SnapshotUnchanged.
        private static readonly ConcurrentDictionary<Guid, Dictionary<(uint, string), (double, bool)>> _lastSnapshot
            = new ConcurrentDictionary<Guid, Dictionary<(uint, string), (double, bool)>>();

        // Per-vessel last forced-send time for heartbeat
        private static readonly ConcurrentDictionary<Guid, DateTime> _lastForceSent
            = new ConcurrentDictionary<Guid, DateTime>();

        // Called by VesselCommon.RemoveVesselFromSystems — clears stale snapshots.
        public static void RemoveVessel(Guid vesselId)
        {
            _lastSnapshot.TryRemove(vesselId, out _);
            _lastForceSent.TryRemove(vesselId, out _);
        }

        // ─────────────────────────────────────────────────────────────────────

        private static readonly List<VesselResourceInfo> Resources = new List<VesselResourceInfo>();

        public void SendMessage(IMessageData msg)
        {
            NetworkSender.QueueOutgoingMessage(MessageFactory.CreateNew<VesselCliMsg>(msg));
        }

        public void SendVesselResources(Vessel vessel)
        {
            var vesselId = vessel.id;
            var resourceCount = 0;

            // Build the current snapshot while also populating Resources list.
            var currentSnap = new Dictionary<(uint, string), (double, bool)>();

            for (var i = 0; i < vessel.protoVessel.protoPartSnapshots.Count; i++)
            {
                if (vessel.protoVessel.protoPartSnapshots[i]?.resources == null) continue;

                uint partId = vessel.protoVessel.protoPartSnapshots[i].flightID;

                for (var j = 0; j < vessel.protoVessel.protoPartSnapshots[i].resources.Count; j++)
                {
                    var resource = vessel.protoVessel.protoPartSnapshots[i].resources[j]?.resourceRef;
                    if (resource == null) continue;

                    currentSnap[(partId, resource.resourceName)] = (resource.amount, resource.flowState);

                    if (Resources.Count > resourceCount)
                    {
                        Resources[resourceCount].ResourceName = resource.resourceName;
                        Resources[resourceCount].PartFlightId = partId;
                        Resources[resourceCount].Amount       = resource.amount;
                        Resources[resourceCount].FlowState    = resource.flowState;
                    }
                    else
                    {
                        Resources.Add(new VesselResourceInfo
                        {
                            ResourceName = resource.resourceName,
                            PartFlightId = partId,
                            Amount       = resource.amount,
                            FlowState    = resource.flowState,
                        });
                    }

                    resourceCount++;
                }
            }

            // ── Delta check ─────────────────────────────────────────────────
            bool forceHeartbeat = !_lastForceSent.TryGetValue(vesselId, out var lastSent)
                                  || (LunaComputerTime.UtcNow - lastSent).TotalMilliseconds >= ForceSendIntervalMs;

            if (!forceHeartbeat && SnapshotUnchanged(vesselId, currentSnap))
                return; // Nothing changed; skip this send entirely.

            // Persist new snapshot and update heartbeat time.
            _lastSnapshot[vesselId] = currentSnap;
            _lastForceSent[vesselId] = LunaComputerTime.UtcNow;

            // ── Build and send the message ──────────────────────────────────
            var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselResourceMsgData>();
            msgData.GameTime  = TimeSyncSystem.UniversalTime;
            msgData.VesselId  = vesselId;

            msgData.ResourcesCount = resourceCount;

            if (msgData.Resources.Length < resourceCount)
                msgData.Resources = new VesselResourceInfo[resourceCount];

            for (var i = 0; i < resourceCount; i++)
            {
                if (msgData.Resources[i] == null)
                    msgData.Resources[i] = new VesselResourceInfo(Resources[i]);
                else
                    msgData.Resources[i].CopyFrom(Resources[i]);
            }

            SendMessage(msgData);
        }

        // Returns true when every entry in the current snapshot matches the last one.
        private static bool SnapshotUnchanged(Guid vesselId,
            Dictionary<(uint, string), (double, bool)> current)
        {
            if (!_lastSnapshot.TryGetValue(vesselId, out var prev))
                return false; // First ever send for this vessel.

            if (prev.Count != current.Count)
                return false; // Part count changed (decoupling, jettison, …).

            foreach (var kv in current)
            {
                if (!prev.TryGetValue(kv.Key, out var prevVal))
                    return false;

                // Item1 = amount, Item2 = flowState
                if (Math.Abs(kv.Value.Item1 - prevVal.Item1) > ResourceEpsilon
                    || kv.Value.Item2 != prevVal.Item2)
                    return false;
            }

            return true;
        }
    }
}
