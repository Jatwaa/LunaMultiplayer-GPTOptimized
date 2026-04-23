// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Status.StatusSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SafetyBubble;
using LmpClient.Systems.SettingsSys;
using LmpClient.VesselUtilities;
using LmpCommon;
using System;
using System.Collections.Concurrent;
using System.Text;
using UnityEngine;

namespace LmpClient.Systems.Status
{
  public class StatusSystem : MessageSystem<StatusSystem, StatusMessageSender, StatusMessageHandler>
  {
    private static readonly StringBuilder StrBuilder = new StringBuilder();

    public PlayerStatus MyPlayerStatus { get; } = new PlayerStatus();

    public ConcurrentDictionary<string, PlayerStatus> PlayerStatusList { get; } = new ConcurrentDictionary<string, PlayerStatus>();

    private PlayerStatus LastPlayerStatus { get; } = new PlayerStatus();

    private bool StatusIsDifferent => this.MyPlayerStatus.VesselText != this.LastPlayerStatus.VesselText || this.MyPlayerStatus.StatusText != this.LastPlayerStatus.StatusText;

    public override string SystemName { get; } = nameof (StatusSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.MyPlayerStatus.PlayerName = SettingsSystem.CurrentSettings.PlayerName;
      this.MyPlayerStatus.StatusText = this.LastPlayerStatus.StatusText = StatusTexts.Syncing;
      this.MyPlayerStatus.VesselText = this.LastPlayerStatus.VesselText = string.Empty;
      this.MessageSender.SendOwnStatus();
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.CheckPlayerStatus)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.PlayerStatusList.Clear();
    }

    public int GetPlayerCount() => this.PlayerStatusList.Count;

    public PlayerStatus GetPlayerStatus(string playerName) => playerName == SettingsSystem.CurrentSettings.PlayerName ? this.MyPlayerStatus : (this.PlayerStatusList.ContainsKey(playerName) ? this.PlayerStatusList[playerName] : (PlayerStatus) null);

    public void RemovePlayer(string playerToRemove)
    {
      if (!this.PlayerStatusList.ContainsKey(playerToRemove))
        return;
      this.PlayerStatusList.TryRemove(playerToRemove, out PlayerStatus _);
    }

    private void CheckPlayerStatus()
    {
      if (!this.Enabled || HighLogic.LoadedScene < 5 || HighLogic.LoadedScene > 8)
        return;
      this.MyPlayerStatus.VesselText = StatusSystem.GetVesselText();
      this.MyPlayerStatus.StatusText = this.GetStatusText();
      if (this.StatusIsDifferent)
      {
        this.LastPlayerStatus.VesselText = this.MyPlayerStatus.VesselText;
        this.LastPlayerStatus.StatusText = this.MyPlayerStatus.StatusText;
        this.MessageSender.SendOwnStatus();
      }
    }

    private static string GetVesselText() => VesselCommon.IsSpectating || !Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) ? string.Empty : FlightGlobals.ActiveVessel.vesselName;

    private static string GetCurrentShipStatus()
    {
      if (LmpClient.Base.System<SafetyBubbleSystem>.Singleton.IsInSafetyBubble(FlightGlobals.ActiveVessel))
        return StatusTexts.InsideSafetyBubble;
      StatusSystem.StrBuilder.Length = 0;
      Vessel.Situations situation = FlightGlobals.ActiveVessel.situation;
      if (situation <= 16)
      {
        switch (situation - 1)
        {
          case 0:
            return StatusSystem.StrBuilder.Append(StatusTexts.Landed).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
          case 1:
            return StatusSystem.StrBuilder.Append(StatusTexts.Splashed).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
          case 2:
            break;
          case 3:
            return StatusSystem.StrBuilder.Append(StatusTexts.Launching).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
          default:
            if (situation == 8)
              return StatusSystem.StrBuilder.Append(StatusTexts.Flying).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
            if (situation == 16)
              return FlightGlobals.ActiveVessel.verticalSpeed > 0.0 ? StatusSystem.StrBuilder.Append(StatusTexts.Ascending).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString() : StatusSystem.StrBuilder.Append(StatusTexts.Descending).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
            break;
        }
      }
      else
      {
        if (situation == 32)
          return StatusSystem.StrBuilder.Append(StatusTexts.Orbiting).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
        if (situation != 64)
        {
          if (situation == 128)
            return StatusSystem.StrBuilder.Append(StatusTexts.Docked).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
        }
        else
          return FlightGlobals.ActiveVessel.orbit.timeToPe < 0.0 ? StatusSystem.StrBuilder.Append(StatusTexts.Escaping).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString() : StatusSystem.StrBuilder.Append(StatusTexts.Encountering).Append(' ').Append(FlightGlobals.ActiveVessel.mainBody.bodyName).ToString();
      }
      return StatusTexts.Error;
    }

    private static string GetSpectatingShipStatus()
    {
      if (LockSystem.LockQuery.ControlLockBelongsToPlayer(FlightGlobals.ActiveVessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return StatusTexts.WaitingControl;
      StatusSystem.StrBuilder.Length = 0;
      return StatusSystem.StrBuilder.Append(StatusTexts.Spectating).Append(' ').Append(LockSystem.LockQuery.GetControlLockOwner(FlightGlobals.ActiveVessel.id)).ToString();
    }

    private string GetStatusText()
    {
      switch (HighLogic.LoadedScene - 5)
      {
        case 0:
          return StatusTexts.SpaceCenter;
        case 1:
          EditorFacility editorFacility = EditorDriver.editorFacility;
          if (editorFacility == 1)
            return StatusTexts.BuildingVab;
          if (editorFacility == 2)
            return StatusTexts.BuildingSph;
          break;
        case 2:
          if (Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null))
            return !VesselCommon.IsSpectating ? StatusSystem.GetCurrentShipStatus() : StatusSystem.GetSpectatingShipStatus();
          break;
        case 3:
          return StatusTexts.TrackStation;
      }
      return this.MyPlayerStatus.StatusText;
    }
  }
}
