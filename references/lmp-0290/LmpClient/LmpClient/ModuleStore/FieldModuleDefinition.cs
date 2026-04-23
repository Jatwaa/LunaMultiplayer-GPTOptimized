// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.FieldModuleDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;

namespace LmpClient.ModuleStore
{
  public class FieldModuleDefinition
  {
    public Type ModuleType { get; }

    public Dictionary<string, FieldInfo> PersistentModuleField { get; }

    public FieldModuleDefinition(Type moduleType, IEnumerable<FieldInfo> persistentFields)
    {
      this.ModuleType = moduleType;
      this.PersistentModuleField = Enumerable.ToDictionary<FieldInfo, string, FieldInfo>(persistentFields, (Func<FieldInfo, string>) (k => k.Name), (Func<FieldInfo, FieldInfo>) (k => k));
    }
  }
}
