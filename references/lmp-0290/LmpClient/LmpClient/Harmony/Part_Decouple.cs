// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Part_Decouple
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.Systems.VesselDecoupleSys;
using LmpClient.Systems.VesselUndockSys;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Part))]
  [HarmonyPatch("decouple")]
  public class Part_Decouple
  {
    [HarmonyPrefix]
    private static bool PrefixDecouple(Part __instance, float breakForce, ref Vessel __state)
    {
      if (MainSystem.NetworkState < ClientState.Connected || !Object.op_Implicit((Object) __instance.vessel))
        return true;
      if (!(System<VesselDecoupleSystem>.Singleton.ManuallyDecouplingVesselId == __instance.vessel.id) && !(System<VesselUndockSystem>.Singleton.ManuallyUndockingVesselId == __instance.vessel.id) && __instance.vessel.IsImmortal())
        return false;
      __state = __instance.vessel;
      PartEvent.onPartDecoupling.Fire(__instance, breakForce);
      return true;
    }

    [HarmonyPostfix]
    private static void PostfixDecouple(Part __instance, float breakForce, ref Vessel __state) => PartEvent.onPartDecoupled.Fire(__instance, breakForce, __state);
  }
}
