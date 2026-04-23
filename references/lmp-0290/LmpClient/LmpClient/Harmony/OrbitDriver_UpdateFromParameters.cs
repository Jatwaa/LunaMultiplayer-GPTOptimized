// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.OrbitDriver_UpdateFromParameters
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (OrbitDriver))]
  [HarmonyPatch("updateFromParameters")]
  [HarmonyPatch(new Type[] {typeof (bool)})]
  public class OrbitDriver_UpdateFromParameters
  {
    [HarmonyPrefix]
    private static bool PrefixUpdateFromParameters(OrbitDriver __instance, ref double ___updateUT)
    {
      if (MainSystem.NetworkState < ClientState.Connected || Object.op_Equality((Object) __instance.vessel, (Object) null))
        return true;
      OrbitDriver_UpdateFromParameters.UpdateFromParameters(__instance, ref ___updateUT);
      return false;
    }

    private static void UpdateFromParameters(OrbitDriver driver, ref double updateUT)
    {
      updateUT = Planetarium.GetUniversalTime();
      driver.orbit.UpdateFromUT(updateUT);
      driver.pos = driver.orbit.pos;
      driver.vel = driver.orbit.vel;
      ((Vector3d) ref driver.pos).Swizzle();
      ((Vector3d) ref driver.vel).Swizzle();
      if (double.IsNaN(driver.pos.x))
      {
        MonoBehaviour.print((object) ("ObT : " + (object) driver.orbit.ObT + "\nM : " + (object) driver.orbit.meanAnomaly + "\nE : " + (object) driver.orbit.eccentricAnomaly + "\nV : " + (object) driver.orbit.trueAnomaly + "\nRadius: " + (object) driver.orbit.radius + "\nvel: " + driver.vel.ToString() + "\nAN: " + driver.orbit.an.ToString() + "\nperiod: " + (object) driver.orbit.period + "\n"));
        if (Object.op_Implicit((Object) driver.vessel))
        {
          Debug.LogWarning((object) ("[LMP - OrbitDriver Warning!]: " + driver.vessel.vesselName + " had a NaN Orbit and was removed."));
          driver.vessel.Unload();
          LmpClient.Base.System<VesselRemoveSystem>.Singleton.MessageSender.SendVesselRemove(driver.vessel.id);
          LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(driver.vessel.id, true, "Corrupt vessel orbit");
          return;
        }
      }
      if (driver.reverse)
        driver.referenceBody.position = Vector3d.op_Subtraction(!Object.op_Implicit((Object) driver.celestialBody) ? Vector3d.op_Implicit(driver.driverTransform.position) : driver.celestialBody.position, driver.pos);
      else if (Object.op_Implicit((Object) driver.vessel))
      {
        if (LmpClient.Base.System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(driver.vessel.id))
          return;
        Vector3d vector3d = Vector3d.op_Implicit(Quaternion.op_Multiply(driver.driverTransform.rotation, driver.vessel.localCoM));
        driver.vessel.SetPosition(Vector3d.op_Subtraction(Vector3d.op_Addition(driver.referenceBody.position, driver.pos), vector3d));
      }
      else if (!Object.op_Implicit((Object) driver.celestialBody))
        driver.driverTransform.position = Vector3d.op_Implicit(Vector3d.op_Addition(driver.referenceBody.position, driver.pos));
      else
        driver.celestialBody.position = Vector3d.op_Addition(driver.referenceBody.position, driver.pos);
    }
  }
}
