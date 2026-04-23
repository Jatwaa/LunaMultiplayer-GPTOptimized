// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareUpgradeableFacilities.ShareUpgradeableFacilitiesMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.ShareCareer;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgradeables;

namespace LmpClient.Systems.ShareUpgradeableFacilities
{
  public class ShareUpgradeableFacilitiesMessageHandler : 
    SubSystem<ShareUpgradeableFacilitiesSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.FacilityUpgrade || !(data is ShareProgressFacilityUpgradeMsgData facilityUpgradeMsgData))
        return;
      string facilityId = facilityUpgradeMsgData.FacilityId;
      int level = facilityUpgradeMsgData.Level;
      LunaLog.Log("Queue FacilityLevelUpdate.");
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => ShareUpgradeableFacilitiesMessageHandler.FacilityLevelUpdate(facilityId, level)));
    }

    private static void FacilityLevelUpdate(string facilityId, int newLevel)
    {
      SubSystem<ShareUpgradeableFacilitiesSystem>.System.StartIgnoringEvents();
      UpgradeableFacility upgradeableFacility = ((IEnumerable<UpgradeableFacility>) Object.FindObjectsOfType<UpgradeableFacility>()).FirstOrDefault<UpgradeableFacility>((Func<UpgradeableFacility, bool>) (o => ((UpgradeableObject) o).id == facilityId));
      if (Object.op_Inequality((Object) upgradeableFacility, (Object) null))
        ((UpgradeableObject) upgradeableFacility).SetLevel(newLevel);
      SubSystem<ShareUpgradeableFacilitiesSystem>.System.StopIgnoringEvents();
    }
  }
}
