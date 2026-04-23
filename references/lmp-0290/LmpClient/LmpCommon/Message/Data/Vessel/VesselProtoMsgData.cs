// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselProtoMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselProtoMsgData : VesselBaseMsgData
  {
    public int NumBytes;
    public byte[] Data = new byte[0];
    public bool ForceReload;

    internal VesselProtoMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Proto;

    public override string ClassName { get; } = nameof (VesselProtoMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.ForceReload);
      Common.ThreadSafeCompress((object) this, ref this.Data, ref this.NumBytes);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.Data, 0, this.NumBytes);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ForceReload = lidgrenMsg.ReadBoolean();
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.Data, 0, this.NumBytes);
      Common.ThreadSafeDecompress((object) this, ref this.Data, this.NumBytes, out this.NumBytes);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 1 + 4 + this.NumBytes;
  }
}
