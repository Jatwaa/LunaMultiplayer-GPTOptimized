// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Screenshots.ScreenshotsWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.Screenshot;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UniLinq;
using UnityEngine;

namespace LmpClient.Windows.Screenshots
{
  public class ScreenshotsWindow : SystemWindow<ScreenshotsWindow, ScreenshotSystem>
  {
    private static string _screenshotKeyLabel;
    private const int UpdateIntervalMs = 1500;
    protected const float FoldersWindowHeight = 300f;
    protected const float FoldersWindowWidth = 200f;
    protected const float LibraryWindowHeight = 600f;
    protected const float LibraryWindowWidth = 600f;
    protected const float ImageWindowHeight = 762f;
    protected const float ImageWindowWidth = 1024f;
    private static Rect _libraryWindowRect;
    private static Rect _imageWindowRect;
    private static GUILayoutOption[] _foldersLayoutOptions;
    private static GUILayoutOption[] _libraryLayoutOptions;
    private static Vector2 _foldersScrollPos;
    private static Vector2 _libraryScrollPos;
    private static Vector2 _imageScrollPos;
    private static string _selectedFolder;
    private static long _selectedImage;
    private static DateTime _lastGuiUpdateTime = DateTime.MinValue;
    private static readonly List<LmpClient.Systems.Screenshot.Screenshot> Miniatures = new List<LmpClient.Systems.Screenshot.Screenshot>();
    private static bool _display;

    private static string ScreenshotKeyLabel
    {
      get
      {
        if (string.IsNullOrEmpty(ScreenshotsWindow._screenshotKeyLabel))
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("Screenshot key: ");
          if (!GameSettings.TAKE_SCREENSHOT.primary.isNone)
            stringBuilder.Append((object) GameSettings.TAKE_SCREENSHOT.primary.code);
          if (!GameSettings.TAKE_SCREENSHOT.secondary.isNone)
          {
            if (!GameSettings.TAKE_SCREENSHOT.primary.isNone)
              stringBuilder.Append("/");
            stringBuilder.Append((object) GameSettings.TAKE_SCREENSHOT.secondary.code);
          }
          if (GameSettings.TAKE_SCREENSHOT.primary.isNone && GameSettings.TAKE_SCREENSHOT.secondary.isNone)
            stringBuilder.Append("NONE!");
          ScreenshotsWindow._screenshotKeyLabel = stringBuilder.ToString();
        }
        return ScreenshotsWindow._screenshotKeyLabel;
      }
    }

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.DrawRefreshButton((Action) (() => SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MessageSender.RequestFolders()));
      GUILayout.Label(ScreenshotsWindow.ScreenshotKeyLabel, Array.Empty<GUILayoutOption>());
      GUILayout.Space(15f);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      ScreenshotsWindow._foldersScrollPos = GUILayout.BeginScrollView(ScreenshotsWindow._foldersScrollPos, Array.Empty<GUILayoutOption>());
      foreach (string key in (IEnumerable<string>) SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MiniatureImages.Keys)
        this.DrawFolderButton(key);
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    private void DrawFolderButton(string folderName)
    {
      if (GUILayout.Toggle(ScreenshotsWindow._selectedFolder == folderName, folderName, this.GetFolderStyle(folderName), Array.Empty<GUILayoutOption>()))
      {
        if (!(ScreenshotsWindow._selectedFolder != folderName))
          return;
        ScreenshotsWindow._selectedFolder = folderName;
        SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.RequestMiniaturesIfNeeded(ScreenshotsWindow._selectedFolder);
        ScreenshotsWindow._selectedImage = 0L;
        ScreenshotsWindow.Miniatures.Clear();
      }
      else if (ScreenshotsWindow._selectedFolder == folderName)
      {
        ScreenshotsWindow._selectedFolder = (string) null;
        ScreenshotsWindow.Miniatures.Clear();
      }
    }

    private GUIStyle GetFolderStyle(string folderName) => SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.FoldersWithNewContent.Contains(folderName) ? StyleLibrary.RedFontButtonStyle : StyleLibrary.Skin.button;

    public void DrawLibraryContent(int windowId)
    {
      this.DrawCloseButton((Action) (() => ScreenshotsWindow._selectedFolder = (string) null), ScreenshotsWindow._libraryWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.DrawRefreshButton((Action) (() =>
      {
        ScreenshotsWindow._selectedImage = 0L;
        SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MessageSender.RequestMiniatures(ScreenshotsWindow._selectedFolder);
        ScreenshotsWindow.Miniatures.Clear();
      }));
      GUILayout.Space(15f);
      if (string.IsNullOrEmpty(ScreenshotsWindow._selectedFolder))
        return;
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      ScreenshotsWindow._libraryScrollPos = GUILayout.BeginScrollView(ScreenshotsWindow._libraryScrollPos, Array.Empty<GUILayoutOption>());
      if (Enumerable.Any<LmpClient.Systems.Screenshot.Screenshot>((IEnumerable<LmpClient.Systems.Screenshot.Screenshot>) ScreenshotsWindow.Miniatures))
      {
        for (int index = 0; index < ScreenshotsWindow.Miniatures.Count; index += 4)
        {
          GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
          GUILayout.FlexibleSpace();
          ScreenshotsWindow.DrawMiniature(ScreenshotsWindow.Miniatures[index]);
          GUILayout.FlexibleSpace();
          if (ScreenshotsWindow.Miniatures.Count > index + 1)
          {
            GUILayout.FlexibleSpace();
            ScreenshotsWindow.DrawMiniature(ScreenshotsWindow.Miniatures[index + 1]);
            GUILayout.FlexibleSpace();
          }
          if (ScreenshotsWindow.Miniatures.Count > index + 2)
          {
            GUILayout.FlexibleSpace();
            ScreenshotsWindow.DrawMiniature(ScreenshotsWindow.Miniatures[index + 2]);
            GUILayout.FlexibleSpace();
          }
          if (ScreenshotsWindow.Miniatures.Count > index + 3)
          {
            GUILayout.FlexibleSpace();
            ScreenshotsWindow.DrawMiniature(ScreenshotsWindow.Miniatures[index + 3]);
            GUILayout.FlexibleSpace();
          }
          GUILayout.EndHorizontal();
        }
      }
      else
        this.DrawWaitIcon(false);
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    private static void DrawMiniature(LmpClient.Systems.Screenshot.Screenshot miniature)
    {
      if (!GUILayout.Button((Texture) miniature.Texture, new GUILayoutOption[2]
      {
        GUILayout.Width((float) miniature.Width),
        GUILayout.Height((float) miniature.Height)
      }))
        return;
      ScreenshotsWindow._selectedImage = miniature.DateTaken;
    }

    public void DrawImageContent(int windowId)
    {
      this.DrawCloseButton((Action) (() => ScreenshotsWindow._selectedImage = 0L), ScreenshotsWindow._imageWindowRect);
      if (GUI.RepeatButton(new Rect(((Rect) ref ScreenshotsWindow._imageWindowRect).width - 15f, ((Rect) ref ScreenshotsWindow._imageWindowRect).height - 15f, 10f, 10f), (Texture) StyleLibrary.ResizeIcon, StyleLibrary.ResizeButtonStyle))
        this.ResizingWindow = true;
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(StyleLibrary.SaveIcon, Array.Empty<GUILayoutOption>()))
      {
        SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.SaveImage(ScreenshotsWindow._selectedFolder, ScreenshotsWindow._selectedImage);
        ScreenshotsWindow._selectedImage = 0L;
      }
      GUILayout.EndHorizontal();
      GUILayout.Space(15f);
      if (ScreenshotsWindow._selectedImage == 0L)
        return;
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      ScreenshotsWindow._imageScrollPos = GUILayout.BeginScrollView(ScreenshotsWindow._imageScrollPos, Array.Empty<GUILayoutOption>());
      ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary;
      LmpClient.Systems.Screenshot.Screenshot screenShot;
      if (SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.DownloadedImages.TryGetValue(ScreenshotsWindow._selectedFolder, out concurrentDictionary) && concurrentDictionary.TryGetValue(ScreenshotsWindow._selectedImage, out screenShot))
      {
        ScreenshotsWindow.DrawImageCentered(screenShot);
      }
      else
      {
        this.DrawWaitIcon(false);
        SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MessageSender.RequestImage(ScreenshotsWindow._selectedFolder, ScreenshotsWindow._selectedImage);
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
      if (ScreenshotsWindow.Miniatures.Count > 1)
      {
        GUILayout.Space(15f);
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        if (GUILayout.Button(StyleLibrary.CycleFirstIcon, Array.Empty<GUILayoutOption>()))
          ScreenshotsWindow._selectedImage = ScreenshotsWindow.Miniatures[0].DateTaken;
        if (GUILayout.Button(StyleLibrary.CyclePreviousIcon, Array.Empty<GUILayoutOption>()))
          ScreenshotsWindow._selectedImage = ScreenshotsWindow.Miniatures[ScreenshotsWindow.CurrentIndex > 0 ? ScreenshotsWindow.CurrentIndex - 1 : ScreenshotsWindow.Miniatures.Count - 1].DateTaken;
        if (GUILayout.Button(StyleLibrary.CycleNextIcon, Array.Empty<GUILayoutOption>()))
          ScreenshotsWindow._selectedImage = ScreenshotsWindow.Miniatures[ScreenshotsWindow.CurrentIndex < ScreenshotsWindow.Miniatures.Count - 1 ? ScreenshotsWindow.CurrentIndex + 1 : 0].DateTaken;
        if (GUILayout.Button(StyleLibrary.CycleLastIcon, Array.Empty<GUILayoutOption>()))
          ScreenshotsWindow._selectedImage = ScreenshotsWindow.Miniatures[ScreenshotsWindow.Miniatures.Count - 1].DateTaken;
        GUILayout.EndHorizontal();
      }
      GUILayout.Space(15f);
    }

    private static void DrawImageCentered(LmpClient.Systems.Screenshot.Screenshot screenShot)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.Label((Texture) screenShot.Texture, Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
    }

    private static int CurrentIndex => ScreenshotsWindow._selectedImage == 0L ? 0 : ScreenshotsWindow.Miniatures.FindIndex((Predicate<LmpClient.Systems.Screenshot.Screenshot>) (s => s.DateTaken.Equals(ScreenshotsWindow._selectedImage)));

    public override bool Display
    {
      get => base.Display && ScreenshotsWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set
      {
        if (!value)
          ScreenshotsWindow.Reset();
        if (value && !ScreenshotsWindow._display && SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MiniatureImages.Count == 0)
          SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MessageSender.RequestFolders();
        base.Display = ScreenshotsWindow._display = value;
      }
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display)
        return;
      if (TimeUtil.IsInInterval(ref ScreenshotsWindow._lastGuiUpdateTime, 1500))
      {
        ScreenshotsWindow.Miniatures.Clear();
        ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary;
        if (!string.IsNullOrEmpty(ScreenshotsWindow._selectedFolder) && SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MiniatureImages.TryGetValue(ScreenshotsWindow._selectedFolder, out concurrentDictionary))
          ScreenshotsWindow.Miniatures.AddRange((IEnumerable<LmpClient.Systems.Screenshot.Screenshot>) Enumerable.OrderBy<LmpClient.Systems.Screenshot.Screenshot, long>((IEnumerable<LmpClient.Systems.Screenshot.Screenshot>) concurrentDictionary.Values, (Func<LmpClient.Systems.Screenshot.Screenshot, long>) (v => v.DateTaken)));
      }
      if (Input.GetMouseButtonUp(0))
        this.ResizingWindow = false;
      if (!this.ResizingWindow)
        return;
      ((Rect) ref ScreenshotsWindow._imageWindowRect).width = (float) ((double) Input.mousePosition.x - (double) ((Rect) ref ScreenshotsWindow._imageWindowRect).x + 10.0);
      ((Rect) ref ScreenshotsWindow._imageWindowRect).height = (float) ((double) Screen.height - (double) Input.mousePosition.y - (double) ((Rect) ref ScreenshotsWindow._imageWindowRect).y + 10.0);
    }

    protected override void DrawGui()
    {
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154323, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.ScreenshotWindowText.Folders, ScreenshotsWindow._foldersLayoutOptions));
      if (!string.IsNullOrEmpty(ScreenshotsWindow._selectedFolder) && SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.MiniatureImages.ContainsKey(ScreenshotsWindow._selectedFolder))
      {
        // ISSUE: method pointer
        ScreenshotsWindow._libraryWindowRect = this.FixWindowPos(GUILayout.Window(1664154324, ScreenshotsWindow._libraryWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawLibraryContent)), ScreenshotsWindow._selectedFolder + " " + LocalizationContainer.ScreenshotWindowText.Screenshots, ScreenshotsWindow._libraryLayoutOptions));
      }
      if (string.IsNullOrEmpty(ScreenshotsWindow._selectedFolder) || !SystemWindow<ScreenshotsWindow, ScreenshotSystem>.System.DownloadedImages.ContainsKey(ScreenshotsWindow._selectedFolder) || ScreenshotsWindow._selectedImage <= 0L)
        return;
      // ISSUE: method pointer
      ScreenshotsWindow._imageWindowRect = this.FixWindowPos(GUILayout.Window(1664154325, ScreenshotsWindow._imageWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawImageContent)), string.Format("{0} [{1}] - {2:yyyy/MM/dd HH:mm:ss} UTC", (object) ScreenshotsWindow._selectedFolder, (object) (ScreenshotsWindow.CurrentIndex + 1), (object) DateTime.FromBinary(ScreenshotsWindow._selectedImage)), Array.Empty<GUILayoutOption>()));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect(50f, (float) ((double) Screen.height / 2.0 - 150.0), 200f, 300f);
      ScreenshotsWindow._libraryWindowRect = new Rect((float) ((double) Screen.width / 2.0 - 300.0), (float) ((double) Screen.height / 2.0 - 300.0), 600f, 600f);
      ScreenshotsWindow._imageWindowRect = new Rect((float) ((double) Screen.width / 2.0 - 1024.0), (float) ((double) Screen.height / 2.0 - 381.0), 1024f, 762f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      ScreenshotsWindow._foldersLayoutOptions = new GUILayoutOption[4];
      ScreenshotsWindow._foldersLayoutOptions[0] = GUILayout.MinWidth(200f);
      ScreenshotsWindow._foldersLayoutOptions[1] = GUILayout.MaxWidth(200f);
      ScreenshotsWindow._foldersLayoutOptions[2] = GUILayout.MinHeight(300f);
      ScreenshotsWindow._foldersLayoutOptions[3] = GUILayout.MaxHeight(300f);
      ScreenshotsWindow._libraryLayoutOptions = new GUILayoutOption[4];
      ScreenshotsWindow._libraryLayoutOptions[0] = GUILayout.MinWidth(600f);
      ScreenshotsWindow._libraryLayoutOptions[1] = GUILayout.MaxWidth(600f);
      ScreenshotsWindow._libraryLayoutOptions[2] = GUILayout.MinHeight(600f);
      ScreenshotsWindow._libraryLayoutOptions[3] = GUILayout.MaxHeight(600f);
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_ScreenshotLock");
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
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2) || !string.IsNullOrEmpty(ScreenshotsWindow._selectedFolder) && ((Rect) ref ScreenshotsWindow._libraryWindowRect).Contains(vector2) || ScreenshotsWindow._selectedImage > 0L && ((Rect) ref ScreenshotsWindow._imageWindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_ScreenshotLock");
          this.IsWindowLocked = true;
        }
        if (!flag && this.IsWindowLocked)
          this.RemoveWindowLock();
      }
      if (this.Display || !this.IsWindowLocked)
        return;
      this.RemoveWindowLock();
    }

    protected override void NetworkEventHandler(ClientState data)
    {
      if (data > ClientState.Disconnected)
        return;
      this.Display = false;
    }

    private static void Reset()
    {
      ScreenshotsWindow._selectedFolder = (string) null;
      ScreenshotsWindow._selectedImage = 0L;
    }
  }
}
