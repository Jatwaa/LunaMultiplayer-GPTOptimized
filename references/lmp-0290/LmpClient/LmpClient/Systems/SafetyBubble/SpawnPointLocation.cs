// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SafetyBubble.SpawnPointLocation
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using UnityEngine;

namespace LmpClient.Systems.SafetyBubble
{
  public class SpawnPointLocation
  {
    public readonly double Latitude;
    public readonly double Longitude;
    public readonly double Altitude;
    public Transform Transform;
    public readonly CelestialBody Body;

    public Vector3d Position => this.Body.GetWorldSurfacePosition(this.Latitude, this.Longitude, this.Altitude);

    public SpawnPointLocation(
      PSystemSetup.SpaceCenterFacility.SpawnPoint spawnPoint,
      CelestialBody celestialBody)
    {
      this.Transform = spawnPoint.GetSpawnPointTransform();
      this.Latitude = spawnPoint.latitude;
      this.Longitude = spawnPoint.longitude;
      this.Altitude = spawnPoint.altitude;
      this.Body = celestialBody;
    }

    public SpawnPointLocation(LaunchSite.SpawnPoint spawnPoint, CelestialBody celestialBody)
    {
      this.Transform = spawnPoint.GetSpawnPointTransform();
      this.Latitude = spawnPoint.latitude;
      this.Longitude = spawnPoint.longitude;
      this.Altitude = spawnPoint.altitude;
      this.Body = celestialBody;
    }
  }
}
