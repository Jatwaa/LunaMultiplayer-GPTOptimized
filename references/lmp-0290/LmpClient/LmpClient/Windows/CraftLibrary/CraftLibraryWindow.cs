// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.CraftLibrary.CraftLibraryWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.CraftLibrary;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UniLinq;
using UnityEngine;

namespace LmpClient.Windows.CraftLibrary
{
  public class CraftLibraryWindow : SystemWindow<CraftLibraryWindow, CraftLibrarySystem>
  {
    private const int UpdateIntervalMs = 1500;
    private const float FoldersWindowHeight = 300f;
    private const float FoldersWindowWidth = 200f;
    private const float LibraryWindowHeight = 300f;
    private const float LibraryWindowWidth = 400f;
    private const float UploadWindowHeight = 300f;
    private const float UploadWindowWidth = 400f;
    private static Rect _libraryWindowRect;
    private static Rect _uploadWindowRect;
    private static GUILayoutOption[] _foldersLayoutOptions;
    private static GUILayoutOption[] _libraryLayoutOptions;
    private static GUILayoutOption[] _uploadLayoutOptions;
    private static Vector2 _foldersScrollPos;
    private static Vector2 _libraryScrollPos;
    private static Vector2 _uploadScrollPos;
    private static string _selectedFolder;
    private static bool _drawUploadScreen;
    private static DateTime _lastGuiUpdateTime = DateTime.MinValue;
    private static readonly List<CraftBasicEntry> VabCrafts = new List<CraftBasicEntry>();
    private static readonly List<CraftBasicEntry> SphCrafts = new List<CraftBasicEntry>();
    private static readonly List<CraftBasicEntry> SubAssemblyCrafts = new List<CraftBasicEntry>();
    private static bool _display;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      CraftLibraryWindow.DrawRefreshAndUploadButton((Action) (() => SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.MessageSender.SendRequestFoldersMsg()), (Action) (() => CraftLibraryWindow._drawUploadScreen = true));
      GUILayout.Space(15f);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      CraftLibraryWindow._foldersScrollPos = GUILayout.BeginScrollView(CraftLibraryWindow._foldersScrollPos, Array.Empty<GUILayoutOption>());
      if (!SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftInfo.Keys.Any<string>())
      {
        CraftLibraryWindow._selectedFolder = (string) null;
      }
      else
      {
        foreach (string key in (IEnumerable<string>) SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftInfo.Keys)
          this.DrawFolderButton(key);
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    private void DrawFolderButton(string folderName)
    {
      if (GUILayout.Toggle(CraftLibraryWindow._selectedFolder == folderName, folderName, this.GetFolderStyle(folderName), Array.Empty<GUILayoutOption>()))
      {
        if (!(CraftLibraryWindow._selectedFolder != folderName))
          return;
        CraftLibraryWindow._selectedFolder = folderName;
        SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.RequestCraftListIfNeeded(CraftLibraryWindow._selectedFolder);
      }
      else if (CraftLibraryWindow._selectedFolder == folderName)
        CraftLibraryWindow._selectedFolder = (string) null;
    }

    private GUIStyle GetFolderStyle(string folderName) => SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.FoldersWithNewContent.Contains(folderName) ? StyleLibrary.RedFontButtonStyle : StyleLibrary.Skin.button;

    public void DrawLibraryContent(int windowId)
    {
      this.DrawCloseButton((Action) (() => CraftLibraryWindow._selectedFolder = (string) null), CraftLibraryWindow._libraryWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.DrawRefreshButton((Action) (() => SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.MessageSender.SendRequestCraftListMsg(CraftLibraryWindow._selectedFolder)));
      GUILayout.Space(15f);
      if (string.IsNullOrEmpty(CraftLibraryWindow._selectedFolder))
        return;
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      CraftLibraryWindow._libraryScrollPos = GUILayout.BeginScrollView(CraftLibraryWindow._libraryScrollPos, Array.Empty<GUILayoutOption>());
      if (CraftLibraryWindow.SphCrafts.Any<CraftBasicEntry>())
      {
        GUILayout.Label("SPH", StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        for (int index = 0; index < CraftLibraryWindow.SphCrafts.Count; ++index)
          this.DrawCraftEntry(CraftLibraryWindow.SphCrafts[index]);
        GUILayout.Space(5f);
      }
      if (CraftLibraryWindow.VabCrafts.Any<CraftBasicEntry>())
      {
        GUILayout.Label("VAB", StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        for (int index = 0; index < CraftLibraryWindow.VabCrafts.Count; ++index)
          this.DrawCraftEntry(CraftLibraryWindow.VabCrafts[index]);
        GUILayout.Space(5f);
      }
      if (CraftLibraryWindow.SubAssemblyCrafts.Any<CraftBasicEntry>())
      {
        GUILayout.Label("Subassembly", StyleLibrary.BoldRedLabelStyle, Array.Empty<GUILayoutOption>());
        for (int index = 0; index < CraftLibraryWindow.SubAssemblyCrafts.Count; ++index)
          this.DrawCraftEntry(CraftLibraryWindow.SubAssemblyCrafts[index]);
        GUILayout.Space(5f);
      }
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    private void DrawCraftEntry(CraftBasicEntry craftBasicEntry)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(craftBasicEntry.CraftName, Array.Empty<GUILayoutOption>());
      if (craftBasicEntry.FolderName == SettingsSystem.CurrentSettings.PlayerName)
      {
        if (GUILayout.Button(StyleLibrary.DeleteIcon, new GUILayoutOption[1]
        {
          GUILayout.Width(35f)
        }))
        {
          SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.MessageSender.SendDeleteCraftMsg(craftBasicEntry);
          CraftLibraryWindow._selectedFolder = (string) null;
        }
      }
      else if (GUILayout.Button(StyleLibrary.SaveIcon, new GUILayoutOption[1]
      {
        GUILayout.Width(35f)
      }))
      {
        ConcurrentDictionary<string, CraftEntry> concurrentDictionary;
        if (SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftDownloaded.TryGetValue(CraftLibraryWindow._selectedFolder, out concurrentDictionary) && !concurrentDictionary.ContainsKey(craftBasicEntry.CraftName))
          SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.RequestCraft(craftBasicEntry);
        CraftLibraryWindow._selectedFolder = (string) null;
      }
      GUILayout.EndHorizontal();
    }

    public void DrawUploadScreenContent(int windowId)
    {
      this.DrawCloseButton((Action) (() => CraftLibraryWindow._drawUploadScreen = false), CraftLibraryWindow._uploadWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      this.DrawRefreshButton((Action) (() => SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.RefreshOwnCrafts()));
      GUILayout.Space(15f);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      CraftLibraryWindow._uploadScrollPos = GUILayout.BeginScrollView(CraftLibraryWindow._uploadScrollPos, Array.Empty<GUILayoutOption>());
      for (int index = 0; index < SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.OwnCrafts.Count; ++index)
        this.DrawUploadCraftEntry(SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.OwnCrafts[index]);
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndVertical();
    }

    private void DrawUploadCraftEntry(CraftEntry craftEntry)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("{0} ({1})", (object) craftEntry.CraftName, (object) craftEntry.CraftType), Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(StyleLibrary.UploadIcon, new GUILayoutOption[1]
      {
        GUILayout.Width(35f)
      }))
      {
        SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.SendCraft(craftEntry);
        CraftLibraryWindow._drawUploadScreen = false;
      }
      GUILayout.EndHorizontal();
    }

    private static void DrawRefreshAndUploadButton(Action refreshAction, Action uploadAction)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (GUILayout.Button(StyleLibrary.RefreshIcon, Array.Empty<GUILayoutOption>()))
        refreshAction();
      if (GUILayout.Button(StyleLibrary.UploadIcon, Array.Empty<GUILayoutOption>()))
        uploadAction();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
    }

    public override bool Display
    {
      get => base.Display && CraftLibraryWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set
      {
        if (!value)
          CraftLibraryWindow.Reset();
        if (value && !CraftLibraryWindow._display && SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftInfo.Count == 0)
          SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.MessageSender.SendRequestFoldersMsg();
        base.Display = CraftLibraryWindow._display = value;
      }
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display || !TimeUtil.IsInInterval(ref CraftLibraryWindow._lastGuiUpdateTime, 1500))
        return;
      CraftLibraryWindow.VabCrafts.Clear();
      CraftLibraryWindow.SphCrafts.Clear();
      CraftLibraryWindow.SubAssemblyCrafts.Clear();
      ConcurrentDictionary<string, CraftBasicEntry> concurrentDictionary;
      if (!string.IsNullOrEmpty(CraftLibraryWindow._selectedFolder) && SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftInfo.TryGetValue(CraftLibraryWindow._selectedFolder, out concurrentDictionary))
      {
        foreach (IGrouping<CraftType, CraftBasicEntry> collection in Enumerable.ToArray<IGrouping<CraftType, CraftBasicEntry>>(Enumerable.GroupBy<CraftBasicEntry, CraftType>((IEnumerable<CraftBasicEntry>) concurrentDictionary.Values, (Func<CraftBasicEntry, CraftType>) (v => v.CraftType))))
        {
          switch (collection.Key)
          {
            case CraftType.Vab:
              CraftLibraryWindow.VabCrafts.AddRange((IEnumerable<CraftBasicEntry>) collection);
              break;
            case CraftType.Sph:
              CraftLibraryWindow.SphCrafts.AddRange((IEnumerable<CraftBasicEntry>) collection);
              break;
            case CraftType.Subassembly:
              CraftLibraryWindow.SubAssemblyCrafts.AddRange((IEnumerable<CraftBasicEntry>) collection);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    protected override void DrawGui()
    {
      if (this.Display)
      {
        // ISSUE: method pointer
        this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154311, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.CraftLibraryWindowText.Folders, CraftLibraryWindow._foldersLayoutOptions));
      }
      if (this.Display && !string.IsNullOrEmpty(CraftLibraryWindow._selectedFolder) && SystemWindow<CraftLibraryWindow, CraftLibrarySystem>.System.CraftInfo.ContainsKey(CraftLibraryWindow._selectedFolder))
      {
        // ISSUE: method pointer
        CraftLibraryWindow._libraryWindowRect = this.FixWindowPos(GUILayout.Window(1664154312, CraftLibraryWindow._libraryWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawLibraryContent)), CraftLibraryWindow._selectedFolder + " " + LocalizationContainer.CraftLibraryWindowText.Crafts, CraftLibraryWindow._libraryLayoutOptions));
      }
      if (!this.Display || !CraftLibraryWindow._drawUploadScreen)
        return;
      // ISSUE: method pointer
      CraftLibraryWindow._uploadWindowRect = this.FixWindowPos(GUILayout.Window(1664154313, CraftLibraryWindow._uploadWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawUploadScreenContent)), LocalizationContainer.CraftLibraryWindowText.Upload, CraftLibraryWindow._uploadLayoutOptions));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect(50f, (float) ((double) Screen.height / 2.0 - 150.0), 200f, 300f);
      CraftLibraryWindow._libraryWindowRect = new Rect((float) ((double) Screen.width / 2.0 - 200.0), (float) ((double) Screen.height / 2.0 - 150.0), 400f, 300f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      CraftLibraryWindow._foldersLayoutOptions = new GUILayoutOption[4];
      CraftLibraryWindow._foldersLayoutOptions[0] = GUILayout.MinWidth(200f);
      CraftLibraryWindow._foldersLayoutOptions[1] = GUILayout.MaxWidth(200f);
      CraftLibraryWindow._foldersLayoutOptions[2] = GUILayout.MinHeight(300f);
      CraftLibraryWindow._foldersLayoutOptions[3] = GUILayout.MaxHeight(300f);
      CraftLibraryWindow._libraryLayoutOptions = new GUILayoutOption[4];
      CraftLibraryWindow._libraryLayoutOptions[0] = GUILayout.MinWidth(400f);
      CraftLibraryWindow._libraryLayoutOptions[1] = GUILayout.MaxWidth(400f);
      CraftLibraryWindow._libraryLayoutOptions[2] = GUILayout.MinHeight(300f);
      CraftLibraryWindow._libraryLayoutOptions[3] = GUILayout.MaxHeight(300f);
      CraftLibraryWindow._uploadLayoutOptions = new GUILayoutOption[4];
      CraftLibraryWindow._uploadLayoutOptions[0] = GUILayout.MinWidth(400f);
      CraftLibraryWindow._uploadLayoutOptions[1] = GUILayout.MaxWidth(400f);
      CraftLibraryWindow._uploadLayoutOptions[2] = GUILayout.MinHeight(300f);
      CraftLibraryWindow._uploadLayoutOptions[3] = GUILayout.MaxHeight(300f);
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_CraftLibraryLock");
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
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2) || !string.IsNullOrEmpty(CraftLibraryWindow._selectedFolder) && ((Rect) ref CraftLibraryWindow._libraryWindowRect).Contains(vector2) || CraftLibraryWindow._drawUploadScreen && ((Rect) ref CraftLibraryWindow._uploadWindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_CraftLibraryLock");
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
      CraftLibraryWindow._selectedFolder = (string) null;
      CraftLibraryWindow._drawUploadScreen = false;
    }
  }
}
