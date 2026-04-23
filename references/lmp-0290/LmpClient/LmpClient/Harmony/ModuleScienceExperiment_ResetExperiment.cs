// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Harmony.ModuleScienceExperiment_ResetExperiment
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.ModuleStore.Harmony
{
  [HarmonyPatch(typeof (ModuleScienceExperiment))]
  [HarmonyPatch("resetExperiment")]
  public class ModuleScienceExperiment_ResetExperiment
  {
    [HarmonyPostfix]
    private static void PostfixResetExperiment(ModuleScienceExperiment __instance) => ExperimentEvent.onExperimentReset.Fire(((PartModule) __instance).vessel);
  }
}
