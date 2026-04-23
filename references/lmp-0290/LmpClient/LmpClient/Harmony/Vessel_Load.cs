// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Vessel_Load
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Systems.SafetyBubble;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Vessel))]
  [HarmonyPatch("Load")]
  public class Vessel_Load
  {
    [HarmonyPrefix]
    private static bool PrefixLoad(Vessel __instance) => !Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || !FlightGlobals.ActiveVessel.loaded || !(FlightGlobals.ActiveVessel.id != __instance.id) || !System<SafetyBubbleSystem>.Singleton.IsInSafetyBubble(__instance);
  }
}
