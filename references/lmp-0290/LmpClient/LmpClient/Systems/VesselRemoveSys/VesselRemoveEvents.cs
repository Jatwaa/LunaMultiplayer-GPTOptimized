// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselRemoveSys.VesselRemoveEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Localization;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.VesselUtilities;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.VesselRemoveSys
{
  public class VesselRemoveEvents : SubSystem<VesselRemoveSystem>
  {
    private static Guid _recoveringTerminatingVesselId = Guid.Empty;

    public void OnVesselWillDestroy(Vessel dyingVessel)
    {
      if (LockSystem.LockQuery.UnloadedUpdateLockExists(dyingVessel.id) && !LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(dyingVessel.id, SettingsSystem.CurrentSettings.PlayerName) && !(dyingVessel.id == VesselRemoveEvents._recoveringTerminatingVesselId))
        return;
      bool flag = Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && FlightGlobals.ActiveVessel.id == dyingVessel.id;
      string str = dyingVessel.id == VesselRemoveEvents._recoveringTerminatingVesselId ? "Recovered/Terminated" : "Destroyed";
      LunaLog.Log(string.Format("[LMP]: Removing vessel {0}-{1}, Name: {2} from the server: {3}", (object) dyingVessel.id, (object) dyingVessel.persistentId, (object) dyingVessel.vesselName, (object) str));
      if (!flag)
      {
        SubSystem<VesselRemoveSystem>.System.KillVessel(dyingVessel.id, true, "OnVesselWillDestroy - " + str);
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(dyingVessel);
      }
      else
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(dyingVessel, false);
      LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks((IEnumerable<string>) null, dyingVessel.id, 0.5f);
      RemoveEvent.onLmpDestroyVessel.Fire(dyingVessel);
      VesselCommon.RemoveVesselFromSystems(dyingVessel.id);
    }

    public void OnVesselRecovering(Vessel recoveredVessel) => this.OnVesselRecovered(recoveredVessel.protoVessel, false);

    public void OnVesselRecovered(ProtoVessel recoveredVessel, bool quick)
    {
      if (!LockSystem.LockQuery.CanRecoverOrTerminateTheVessel(recoveredVessel.vesselID, SettingsSystem.CurrentSettings.PlayerName))
      {
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CannotRecover, 5f, (ScreenMessageStyle) 0);
      }
      else
      {
        VesselRemoveEvents._recoveringTerminatingVesselId = recoveredVessel.vesselID;
        LunaLog.Log(string.Format("[LMP]: Removing vessel {0}, Name: {1} from the server: Recovered", (object) recoveredVessel.vesselID, (object) recoveredVessel.vesselName));
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(recoveredVessel.vesselID);
        LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks((IEnumerable<string>) null, recoveredVessel.vesselID, 1f);
        SubSystem<VesselRemoveSystem>.System.RemovedVessels.TryAdd(recoveredVessel.vesselID, DateTime.Now);
        RemoveEvent.onLmpRecoveredVessel.Fire(recoveredVessel);
        VesselCommon.RemoveVesselFromSystems(recoveredVessel.vesselID);
      }
    }

    public void OnVesselTerminated(ProtoVessel terminatedVessel)
    {
      if (!LockSystem.LockQuery.CanRecoverOrTerminateTheVessel(terminatedVessel.vesselID, SettingsSystem.CurrentSettings.PlayerName))
      {
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CannotTerminate, 5f, (ScreenMessageStyle) 0);
      }
      else
      {
        VesselRemoveEvents._recoveringTerminatingVesselId = terminatedVessel.vesselID;
        LunaLog.Log(string.Format("[LMP]: Removing vessel {0}, Name: {1} from the server: Terminated", (object) terminatedVessel.vesselID, (object) terminatedVessel.vesselName));
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(terminatedVessel.vesselID);
        LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks((IEnumerable<string>) null, terminatedVessel.vesselID, 1f);
        SubSystem<VesselRemoveSystem>.System.RemovedVessels.TryAdd(terminatedVessel.vesselID, DateTime.Now);
        RemoveEvent.onLmpRecoveredVessel.Fire(terminatedVessel);
        VesselCommon.RemoveVesselFromSystems(terminatedVessel.vesselID);
      }
    }

    public void OnRevertToLaunch()
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      LunaLog.Log("[LMP]: Detected a revert to launch!");
      VesselRemoveEvents.RemoveOldVesselAndItsDebris(FlightGlobals.ActiveVessel, (ProtoCrewMember.RosterStatus) 1);
      SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(FlightGlobals.ActiveVessel, false);
      VesselCommon.RemoveVesselFromSystems(FlightGlobals.ActiveVessel.id);
    }

    public void OnRevertToEditor(EditorFacility data)
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      LunaLog.Log(string.Format("[LMP]: Detected a revert to editor! {0}", (object) data));
      VesselRemoveEvents.RemoveOldVesselAndItsDebris(FlightGlobals.ActiveVessel, (ProtoCrewMember.RosterStatus) 0);
      SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(FlightGlobals.ActiveVessel);
      SubSystem<VesselRemoveSystem>.System.RemovedVessels.TryAdd(FlightGlobals.ActiveVessel.id, DateTime.Now);
      VesselCommon.RemoveVesselFromSystems(FlightGlobals.ActiveVessel.id);
    }

    private static void RemoveOldVesselAndItsDebris(
      Vessel vessel,
      ProtoCrewMember.RosterStatus kerbalStatus)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      if (FlightGlobals.ActiveVessel.isEVA)
      {
        ProtoCrewMember protoCrewMember = HighLogic.CurrentGame.CrewRoster[FlightGlobals.ActiveVessel.vesselName];
        if (protoCrewMember != null)
          protoCrewMember.rosterStatus = kerbalStatus;
        SubSystem<VesselRemoveSystem>.System.KillVessel(FlightGlobals.ActiveVessel.id, true, "Revert. Active vessel is a kerbal");
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(FlightGlobals.ActiveVessel);
      }
      foreach (Vessel vessel1 in Enumerable.Distinct<Vessel>(Enumerable.Where<Vessel>((IEnumerable<Vessel>) FlightGlobals.Vessels, (Func<Vessel, bool>) (v => Object.op_Inequality((Object) v, (Object) null) && Object.op_Implicit((Object) v.rootPart) && (int) v.rootPart.missionID == (int) vessel.rootPart.missionID && v.id != vessel.id))))
      {
        if (vessel1.isEVA)
        {
          ProtoCrewMember protoCrewMember = HighLogic.CurrentGame.CrewRoster[vessel1.vesselName];
          if (protoCrewMember != null)
            protoCrewMember.rosterStatus = kerbalStatus;
        }
        SubSystem<VesselRemoveSystem>.System.MessageSender.SendVesselRemove(vessel1);
        SubSystem<VesselRemoveSystem>.System.RemovedVessels.TryAdd(vessel1.id, DateTime.Now);
        VesselCommon.RemoveVesselFromSystems(vessel1.id);
      }
    }
  }
}
