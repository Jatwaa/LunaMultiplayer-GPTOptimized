// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareReputation.ShareReputationMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.ShareCareer;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ShareReputation
{
  public class ShareReputationMessageHandler : SubSystem<ShareReputationSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.ReputationUpdate || !(data is ShareProgressReputationMsgData reputationMsgData))
        return;
      float reputation = reputationMsgData.Reputation;
      LunaLog.Log(string.Format("Queue ReputationUpdate with: {0}", (object) reputation));
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => ShareReputationMessageHandler.ReputationUpdate(reputation)));
    }

    private static void ReputationUpdate(float reputation)
    {
      SubSystem<ShareReputationSystem>.System.SetReputationWithoutTriggeringEvent(reputation);
      LunaLog.Log(string.Format("ReputationUpdate received - reputation changed to: {0}", (object) reputation));
    }
  }
}
