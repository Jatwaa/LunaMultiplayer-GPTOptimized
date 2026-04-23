// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFairingsSys.VesselFairingsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselFairingsSys
{
  public class VesselFairingsMessageHandler : SubSystem<VesselFairingsSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselFairingMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselFairingsSystem>.System.VesselFairings.ContainsKey(data.VesselId))
        SubSystem<VesselFairingsSystem>.System.VesselFairings.TryAdd(data.VesselId, new VesselFairingQueue());
      VesselFairingQueue vesselFairingQueue;
      if (!SubSystem<VesselFairingsSystem>.System.VesselFairings.TryGetValue(data.VesselId, out vesselFairingQueue))
        return;
      VesselFairing result;
      if (vesselFairingQueue.TryPeek(out result) && result.GameTime > data.GameTime)
        vesselFairingQueue.Clear();
      vesselFairingQueue.Enqueue(data);
    }
  }
}
