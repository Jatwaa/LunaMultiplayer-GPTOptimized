// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Handshake.HandshakeRequestMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Handshake
{
  public class HandshakeRequestMsgData : HandshakeBaseMsgData
  {
    public string PlayerName;
    public string UniqueIdentifier;

    internal HandshakeRequestMsgData()
    {
    }

    public override HandshakeMessageType HandshakeMessageType => HandshakeMessageType.Request;

    public override string ClassName { get; } = nameof (HandshakeRequestMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PlayerName);
      lidgrenMsg.Write(this.UniqueIdentifier);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerName = lidgrenMsg.ReadString();
      this.UniqueIdentifier = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PlayerName.GetByteCount() + this.UniqueIdentifier.GetByteCount();
  }
}
