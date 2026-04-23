// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Connection.ConnectionWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpClient.Windows.Options;
using LmpClient.Windows.ServerList;
using LmpCommon;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Windows.Connection
{
  public class ConnectionWindow : Window<ConnectionWindow>
  {
    private static int _selectedIndex;
    protected const float WindowHeight = 400f;
    protected const float WindowWidth = 400f;
    private readonly string _title = string.Format("Luna Multiplayer {0} PID: {1}", (object) LmpVersioning.CurrentVersion, (object) CommonUtil.ProcessId);

    protected override void DrawWindowContent(int windowId)
    {
      GUI.DragWindow(this.MoveRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      this.DrawPlayerNameSection();
      ConnectionWindow.DrawTopButtons();
      this.DrawCustomServers();
      GUILayout.Label(MainSystem.Singleton.Status, StyleLibrary.StatusStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndVertical();
    }

    private void DrawCustomServers()
    {
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.CustomServers, Array.Empty<GUILayoutOption>());
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, new GUILayoutOption[2]
      {
        GUILayout.Width(395f),
        GUILayout.Height(300f)
      });
      if (GUILayout.Button(StyleLibrary.PlusIcon, Array.Empty<GUILayoutOption>()))
      {
        SettingsSystem.CurrentSettings.Servers.Insert(0, new ServerEntry());
        SettingsSystem.SaveSettings();
      }
      for (int index = 0; index < SettingsSystem.CurrentSettings.Servers.Count; ++index)
      {
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        bool flag = GUILayout.Toggle(ConnectionWindow.SelectedIndex == index, SettingsSystem.CurrentSettings.Servers[index].Name, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
        if (GUILayout.Button(StyleLibrary.DeleteIcon, new GUILayoutOption[1]
        {
          GUILayout.Width(35f)
        }))
        {
          SettingsSystem.CurrentSettings.Servers.RemoveAt(ConnectionWindow.SelectedIndex);
          SettingsSystem.SaveSettings();
        }
        else
        {
          GUILayout.EndHorizontal();
          if (flag)
            this.DrawServerEntry(index);
        }
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }

    private void DrawServerEntry(int serverPos)
    {
      ConnectionWindow.SelectedIndex = serverPos;
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.Name, this.LabelOptions);
      string str1 = GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Name, Array.Empty<GUILayoutOption>());
      if (str1 != SettingsSystem.CurrentSettings.Servers[serverPos].Name)
      {
        SettingsSystem.CurrentSettings.Servers[serverPos].Name = str1;
        SettingsSystem.SaveSettings();
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.Address, this.LabelOptions);
      string str2 = GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Address, Array.Empty<GUILayoutOption>());
      if (str2 != SettingsSystem.CurrentSettings.Servers[serverPos].Address)
      {
        SettingsSystem.CurrentSettings.Servers[serverPos].Address = str2;
        SettingsSystem.SaveSettings();
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.Port, this.LabelOptions);
      int result;
      if (int.TryParse(GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Port.ToString(), Array.Empty<GUILayoutOption>()), out result) && result != SettingsSystem.CurrentSettings.Servers[serverPos].Port)
      {
        SettingsSystem.CurrentSettings.Servers[serverPos].Port = result;
        SettingsSystem.SaveSettings();
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.Password, this.LabelOptions);
      string str3 = GUILayout.PasswordField(SettingsSystem.CurrentSettings.Servers[serverPos].Password, '*', 30, Array.Empty<GUILayoutOption>());
      if (str3 != SettingsSystem.CurrentSettings.Servers[serverPos].Password)
      {
        SettingsSystem.CurrentSettings.Servers[serverPos].Password = str3;
        SettingsSystem.SaveSettings();
      }
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    private static void DrawTopButtons()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (MainSystem.NetworkState <= ClientState.Disconnected)
      {
        GUI.enabled = SettingsSystem.CurrentSettings.Servers.Count > ConnectionWindow.SelectedIndex && ConnectionWindow.SelectedIndex >= 0;
        if (GUILayout.Button(StyleLibrary.ConnectBigIcon, Array.Empty<GUILayoutOption>()))
          NetworkConnection.ConnectToServer(SettingsSystem.CurrentSettings.Servers[ConnectionWindow.SelectedIndex].Address, SettingsSystem.CurrentSettings.Servers[ConnectionWindow.SelectedIndex].Port, SettingsSystem.CurrentSettings.Servers[ConnectionWindow.SelectedIndex].Password);
      }
      else if (GUILayout.Button(StyleLibrary.DisconnectBigIcon, Array.Empty<GUILayoutOption>()))
        NetworkConnection.Disconnect("Cancelled connection to server");
      GUI.enabled = true;
      Window<OptionsWindow>.Singleton.Display = GUILayout.Toggle(Window<OptionsWindow>.Singleton.Display, StyleLibrary.SettingsBigIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      Window<ServerListWindow>.Singleton.Display = GUILayout.Toggle(Window<ServerListWindow>.Singleton.Display, StyleLibrary.ServerBigIcon, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }

    private void DrawPlayerNameSection()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ConnectionWindowText.PlayerName, this.LabelOptions);
      GUI.enabled = MainSystem.NetworkState <= ClientState.Disconnected;
      string str = GUILayout.TextArea(SettingsSystem.CurrentSettings.PlayerName, 32, Array.Empty<GUILayoutOption>());
      if (str != SettingsSystem.CurrentSettings.PlayerName)
      {
        SettingsSystem.CurrentSettings.PlayerName = str.Trim().Replace("\n", "");
        SettingsSystem.SaveSettings();
      }
      GUI.enabled = true;
      GUILayout.EndHorizontal();
    }

    private static int SelectedIndex
    {
      get => SettingsSystem.CurrentSettings.Servers.Count == 0 ? -1 : ConnectionWindow._selectedIndex;
      set => ConnectionWindow._selectedIndex = value;
    }

    public override bool Display => SettingsSystem.CurrentSettings.DisclaimerAccepted && MainSystem.ToolbarShowGui && HighLogic.LoadedScene == 2;

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width * 0.899999976158142 - 400.0), (float) ((double) Screen.height / 2.0 - 200.0), 400f, 400f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      StyleLibrary.StatusStyle = new GUIStyle(GUI.skin.label)
      {
        normal = {
          textColor = Color.yellow
        }
      };
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(400f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(400f);
      this.LayoutOptions[2] = GUILayout.MinHeight(400f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(400f);
      this.LabelOptions = new GUILayoutOption[1];
      this.LabelOptions[0] = GUILayout.Width(100f);
    }

    protected override void OnCloseButton()
    {
      base.OnCloseButton();
      MainSystem.ToolbarShowGui = false;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154306, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), this._title, Array.Empty<GUILayoutOption>()));
  }
}
