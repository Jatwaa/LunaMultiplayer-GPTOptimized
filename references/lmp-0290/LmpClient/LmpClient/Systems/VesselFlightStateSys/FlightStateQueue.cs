// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.FlightStateQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Extensions;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class FlightStateQueue : 
    CachedConcurrentQueue<VesselFlightStateUpdate, VesselFlightStateMsgData>
  {
    protected override void AssignFromMessage(
      VesselFlightStateUpdate value,
      VesselFlightStateMsgData msgData)
    {
      value.VesselId = msgData.VesselId;
      value.GameTimeStamp = msgData.GameTime;
      value.SubspaceId = msgData.SubspaceId;
      value.PingSec = msgData.PingSec;
      value.CtrlState.CopyFrom(msgData);
    }
  }
}
