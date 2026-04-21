using LmpClient;
using LmpClient.Base;
using LmpClient.Systems.VesselRemoveSys;
using LmpCommon.Enums;
using System;

namespace LmpClient.Systems.Revert
{
    public class RevertEvents : SubSystem<RevertSystem>
    {
        private static bool _revertingToLaunch = false;

        public void OnVesselChange(Vessel data)
        {
            if (_revertingToLaunch)
            {
                _revertingToLaunch = false;
                return;
            }

            System.StartingVesselId = Guid.Empty;
        }

        public void VesselAssembled(Vessel vessel, ShipConstruct construct)
        {
            System.StartingVesselId = vessel.id;
        }

        /// <summary>
        /// Called by the Harmony prefix on FlightDriver.RevertToLaunch.
        /// Sends a vessel-remove to the server (addToKillList=false) so the server
        /// clears the craft before the player relaunches with the same vessel ID.
        /// Only the owning player can trigger this locally — KSP's revert is a
        /// local game function and cannot be called remotely by another client.
        /// </summary>
        public void OnRevertToLaunch()
        {
            _revertingToLaunch = true;

            var vessel = FlightGlobals.ActiveVessel;
            if (vessel == null) return;

            System.StartingVesselId = vessel.id;

            // Only send if we're connected — no point queuing a remove when offline
            if (MainSystem.NetworkState < ClientState.Running) return;

            // addToKillList = false: we will relaunch with the same vessel ID,
            // so future proto updates for this GUID must be accepted by the server.
            LunaLog.Log($"[LMP]: Revert to launch — cleaning up vessel {vessel.id} on server.");
            VesselRemoveSystem.Singleton.MessageSender.SendVesselRemove(vessel.id, keepVesselInRemoveList: false);
        }

        public void GameSceneLoadRequested(GameScenes data)
        {
            if (data != GameScenes.FLIGHT && _revertingToLaunch)
                _revertingToLaunch = false;
        }
    }
}
