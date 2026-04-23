// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightDriver_ReturnToEditor
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightDriver))]
  [HarmonyPatch("ReturnToEditor")]
  public class FlightDriver_ReturnToEditor
  {
    [HarmonyPrefix]
    private static void PrefixReturnToEditor(EditorFacility facility) => RevertEvent.onReturningToEditor.Fire(facility);

    [HarmonyPostfix]
    private static void PostfixReturnToEditor(EditorFacility facility) => RevertEvent.onReturnedToEditor.Fire(facility);
  }
}
