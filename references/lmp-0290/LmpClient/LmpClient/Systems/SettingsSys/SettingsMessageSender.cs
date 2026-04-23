// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.SettingsMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Settings;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.SettingsSys
{
  public class SettingsMessageSender : SubSystem<SettingsSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => throw new NotImplementedException("We never send settings in this way!");

    public void SendSettingsRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<SettingsCliMsg, SettingsRequestMsgData>())));
  }
}
