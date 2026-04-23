// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ScenarioDiscoverableObjects_UpdateSpaceObjects
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ScenarioDiscoverableObjects))]
  [HarmonyPatch("UpdateSpaceObjects")]
  public class ScenarioDiscoverableObjects_UpdateSpaceObjects
  {
    [HarmonyPrefix]
    private static bool PrefixUpdateSpaceObjects() => MainSystem.NetworkState < ClientState.Connected || LockSystem.LockQuery.AsteroidCometLockBelongsToPlayer(SettingsSystem.CurrentSettings.PlayerName);
  }
}
