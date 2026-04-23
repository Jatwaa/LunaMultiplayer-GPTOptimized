// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncField
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncField
  {
    public double GameTime;
    public Guid VesselId;
    public uint PartFlightId;
    public string ModuleName;
    public string FieldName;
    public PartSyncFieldType FieldType;
    public string StrValue;
    public bool BoolValue;
    public short ShortValue;
    public ushort UShortValue;
    public int IntValue;
    public uint UIntValue;
    public float FloatValue;
    public long LongValue;
    public ulong ULongValue;
    public double DoubleValue;
    public Vector2 Vector2Value;
    public Vector3 Vector3Value;
    public Quaternion QuaternionValue;

    public void ProcessPartFieldSync()
    {
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !VesselCommon.DoVesselChecks(this.VesselId))
        return;
      ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.PartFlightId);
      ProtoPartModuleSnapshot partModuleSnapshot = protoPart != null ? protoPart.FindProtoPartModuleInProtoPart(this.ModuleName) : (ProtoPartModuleSnapshot) null;
      if (partModuleSnapshot == null)
        return;
      switch (this.FieldType)
      {
        case PartSyncFieldType.Boolean:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.BoolValue, false);
          PartModuleEvent.onPartModuleBoolFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.BoolValue);
          break;
        case PartSyncFieldType.Short:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.ShortValue, false);
          PartModuleEvent.onPartModuleShortFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.ShortValue);
          break;
        case PartSyncFieldType.UShort:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.UShortValue, false);
          PartModuleEvent.onPartModuleUShortFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.UShortValue);
          break;
        case PartSyncFieldType.Integer:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.IntValue, false);
          PartModuleEvent.onPartModuleIntFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.IntValue);
          break;
        case PartSyncFieldType.UInteger:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.UIntValue, false);
          PartModuleEvent.onPartModuleUIntFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.UIntValue);
          break;
        case PartSyncFieldType.Float:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.FloatValue, false);
          PartModuleEvent.onPartModuleFloatFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.FloatValue);
          break;
        case PartSyncFieldType.Long:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.LongValue, false);
          PartModuleEvent.onPartModuleLongFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.LongValue);
          break;
        case PartSyncFieldType.ULong:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.ULongValue, false);
          PartModuleEvent.onPartModuleULongFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.ULongValue);
          break;
        case PartSyncFieldType.Double:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.DoubleValue, false);
          PartModuleEvent.onPartModuleDoubleFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.DoubleValue);
          break;
        case PartSyncFieldType.Vector2:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.Vector2Value, false);
          PartModuleEvent.onPartModuleVector2FieldProcessed.Fire(partModuleSnapshot, this.FieldName, Vector2.op_Implicit(this.Vector2Value));
          break;
        case PartSyncFieldType.Vector3:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.Vector3Value, false);
          PartModuleEvent.onPartModuleVector3FieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.Vector3Value);
          break;
        case PartSyncFieldType.Quaternion:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.QuaternionValue, false);
          PartModuleEvent.onPartModuleQuaternionFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.QuaternionValue);
          break;
        case PartSyncFieldType.String:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.StrValue, false);
          PartModuleEvent.onPartModuleStringFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.StrValue);
          break;
        case PartSyncFieldType.Object:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.StrValue, false);
          PartModuleEvent.onPartModuleObjectFieldProcessed.Fire(partModuleSnapshot, this.FieldName, (object) this.StrValue);
          break;
        case PartSyncFieldType.Enum:
          partModuleSnapshot.moduleValues.SetValue(this.FieldName, this.StrValue, false);
          PartModuleEvent.onPartModuleEnumFieldProcessed.Fire(partModuleSnapshot, this.FieldName, this.IntValue, this.StrValue);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
