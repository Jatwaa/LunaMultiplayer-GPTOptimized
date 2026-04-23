// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselLockSys.VesselLockSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Localization;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.VesselUtilities;
using LmpCommon.Locks;
using System;

namespace LmpClient.Systems.VesselLockSys
{
  public class VesselLockSystem : LmpClient.Base.System<VesselLockSystem>
  {
    public const string SpectateLock = "LMP_Spectating";
    public const ControlTypes BlockAllControls = (ControlTypes) 884394376824814271;
    private ScreenMessage _spectateMessage;

    private string GetVesselOwner => !VesselCommon.IsSpectating ? "" : LockSystem.LockQuery.GetControlLockOwner(FlightGlobals.ActiveVessel.id);

    private VesselLockEvents VesselLockEvents { get; } = new VesselLockEvents();

    private string SpectatingMessage => !VesselCommon.IsSpectating ? "" : LocalizationContainer.ScreenText.Spectating + " " + this.GetVesselOwner + ".";

    public override string SystemName { get; } = nameof (VesselLockSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.VesselLockEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselLoaded.Add(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(VesselLoaded)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) this.VesselLockEvents, __methodptr(LockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) this.VesselLockEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      VesselUnloadEvent.onVesselUnloading.Add(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(VesselUnloading)));
      // ISSUE: method pointer
      FlightDriverEvent.onFlightStarted.Add(new EventVoid.OnEvent((object) this.VesselLockEvents, __methodptr(FlightStarted)));
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.UpdateOnScreenSpectateMessage)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.VesselLockEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselLoaded.Remove(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(VesselLoaded)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Remove(new EventData<LockDefinition>.OnEvent((object) this.VesselLockEvents, __methodptr(LockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) this.VesselLockEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      VesselUnloadEvent.onVesselUnloading.Remove(new EventData<Vessel>.OnEvent((object) this.VesselLockEvents, __methodptr(VesselUnloading)));
      // ISSUE: method pointer
      FlightDriverEvent.onFlightStarted.Remove(new EventVoid.OnEvent((object) this.VesselLockEvents, __methodptr(FlightStarted)));
    }

    private void UpdateOnScreenSpectateMessage()
    {
      if (VesselCommon.IsSpectating)
      {
        if (this._spectateMessage != null)
          this._spectateMessage.duration = 0.0f;
        this._spectateMessage = LunaScreenMsg.PostScreenMessage(this.SpectatingMessage, 2000f, (ScreenMessageStyle) 0);
      }
      else if (this._spectateMessage != null)
      {
        this._spectateMessage.duration = 0.0f;
        this._spectateMessage = (ScreenMessage) null;
      }
    }

    public void StartSpectating(Guid spectatingVesselId)
    {
      VesselCommon.IsSpectating = true;
      InputLockManager.SetControlLock((ControlTypes) 884394376824814271L, "LMP_Spectating");
      LmpClient.Base.System<LockSystem>.Singleton.AcquireSpectatorLock();
      LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllPlayerSpecifiedLocks(LockType.Control, LockType.Update, LockType.Kerbal, LockType.UnloadedUpdate);
      HighLogic.CurrentGame.Parameters.Flight.CanEVA = false;
      SpectateEvent.onStartSpectating.Fire();
    }

    public void StopSpectating()
    {
      VesselCommon.IsSpectating = false;
      InputLockManager.RemoveControlLock("LMP_Spectating");
      if (LockSystem.LockQuery.SpectatorLockExists(SettingsSystem.CurrentSettings.PlayerName))
        LmpClient.Base.System<LockSystem>.Singleton.ReleaseSpectatorLock();
      foreach (Vessel vessel in FlightGlobals.Vessels)
      {
        if (!LockSystem.LockQuery.UnloadedUpdateLockExists(vessel.id))
          LmpClient.Base.System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(vessel.id);
      }
      if (HighLogic.CurrentGame?.Parameters?.Flight != null)
        HighLogic.CurrentGame.Parameters.Flight.CanEVA = true;
      if (PauseMenu.exists && PauseMenu.isOpen)
        PauseMenu.canSaveAndExit = FlightGlobals.ClearToSave();
      SpectateEvent.onFinishedSpectating.Fire();
    }
  }
}
