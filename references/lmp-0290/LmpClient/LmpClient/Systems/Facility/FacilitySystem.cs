// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Facility.FacilitySystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.Facility
{
  public class FacilitySystem : 
    MessageSystem<FacilitySystem, FacilityMessageSender, FacilityMessageHandler>
  {
    public readonly HashSet<string> DestroyedFacilities = new HashSet<string>();
    public string BuildingIdToIgnore;

    public FacilityEvents FacilityEvents { get; } = new FacilityEvents();

    public override string SystemName { get; } = nameof (FacilitySystem);

    protected override void OnEnabled()
    {
      // ISSUE: method pointer
      GameEvents.OnKSCStructureRepairing.Add(new EventData<DestructibleBuilding>.OnEvent((object) this.FacilityEvents, __methodptr(FacilityRepairing)));
      // ISSUE: method pointer
      GameEvents.OnKSCStructureCollapsing.Add(new EventData<DestructibleBuilding>.OnEvent((object) this.FacilityEvents, __methodptr(FacilityCollapsing)));
      // ISSUE: method pointer
      GameEvents.onFlightReady.Add(new EventVoid.OnEvent((object) this.FacilityEvents, __methodptr(FlightReady)));
      base.OnEnabled();
    }

    protected override void OnDisabled()
    {
      this.DestroyedFacilities.Clear();
      // ISSUE: method pointer
      GameEvents.OnKSCStructureRepaired.Remove(new EventData<DestructibleBuilding>.OnEvent((object) this.FacilityEvents, __methodptr(FacilityRepairing)));
      // ISSUE: method pointer
      GameEvents.OnKSCStructureCollapsing.Remove(new EventData<DestructibleBuilding>.OnEvent((object) this.FacilityEvents, __methodptr(FacilityCollapsing)));
      // ISSUE: method pointer
      GameEvents.onFlightReady.Remove(new EventVoid.OnEvent((object) this.FacilityEvents, __methodptr(FlightReady)));
      base.OnDisabled();
    }

    public void RepairFacilityWithoutSendingMessage(DestructibleBuilding building)
    {
      if (Object.op_Equality((Object) building, (Object) null) || building.IsIntact || !building.IsDestroyed)
        return;
      this.BuildingIdToIgnore = building.id;
      building.Repair();
      this.BuildingIdToIgnore = string.Empty;
    }

    public void CollapseFacilityWithoutSendingMessage(DestructibleBuilding building)
    {
      if (Object.op_Equality((Object) building, (Object) null) || !building.IsIntact || building.IsDestroyed)
        return;
      this.BuildingIdToIgnore = building.id;
      building.Demolish();
      this.BuildingIdToIgnore = string.Empty;
    }

    public void CollapseFacilityWithoutSfx(DestructibleBuilding building)
    {
      if (Object.op_Equality((Object) building, (Object) null) || !building.IsIntact || building.IsDestroyed)
        return;
      this.BuildingIdToIgnore = building.id;
      foreach (DestructibleBuilding.CollapsibleObject collapsibleObject in building.CollapsibleObjects)
        collapsibleObject.SetDestroyed(true);
      Traverse.Create((object) building).Field("destroyed").SetValue((object) true);
      Traverse.Create((object) building).Field("intact").SetValue((object) false);
      this.BuildingIdToIgnore = string.Empty;
    }
  }
}
