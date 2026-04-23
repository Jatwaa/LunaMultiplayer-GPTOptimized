// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Structures.ModuleDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpCommon.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UniLinq;

namespace LmpClient.ModuleStore.Structures
{
  public class ModuleDefinition
  {
    [XmlComment(Value = "Module that we are modifying")]
    public string ModuleName { get; set; }

    [XmlComment(Value = "Fields to sync")]
    public List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

    [XmlComment(Value = "Methods to sync")]
    public List<MethodDefinition> Methods { get; set; } = new List<MethodDefinition>();

    [XmlIgnore]
    public Dictionary<string, FieldDefinition> CustomizedFields { get; private set; } = new Dictionary<string, FieldDefinition>();

    [XmlIgnore]
    public Dictionary<string, MethodDefinition> CustomizedMethods { get; private set; } = new Dictionary<string, MethodDefinition>();

    public void Init()
    {
      this.CustomizedFields = Enumerable.ToDictionary<FieldDefinition, string, FieldDefinition>(this.Fields.DistinctBy<FieldDefinition, string>((Func<FieldDefinition, string>) (f => f.FieldName)), (Func<FieldDefinition, string>) (f => f.FieldName), (Func<FieldDefinition, FieldDefinition>) (f => f));
      this.CustomizedMethods = Enumerable.ToDictionary<MethodDefinition, string, MethodDefinition>(this.Methods.DistinctBy<MethodDefinition, string>((Func<MethodDefinition, string>) (m => m.MethodName)), (Func<MethodDefinition, string>) (f => f.MethodName), (Func<MethodDefinition, MethodDefinition>) (f => f));
    }

    public void MergeWith(ModuleDefinition other)
    {
      this.Fields.AddRange((IEnumerable<FieldDefinition>) other.Fields);
      this.Methods.AddRange((IEnumerable<MethodDefinition>) other.Methods);
      this.Init();
    }
  }
}
