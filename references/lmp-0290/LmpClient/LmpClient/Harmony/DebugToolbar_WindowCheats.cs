// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.DebugToolbar_WindowCheats
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (DebugToolbar))]
  [HarmonyPatch("WindowCheats")]
  public class DebugToolbar_WindowCheats
  {
    [HarmonyPrefix]
    private static bool PrefixWindowCheats(DebugToolbar __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected || SettingsSystem.ServerSettings.AllowCheats)
        return true;
      GUILayout.Label("Cheats are disabled on this server", Array.Empty<GUILayoutOption>());
      return false;
    }
  }
}
