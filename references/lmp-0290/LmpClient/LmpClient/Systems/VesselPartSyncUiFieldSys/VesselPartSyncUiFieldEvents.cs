// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiFieldEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.ModuleStore;
using LmpClient.ModuleStore.Structures;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Locks;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiFieldEvents : SubSystem<VesselPartSyncUiFieldSystem>
  {
    private static bool CallIsValid(PartModule module)
    {
      Vessel vessel = module.vessel;
      return !Object.op_Equality((Object) vessel, (Object) null) && vessel.loaded && vessel.protoVessel != null && !Object.op_Equality((Object) module.part, (Object) null) && !module.vessel.IsImmortal();
    }

    public void LockAcquire(LockDefinition lockDef)
    {
      if (lockDef.Type != LockType.Control || !(lockDef.PlayerName == SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.SubscribeToFieldChanges(FlightGlobals.ActiveVessel);
    }

    public void SubscribeToFieldChanges(Vessel vessel)
    {
      foreach (Part part in vessel.parts)
      {
        foreach (PartModule module in part.Modules)
        {
          if (FieldModuleStore.CustomizedModuleBehaviours.TryGetValue(module.moduleName, out ModuleDefinition _))
          {
            foreach (BaseField field in (BaseFieldList<BaseField, KSPField>) module.Fields)
            {
              if (!(((object) field.uiControlFlight).GetType() != typeof (UI_Toggle)) || !(((object) field.uiControlFlight).GetType() != typeof (UI_FloatRange)) || !(((object) field.uiControlFlight).GetType() != typeof (UI_Cycle)))
              {
                UI_Control uiControlFlight1 = field.uiControlFlight;
                // ISSUE: method pointer
                uiControlFlight1.onFieldChanged = (Callback<BaseField, object>) Delegate.Remove((Delegate) uiControlFlight1.onFieldChanged, (Delegate) new Callback<BaseField, object>((object) null, __methodptr(OnFieldChanged)));
                UI_Control uiControlFlight2 = field.uiControlFlight;
                // ISSUE: method pointer
                uiControlFlight2.onFieldChanged = (Callback<BaseField, object>) Delegate.Combine((Delegate) uiControlFlight2.onFieldChanged, (Delegate) new Callback<BaseField, object>((object) null, __methodptr(OnFieldChanged)));
              }
            }
          }
        }
      }
    }

    private static void OnFieldChanged(BaseField baseField, object oldValue)
    {
      PartModule host = (PartModule) ((BaseField<KSPField>) baseField).host;
      if (!VesselPartSyncUiFieldEvents.CallIsValid(host))
        return;
      Type fieldType = ((BaseField<KSPField>) baseField).FieldInfo.FieldType;
      if (fieldType == typeof (bool))
        SubSystem<VesselPartSyncUiFieldSystem>.System.MessageSender.SendVesselPartSyncUiFieldBoolMsg(host.vessel, host.part, host.moduleName, ((BaseField<KSPField>) baseField).name, (bool) ((BaseField<KSPField>) baseField).GetValue(((BaseField<KSPField>) baseField).host));
      else if (fieldType == typeof (int))
      {
        SubSystem<VesselPartSyncUiFieldSystem>.System.MessageSender.SendVesselPartSyncUiFieldIntMsg(host.vessel, host.part, host.moduleName, ((BaseField<KSPField>) baseField).name, (int) ((BaseField<KSPField>) baseField).GetValue(((BaseField<KSPField>) baseField).host));
      }
      else
      {
        if (!(fieldType == typeof (float)))
          return;
        SubSystem<VesselPartSyncUiFieldSystem>.System.MessageSender.SendVesselPartSyncUiFieldFloatMsg(host.vessel, host.part, host.moduleName, ((BaseField<KSPField>) baseField).name, (float) ((BaseField<KSPField>) baseField).GetValue(((BaseField<KSPField>) baseField).host));
      }
    }
  }
}
