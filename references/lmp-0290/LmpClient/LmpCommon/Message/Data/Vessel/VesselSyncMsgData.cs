// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselSyncMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselSyncMsgData : VesselBaseMsgData
  {
    public int VesselsCount;
    public Guid[] VesselIds = new Guid[0];

    internal VesselSyncMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Sync;

    public override string ClassName { get; } = nameof (VesselSyncMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.VesselsCount);
      for (int index = 0; index < this.VesselsCount; ++index)
        GuidUtil.Serialize(this.VesselIds[index], lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.VesselsCount = lidgrenMsg.ReadInt32();
      if (this.VesselIds.Length < this.VesselsCount)
        this.VesselIds = new Guid[this.VesselsCount];
      for (int index = 0; index < this.VesselsCount; ++index)
        this.VesselIds[index] = GuidUtil.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + GuidUtil.ByteSize * this.VesselsCount;
  }
}
