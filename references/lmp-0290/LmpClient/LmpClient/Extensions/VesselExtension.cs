// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.VesselExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using System;
using UnityEngine;

namespace LmpClient.Extensions
{
  public static class VesselExtension
  {
    public static bool IsCometOrAsteroid(this Vessel vessel) => vessel.IsComet() || vessel.IsAsteroid();

    public static bool IsComet(this Vessel vessel) => Object.op_Inequality((Object) vessel, (Object) null) && !vessel.loaded ? vessel.protoVessel.IsCometOrAsteroid() : Object.op_Implicit((Object) vessel) && vessel.parts != null && vessel.parts.Count == 1 && vessel.parts[0].partName == "PotatoComet";

    public static bool IsAsteroid(this Vessel vessel) => Object.op_Inequality((Object) vessel, (Object) null) && !vessel.loaded ? vessel.protoVessel.IsCometOrAsteroid() : Object.op_Implicit((Object) vessel) && vessel.parts != null && vessel.parts.Count == 1 && vessel.parts[0].partName == "PotatoRoid";

    public static void AdvanceShipPosition(this Vessel vessel, double time)
    {
      if (vessel.situation <= 8)
        return;
      Vector3d relativePositionAtUt = vessel.orbit.getRelativePositionAtUT(time);
      Vector3d orbitalVelocityAtUt = vessel.orbit.getOrbitalVelocityAtUT(time);
      if (!vessel.packed)
        vessel.GoOnRails();
      vessel.orbit.UpdateFromStateVectors(relativePositionAtUt, orbitalVelocityAtUt, vessel.mainBody, time);
      vessel.orbitDriver.updateFromParameters();
      OrbitPhysicsManager.CheckReferenceFrame();
      OrbitPhysicsManager.HoldVesselUnpack(10);
      vessel.IgnoreGForces(20);
    }

    public static float GetShipCosts(this Vessel vessel, out float dryCost, out float fuelCost)
    {
      dryCost = 0.0f;
      fuelCost = 0.0f;
      foreach (Part part in vessel.parts)
      {
        float num1 = part.partInfo.cost + part.GetModuleCosts(part.partInfo.cost, (ModifierStagingSituation) 0);
        float num2 = 0.0f;
        foreach (PartResource resource in part.Resources)
        {
          num1 -= resource.info.unitCost * (float) resource.maxAmount;
          num2 += resource.info.unitCost * (float) resource.amount;
        }
        dryCost += num1;
        fuelCost += num2;
      }
      return dryCost + fuelCost;
    }

    public static void FreezePosition(this Vessel vessel)
    {
      if (!Object.op_Inequality((Object) vessel, (Object) null) || vessel.packed || vessel.parts.Count <= 0 || !Object.op_Implicit((Object) vessel.rootPart) || !Object.op_Implicit((Object) vessel.rootPart.Rigidbody) || vessel.rootPart.Rigidbody.constraints != 0)
        return;
      vessel.parts?.ForEach((Action<Part>) (p => p.Rigidbody.constraints = (RigidbodyConstraints) 126));
    }

    public static void UnfreezePosition(this Vessel vessel)
    {
      if (!Object.op_Inequality((Object) vessel, (Object) null) || vessel.packed || vessel.parts.Count <= 0 || !Object.op_Implicit((Object) vessel.rootPart) || !Object.op_Implicit((Object) vessel.rootPart.Rigidbody) || vessel.rootPart.Rigidbody.constraints != 126)
        return;
      vessel.parts?.ForEach((Action<Part>) (p => p.Rigidbody.constraints = (RigidbodyConstraints) 0));
    }

    public static Part FindPart(this Vessel vessel, uint partFlightId)
    {
      if (Object.op_Inequality((Object) vessel, (Object) null) && !vessel.packed && vessel.parts.Count > 0)
      {
        for (int index = 0; index < vessel.parts.Count; ++index)
        {
          if ((int) vessel.parts[index].flightID == (int) partFlightId)
            return vessel.parts[index];
        }
      }
      return (Part) null;
    }

    public static void RemoveAllCrew(this Vessel vessel)
    {
      foreach (Part part in vessel.Parts)
      {
        foreach (ProtoCrewMember protoCrewMember in part.protoModuleCrew)
          vessel.RemoveCrew(protoCrewMember);
      }
      foreach (ProtoCrewMember protoCrewMember in vessel.GetVesselCrew())
        vessel.RemoveCrew(protoCrewMember);
      vessel.DespawnCrew();
    }

    public static bool IsImmortal(this Vessel vessel)
    {
      if (Object.op_Implicit((Object) vessel.rootPart))
        return float.IsPositiveInfinity(vessel.rootPart.crashTolerance);
      return LockSystem.LockQuery.UpdateLockExists(vessel.id) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName);
    }

    public static void SetImmortal(this Vessel vessel, bool immortal)
    {
      if (Object.op_Equality((Object) vessel, (Object) null) || Object.op_Implicit((Object) vessel.rootPart) && float.IsPositiveInfinity(vessel.rootPart.crashTolerance) == immortal)
        return;
      PartBuoyancy component1 = ((Component) vessel).GetComponent<PartBuoyancy>();
      if (Object.op_Implicit((Object) component1))
        ((Behaviour) component1).enabled = !immortal;
      CollisionEnhancer component2 = ((Component) vessel).GetComponent<CollisionEnhancer>();
      if (Object.op_Implicit((Object) component2))
        ((Behaviour) component2).enabled = !immortal;
      FlightIntegrator component3 = ((Component) vessel).GetComponent<FlightIntegrator>();
      if (Object.op_Implicit((Object) component3))
        ((Behaviour) component3).enabled = !immortal;
      if (vessel.loaded)
      {
        LunaLog.Log(string.Format("Making vessel {0} {1}", (object) vessel.id, immortal ? (object) nameof (immortal) : (object) "mortal"));
        foreach (Part part in vessel.Parts)
          part.SetImmortal(immortal);
      }
      if (immortal)
        ImmortalEvent.onVesselImmortal.Fire(vessel);
      else
        ImmortalEvent.onVesselMortal.Fire(vessel);
    }
  }
}
