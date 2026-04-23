// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Part_Couple
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.VesselUtilities;
using System;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Part))]
  [HarmonyPatch("Couple")]
  public class Part_Couple
  {
    [HarmonyPrefix]
    private static bool PrefixCouple(Part __instance, Part tgtPart, ref Guid __state)
    {
      if (VesselCommon.IsSpectating)
        return false;
      __state = __instance.vessel.id;
      PartEvent.onPartCoupling.Fire(__instance, tgtPart);
      return true;
    }

    [HarmonyPostfix]
    private static void PostfixCouple(Part __instance, Part tgtPart, ref Guid __state)
    {
      if (VesselCommon.IsSpectating)
        return;
      PartEvent.onPartCoupled.Fire(__instance, tgtPart, __state);
    }
  }
}
