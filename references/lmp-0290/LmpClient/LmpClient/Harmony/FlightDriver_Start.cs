// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightDriver_Start
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightDriver))]
  [HarmonyPatch("Start")]
  public class FlightDriver_Start
  {
    [HarmonyPostfix]
    private static void PostfixStart() => FlightDriverEvent.onFlightStarted.Fire();
  }
}
