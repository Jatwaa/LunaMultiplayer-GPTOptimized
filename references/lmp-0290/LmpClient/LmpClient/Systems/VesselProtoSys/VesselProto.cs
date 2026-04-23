// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProto
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.VesselUtilities;
using System;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProto
  {
    public Guid VesselId;
    public byte[] RawData = new byte[0];
    public int NumBytes;
    public double GameTime;
    public bool ForceReload;

    public Vessel LoadVessel() => (Vessel) null;

    public ProtoVessel CreateProtoVessel()
    {
      ConfigNode configNode = this.RawData.DeserializeToConfigNode(this.NumBytes);
      if (configNode == null || configNode.VesselHasNaNPosition())
      {
        LunaLog.LogError(string.Format("Received a malformed vessel from SERVER. Id {0}", (object) this.VesselId));
        LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(this.VesselId, true, "Malformed vessel");
        return (ProtoVessel) null;
      }
      ProtoVessel vesselFromConfigNode = VesselSerializer.CreateSafeProtoVesselFromConfigNode(configNode, this.VesselId);
      if (vesselFromConfigNode != null)
        return vesselFromConfigNode;
      LunaLog.LogError(string.Format("Received a malformed vessel from SERVER. Id {0}", (object) this.VesselId));
      LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(this.VesselId, true, "Malformed vessel");
      return (ProtoVessel) null;
    }
  }
}
