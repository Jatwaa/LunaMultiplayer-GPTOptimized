// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiField
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiField
  {
    public double GameTime;
    public Guid VesselId;
    public uint PartFlightId;
    public string ModuleName;
    public string FieldName;
    public PartSyncFieldType FieldType;
    public bool BoolValue;
    public int IntValue;
    public float FloatValue;

    public void ProcessPartMethodSync()
    {
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !VesselCommon.DoVesselChecks(this.VesselId))
        return;
      ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.PartFlightId);
      if (protoPart == null)
        return;
      ProtoPartModuleSnapshot moduleInProtoPart = protoPart.FindProtoPartModuleInProtoPart(this.ModuleName);
      if (moduleInProtoPart != null)
      {
        switch (this.FieldType)
        {
          case PartSyncFieldType.Boolean:
            moduleInProtoPart.moduleValues.SetValue(this.FieldName, this.BoolValue, false);
            if (Object.op_Inequality((Object) moduleInProtoPart.moduleRef, (Object) null))
              ((BaseField<KSPField>) ((BaseFieldList<BaseField, KSPField>) moduleInProtoPart.moduleRef.Fields)[this.FieldName]).SetValue((object) this.BoolValue, (object) moduleInProtoPart.moduleRef);
            PartModuleEvent.onPartModuleBoolFieldProcessed.Fire(moduleInProtoPart, this.FieldName, this.BoolValue);
            break;
          case PartSyncFieldType.Integer:
            moduleInProtoPart.moduleValues.SetValue(this.FieldName, this.IntValue, false);
            if (Object.op_Inequality((Object) moduleInProtoPart.moduleRef, (Object) null))
              ((BaseField<KSPField>) ((BaseFieldList<BaseField, KSPField>) moduleInProtoPart.moduleRef.Fields)[this.FieldName]).SetValue((object) this.IntValue, (object) moduleInProtoPart.moduleRef);
            PartModuleEvent.onPartModuleIntFieldProcessed.Fire(moduleInProtoPart, this.FieldName, this.IntValue);
            break;
          case PartSyncFieldType.Float:
            moduleInProtoPart.moduleValues.SetValue(this.FieldName, this.FloatValue, false);
            if (Object.op_Inequality((Object) moduleInProtoPart.moduleRef, (Object) null))
              ((BaseField<KSPField>) ((BaseFieldList<BaseField, KSPField>) moduleInProtoPart.moduleRef.Fields)[this.FieldName]).SetValue((object) this.FloatValue, (object) moduleInProtoPart.moduleRef);
            PartModuleEvent.onPartModuleFloatFieldProcessed.Fire(moduleInProtoPart, this.FieldName, this.FloatValue);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
}
