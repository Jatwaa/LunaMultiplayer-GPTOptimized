// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Screenshot.ScreenshotSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.Screenshot
{
  public class ScreenshotSystem : 
    MessageSystem<ScreenshotSystem, ScreenshotMessageSender, ScreenshotMessageHandler>
  {
    private static readonly string ScreenshotsFolder = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Screenshots");
    private static DateTime _lastTakenScreenshot = DateTime.MinValue;

    public ConcurrentDictionary<string, ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>> MiniatureImages { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>>();

    public ConcurrentDictionary<string, ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>> DownloadedImages { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>>();

    public List<string> FoldersWithNewContent { get; } = new List<string>();

    public bool NewContent => this.FoldersWithNewContent.Any<string>();

    public override string SystemName { get; } = nameof (ScreenshotSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.MessageSender.RequestFolders();
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.CheckScreenshots)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.MiniatureImages.Clear();
      this.DownloadedImages.Clear();
      this.FoldersWithNewContent.Clear();
    }

    public void CheckScreenshots()
    {
      if (!GameSettings.TAKE_SCREENSHOT.GetKeyDown(false))
        return;
      if (TimeUtil.IsInInterval(ref ScreenshotSystem._lastTakenScreenshot, SettingsSystem.ServerSettings.MinScreenshotIntervalMs))
      {
        string path = CommonUtil.CombinePaths(MainSystem.KspPath, "Screenshots");
        CoroutineUtil.StartDelayedRoutine(nameof (CheckScreenshots), (Action) (() =>
        {
          FileInfo fileInfo = ((IEnumerable<FileInfo>) new DirectoryInfo(path).GetFiles()).OrderByDescending<FileInfo, DateTime>((Func<FileInfo, DateTime>) (f => f.LastWriteTime)).FirstOrDefault<FileInfo>();
          if (fileInfo == null)
            return;
          byte[] imageData = ScreenshotSystem.ScaleScreenshot(File.ReadAllBytes(fileInfo.FullName), 800, 600);
          SystemBase.TaskFactory.StartNew((Action) (() => this.MessageSender.SendScreenshot(imageData)));
          LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.ScreenshotTaken, 10f, (ScreenMessageStyle) 0);
        }), 0.3f);
      }
      else
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.ScreenshotInterval.Replace("$1", TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.MinScreenshotIntervalMs).TotalSeconds.ToString((IFormatProvider) CultureInfo.InvariantCulture)), 20f, (ScreenMessageStyle) 0);
    }

    public void SaveImage(string folder, long dateTaken)
    {
      ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary;
      LmpClient.Systems.Screenshot.Screenshot screenshot;
      if (!this.DownloadedImages.TryGetValue(folder, out concurrentDictionary) || !concurrentDictionary.TryGetValue(dateTaken, out screenshot))
        return;
      string path = CommonUtil.CombinePaths(ScreenshotSystem.ScreenshotsFolder, folder);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      File.WriteAllBytes(CommonUtil.CombinePaths(path, string.Format("{0}.png", (object) dateTaken)), screenshot.Data);
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.ImageSaved, 20f, (ScreenMessageStyle) 0);
    }

    public void RequestMiniaturesIfNeeded(string selectedFolder)
    {
      if (this.FoldersWithNewContent.Contains(selectedFolder))
      {
        this.FoldersWithNewContent.Remove(selectedFolder);
        this.MessageSender.RequestMiniatures(selectedFolder);
      }
      else
      {
        if (this.MiniatureImages.GetOrAdd(selectedFolder, new ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>()).Count != 0)
          return;
        this.MessageSender.RequestMiniatures(selectedFolder);
      }
    }

    private static byte[] ScaleScreenshot(byte[] source, int maxWidth, int maxHeight)
    {
      Texture2D texture2D1 = new Texture2D(1, 1);
      ImageConversion.LoadImage(texture2D1, source);
      double num = Math.Min((double) maxWidth / (double) ((Texture) texture2D1).width, (double) maxHeight / (double) ((Texture) texture2D1).height);
      Texture2D texture2D2 = new Texture2D((int) ((double) ((Texture) texture2D1).width * num), (int) ((double) ((Texture) texture2D1).height * num));
      for (int index1 = 0; index1 < ((Texture) texture2D2).height; ++index1)
      {
        for (int index2 = 0; index2 < ((Texture) texture2D2).width; ++index2)
        {
          Color pixelBilinear = texture2D1.GetPixelBilinear((float) index2 / (float) ((Texture) texture2D2).width, (float) index1 / (float) ((Texture) texture2D2).height);
          texture2D2.SetPixel(index2, index1, pixelBilinear);
        }
      }
      texture2D2.Apply();
      return ImageConversion.EncodeToPNG(texture2D2);
    }
  }
}
