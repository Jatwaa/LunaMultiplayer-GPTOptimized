// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.UIPartActionButton_OnClick
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.ModuleStore;
using LmpClient.ModuleStore.Structures;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (UIPartActionButton))]
  [HarmonyPatch("OnClick")]
  public class UIPartActionButton_OnClick
  {
    [HarmonyPrefix]
    private static void PrefixOnClick(
      UIPartActionButton __instance,
      ref PartModule ___partModule,
      ref BaseEvent ___evt)
    {
      ModuleDefinition moduleDefinition;
      if (!((UIPartActionItem) __instance).IsModule || !Object.op_Inequality((Object) ___partModule, (Object) null) || ___evt == null || !FieldModuleStore.CustomizedModuleBehaviours.TryGetValue(___partModule.moduleName, out moduleDefinition) || !moduleDefinition.CustomizedMethods.ContainsKey(___evt.name))
        return;
      PartModuleEvent.onPartModuleMethodCalling.Fire(___partModule, ___evt.name);
    }
  }
}
