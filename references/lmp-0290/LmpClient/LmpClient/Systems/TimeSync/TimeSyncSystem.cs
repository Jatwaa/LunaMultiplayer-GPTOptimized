// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.TimeSync.TimeSyncSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.Warp;
using LmpCommon.Locks;
using LmpCommon.Time;
using System;
using UniLinq;
using UnityEngine;
using UnityEngine.Profiling;

namespace LmpClient.Systems.TimeSync
{
  public class TimeSyncSystem : LmpClient.Base.System<TimeSyncSystem>
  {
    private static double _universalTime;
    private const int MinPhysicsClockMsError = 25;
    private const float MinPhysicsClockRate = 0.85f;
    private const float MaxPhysicsClockRate = 1.2f;
    private const int MaxPhysicsClockMsError = 3500;

    public TimeSyncEvents TimerSyncEvents { get; } = new TimeSyncEvents();

    public static long ServerStartTime { get; set; }

    public static double UniversalTime
    {
      get => !MainSystem.IsUnityThread ? TimeSyncSystem._universalTime : Planetarium.GetUniversalTime();
      private set => TimeSyncSystem._universalTime = value;
    }

    public static double ServerClockSec => TimeUtil.TicksToSeconds((double) (LunaNetworkTime.UtcNow.Ticks - TimeSyncSystem.ServerStartTime));

    public static double CurrentErrorSec => TimeSyncSystem.UniversalTime - LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTime;

    private static bool CurrentlyWarping => LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspace == -1;

    private static bool CanSyncTime => HighLogic.LoadedScene - 5 <= 3;

    public override string SystemName { get; } = nameof (TimeSyncSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      TimingManager.FixedUpdateAdd((TimingManager.TimingStage) 2, new TimingManager.UpdateAction((object) null, __methodptr(UpdateUniversalTime)));
      // ISSUE: method pointer
      TimingManager.FixedUpdateAdd((TimingManager.TimingStage) 2, new TimingManager.UpdateAction((object) this, __methodptr(CheckGameTime)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Add(new EventVoid.OnEvent((object) this.TimerSyncEvents, __methodptr(OnStartSpectating)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      TimingManager.FixedUpdateRemove((TimingManager.TimingStage) 2, new TimingManager.UpdateAction((object) null, __methodptr(UpdateUniversalTime)));
      // ISSUE: method pointer
      TimingManager.FixedUpdateRemove((TimingManager.TimingStage) 2, new TimingManager.UpdateAction((object) this, __methodptr(CheckGameTime)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Remove(new EventVoid.OnEvent((object) this.TimerSyncEvents, __methodptr(OnStartSpectating)));
      TimeSyncSystem.ServerStartTime = 0L;
    }

    private void CheckGameTime()
    {
      Profiler.BeginSample(nameof (CheckGameTime));
      if (this.Enabled && !TimeSyncSystem.CurrentlyWarping && TimeSyncSystem.CanSyncTime && !LmpClient.Base.System<WarpSystem>.Singleton.WaitingSubspaceIdFromServer)
      {
        double currentSubspaceTime = LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTime;
        double milliseconds = TimeUtil.SecondsToMilliseconds(TimeSyncSystem.CurrentErrorSec);
        if (Math.Abs(milliseconds) < 25.0)
          UnityEngine.Time.timeScale = 1f;
        if (Math.Abs(milliseconds) > 25.0 && Math.Abs(milliseconds) < 3500.0)
          TimeSyncSystem.SkewClock();
        else if (Math.Abs(milliseconds) > 3500.0)
        {
          LunaLog.LogWarning(string.Format("[LMP] Adjusted time from: {0} to: {1} due to error: {2}", (object) TimeSyncSystem.UniversalTime, (object) currentSubspaceTime, (object) milliseconds));
          this.SetGameTime(currentSubspaceTime);
        }
      }
      Profiler.EndSample();
    }

    private static void UpdateUniversalTime() => TimeSyncSystem.UniversalTime = Planetarium.GetUniversalTime();

    public void ForceTimeSync()
    {
      if (!this.Enabled || TimeSyncSystem.CurrentlyWarping || !TimeSyncSystem.CanSyncTime || LmpClient.Base.System<WarpSystem>.Singleton.WaitingSubspaceIdFromServer)
        return;
      double currentSubspaceTime = LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTime;
      LunaLog.LogWarning(string.Format("FORCING a time sync from: {0} to: {1}. Error: {2}", (object) TimeSyncSystem.UniversalTime, (object) currentSubspaceTime, (object) TimeUtil.SecondsToMilliseconds(TimeSyncSystem.CurrentErrorSec)));
      this.SetGameTime(currentSubspaceTime);
    }

    public void SetGameTime(double targetTick)
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        foreach (Vessel vessel in Enumerable.Where<Vessel>(Enumerable.Select<LockDefinition, Vessel>(LockSystem.LockQuery.GetAllUnloadedUpdateLocks(SettingsSystem.CurrentSettings.PlayerName), (Func<LockDefinition, Vessel>) (l => FlightGlobals.FindVessel(l.VesselId))), (Func<Vessel, bool>) (v => Object.op_Inequality((Object) v, (Object) null))))
          vessel.AdvanceShipPosition(targetTick);
      }
      Planetarium.SetUniversalTime(targetTick);
    }

    private static void SkewClock() => UnityEngine.Time.timeScale = Mathf.Clamp((float) Math.Pow(2.0, -TimeSyncSystem.CurrentErrorSec), 0.85f, 1.2f);
  }
}
