// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Admin.AdminBaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Admin
{
  public abstract class AdminBaseMsgData : MessageData
  {
    public string AdminPassword;

    internal AdminBaseMsgData()
    {
    }

    public override ushort SubType => (ushort) this.AdminMessageType;

    public virtual AdminMessageType AdminMessageType => throw new NotImplementedException();

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg) => lidgrenMsg.Write(this.AdminPassword);

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg) => this.AdminPassword = lidgrenMsg.ReadString();

    internal override int InternalGetMessageSize() => this.AdminPassword.GetByteCount();
  }
}
