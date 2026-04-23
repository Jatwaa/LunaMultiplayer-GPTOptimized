// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncCallSys.VesselPartSyncCallSystem
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

namespace LmpClient.Systems.VesselPartSyncCallSys
{
  public class VesselPartSyncCallSystem : 
    MessageSystem<VesselPartSyncCallSystem, VesselPartSyncCallMessageSender, VesselPartSyncCallMessageHandler>
  {
    public bool PartSyncSystemReady => this.Enabled && HighLogic.LoadedScene >= 7 && (double) Time.timeSinceLevelLoad > 1.0;

    private VesselPartSyncCallEvents VesselPartModuleSyncCallEvents { get; } = new VesselPartSyncCallEvents();

    public ConcurrentDictionary<Guid, VesselPartSyncCallQueue> VesselPartsSyncs { get; } = new ConcurrentDictionary<Guid, VesselPartSyncCallQueue>();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselPartSyncCallSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleMethodCalling.Add(new EventData<PartModule, string>.OnEvent((object) this.VesselPartModuleSyncCallEvents, __methodptr(PartModuleMethodCalled)));
      this.SetupRoutine(new RoutineDefinition(250, RoutineExecution.Update, new Action(this.ProcessVesselPartSyncCalls)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      PartModuleEvent.onPartModuleMethodCalling.Remove(new EventData<PartModule, string>.OnEvent((object) this.VesselPartModuleSyncCallEvents, __methodptr(PartModuleMethodCalled)));
      this.VesselPartsSyncs.Clear();
    }

    private void ProcessVesselPartSyncCalls()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselPartSyncCallQueue>> enumerator = this.VesselPartsSyncs.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselPartSyncCallQueue> current = enumerator.Current;
          while (true)
          {
            VesselPartSyncCall result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessPartMethodCallSync();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselPartsSyncs.TryRemove(vesselId, out VesselPartSyncCallQueue _);
  }
}
