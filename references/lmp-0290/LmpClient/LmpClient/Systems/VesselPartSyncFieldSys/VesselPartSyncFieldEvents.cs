// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncFieldEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.ModuleStore;
using LmpClient.ModuleStore.Structures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncFieldEvents : SubSystem<VesselPartSyncFieldSystem>
  {
    private static readonly Dictionary<Guid, Dictionary<uint, Dictionary<string, Dictionary<string, TimeToSend>>>> LastSendTimeDictionary = new Dictionary<Guid, Dictionary<uint, Dictionary<string, Dictionary<string, TimeToSend>>>>();

    private static bool CallIsValid(PartModule module, string fieldName)
    {
      Vessel vessel = module.vessel;
      if (Object.op_Equality((Object) vessel, (Object) null) || !vessel.loaded || vessel.protoVessel == null)
        return false;
      Part part = module.part;
      if (Object.op_Equality((Object) part, (Object) null) || part.vessel.IsImmortal())
        return false;
      ModuleDefinition moduleDefinition;
      if (FieldModuleStore.CustomizedModuleBehaviours.TryGetValue(module.moduleName, out moduleDefinition))
      {
        FieldDefinition fieldCust;
        if (moduleDefinition.CustomizedFields.TryGetValue(fieldName, out fieldCust))
          return VesselPartSyncFieldEvents.LastSendTimeDictionary.GetOrAdd<Guid, Dictionary<uint, Dictionary<string, Dictionary<string, TimeToSend>>>>(module.vessel.id, (Func<Dictionary<uint, Dictionary<string, Dictionary<string, TimeToSend>>>>) (() => new Dictionary<uint, Dictionary<string, Dictionary<string, TimeToSend>>>())).GetOrAdd<uint, Dictionary<string, Dictionary<string, TimeToSend>>>(module.part.flightID, (Func<Dictionary<string, Dictionary<string, TimeToSend>>>) (() => new Dictionary<string, Dictionary<string, TimeToSend>>())).GetOrAdd<string, Dictionary<string, TimeToSend>>(module.moduleName, (Func<Dictionary<string, TimeToSend>>) (() => new Dictionary<string, TimeToSend>())).GetOrAdd<string, TimeToSend>(fieldName, (Func<TimeToSend>) (() => new TimeToSend(fieldCust.MaxIntervalInMs))).ReadyToSend();
      }
      return true;
    }

    public void PartModuleBoolFieldChanged(PartModule module, string fieldName, bool newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new BOOL value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldBoolMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleShortFieldChanged(PartModule module, string fieldName, short newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new SHORT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldShortMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleUshortFieldChanged(PartModule module, string fieldName, ushort newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new USHORT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldUshortMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleIntFieldChanged(PartModule module, string fieldName, int newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new INT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldIntMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleUintFieldChanged(PartModule module, string fieldName, uint newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new UINT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldUIntMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleFloatFieldChanged(PartModule module, string fieldName, float newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new FLOAT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldFloatMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleLongFieldChanged(PartModule module, string fieldName, long newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new LONG value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldLongMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleUlongFieldChanged(PartModule module, string fieldName, ulong newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new ULONG value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldULongMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleDoubleFieldChanged(PartModule module, string fieldName, double newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new DOUBLE value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldDoubleMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleVector2FieldChanged(
      PartModule module,
      string fieldName,
      Vector2 newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new VECTOR2 value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldVector2Msg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleVector3FieldChanged(
      PartModule module,
      string fieldName,
      Vector3 newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new VECTOR3 value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldVector3Msg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleQuaternionFieldChanged(
      PartModule module,
      string fieldName,
      Quaternion newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new QUATERNION value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldQuaternionMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleStringFieldChanged(PartModule module, string fieldName, string newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new STRING value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValue));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldStringMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleObjectFieldChanged(PartModule module, string fieldName, object newValue)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new OBJECT value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, newValue));
      LunaLog.LogWarning(string.Format("Field {0} in module {1} from part {2} has a field type that is not supported!", (object) fieldName, (object) module.moduleName, (object) module.part.flightID));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldObjectMsg(module.vessel, module.part, module.moduleName, fieldName, newValue);
    }

    public void PartModuleEnumFieldChanged(
      PartModule module,
      string fieldName,
      int newValue,
      string newValueStr)
    {
      if (!VesselPartSyncFieldEvents.CallIsValid(module, fieldName))
        return;
      LunaLog.Log(string.Format("Field {0} in module {1} from part {2} has a new ENUM value of {3}.", (object) fieldName, (object) module.moduleName, (object) module.part.flightID, (object) newValueStr));
      SubSystem<VesselPartSyncFieldSystem>.System.MessageSender.SendVesselPartSyncFieldEnumMsg(module.vessel, module.part, module.moduleName, fieldName, newValue, newValueStr);
    }
  }
}
