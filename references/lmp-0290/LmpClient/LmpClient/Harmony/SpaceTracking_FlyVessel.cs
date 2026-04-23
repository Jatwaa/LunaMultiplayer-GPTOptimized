// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.SpaceTracking_FlyVessel
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (SpaceTracking))]
  [HarmonyPatch("FlyVessel")]
  public class SpaceTracking_FlyVessel
  {
    [HarmonyPrefix]
    private static bool PrefixFlyVessel(SpaceTracking __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return true;
      Vessel selectedVessel = __instance.SelectedVessel;
      return !Object.op_Inequality((Object) selectedVessel, (Object) null) || selectedVessel.situation != 4 || LockSystem.LockQuery.CanRecoverOrTerminateTheVessel(selectedVessel.id, SettingsSystem.CurrentSettings.PlayerName);
    }
  }
}
