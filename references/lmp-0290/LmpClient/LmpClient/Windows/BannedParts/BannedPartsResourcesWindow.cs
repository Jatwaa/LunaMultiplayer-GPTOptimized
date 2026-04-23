// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.BannedParts.BannedPartsResourcesWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Windows.BannedParts
{
  public class BannedPartsResourcesWindow : Window<BannedPartsResourcesWindow>
  {
    private const float WindowHeight = 300f;
    private const float WindowWidth = 400f;
    private static string[] _bannedParts = new string[0];
    private static string[] _bannedResources = new string[0];
    private static int _partCount = 0;
    private static string _vesselName;
    private static bool _display;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      if (BannedPartsResourcesWindow._partCount > 0)
      {
        GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.BannedPartsResourcesWindowText.TooManyParts, (object) SettingsSystem.ServerSettings.MaxVesselParts), StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
      }
      else
      {
        GUILayout.Label(BannedPartsResourcesWindow._vesselName + " " + LocalizationContainer.BannedPartsResourcesWindowText.Text, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.Space(5f);
        GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
        this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, Array.Empty<GUILayoutOption>());
        if (((IEnumerable<string>) BannedPartsResourcesWindow._bannedParts).Any<string>())
        {
          GUILayout.Label(LocalizationContainer.BannedPartsResourcesWindowText.BannedParts, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
          foreach (string bannedPart in BannedPartsResourcesWindow._bannedParts)
            GUILayout.Label(bannedPart, Array.Empty<GUILayoutOption>());
        }
        if (((IEnumerable<string>) BannedPartsResourcesWindow._bannedResources).Any<string>())
        {
          GUILayout.Label(LocalizationContainer.BannedPartsResourcesWindowText.BannedResources, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
          foreach (string bannedResource in BannedPartsResourcesWindow._bannedResources)
            GUILayout.Label(bannedResource, Array.Empty<GUILayoutOption>());
        }
        if (BannedPartsResourcesWindow._partCount > SettingsSystem.ServerSettings.MaxVesselParts)
          GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.BannedPartsResourcesWindowText.TooManyParts, (object) SettingsSystem.ServerSettings.MaxVesselParts), StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
      }
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && BannedPartsResourcesWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set => base.Display = BannedPartsResourcesWindow._display = value;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154322, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.BannedPartsResourcesWindowText.Title, this.LayoutOptions));

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width / 2.0 - 200.0), (float) ((double) Screen.height / 2.0 - 150.0), 400f, 300f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(400f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(400f);
      this.LayoutOptions[2] = GUILayout.MinHeight(300f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(300f);
      this.ScrollPos = new Vector2();
    }

    public void DisplayBannedPartsResourcesDialog(
      string vesselName,
      string[] bannedParts,
      string[] bannedResources,
      int partCount = 0)
    {
      if (this.Display)
        return;
      BannedPartsResourcesWindow._vesselName = vesselName;
      BannedPartsResourcesWindow._bannedParts = bannedParts;
      BannedPartsResourcesWindow._bannedResources = bannedResources;
      BannedPartsResourcesWindow._partCount = partCount;
      this.Display = true;
    }
  }
}
