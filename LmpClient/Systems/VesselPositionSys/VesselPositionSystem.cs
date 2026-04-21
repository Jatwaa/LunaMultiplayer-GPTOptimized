using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.VesselUtilities;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace LmpClient.Systems.VesselPositionSys
{
    /// <summary>
    /// System that handle the received vessel update messages and also sends them
    /// </summary>
    public class VesselPositionSystem : MessageSystem<VesselPositionSystem, VesselPositionMessageSender, VesselPositionMessageHandler>
    {
        #region Fields & properties

        private static DateTime LastVesselUpdatesSentTime { get; set; } = LunaComputerTime.UtcNow;

        private static int UpdateIntervalLockedToUnity => (int)(Math.Floor(SettingsSystem.ServerSettings.VesselUpdatesMsInterval
            / TimeSpan.FromSeconds(Time.fixedDeltaTime).TotalMilliseconds) * TimeSpan.FromSeconds(Time.fixedDeltaTime).TotalMilliseconds);

        private static int SecondaryVesselUpdatesUpdateIntervalLockedToUnity => (int)(Math.Floor(SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval
            / TimeSpan.FromSeconds(Time.fixedDeltaTime).TotalMilliseconds) * TimeSpan.FromSeconds(Time.fixedDeltaTime).TotalMilliseconds);

        private static bool TimeToSendVesselUpdate => VesselCommon.PlayerVesselsNearby() ?
            (LunaComputerTime.UtcNow - LastVesselUpdatesSentTime).TotalMilliseconds > UpdateIntervalLockedToUnity :
            (LunaComputerTime.UtcNow - LastVesselUpdatesSentTime).TotalMilliseconds > SecondaryVesselUpdatesUpdateIntervalLockedToUnity;

        public bool PositionUpdateSystemReady => Enabled && FlightGlobals.ActiveVessel != null &&
                                         FlightGlobals.ready && FlightGlobals.ActiveVessel.loaded &&
                                         FlightGlobals.ActiveVessel.state != Vessel.State.DEAD &&
                                         FlightGlobals.ActiveVessel.vesselType != VesselType.Flag;

        public PositionEvents PositionEvents { get; } = new PositionEvents();

        public bool PositionUpdateSystemBasicReady => Enabled && PositionUpdateSystemReady || HighLogic.LoadedScene == GameScenes.TRACKSTATION;

        public static ConcurrentDictionary<Guid, VesselPositionUpdate> CurrentVesselUpdate { get; } =
            new ConcurrentDictionary<Guid, VesselPositionUpdate>();

        public static ConcurrentDictionary<Guid, PositionUpdateQueue> TargetVesselUpdateQueue { get; } =
            new ConcurrentDictionary<Guid, PositionUpdateQueue>();

        private List<Vessel> SecondaryVesselsToUpdate { get; } = new List<Vessel>();
        private List<Vessel> AbandonedVesselsToUpdate { get; } = new List<Vessel>();

        // Per-vessel last-sent timestamps for adaptive secondary update throttling.
        // Keyed by vessel ID; value is the UTC time the last position message was sent.
        private static readonly ConcurrentDictionary<Guid, DateTime> _lastSecondaryUpdateTime
            = new ConcurrentDictionary<Guid, DateTime>();

        // How long to wait between position sends depending on vessel state:
        private const int DebrisLandedUpdateIntervalMs     = 0;      // 0 = skip entirely (static)
        private const int DebrisOrbitingUpdateIntervalMs   = 2000;   // debris in flight: 2 s
        private const int VesselLandedUpdateIntervalMs     = 1000;   // landed non-debris: 1 s

        private const TimingManager.TimingStage HandlePositionsStage = TimingManager.TimingStage.BetterLateThanNever;
        private const TimingManager.TimingStage SendPositionsStage = TimingManager.TimingStage.BetterLateThanNever;

        #endregion

        #region Base overrides

        public override string SystemName { get; } = nameof(VesselPositionSystem);

        protected override bool ProcessMessagesInUnityThread => false;

        protected override void OnEnabled()
        {
            base.OnEnabled();

            TimingManager.FixedUpdateAdd(HandlePositionsStage, HandleVesselUpdates);

            //Send the position updates after all the calculations are done. If you send it in the fixed update sometimes weird rubber banding appear (specially in space)
            TimingManager.LateUpdateAdd(SendPositionsStage, SendVesselPositionUpdates);

            //It's important that SECONDARY vessels send their position in the UPDATE as their parameters will NOT be updated on the fixed update if the are packed.
            //https://forum.kerbalspaceprogram.com/index.php?/topic/173885-packed-vessels-position-isnt-reliable-from-fixedupdate/
            SetupRoutine(new RoutineDefinition(SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval, RoutineExecution.LateUpdate, SendSecondaryVesselPositionUpdates));
            //SetupRoutine(new RoutineDefinition(SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval, RoutineExecution.LateUpdate, SendUnloadedSecondaryVesselPositionUpdates));

            WarpEvent.onTimeWarpStopped.Add(PositionEvents.WarpStopped);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            TimingManager.FixedUpdateRemove(HandlePositionsStage, HandleVesselUpdates);
            TimingManager.LateUpdateRemove(SendPositionsStage, SendVesselPositionUpdates);

            CurrentVesselUpdate.Clear();
            TargetVesselUpdateQueue.Clear();
            _lastSecondaryUpdateTime.Clear();
        }

        private void HandleVesselUpdates()
        {
            Profiler.BeginSample(nameof(HandleVesselUpdates));

            float dt = Time.fixedDeltaTime;
            foreach (var keyVal in CurrentVesselUpdate)
            {
                var update = keyVal.Value;
                
                // --- Dead Reckoning Extrapolation ---
                // Move predicted position forward based on current velocity
                Vector3 velocity = new Vector3((float)update.Velocity.x, (float)update.Velocity.y, (float)update.Velocity.z);
                update.PredictedPosition += velocity * dt;

                // Calculate error between prediction and authoritative server target
                if (update.Target != null)
                {
                    // Use simple linear blend (Spring-like) to steer prediction toward target
                    float blendFactor = 0.1f; // Adjust for "tightness" of movement
                    
                    Vector3 targetPos = new Vector3(
                        (float)update.Target.LatLonAlt[0], 
                        (float)update.Target.LatLonAlt[1], 
                        (float)update.Target.LatLonAlt[2]
                    );
                    
                    update.PredictedPosition = Vector3.Lerp(update.PredictedPosition, targetPos, blendFactor);
                    update.PredictionError = Vector3.Distance(update.PredictedPosition, targetPos);
                }

                update.ApplyInterpolatedVesselUpdate();
            }

            Profiler.EndSample();
        }

        #endregion

        #region FixedUpdate methods

        /// <summary>
        /// Send the updates of our own vessel. We only send them after an interval specified.
        /// If the other player vessels are far we don't send them very often.
        /// </summary>
        private void SendVesselPositionUpdates()
        {
            Profiler.BeginSample(nameof(SendVesselPositionUpdates));

            if (PositionUpdateSystemReady && TimeToSendVesselUpdate && !VesselCommon.IsSpectating)
            {
                MessageSender.SendVesselPositionUpdate(FlightGlobals.ActiveVessel);
                LastVesselUpdatesSentTime = LunaComputerTime.UtcNow;
            }

            Profiler.EndSample();
        }

        #endregion

        #region Update methods

        /// <summary>
        /// Send updates for vessels that we own the update lock, with adaptive per-vessel throttling:
        ///   • Debris that is landed/splashed  → skip (it is physically static; no need to broadcast)
        ///   • Debris that is in flight/orbit  → send every 2 s (slow-moving relative to game time)
        ///   • Any vessel that is landed/splashed → send every 1 s (not moving, saves bandwidth)
        ///   • Anything else                   → send at the normal secondary interval (~150 ms)
        /// </summary>
        private void SendSecondaryVesselPositionUpdates()
        {
            if (!PositionUpdateSystemReady || VesselCommon.IsSpectating) return;

            SecondaryVesselsToUpdate.Clear();
            SecondaryVesselsToUpdate.AddRange(VesselCommon.GetSecondaryVessels());

            var now = LunaComputerTime.UtcNow;

            for (var i = 0; i < SecondaryVesselsToUpdate.Count; i++)
            {
                var vessel = SecondaryVesselsToUpdate[i];

                // If we are replaying past updates from a future-subspace player, skip sending.
                if (VesselHavePositionUpdatesQueued(vessel.id)) continue;

                bool isDebris  = vessel.vesselType == VesselType.Debris;
                bool isStatic  = vessel.Landed || vessel.Splashed;

                // Landed/splashed debris is completely static — never send position updates for it.
                if (isDebris && isStatic) continue;

                // Determine how many ms must have elapsed before sending this vessel again.
                int requiredIntervalMs = isDebris ? DebrisOrbitingUpdateIntervalMs
                                       : isStatic  ? VesselLandedUpdateIntervalMs
                                       : 0; // 0 = always send (governed by the routine interval itself)

                if (requiredIntervalMs > 0)
                {
                    if (_lastSecondaryUpdateTime.TryGetValue(vessel.id, out var lastSent) &&
                        (now - lastSent).TotalMilliseconds < requiredIntervalMs)
                        continue;
                }

                MessageSender.SendVesselPositionUpdate(vessel);
                _lastSecondaryUpdateTime[vessel.id] = now;
            }
        }

        /// <summary>
        /// Send updates for vessels that we own the update lock. And also send it for the abandoned ones
        /// </summary>
        private void SendUnloadedSecondaryVesselPositionUpdates()
        {
            if (PositionUpdateSystemBasicReady && !VesselCommon.IsSpectating)
            {
                AbandonedVesselsToUpdate.Clear();
                AbandonedVesselsToUpdate.AddRange(VesselCommon.GetUnloadedSecondaryVessels());

                for (var i = 0; i < AbandonedVesselsToUpdate.Count; i++)
                {
                    //This is the case when you've got an update lock from a vessel that was controlled by a player in a future subspace.
                    //You don't need to send position updates for it as you're replaying them from the past
                    if (VesselHavePositionUpdatesQueued(AbandonedVesselsToUpdate[i].id))
                        continue;

                    UpdateUnloadedVesselValues(AbandonedVesselsToUpdate[i]);
                    MessageSender.SendVesselPositionUpdate(AbandonedVesselsToUpdate[i]);
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Forcefully updates a vessel position
        /// </summary>
        public void ForceUpdateVesselPosition(Guid vesselId)
        {
            if (CurrentVesselUpdate.TryGetValue(vesselId, out var posUpdate))
                posUpdate.UpdateVesselWithPositionData();
        }

        /// <summary>
        /// Checks if the given vessel id has position messages stored to be replayed
        /// </summary>
        public bool VesselHavePositionUpdatesQueued(Guid vesselId)
        {
            if (TargetVesselUpdateQueue.TryGetValue(vesselId, out var positionQueue))
            {
                return positionQueue.Count > 0;
            }

            return false;
        }

        /// <summary>
        /// Removes a vessel from the system and fully drains its position queue so
        /// no stale future positions can drive a brief ghost after the vessel is gone.
        /// </summary>
        public void RemoveVessel(Guid vesselId)
        {
            CurrentVesselUpdate.TryRemove(vesselId, out _);
            _lastSecondaryUpdateTime.TryRemove(vesselId, out _);
            if (TargetVesselUpdateQueue.TryRemove(vesselId, out var leftoverQueue))
            {
                // Drain any items that were enqueued before the remove was seen
                while (leftoverQueue.TryDequeue(out _)) { }
            }
        }

        /// <summary>
        /// Force adjustment of interpolation times
        /// </summary>
        public void AdjustExtraInterpolationTimes()
        {
            //Remove the target of the current update so basically the interpolation is stopped and on the next frame we pick a new one
            foreach (var keyVal in CurrentVesselUpdate)
            {
                keyVal.Value.Target = null;
            }

            //Now cleanup the target dictionary of old positions
            foreach (var keyVal in TargetVesselUpdateQueue)
            {
                while (keyVal.Value.TryPeek(out var targetUpd) && PositionUpdateIsTooOld(targetUpd))
                    keyVal.Value.TryDequeue(out _);
            }
        }

        #endregion

        #region Private methods

        private static bool PositionUpdateIsTooOld(VesselPositionUpdate update)
        {
            return update.GameTimeStamp < TimeSyncSystem.UniversalTime - VesselCommon.PositionAndFlightStateMessageOffsetSec(update.PingSec);
        }

        /// <summary>
        /// Unloaded vessels don't update their lat/lon/alt and it's orbit params.
        /// As we have the unloadedupdate lock of that vessel we need to refresh those values manually
        /// </summary>
        private static void UpdateUnloadedVesselValues(Vessel vessel)
        {
            if (vessel.orbit != null)
            {
                vessel.UpdatePosVel();
                if (!vessel.LandedOrSplashed)
                {
                    if (vessel.orbitDriver) vessel.orbitDriver.UpdateOrbit();
                    vessel.mainBody.GetLatLonAltOrbital(vessel.orbit.pos, out vessel.latitude, out vessel.longitude, out vessel.altitude);
                }
            }
        }

        #endregion
    }
}
