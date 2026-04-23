// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUpdateSys.VesselUpdateMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselUpdateSys
{
  public class VesselUpdateMessageHandler : SubSystem<VesselUpdateSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselUpdateMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselUpdateSystem>.System.VesselUpdates.ContainsKey(data.VesselId))
        SubSystem<VesselUpdateSystem>.System.VesselUpdates.TryAdd(data.VesselId, new VesselUpdateQueue());
      VesselUpdateQueue vesselUpdateQueue;
      if (!SubSystem<VesselUpdateSystem>.System.VesselUpdates.TryGetValue(data.VesselId, out vesselUpdateQueue))
        return;
      vesselUpdateQueue.Enqueue(data);
    }
  }
}
