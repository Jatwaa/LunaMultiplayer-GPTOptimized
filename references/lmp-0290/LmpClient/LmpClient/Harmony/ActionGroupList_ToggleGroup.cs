// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ActionGroupList_ToggleGroup
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ActionGroupList))]
  [HarmonyPatch("ToggleGroup")]
  public class ActionGroupList_ToggleGroup
  {
    [HarmonyPostfix]
    private static void PostFixToggleGroup(ActionGroupList __instance, KSPActionGroup group)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      int groupIndex = BaseAction.GetGroupIndex(group);
      if (Planetarium.GetUniversalTime() < __instance.cooldownTimes[groupIndex])
        return;
      ActionGroupEvent.onActionGroupFired.Fire(__instance.v, group, __instance.groups[groupIndex]);
    }
  }
}
