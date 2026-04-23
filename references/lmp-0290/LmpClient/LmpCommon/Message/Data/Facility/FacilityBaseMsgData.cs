// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Facility.FacilityBaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Facility
{
  public abstract class FacilityBaseMsgData : MessageData
  {
    public string ObjectId;

    internal FacilityBaseMsgData()
    {
    }

    public override ushort SubType => (ushort) this.FacilityMessageType;

    public virtual FacilityMessageType FacilityMessageType => throw new NotImplementedException();

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg) => lidgrenMsg.Write(this.ObjectId);

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg) => this.ObjectId = lidgrenMsg.ReadString();

    internal override int InternalGetMessageSize() => this.ObjectId.GetByteCount();
  }
}
