// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiFieldSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using LmpCommon.Locks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiFieldSystem : 
    MessageSystem<VesselPartSyncUiFieldSystem, VesselPartSyncUiFieldMessageSender, VesselPartSyncUiFieldMessageHandler>
  {
    public bool PartSyncSystemReady => this.Enabled && HighLogic.LoadedScene >= 7 && (double) Time.timeSinceLevelLoad > 1.0;

    private VesselPartSyncUiFieldEvents VesselPartModuleSyncUiFieldEvents { get; } = new VesselPartSyncUiFieldEvents();

    public ConcurrentDictionary<Guid, VesselPartSyncUiFieldQueue> VesselPartsUiFieldsSyncs { get; } = new ConcurrentDictionary<Guid, VesselPartSyncUiFieldQueue>();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselPartSyncUiFieldSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) this.VesselPartModuleSyncUiFieldEvents, __methodptr(LockAcquire)));
      this.SetupRoutine(new RoutineDefinition(250, RoutineExecution.Update, new Action(this.ProcessVesselPartUiFieldsSyncs)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) this.VesselPartModuleSyncUiFieldEvents, __methodptr(LockAcquire)));
      this.VesselPartsUiFieldsSyncs.Clear();
    }

    private void ProcessVesselPartUiFieldsSyncs()
    {
      if (HighLogic.LoadedScene < 5)
        return;
      using (IEnumerator<KeyValuePair<Guid, VesselPartSyncUiFieldQueue>> enumerator = this.VesselPartsUiFieldsSyncs.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselPartSyncUiFieldQueue> current = enumerator.Current;
          while (true)
          {
            VesselPartSyncUiField result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessPartMethodSync();
              current.Value.Recycle(result);
            }
            else
              goto label_7;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselPartsUiFieldsSyncs.TryRemove(vesselId, out VesselPartSyncUiFieldQueue _);
  }
}
