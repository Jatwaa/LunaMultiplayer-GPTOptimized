// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.SpaceTracking_SetVessel
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI;
using KSP.UI.Screens;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (SpaceTracking))]
  [HarmonyPatch("SetVessel")]
  public class SpaceTracking_SetVessel
  {
    [HarmonyPostfix]
    private static void PostfixSetVessel(SpaceTracking __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected || !Object.op_Inequality((Object) __instance.SelectedVessel, (Object) null))
        return;
      if (!LockSystem.LockQuery.CanRecoverOrTerminateTheVessel(__instance.SelectedVessel.id, SettingsSystem.CurrentSettings.PlayerName))
      {
        if (__instance.SelectedVessel.situation == 4)
          UIExtensions.Lock((Selectable) __instance.FlyButton);
        else
          UIExtensions.Unlock((Selectable) __instance.FlyButton);
        UIExtensions.Lock((Selectable) __instance.DeleteButton);
        UIExtensions.Lock((Selectable) __instance.RecoverButton);
      }
      else if (__instance.SelectedVessel.IsRecoverable)
      {
        UIExtensions.Unlock((Selectable) __instance.FlyButton);
        UIExtensions.Unlock((Selectable) __instance.DeleteButton);
        UIExtensions.Unlock((Selectable) __instance.RecoverButton);
      }
      else
      {
        UIExtensions.Unlock((Selectable) __instance.FlyButton);
        UIExtensions.Unlock((Selectable) __instance.DeleteButton);
        UIExtensions.Lock((Selectable) __instance.RecoverButton);
      }
    }
  }
}
