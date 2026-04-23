// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Options.OptionsWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Network;
using LmpClient.Systems.Mod;
using LmpClient.Systems.PlayerColorSys;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpClient.Windows.Status;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using UnityEngine;

namespace LmpClient.Windows.Options
{
  public class OptionsWindow : Window<OptionsWindow>
  {
    private const float WindowHeight = 400f;
    private const float WindowWidth = 300f;
    private const float UniverseConverterWindowHeight = 300f;
    private const float UniverseConverterWindowWidth = 200f;
    private static Color _tempColor = new Color(1f, 1f, 1f, 1f);
    private static GUIStyle _tempColorLabelStyle;
    private static bool _showGeneralSettings;
    private static bool _showBadNetworkSimulationSettings;
    private static bool _showAdvancedNetworkSettings;
    private static bool _showClockOffsetSettings;
    private static bool _infiniteTimeout;
    private static Rect _universeConverterWindowRect;
    private static GUILayoutOption[] _universeConverterLayoutOptions;
    private static bool _displayUniverseConverterDialog;
    private static bool _display;
    private readonly GUILayoutOption[] _smallOption = new GUILayoutOption[2]
    {
      GUILayout.Width(20f),
      GUILayout.ExpandWidth(false)
    };

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.OptionsWindowText.Language, Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(LocalizationContainer.GetCurrentLanguageAsText(), Array.Empty<GUILayoutOption>()))
      {
        LocalizationContainer.LoadLanguage(LocalizationContainer.GetNextLanguage());
        SettingsSystem.CurrentSettings.Language = LocalizationContainer.CurrentLanguage;
        SettingsSystem.SaveSettings();
      }
      GUILayout.EndHorizontal();
      GUILayout.Label(LocalizationContainer.OptionsWindowText.Color, Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(StyleLibrary.Skin.box, new GUILayoutOption[1]
      {
        GUILayout.ExpandWidth(true)
      });
      GUILayout.Label(SettingsSystem.CurrentSettings.PlayerName, OptionsWindow._tempColorLabelStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.OptionsWindowText.Red, this._smallOption);
      OptionsWindow._tempColor.r = GUILayout.HorizontalScrollbar(OptionsWindow._tempColor.r, 0.0f, 0.0f, 1f, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.OptionsWindowText.Green, this._smallOption);
      OptionsWindow._tempColor.g = GUILayout.HorizontalScrollbar(OptionsWindow._tempColor.g, 0.0f, 0.0f, 1f, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.OptionsWindowText.Blue, this._smallOption);
      OptionsWindow._tempColor.b = GUILayout.HorizontalScrollbar(OptionsWindow._tempColor.b, 0.0f, 0.0f, 1f, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      OptionsWindow._tempColorLabelStyle.fontStyle = (FontStyle) 1;
      OptionsWindow._tempColorLabelStyle.fontSize = 40;
      OptionsWindow._tempColorLabelStyle.alignment = (TextAnchor) 4;
      OptionsWindow._tempColorLabelStyle.active.textColor = OptionsWindow._tempColor;
      OptionsWindow._tempColorLabelStyle.normal.textColor = OptionsWindow._tempColor;
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(LocalizationContainer.OptionsWindowText.Random, Array.Empty<GUILayoutOption>()))
        OptionsWindow._tempColor = PlayerColorSystem.GenerateRandomColor();
      if (GUILayout.Button(LocalizationContainer.OptionsWindowText.Set, Array.Empty<GUILayoutOption>()))
      {
        Window<StatusWindow>.Singleton.ColorEventHandled = false;
        SettingsSystem.CurrentSettings.PlayerColor = OptionsWindow._tempColor;
        SettingsSystem.SaveSettings();
        if (MainSystem.NetworkState == ClientState.Running)
          LmpClient.Base.System<PlayerColorSystem>.Singleton.MessageSender.SendPlayerColorToServer();
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(10f);
      if (GUILayout.Button(LocalizationContainer.OptionsWindowText.GenerateLmpModControl, Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<ModSystem>.Singleton.GenerateModControlFile(false);
      if (GUILayout.Button(LocalizationContainer.OptionsWindowText.GenerateLmpModControl + " + SHA", Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<ModSystem>.Singleton.GenerateModControlFile(true);
      OptionsWindow._displayUniverseConverterDialog = GUILayout.Toggle(OptionsWindow._displayUniverseConverterDialog, LocalizationContainer.OptionsWindowText.GenerateUniverse, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      GUILayout.Space(10f);
      OptionsWindow.DrawGeneralSettings();
      OptionsWindow.DrawNetworkSettings();
      OptionsWindow.DrawAdvancedDebugOptions();
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
    }

    private static void DrawGeneralSettings()
    {
      OptionsWindow._showGeneralSettings = GUILayout.Toggle(OptionsWindow._showGeneralSettings, LocalizationContainer.OptionsWindowText.GeneralSettings, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (!OptionsWindow._showGeneralSettings)
        return;
      GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.OptionsWindowText.ChatBuffer, (object) SettingsSystem.CurrentSettings.ChatBuffer), Array.Empty<GUILayoutOption>());
      GUI.enabled = MainSystem.NetworkState <= ClientState.Disconnected;
      int num = (int) Math.Round((double) GUILayout.HorizontalScrollbar((float) SettingsSystem.CurrentSettings.ChatBuffer, 10f, 1f, 500f, Array.Empty<GUILayoutOption>()));
      if (num != SettingsSystem.CurrentSettings.ChatBuffer)
      {
        SettingsSystem.CurrentSettings.ChatBuffer = num;
        SettingsSystem.SaveSettings();
      }
      GUI.enabled = true;
      bool flag = GUILayout.Toggle(SettingsSystem.CurrentSettings.IgnoreSyncChecks, LocalizationContainer.OptionsWindowText.IgnoreSyncChecks, Array.Empty<GUILayoutOption>());
      if (flag != SettingsSystem.CurrentSettings.IgnoreSyncChecks)
      {
        SettingsSystem.CurrentSettings.IgnoreSyncChecks = flag;
        SettingsSystem.SaveSettings();
      }
    }

    private static void DrawNetworkSettings()
    {
      OptionsWindow._showAdvancedNetworkSettings = GUILayout.Toggle(OptionsWindow._showAdvancedNetworkSettings, LocalizationContainer.OptionsWindowText.NetworkSettings, StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (!OptionsWindow._showAdvancedNetworkSettings)
        return;
      if (MainSystem.NetworkState > ClientState.Disconnected)
        GUILayout.Label(LocalizationContainer.OptionsWindowText.CannotChangeWhileConnected, Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.OptionsWindowText.Mtu, (object) NetworkMain.Config.MaximumTransmissionUnit), Array.Empty<GUILayoutOption>());
      if (MainSystem.NetworkState <= ClientState.Disconnected)
      {
        if ((uint) NetworkMain.ClientConnection.Status > 0U)
        {
          if (GUILayout.Button(LocalizationContainer.OptionsWindowText.ResetNetwork, Array.Empty<GUILayoutOption>()))
            NetworkMain.ResetNetworkSystem();
        }
        else
        {
          int num = (int) Math.Round((double) GUILayout.HorizontalScrollbar((float) SettingsSystem.CurrentSettings.Mtu, 0.0f, 1f, 8191f, Array.Empty<GUILayoutOption>()));
          if (num != SettingsSystem.CurrentSettings.Mtu)
          {
            NetworkMain.Config.MaximumTransmissionUnit = SettingsSystem.CurrentSettings.Mtu = num;
            SettingsSystem.SaveSettings();
          }
          bool flag = GUILayout.Toggle(SettingsSystem.CurrentSettings.AutoExpandMtu, LocalizationContainer.OptionsWindowText.AutoExpandMtu, Array.Empty<GUILayoutOption>());
          if (flag != SettingsSystem.CurrentSettings.AutoExpandMtu)
          {
            NetworkMain.Config.AutoExpandMTU = SettingsSystem.CurrentSettings.AutoExpandMtu = flag;
            SettingsSystem.SaveSettings();
          }
        }
      }
      GUILayout.Label((double) SettingsSystem.CurrentSettings.TimeoutSeconds == 3.40282346638529E+38 ? LocalizationContainer.OptionsWindowText.ConnectionTimeout + " ∞" : string.Format("{0} {1} sec", (object) LocalizationContainer.OptionsWindowText.ConnectionTimeout, (object) NetworkMain.Config.ConnectionTimeout), Array.Empty<GUILayoutOption>());
      if (MainSystem.NetworkState <= ClientState.Disconnected)
      {
        OptionsWindow._infiniteTimeout = (double) SettingsSystem.CurrentSettings.TimeoutSeconds == 3.40282346638529E+38;
        GUI.enabled = !OptionsWindow._infiniteTimeout;
        int num = (int) Math.Round((double) GUILayout.HorizontalScrollbar(SettingsSystem.CurrentSettings.TimeoutSeconds, 0.0f, NetworkMain.Config.PingInterval, 120f, Array.Empty<GUILayoutOption>()));
        if ((double) num != (double) SettingsSystem.CurrentSettings.TimeoutSeconds)
        {
          NetworkMain.Config.ConnectionTimeout = SettingsSystem.CurrentSettings.TimeoutSeconds = (float) num;
          SettingsSystem.SaveSettings();
        }
        GUI.enabled = true;
        OptionsWindow._infiniteTimeout = GUILayout.Toggle(OptionsWindow._infiniteTimeout, "∞", Array.Empty<GUILayoutOption>());
        if (OptionsWindow._infiniteTimeout)
        {
          NetworkMain.Config.ConnectionTimeout = SettingsSystem.CurrentSettings.TimeoutSeconds = float.MaxValue;
          SettingsSystem.SaveSettings();
        }
      }
    }

    private static void DrawAdvancedDebugOptions()
    {
      GUILayout.Label("Debug settings", Array.Empty<GUILayoutOption>());
      if (GUILayout.Button("Check Common.dll stock parts", Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<ModSystem>.Singleton.CheckCommonStockParts();
      if (GUILayout.Button("Regenerate translation files", Array.Empty<GUILayoutOption>()))
        LocalizationContainer.RegenerateTranslations();
      GUILayout.Space(10f);
      OptionsWindow._showBadNetworkSimulationSettings = GUILayout.Toggle(OptionsWindow._showBadNetworkSimulationSettings, "Bad network simulation", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (OptionsWindow._showBadNetworkSimulationSettings)
      {
        if (MainSystem.NetworkState <= ClientState.Disconnected)
        {
          GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
          if (GUILayout.Button("Random", Array.Empty<GUILayoutOption>()))
            NetworkMain.RandomizeBadConnectionValues();
          if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
            NetworkMain.ResetBadConnectionValues();
          GUILayout.EndHorizontal();
        }
        if (MainSystem.NetworkState > ClientState.Disconnected)
          GUILayout.Label("Cannot change values while connected", Array.Empty<GUILayoutOption>());
        GUILayout.Label(string.Format("Packet loss: {0:F1}%", (object) (float) ((double) NetworkMain.Config.SimulatedLoss * 100.0)), Array.Empty<GUILayoutOption>());
        if (MainSystem.NetworkState <= ClientState.Disconnected)
          NetworkMain.Config.SimulatedLoss = (float) Math.Round((double) GUILayout.HorizontalScrollbar(NetworkMain.Config.SimulatedLoss, 0.0f, 0.0f, 1f, Array.Empty<GUILayoutOption>()), 3);
        GUILayout.Label(string.Format("Packet duplication: {0:F1}%", (object) (float) ((double) NetworkMain.Config.SimulatedDuplicatesChance * 100.0)), Array.Empty<GUILayoutOption>());
        if (MainSystem.NetworkState <= ClientState.Disconnected)
          NetworkMain.Config.SimulatedDuplicatesChance = (float) Math.Round((double) GUILayout.HorizontalScrollbar(NetworkMain.Config.SimulatedDuplicatesChance, 0.0f, 0.0f, 1f, Array.Empty<GUILayoutOption>()), 3);
        GUILayout.Label(string.Format("Max random latency: {0:F1} ms", (object) (float) ((double) NetworkMain.Config.SimulatedRandomLatency * 1000.0)), Array.Empty<GUILayoutOption>());
        if (MainSystem.NetworkState <= ClientState.Disconnected)
          NetworkMain.Config.SimulatedRandomLatency = (float) Math.Round((double) GUILayout.HorizontalScrollbar(NetworkMain.Config.SimulatedRandomLatency, 0.0f, 0.0f, 3f, Array.Empty<GUILayoutOption>()), 4);
        GUILayout.Label(string.Format("Min latency: {0:F1} ms", (object) (float) ((double) NetworkMain.Config.SimulatedMinimumLatency * 1000.0)), Array.Empty<GUILayoutOption>());
        if (MainSystem.NetworkState <= ClientState.Disconnected)
          NetworkMain.Config.SimulatedMinimumLatency = (float) Math.Round((double) GUILayout.HorizontalScrollbar(NetworkMain.Config.SimulatedMinimumLatency, 0.0f, 0.0f, 3f, Array.Empty<GUILayoutOption>()), 4);
      }
      OptionsWindow._showClockOffsetSettings = GUILayout.Toggle(OptionsWindow._showClockOffsetSettings, "Clock offset simulation", StyleLibrary.ToggleButtonStyle, Array.Empty<GUILayoutOption>());
      if (!OptionsWindow._showClockOffsetSettings)
        return;
      GUILayout.Label(string.Format("Computer clock offset: {0:F1} min", (object) LunaComputerTime.SimulatedMinutesTimeOffset), Array.Empty<GUILayoutOption>());
      LunaComputerTime.SimulatedMinutesTimeOffset = (float) Math.Round((double) GUILayout.HorizontalScrollbar(LunaComputerTime.SimulatedMinutesTimeOffset, 0.0f, -15f, 15f, Array.Empty<GUILayoutOption>()), 3);
      GUILayout.Label(string.Format("NTP server time offset: {0:F1} ms", (object) LunaNetworkTime.SimulatedMsTimeOffset), Array.Empty<GUILayoutOption>());
      LunaNetworkTime.SimulatedMsTimeOffset = (float) Math.Round((double) GUILayout.HorizontalScrollbar(LunaNetworkTime.SimulatedMsTimeOffset, 0.0f, -2500f, 2500f, Array.Empty<GUILayoutOption>()), 3);
    }

    private void DrawUniverseConverterDialog(int windowId)
    {
      this.DrawCloseButton((Action) (() => OptionsWindow._displayUniverseConverterDialog = false), OptionsWindow._universeConverterWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, Array.Empty<GUILayoutOption>());
      foreach (string savedName in UniverseConverter.GetSavedNames())
      {
        if (GUILayout.Button(savedName, Array.Empty<GUILayoutOption>()))
          UniverseConverter.GenerateUniverse(savedName);
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && OptionsWindow._display;
      set
      {
        if (!OptionsWindow._display & value)
          OptionsWindow._tempColor = SettingsSystem.CurrentSettings.PlayerColor;
        base.Display = OptionsWindow._display = value;
      }
    }

    protected override void DrawGui()
    {
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154315, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.OptionsWindowText.Title, this.LayoutOptions));
      if (!OptionsWindow._displayUniverseConverterDialog)
        return;
      // ISSUE: method pointer
      OptionsWindow._universeConverterWindowRect = this.FixWindowPos(GUILayout.Window(1664154316, OptionsWindow._universeConverterWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawUniverseConverterDialog)), "Universe converter", OptionsWindow._universeConverterLayoutOptions));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width / 2.0 - 150.0), (float) ((double) Screen.height / 2.0 - 200.0), 300f, 400f);
      OptionsWindow._universeConverterWindowRect = new Rect((float) Screen.width * 0.025f, (float) Screen.height * 0.025f, 300f, 400f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.Width(300f);
      this.LayoutOptions[1] = GUILayout.Height(400f);
      this.LayoutOptions[2] = GUILayout.ExpandWidth(true);
      this.LayoutOptions[3] = GUILayout.ExpandHeight(true);
      OptionsWindow._universeConverterLayoutOptions = new GUILayoutOption[4];
      OptionsWindow._universeConverterLayoutOptions[0] = GUILayout.Width(200f);
      OptionsWindow._universeConverterLayoutOptions[1] = GUILayout.Height(300f);
      OptionsWindow._universeConverterLayoutOptions[2] = GUILayout.ExpandWidth(true);
      OptionsWindow._universeConverterLayoutOptions[3] = GUILayout.ExpandHeight(true);
      OptionsWindow._tempColor = new Color();
      OptionsWindow._tempColorLabelStyle = new GUIStyle(GUI.skin.label);
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_OptionsLock");
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
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_OptionsLock");
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
