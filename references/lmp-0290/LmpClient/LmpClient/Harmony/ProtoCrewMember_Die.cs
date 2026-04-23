// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ProtoCrewMember_Die
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ProtoCrewMember))]
  [HarmonyPatch("Die")]
  public class ProtoCrewMember_Die
  {
    [HarmonyPrefix]
    private static bool PrefixDie(ProtoCrewMember __instance) => MainSystem.NetworkState < ClientState.Connected || LockSystem.LockQuery.KerbalLockBelongsToPlayer(__instance.name, SettingsSystem.CurrentSettings.PlayerName);
  }
}
