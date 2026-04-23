// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ScenarioDiscoverableObjects_SpawnComet
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.AsteroidComet;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ScenarioDiscoverableObjects))]
  [HarmonyPatch("SpawnComet")]
  [HarmonyPatch(new Type[] {})]
  public class ScenarioDiscoverableObjects_SpawnComet
  {
    [HarmonyPrefix]
    private static bool PrefixSpawnComet() => MainSystem.NetworkState < ClientState.Connected || LockSystem.LockQuery.AsteroidCometLockBelongsToPlayer(SettingsSystem.CurrentSettings.PlayerName) && LmpClient.Base.System<AsteroidCometSystem>.Singleton.GetCometCount() < SettingsSystem.ServerSettings.MaxNumberOfComets;
  }
}
