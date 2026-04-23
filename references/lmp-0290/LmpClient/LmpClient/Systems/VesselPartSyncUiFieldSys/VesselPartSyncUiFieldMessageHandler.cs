// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiFieldMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiFieldMessageHandler : 
    SubSystem<VesselPartSyncUiFieldSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselPartSyncUiFieldMsgData data) || !VesselCommon.DoVesselChecks(data.VesselId))
        return;
      if (!SubSystem<VesselPartSyncUiFieldSystem>.System.VesselPartsUiFieldsSyncs.ContainsKey(data.VesselId))
        SubSystem<VesselPartSyncUiFieldSystem>.System.VesselPartsUiFieldsSyncs.TryAdd(data.VesselId, new VesselPartSyncUiFieldQueue());
      VesselPartSyncUiFieldQueue syncUiFieldQueue;
      if (!SubSystem<VesselPartSyncUiFieldSystem>.System.VesselPartsUiFieldsSyncs.TryGetValue(data.VesselId, out syncUiFieldQueue))
        return;
      syncUiFieldQueue.Enqueue(data);
    }
  }
}
