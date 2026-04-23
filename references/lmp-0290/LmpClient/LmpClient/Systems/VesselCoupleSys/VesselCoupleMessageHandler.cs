// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCoupleMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Systems.Warp;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;
using UnityEngine;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCoupleMessageHandler : SubSystem<VesselCoupleSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselCoupleMsgData data) || LmpClient.Base.System<VesselRemoveSystem>.Singleton.VesselWillBeKilled(data.VesselId))
        return;
      if (Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && (FlightGlobals.ActiveVessel.id == data.VesselId || FlightGlobals.ActiveVessel.id == data.CoupledVesselId))
      {
        LunaLog.Log("Received a coupling against our own vessel! We own the " + (FlightGlobals.ActiveVessel.id == data.VesselId ? "Dominant" : "Weak") + " vessel");
        LmpClient.Base.System<WarpSystem>.Singleton.WarpIfSubspaceIsMoreAdvanced(data.SubspaceId);
      }
      if (!SubSystem<VesselCoupleSystem>.System.VesselCouples.ContainsKey(data.VesselId))
        SubSystem<VesselCoupleSystem>.System.VesselCouples.TryAdd(data.VesselId, new VesselCoupleQueue());
      VesselCoupleQueue vesselCoupleQueue;
      if (!SubSystem<VesselCoupleSystem>.System.VesselCouples.TryGetValue(data.VesselId, out vesselCoupleQueue))
        return;
      VesselCouple result;
      if (vesselCoupleQueue.TryPeek(out result) && result.GameTime > data.GameTime)
        vesselCoupleQueue.Clear();
      if (data.GameTime <= TimeSyncSystem.UniversalTime)
        VesselCouple.ProcessCouple(data);
      else
        vesselCoupleQueue.Enqueue(data);
    }
  }
}
