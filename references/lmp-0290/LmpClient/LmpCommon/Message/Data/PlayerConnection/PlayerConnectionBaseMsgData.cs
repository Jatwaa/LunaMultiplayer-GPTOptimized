// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.PlayerConnection.PlayerConnectionBaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.PlayerConnection
{
  public abstract class PlayerConnectionBaseMsgData : MessageData
  {
    public string PlayerName;

    internal PlayerConnectionBaseMsgData()
    {
    }

    public override ushort SubType => (ushort) this.PlayerConnectionMessageType;

    public virtual PlayerConnectionMessageType PlayerConnectionMessageType => throw new NotImplementedException();

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg) => lidgrenMsg.Write(this.PlayerName);

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg) => this.PlayerName = lidgrenMsg.ReadString();

    internal override int InternalGetMessageSize() => this.PlayerName.GetByteCount();
  }
}
