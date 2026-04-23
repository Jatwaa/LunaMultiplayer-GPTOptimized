// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SafetyBubble.SafetyBubbleSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.SettingsSys;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.SafetyBubble
{
  public class SafetyBubbleSystem : LmpClient.Base.System<SafetyBubbleSystem>
  {
    public GameObject SafetyBubbleObject;
    public GameObject SafetyBubbleObjectX;
    public GameObject SafetyBubbleObjectY;

    public Dictionary<string, List<SpawnPointLocation>> SpawnPoints { get; } = new Dictionary<string, List<SpawnPointLocation>>();

    public SafetyBubbleEvents SafetyBubbleEvents { get; } = new SafetyBubbleEvents();

    public override string SystemName { get; } = nameof (SafetyBubbleSystem);

    protected override void OnEnabled()
    {
      this.FillUpPositions();
      // ISSUE: method pointer
      GameEvents.onFlightReady.Add(new EventVoid.OnEvent((object) this.SafetyBubbleEvents, __methodptr(FlightReady)));
    }

    protected override void OnDisabled()
    {
      this.SpawnPoints.Clear();
      // ISSUE: method pointer
      GameEvents.onFlightReady.Remove(new EventVoid.OnEvent((object) this.SafetyBubbleEvents, __methodptr(FlightReady)));
    }

    public bool IsInSafetyBubble(Vessel vessel) => !Object.op_Equality((Object) vessel, (Object) null) && vessel.situation <= 8 && (double) SettingsSystem.ServerSettings.SafetyBubbleDistance > 0.0 && this.IsInSafetyBubble(vessel.latitude, vessel.longitude, vessel.altitude, vessel.mainBody);

    public bool IsInSafetyBubble(ProtoVessel protoVessel)
    {
      if (protoVessel == null)
        return true;
      return protoVessel.orbitSnapShot != null && this.IsInSafetyBubble(protoVessel.latitude, protoVessel.longitude, protoVessel.altitude, protoVessel.orbitSnapShot.ReferenceBodyIndex);
    }

    public void DestroySafetyBubble(float waitSeconds)
    {
      if (Object.op_Inequality((Object) this.SafetyBubbleObject, (Object) null))
        Object.Destroy((Object) this.SafetyBubbleObject, waitSeconds);
      if (Object.op_Inequality((Object) this.SafetyBubbleObjectX, (Object) null))
        Object.Destroy((Object) this.SafetyBubbleObjectX, waitSeconds);
      if (!Object.op_Inequality((Object) this.SafetyBubbleObjectY, (Object) null))
        return;
      Object.Destroy((Object) this.SafetyBubbleObjectY, waitSeconds);
    }

    public void DrawSafetyBubble()
    {
      this.DestroySafetyBubble(0.0f);
      SpawnPointLocation safetySpawnPoint = this.GetSafetySpawnPoint(FlightGlobals.ActiveVessel);
      if (safetySpawnPoint == null)
        return;
      this.SafetyBubbleObject = new GameObject();
      this.SafetyBubbleObject.transform.position = Vector3d.op_Implicit(safetySpawnPoint.Position);
      this.SafetyBubbleObject.transform.rotation = Quaternion.LookRotation(Vector3d.op_Implicit(safetySpawnPoint.Body.GetSurfaceNVector(safetySpawnPoint.Latitude, safetySpawnPoint.Longitude)));
      this.SafetyBubbleObjectX = new GameObject();
      this.SafetyBubbleObjectX.transform.position = Vector3d.op_Implicit(safetySpawnPoint.Position);
      this.SafetyBubbleObjectX.transform.rotation = Quaternion.op_Multiply(this.SafetyBubbleObject.transform.rotation, Quaternion.Euler(0.0f, 90f, 0.0f));
      this.SafetyBubbleObjectY = new GameObject();
      this.SafetyBubbleObjectY.transform.position = Vector3d.op_Implicit(safetySpawnPoint.Position);
      this.SafetyBubbleObjectY.transform.rotation = Quaternion.op_Multiply(this.SafetyBubbleObject.transform.rotation, Quaternion.Euler(90f, 90f, 0.0f));
      SafetyBubbleSystem.DrawCircleAround(safetySpawnPoint.Position, SafetyBubbleSystem.CreateLineRenderer(this.SafetyBubbleObject));
      SafetyBubbleSystem.DrawCircleAround(safetySpawnPoint.Position, SafetyBubbleSystem.CreateLineRenderer(this.SafetyBubbleObjectX));
      SafetyBubbleSystem.DrawCircleAround(safetySpawnPoint.Position, SafetyBubbleSystem.CreateLineRenderer(this.SafetyBubbleObjectY));
      this.DestroySafetyBubble(10f);
    }

    private static void DrawCircleAround(Vector3d center, LineRenderer lineRenderer)
    {
      float num1 = 0.0f;
      for (int index = 0; index < lineRenderer.positionCount; ++index)
      {
        num1 += 0.06283186f;
        float num2 = SettingsSystem.ServerSettings.SafetyBubbleDistance * Mathf.Cos(num1);
        float num3 = SettingsSystem.ServerSettings.SafetyBubbleDistance * Mathf.Sin(num1);
        float num4 = num2 + (float) center.x;
        float num5 = num3 + (float) center.y;
        lineRenderer.SetPosition(index, new Vector3(num4, num5, 0.0f));
      }
    }

    private static LineRenderer CreateLineRenderer(GameObject gameObj)
    {
      LineRenderer lineRenderer = gameObj.AddComponent<LineRenderer>();
      ((Renderer) lineRenderer).material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
      lineRenderer.startWidth = 0.3f;
      lineRenderer.endWidth = 0.3f;
      lineRenderer.startColor = Color.red;
      lineRenderer.endColor = Color.red;
      lineRenderer.positionCount = 629;
      lineRenderer.useWorldSpace = false;
      return lineRenderer;
    }

    private SpawnPointLocation GetSafetySpawnPoint(Vessel vessel)
    {
      foreach (SpawnPointLocation safetySpawnPoint in this.SpawnPoints[vessel.mainBody.name])
      {
        if (Vector3d.Distance(Vector3d.op_Implicit(vessel.vesselTransform.position), safetySpawnPoint.Position) < (double) SettingsSystem.ServerSettings.SafetyBubbleDistance)
          return safetySpawnPoint;
      }
      return (SpawnPointLocation) null;
    }

    private void FillUpPositions()
    {
      foreach (PSystemSetup.SpaceCenterFacility facilityLaunchSite in PSystemSetup.Instance.SpaceCenterFacilityLaunchSites)
      {
        if (!this.SpawnPoints.ContainsKey(facilityLaunchSite.hostBody.name))
          this.SpawnPoints.Add(facilityLaunchSite.hostBody.name, new List<SpawnPointLocation>());
        foreach (PSystemSetup.SpaceCenterFacility.SpawnPoint spawnPoint in facilityLaunchSite.spawnPoints)
          this.SpawnPoints[facilityLaunchSite.hostBody.name].Add(new SpawnPointLocation(spawnPoint, facilityLaunchSite.hostBody));
      }
      foreach (LaunchSite stockLaunchSite in PSystemSetup.Instance.StockLaunchSites)
      {
        if (!this.SpawnPoints.ContainsKey(stockLaunchSite.Body.name))
          this.SpawnPoints.Add(stockLaunchSite.Body.name, new List<SpawnPointLocation>());
        foreach (LaunchSite.SpawnPoint spawnPoint in stockLaunchSite.spawnPoints)
          this.SpawnPoints[stockLaunchSite.Body.name].Add(new SpawnPointLocation(spawnPoint, stockLaunchSite.Body));
      }
    }

    private bool IsInSafetyBubble(double lat, double lon, double alt, int bodyIndex)
    {
      if (bodyIndex < FlightGlobals.Bodies.Count)
      {
        CelestialBody body = FlightGlobals.Bodies[bodyIndex];
        return !Object.op_Equality((Object) body, (Object) null) && this.IsInSafetyBubble(FlightGlobals.Bodies[bodyIndex].GetWorldSurfacePosition(lat, lon, alt), body);
      }
      LunaLog.LogError(string.Format("Body index {0} is out of range!", (object) bodyIndex));
      return false;
    }

    private bool IsInSafetyBubble(double lat, double lon, double alt, CelestialBody body) => !Object.op_Equality((Object) body, (Object) null) && this.IsInSafetyBubble(body.GetWorldSurfacePosition(lat, lon, alt), body);

    private bool IsInSafetyBubble(Vector3d position, CelestialBody body)
    {
      if (!this.SpawnPoints.ContainsKey(body.name))
        return false;
      foreach (SpawnPointLocation spawnPointLocation in this.SpawnPoints[body.name])
      {
        if (Vector3d.Distance(position, spawnPointLocation.Position) < (double) SettingsSystem.ServerSettings.SafetyBubbleDistance)
          return true;
      }
      return false;
    }
  }
}
