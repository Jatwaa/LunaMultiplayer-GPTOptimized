// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightDriver_RevertToLaunch
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightDriver))]
  [HarmonyPatch("RevertToLaunch")]
  public class FlightDriver_RevertToLaunch
  {
    [HarmonyPrefix]
    private static void PrefixRevertToLaunch() => RevertEvent.onRevertingToLaunch.Fire();

    [HarmonyPostfix]
    private static void PostfixRevertToLaunch() => RevertEvent.onRevertedToLaunch.Fire();
  }
}
