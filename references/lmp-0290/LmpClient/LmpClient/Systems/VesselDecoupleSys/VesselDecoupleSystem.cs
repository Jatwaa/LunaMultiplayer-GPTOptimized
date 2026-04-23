// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselDecoupleSys.VesselDecoupleSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselDecoupleSys
{
  public class VesselDecoupleSystem : 
    MessageSystem<VesselDecoupleSystem, VesselDecoupleMessageSender, VesselDecoupleMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselDecoupleQueue> VesselDecouples { get; } = new ConcurrentDictionary<Guid, VesselDecoupleQueue>();

    private VesselDecoupleEvents VesselDecoupleEvents { get; } = new VesselDecoupleEvents();

    public bool IgnoreEvents { get; set; }

    public Guid ManuallyDecouplingVesselId { get; set; }

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselDecoupleSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      PartEvent.onPartDecoupling.Add(new EventData<Part, float>.OnEvent((object) this.VesselDecoupleEvents, __methodptr(DecoupleStart)));
      // ISSUE: method pointer
      PartEvent.onPartDecoupled.Add(new EventData<Part, float, Vessel>.OnEvent((object) this.VesselDecoupleEvents, __methodptr(DecoupleComplete)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.ProcessVesselDecouples)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      PartEvent.onPartDecoupling.Remove(new EventData<Part, float>.OnEvent((object) this.VesselDecoupleEvents, __methodptr(DecoupleStart)));
      // ISSUE: method pointer
      PartEvent.onPartDecoupled.Remove(new EventData<Part, float, Vessel>.OnEvent((object) this.VesselDecoupleEvents, __methodptr(DecoupleComplete)));
      this.VesselDecouples.Clear();
    }

    private void ProcessVesselDecouples()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselDecoupleQueue>> enumerator = this.VesselDecouples.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselDecoupleQueue> current = enumerator.Current;
          while (true)
          {
            VesselDecouple result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessDecouple();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselDecouples.TryRemove(vesselId, out VesselDecoupleQueue _);
  }
}
