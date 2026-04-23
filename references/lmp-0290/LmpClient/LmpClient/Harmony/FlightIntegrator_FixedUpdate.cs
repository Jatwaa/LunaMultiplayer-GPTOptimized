// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightIntegrator_FixedUpdate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Extensions;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightIntegrator))]
  [HarmonyPatch("FixedUpdate")]
  public class FlightIntegrator_FixedUpdate
  {
    [HarmonyPrefix]
    private static bool PrefixFixedUpdate(FlightIntegrator __instance) => MainSystem.NetworkState < ClientState.Connected || Object.op_Equality((Object) ((VesselModule) __instance).Vessel, (Object) FlightGlobals.ActiveVessel) || !((VesselModule) __instance).Vessel.IsImmortal();
  }
}
