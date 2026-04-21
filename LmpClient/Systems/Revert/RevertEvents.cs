using LmpClient;
using LmpClient.Base;
using LmpClient.Systems.VesselRemoveSys;
using LmpCommon.Enums;
using System;

namespace LmpClient.Systems.Revert
{
    public class RevertEvents : SubSystem<RevertSystem>
    {
        // Set true between onRevertingToLaunch and the follow-up onVesselChange so we
        // don't interpret the vessel swap as a voluntary vessel switch (which would
        // clear StartingVesselId and break future revert tracking).
        private static bool _revertingToLaunch = false;

        // ── Vessel tracking ───────────────────────────────────────────────────

        public void OnVesselChange(Vessel data)
        {
            if (_revertingToLaunch)
            {
                _revertingToLaunch = false;
                return;
            }

            // Player voluntarily switched away from the launched vessel —
            // stock revert is no longer possible, so clear the tracker.
            System.StartingVesselId = Guid.Empty;
        }

        public void VesselAssembled(Vessel vessel, ShipConstruct construct)
        {
            // New vessel launched from the editor — this is our revert baseline.
            System.StartingVesselId = vessel.id;
        }

        // ── Revert to Launch ─────────────────────────────────────────────────

        /// <summary>
        /// Fired by the Harmony prefix on <see cref="FlightDriver.RevertToLaunch"/>.
        ///
        /// Stock KSP only calls RevertToLaunch when ALL of the following are true:
        ///   • Same play session (no scene change, no save reload)
        ///   • Still in the flight scene
        ///   • Active vessel == the vessel that was launched (StartingVesselId matches)
        ///   • Vessel is active — not recovered or terminated
        ///   • Revert not disabled in difficulty settings
        ///
        /// PauseMenu_DrawRevertOptions already disables the revert button when any of
        /// these conditions fail, so by the time this prefix fires KSP has already
        /// validated all rules.  We do not need to re-check them here.
        /// </summary>
        public void OnRevertToLaunch()
        {
            _revertingToLaunch = true;

            var vessel = FlightGlobals.ActiveVessel;
            if (vessel == null) return;

            System.StartingVesselId = vessel.id;

            SendRevertRemove(vessel.id, "Revert to Launch");
        }

        // ── Revert to VAB / SPH ───────────────────────────────────────────────

        /// <summary>
        /// Fired by the Harmony prefix on <see cref="FlightDriver.RevertToPrelaunch"/>.
        /// Same stock conditions apply as for Revert to Launch.
        /// The player is going back to the editor, so we clean up the server-side craft
        /// with the same addToKillList=false so they can relaunch with the same vessel ID.
        /// </summary>
        public void OnReturningToEditor(EditorFacility facility)
        {
            var vessel = FlightGlobals.ActiveVessel;
            if (vessel == null) return;

            // Going back to editor counts as leaving the vessel; mark StartingVesselId so
            // the next launch from the editor correctly sets a fresh baseline.
            System.StartingVesselId = vessel.id;

            SendRevertRemove(vessel.id, $"Revert to {facility}");
        }

        // ── Scene / session guards ────────────────────────────────────────────

        public void GameSceneLoadRequested(GameScenes data)
        {
            // If the player leaves the flight scene without reverting (e.g. goes to
            // Space Center, Tracking Station), stock revert is no longer possible.
            // Clear the flag so any in-progress revert state is dropped.
            if (data != GameScenes.FLIGHT && _revertingToLaunch)
                _revertingToLaunch = false;
        }

        // ── Helper ────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends a vessel-remove to the server with <c>addToKillList=false</c> so the
        /// server clears the craft entry before the player relaunches with the same GUID.
        /// No-op when not connected.
        /// </summary>
        private static void SendRevertRemove(Guid vesselId, string context)
        {
            if (MainSystem.NetworkState < ClientState.Running) return;

            // addToKillList = false:
            //   • The server removes the vessel from its store and broadcasts the remove
            //     to all clients so they despawn it.
            //   • The vessel GUID is NOT added to RemovedVessels, so the next proto
            //     message from this GUID (the fresh relaunch) is accepted normally.
            LunaLog.Log($"[LMP]: {context} — cleaning up vessel {vesselId} on server.");
            VesselRemoveSystem.Singleton.MessageSender.SendVesselRemove(vesselId, keepVesselInRemoveList: false);
        }
    }
}
