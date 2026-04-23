// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Vessel_Unload
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Vessel))]
  [HarmonyPatch("Unload")]
  public class Vessel_Unload
  {
    [HarmonyPrefix]
    private static void PrefixUnload(Vessel __instance) => VesselUnloadEvent.onVesselUnloading.Fire(__instance);

    [HarmonyPostfix]
    private static void PostfixUnload(Vessel __instance) => VesselUnloadEvent.onVesselUnloaded.Fire(__instance);
  }
}
