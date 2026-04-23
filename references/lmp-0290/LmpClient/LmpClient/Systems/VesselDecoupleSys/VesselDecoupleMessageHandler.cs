// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselDecoupleSys.VesselDecoupleMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselDecoupleSys
{
  public class VesselDecoupleMessageHandler : SubSystem<VesselDecoupleSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselDecoupleMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselDecoupleSystem>.System.VesselDecouples.ContainsKey(data.VesselId))
        SubSystem<VesselDecoupleSystem>.System.VesselDecouples.TryAdd(data.VesselId, new VesselDecoupleQueue());
      VesselDecoupleQueue vesselDecoupleQueue;
      if (!SubSystem<VesselDecoupleSystem>.System.VesselDecouples.TryGetValue(data.VesselId, out vesselDecoupleQueue))
        return;
      VesselDecouple result;
      if (vesselDecoupleQueue.TryPeek(out result) && result.GameTime > data.GameTime)
        vesselDecoupleQueue.Clear();
      vesselDecoupleQueue.Enqueue(data);
    }
  }
}
