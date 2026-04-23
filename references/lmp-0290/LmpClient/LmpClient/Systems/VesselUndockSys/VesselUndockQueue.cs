// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUndockSys.VesselUndockQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselUndockSys
{
  public class VesselUndockQueue : CachedConcurrentQueue<VesselUndock, VesselUndockMsgData>
  {
    protected override void AssignFromMessage(VesselUndock value, VesselUndockMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.PartFlightId = msgData.PartFlightId;
      value.NewVesselId = msgData.NewVesselId;
      value.DockedInfo = new DockedVesselInfo()
      {
        name = msgData.DockedInfoName.Clone() as string,
        rootPartUId = msgData.DockedInfoRootPartUId,
        vesselType = (VesselType) msgData.DockedInfoVesselType
      };
    }
  }
}
