// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Facility.FacilityMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Facility;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Facility
{
  public class FacilityMessageSender : SubSystem<FacilitySystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<FacilityCliMsg>(msg))));

    public void SendFacilityCollapseMsg(string objectId)
    {
      FacilityCollapseMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<FacilityCollapseMsgData>();
      newMessageData.ObjectId = objectId;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendFacilityRepairMsg(string objectId)
    {
      FacilityRepairMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<FacilityRepairMsgData>();
      newMessageData.ObjectId = objectId;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
