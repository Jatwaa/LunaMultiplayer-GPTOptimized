// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareFunds.ShareFundsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.ShareCareer;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ShareFunds
{
  public class ShareFundsMessageHandler : SubSystem<ShareFundsSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || (uint) data.ShareProgressMessageType > 0U || !(data is ShareProgressFundsMsgData progressFundsMsgData))
        return;
      double funds = progressFundsMsgData.Funds;
      LunaLog.Log(string.Format("Queue FundsUpdate with: {0}", (object) funds));
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => ShareFundsMessageHandler.FundsUpdate(funds)));
    }

    private static void FundsUpdate(double funds)
    {
      SubSystem<ShareFundsSystem>.System.SetFundsWithoutTriggeringEvent(funds);
      LunaLog.Log(string.Format("FundsUpdate received - funds changed to: {0}", (object) funds));
    }
  }
}
