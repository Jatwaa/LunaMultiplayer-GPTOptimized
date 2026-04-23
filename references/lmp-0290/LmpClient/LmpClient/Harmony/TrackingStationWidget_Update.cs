// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.TrackingStationWidget_Update
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Events;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (TrackingStationWidget))]
  [HarmonyPatch("Update")]
  public class TrackingStationWidget_Update
  {
    [HarmonyPostfix]
    private static void PostUpdate(TrackingStationWidget __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      LabelEvent.onMapWidgetTextProcessed.Fire(__instance);
    }
  }
}
