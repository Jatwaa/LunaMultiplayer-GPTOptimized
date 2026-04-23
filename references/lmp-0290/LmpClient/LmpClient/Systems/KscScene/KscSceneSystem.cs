// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KscScene.KscSceneSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Events;
using LmpCommon.Locks;
using System;
using System.Reflection;
using UnityEngine;

namespace LmpClient.Systems.KscScene
{
  public class KscSceneSystem : LmpClient.Base.System<KscSceneSystem>
  {
    private static MethodInfo BuildSpaceTrackingVesselList { get; } = typeof (SpaceTracking).GetMethod("buildVesselsList", AccessTools.all);

    private static KscSceneEvents KscSceneEvents { get; } = new KscSceneEvents();

    public override string SystemName { get; } = nameof (KscSceneSystem);

    protected override void OnEnabled()
    {
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnLockRelease)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Add(new EventData<GameScenes>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnSceneRequested)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Add(new EventData<Vessel>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnVesselCreated)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Add(new EventData<Vessel, bool>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.onVesselRename.Add(new EventData<GameEvents.HostedFromToAction<Vessel, string>>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnVesselRename)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.FixedUpdate, new Action(KscSceneSystem.IncreaseTimeWhileInEditor)));
    }

    protected override void OnDisabled()
    {
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Remove(new EventData<LockDefinition>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnLockRelease)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Remove(new EventData<GameScenes>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnSceneRequested)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Remove(new EventData<Vessel>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnVesselCreated)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Remove(new EventData<Vessel, bool>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.onVesselRename.Remove(new EventData<GameEvents.HostedFromToAction<Vessel, string>>.OnEvent((object) KscSceneSystem.KscSceneEvents, __methodptr(OnVesselRename)));
    }

    private static void IncreaseTimeWhileInEditor()
    {
      if (HighLogic.LoadedSceneHasPlanetarium || HighLogic.LoadedScene < 5)
        return;
      Planetarium.fetch.time += (double) Time.fixedDeltaTime;
      HighLogic.CurrentGame.flightState.universalTime = Planetarium.fetch.time;
    }

    public void RefreshTrackingStationVessels()
    {
      if (HighLogic.LoadedScene != 8)
        return;
      SpaceTracking objectOfType = Object.FindObjectOfType<SpaceTracking>();
      if (Object.op_Inequality((Object) objectOfType, (Object) null))
        KscSceneSystem.BuildSpaceTrackingVesselList?.Invoke((object) objectOfType, (object[]) null);
    }
  }
}
