// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.OrbitDriver_UpdateOrbit
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.Systems.VesselPositionSys;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (OrbitDriver))]
  [HarmonyPatch("UpdateOrbit")]
  public class OrbitDriver_UpdateOrbit
  {
    [HarmonyPrefix]
    private static bool PrefixUpdateOrbit(
      OrbitDriver __instance,
      bool offset,
      ref bool ___ready,
      ref double ___fdtLast,
      ref bool ___isHyperbolic)
    {
      if (MainSystem.NetworkState < ClientState.Connected || Object.op_Equality((Object) __instance.vessel, (Object) null) || Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && Object.op_Equality((Object) __instance.vessel, (Object) FlightGlobals.ActiveVessel) && __instance.vessel.packed || !__instance.vessel.IsImmortal() && __instance.vessel.packed)
        return true;
      OrbitDriver_UpdateOrbit.UpdateOrbit(__instance, offset, ref ___ready, ref ___fdtLast, ref ___isHyperbolic);
      return false;
    }

    private static void UpdateOrbit(
      OrbitDriver driver,
      bool offset,
      ref bool ready,
      ref double fdtLast,
      ref bool isHyperbolic)
    {
      if (!ready)
        return;
      driver.lastMode = driver.updateMode;
      if (System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(driver.vessel.id) && driver.updateMode == null || driver.updateMode == 1)
      {
        driver.updateFromParameters();
        if (Object.op_Implicit((Object) driver.vessel))
          driver.CheckDominantBody(Vector3d.op_Addition(driver.referenceBody.position, driver.pos));
      }
      if (Object.op_Implicit((Object) driver.vessel) && Object.op_Implicit((Object) driver.vessel.rootPart) && Object.op_Implicit((Object) driver.vessel.rootPart.rb))
      {
        if (!offset)
          fdtLast = 0.0;
        if (!driver.CheckDominantBody(driver.vessel.CoMD))
          driver.TrackRigidbody(driver.referenceBody, -fdtLast);
      }
      fdtLast = (double) TimeWarp.fixedDeltaTime;
      if (isHyperbolic && driver.orbit.eccentricity < 1.0)
      {
        isHyperbolic = false;
        if (Object.op_Inequality((Object) driver.vessel, (Object) null))
          GameEvents.onVesselOrbitClosed.Fire(driver.vessel);
      }
      if (!isHyperbolic && driver.orbit.eccentricity > 1.0)
      {
        isHyperbolic = true;
        if (Object.op_Inequality((Object) driver.vessel, (Object) null))
          GameEvents.onVesselOrbitEscaped.Fire(driver.vessel);
      }
      if (!driver.drawOrbit)
        return;
      driver.orbit.DrawOrbit();
    }
  }
}
