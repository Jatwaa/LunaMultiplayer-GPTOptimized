// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Vessel_GoOnRails
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Vessel))]
  [HarmonyPatch("GoOnRails")]
  public class Vessel_GoOnRails
  {
    [HarmonyPostfix]
    private static void PostfixGoOnRails(Vessel __instance) => RailEvent.onVesselGoneOnRails.Fire(__instance);
  }
}
