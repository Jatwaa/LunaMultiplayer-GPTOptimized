// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselRemoveSys.VesselRemoveMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselRemoveSys
{
  public class VesselRemoveMessageSender : SubSystem<VesselRemoveSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg))));

    public void SendVesselRemove(global::Vessel vessel, bool keepVesselInRemoveList = true)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      this.SendVesselRemove(vessel.id, keepVesselInRemoveList);
    }

    public void SendVesselRemove(Guid vesselId, bool keepVesselInRemoveList = true)
    {
      LunaLog.Log(string.Format("[LMP]: Removing {0} from the server", (object) vesselId));
      VesselRemoveMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselRemoveMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vesselId;
      newMessageData.AddToKillList = keepVesselInRemoveList;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
