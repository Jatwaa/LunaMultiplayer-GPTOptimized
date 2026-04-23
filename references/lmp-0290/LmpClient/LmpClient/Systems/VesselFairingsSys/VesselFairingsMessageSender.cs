// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFairingsSys.VesselFairingsMessageSender
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
using UnityEngine;

namespace LmpClient.Systems.VesselFairingsSys
{
  public class VesselFairingsMessageSender : SubSystem<VesselFairingsSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselFairingDeployed(global::Vessel vessel, uint partFlightId)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      VesselFairingMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselFairingMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = partFlightId;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
