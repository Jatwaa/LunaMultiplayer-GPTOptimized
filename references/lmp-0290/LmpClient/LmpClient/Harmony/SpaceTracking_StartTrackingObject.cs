// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.SpaceTracking_StartTrackingObject
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (SpaceTracking))]
  [HarmonyPatch("StartTrackingObject")]
  public class SpaceTracking_StartTrackingObject
  {
    [HarmonyPostfix]
    private static void PostfixStartTrackingObject(Vessel v) => TrackingEvent.onStartTrackingAsteroidOrComet.Fire(v);
  }
}
