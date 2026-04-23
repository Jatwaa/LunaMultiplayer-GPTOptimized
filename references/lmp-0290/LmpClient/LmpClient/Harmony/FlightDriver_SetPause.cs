// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightDriver_SetPause
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightDriver))]
  [HarmonyPatch("SetPause")]
  public class FlightDriver_SetPause
  {
    [HarmonyPrefix]
    private static bool PrefixSetPause(bool pauseState, bool postScreenMessage) => MainSystem.NetworkState < ClientState.Connected || !pauseState;
  }
}
