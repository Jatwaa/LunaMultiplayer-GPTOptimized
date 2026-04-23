// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Strategy_CanBeActivated
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
  [HarmonyPatch("CanBeActivated")]
  public class Strategy_CanBeActivated
  {
    [HarmonyPrefix]
    private static bool PrefixCanBeActivated() => MainSystem.NetworkState < ClientState.Connected || !System<ShareStrategySystem>.Singleton.Enabled || !System<ShareStrategySystem>.Singleton.IgnoreEvents;
  }
}
