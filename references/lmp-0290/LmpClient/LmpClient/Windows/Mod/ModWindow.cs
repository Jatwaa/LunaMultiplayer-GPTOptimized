// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Mod.ModWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.Mod;
using LmpCommon.ModFile.Structure;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Windows.Mod
{
  public class ModWindow : Window<ModWindow>
  {
    private const float WindowHeight = 600f;
    private const float WindowWidth = 600f;
    private static Vector2 _missingExpansionsScrollPos;
    private static Vector2 _mandatoryFilesNotFoundScrollPos;
    private static Vector2 _mandatoryFilesDifferentShaScrollPos;
    private static Vector2 _forbiddenFilesScrollPos;
    private static Vector2 _nonListedFilesScrollPos;
    private static Vector2 _mandatoryPartsScrollPos;
    private static Vector2 _forbiddenPartsScrollPos;
    private static bool _display;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.Space(10f);
      this.ScrollPos = GUILayout.BeginScrollView(ModWindow._missingExpansionsScrollPos, Array.Empty<GUILayoutOption>());
      if (Enumerable.Any<string>((IEnumerable<string>) LmpClient.Base.System<ModSystem>.Singleton.MissingExpansions))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.MissingExpansions, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._missingExpansionsScrollPos = GUILayout.BeginScrollView(ModWindow._missingExpansionsScrollPos, Array.Empty<GUILayoutOption>());
        foreach (string missingExpansion in LmpClient.Base.System<ModSystem>.Singleton.MissingExpansions)
          GUILayout.Label(missingExpansion, Array.Empty<GUILayoutOption>());
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      if (Enumerable.Any<DllFile>((IEnumerable<DllFile>) LmpClient.Base.System<ModSystem>.Singleton.MandatoryFilesNotFound))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.MandatoryModsNotFound, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._mandatoryFilesNotFoundScrollPos = GUILayout.BeginScrollView(ModWindow._mandatoryFilesNotFoundScrollPos, Array.Empty<GUILayoutOption>());
        foreach (DllFile dllFile in LmpClient.Base.System<ModSystem>.Singleton.MandatoryFilesNotFound)
        {
          GUILayout.Label(dllFile.FilePath, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(dllFile.Text))
            GUILayout.Label(dllFile.Text, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(dllFile.Link) && GUILayout.Button(LocalizationContainer.ModWindowText.Link, StyleLibrary.HyperlinkLabelStyle, Array.Empty<GUILayoutOption>()))
            Application.OpenURL(dllFile.Link);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      if (Enumerable.Any<DllFile>((IEnumerable<DllFile>) LmpClient.Base.System<ModSystem>.Singleton.MandatoryFilesDifferentSha))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.MandatoryModsDifferentShaFound, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._mandatoryFilesDifferentShaScrollPos = GUILayout.BeginScrollView(ModWindow._mandatoryFilesDifferentShaScrollPos, Array.Empty<GUILayoutOption>());
        foreach (DllFile dllFile in LmpClient.Base.System<ModSystem>.Singleton.MandatoryFilesDifferentSha)
        {
          GUILayout.Label(dllFile.FilePath, Array.Empty<GUILayoutOption>());
          GUILayout.Label(dllFile.Sha, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(dllFile.Text))
            GUILayout.Label(dllFile.Text, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(dllFile.Link) && GUILayout.Button(LocalizationContainer.ModWindowText.Link, StyleLibrary.HyperlinkLabelStyle, Array.Empty<GUILayoutOption>()))
            Application.OpenURL(dllFile.Link);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      if (Enumerable.Any<ForbiddenDllFile>((IEnumerable<ForbiddenDllFile>) LmpClient.Base.System<ModSystem>.Singleton.ForbiddenFilesFound))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.ForbiddenFilesFound, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._forbiddenFilesScrollPos = GUILayout.BeginScrollView(ModWindow._forbiddenFilesScrollPos, Array.Empty<GUILayoutOption>());
        foreach (ForbiddenDllFile forbiddenDllFile in LmpClient.Base.System<ModSystem>.Singleton.ForbiddenFilesFound)
        {
          GUILayout.Label(forbiddenDllFile.FilePath, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(forbiddenDllFile.Text))
            GUILayout.Label(forbiddenDllFile.Text, Array.Empty<GUILayoutOption>());
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      if (Enumerable.Any<string>((IEnumerable<string>) LmpClient.Base.System<ModSystem>.Singleton.NonListedFilesFound))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.NonListedFilesFound, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._nonListedFilesScrollPos = GUILayout.BeginScrollView(ModWindow._nonListedFilesScrollPos, Array.Empty<GUILayoutOption>());
        foreach (string str in LmpClient.Base.System<ModSystem>.Singleton.NonListedFilesFound)
          GUILayout.Label(str, Array.Empty<GUILayoutOption>());
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      if (Enumerable.Any<MandatoryPart>((IEnumerable<MandatoryPart>) LmpClient.Base.System<ModSystem>.Singleton.MandatoryPartsNotFound))
      {
        GUILayout.Label(LocalizationContainer.ModWindowText.MandatoryPartsNotFound, StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        ModWindow._mandatoryPartsScrollPos = GUILayout.BeginScrollView(ModWindow._mandatoryPartsScrollPos, Array.Empty<GUILayoutOption>());
        foreach (MandatoryPart mandatoryPart in LmpClient.Base.System<ModSystem>.Singleton.MandatoryPartsNotFound)
        {
          GUILayout.Label(mandatoryPart.PartName, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(mandatoryPart.Text))
            GUILayout.Label(mandatoryPart.Text, Array.Empty<GUILayoutOption>());
          if (!string.IsNullOrEmpty(mandatoryPart.Link) && GUILayout.Button(LocalizationContainer.ModWindowText.Link, StyleLibrary.HyperlinkLabelStyle, Array.Empty<GUILayoutOption>()))
            Application.OpenURL(mandatoryPart.Link);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && ModWindow._display && HighLogic.LoadedScene == 2;
      set => base.Display = ModWindow._display = value;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154310, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.ModWindowText.Title, this.LayoutOptions));

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width / 2.0 - 300.0), (float) ((double) Screen.height / 2.0 - 300.0), 600f, 600f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(600f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(600f);
      this.LayoutOptions[2] = GUILayout.MinHeight(600f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(600f);
    }
  }
}
