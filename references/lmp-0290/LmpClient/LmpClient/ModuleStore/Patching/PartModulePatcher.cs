// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Patching.PartModulePatcher
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.ModuleStore.Structures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LmpClient.ModuleStore.Patching
{
  public class PartModulePatcher
  {
    private static readonly ConcurrentDictionary<MethodBase, List<CodeInstruction>> InstructionsBackup = new ConcurrentDictionary<MethodBase, List<CodeInstruction>>();
    private static readonly HarmonyMethod RestoreTranspilerMethod = new HarmonyMethod(typeof (PartModulePatcher).GetMethod("Restore"));
    private static readonly HarmonyMethod BackupAndCallTranspilerMethod = new HarmonyMethod(typeof (PartModulePatcher).GetMethod("BackupAndCallTranspiler"));

    public static void PatchFieldsAndMethods(Type partModule)
    {
      ModuleDefinition moduleDefinition;
      if (!FieldModuleStore.CustomizedModuleBehaviours.TryGetValue(partModule.Name, out moduleDefinition) || !moduleDefinition.CustomizedFields.Any<KeyValuePair<string, FieldDefinition>>())
        return;
      foreach (MethodInfo methodInfo in ((IEnumerable<MethodInfo>) partModule.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "OnUpdate" || m.Name == "OnFixedUpdate" || m.Name == "FixedUpdate" || m.Name == "Update" || m.Name == "LateUpdate" || ((IEnumerable<object>) m.GetCustomAttributes(typeof (KSPAction), false)).Any<object>() || ((IEnumerable<object>) m.GetCustomAttributes(typeof (KSPEvent), false)).Any<object>((Func<object, bool>) (a => ((KSPEvent) a).guiActive)))))
      {
        if (methodInfo.GetMethodBody() != null)
        {
          try
          {
            LunaLog.Log("Patching method " + methodInfo.Name + " for field changes in module " + partModule.Name + " of assembly " + partModule.Assembly.GetName().Name);
            HarmonyPatcher.HarmonyInstance.Patch((MethodBase) methodInfo, (HarmonyMethod) null, (HarmonyMethod) null, PartModulePatcher.BackupAndCallTranspilerMethod, (HarmonyMethod) null);
          }
          catch (Exception ex)
          {
            LunaLog.LogError("Could not patch method " + methodInfo.Name + " for field changes in module " + partModule.Name + " " + string.Format("of assembly {0}. Details: {1}", (object) partModule.Assembly.GetName().Name, (object) ex));
            HarmonyPatcher.HarmonyInstance.Patch((MethodBase) methodInfo, (HarmonyMethod) null, (HarmonyMethod) null, PartModulePatcher.RestoreTranspilerMethod, (HarmonyMethod) null);
          }
        }
      }
    }

    public static IEnumerable<CodeInstruction> Restore(
      MethodBase originalMethod)
    {
      List<CodeInstruction> source;
      PartModulePatcher.InstructionsBackup.TryGetValue(originalMethod, out source);
      return ((IEnumerable<CodeInstruction>) source).AsEnumerable<CodeInstruction>();
    }

    public static IEnumerable<CodeInstruction> BackupAndCallTranspiler(
      ILGenerator generator,
      MethodBase originalMethod,
      IEnumerable<CodeInstruction> instructions)
    {
      if (originalMethod.DeclaringType == (Type) null)
        return instructions.AsEnumerable<CodeInstruction>();
      List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
      PartModulePatcher.InstructionsBackup.AddOrUpdate(originalMethod, codes, (Func<MethodBase, List<CodeInstruction>, List<CodeInstruction>>) ((methodBase, oldCodes) => codes));
      return new FieldChangeTranspiler(generator, originalMethod, codes).Transpile();
    }
  }
}
