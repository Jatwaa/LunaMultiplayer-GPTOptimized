// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Flag.FlagMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Flag;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Flag
{
  public class FlagMessageSender : SubSystem<FlagSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<FlagCliMsg>(msg))));

    public void SendFlagsRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<FlagCliMsg, FlagListRequestMsgData>())));

    public FlagDataMsgData GetFlagMessageData(string flagName, byte[] flagData)
    {
      FlagDataMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<FlagDataMsgData>();
      newMessageData.Flag.Owner = SettingsSystem.CurrentSettings.PlayerName;
      newMessageData.Flag.FlagName = flagName;
      newMessageData.Flag.NumBytes = flagData.Length;
      if (newMessageData.Flag.FlagData.Length < flagData.Length)
        newMessageData.Flag.FlagData = new byte[flagData.Length];
      Array.Copy((Array) flagData, (Array) newMessageData.Flag.FlagData, flagData.Length);
      return newMessageData;
    }
  }
}
