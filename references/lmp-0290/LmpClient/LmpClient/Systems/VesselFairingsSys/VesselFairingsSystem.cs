// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFairingsSys.VesselFairingsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselFairingsSys
{
  public class VesselFairingsSystem : 
    MessageSystem<VesselFairingsSystem, VesselFairingsMessageSender, VesselFairingsMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselFairingQueue> VesselFairings { get; } = new ConcurrentDictionary<Guid, VesselFairingQueue>();

    private VesselFairingEvents VesselFairingEvents { get; } = new VesselFairingEvents();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselFairingsSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onFairingsDeployed.Add(new EventData<Part>.OnEvent((object) this.VesselFairingEvents, __methodptr(FairingsDeployed)));
      this.SetupRoutine(new RoutineDefinition(1500, RoutineExecution.Update, new Action(this.ProcessVesselFairings)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onFairingsDeployed.Remove(new EventData<Part>.OnEvent((object) this.VesselFairingEvents, __methodptr(FairingsDeployed)));
      this.VesselFairings.Clear();
    }

    private void ProcessVesselFairings()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselFairingQueue>> enumerator = this.VesselFairings.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselFairingQueue> current = enumerator.Current;
          while (true)
          {
            VesselFairing result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessFairing();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }

    public void RemoveVessel(Guid vesselId) => this.VesselFairings.TryRemove(vesselId, out VesselFairingQueue _);
  }
}
