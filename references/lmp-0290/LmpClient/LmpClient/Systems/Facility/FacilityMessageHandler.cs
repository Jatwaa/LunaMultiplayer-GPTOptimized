// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Facility.FacilityMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.Facility;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.Facility
{
  public class FacilityMessageHandler : SubSystem<FacilitySystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      FacilityBaseMsgData msgData = msg.Data as FacilityBaseMsgData;
      if (msgData == null)
        return;
      DestructibleBuilding building = ((IEnumerable<DestructibleBuilding>) Object.FindObjectsOfType<DestructibleBuilding>()).FirstOrDefault<DestructibleBuilding>((Func<DestructibleBuilding, bool>) (o => o.id == msgData.ObjectId));
      if (!Object.op_Inequality((Object) building, (Object) null))
        return;
      switch (msgData.FacilityMessageType)
      {
        case FacilityMessageType.Repair:
          SubSystem<FacilitySystem>.System.DestroyedFacilities.Remove(building.id);
          SubSystem<FacilitySystem>.System.RepairFacilityWithoutSendingMessage(building);
          break;
        case FacilityMessageType.Collapse:
          SubSystem<FacilitySystem>.System.DestroyedFacilities.Add(building.id);
          SubSystem<FacilitySystem>.System.CollapseFacilityWithoutSendingMessage(building);
          break;
      }
    }
  }
}
