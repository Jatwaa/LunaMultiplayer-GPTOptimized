// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.VesselPositionSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.VesselUtilities;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace LmpClient.Systems.VesselPositionSys
{
  public class VesselPositionSystem : 
    MessageSystem<VesselPositionSystem, VesselPositionMessageSender, VesselPositionMessageHandler>
  {
    private const TimingManager.TimingStage HandlePositionsStage = (TimingManager.TimingStage) 8;
    private const TimingManager.TimingStage SendPositionsStage = (TimingManager.TimingStage) 8;

    private static DateTime LastVesselUpdatesSentTime { get; set; } = LunaComputerTime.UtcNow;

    private static int UpdateIntervalLockedToUnity => (int) (Math.Floor((double) SettingsSystem.ServerSettings.VesselUpdatesMsInterval / TimeSpan.FromSeconds((double) UnityEngine.Time.fixedDeltaTime).TotalMilliseconds) * TimeSpan.FromSeconds((double) UnityEngine.Time.fixedDeltaTime).TotalMilliseconds);

    private static int SecondaryVesselUpdatesUpdateIntervalLockedToUnity => (int) (Math.Floor((double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval / TimeSpan.FromSeconds((double) UnityEngine.Time.fixedDeltaTime).TotalMilliseconds) * TimeSpan.FromSeconds((double) UnityEngine.Time.fixedDeltaTime).TotalMilliseconds);

    private static bool TimeToSendVesselUpdate => !VesselCommon.PlayerVesselsNearby() ? (LunaComputerTime.UtcNow - VesselPositionSystem.LastVesselUpdatesSentTime).TotalMilliseconds > (double) VesselPositionSystem.SecondaryVesselUpdatesUpdateIntervalLockedToUnity : (LunaComputerTime.UtcNow - VesselPositionSystem.LastVesselUpdatesSentTime).TotalMilliseconds > (double) VesselPositionSystem.UpdateIntervalLockedToUnity;

    public bool PositionUpdateSystemReady => this.Enabled && Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && FlightGlobals.ready && FlightGlobals.ActiveVessel.loaded && FlightGlobals.ActiveVessel.state != 2 && FlightGlobals.ActiveVessel.vesselType != 12;

    public PositionEvents PositionEvents { get; } = new PositionEvents();

    public bool PositionUpdateSystemBasicReady => this.Enabled && this.PositionUpdateSystemReady || HighLogic.LoadedScene == 8;

    public static ConcurrentDictionary<Guid, VesselPositionUpdate> CurrentVesselUpdate { get; } = new ConcurrentDictionary<Guid, VesselPositionUpdate>();

    public static ConcurrentDictionary<Guid, PositionUpdateQueue> TargetVesselUpdateQueue { get; } = new ConcurrentDictionary<Guid, PositionUpdateQueue>();

    private List<Vessel> SecondaryVesselsToUpdate { get; } = new List<Vessel>();

    private List<Vessel> AbandonedVesselsToUpdate { get; } = new List<Vessel>();

    public override string SystemName { get; } = nameof (VesselPositionSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      TimingManager.FixedUpdateAdd((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(HandleVesselUpdates)));
      // ISSUE: method pointer
      TimingManager.LateUpdateAdd((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(SendVesselPositionUpdates)));
      this.SetupRoutine(new RoutineDefinition(SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval, RoutineExecution.LateUpdate, new Action(this.SendSecondaryVesselPositionUpdates)));
      // ISSUE: method pointer
      WarpEvent.onTimeWarpStopped.Add(new EventVoid.OnEvent((object) this.PositionEvents, __methodptr(WarpStopped)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      TimingManager.FixedUpdateRemove((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(HandleVesselUpdates)));
      // ISSUE: method pointer
      TimingManager.LateUpdateRemove((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(SendVesselPositionUpdates)));
      VesselPositionSystem.CurrentVesselUpdate.Clear();
      VesselPositionSystem.TargetVesselUpdateQueue.Clear();
    }

    private void HandleVesselUpdates()
    {
      Profiler.BeginSample(nameof (HandleVesselUpdates));
      foreach (KeyValuePair<Guid, VesselPositionUpdate> keyValuePair in VesselPositionSystem.CurrentVesselUpdate)
        keyValuePair.Value.ApplyInterpolatedVesselUpdate();
      Profiler.EndSample();
    }

    private void SendVesselPositionUpdates()
    {
      Profiler.BeginSample(nameof (SendVesselPositionUpdates));
      if (this.PositionUpdateSystemReady && VesselPositionSystem.TimeToSendVesselUpdate && !VesselCommon.IsSpectating)
      {
        this.MessageSender.SendVesselPositionUpdate(FlightGlobals.ActiveVessel);
        VesselPositionSystem.LastVesselUpdatesSentTime = LunaComputerTime.UtcNow;
      }
      Profiler.EndSample();
    }

    private void SendSecondaryVesselPositionUpdates()
    {
      if (!this.PositionUpdateSystemReady || VesselCommon.IsSpectating)
        return;
      this.SecondaryVesselsToUpdate.Clear();
      this.SecondaryVesselsToUpdate.AddRange(VesselCommon.GetSecondaryVessels());
      for (int index = 0; index < this.SecondaryVesselsToUpdate.Count; ++index)
      {
        if (!this.VesselHavePositionUpdatesQueued(this.SecondaryVesselsToUpdate[index].id))
          this.MessageSender.SendVesselPositionUpdate(this.SecondaryVesselsToUpdate[index]);
      }
    }

    private void SendUnloadedSecondaryVesselPositionUpdates()
    {
      if (!this.PositionUpdateSystemBasicReady || VesselCommon.IsSpectating)
        return;
      this.AbandonedVesselsToUpdate.Clear();
      this.AbandonedVesselsToUpdate.AddRange(VesselCommon.GetUnloadedSecondaryVessels());
      for (int index = 0; index < this.AbandonedVesselsToUpdate.Count; ++index)
      {
        if (!this.VesselHavePositionUpdatesQueued(this.AbandonedVesselsToUpdate[index].id))
        {
          VesselPositionSystem.UpdateUnloadedVesselValues(this.AbandonedVesselsToUpdate[index]);
          this.MessageSender.SendVesselPositionUpdate(this.AbandonedVesselsToUpdate[index]);
        }
      }
    }

    public void ForceUpdateVesselPosition(Guid vesselId)
    {
      VesselPositionUpdate vesselPositionUpdate;
      if (!VesselPositionSystem.CurrentVesselUpdate.TryGetValue(vesselId, out vesselPositionUpdate))
        return;
      vesselPositionUpdate.UpdateVesselWithPositionData();
    }

    public bool VesselHavePositionUpdatesQueued(Guid vesselId)
    {
      PositionUpdateQueue positionUpdateQueue;
      return VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(vesselId, out positionUpdateQueue) && positionUpdateQueue.Count > 0;
    }

    public void RemoveVessel(Guid vesselId)
    {
      VesselPositionSystem.CurrentVesselUpdate.TryRemove(vesselId, out VesselPositionUpdate _);
      VesselPositionSystem.TargetVesselUpdateQueue.TryRemove(vesselId, out PositionUpdateQueue _);
    }

    public void AdjustExtraInterpolationTimes()
    {
      foreach (KeyValuePair<Guid, VesselPositionUpdate> keyValuePair in VesselPositionSystem.CurrentVesselUpdate)
        keyValuePair.Value.Target = (VesselPositionUpdate) null;
      using (IEnumerator<KeyValuePair<Guid, PositionUpdateQueue>> enumerator = VesselPositionSystem.TargetVesselUpdateQueue.GetEnumerator())
      {
label_12:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, PositionUpdateQueue> current = enumerator.Current;
          while (true)
          {
            VesselPositionUpdate result;
            if (current.Value.TryPeek(out result) && VesselPositionSystem.PositionUpdateIsTooOld(result))
              current.Value.TryDequeue(out VesselPositionUpdate _);
            else
              goto label_12;
          }
        }
      }
    }

    private static bool PositionUpdateIsTooOld(VesselPositionUpdate update) => update.GameTimeStamp < TimeSyncSystem.UniversalTime - (double) VesselCommon.PositionAndFlightStateMessageOffsetSec(update.PingSec);

    private static void UpdateUnloadedVesselValues(Vessel vessel)
    {
      if (vessel.orbit == null)
        return;
      vessel.UpdatePosVel();
      if (!vessel.LandedOrSplashed)
      {
        if (Object.op_Implicit((Object) vessel.orbitDriver))
          vessel.orbitDriver.UpdateOrbit(true);
        vessel.mainBody.GetLatLonAltOrbital(vessel.orbit.pos, ref vessel.latitude, ref vessel.longitude, ref vessel.altitude);
      }
    }
  }
}
