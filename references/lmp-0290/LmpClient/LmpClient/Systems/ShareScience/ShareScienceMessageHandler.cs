// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScience.ShareScienceMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ShareScience
{
  public class ShareScienceMessageHandler : SubSystem<ShareScienceSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.ScienceUpdate || !(data is ShareProgressScienceMsgData progressScienceMsgData))
        return;
      float science = progressScienceMsgData.Science;
      LunaLog.Log(string.Format("Queue ScienceUpdate with: {0}", (object) science));
      SubSystem<ShareScienceSystem>.System.QueueAction((Action) (() => ShareScienceMessageHandler.ScienceUpdate(science)));
    }

    private static void ScienceUpdate(float science)
    {
      SubSystem<ShareScienceSystem>.System.SetScienceWithoutTriggeringEvent(science);
      LunaLog.Log(string.Format("ScienceUpdate received - science changed to: {0}", (object) science));
    }
  }
}
