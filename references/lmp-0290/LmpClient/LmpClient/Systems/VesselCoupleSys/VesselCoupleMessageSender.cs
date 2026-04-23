// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCoupleMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpCommon.Enums;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCoupleMessageSender : SubSystem<VesselCoupleSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselCouple(
      global::Vessel vessel,
      uint partFlightId,
      Guid coupledVesselId,
      uint coupledPartFlightId,
      CoupleTrigger trigger)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      VesselCoupleMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselCoupleMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = partFlightId;
      newMessageData.CoupledVesselId = coupledVesselId;
      newMessageData.CoupledPartFlightId = coupledPartFlightId;
      newMessageData.SubspaceId = LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspace;
      newMessageData.Trigger = (int) trigger;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
