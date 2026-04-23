// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Strategy_Deactivate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using Strategies;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Strategy))]
  [HarmonyPatch("Deactivate")]
  public class Strategy_Deactivate
  {
    [HarmonyPostfix]
    private static void PostfixDeactivate(Strategy __instance, ref bool __result)
    {
      if (!__result)
        return;
      StrategyEvent.onStrategyDeactivated.Fire(__instance);
    }
  }
}
