// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Flag.FlagSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpCommon;
using LmpCommon.Flags;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.Flag
{
  public class FlagSystem : MessageSystem<FlagSystem, FlagMessageSender, FlagMessageHandler>
  {
    public FlagEvents FlagEvents { get; } = new FlagEvents();

    public static string LmpFlagPath { get; } = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Flags");

    public ConcurrentDictionary<string, ExtendedFlagInfo> ServerFlags { get; } = new ConcurrentDictionary<string, ExtendedFlagInfo>();

    private bool FlagSystemReady => this.Enabled && HighLogic.CurrentGame?.flagURL != null;

    public override string SystemName { get; } = nameof (FlagSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onFlagSelect.Add(new EventData<string>.OnEvent((object) this.FlagEvents, __methodptr(OnFlagSelect)));
      // ISSUE: method pointer
      GameEvents.onMissionFlagSelect.Add(new EventData<string>.OnEvent((object) this.FlagEvents, __methodptr(OnMissionFlagSelect)));
      this.SetupRoutine(new RoutineDefinition(5000, RoutineExecution.Update, new Action(this.HandleFlags)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.ServerFlags.Clear();
      // ISSUE: method pointer
      GameEvents.onFlagSelect.Remove(new EventData<string>.OnEvent((object) this.FlagEvents, __methodptr(OnFlagSelect)));
      // ISSUE: method pointer
      GameEvents.onMissionFlagSelect.Remove(new EventData<string>.OnEvent((object) this.FlagEvents, __methodptr(OnMissionFlagSelect)));
    }

    private void HandleFlags()
    {
      if (!this.FlagSystemReady)
        return;
      foreach (KeyValuePair<string, ExtendedFlagInfo> keyValuePair in this.ServerFlags.Where<KeyValuePair<string, ExtendedFlagInfo>>((Func<KeyValuePair<string, ExtendedFlagInfo>, bool>) (v => !v.Value.Loaded)))
      {
        this.HandleFlag(keyValuePair.Value);
        keyValuePair.Value.Loaded = true;
      }
    }

    public bool FlagExists(string flagUrl) => GameDatabase.Instance.ExistsTexture(flagUrl);

    public void SendFlag(string flagUrl)
    {
      ExtendedFlagInfo extendedFlagInfo;
      if (((IEnumerable<string>) DefaultFlags.DefaultFlagList).Contains<string>(flagUrl) || this.ServerFlags.TryGetValue(flagUrl, out extendedFlagInfo) && extendedFlagInfo.Owner != SettingsSystem.CurrentSettings.PlayerName || GameDatabase.Instance.GetTextureInfo(flagUrl) == null)
        return;
      string path = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", flagUrl + ".png");
      if (!File.Exists(path))
      {
        LunaLog.LogError("Cannot upload flag " + Path.GetFileName(flagUrl) + " file not found");
      }
      else
      {
        byte[] numArray = File.ReadAllBytes(path);
        if (numArray.Length > 1000000)
        {
          LunaLog.LogError("Cannot upload flag " + Path.GetFileName(flagUrl) + " size is greater than 1Mb!");
        }
        else
        {
          if (extendedFlagInfo != null && extendedFlagInfo.ShaSum == Common.CalculateSha256Hash(numArray))
            return;
          LunaLog.Log("[LMP]: Uploading " + Path.GetFileName(flagUrl) + " flag");
          this.MessageSender.SendMessage((IMessageData) this.MessageSender.GetFlagMessageData(flagUrl, numArray));
        }
      }
    }

    private void HandleFlag(ExtendedFlagInfo flagInfo)
    {
      if (this.FlagExists(flagInfo.FlagName))
        return;
      Texture2D texture2D = new Texture2D(4, 4);
      if (ImageConversion.LoadImage(texture2D, flagInfo.FlagData))
      {
        ((Object) texture2D).name = flagInfo.FlagName;
        GameDatabase.Instance.databaseTexture.Add(new GameDatabase.TextureInfo((UrlDir.UrlFile) null, texture2D, true, true, false)
        {
          name = flagInfo.FlagName
        });
        LunaLog.Log("[LMP]: Loaded flag " + ((Object) texture2D).name);
      }
      else
        LunaLog.LogError("[LMP]: Failed to load flag " + flagInfo.FlagName);
    }
  }
}
