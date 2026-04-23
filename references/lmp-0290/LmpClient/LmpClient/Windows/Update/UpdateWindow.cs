// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Update.UpdateWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using LmpGlobal;
using System;
using UnityEngine;

namespace LmpClient.Windows.Update
{
  public class UpdateWindow : Window<UpdateWindow>
  {
    public static Version LatestVersion;
    public static string Changelog;
    private static bool _display;
    private const float WindowHeight = 250f;
    private const float WindowWidth = 400f;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.Label(LocalizationContainer.UpdateWindowText.Text ?? "", LmpVersioning.IsCompatible(UpdateWindow.LatestVersion) ? StyleLibrary.BoldGreenLabelStyle : StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.UpdateWindowText.CurrentVersion, (object) LmpVersioning.CurrentVersion), Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("{0} {1}", (object) LocalizationContainer.UpdateWindowText.LatestVersion, (object) UpdateWindow.LatestVersion), Array.Empty<GUILayoutOption>());
      GUILayout.EndVertical();
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      if (LmpVersioning.IsCompatible(UpdateWindow.LatestVersion))
        GUILayout.Label(LocalizationContainer.UpdateWindowText.StillCompatible ?? "", StyleLibrary.BoldGreenLabelStyle, Array.Empty<GUILayoutOption>());
      else
        GUILayout.Label(LocalizationContainer.UpdateWindowText.NotCompatible ?? "", StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
      GUILayout.EndVertical();
      GUILayout.Label(LocalizationContainer.UpdateWindowText.Changelog, Array.Empty<GUILayoutOption>());
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, new GUILayoutOption[2]
      {
        GUILayout.Width(395f),
        GUILayout.Height(150f)
      });
      GUILayout.Label(UpdateWindow.Changelog, Array.Empty<GUILayoutOption>());
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      if (GUILayout.Button(StyleLibrary.DownloadBigIcon, Array.Empty<GUILayoutOption>()))
      {
        Application.OpenURL(RepoConstants.LatestGithubReleaseUrl);
        this.Display = false;
      }
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && UpdateWindow._display && HighLogic.LoadedScene <= 2 && SettingsSystem.CurrentSettings.DisclaimerAccepted;
      set => base.Display = UpdateWindow._display = value;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154328, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.UpdateWindowText.Title, this.LayoutOptions));

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) Screen.width - 450f, (float) ((double) Screen.height / 2.0 - 125.0), 400f, 250f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(400f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(400f);
      this.LayoutOptions[2] = GUILayout.MinHeight(250f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(250f);
      this.TextAreaOptions = new GUILayoutOption[1];
      this.TextAreaOptions[0] = GUILayout.ExpandWidth(true);
    }
  }
}
