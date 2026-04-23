// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselDecoupleMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselDecoupleMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public float BreakForce;
    public Guid NewVesselId;

    internal VesselDecoupleMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Decouple;

    public override string ClassName { get; } = nameof (VesselDecoupleMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
      lidgrenMsg.Write(this.BreakForce);
      GuidUtil.Serialize(this.NewVesselId, lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.BreakForce = lidgrenMsg.ReadFloat();
      this.NewVesselId = GuidUtil.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + 4 + GuidUtil.ByteSize;
  }
}
