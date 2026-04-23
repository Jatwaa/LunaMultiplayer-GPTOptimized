// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.TimeWarp_SetRate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.Warp;
using LmpCommon.Enums;
using System;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (TimeWarp))]
  [HarmonyPatch("SetRate")]
  [HarmonyPatch(new Type[] {typeof (int), typeof (bool), typeof (bool)})]
  public class TimeWarp_SetRate
  {
    [HarmonyPrefix]
    private static bool PrefixSetRate(
      ref int rate_index,
      ref bool instant,
      ref bool postScreenMessage)
    {
      return MainSystem.NetworkState < ClientState.Connected || LmpClient.Base.System<WarpSystem>.Singleton.WarpValidation();
    }
  }
}
