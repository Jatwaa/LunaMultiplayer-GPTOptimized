// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Chat.ChatMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace LmpClient.Systems.Chat
{
  public class ChatMessageHandler : SubSystem<ChatSystem>, IMessageHandler
  {
    private StringBuilder sb = new StringBuilder();

    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ChatMsgData data))
        return;
      this.sb.Length = 0;
      this.sb.Append(data.From).Append(": ").Append(data.Text);
      SubSystem<ChatSystem>.System.NewChatMessages.Enqueue(new Tuple<string, string, string>(data.From, data.Text, this.sb.ToString()));
    }
  }
}
