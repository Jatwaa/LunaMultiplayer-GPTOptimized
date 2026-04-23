// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.VesselFlightStateMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class VesselFlightStateMessageHandler : SubSystem<VesselFlightStateSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselFlightStateMsgData data))
        return;
      Guid vesselId = data.VesselId;
      if (!VesselCommon.DoVesselChecks(vesselId) || !SubSystem<VesselFlightStateSystem>.System.FlightStateSystemReady || !SubSystem<VesselFlightStateSystem>.System.FlyByWireDictionary.ContainsKey(vesselId))
        return;
      if (!VesselFlightStateSystem.CurrentFlightState.ContainsKey(vesselId))
      {
        VesselFlightStateSystem.CurrentFlightState.TryAdd(vesselId, new VesselFlightStateUpdate(data));
        VesselFlightStateSystem.TargetFlightStateQueue.TryAdd(vesselId, new FlightStateQueue());
      }
      else
      {
        FlightStateQueue flightStateQueue;
        VesselFlightStateSystem.TargetFlightStateQueue.TryGetValue(vesselId, out flightStateQueue);
        flightStateQueue?.Enqueue(data);
      }
    }
  }
}
