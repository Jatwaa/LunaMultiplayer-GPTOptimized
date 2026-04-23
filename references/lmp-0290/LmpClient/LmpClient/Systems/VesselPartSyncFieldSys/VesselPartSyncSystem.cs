// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncFieldSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncFieldSystem : 
    MessageSystem<VesselPartSyncFieldSystem, VesselPartSyncFieldMessageSender, VesselPartSyncFieldMessageHandler>
  {
    public bool PartSyncSystemReady => this.Enabled && HighLogic.LoadedScene >= 7 && (double) Time.timeSinceLevelLoad > 1.0;

    private VesselPartSyncFieldEvents VesselPartModuleSyncFieldEvents { get; } = new VesselPartSyncFieldEvents();

    public ConcurrentDictionary<Guid, VesselPartSyncFieldQueue> VesselPartsSyncs { get; } = new ConcurrentDictionary<Guid, VesselPartSyncFieldQueue>();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselPartSyncFieldSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleBoolFieldChanged.Add(new EventData<PartModule, string, bool>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleBoolFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleShortFieldChanged.Add(new EventData<PartModule, string, short>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleShortFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleUShortFieldChanged.Add(new EventData<PartModule, string, ushort>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUshortFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleIntFieldChanged.Add(new EventData<PartModule, string, int>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleIntFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleUIntFieldChanged.Add(new EventData<PartModule, string, uint>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUintFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleFloatFieldChanged.Add(new EventData<PartModule, string, float>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleFloatFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleLongFieldChanged.Add(new EventData<PartModule, string, long>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleLongFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleULongFieldChanged.Add(new EventData<PartModule, string, ulong>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUlongFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleDoubleFieldChanged.Add(new EventData<PartModule, string, double>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleDoubleFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleVector2FieldChanged.Add(new EventData<PartModule, string, Vector2>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleVector2FieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleVector3FieldChanged.Add(new EventData<PartModule, string, Vector3>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleVector3FieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleQuaternionFieldChanged.Add(new EventData<PartModule, string, Quaternion>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleQuaternionFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleStringFieldChanged.Add(new EventData<PartModule, string, string>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleStringFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleObjectFieldChanged.Add(new EventData<PartModule, string, object>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleObjectFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleEnumFieldChanged.Add(new EventData<PartModule, string, int, string>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleEnumFieldChanged)));
      this.SetupRoutine(new RoutineDefinition(250, RoutineExecution.Update, new Action(this.ProcessVesselPartSyncs)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleBoolFieldChanged.Remove(new EventData<PartModule, string, bool>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleBoolFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleShortFieldChanged.Remove(new EventData<PartModule, string, short>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleShortFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleUShortFieldChanged.Remove(new EventData<PartModule, string, ushort>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUshortFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleIntFieldChanged.Remove(new EventData<PartModule, string, int>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleIntFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleUIntFieldChanged.Remove(new EventData<PartModule, string, uint>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUintFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleFloatFieldChanged.Remove(new EventData<PartModule, string, float>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleFloatFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleLongFieldChanged.Remove(new EventData<PartModule, string, long>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleLongFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleULongFieldChanged.Remove(new EventData<PartModule, string, ulong>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleUlongFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleDoubleFieldChanged.Remove(new EventData<PartModule, string, double>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleDoubleFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleVector2FieldChanged.Remove(new EventData<PartModule, string, Vector2>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleVector2FieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleVector3FieldChanged.Remove(new EventData<PartModule, string, Vector3>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleVector3FieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleQuaternionFieldChanged.Remove(new EventData<PartModule, string, Quaternion>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleQuaternionFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleStringFieldChanged.Remove(new EventData<PartModule, string, string>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleStringFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleObjectFieldChanged.Remove(new EventData<PartModule, string, object>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleObjectFieldChanged)));
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleEnumFieldChanged.Remove(new EventData<PartModule, string, int, string>.OnEvent((object) this.VesselPartModuleSyncFieldEvents, __methodptr(PartModuleEnumFieldChanged)));
      this.VesselPartsSyncs.Clear();
    }

    private void ProcessVesselPartSyncs()
    {
      if (HighLogic.LoadedScene < 5)
        return;
      using (IEnumerator<KeyValuePair<Guid, VesselPartSyncFieldQueue>> enumerator = this.VesselPartsSyncs.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselPartSyncFieldQueue> current = enumerator.Current;
          while (true)
          {
            VesselPartSyncField result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessPartFieldSync();
              current.Value.Recycle(result);
            }
            else
              goto label_7;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselPartsSyncs.TryRemove(vesselId, out VesselPartSyncFieldQueue _);
  }
}
