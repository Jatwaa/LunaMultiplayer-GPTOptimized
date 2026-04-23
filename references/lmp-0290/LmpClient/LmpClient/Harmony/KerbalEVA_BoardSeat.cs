// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Harmony.KerbalEVA_BoardSeat
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using System;
using UnityEngine;

namespace LmpClient.ModuleStore.Harmony
{
  [HarmonyPatch(typeof (KerbalEVA))]
  [HarmonyPatch("BoardSeat")]
  public class KerbalEVA_BoardSeat
  {
    private static Guid KerbalVesselId;
    private static string KerbalName;

    [HarmonyPrefix]
    private static void PrefixBoardSeat(KerbalEVA __instance, KerbalSeat seat)
    {
      if (!Object.op_Inequality((Object) ((PartModule) __instance).vessel, (Object) null))
        return;
      KerbalEVA_BoardSeat.KerbalVesselId = ((PartModule) __instance).vessel.id;
      KerbalEVA_BoardSeat.KerbalName = ((PartModule) __instance).vessel.vesselName;
    }

    [HarmonyPostfix]
    private static void PostfixBoardSeat(KerbalEVA __instance, bool __result, KerbalSeat seat)
    {
      if (!__result)
        return;
      ExternalSeatEvent.onExternalSeatBoard.Fire(((PartModule) seat).vessel, KerbalEVA_BoardSeat.KerbalVesselId, KerbalEVA_BoardSeat.KerbalName);
    }
  }
}
