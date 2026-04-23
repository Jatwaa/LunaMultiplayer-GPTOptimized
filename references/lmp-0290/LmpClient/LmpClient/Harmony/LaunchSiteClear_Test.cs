// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.LaunchSiteClear_Test
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpCommon.Enums;
using PreFlightTests;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (LaunchSiteClear))]
  [HarmonyPatch("Test")]
  public class LaunchSiteClear_Test
  {
    [HarmonyPostfix]
    private static void PostfixTest(ref bool __result)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      __result = true;
    }
  }
}
