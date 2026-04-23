// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerColorSys.PlayerColorMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Color;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.PlayerColorSys
{
  public class PlayerColorMessageSender : SubSystem<PlayerColorSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<PlayerColorCliMsg>(msg))));

    public void SendColorsRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<PlayerColorCliMsg, PlayerColorRequestMsgData>())));

    public void SendPlayerColorToServer()
    {
      PlayerColorSetMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<PlayerColorSetMsgData>();
      newMessageData.PlayerColor.PlayerName = SettingsSystem.CurrentSettings.PlayerName;
      newMessageData.PlayerColor.Color[0] = SettingsSystem.CurrentSettings.PlayerColor.r;
      newMessageData.PlayerColor.Color[1] = SettingsSystem.CurrentSettings.PlayerColor.g;
      newMessageData.PlayerColor.Color[2] = SettingsSystem.CurrentSettings.PlayerColor.b;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
