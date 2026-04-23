// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Chat.ChatMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Chat
{
  public class ChatMsgData : MessageData
  {
    public string From;
    public string Text;
    public bool Relay;

    internal ChatMsgData()
    {
    }

    public override string ClassName { get; } = nameof (ChatMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.From);
      lidgrenMsg.Write(this.Text);
      lidgrenMsg.Write(this.Relay);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      this.From = lidgrenMsg.ReadString();
      this.Text = lidgrenMsg.ReadString();
      this.Relay = lidgrenMsg.ReadBoolean();
    }

    internal override int InternalGetMessageSize() => this.From.GetByteCount() + this.Text.GetByteCount() + 1;
  }
}
