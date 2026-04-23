// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselCoupleMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselCoupleMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public Guid CoupledVesselId;
    public uint CoupledPartFlightId;
    public int SubspaceId;
    public int Trigger;

    internal VesselCoupleMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Couple;

    public override string ClassName { get; } = nameof (VesselCoupleMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
      GuidUtil.Serialize(this.CoupledVesselId, lidgrenMsg);
      lidgrenMsg.Write(this.CoupledPartFlightId);
      lidgrenMsg.Write(this.SubspaceId);
      lidgrenMsg.Write(this.Trigger);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.CoupledVesselId = GuidUtil.Deserialize(lidgrenMsg);
      this.CoupledPartFlightId = lidgrenMsg.ReadUInt32();
      this.SubspaceId = lidgrenMsg.ReadInt32();
      this.Trigger = lidgrenMsg.ReadInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 8 + GuidUtil.ByteSize + 8;
  }
}
