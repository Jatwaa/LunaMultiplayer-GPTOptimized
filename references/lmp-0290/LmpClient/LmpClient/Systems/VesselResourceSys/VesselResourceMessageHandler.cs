// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselResourceSys.VesselResourceMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselResourceSys
{
  public class VesselResourceMessageHandler : SubSystem<VesselResourceSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselResourceMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselResourceSystem>.System.VesselResources.ContainsKey(data.VesselId))
        SubSystem<VesselResourceSystem>.System.VesselResources.TryAdd(data.VesselId, new VesselResourceQueue());
      VesselResourceQueue vesselResourceQueue;
      if (!SubSystem<VesselResourceSystem>.System.VesselResources.TryGetValue(data.VesselId, out vesselResourceQueue))
        return;
      vesselResourceQueue.Enqueue(data);
    }
  }
}
