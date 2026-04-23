// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselUndockMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselUndockMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public Guid NewVesselId;
    public string DockedInfoName;
    public uint DockedInfoRootPartUId;
    public int DockedInfoVesselType;

    internal VesselUndockMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Undock;

    public override string ClassName { get; } = nameof (VesselUndockMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
      GuidUtil.Serialize(this.NewVesselId, lidgrenMsg);
      lidgrenMsg.Write(this.DockedInfoName);
      lidgrenMsg.Write(this.DockedInfoRootPartUId);
      lidgrenMsg.Write(this.DockedInfoVesselType);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.NewVesselId = GuidUtil.Deserialize(lidgrenMsg);
      this.DockedInfoName = lidgrenMsg.ReadString();
      this.DockedInfoRootPartUId = lidgrenMsg.ReadUInt32();
      this.DockedInfoVesselType = lidgrenMsg.ReadInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 8 + 4 + GuidUtil.ByteSize + this.DockedInfoName.GetByteCount();
  }
}
