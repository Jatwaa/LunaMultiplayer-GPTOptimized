// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUpdateSys.VesselUpdateSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.TimeSync;
using LmpClient.VesselUtilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselUpdateSys
{
  public class VesselUpdateSystem : 
    MessageSystem<VesselUpdateSystem, VesselUpdateMessageSender, VesselUpdateMessageHandler>
  {
    public bool VesselUpdateSystemReady => this.Enabled && Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && HighLogic.LoadedScene == 7 && FlightGlobals.ready && FlightGlobals.ActiveVessel.loaded && FlightGlobals.ActiveVessel.state != 2 && FlightGlobals.ActiveVessel.vesselType != 12;

    private List<Vessel> SecondaryVesselsToUpdate { get; } = new List<Vessel>();

    private List<Vessel> AbandonedVesselsToUpdate { get; } = new List<Vessel>();

    public ConcurrentDictionary<Guid, VesselUpdateQueue> VesselUpdates { get; } = new ConcurrentDictionary<Guid, VesselUpdateQueue>();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselUpdateSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(1500, RoutineExecution.Update, new Action(this.SendVesselUpdates)));
      this.SetupRoutine(new RoutineDefinition(1500, RoutineExecution.Update, new Action(this.ProcessVesselUpdates)));
      this.SetupRoutine(new RoutineDefinition(5000, RoutineExecution.Update, new Action(this.SendSecondaryVesselUpdates)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.VesselUpdates.Clear();
    }

    private void ProcessVesselUpdates()
    {
      if (HighLogic.LoadedScene < 5)
        return;
      using (IEnumerator<KeyValuePair<Guid, VesselUpdateQueue>> enumerator = this.VesselUpdates.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselUpdateQueue> current = enumerator.Current;
          while (true)
          {
            VesselUpdate result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessVesselUpdate();
              current.Value.Recycle(result);
            }
            else
              goto label_7;
          }
        }
      }
    }

    private void SendVesselUpdates()
    {
      if (VesselCommon.IsSpectating || !this.VesselUpdateSystemReady)
        return;
      this.MessageSender.SendVesselUpdate(FlightGlobals.ActiveVessel);
    }

    private void SendSecondaryVesselUpdates()
    {
      if (VesselCommon.IsSpectating)
        return;
      this.SecondaryVesselsToUpdate.Clear();
      this.SecondaryVesselsToUpdate.AddRange(VesselCommon.GetSecondaryVessels());
      for (int index = 0; index < this.SecondaryVesselsToUpdate.Count; ++index)
        this.MessageSender.SendVesselUpdate(this.SecondaryVesselsToUpdate[index]);
    }

    private void SendUnloadedSecondaryVesselUpdates()
    {
      if (VesselCommon.IsSpectating)
        return;
      this.AbandonedVesselsToUpdate.Clear();
      this.AbandonedVesselsToUpdate.AddRange(VesselCommon.GetUnloadedSecondaryVessels());
      for (int index = 0; index < this.AbandonedVesselsToUpdate.Count; ++index)
        this.MessageSender.SendVesselUpdate(this.AbandonedVesselsToUpdate[index]);
    }
  }
}
