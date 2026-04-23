// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareUpgradeableFacilities.ShareUpgradeableFacilitiesMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.ShareUpgradeableFacilities
{
  public class ShareUpgradeableFacilitiesMessageSender : 
    SubSystem<ShareUpgradeableFacilitiesSystem>,
    IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendFacilityUpgradeMessage(string facilityId, int level, float normLevel)
    {
      ShareProgressFacilityUpgradeMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressFacilityUpgradeMsgData>();
      newMessageData.FacilityId = facilityId;
      newMessageData.Level = level;
      newMessageData.NormLevel = normLevel;
      SubSystem<ShareUpgradeableFacilitiesSystem>.System.MessageSender.SendMessage((IMessageData) newMessageData);
    }
  }
}
