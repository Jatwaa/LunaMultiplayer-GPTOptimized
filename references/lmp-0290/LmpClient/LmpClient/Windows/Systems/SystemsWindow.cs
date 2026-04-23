// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Systems.SystemsWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.CraftLibrary;
using LmpClient.Systems.Facility;
using LmpClient.Systems.Flag;
using LmpClient.Systems.Groups;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.KscScene;
using LmpClient.Systems.Lock;
using LmpClient.Systems.Mod;
using LmpClient.Systems.PlayerColorSys;
using LmpClient.Systems.PlayerConnection;
using LmpClient.Systems.Scenario;
using LmpClient.Systems.ShareAchievements;
using LmpClient.Systems.ShareContracts;
using LmpClient.Systems.ShareFunds;
using LmpClient.Systems.ShareReputation;
using LmpClient.Systems.ShareScience;
using LmpClient.Systems.ShareScienceSubject;
using LmpClient.Systems.ShareStrategy;
using LmpClient.Systems.ShareTechnology;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselActionGroupSys;
using LmpClient.Systems.VesselCrewSys;
using LmpClient.Systems.VesselEvaEditorSys;
using LmpClient.Systems.VesselFairingsSys;
using LmpClient.Systems.VesselFlightStateSys;
using LmpClient.Systems.VesselImmortalSys;
using LmpClient.Systems.VesselLockSys;
using LmpClient.Systems.VesselPartSyncCallSys;
using LmpClient.Systems.VesselPartSyncFieldSys;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Systems.VesselResourceSys;
using LmpClient.Systems.VesselSwitcherSys;
using LmpClient.Systems.VesselUpdateSys;
using LmpClient.Systems.Warp;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Windows.Systems
{
  public class SystemsWindow : Window<SystemsWindow>
  {
    private static bool _display;
    private const float WindowHeight = 400f;
    private const float WindowWidth = 200f;

    protected override void DrawWindowContent(int windowId)
    {
      GUI.DragWindow(this.MoveRect);
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, new GUILayoutOption[2]
      {
        GUILayout.Width(200f),
        GUILayout.Height(400f)
      });
      SystemsWindow.PrintSystemButtons();
      GUILayout.EndScrollView();
    }

    private static void PrintSystemButtons()
    {
      LmpClient.Base.System<CraftLibrarySystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<CraftLibrarySystem>.Singleton.Enabled, LmpClient.Base.System<CraftLibrarySystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<FacilitySystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<FacilitySystem>.Singleton.Enabled, LmpClient.Base.System<FacilitySystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<FlagSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<FlagSystem>.Singleton.Enabled, LmpClient.Base.System<FlagSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<KscSceneSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<KscSceneSystem>.Singleton.Enabled, LmpClient.Base.System<KscSceneSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<GroupSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<GroupSystem>.Singleton.Enabled, LmpClient.Base.System<GroupSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<KerbalSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<KerbalSystem>.Singleton.Enabled, LmpClient.Base.System<KerbalSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<LockSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<LockSystem>.Singleton.Enabled, LmpClient.Base.System<LockSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ModSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ModSystem>.Singleton.Enabled, LmpClient.Base.System<ModSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<PlayerColorSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<PlayerColorSystem>.Singleton.Enabled, LmpClient.Base.System<PlayerColorSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<PlayerConnectionSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<PlayerConnectionSystem>.Singleton.Enabled, LmpClient.Base.System<PlayerConnectionSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ScenarioSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ScenarioSystem>.Singleton.Enabled, LmpClient.Base.System<ScenarioSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<TimeSyncSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<TimeSyncSystem>.Singleton.Enabled, LmpClient.Base.System<TimeSyncSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<WarpSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<WarpSystem>.Singleton.Enabled, LmpClient.Base.System<WarpSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselCrewSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselCrewSystem>.Singleton.Enabled, LmpClient.Base.System<VesselCrewSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselFlightStateSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselFlightStateSystem>.Singleton.Enabled, LmpClient.Base.System<VesselFlightStateSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselImmortalSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselImmortalSystem>.Singleton.Enabled, LmpClient.Base.System<VesselImmortalSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselLockSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselLockSystem>.Singleton.Enabled, LmpClient.Base.System<VesselLockSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselPositionSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselPositionSystem>.Singleton.Enabled, LmpClient.Base.System<VesselPositionSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselUpdateSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselUpdateSystem>.Singleton.Enabled, LmpClient.Base.System<VesselUpdateSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselPartSyncFieldSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselPartSyncFieldSystem>.Singleton.Enabled, LmpClient.Base.System<VesselPartSyncFieldSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselPartSyncCallSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselPartSyncCallSystem>.Singleton.Enabled, LmpClient.Base.System<VesselPartSyncCallSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselResourceSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselResourceSystem>.Singleton.Enabled, LmpClient.Base.System<VesselResourceSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselActionGroupSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselActionGroupSystem>.Singleton.Enabled, LmpClient.Base.System<VesselActionGroupSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselFairingsSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselFairingsSystem>.Singleton.Enabled, LmpClient.Base.System<VesselFairingsSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselProtoSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselProtoSystem>.Singleton.Enabled, LmpClient.Base.System<VesselProtoSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselRemoveSystem>.Singleton.Enabled, LmpClient.Base.System<VesselRemoveSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselSwitcherSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselSwitcherSystem>.Singleton.Enabled, LmpClient.Base.System<VesselSwitcherSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<VesselEvaEditorSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<VesselEvaEditorSystem>.Singleton.Enabled, LmpClient.Base.System<VesselEvaEditorSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareFundsSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareFundsSystem>.Singleton.Enabled, LmpClient.Base.System<ShareFundsSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareScienceSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareScienceSystem>.Singleton.Enabled, LmpClient.Base.System<ShareScienceSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.Enabled, LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareReputationSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareReputationSystem>.Singleton.Enabled, LmpClient.Base.System<ShareReputationSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareTechnologySystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareTechnologySystem>.Singleton.Enabled, LmpClient.Base.System<ShareTechnologySystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareContractsSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareContractsSystem>.Singleton.Enabled, LmpClient.Base.System<ShareContractsSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareAchievementsSystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareAchievementsSystem>.Singleton.Enabled, LmpClient.Base.System<ShareAchievementsSystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<ShareStrategySystem>.Singleton.Enabled = GUILayout.Toggle(LmpClient.Base.System<ShareStrategySystem>.Singleton.Enabled, LmpClient.Base.System<ShareStrategySystem>.Singleton.SystemName, Array.Empty<GUILayoutOption>());
    }

    public override bool Display
    {
      get => base.Display && SystemsWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set => base.Display = SystemsWindow._display = value;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154320, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), "Systems", this.LayoutOptions));

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) Screen.width - 250f, (float) ((double) Screen.height / 2.0 - 200.0), 200f, 400f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(200f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(200f);
      this.LayoutOptions[2] = GUILayout.MinHeight(400f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(400f);
      this.TextAreaOptions = new GUILayoutOption[1];
      this.TextAreaOptions[0] = GUILayout.ExpandWidth(true);
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_SystemsWindowsLock");
    }

    public override void CheckWindowLock()
    {
      if (this.Display)
      {
        if (MainSystem.NetworkState < ClientState.Running || HighLogic.LoadedSceneIsFlight)
        {
          this.RemoveWindowLock();
          return;
        }
        Vector2 vector2 = Vector2.op_Implicit(Input.mousePosition);
        vector2.y = (float) Screen.height - vector2.y;
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_SystemsWindowsLock");
          this.IsWindowLocked = true;
        }
        if (!flag && this.IsWindowLocked)
          this.RemoveWindowLock();
      }
      if (this.Display || !this.IsWindowLocked)
        return;
      this.RemoveWindowLock();
    }
  }
}
