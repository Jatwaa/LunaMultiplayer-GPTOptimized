// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselResourceSys.VesselResourceSystem
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

namespace LmpClient.Systems.VesselResourceSys
{
  public class VesselResourceSystem : 
    MessageSystem<VesselResourceSystem, VesselResourceMessageSender, VesselResourceMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselResourceQueue> VesselResources { get; } = new ConcurrentDictionary<Guid, VesselResourceQueue>();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselResourceSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(2500, RoutineExecution.Update, new Action(this.SendVesselResources)));
      this.SetupRoutine(new RoutineDefinition(2500, RoutineExecution.Update, new Action(this.ProcessVesselResources)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.VesselResources.Clear();
    }

    private void ProcessVesselResources()
    {
      if (HighLogic.LoadedScene < 5)
        return;
      using (IEnumerator<KeyValuePair<Guid, VesselResourceQueue>> enumerator = this.VesselResources.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselResourceQueue> current = enumerator.Current;
          while (true)
          {
            VesselResource result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessVesselResource();
              current.Value.Recycle(result);
            }
            else
              goto label_7;
          }
        }
      }
    }

    private void SendVesselResources()
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || !FlightGlobals.ActiveVessel.loaded || VesselCommon.IsSpectating)
        return;
      this.MessageSender.SendVesselResources(FlightGlobals.ActiveVessel);
    }

    public void RemoveVessel(Guid vesselId) => this.VesselResources.TryRemove(vesselId, out VesselResourceQueue _);
  }
}
