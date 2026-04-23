// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Facility.FacilityEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Scenario;
using LmpClient.VesselUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.Facility
{
  public class FacilityEvents : SubSystem<FacilitySystem>
  {
    public void FacilityRepairing(DestructibleBuilding building)
    {
      if (SubSystem<FacilitySystem>.System.BuildingIdToIgnore == building.id)
        return;
      SubSystem<FacilitySystem>.System.DestroyedFacilities.Remove(building.id);
      SubSystem<FacilitySystem>.System.MessageSender.SendFacilityRepairMsg(building.id);
      LmpClient.Base.System<ScenarioSystem>.Singleton.SendScenarioModules();
    }

    public void FacilityCollapsing(DestructibleBuilding building)
    {
      if (SubSystem<FacilitySystem>.System.BuildingIdToIgnore == building.id || VesselCommon.IsSpectating)
        return;
      SubSystem<FacilitySystem>.System.DestroyedFacilities.Add(building.id);
      SubSystem<FacilitySystem>.System.MessageSender.SendFacilityCollapseMsg(building.id);
      LmpClient.Base.System<ScenarioSystem>.Singleton.SendScenarioModules();
    }

    public void FlightReady()
    {
      foreach (DestructibleBuilding building in ((IEnumerable<DestructibleBuilding>) Object.FindObjectsOfType<DestructibleBuilding>()).Where<DestructibleBuilding>((Func<DestructibleBuilding, bool>) (b => !b.IsDestroyed)))
      {
        if (SubSystem<FacilitySystem>.System.DestroyedFacilities.Contains(building.id))
          SubSystem<FacilitySystem>.System.CollapseFacilityWithoutSfx(building);
      }
    }
  }
}
