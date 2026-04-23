// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUndockSys.VesselUndockSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselUndockSys
{
  public class VesselUndockSystem : 
    MessageSystem<VesselUndockSystem, VesselUndockMessageSender, VesselUndockMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselUndockQueue> VesselUndocks { get; } = new ConcurrentDictionary<Guid, VesselUndockQueue>();

    private VesselUndockEvents VesselUndockEvents { get; } = new VesselUndockEvents();

    public bool IgnoreEvents { get; set; }

    public Guid ManuallyUndockingVesselId { get; set; }

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselUndockSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      PartEvent.onPartUndocking.Add(new EventData<Part, DockedVesselInfo>.OnEvent((object) this.VesselUndockEvents, __methodptr(UndockStart)));
      // ISSUE: method pointer
      PartEvent.onPartUndocked.Add(new EventData<Part, DockedVesselInfo, Vessel>.OnEvent((object) this.VesselUndockEvents, __methodptr(UndockComplete)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.ProcessVesselUndocks)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      PartEvent.onPartUndocking.Remove(new EventData<Part, DockedVesselInfo>.OnEvent((object) this.VesselUndockEvents, __methodptr(UndockStart)));
      // ISSUE: method pointer
      PartEvent.onPartUndocked.Remove(new EventData<Part, DockedVesselInfo, Vessel>.OnEvent((object) this.VesselUndockEvents, __methodptr(UndockComplete)));
      this.VesselUndocks.Clear();
    }

    private void ProcessVesselUndocks()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselUndockQueue>> enumerator = this.VesselUndocks.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselUndockQueue> current = enumerator.Current;
          while (true)
          {
            VesselUndock result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessUndock();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselUndocks.TryRemove(vesselId, out VesselUndockQueue _);
  }
}
