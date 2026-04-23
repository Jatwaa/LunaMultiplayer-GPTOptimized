// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Handshake.HandshakeReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Handshake
{
  public class HandshakeReplyMsgData : HandshakeBaseMsgData
  {
    public HandshakeReply Response;
    public string Reason;
    public bool ModControl;
    public long ServerStartTime;
    public string ModFileData;

    internal HandshakeReplyMsgData()
    {
    }

    public override HandshakeMessageType HandshakeMessageType => HandshakeMessageType.Reply;

    public override string ClassName { get; } = nameof (HandshakeReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write((int) this.Response);
      lidgrenMsg.Write(this.Reason);
      lidgrenMsg.Write(this.ModControl);
      lidgrenMsg.WritePadBits();
      lidgrenMsg.Write(this.ServerStartTime);
      lidgrenMsg.Write(this.ModFileData);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Response = (HandshakeReply) lidgrenMsg.ReadInt32();
      this.Reason = lidgrenMsg.ReadString();
      this.ModControl = lidgrenMsg.ReadBoolean();
      lidgrenMsg.SkipPadBits();
      this.ServerStartTime = lidgrenMsg.ReadInt64();
      this.ModFileData = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + this.Reason.GetByteCount() + 1 + 8 + this.ModFileData.GetByteCount();
  }
}
