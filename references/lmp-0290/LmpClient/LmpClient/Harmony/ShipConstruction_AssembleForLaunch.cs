// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ShipConstruction_AssembleForLaunch
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using System;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ShipConstruction))]
  [HarmonyPatch("AssembleForLaunch")]
  [HarmonyPatch(new Type[] {typeof (ShipConstruct), typeof (string), typeof (string), typeof (string), typeof (Game), typeof (VesselCrewManifest), typeof (bool), typeof (bool), typeof (bool), typeof (bool), typeof (Orbit), typeof (bool), typeof (bool)})]
  public class ShipConstruction_AssembleForLaunch
  {
    [HarmonyPrefix]
    private static void PrefixAssembleForLaunch(
      ShipConstruct ship,
      string landedAt,
      string displaylandedAt,
      string flagURL,
      Game sceneState,
      VesselCrewManifest crewManifest,
      bool fromShipAssembly,
      bool setActiveVessel,
      bool isLanded,
      bool preCreate,
      Orbit orbit,
      bool orbiting,
      bool isSplashed)
    {
      if (!fromShipAssembly || ship == null)
        return;
      VesselAssemblyEvent.onAssemblingVessel.Fire(ship);
    }

    [HarmonyPostfix]
    private static void PostfixAssembleForLaunch(
      ShipConstruct ship,
      string landedAt,
      string displaylandedAt,
      string flagURL,
      Game sceneState,
      VesselCrewManifest crewManifest,
      bool fromShipAssembly,
      bool setActiveVessel,
      bool isLanded,
      bool preCreate,
      Orbit orbit,
      bool orbiting,
      bool isSplashed,
      Vessel __result)
    {
      if (!fromShipAssembly || !Object.op_Implicit((Object) __result) || ship == null)
        return;
      VesselAssemblyEvent.onAssembledVessel.Fire(__result, ship);
    }
  }
}
