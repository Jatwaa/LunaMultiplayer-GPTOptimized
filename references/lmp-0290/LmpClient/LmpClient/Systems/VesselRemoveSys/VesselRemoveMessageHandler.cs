// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselRemoveSys.VesselRemoveMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;
using UnityEngine;

namespace LmpClient.Systems.VesselRemoveSys
{
  public class VesselRemoveMessageHandler : SubSystem<VesselRemoveSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is VesselRemoveMsgData data) || !VesselCommon.IsSpectating && Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == data.VesselId)
        return;
      SubSystem<VesselRemoveSystem>.System.KillVessel(data.VesselId, data.AddToKillList, "Received a vessel remove message from server");
    }
  }
}
