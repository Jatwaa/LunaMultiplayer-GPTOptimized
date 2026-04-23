// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.FieldModuleStore
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.ModuleStore.Structures;
using LmpClient.Utilities;
using LmpCommon.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LmpClient.ModuleStore
{
  public class FieldModuleStore
  {
    private static readonly string CustomPartSyncFolder = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "PartSync");
    public static Dictionary<string, ModuleDefinition> CustomizedModuleBehaviours = new Dictionary<string, ModuleDefinition>();

    private static IEnumerable<Type> PartModuleTypes { get; } = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (a => a.GetLoadableTypes())).Where<Type>((Func<Type, bool>) (t => t.IsClass && t.IsSubclassOf(typeof (PartModule))));

    public static void ReadCustomizationXml()
    {
      List<ModuleDefinition> source = new List<ModuleDefinition>();
      foreach (string file in Directory.GetFiles(FieldModuleStore.CustomPartSyncFolder, "*.xml", SearchOption.AllDirectories))
      {
        ModuleDefinition moduleDefinition = LunaXmlSerializer.ReadXmlFromPath<ModuleDefinition>(file);
        moduleDefinition.ModuleName = Path.GetFileNameWithoutExtension(file);
        source.Add(moduleDefinition);
      }
      FieldModuleStore.CustomizedModuleBehaviours = source.ToDictionary<ModuleDefinition, string, ModuleDefinition>((Func<ModuleDefinition, string>) (m => m.ModuleName), (Func<ModuleDefinition, ModuleDefinition>) (v => v));
      List<ModuleDefinition> moduleDefinitionList = new List<ModuleDefinition>();
      foreach (ModuleDefinition moduleDefinition in FieldModuleStore.CustomizedModuleBehaviours.Values)
      {
        ModuleDefinition value = moduleDefinition;
        Type moduleClass = FieldModuleStore.PartModuleTypes.FirstOrDefault<Type>((Func<Type, bool>) (t => t.Name == value.ModuleName));
        if (moduleClass != (Type) null)
        {
          FieldModuleStore.AddParentsCustomizations(value, moduleClass);
          moduleDefinitionList.AddRange((IEnumerable<ModuleDefinition>) FieldModuleStore.GetChildCustomizations(value, moduleClass));
        }
      }
      foreach (ModuleDefinition moduleDefinition in moduleDefinitionList)
      {
        if (!FieldModuleStore.CustomizedModuleBehaviours.ContainsKey(moduleDefinition.ModuleName))
          FieldModuleStore.CustomizedModuleBehaviours.Add(moduleDefinition.ModuleName, moduleDefinition);
      }
      foreach (ModuleDefinition moduleDefinition in FieldModuleStore.CustomizedModuleBehaviours.Values)
        moduleDefinition.Init();
    }

    private static List<ModuleDefinition> GetChildCustomizations(
      ModuleDefinition moduleDefinition,
      Type moduleClass)
    {
      List<ModuleDefinition> childCustomizations = new List<ModuleDefinition>();
      foreach (Type type in FieldModuleStore.PartModuleTypes.Where<Type>((Func<Type, bool>) (t => t.BaseType == moduleClass)))
      {
        if (!FieldModuleStore.CustomizedModuleBehaviours.ContainsKey(type.Name))
          childCustomizations.Add(new ModuleDefinition()
          {
            ModuleName = type.Name,
            Fields = moduleDefinition.Fields,
            Methods = moduleDefinition.Methods
          });
      }
      return childCustomizations;
    }

    private static void AddParentsCustomizations(
      ModuleDefinition moduleDefinition,
      Type moduleClass)
    {
      if (moduleClass.BaseType == (Type) null || moduleClass.BaseType == typeof (MonoBehaviour))
        return;
      ModuleDefinition other;
      if (FieldModuleStore.CustomizedModuleBehaviours.TryGetValue(moduleClass.BaseType.Name, out other))
        moduleDefinition.MergeWith(other);
      FieldModuleStore.AddParentsCustomizations(moduleDefinition, moduleClass.BaseType);
    }
  }
}
