// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselDecoupleSys.VesselDecoupleQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselDecoupleSys
{
  public class VesselDecoupleQueue : CachedConcurrentQueue<VesselDecouple, VesselDecoupleMsgData>
  {
    protected override void AssignFromMessage(VesselDecouple value, VesselDecoupleMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.PartFlightId = msgData.PartFlightId;
      value.BreakForce = msgData.BreakForce;
      value.NewVesselId = msgData.NewVesselId;
    }
  }
}
