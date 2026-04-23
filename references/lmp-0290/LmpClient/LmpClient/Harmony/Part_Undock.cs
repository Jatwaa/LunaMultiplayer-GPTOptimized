// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Part_Undock
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Part))]
  [HarmonyPatch("Undock")]
  public class Part_Undock
  {
    [HarmonyPrefix]
    private static void PrefixUndock(
      Part __instance,
      DockedVesselInfo newVesselInfo,
      ref Vessel __state)
    {
      __state = __instance.vessel;
      PartEvent.onPartUndocking.Fire(__instance, newVesselInfo);
    }

    [HarmonyPostfix]
    private static void PostfixUndock(
      Part __instance,
      DockedVesselInfo newVesselInfo,
      ref Vessel __state)
    {
      PartEvent.onPartUndocked.Fire(__instance, newVesselInfo, __state);
    }
  }
}
