// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselImmortalSys.VesselImmortalSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Locks;
using UnityEngine;

namespace LmpClient.Systems.VesselImmortalSys
{
  public class VesselImmortalSystem : System<VesselImmortalSystem>
  {
    public static VesselImmortalEvents VesselImmortalEvents { get; } = new VesselImmortalEvents();

    public override string SystemName { get; } = nameof (VesselImmortalSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      RailEvent.onVesselGoneOnRails.Add(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselGoOnRails)));
      // ISSUE: method pointer
      RailEvent.onVesselGoneOffRails.Add(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselGoOffRails)));
      // ISSUE: method pointer
      GameEvents.onVesselPartCountChanged.Add(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(PartCountChanged)));
      // ISSUE: method pointer
      GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Add(new EventVoid.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(StartSpectating)));
      // ISSUE: method pointer
      SpectateEvent.onFinishedSpectating.Add(new EventVoid.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(FinishSpectating)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnLockRelease)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Add(new EventData<Vessel, bool>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Add(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnVesselCreated)));
      foreach (Vessel vessel in FlightGlobals.VesselsLoaded)
        this.SetImmortalStateBasedOnLock(vessel);
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      RailEvent.onVesselGoneOnRails.Remove(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselGoOnRails)));
      // ISSUE: method pointer
      RailEvent.onVesselGoneOffRails.Remove(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselGoOffRails)));
      // ISSUE: method pointer
      GameEvents.onVesselPartCountChanged.Remove(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(PartCountChanged)));
      // ISSUE: method pointer
      GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Remove(new EventVoid.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(StartSpectating)));
      // ISSUE: method pointer
      SpectateEvent.onFinishedSpectating.Remove(new EventVoid.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(FinishSpectating)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Remove(new EventData<LockDefinition>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnLockRelease)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Remove(new EventData<Vessel, bool>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Remove(new EventData<Vessel>.OnEvent((object) VesselImmortalSystem.VesselImmortalEvents, __methodptr(OnVesselCreated)));
      foreach (Vessel vessel in FlightGlobals.VesselsLoaded)
        vessel.SetImmortal(false);
    }

    public void SetImmortalStateBasedOnLock(Vessel vessel)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      bool flag = LockSystem.LockQuery.ControlLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName) || LockSystem.LockQuery.UpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName) || !LockSystem.LockQuery.UpdateLockExists(vessel.id);
      vessel.SetImmortal(!flag);
    }
  }
}
