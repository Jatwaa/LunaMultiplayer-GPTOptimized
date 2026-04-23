// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareReputation.ShareReputationMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.ShareReputation
{
  public class ShareReputationMessageSender : SubSystem<ShareReputationSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendReputationMsg(float reputation, string reason)
    {
      ShareProgressReputationMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressReputationMsgData>();
      newMessageData.Reputation = reputation;
      newMessageData.Reason = reason;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
