using ByteSizeLib;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Server;
using LmpCommon.Message.Types;
using Server.Client;
using Server.Context;
using Server.Log;
using Server.Message.Base;
using Server.Server;
using Server.System;
using Server.System.Vessel;
using Server.Web.Structures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server.Message
{
    public class VesselMsgReader : ReaderBase
    {
        // ── Proximity tracking ───────────────────────────────────────────────
        // Maps  movingVesselId  →  set of nearVesselIds currently in physics range.
        // When the set changes we emit PROXIMITY_ENTER / PROXIMITY_EXIT to LockTraceLog.
        private static readonly ConcurrentDictionary<Guid, HashSet<Guid>> _proximityState
            = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        // Lock per moving vessel to serialise proximity-set updates
        private static readonly ConcurrentDictionary<Guid, object> _proximityLocks
            = new ConcurrentDictionary<Guid, object>();

        // ── Proximity throttle ───────────────────────────────────────────────
        // Position messages arrive at up to 20 Hz per vessel.  Recomputing the
        // full proximity set on every message is wasteful — enter/exit events
        // only matter when a vessel actually crosses a cell boundary, which
        // happens at most a few times per minute.  We rate-limit the check to
        // once every ProximityCheckIntervalMs per vessel.
        private const int ProximityCheckIntervalMs = 500;
        private static readonly ConcurrentDictionary<Guid, DateTime> _lastProximityCheck
            = new ConcurrentDictionary<Guid, DateTime>();

        // ── Vessel proto version tracking for sync de-duplication ────────────
        // Clients send a 10-second heartbeat listing vessel GUIDs they know about,
        // but in practice the list is always empty (FlightGlobals isn't populated
        // during all scene states).  Without de-duplication the server blasts the
        // full proto of every stored vessel — including 100-160 KB behemoths — to
        // every client every 10 seconds.  KSP has to parse and load each one,
        // causing multi-second FPS drops.
        //
        // Fix: assign an incrementing version number to each vessel whenever its
        // owner pushes a new proto.  During sync, only send a vessel to a client
        // if the client hasn't already received the current version.  On disconnect,
        // clear that client's records so a reconnect triggers a fresh full sync.
        private static long _vesselVersionCounter;
        // vesselId → monotonically increasing version (bumped in HandleVesselProto only)
        private static readonly ConcurrentDictionary<Guid, long> _vesselProtoVersion
            = new ConcurrentDictionary<Guid, long>();
        // playerName → { vesselId → version last sent to this client }
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, long>> _clientReceivedVersion
            = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, long>>();

        /// <summary>
        /// Called from <see cref="ClientConnectionHandler.DisconnectClient"/> so that a
        /// reconnecting player always receives a fresh full vessel sync.
        /// </summary>
        public static void ClearClientSyncCache(string playerName)
        {
            _clientReceivedVersion.TryRemove(playerName, out _);
        }

        public override void HandleMessage(ClientStructure client, IClientMessageBase message)
        {
            var messageData = message.Data as VesselBaseMsgData;
            switch (messageData?.VesselMessageType)
            {
                case VesselMessageType.Sync:
                    HandleVesselsSync(client, messageData);
                    message.Recycle();
                    break;
                case VesselMessageType.Proto:
                    HandleVesselProto(client, messageData);
                    break;
                case VesselMessageType.Remove:
                    HandleVesselRemove(client, messageData);
                    break;
                case VesselMessageType.Position:
                    var posData = (VesselPositionMsgData)messageData;

                    // Track which vessel this client is actively piloting
                    client.OwnedVesselId = posData.VesselId;

                    // Update the SpatialGrid (lat/lon/alt as x/y/z)
                    var pos = new global::System.Numerics.Vector3(
                        (float)posData.LatLonAlt[0],
                        (float)posData.LatLonAlt[1],
                        (float)posData.LatLonAlt[2]);

                    SpatialGrid.InstanceProperty.UpdateVesselPosition(posData.VesselId, pos);

                    // Feed live telemetry to the web dashboard
                    {
                        var vx     = posData.VelocityVector[0];
                        var vy     = posData.VelocityVector[1];
                        var vz     = posData.VelocityVector[2];
                        var speed  = Math.Sqrt(vx * vx + vy * vy + vz * vz);
                        var body   = posData.BodyName ?? "Unknown";

                        // Best-effort vessel name/type from store (may not be loaded yet)
                        VesselStoreSystem.CurrentVessels.TryGetValue(posData.VesselId, out var stored);
                        var vName     = stored?.Fields.GetSingle("name")?.Value ?? "Unknown";
                        var vType     = stored?.Fields.GetSingle("type")?.Value ?? "Unknown";

                        TelemetryData.UpdatePlayer(
                            posData.VesselId,
                            client.PlayerName,
                            vName,
                            vType,
                            posData.LatLonAlt[0],   // latitude
                            posData.LatLonAlt[1],   // longitude
                            posData.LatLonAlt[2],   // altitude
                            body,
                            speed,
                            0,                      // heading — not in position message
                            posData.HeightFromTerrain);
                    }

                    // Relay to spatially interested clients
                    var interestedClients = SpatialGrid.InstanceProperty.GetInterestedClientsFullDetail(pos).ToList();

                    foreach (var targetClient in interestedClients)
                    {
                        if (targetClient != client)
                            MessageQueuer.SendToClient<VesselSrvMsg>(targetClient, messageData);
                    }

                    // Throttled proximity-change tracking — only recompute enter/exit
                    // events every ProximityCheckIntervalMs to avoid doing a full set-diff
                    // on every position message (up to 20 Hz per vessel).
                    var now = global::System.DateTime.UtcNow;
                    bool dueForProximityCheck =
                        !_lastProximityCheck.TryGetValue(posData.VesselId, out var lastCheck)
                        || (now - lastCheck).TotalMilliseconds >= ProximityCheckIntervalMs;

                    if (dueForProximityCheck)
                    {
                        _lastProximityCheck[posData.VesselId] = now;

                        var nowNear = new HashSet<Guid>(
                            interestedClients
                                .Where(c => c != client && c.OwnedVesselId != Guid.Empty)
                                .Select(c => c.OwnedVesselId));

                        TrackProximityChanges(client, posData.VesselId, nowNear, interestedClients);
                    }
                    break;
                case VesselMessageType.Flightstate:
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    VesselDataUpdater.WriteFlightstateDataToFile(messageData);
                    break;
                case VesselMessageType.Update:
                    VesselDataUpdater.WriteUpdateDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.Resource:
                    VesselDataUpdater.WriteResourceDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.PartSyncField:
                    VesselDataUpdater.WritePartSyncFieldDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.PartSyncUiField:
                    VesselDataUpdater.WritePartSyncUiFieldDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.PartSyncCall:
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.ActionGroup:
                    VesselDataUpdater.WriteActionGroupDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.Fairing:
                    VesselDataUpdater.WriteFairingDataToFile(messageData);
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.Decouple:
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                case VesselMessageType.Couple:
                    HandleVesselCouple(client, messageData);
                    break;
                case VesselMessageType.Undock:
                    MessageQueuer.RelayMessage<VesselSrvMsg>(client, messageData);
                    break;
                default:
                    throw new NotImplementedException("Vessel message type not implemented");
            }
        }

        private static void HandleVesselRemove(ClientStructure client, VesselBaseMsgData message)
        {
            var data = (VesselRemoveMsgData)message;

            if (LockSystem.LockQuery.ControlLockExists(data.VesselId) && !LockSystem.LockQuery.ControlLockBelongsToPlayer(data.VesselId, client.PlayerName))
                return;

            if (VesselStoreSystem.VesselExists(data.VesselId))
            {
                LunaLog.Debug($"Removing vessel {data.VesselId} from {client.PlayerName}");
                VesselStoreSystem.RemoveVessel(data.VesselId);
            }

            if (data.AddToKillList)
                VesselContext.RemovedVessels.TryAdd(data.VesselId, 0);

            //Relay the message.
            MessageQueuer.RelayMessage<VesselSrvMsg>(client, data);
        }

        private static void HandleVesselProto(ClientStructure client, VesselBaseMsgData message)
        {
            var msgData = (VesselProtoMsgData)message;

            if (VesselContext.RemovedVessels.ContainsKey(msgData.VesselId)) return;

            if (msgData.NumBytes == 0)
            {
                LunaLog.Warning($"Received a vessel with 0 bytes ({msgData.VesselId}) from {client.PlayerName}.");
                return;
            }

            var vesselText = Encoding.UTF8.GetString(msgData.Data, 0, msgData.NumBytes);

            // Reject vessels whose name is an unresolved KSP localization key (e.g. #autoLOC_8005483).
            // These are saved in an incomplete state (mid-editor, broken spawn) and cause
            // FlightIntegrator / Part.Update / ModuleCommand NullReferenceExceptions on every
            // client that loads them.
            if (!IsVesselNameValid(vesselText, msgData.VesselId, client.PlayerName))
                return;

            if (!VesselStoreSystem.VesselExists(msgData.VesselId))
            {
                LunaLog.Debug($"Saving vessel {msgData.VesselId} ({ByteSize.FromBytes(msgData.NumBytes).KiloBytes} KB) from {client.PlayerName}.");
            }

            VesselDataUpdater.RawConfigNodeInsertOrUpdate(msgData.VesselId, vesselText);
            _vesselProtoVersion[msgData.VesselId] = Interlocked.Increment(ref _vesselVersionCounter);
            MessageQueuer.RelayMessage<VesselSrvMsg>(client, msgData);
        }

        /// <summary>
        /// Returns false (and logs a warning) when the vessel text has a missing, empty, or
        /// unresolved localization-key name.  Avoids storing junk vessels that crash KSP's
        /// FlightIntegrator and Part update loops on every connecting client.
        /// </summary>
        private static bool IsVesselNameValid(string vesselText, Guid vesselId, string playerName)
        {
            const string namePrefix = "name = ";
            var nameStart = vesselText.IndexOf(namePrefix, StringComparison.Ordinal);
            if (nameStart < 0)
            {
                LunaLog.Warning($"Rejecting vessel {vesselId} from {playerName}: no name field found.");
                return false;
            }

            nameStart += namePrefix.Length;
            var nameEnd = vesselText.IndexOf('\n', nameStart);
            var vesselName = (nameEnd >= 0
                ? vesselText.Substring(nameStart, nameEnd - nameStart)
                : vesselText.Substring(nameStart)).Trim();

            if (string.IsNullOrEmpty(vesselName))
            {
                LunaLog.Warning($"Rejecting vessel {vesselId} from {playerName}: empty name.");
                return false;
            }

            if (vesselName.StartsWith("#autoLOC_", StringComparison.OrdinalIgnoreCase))
            {
                LunaLog.Warning($"Rejecting vessel {vesselId} from {playerName}: unresolved localization key '{vesselName}' — vessel was saved in an incomplete state.");
                return false;
            }

            return true;
        }

        // ── Proximity change tracker ─────────────────────────────────────────

        private static void TrackProximityChanges(
            ClientStructure movingClient,
            Guid movingVesselId,
            HashSet<Guid> nowNear,
            List<ClientStructure> interestedClients)
        {
            var stateLock = _proximityLocks.GetOrAdd(movingVesselId, _ => new object());

            lock (stateLock)
            {
                var prevNear = _proximityState.GetOrAdd(movingVesselId, _ => new HashSet<Guid>());

                // Vessels that just entered range
                foreach (var nearId in nowNear)
                {
                    if (!prevNear.Contains(nearId))
                    {
                        var nearClient = interestedClients.FirstOrDefault(c => c.OwnedVesselId == nearId);
                        LockTraceLog.ProximityChange(
                            "PROXIMITY_ENTER",
                            movingClient.PlayerName, movingVesselId,
                            nearClient?.PlayerName ?? "unknown", nearId,
                            0f);  // distance not computed — grid cells give range not exact dist
                    }
                }

                // Vessels that just left range
                foreach (var prevId in prevNear)
                {
                    if (!nowNear.Contains(prevId))
                    {
                        LockTraceLog.ProximityChange(
                            "PROXIMITY_EXIT",
                            movingClient.PlayerName, movingVesselId,
                            "unknown", prevId,
                            0f);
                    }
                }

                // Update the stored set
                _proximityState[movingVesselId] = nowNear;
            }
        }

        private static void HandleVesselsSync(ClientStructure client, VesselBaseMsgData message)
        {
            var syncSw = Stopwatch.StartNew();
            var msgData = (VesselSyncMsgData)message;

            // Before syncing, purge any stale PRELAUNCH / launchpad vessels that have no
            // active owner. These are left behind when a player launched/reverted without
            // the server receiving an updated proto, and will collide with the joining
            // player's craft if they go to the launchpad.
            PurgeStaleKscVessels();

            var allVessels = VesselStoreSystem.CurrentVessels.Keys.ToList();

            //Here we only remove the vessels that the client ALREADY HAS so we only send the vessels they DON'T have
            for (var i = 0; i < msgData.VesselsCount; i++)
                allVessels.Remove(msgData.VesselIds[i]);

            // Per-client version tracking: even if the client reports no known vessels,
            // skip any vessel whose proto hasn't changed since we last sent it to this client.
            // This prevents KSP from parsing and loading 100-160 KB protos every 10 seconds
            // for vessels that haven't structurally changed, which was causing 5 FPS drops.
            var clientVersions = _clientReceivedVersion.GetOrAdd(client.PlayerName,
                _ => new ConcurrentDictionary<Guid, long>());

            var sentCount = 0;
            foreach (var vesselId in allVessels)
            {
                // Assign a version on first encounter (vessels loaded from disk at startup).
                var currentVersion = _vesselProtoVersion.GetOrAdd(vesselId,
                    _ => Interlocked.Increment(ref _vesselVersionCounter));

                // Skip if this client already has the current version.
                if (clientVersions.TryGetValue(vesselId, out var clientVersion) && clientVersion >= currentVersion)
                    continue;

                var vesselData = VesselStoreSystem.GetVesselInConfigNodeFormat(vesselId);
                if (vesselData.Length == 0) continue;

                var protoMsg = ServerContext.ServerMessageFactory.CreateNewMessageData<VesselProtoMsgData>();
                protoMsg.Data = Encoding.UTF8.GetBytes(vesselData);
                protoMsg.NumBytes = vesselData.Length;
                protoMsg.VesselId = vesselId;

                MessageQueuer.SendToClient<VesselSrvMsg>(client, protoMsg);
                clientVersions[vesselId] = currentVersion;
                sentCount++;
            }

            syncSw.Stop();
            if (sentCount > 0)
            {
                LunaLog.Debug($"Sending {client.PlayerName} {sentCount} vessels");
                LagReporter.RecordVesselSync(syncSw.Elapsed.TotalMilliseconds,
                    sentCount, VesselStoreSystem.CurrentVessels.Count, client.PlayerName);
            }
        }

        /// <summary>
        /// Removes PRELAUNCH / launchpad vessels from the store that have no actively
        /// connected owner. Called at vessel-sync time so joining players never receive
        /// a ghost rocket sitting on the launchpad.
        /// </summary>
        private static void PurgeStaleKscVessels()
        {
            // Build a set of vessel IDs actively owned by connected players
            var ownedVessels = new global::System.Collections.Generic.HashSet<Guid>(
                ServerContext.Clients.Values
                    .Where(c => c.Authenticated && c.OwnedVesselId != Guid.Empty)
                    .Select(c => c.OwnedVesselId));

            foreach (var kvp in VesselStoreSystem.CurrentVessels.ToArray())
            {
                if (ownedVessels.Contains(kvp.Key)) continue; // actively piloted, leave it

                var sit      = kvp.Value.Fields.GetSingle("sit")?.Value?.ToLower()      ?? "";
                var landedAt = kvp.Value.Fields.GetSingle("landedAt")?.Value?.ToLower() ?? "";
                var landed   = kvp.Value.Fields.GetSingle("landed")?.Value?.ToLower()   ?? "";

                bool isKscGhost =
                    sit == "prelaunch" ||
                    ((landed == "true") &&
                     (landedAt.Contains("launchpad") || landedAt.Contains("runway") ||
                      landedAt.Contains("ksc")       || landedAt.Contains("launchsite")));

                if (!isKscGhost) continue;

                LunaLog.Normal($"Purging stale KSC vessel {kvp.Key} (sit={sit}, landedAt={landedAt}) before sync");

                VesselStoreSystem.RemoveVessel(kvp.Key);

                var removeMsg = ServerContext.ServerMessageFactory.CreateNewMessageData<VesselRemoveMsgData>();
                removeMsg.VesselId = kvp.Key;
                MessageQueuer.SendToAllClients<VesselSrvMsg>(removeMsg);
            }
        }

        private static void HandleVesselCouple(ClientStructure client, VesselBaseMsgData message)
        {
            var msgData = (VesselCoupleMsgData)message;

            LunaLog.Debug($"Coupling message received! Dominant vessel: {msgData.VesselId}");
            MessageQueuer.RelayMessage<VesselSrvMsg>(client, msgData);

            if (VesselContext.RemovedVessels.ContainsKey(msgData.CoupledVesselId)) return;

            //Now remove the weak vessel but DO NOT add to the removed vessels as they might undock!!!
            LunaLog.Debug($"Removing weak coupled vessel {msgData.CoupledVesselId}");
            VesselStoreSystem.RemoveVessel(msgData.CoupledVesselId);

            //Tell all clients to remove the weak vessel
            var removeMsgData = ServerContext.ServerMessageFactory.CreateNewMessageData<VesselRemoveMsgData>();
            removeMsgData.VesselId = msgData.CoupledVesselId;

            MessageQueuer.SendToAllClients<VesselSrvMsg>(removeMsgData);
        }
    }
}
