// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProtoEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.ShareScienceSubject;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProtoEvents : SubSystem<VesselProtoSystem>
  {
    public void WarpStopped() => SubSystem<VesselProtoSystem>.System.CheckVesselsToLoad();

    public void FlightReady()
    {
      if (VesselCommon.IsSpectating || Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null) || FlightGlobals.ActiveVessel.id == Guid.Empty)
        return;
      SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel, true);
    }

    internal void OnSceneRequested(GameScenes requestedScene)
    {
      if (!HighLogic.LoadedSceneIsFlight || requestedScene == 7 || VesselCommon.IsSpectating)
        return;
      LmpClient.Base.System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel);
    }

    public void TriggeredDataTransmission(ScienceData science, Vessel vessel, bool data)
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      ScienceSubject subjectById = ResearchAndDevelopment.GetSubjectByID(science.subjectID);
      if (subjectById != null)
      {
        LunaLog.Log("Detected a experiment transmission. Sending vessel definition to the server");
        SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel, true);
        LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.MessageSender.SendScienceSubjectMessage(subjectById);
      }
    }

    public void ExperimentStored(ScienceData science)
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      ScienceSubject subjectById = ResearchAndDevelopment.GetSubjectByID(science.subjectID);
      if (subjectById != null)
      {
        LunaLog.Log("Detected a experiment stored. Sending vessel definition to the server");
        SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel, true);
        LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.MessageSender.SendScienceSubjectMessage(subjectById);
      }
    }

    public void ExperimentReset(Vessel data)
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      LunaLog.Log("Detected a experiment reset. Sending vessel definition to the server");
      SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel, true);
    }

    public void PartUndocked(Part part, DockedVesselInfo dockedInfo, Vessel originalVessel)
    {
      if (VesselCommon.IsSpectating || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(originalVessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(part.vessel);
      SubSystem<VesselProtoSystem>.System.DelayedSendVesselMessage(originalVessel.id, 0.5f);
    }

    public void PartDecoupled(Part part, float breakForce, Vessel originalVessel)
    {
      if (VesselCommon.IsSpectating || Object.op_Equality((Object) originalVessel, (Object) null) || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(originalVessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(part.vessel);
      SubSystem<VesselProtoSystem>.System.DelayedSendVesselMessage(originalVessel.id, 0.5f);
    }

    public void PartCoupled(Part partFrom, Part partTo, Guid removedVesselId)
    {
      if (VesselCommon.IsSpectating || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(partFrom.vessel.id, SettingsSystem.CurrentSettings.PlayerName) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(removedVesselId, SettingsSystem.CurrentSettings.PlayerName))
        return;
      SubSystem<VesselProtoSystem>.System.MessageSender.SendVesselMessage(partFrom.vessel);
    }
  }
}
