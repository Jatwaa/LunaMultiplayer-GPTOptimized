// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Admin.AdminMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.Admin;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Admin
{
  public class AdminMessageHandler : SubSystem<AdminSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is AdminBaseMsgData data))
        return;
      if (data.AdminMessageType != AdminMessageType.Reply)
        throw new ArgumentOutOfRangeException();
      LunaScreenMsg.PostScreenMessage(string.Format("Admin command reply: {0}", (object) ((AdminReplyMsgData) data).Response), 5f, (ScreenMessageStyle) 2);
    }
  }
}
