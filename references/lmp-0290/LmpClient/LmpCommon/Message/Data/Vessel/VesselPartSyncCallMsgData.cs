// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselPartSyncCallMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselPartSyncCallMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public string ModuleName;
    public string MethodName;

    internal VesselPartSyncCallMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.PartSyncCall;

    public override string ClassName { get; } = nameof (VesselPartSyncCallMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
      lidgrenMsg.Write(this.ModuleName);
      lidgrenMsg.Write(this.MethodName);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.ModuleName = lidgrenMsg.ReadString();
      this.MethodName = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + this.ModuleName.GetByteCount() + this.MethodName.GetByteCount();
  }
}
