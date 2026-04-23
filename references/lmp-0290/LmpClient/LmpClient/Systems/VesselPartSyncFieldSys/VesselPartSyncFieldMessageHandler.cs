// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncFieldMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncFieldMessageHandler : 
    SubSystem<VesselPartSyncFieldSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselPartSyncFieldMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselPartSyncFieldSystem>.System.VesselPartsSyncs.ContainsKey(data.VesselId))
        SubSystem<VesselPartSyncFieldSystem>.System.VesselPartsSyncs.TryAdd(data.VesselId, new VesselPartSyncFieldQueue());
      VesselPartSyncFieldQueue partSyncFieldQueue;
      if (!SubSystem<VesselPartSyncFieldSystem>.System.VesselPartsSyncs.TryGetValue(data.VesselId, out partSyncFieldQueue))
        return;
      partSyncFieldQueue.Enqueue(data);
    }
  }
}
