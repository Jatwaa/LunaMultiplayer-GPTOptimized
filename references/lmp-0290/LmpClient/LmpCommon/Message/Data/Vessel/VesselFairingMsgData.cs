// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselFairingMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselFairingMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;

    internal VesselFairingMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Fairing;

    public override string ClassName { get; } = nameof (VesselFairingMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4;
  }
}
