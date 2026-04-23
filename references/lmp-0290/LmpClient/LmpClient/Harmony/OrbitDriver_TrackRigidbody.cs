// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.OrbitDriver_TrackRigidbody
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Systems.VesselPositionSys;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (OrbitDriver))]
  [HarmonyPatch("TrackRigidbody")]
  public class OrbitDriver_TrackRigidbody
  {
    [HarmonyPrefix]
    private static bool PrefixTrackRigidbody(
      OrbitDriver __instance,
      CelestialBody refBody,
      double fdtOffset,
      ref double ___updateUT)
    {
      if (MainSystem.NetworkState < ClientState.Connected || Object.op_Equality((Object) __instance.vessel, (Object) null))
        return true;
      OrbitDriver_TrackRigidbody.TrackRigidbody(__instance, refBody, fdtOffset, ref ___updateUT);
      return false;
    }

    private static void TrackRigidbody(
      OrbitDriver driver,
      CelestialBody refBody,
      double fdtOffset,
      ref double updateUT)
    {
      updateUT = Planetarium.GetUniversalTime();
      Vector3d vector3d;
      if (Object.op_Inequality((Object) driver.vessel, (Object) null))
      {
        OrbitDriver orbitDriver = driver;
        vector3d = Vector3d.op_Subtraction(driver.vessel.CoMD, driver.referenceBody.position);
        Vector3d xzy = ((Vector3d) ref vector3d).xzy;
        orbitDriver.pos = xzy;
      }
      if (Object.op_Inequality((Object) driver.vessel, (Object) null) && Object.op_Inequality((Object) driver.vessel.rootPart, (Object) null) && Object.op_Inequality((Object) driver.vessel.rootPart.rb, (Object) null))
      {
        updateUT += fdtOffset;
        driver.vel = Vector3d.op_Addition(((Vector3d) ref driver.vessel.velocityD).xzy, driver.orbit.GetRotFrameVelAtPos(driver.referenceBody, driver.pos));
      }
      else if (driver.updateMode == 2)
        driver.vel = driver.orbit.GetRotFrameVel(driver.referenceBody);
      if (Object.op_Inequality((Object) refBody, (Object) driver.referenceBody))
      {
        if (Object.op_Inequality((Object) driver.vessel, (Object) null))
        {
          OrbitDriver orbitDriver = driver;
          vector3d = Vector3d.op_Subtraction(driver.vessel.CoMD, refBody.position);
          Vector3d xzy = ((Vector3d) ref vector3d).xzy;
          orbitDriver.pos = xzy;
        }
        driver.vel = Vector3d.op_Addition(driver.vel, Vector3d.op_Subtraction(driver.referenceBody.GetFrameVel(), refBody.GetFrameVel()));
      }
      driver.lastTrackUT = updateUT;
      if (System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(driver.vessel.id))
        return;
      driver.orbit.UpdateFromStateVectors(driver.pos, driver.vel, refBody, updateUT);
      ((Vector3d) ref driver.pos).Swizzle();
      ((Vector3d) ref driver.vel).Swizzle();
    }
  }
}
