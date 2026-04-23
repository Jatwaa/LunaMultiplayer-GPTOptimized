// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.VesselPositionMessageHandler
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

namespace LmpClient.Systems.VesselPositionSys
{
  public class VesselPositionMessageHandler : SubSystem<VesselPositionSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselPositionMsgData data))
        return;
      Guid vesselId = data.VesselId;
      if (!VesselCommon.DoVesselChecks(vesselId))
        return;
      if (!VesselPositionSystem.CurrentVesselUpdate.ContainsKey(vesselId))
      {
        VesselPositionSystem.CurrentVesselUpdate.TryAdd(vesselId, new VesselPositionUpdate(data));
        VesselPositionSystem.TargetVesselUpdateQueue.TryAdd(vesselId, new PositionUpdateQueue());
      }
      else
      {
        PositionUpdateQueue positionUpdateQueue;
        VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(vesselId, out positionUpdateQueue);
        positionUpdateQueue?.Enqueue(data);
      }
    }
  }
}
