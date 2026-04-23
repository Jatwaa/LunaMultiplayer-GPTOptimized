// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Patching.FieldChangeTranspiler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.ModuleStore.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LmpClient.ModuleStore.Patching
{
  public class FieldChangeTranspiler
  {
    private readonly ModuleDefinition _definition;
    private readonly ILGenerator _generator;
    private readonly MethodBase _originalMethod;
    private readonly List<CodeInstruction> _codes;
    private readonly Dictionary<int, int> _fieldIndexToLocalVarDictionary = new Dictionary<int, int>();

    private int LastIndex => this._codes.Count - 1;

    public FieldChangeTranspiler(
      ILGenerator generator,
      MethodBase originalMethod,
      List<CodeInstruction> codes)
    {
      this._definition = FieldModuleStore.CustomizedModuleBehaviours[originalMethod.DeclaringType.Name];
      this._generator = generator;
      this._originalMethod = originalMethod;
      this._codes = codes;
      this._fieldIndexToLocalVarDictionary.Clear();
    }

    public IEnumerable<CodeInstruction> Transpile()
    {
      this.TranspileBackupFields();
      this.TranspileEvaluations();
      return ((IEnumerable<CodeInstruction>) this._codes).AsEnumerable<CodeInstruction>();
    }

    private void TranspileBackupFields()
    {
      List<FieldDefinition> list = this._definition.Fields.ToList<FieldDefinition>();
      for (int index = 0; index < list.Count; ++index)
      {
        FieldInfo fieldInfo = AccessTools.Field(this._originalMethod.DeclaringType, list[index].FieldName);
        if (fieldInfo == (FieldInfo) null)
        {
          LunaLog.LogError(string.Format("Field {0} not found in module {1}", (object) list[index].FieldName, (object) this._originalMethod.DeclaringType));
        }
        else
        {
          LocalBuilder localBuilder = this._generator.DeclareLocal(fieldInfo.FieldType);
          this._fieldIndexToLocalVarDictionary.Add(index, localBuilder.LocalIndex);
          this._codes.Insert(0, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
          this._codes.Insert(1, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
          switch (this._fieldIndexToLocalVarDictionary[index])
          {
            case 0:
              this._codes.Insert(2, new CodeInstruction(OpCodes.Stloc_0, (object) null));
              break;
            case 1:
              this._codes.Insert(2, new CodeInstruction(OpCodes.Stloc_1, (object) null));
              break;
            case 2:
              this._codes.Insert(2, new CodeInstruction(OpCodes.Stloc_2, (object) null));
              break;
            case 3:
              this._codes.Insert(2, new CodeInstruction(OpCodes.Stloc_3, (object) null));
              break;
            default:
              this._codes.Insert(2, new CodeInstruction(OpCodes.Stloc_S, (object) this._fieldIndexToLocalVarDictionary[index]));
              break;
          }
        }
      }
    }

    private void TranspileEvaluations()
    {
      List<CodeInstruction> startComparisonInstructions = new List<CodeInstruction>();
      List<CodeInstruction> jmpInstructions = new List<CodeInstruction>();
      List<FieldDefinition> list = this._definition.Fields.DistinctBy<FieldDefinition, string>((Func<FieldDefinition, string>) (f => f.FieldName)).ToList<FieldDefinition>();
      for (int index = 0; index < list.Count; ++index)
      {
        FieldInfo fieldInfo = AccessTools.Field(this._originalMethod.DeclaringType, list[index].FieldName);
        if (!(fieldInfo == (FieldInfo) null))
        {
          LocalBuilder localBuilder = this._generator.DeclareLocal(typeof (bool));
          switch (this._fieldIndexToLocalVarDictionary[index])
          {
            case 0:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_0, (object) null));
              break;
            case 1:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_1, (object) null));
              break;
            case 2:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_2, (object) null));
              break;
            case 3:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_3, (object) null));
              break;
            default:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_S, (object) this._fieldIndexToLocalVarDictionary[index]));
              break;
          }
          if (index == 0)
            this.RedirectExistingReturns(this._codes);
          else
            startComparisonInstructions.Add(this._codes[this._codes.Count - 2]);
          if (fieldInfo.FieldType == typeof (Quaternion))
          {
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Call, (object) AccessTools.Method(typeof (Quaternion), "op_Inequality", (Type[]) null, (Type[]) null)));
          }
          else if (fieldInfo.FieldType == typeof (Vector2))
          {
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Call, (object) AccessTools.Method(typeof (Vector2), "op_Inequality", (Type[]) null, (Type[]) null)));
          }
          else if (fieldInfo.FieldType == typeof (Vector3))
          {
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Call, (object) AccessTools.Method(typeof (Vector3), "op_Inequality", (Type[]) null, (Type[]) null)));
          }
          else
          {
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ceq, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldc_I4_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ceq, (object) null));
          }
          switch (localBuilder.LocalIndex)
          {
            case 0:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Stloc_0, (object) null));
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_0, (object) null));
              break;
            case 1:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Stloc_1, (object) null));
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_1, (object) null));
              break;
            case 2:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Stloc_2, (object) null));
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_2, (object) null));
              break;
            case 3:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Stloc_3, (object) null));
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_3, (object) null));
              break;
            default:
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Stloc_S, (object) localBuilder.LocalIndex));
              this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldloc_S, (object) localBuilder.LocalIndex));
              break;
          }
          if (index == list.Count - 1)
          {
            if (!this._codes[this.LastIndex].labels.Any<Label>())
              this._codes[this.LastIndex].labels.Add(this._generator.DefineLabel());
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Brfalse_S, (object) this._codes[this.LastIndex].labels[0]));
          }
          else
          {
            CodeInstruction codeInstruction = new CodeInstruction(OpCodes.Brfalse_S, (object) null);
            this._codes.Insert(this.LastIndex, codeInstruction);
            jmpInstructions.Add(codeInstruction);
          }
          this.LoadFunctionByFieldType(fieldInfo.FieldType);
          this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
          this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldstr, (object) fieldInfo.Name));
          this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
          this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldfld, (object) fieldInfo));
          if (fieldInfo.FieldType.IsEnum)
          {
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldarg_0, (object) null));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldflda, (object) fieldInfo));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Constrained, (object) fieldInfo.FieldType));
            this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (object), "ToString", (Type[]) null, (Type[]) null)));
          }
          this.CallFunctionByFieldType(fieldInfo.FieldType);
        }
      }
      this.FixFallbackInstructions(startComparisonInstructions, jmpInstructions);
    }

    private void FixFallbackInstructions(
      List<CodeInstruction> startComparisonInstructions,
      List<CodeInstruction> jmpInstructions)
    {
      for (int index = 0; index < startComparisonInstructions.Count; ++index)
      {
        Label label = this._generator.DefineLabel();
        startComparisonInstructions[index].labels.Add(label);
        jmpInstructions[index].operand = (object) label;
      }
    }

    private void RedirectExistingReturns(List<CodeInstruction> codes)
    {
      Label label = this._generator.DefineLabel();
      codes[codes.Count - 2].labels.Add(label);
      List<Label> labels = ((IEnumerable<CodeInstruction>) codes).Last<CodeInstruction>().labels;
      for (int index = 0; index < codes.Count - 1; ++index)
      {
        if (codes[index].opcode == OpCodes.Ret)
        {
          codes[index].opcode = OpCodes.Br;
          codes[index].operand = (object) label;
        }
        if (codes[index].operand is Label operand && labels.Contains(operand))
          codes[index].operand = (object) label;
      }
    }

    private void LoadFunctionByFieldType(Type fieldType)
    {
      if (fieldType == typeof (bool))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleBoolFieldChanged")));
      else if (fieldType == typeof (short))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleShortFieldChanged")));
      else if (fieldType == typeof (ushort))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleUShortFieldChanged")));
      else if (fieldType == typeof (int))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleIntFieldChanged")));
      else if (fieldType == typeof (uint))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleUIntFieldChanged")));
      else if (fieldType == typeof (float))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleFloatFieldChanged")));
      else if (fieldType == typeof (long))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleLongFieldChanged")));
      else if (fieldType == typeof (ulong))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleULongFieldChanged")));
      else if (fieldType == typeof (double))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleDoubleFieldChanged")));
      else if (fieldType == typeof (string))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleStringFieldChanged")));
      else if (fieldType == typeof (Quaternion))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleQuaternionFieldChanged")));
      else if (fieldType == typeof (Vector2))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleVector2FieldChanged")));
      else if (fieldType == typeof (Vector3))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleVector3FieldChanged")));
      else if (fieldType.IsEnum)
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleEnumFieldChanged")));
      else
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Ldsfld, (object) AccessTools.Field(typeof (PartModuleEvent), "onPartModuleObjectFieldChanged")));
    }

    private void CallFunctionByFieldType(Type fieldType)
    {
      if (fieldType == typeof (bool))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, bool>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (short))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, short>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (ushort))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, ushort>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (int))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, int>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (uint))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, uint>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (float))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, float>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (long))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, long>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (ulong))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, ulong>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (double))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, double>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (string))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, string>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (Quaternion))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, Quaternion>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (Vector2))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, Vector2>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType == typeof (Vector3))
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, Vector3>), "Fire", (Type[]) null, (Type[]) null)));
      else if (fieldType.IsEnum)
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, int, string>), "Fire", (Type[]) null, (Type[]) null)));
      else
        this._codes.Insert(this.LastIndex, new CodeInstruction(OpCodes.Callvirt, (object) AccessTools.Method(typeof (EventData<PartModule, string, object>), "Fire", (Type[]) null, (Type[]) null)));
    }
  }
}
