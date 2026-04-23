// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.CraftLibrary.CraftLibrarySystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LmpClient.Systems.CraftLibrary
{
  public class CraftLibrarySystem : 
    MessageSystem<CraftLibrarySystem, CraftLibraryMessageSender, CraftLibraryMessageHandler>
  {
    private static readonly string SaveFolder = CommonUtil.CombinePaths(MainSystem.KspPath, "saves", "LunaMultiplayer");
    private static DateTime _lastRequest = DateTime.MinValue;

    public ConcurrentDictionary<string, ConcurrentDictionary<string, CraftBasicEntry>> CraftInfo { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, CraftBasicEntry>>();

    public ConcurrentDictionary<string, ConcurrentDictionary<string, CraftEntry>> CraftDownloaded { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, CraftEntry>>();

    public List<CraftEntry> OwnCrafts { get; } = new List<CraftEntry>();

    public ConcurrentQueue<string> DownloadedCraftsNotification { get; } = new ConcurrentQueue<string>();

    public List<string> FoldersWithNewContent { get; } = new List<string>();

    public bool NewContent => this.FoldersWithNewContent.Any<string>();

    public override string SystemName { get; } = nameof (CraftLibrarySystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.RefreshOwnCrafts();
      this.MessageSender.SendRequestFoldersMsg();
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.NotifyDownloadedCrafts)));
    }

    private void NotifyDownloadedCrafts()
    {
      string result;
      while (this.DownloadedCraftsNotification.TryDequeue(out result))
        LunaScreenMsg.PostScreenMessage("(" + result + ") " + LocalizationContainer.ScreenText.CraftSaved, 5f, (ScreenMessageStyle) 0);
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.CraftInfo.Clear();
      this.CraftDownloaded.Clear();
    }

    public void RefreshOwnCrafts()
    {
      this.OwnCrafts.Clear();
      string path1 = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Ships", "VAB");
      if (Directory.Exists(path1))
      {
        foreach (string file in Directory.GetFiles(path1))
        {
          byte[] numArray = File.ReadAllBytes(file);
          this.OwnCrafts.Add(new CraftEntry()
          {
            CraftName = Path.GetFileNameWithoutExtension(file),
            CraftType = CraftType.Vab,
            FolderName = SettingsSystem.CurrentSettings.PlayerName,
            CraftData = numArray,
            CraftNumBytes = numArray.Length
          });
        }
      }
      string path2 = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Ships", "SPH");
      if (Directory.Exists(path2))
      {
        foreach (string file in Directory.GetFiles(path2))
        {
          byte[] numArray = File.ReadAllBytes(file);
          this.OwnCrafts.Add(new CraftEntry()
          {
            CraftName = Path.GetFileNameWithoutExtension(file),
            CraftType = CraftType.Sph,
            FolderName = SettingsSystem.CurrentSettings.PlayerName,
            CraftData = numArray,
            CraftNumBytes = numArray.Length
          });
        }
      }
      string path3 = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Subassemblies");
      if (!Directory.Exists(path3))
        return;
      foreach (string file in Directory.GetFiles(path3))
      {
        byte[] numArray = File.ReadAllBytes(file);
        this.OwnCrafts.Add(new CraftEntry()
        {
          CraftName = Path.GetFileNameWithoutExtension(file),
          CraftType = CraftType.Subassembly,
          FolderName = SettingsSystem.CurrentSettings.PlayerName,
          CraftData = numArray,
          CraftNumBytes = numArray.Length
        });
      }
    }

    public void SaveCraftToDisk(CraftEntry craft)
    {
      string str;
      switch (craft.CraftType)
      {
        case CraftType.Vab:
          str = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Ships", "VAB");
          break;
        case CraftType.Sph:
          str = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Ships", "SPH");
          break;
        case CraftType.Subassembly:
          str = CommonUtil.CombinePaths(CraftLibrarySystem.SaveFolder, "Subassemblies");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      File.WriteAllBytes(CommonUtil.CombinePaths(str, craft.CraftName + ".craft"), craft.CraftData);
      this.DownloadedCraftsNotification.Enqueue(craft.CraftName);
    }

    public void SendCraft(CraftEntry craft)
    {
      if (TimeUtil.IsInInterval(ref CraftLibrarySystem._lastRequest, SettingsSystem.ServerSettings.MinCraftLibraryRequestIntervalMs))
      {
        this.MessageSender.SendCraftMsg(craft);
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CraftUploaded, 10f, (ScreenMessageStyle) 0);
      }
      else
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CraftLibraryInterval.Replace("$1", TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.MinCraftLibraryRequestIntervalMs).TotalSeconds.ToString((IFormatProvider) CultureInfo.InvariantCulture)), 20f, (ScreenMessageStyle) 0);
    }

    public void RequestCraft(CraftBasicEntry craft)
    {
      if (TimeUtil.IsInInterval(ref CraftLibrarySystem._lastRequest, SettingsSystem.ServerSettings.MinCraftLibraryRequestIntervalMs))
        this.MessageSender.SendRequestCraftMsg(craft);
      else
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CraftLibraryInterval.Replace("$1", TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.MinCraftLibraryRequestIntervalMs).TotalSeconds.ToString((IFormatProvider) CultureInfo.InvariantCulture)), 20f, (ScreenMessageStyle) 0);
    }

    public void RequestCraftListIfNeeded(string selectedFolder)
    {
      if (this.FoldersWithNewContent.Contains(selectedFolder))
      {
        this.FoldersWithNewContent.Remove(selectedFolder);
        this.MessageSender.SendRequestCraftListMsg(selectedFolder);
      }
      else
      {
        if (this.CraftInfo.GetOrAdd(selectedFolder, new ConcurrentDictionary<string, CraftBasicEntry>()).Count != 0)
          return;
        this.MessageSender.SendRequestCraftListMsg(selectedFolder);
      }
    }
  }
}
