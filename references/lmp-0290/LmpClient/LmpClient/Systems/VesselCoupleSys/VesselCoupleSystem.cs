// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCoupleSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCoupleSystem : 
    MessageSystem<VesselCoupleSystem, VesselCoupleMessageSender, VesselCoupleMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselCoupleQueue> VesselCouples { get; } = new ConcurrentDictionary<Guid, VesselCoupleQueue>();

    private VesselCoupleEvents VesselCoupleEvents { get; } = new VesselCoupleEvents();

    public bool IgnoreEvents { get; set; }

    protected override bool ProcessMessagesInUnityThread => true;

    public override string SystemName { get; } = nameof (VesselCoupleSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      PartEvent.onPartCoupling.Add(new EventData<Part, Part>.OnEvent((object) this.VesselCoupleEvents, __methodptr(CoupleStart)));
      // ISSUE: method pointer
      PartEvent.onPartCoupled.Add(new EventData<Part, Part, Guid>.OnEvent((object) this.VesselCoupleEvents, __methodptr(CoupleComplete)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.ProcessVesselCouples)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      PartEvent.onPartCoupling.Remove(new EventData<Part, Part>.OnEvent((object) this.VesselCoupleEvents, __methodptr(CoupleStart)));
      // ISSUE: method pointer
      PartEvent.onPartCoupled.Remove(new EventData<Part, Part, Guid>.OnEvent((object) this.VesselCoupleEvents, __methodptr(CoupleComplete)));
      this.VesselCouples.Clear();
    }

    private void ProcessVesselCouples()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselCoupleQueue>> enumerator = this.VesselCouples.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselCoupleQueue> current = enumerator.Current;
          while (true)
          {
            VesselCouple result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessCouple();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselCouples.TryRemove(vesselId, out VesselCoupleQueue _);
  }
}
