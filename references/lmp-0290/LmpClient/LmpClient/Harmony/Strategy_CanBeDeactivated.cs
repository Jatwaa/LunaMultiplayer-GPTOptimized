// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Strategy_CanBeDeactivated
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Systems.ShareStrategy;
using LmpCommon.Enums;
using Strategies;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Strategy))]
  [HarmonyPatch("CanBeDeactivated")]
  public class Strategy_CanBeDeactivated
  {
    [HarmonyPrefix]
    private static bool PrefixCanBeDeactivated() => MainSystem.NetworkState < ClientState.Connected || !System<ShareStrategySystem>.Singleton.Enabled || !System<ShareStrategySystem>.Singleton.IgnoreEvents;
  }
}
