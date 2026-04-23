// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCoupleQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCoupleQueue : CachedConcurrentQueue<VesselCouple, VesselCoupleMsgData>
  {
    protected override void AssignFromMessage(VesselCouple value, VesselCoupleMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.PartFlightId = msgData.PartFlightId;
      value.CoupledPartFlightId = msgData.CoupledPartFlightId;
      value.CoupledVesselId = msgData.CoupledVesselId;
      value.Trigger = (CoupleTrigger) msgData.Trigger;
    }
  }
}
