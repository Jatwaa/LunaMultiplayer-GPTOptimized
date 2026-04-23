// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.ExtensionMethods.VesselPositioner
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.TimeSync;
using LmpCommon;
using UnityEngine;

namespace LmpClient.Systems.VesselPositionSys.ExtensionMethods
{
  public static class VesselPositioner
  {
    public static void SetVesselPosition(
      this Vessel vessel,
      VesselPositionUpdate update,
      VesselPositionUpdate target,
      float percentage)
    {
      if (Object.op_Equality((Object) vessel, (Object) null) || update == null || target == null)
        return;
      CelestialBody lerpedBody = (double) percentage < 0.5 ? update.Body : target.Body;
      VesselPositioner.ApplyOrbitInterpolation(vessel, update, target, lerpedBody, percentage);
      vessel.staticPressurekPa = FlightGlobals.getStaticPressure(target.LatLonAlt[2], lerpedBody);
      vessel.heightFromTerrain = target.HeightFromTerrain;
      VesselPositioner.ApplyInterpolationsToVessel(vessel, update, target, lerpedBody, percentage);
      vessel.protoVessel.UpdatePositionValues(vessel);
    }

    private static void ApplyOrbitInterpolation(
      Vessel vessel,
      VesselPositionUpdate update,
      VesselPositionUpdate target,
      CelestialBody lerpedBody,
      float percentage)
    {
      Vector3d relativePositionAtUt1 = update.KspOrbit.getRelativePositionAtUT(TimeSyncSystem.UniversalTime);
      Vector3d relativePositionAtUt2 = target.KspOrbit.getRelativePositionAtUT(TimeSyncSystem.UniversalTime);
      Vector3d orbitalVelocityAtUt1 = update.KspOrbit.getOrbitalVelocityAtUT(TimeSyncSystem.UniversalTime);
      Vector3d orbitalVelocityAtUt2 = target.KspOrbit.getOrbitalVelocityAtUT(TimeSyncSystem.UniversalTime);
      Vector3d vector3d1 = Vector3d.Lerp(relativePositionAtUt1, relativePositionAtUt2, (double) percentage);
      Vector3d vector3d2 = Vector3d.Lerp(orbitalVelocityAtUt1, orbitalVelocityAtUt2, (double) percentage);
      vessel.orbit.UpdateFromStateVectors(vector3d1, vector3d2, lerpedBody, TimeSyncSystem.UniversalTime);
    }

    private static void ApplyInterpolationsToVessel(
      Vessel vessel,
      VesselPositionUpdate update,
      VesselPositionUpdate target,
      CelestialBody lerpedBody,
      float percentage)
    {
      Quaternion quaternion = Quaternion.Slerp(update.SurfaceRelRotation, target.SurfaceRelRotation, percentage);
      vessel.srfRelRotation = quaternion;
      vessel.Landed = (double) percentage < 0.5 ? update.Landed : target.Landed;
      vessel.Splashed = (double) percentage < 0.5 ? update.Splashed : target.Splashed;
      vessel.latitude = LunaMath.Lerp(update.LatLonAlt[0], target.LatLonAlt[0], percentage);
      vessel.longitude = LunaMath.Lerp(update.LatLonAlt[1], target.LatLonAlt[1], percentage);
      vessel.altitude = LunaMath.Lerp(update.LatLonAlt[2], target.LatLonAlt[2], percentage);
      Quaternion rotation = Quaternion.op_Multiply(QuaternionD.op_Implicit(lerpedBody.rotation), quaternion);
      Vector3d position = vessel.situation <= 8 ? lerpedBody.GetWorldSurfacePosition(vessel.latitude, vessel.longitude, vessel.altitude) : vessel.orbit.getPositionAtUT(TimeSyncSystem.UniversalTime);
      VesselPositioner.SetVesselPositionAndRotation(vessel, position, rotation);
    }

    private static void SetVesselPositionAndRotation(
      Vessel vessel,
      Vector3d position,
      Quaternion rotation)
    {
      if (!vessel.loaded)
      {
        vessel.vesselTransform.position = Vector3d.op_Implicit(position);
        vessel.vesselTransform.rotation = rotation;
      }
      else
      {
        for (int index = 0; index < vessel.parts.Count; ++index)
        {
          vessel.parts[index].partTransform.rotation = Quaternion.op_Multiply(rotation, vessel.parts[index].orgRot);
          if (vessel.packed || vessel.parts[index].physicalSignificance == 0)
            vessel.parts[index].partTransform.position = Vector3d.op_Implicit(Vector3d.op_Addition(position, Quaternion.op_Multiply(vessel.vesselTransform.rotation, vessel.parts[index].orgPos)));
          vessel.parts[index].ResumeVelocity();
        }
      }
    }
  }
}
