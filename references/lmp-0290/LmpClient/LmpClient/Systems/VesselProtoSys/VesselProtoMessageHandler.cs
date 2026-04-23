// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProtoMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.VesselRemoveSys;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProtoMessageHandler : SubSystem<VesselProtoSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselProtoMsgData data) || LmpClient.Base.System<VesselRemoveSystem>.Singleton.VesselWillBeKilled(data.VesselId))
        return;
      if (!SubSystem<VesselProtoSystem>.System.VesselProtos.ContainsKey(data.VesselId))
        SubSystem<VesselProtoSystem>.System.VesselProtos.TryAdd(data.VesselId, new VesselProtoQueue());
      VesselProtoQueue vesselProtoQueue;
      if (!SubSystem<VesselProtoSystem>.System.VesselProtos.TryGetValue(data.VesselId, out vesselProtoQueue))
        return;
      vesselProtoQueue.Enqueue(data);
    }
  }
}
