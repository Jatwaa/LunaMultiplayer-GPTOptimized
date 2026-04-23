// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Status.StatusMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.PlayerStatus;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Status
{
  public class StatusMessageSender : SubSystem<StatusSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<PlayerStatusCliMsg>(msg))));

    public void SendPlayersRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<PlayerStatusCliMsg, PlayerStatusRequestMsgData>())));

    public void SendOwnStatus()
    {
      PlayerStatusSetMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<PlayerStatusSetMsgData>();
      newMessageData.PlayerStatus.PlayerName = SettingsSystem.CurrentSettings.PlayerName;
      newMessageData.PlayerStatus.StatusText = SubSystem<StatusSystem>.System.MyPlayerStatus.StatusText;
      newMessageData.PlayerStatus.VesselText = SubSystem<StatusSystem>.System.MyPlayerStatus.VesselText;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
