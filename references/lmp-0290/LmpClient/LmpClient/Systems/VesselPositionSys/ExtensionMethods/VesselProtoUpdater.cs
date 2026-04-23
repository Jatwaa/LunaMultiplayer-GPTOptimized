// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.ExtensionMethods.VesselProtoUpdater
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using UnityEngine;

namespace LmpClient.Systems.VesselPositionSys.ExtensionMethods
{
  public static class VesselProtoUpdater
  {
    public static void UpdatePositionValues(this ProtoVessel protoVessel, Vessel vessel)
    {
      if (protoVessel == null)
        return;
      protoVessel.latitude = vessel.latitude;
      protoVessel.longitude = vessel.longitude;
      protoVessel.altitude = vessel.altitude;
      protoVessel.height = vessel.heightFromTerrain;
      ((Vector3) ref protoVessel.normal)[0] = vessel.terrainNormal.x;
      ((Vector3) ref protoVessel.normal)[1] = vessel.terrainNormal.y;
      ((Vector3) ref protoVessel.normal)[2] = vessel.terrainNormal.z;
      ((Quaternion) ref protoVessel.rotation)[0] = vessel.srfRelRotation.x;
      ((Quaternion) ref protoVessel.rotation)[1] = vessel.srfRelRotation.y;
      ((Quaternion) ref protoVessel.rotation)[2] = vessel.srfRelRotation.z;
      ((Quaternion) ref protoVessel.rotation)[3] = vessel.srfRelRotation.w;
      protoVessel.orbitSnapShot.inclination = vessel.orbit.inclination;
      protoVessel.orbitSnapShot.eccentricity = vessel.orbit.eccentricity;
      protoVessel.orbitSnapShot.semiMajorAxis = vessel.orbit.semiMajorAxis;
      protoVessel.orbitSnapShot.LAN = vessel.orbit.LAN;
      protoVessel.orbitSnapShot.argOfPeriapsis = vessel.orbit.argumentOfPeriapsis;
      protoVessel.orbitSnapShot.meanAnomalyAtEpoch = vessel.orbit.meanAnomalyAtEpoch;
      protoVessel.orbitSnapShot.epoch = vessel.orbit.epoch;
      protoVessel.orbitSnapShot.ReferenceBodyIndex = vessel.orbit.referenceBody.flightGlobalsIndex;
    }

    public static void UpdatePositionValues(
      this ProtoVessel protoVessel,
      VesselPositionUpdate update)
    {
      if (protoVessel == null)
        return;
      protoVessel.latitude = update.LatLonAlt[0];
      protoVessel.longitude = update.LatLonAlt[1];
      protoVessel.altitude = update.LatLonAlt[2];
      protoVessel.height = update.HeightFromTerrain;
      ref Vector3 local1 = ref protoVessel.normal;
      Vector3 normal1 = update.Normal;
      double num1 = (double) ((Vector3) ref normal1)[0];
      ((Vector3) ref local1)[0] = (float) num1;
      ref Vector3 local2 = ref protoVessel.normal;
      Vector3 normal2 = update.Normal;
      double num2 = (double) ((Vector3) ref normal2)[1];
      ((Vector3) ref local2)[1] = (float) num2;
      ref Vector3 local3 = ref protoVessel.normal;
      normal2 = update.Normal;
      double num3 = (double) ((Vector3) ref normal2)[2];
      ((Vector3) ref local3)[2] = (float) num3;
      ((Quaternion) ref protoVessel.rotation)[0] = update.SrfRelRotation[0];
      ((Quaternion) ref protoVessel.rotation)[1] = update.SrfRelRotation[1];
      ((Quaternion) ref protoVessel.rotation)[2] = update.SrfRelRotation[2];
      ((Quaternion) ref protoVessel.rotation)[3] = update.SrfRelRotation[3];
      protoVessel.orbitSnapShot.inclination = update.Orbit[0];
      protoVessel.orbitSnapShot.eccentricity = update.Orbit[1];
      protoVessel.orbitSnapShot.semiMajorAxis = update.Orbit[2];
      protoVessel.orbitSnapShot.LAN = update.Orbit[3];
      protoVessel.orbitSnapShot.argOfPeriapsis = update.Orbit[4];
      protoVessel.orbitSnapShot.meanAnomalyAtEpoch = update.Orbit[5];
      protoVessel.orbitSnapShot.epoch = update.Orbit[6];
      protoVessel.orbitSnapShot.ReferenceBodyIndex = (int) update.Orbit[7];
    }
  }
}
