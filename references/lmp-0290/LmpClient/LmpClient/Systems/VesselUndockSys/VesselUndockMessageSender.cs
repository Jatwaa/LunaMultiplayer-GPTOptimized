// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUndockSys.VesselUndockMessageSender
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

namespace LmpClient.Systems.VesselUndockSys
{
  public class VesselUndockMessageSender : SubSystem<VesselUndockSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselUndock(
      global::Vessel vessel,
      uint partFlightId,
      DockedVesselInfo dockedInfo,
      Guid newVesselId)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      VesselUndockMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselUndockMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = partFlightId;
      newMessageData.NewVesselId = newVesselId;
      newMessageData.DockedInfoName = dockedInfo.name;
      newMessageData.DockedInfoRootPartUId = dockedInfo.rootPartUId;
      newMessageData.DockedInfoVesselType = (int) dockedInfo.vesselType;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
