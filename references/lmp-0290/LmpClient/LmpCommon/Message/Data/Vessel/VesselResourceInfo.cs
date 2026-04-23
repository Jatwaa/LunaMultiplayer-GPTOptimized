// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselResourceInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselResourceInfo
  {
    public uint PartFlightId;
    public string ResourceName;
    public double Amount;
    public bool FlowState;

    public VesselResourceInfo()
    {
    }

    public VesselResourceInfo(VesselResourceInfo resource) => this.CopyFrom(resource);

    public void CopyFrom(VesselResourceInfo resource)
    {
      this.PartFlightId = resource.PartFlightId;
      this.ResourceName = resource.ResourceName;
      this.Amount = resource.Amount;
      this.FlowState = resource.FlowState;
    }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.PartFlightId);
      lidgrenMsg.Write(this.ResourceName);
      lidgrenMsg.Write(this.Amount);
      lidgrenMsg.Write(this.FlowState);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.ResourceName = lidgrenMsg.ReadString();
      this.Amount = lidgrenMsg.ReadDouble();
      this.FlowState = lidgrenMsg.ReadBoolean();
    }

    public int GetByteCount() => 4 + this.ResourceName.GetByteCount() + 8 + 1;
  }
}
