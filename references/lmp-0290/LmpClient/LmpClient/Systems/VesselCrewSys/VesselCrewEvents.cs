// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCrewSys.VesselCrewEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Utilities;
using System;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselCrewSys
{
  public class VesselCrewEvents : SubSystem<VesselCrewSystem>
  {
    public void OnCrewBoard(Guid kerbalId, string kerbalName, Vessel vessel)
    {
      LunaLog.Log("Crew boarding detected!");
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.MessageSender.SendVesselRemove(kerbalId, false);
      LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks((IEnumerable<string>) new string[1]
      {
        kerbalName
      }, kerbalId);
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(kerbalId, true, "Killing kerbal-vessel as it boarded a vessel");
      LmpClient.Base.System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel, true);
    }

    public void OnCrewEva(GameEvents.FromToAction<Part, Part> data) => EvaReady.FireOnCrewEvaReady(data.to.FindModuleImplementing<KerbalEVA>());

    public void CrewEvaReady(Vessel evaVessel) => LmpClient.Base.System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(evaVessel, true);

    public void OnCrewModified(Vessel vessel)
    {
      if (vessel.isEVA || !LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      LmpClient.Base.System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel, true);
    }
  }
}
