// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Harmony.ModuleEngines_Activate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.ModuleStore.Harmony
{
  [HarmonyPatch(typeof (ModuleEngines))]
  [HarmonyPatch("Activate")]
  public class ModuleEngines_Activate
  {
    [HarmonyPostfix]
    private static void PostfixActivate(ModuleEngines __instance)
    {
      if (__instance.staged)
        return;
      foreach (ModuleGimbal moduleGimbal in ((PartModule) __instance).part.FindModulesImplementing<ModuleGimbal>())
        PartModuleEvent.onPartModuleBoolFieldChanged.Fire((PartModule) moduleGimbal, "gimbalActive", moduleGimbal.gimbalActive);
    }
  }
}
