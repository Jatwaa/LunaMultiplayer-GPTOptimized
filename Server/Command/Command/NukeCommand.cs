using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Server;
using Server.Command.Command.Base;
using Server.Context;
using Server.Log;
using Server.Server;
using Server.Settings.Structures;
using Server.System;
using System;

namespace Server.Command.Command
{
    public class NukeCommand : SimpleCommand
    {
        private static long _lastNukeTime;

        public static void CheckTimer()
        {
            //0 or less is disabled.
            if (GeneralSettings.SettingsStore.AutoNuke > 0 &&
                 ServerContext.ServerClock.ElapsedMilliseconds - _lastNukeTime >
                 TimeSpan.FromMinutes(GeneralSettings.SettingsStore.AutoNuke).TotalMilliseconds)
            {
                _lastNukeTime = ServerContext.ServerClock.ElapsedMilliseconds;
                RunNuke();
            }
        }

        public override bool Execute(string commandArgs)
        {
            RunNuke();
            return true;
        }

        private static void RunNuke()
        {
            var removalCount = 0;

            var vesselList = VesselStoreSystem.CurrentVessels.ToArray();
            foreach (var vesselKeyVal in vesselList)
            {
                // Skip vessels actively controlled by a connected player
                if (IsActivelyControlled(vesselKeyVal.Key)) continue;

                if (IsAtKsc(vesselKeyVal.Value))
                {
                    LunaLog.Normal($"Removing vessel: {vesselKeyVal.Key} from KSC");

                    VesselStoreSystem.RemoveVessel(vesselKeyVal.Key);

                    //Send a vessel remove message
                    var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<VesselRemoveMsgData>();
                    msgData.VesselId = vesselKeyVal.Key;

                    MessageQueuer.SendToAllClients<VesselSrvMsg>(msgData);

                    removalCount++;
                }
            }

            if (removalCount > 0)
                LunaLog.Normal($"Nuked {removalCount} vessels around the KSC");
        }

        /// <summary>
        /// Returns true if the vessel is at the KSC (launchpad, runway, or landed at KSC).
        /// Covers both PRELAUNCH situation and landed = true with landedAt containing KSC locations.
        /// </summary>
        private static bool IsAtKsc(global::Server.System.Vessel.Classes.Vessel vessel)
        {
            var situation  = vessel.Fields.GetSingle("sit")?.Value?.ToLower()      ?? "";
            var landedAt   = vessel.Fields.GetSingle("landedAt")?.Value?.ToLower() ?? "";
            var landed     = vessel.Fields.GetSingle("landed")?.Value?.ToLower()   ?? "";
            var splashed   = vessel.Fields.GetSingle("splashed")?.Value?.ToLower() ?? "";

            // PRELAUNCH = sitting on launchpad before launch
            if (situation == "prelaunch") return true;

            // Landed/splashed at a KSC facility
            if ((landed == "true" || splashed == "true") &&
                (landedAt.Contains("ksc")       ||
                 landedAt.Contains("runway")    ||
                 landedAt.Contains("launchpad") ||
                 landedAt.Contains("launchsite")))
                return true;

            return false;
        }

        /// <summary>Returns true if any connected player currently owns this vessel.</summary>
        private static bool IsActivelyControlled(Guid vesselId)
        {
            foreach (var client in ServerContext.Clients.Values)
            {
                if (client.Authenticated && client.OwnedVesselId == vesselId)
                    return true;
            }
            return false;
        }
    }
}
