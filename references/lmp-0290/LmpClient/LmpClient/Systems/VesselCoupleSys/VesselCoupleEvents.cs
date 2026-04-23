// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCoupleEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Systems.Warp;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCoupleEvents : SubSystem<VesselCoupleSystem>
  {
    public void CoupleStart(Part partFrom, Part partTo)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselCoupleSystem>.System.IgnoreEvents || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(partFrom.vessel.id, SettingsSystem.CurrentSettings.PlayerName) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(partTo.vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      LunaLog.Log(string.Format("Detected part couple! Part: {0} Vessel: {1} - CoupledPart: {2} CoupledVessel: {3}", (object) partFrom.partName, (object) partFrom.vessel.id, (object) partTo.partName, (object) partTo.vessel.id));
    }

    public void CoupleComplete(Part partFrom, Part partTo, Guid removedVesselId)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselCoupleSystem>.System.IgnoreEvents || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(partFrom.vessel.id, SettingsSystem.CurrentSettings.PlayerName) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(removedVesselId, SettingsSystem.CurrentSettings.PlayerName))
        return;
      LunaLog.Log(string.Format("Couple complete! Removed vessel: {0}", (object) removedVesselId));
      CoupleTrigger trigger = Object.op_Inequality((Object) partTo.FindModuleImplementing<ModuleDockingNode>(), (Object) null) ? CoupleTrigger.DockingNode : (Object.op_Inequality((Object) partTo.FindModuleImplementing<ModuleGrappleNode>(), (Object) null) ? CoupleTrigger.GrappleNode : (Object.op_Inequality((Object) partTo.FindModuleImplementing<KerbalEVA>(), (Object) null) ? CoupleTrigger.Kerbal : CoupleTrigger.Other));
      SubSystem<VesselCoupleSystem>.System.MessageSender.SendVesselCouple(partFrom.vessel, partTo.flightID, removedVesselId, partFrom.flightID, trigger);
      if (LockSystem.LockQuery.UpdateLockBelongsToPlayer(partFrom.vessel.id, SettingsSystem.CurrentSettings.PlayerName))
      {
        foreach (ProtoCrewMember protoCrewMember in partFrom.vessel.GetVesselCrew())
          LmpClient.Base.System<LockSystem>.Singleton.AcquireKerbalLock(protoCrewMember.name, true);
        VesselCoupleEvents.JumpIfVesselOwnerIsInFuture(removedVesselId);
      }
      else
        VesselCoupleEvents.JumpIfVesselOwnerIsInFuture(partFrom.vessel.id);
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.MessageSender.SendVesselRemove(removedVesselId, false);
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.DelayedKillVessel(removedVesselId, false, "Killing coupled vessel during a detected coupling", 500);
      LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks((IEnumerable<string>) null, removedVesselId);
    }

    private static void JumpIfVesselOwnerIsInFuture(Guid vesselId)
    {
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(vesselId);
      if (controlLockOwner == null)
        return;
      LmpClient.Base.System<WarpSystem>.Singleton.WarpIfSubspaceIsMoreAdvanced(LmpClient.Base.System<WarpSystem>.Singleton.GetPlayerSubspace(controlLockOwner));
    }
  }
}
