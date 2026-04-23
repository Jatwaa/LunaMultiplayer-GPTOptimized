// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Harmony.KerbalEVA_OnDeboardSeat
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.ModuleStore.Harmony
{
  [HarmonyPatch(typeof (KerbalEVA))]
  [HarmonyPatch("OnDeboardSeat")]
  public class KerbalEVA_OnDeboardSeat
  {
    private static Vessel DeboardedVessel;

    [HarmonyPrefix]
    private static void PrefixOnDeboardSeat(KerbalEVA __instance) => KerbalEVA_OnDeboardSeat.DeboardedVessel = ((PartModule) __instance).vessel;

    [HarmonyPostfix]
    private static void PostfixOnDeboardSeat(KerbalEVA __instance) => ExternalSeatEvent.onExternalSeatUnboard.Fire(KerbalEVA_OnDeboardSeat.DeboardedVessel, __instance);
  }
}
