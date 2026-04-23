// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.DebugScreen_OnGameSceneLoad
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens.DebugToolbar;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (DebugScreen))]
  [HarmonyPatch("onGameSceneLoad")]
  public class DebugScreen_OnGameSceneLoad
  {
    [HarmonyPostfix]
    private static void PostFixOnGameSceneLoad(global::DebugToolbar __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      if (!SettingsSystem.ServerSettings.AllowCheats)
        Traverse.Create((object) __instance).Field("_cheatsLocked").SetValue((object) true);
      else
        Traverse.Create((object) __instance).Field("_cheatsLocked").SetValue((object) false);
    }
  }
}
