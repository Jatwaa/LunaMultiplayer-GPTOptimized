// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselActionGroupSys.VesselActionGroupMessageSender
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

namespace LmpClient.Systems.VesselActionGroupSys
{
  public class VesselActionGroupMessageSender : SubSystem<VesselActionGroupSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselActionGroup(global::Vessel vessel, KSPActionGroup actionGrp, bool value)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      VesselActionGroupMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselActionGroupMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.ActionGroupString = actionGrp.ToString();
      newMessageData.ActionGroup = (int) actionGrp;
      newMessageData.Value = value;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
