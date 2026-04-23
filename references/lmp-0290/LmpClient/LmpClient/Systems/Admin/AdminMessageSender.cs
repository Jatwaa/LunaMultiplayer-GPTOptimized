// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Admin.AdminMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Admin;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Admin
{
  public class AdminMessageSender : SubSystem<AdminSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<AdminCliMsg>(msg))));

    public void SendBanPlayerMsg(string playerName, string reason)
    {
      AdminBanMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<AdminBanMsgData>();
      newMessageData.AdminPassword = SubSystem<AdminSystem>.System.AdminPassword;
      newMessageData.PlayerName = playerName;
      newMessageData.Reason = reason;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendKickPlayerMsg(string playerName, string reason)
    {
      AdminKickMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<AdminKickMsgData>();
      newMessageData.AdminPassword = SubSystem<AdminSystem>.System.AdminPassword;
      newMessageData.PlayerName = playerName;
      newMessageData.Reason = reason;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendNukeMsg()
    {
      AdminNukeMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<AdminNukeMsgData>();
      newMessageData.AdminPassword = SubSystem<AdminSystem>.System.AdminPassword;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendDekesslerMsg()
    {
      AdminDekesslerMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<AdminDekesslerMsgData>();
      newMessageData.AdminPassword = SubSystem<AdminSystem>.System.AdminPassword;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendServerRestartMsg()
    {
      AdminRestartServerMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<AdminRestartServerMsgData>();
      newMessageData.AdminPassword = SubSystem<AdminSystem>.System.AdminPassword;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
