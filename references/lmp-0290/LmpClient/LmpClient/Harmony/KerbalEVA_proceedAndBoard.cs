// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.KerbalEVA_proceedAndBoard
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using System;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (KerbalEVA))]
  [HarmonyPatch("proceedAndBoard")]
  public class KerbalEVA_proceedAndBoard
  {
    private static Guid kerbalVesselId = Guid.Empty;
    private static string kerbalName;

    [HarmonyPrefix]
    private static void PrefixProceedAndBoard(KerbalEVA __instance)
    {
      KerbalEVA_proceedAndBoard.kerbalVesselId = ((PartModule) __instance).vessel.id;
      KerbalEVA_proceedAndBoard.kerbalName = ((Object) ((PartModule) __instance).vessel).name;
    }

    [HarmonyPostfix]
    private static void PostfixProceedAndBoard(Part p) => EvaEvent.onCrewEvaBoarded.Fire(KerbalEVA_proceedAndBoard.kerbalVesselId, KerbalEVA_proceedAndBoard.kerbalName, p.vessel);
  }
}
