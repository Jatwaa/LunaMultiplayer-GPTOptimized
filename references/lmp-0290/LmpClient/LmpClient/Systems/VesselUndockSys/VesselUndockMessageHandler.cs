// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUndockSys.VesselUndockMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselUndockSys
{
  public class VesselUndockMessageHandler : SubSystem<VesselUndockSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselUndockMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselUndockSystem>.System.VesselUndocks.ContainsKey(data.VesselId))
        SubSystem<VesselUndockSystem>.System.VesselUndocks.TryAdd(data.VesselId, new VesselUndockQueue());
      VesselUndockQueue vesselUndockQueue;
      if (!SubSystem<VesselUndockSystem>.System.VesselUndocks.TryGetValue(data.VesselId, out vesselUndockQueue))
        return;
      VesselUndock result;
      if (vesselUndockQueue.TryPeek(out result) && result.GameTime > data.GameTime)
        vesselUndockQueue.Clear();
      vesselUndockQueue.Enqueue(data);
    }
  }
}
