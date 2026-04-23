// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.VesselFilter
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.Lock;
using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels
{
  public class VesselFilter
  {
    public static bool HideAsteroids = true;
    public static bool HideDebris = true;
    public static bool HideUncontrolled = false;

    public static void DrawFilters()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      VesselFilter.HideAsteroids = GUILayout.Toggle(VesselFilter.HideAsteroids, "Hide comets/asteroids", Array.Empty<GUILayoutOption>());
      VesselFilter.HideDebris = GUILayout.Toggle(VesselFilter.HideDebris, "Hide debris", Array.Empty<GUILayoutOption>());
      VesselFilter.HideUncontrolled = GUILayout.Toggle(VesselFilter.HideUncontrolled, "Hide uncontrolled", Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }

    public static bool MatchesFilters(Vessel vessel) => (!VesselFilter.HideAsteroids || !vessel.IsCometOrAsteroid()) && (!VesselFilter.HideDebris || vessel.vesselType != 0) && (!VesselFilter.HideUncontrolled || LockSystem.LockQuery.ControlLockExists(vessel.id));
  }
}
