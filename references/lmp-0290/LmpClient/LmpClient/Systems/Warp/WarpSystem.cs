// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.WarpSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using FinePrint.Utilities;
using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Localization;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.Warp
{
  public class WarpSystem : MessageSystem<WarpSystem, WarpMessageSender, WarpMessageHandler>
  {
    private static DateTime _stoppedWarpingTimeStamp;
    private int _currentSubspace = int.MinValue;

    public bool CurrentlyWarping => this.CurrentSubspace == -1;

    public WarpEntryDisplay WarpEntryDisplay { get; } = new WarpEntryDisplay();

    public int CurrentSubspace
    {
      get => this._currentSubspace;
      set
      {
        if (this._currentSubspace == value)
          return;
        this._currentSubspace = value;
        if (!this.ClientSubspaceList.ContainsKey(SettingsSystem.CurrentSettings.PlayerName))
          this.ClientSubspaceList.TryAdd(SettingsSystem.CurrentSettings.PlayerName, this._currentSubspace);
        else
          this.ClientSubspaceList[SettingsSystem.CurrentSettings.PlayerName] = this._currentSubspace;
        this.MessageSender.SendChangeSubspaceMsg(this._currentSubspace);
        if (this._currentSubspace > 0 && !this.SkipSubspaceProcess)
          this.ProcessNewSubspace();
        this.SkipSubspaceProcess = false;
        LunaLog.Log(string.Format("[LMP]: Locked to subspace {0}, time: {1}", (object) this._currentSubspace, (object) this.CurrentSubspaceTime));
      }
    }

    public ConcurrentDictionary<string, int> ClientSubspaceList { get; } = new ConcurrentDictionary<string, int>();

    public ConcurrentDictionary<int, double> Subspaces { get; } = new ConcurrentDictionary<int, double>();

    public int LatestSubspace => !Enumerable.Any<KeyValuePair<int, double>>((IEnumerable<KeyValuePair<int, double>>) this.Subspaces) ? 0 : Enumerable.First<KeyValuePair<int, double>>((IEnumerable<KeyValuePair<int, double>>) Enumerable.OrderByDescending<KeyValuePair<int, double>, double>((IEnumerable<KeyValuePair<int, double>>) this.Subspaces, (Func<KeyValuePair<int, double>, double>) (s => s.Value))).Key;

    private ScreenMessage WarpMessage { get; set; }

    private WarpEvents WarpEvents { get; } = new WarpEvents();

    public bool SkipSubspaceProcess { get; set; }

    public bool WaitingSubspaceIdFromServer { get; set; }

    public bool SyncedToLastSubspace { get; set; }

    public List<SubspaceDisplayEntry> SubspaceEntries { get; set; } = new List<SubspaceDisplayEntry>();

    public override string SystemName { get; } = nameof (WarpSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onTimeWarpRateChanged.Remove(new EventVoid.OnEvent((object) this.WarpEvents, __methodptr(OnTimeWarpChanged)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.WarpEvents, __methodptr(OnSceneChanged)));
      this.ClientSubspaceList.Clear();
      this.Subspaces.Clear();
      this.SubspaceEntries.Clear();
      this._currentSubspace = int.MinValue;
      this.SkipSubspaceProcess = false;
      this.WaitingSubspaceIdFromServer = false;
      this.SyncedToLastSubspace = false;
    }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onTimeWarpRateChanged.Add(new EventVoid.OnEvent((object) this.WarpEvents, __methodptr(OnTimeWarpChanged)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.WarpEvents, __methodptr(OnSceneChanged)));
      if ((uint) SettingsSystem.ServerSettings.WarpMode <= 0U)
        return;
      this.SetupRoutine(new RoutineDefinition(100, RoutineExecution.Update, new Action(this.CheckWarpStopped)));
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.WarpIfSpectatingToController)));
      this.SetupRoutine(new RoutineDefinition(5000, RoutineExecution.Update, new Action(this.CheckStuckAtWarp)));
    }

    private void WarpIfSpectatingToController()
    {
      if (!VesselCommon.IsSpectating)
        return;
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(FlightGlobals.ActiveVessel.id);
      if (!string.IsNullOrEmpty(controlLockOwner))
        this.WarpIfSubspaceIsMoreAdvanced(this.GetPlayerSubspace(controlLockOwner));
    }

    private void CheckStuckAtWarp()
    {
      if (this.CurrentSubspace != -1 || !this.WaitingSubspaceIdFromServer || !TimeUtil.IsInInterval(ref WarpSystem._stoppedWarpingTimeStamp, 15000))
        return;
      LunaLog.LogError("Detected stuck at warping! Requesting subspace ID again!");
      this.RequestNewSubspace();
    }

    private void CheckWarpStopped()
    {
      if (TimeWarp.CurrentRateIndex != 0 || (double) Math.Abs(TimeWarp.CurrentRate - 1f) >= 0.100000001490116 || this.CurrentSubspace != -1 || this.WaitingSubspaceIdFromServer)
        return;
      WarpEvent.onTimeWarpStopped.Fire();
      this.RequestNewSubspace();
    }

    public void SyncToSubspace(int subspaceId)
    {
      if (!WarpSystem.SafeToSync(subspaceId) && subspaceId > 0)
        this.DisplayMessage(LocalizationContainer.ScreenText.UnsafeToSync, 5f);
      else
        this.CurrentSubspace = subspaceId;
    }

    public bool WarpValidation()
    {
      if (SettingsSystem.ServerSettings.WarpMode == WarpMode.None)
      {
        this.DisplayMessage(LocalizationContainer.ScreenText.WarpDisabled, 5f);
        return false;
      }
      if (this.WaitingSubspaceIdFromServer && TimeWarp.CurrentRateIndex > 0)
      {
        this.DisplayMessage(LocalizationContainer.ScreenText.WaitingSubspace, 5f);
        return false;
      }
      if (!VesselCommon.IsSpectating || TimeWarp.CurrentRateIndex <= 0)
        return true;
      this.DisplayMessage(LocalizationContainer.ScreenText.CannotWarpWhileSpectating, 5f);
      return false;
    }

    public void WarpIfSubspaceIsMoreAdvanced(int newSubspace)
    {
      double num;
      if (newSubspace <= 0 || !this.Subspaces.TryGetValue(newSubspace, out num) || this.CurrentSubspaceTimeDifference >= num || this.CurrentSubspace == newSubspace)
        return;
      this.CurrentSubspace = newSubspace;
    }

    public bool PlayerIsInPastSubspace(string player)
    {
      if (!this.ClientSubspaceList.ContainsKey(player) || this.CurrentSubspace < 0)
        return false;
      int clientSubspace = this.ClientSubspaceList[player];
      return clientSubspace != -1 && clientSubspace != this.CurrentSubspace && this.Subspaces[clientSubspace] < this.Subspaces[this.CurrentSubspace];
    }

    public double CurrentSubspaceTime => this.GetSubspaceTime(this.CurrentSubspace);

    public double CurrentSubspaceTimeDifference
    {
      get
      {
        double num;
        return this.CurrentlyWarping ? TimeSyncSystem.UniversalTime - TimeSyncSystem.ServerClockSec : (this.Subspaces.TryGetValue(this.CurrentSubspace, out num) ? num : 0.0);
      }
    }

    public double GetSubspaceTime(int subspace) => this.Subspaces.ContainsKey(subspace) ? TimeSyncSystem.ServerClockSec + this.Subspaces[subspace] : 0.0;

    public int GetPlayerSubspace(string playerName) => this.ClientSubspaceList.ContainsKey(playerName) ? this.ClientSubspaceList[playerName] : 0;

    public void DisplayMessage(string messageText, float messageDuration)
    {
      if (this.WarpMessage != null)
        this.WarpMessage.duration = 0.0f;
      this.WarpMessage = LunaScreenMsg.PostScreenMessage(messageText, messageDuration, (ScreenMessageStyle) 0);
    }

    public void RemovePlayer(string playerName)
    {
      if (!this.ClientSubspaceList.ContainsKey(playerName))
        return;
      this.ClientSubspaceList.TryRemove(playerName, out int _);
    }

    public bool SubspaceIsEqualOrInThePast(int subspaceId)
    {
      if (!this.CurrentlyWarping && this.CurrentSubspace == subspaceId)
        return true;
      double num;
      return subspaceId != -1 && this.Subspaces.TryGetValue(subspaceId, out num) && this.CurrentSubspaceTimeDifference > num;
    }

    public bool SubspaceIsInThePast(int subspaceId)
    {
      double num;
      return !this.CurrentlyWarping && this.CurrentSubspace != subspaceId && subspaceId != -1 && this.Subspaces.TryGetValue(subspaceId, out num) && this.CurrentSubspaceTimeDifference > num;
    }

    public double GetTimeDifferenceWithGivenSubspace(int subspaceId)
    {
      if (subspaceId != -1)
      {
        if (subspaceId == this.CurrentSubspace)
          return 0.0;
        double num;
        if (this.Subspaces.TryGetValue(subspaceId, out num))
          return num - this.CurrentSubspaceTimeDifference;
      }
      return double.MaxValue;
    }

    public void ProcessNewSubspace()
    {
      LmpClient.Base.System<TimeSyncSystem>.Singleton.SetGameTime(this.CurrentSubspaceTime);
      WarpEvent.onTimeWarpStopped.Fire();
    }

    private static bool SafeToSync(int subspaceId)
    {
      if (SettingsSystem.CurrentSettings.IgnoreSyncChecks || HighLogic.LoadedScene != 7 || Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null))
        return true;
      if (VesselCommon.IsSpectating)
        return false;
      if (FlightGlobals.ActiveVessel.situation <= 8)
        return true;
      return FlightGlobals.ActiveVessel.orbit.eccentricity < 1.0 && CelestialUtilities.GetMinimumOrbitalDistance(FlightGlobals.ActiveVessel.mainBody, 1f) < FlightGlobals.ActiveVessel.orbit.PeR;
    }

    private void RequestNewSubspace()
    {
      this.WaitingSubspaceIdFromServer = true;
      this.MessageSender.SendNewSubspace();
      WarpSystem._stoppedWarpingTimeStamp = LunaComputerTime.UtcNow;
    }
  }
}
