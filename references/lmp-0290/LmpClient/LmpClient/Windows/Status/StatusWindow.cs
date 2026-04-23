// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Status.StatusWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Chat;
using LmpClient.Systems.CraftLibrary;
using LmpClient.Systems.PlayerColorSys;
using LmpClient.Systems.Screenshot;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.Status;
using LmpClient.Systems.Warp;
using LmpClient.Utilities;
using LmpClient.Windows.Admin;
using LmpClient.Windows.Chat;
using LmpClient.Windows.CraftLibrary;
using LmpClient.Windows.Debug;
using LmpClient.Windows.Options;
using LmpClient.Windows.Screenshots;
using LmpClient.Windows.Systems;
using LmpClient.Windows.Vessels;
using LmpCommon;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Windows.Status
{
  public class StatusWindow : Window<StatusWindow>
  {
    private static readonly WarpSystem WarpSystem = LmpClient.Base.System<WarpSystem>.Singleton;
    private static Vector2 _scrollPosition;
    private static GUIStyle _subspaceStyle;
    private static GUIStyle _subspaceListStyle;
    private static Dictionary<string, GUIStyle> _playerNameStyle;
    private static GUIStyle _stateTextStyle;
    private static GUIStyle _highlightStyle;
    private const float WindowHeight = 400f;
    private const float WindowWidth = 300f;
    private const float UpdateStatusInterval = 1f;
    private static double _lastStatusUpdate;
    private static readonly List<SubspaceDisplayEntry> SubspaceDisplay = new List<SubspaceDisplayEntry>();
    private static readonly string Title = "LMP - PID: " + CommonUtil.ProcessId;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.DrawTopButtons();
      StatusWindow.DrawSubspaces();
      StatusWindow.DrawDebugSection();
      StatusWindow.DrawBottomButtons();
      GUILayout.EndVertical();
    }

    private static void DrawBottomButtons()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(StyleLibrary.DisconnectIcon, Array.Empty<GUILayoutOption>()))
        MainSystem.Singleton.DisconnectFromGame();
      Window<OptionsWindow>.Singleton.Display = GUILayout.Toggle(Window<OptionsWindow>.Singleton.Display, StyleLibrary.SettingsIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }

    private void DrawTopButtons()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      Window<ChatWindow>.Singleton.Display = GUILayout.Toggle(Window<ChatWindow>.Singleton.Display, !LmpClient.Base.System<ChatSystem>.Singleton.NewMessageReceived || !this.Flash ? StyleLibrary.ChatIcon : StyleLibrary.ChatRedIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      Window<CraftLibraryWindow>.Singleton.Display = GUILayout.Toggle(Window<CraftLibraryWindow>.Singleton.Display, !LmpClient.Base.System<CraftLibrarySystem>.Singleton.NewContent || !this.Flash ? StyleLibrary.RocketIcon : StyleLibrary.RocketRedIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      Window<ScreenshotsWindow>.Singleton.Display = GUILayout.Toggle(Window<ScreenshotsWindow>.Singleton.Display, !LmpClient.Base.System<ScreenshotSystem>.Singleton.NewContent || !this.Flash ? StyleLibrary.CameraIcon : StyleLibrary.CameraRedIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (SettingsSystem.ServerSettings.AllowAdmin)
        Window<AdminWindow>.Singleton.Display = GUILayout.Toggle(Window<AdminWindow>.Singleton.Display, StyleLibrary.AdminIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }

    private static void DrawSubspaces()
    {
      StatusWindow._scrollPosition = GUILayout.BeginScrollView(StatusWindow._scrollPosition, StatusWindow._subspaceListStyle, new GUILayoutOption[1]
      {
        GUILayout.ExpandHeight(true)
      });
      for (int index1 = 0; index1 < StatusWindow.SubspaceDisplay.Count; ++index1)
      {
        GUILayout.BeginVertical(StatusWindow._subspaceStyle, new GUILayoutOption[1]
        {
          GUILayout.ExpandWidth(true)
        });
        GUILayout.BeginHorizontal(new GUILayoutOption[1]
        {
          GUILayout.ExpandWidth(true)
        });
        if (StatusWindow.SubspaceDisplay[index1].SubspaceId == -1)
        {
          GUILayout.Label("WARPING", StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        }
        else
        {
          GUILayout.Label(StatusTexts.GetTimeLabel(StatusWindow.SubspaceDisplay[index1]), Array.Empty<GUILayoutOption>());
          GUILayout.FlexibleSpace();
          if (StatusWindow.NotWarpingAndIsFutureSubspace(StatusWindow.SubspaceDisplay[index1].SubspaceId) && GUILayout.Button(StyleLibrary.SyncIcon, Array.Empty<GUILayoutOption>()))
            LmpClient.Base.System<WarpSystem>.Singleton.SyncToSubspace(StatusWindow.SubspaceDisplay[index1].SubspaceId);
        }
        GUILayout.EndHorizontal();
        for (int index2 = 0; index2 < StatusWindow.SubspaceDisplay[index1].Players.Count; ++index2)
          StatusWindow.DrawPlayerEntry(LmpClient.Base.System<StatusSystem>.Singleton.GetPlayerStatus(StatusWindow.SubspaceDisplay[index1].Players[index2]));
        GUILayout.EndVertical();
      }
      GUILayout.EndScrollView();
      if (Event.current.type != 6)
        return;
      Event.current.Use();
    }

    private static bool NotWarpingAndIsFutureSubspace(int subspaceId) => !StatusWindow.WarpSystem.CurrentlyWarping && StatusWindow.WarpSystem.CurrentSubspace != subspaceId && StatusWindow.WarpSystem.Subspaces.ContainsKey(StatusWindow.WarpSystem.CurrentSubspace) && StatusWindow.WarpSystem.Subspaces.ContainsKey(subspaceId) && StatusWindow.WarpSystem.Subspaces[StatusWindow.WarpSystem.CurrentSubspace] < StatusWindow.WarpSystem.Subspaces[subspaceId];

    private static void DrawPlayerEntry(PlayerStatus playerStatus)
    {
      if (playerStatus == null)
        return;
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (!StatusWindow._playerNameStyle.ContainsKey(playerStatus.PlayerName))
        StatusWindow._playerNameStyle[playerStatus.PlayerName] = new GUIStyle(GUI.skin.label)
        {
          normal = {
            textColor = LmpClient.Base.System<PlayerColorSystem>.Singleton.GetPlayerColor(playerStatus.PlayerName)
          },
          hover = {
            textColor = LmpClient.Base.System<PlayerColorSystem>.Singleton.GetPlayerColor(playerStatus.PlayerName)
          },
          active = {
            textColor = LmpClient.Base.System<PlayerColorSystem>.Singleton.GetPlayerColor(playerStatus.PlayerName)
          },
          fontStyle = (FontStyle) 1,
          stretchWidth = true,
          wordWrap = false
        };
      GUILayout.Label(playerStatus.PlayerName, StatusWindow._playerNameStyle[playerStatus.PlayerName], Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.Label(playerStatus.DisplayText, StatusWindow._stateTextStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }

    private static void DrawDebugSection()
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      Window<DebugWindow>.Singleton.Display = GUILayout.Toggle(Window<DebugWindow>.Singleton.Display, "Debug", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      Window<SystemsWindow>.Singleton.Display = GUILayout.Toggle(Window<SystemsWindow>.Singleton.Display, "Systems", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      Window<VesselsWindow>.Singleton.Display = GUILayout.Toggle(Window<VesselsWindow>.Singleton.Display, "Vessels", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      StatusWindow.DrawDebugSwitches();
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    private static void DrawDebugSwitches()
    {
      bool flag1 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug1, "D1", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag1 != SettingsSystem.CurrentSettings.Debug1)
      {
        SettingsSystem.CurrentSettings.Debug1 = flag1;
        SettingsSystem.SaveSettings();
      }
      bool flag2 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug2, "D2", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag2 != SettingsSystem.CurrentSettings.Debug2)
      {
        SettingsSystem.CurrentSettings.Debug2 = flag2;
        SettingsSystem.SaveSettings();
      }
      bool flag3 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug3, "D3", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag3 != SettingsSystem.CurrentSettings.Debug3)
      {
        SettingsSystem.CurrentSettings.Debug3 = flag3;
        SettingsSystem.SaveSettings();
      }
      bool flag4 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug4, "D4", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag4 != SettingsSystem.CurrentSettings.Debug4)
      {
        SettingsSystem.CurrentSettings.Debug4 = flag4;
        SettingsSystem.SaveSettings();
      }
      bool flag5 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug5, "D5", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag5 != SettingsSystem.CurrentSettings.Debug5)
      {
        SettingsSystem.CurrentSettings.Debug5 = flag5;
        SettingsSystem.SaveSettings();
      }
      bool flag6 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug6, "D6", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag6 != SettingsSystem.CurrentSettings.Debug6)
      {
        SettingsSystem.CurrentSettings.Debug6 = flag6;
        SettingsSystem.SaveSettings();
      }
      bool flag7 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug7, "D7", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag7 != SettingsSystem.CurrentSettings.Debug7)
      {
        SettingsSystem.CurrentSettings.Debug7 = flag7;
        SettingsSystem.SaveSettings();
      }
      bool flag8 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug8, "D8", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag8 != SettingsSystem.CurrentSettings.Debug8)
      {
        SettingsSystem.CurrentSettings.Debug8 = flag8;
        SettingsSystem.SaveSettings();
      }
      bool flag9 = GUILayout.Toggle(SettingsSystem.CurrentSettings.Debug9, "D9", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (flag9 == SettingsSystem.CurrentSettings.Debug9)
        return;
      SettingsSystem.CurrentSettings.Debug9 = flag9;
      SettingsSystem.SaveSettings();
    }

    public override bool Display => SettingsSystem.CurrentSettings.DisclaimerAccepted && MainSystem.ToolbarShowGui && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;

    public bool ColorEventHandled { get; set; } = true;

    protected override void OnCloseButton()
    {
      base.OnCloseButton();
      this.RemoveWindowLock();
      MainSystem.ToolbarShowGui = false;
    }

    protected override void DrawGui()
    {
      if (!this.ColorEventHandled)
      {
        StatusWindow._playerNameStyle = new Dictionary<string, GUIStyle>();
        this.ColorEventHandled = true;
      }
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154307, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), StatusWindow.Title, this.LayoutOptions));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width * 0.899999976158142 - 300.0), (float) ((double) Screen.height / 2.0 - 200.0), 300f, 400f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      StatusWindow._highlightStyle = new GUIStyle(GUI.skin.button)
      {
        normal = {
          textColor = XKCDColors.Red
        },
        active = {
          textColor = XKCDColors.Red
        },
        hover = {
          textColor = XKCDColors.KSPNotSoGoodOrange
        }
      };
      StatusWindow._subspaceStyle = new GUIStyle(StyleLibrary.Skin.box)
      {
        padding = new RectOffset(2, 2, 0, 0),
        margin = new RectOffset(0, 0, 0, 0)
      };
      StatusWindow._subspaceListStyle = new GUIStyle(StyleLibrary.Skin.scrollView)
      {
        padding = new RectOffset(0, 0, 0, 0),
        margin = new RectOffset(0, 0, 0, 0)
      };
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(300f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(300f);
      this.LayoutOptions[2] = GUILayout.MinHeight(400f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(400f);
      StatusWindow._playerNameStyle = new Dictionary<string, GUIStyle>();
      StatusWindow._stateTextStyle = new GUIStyle(GUI.skin.label)
      {
        normal = {
          textColor = XKCDColors.KSPNeutralUIGrey
        }
      };
      StatusWindow._stateTextStyle.hover.textColor = StatusWindow._stateTextStyle.normal.textColor;
      StatusWindow._stateTextStyle.active.textColor = StatusWindow._stateTextStyle.normal.textColor;
      StatusWindow._stateTextStyle.fontStyle = (FontStyle) 0;
      StatusWindow._stateTextStyle.fontSize = 12;
      StatusWindow._stateTextStyle.stretchWidth = true;
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display || (double) Time.realtimeSinceStartup - StatusWindow._lastStatusUpdate <= 1.0)
        return;
      StatusWindow._lastStatusUpdate = (double) Time.realtimeSinceStartup;
      StatusWindow.SubspaceDisplay.Clear();
      StatusWindow.SubspaceDisplay.AddRange((IEnumerable<SubspaceDisplayEntry>) LmpClient.Base.System<WarpSystem>.Singleton.WarpEntryDisplay.GetSubspaceDisplayEntries());
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_PlayerStatusLock");
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
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_PlayerStatusLock");
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
