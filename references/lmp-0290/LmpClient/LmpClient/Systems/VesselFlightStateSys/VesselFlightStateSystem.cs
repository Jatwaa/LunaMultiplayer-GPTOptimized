// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.VesselFlightStateSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpClient.VesselUtilities;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class VesselFlightStateSystem : 
    MessageSystem<VesselFlightStateSystem, VesselFlightStateMessageSender, VesselFlightStateMessageHandler>
  {
    private static DateTime LastVesselFlightStateSentTime { get; set; } = LunaComputerTime.UtcNow;

    private static bool TimeToSendFlightStateUpdate => !VesselCommon.PlayerVesselsNearby() ? (LunaComputerTime.UtcNow - VesselFlightStateSystem.LastVesselFlightStateSentTime).TotalMilliseconds > (double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval : (LunaComputerTime.UtcNow - VesselFlightStateSystem.LastVesselFlightStateSentTime).TotalMilliseconds > (double) SettingsSystem.ServerSettings.VesselUpdatesMsInterval;

    public bool FlightStateSystemReady => this.Enabled && Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && HighLogic.LoadedScene == 7 && FlightGlobals.ready && FlightGlobals.ActiveVessel.loaded && FlightGlobals.ActiveVessel.state != 2 && FlightGlobals.ActiveVessel.vesselType != 12;

    public FlightStateEvents FlightStateEvents { get; } = new FlightStateEvents();

    public ConcurrentDictionary<Guid, FlightInputCallback> FlyByWireDictionary { get; } = new ConcurrentDictionary<Guid, FlightInputCallback>();

    public static ConcurrentDictionary<Guid, VesselFlightStateUpdate> CurrentFlightState { get; } = new ConcurrentDictionary<Guid, VesselFlightStateUpdate>();

    public static ConcurrentDictionary<Guid, FlightStateQueue> TargetFlightStateQueue { get; } = new ConcurrentDictionary<Guid, FlightStateQueue>();

    public override string SystemName { get; } = nameof (VesselFlightStateSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onVesselGoOnRails.Add(new EventData<Vessel>.OnEvent((object) this.FlightStateEvents, __methodptr(OnVesselPack)));
      // ISSUE: method pointer
      GameEvents.onVesselGoOffRails.Add(new EventData<Vessel>.OnEvent((object) this.FlightStateEvents, __methodptr(OnVesselUnpack)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Add(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(OnStartSpectating)));
      // ISSUE: method pointer
      SpectateEvent.onFinishedSpectating.Add(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(OnFinishedSpectating)));
      // ISSUE: method pointer
      TimingManager.LateUpdateAdd((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(SendFlightState)));
      // ISSUE: method pointer
      WarpEvent.onTimeWarpStopped.Add(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(WarpStopped)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onVesselGoOnRails.Remove(new EventData<Vessel>.OnEvent((object) this.FlightStateEvents, __methodptr(OnVesselPack)));
      // ISSUE: method pointer
      GameEvents.onVesselGoOffRails.Remove(new EventData<Vessel>.OnEvent((object) this.FlightStateEvents, __methodptr(OnVesselUnpack)));
      // ISSUE: method pointer
      SpectateEvent.onStartSpectating.Remove(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(OnStartSpectating)));
      // ISSUE: method pointer
      SpectateEvent.onFinishedSpectating.Remove(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(OnFinishedSpectating)));
      // ISSUE: method pointer
      TimingManager.LateUpdateRemove((TimingManager.TimingStage) 8, new TimingManager.UpdateAction((object) this, __methodptr(SendFlightState)));
      // ISSUE: method pointer
      WarpEvent.onTimeWarpStopped.Remove(new EventVoid.OnEvent((object) this.FlightStateEvents, __methodptr(WarpStopped)));
      this.ClearSystem();
    }

    public void ClearSystem()
    {
      foreach (KeyValuePair<Guid, FlightInputCallback> flyByWire in this.FlyByWireDictionary)
      {
        Vessel vessel1 = FlightGlobals.FindVessel(flyByWire.Key);
        if (Object.op_Inequality((Object) vessel1, (Object) null))
        {
          try
          {
            Vessel vessel2 = vessel1;
            vessel2.OnFlyByWire = (FlightInputCallback) Delegate.Remove((Delegate) vessel2.OnFlyByWire, (Delegate) this.FlyByWireDictionary[flyByWire.Key]);
          }
          catch (Exception ex)
          {
          }
        }
      }
      this.FlyByWireDictionary.Clear();
      VesselFlightStateSystem.CurrentFlightState.Clear();
      VesselFlightStateSystem.TargetFlightStateQueue.Clear();
    }

    private void SendFlightState()
    {
      Profiler.BeginSample(nameof (SendFlightState));
      if (this.FlightStateSystemReady && VesselFlightStateSystem.TimeToSendFlightStateUpdate && !VesselCommon.IsSpectating && !FlightGlobals.ActiveVessel.isEVA)
      {
        this.MessageSender.SendCurrentFlightState();
        VesselFlightStateSystem.LastVesselFlightStateSentTime = LunaComputerTime.UtcNow;
      }
      Profiler.EndSample();
    }

    public void AddVesselToSystem(Vessel vessel)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      VesselFlightStateSystem.\u003C\u003Ec__DisplayClass29_0 cDisplayClass290 = new VesselFlightStateSystem.\u003C\u003Ec__DisplayClass29_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass290.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass290.vessel = vessel;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (Object.op_Equality((Object) cDisplayClass290.vessel, (Object) null) || cDisplayClass290.vessel.isEVA || !VesselCommon.IsSpectating && Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) cDisplayClass290.vessel))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      this.FlyByWireDictionary.TryAdd(cDisplayClass290.vessel.id, new FlightInputCallback((object) cDisplayClass290, __methodptr(\u003CAddVesselToSystem\u003Eb__0)));
      // ISSUE: reference to a compiler-generated field
      if (!((IEnumerable<Delegate>) ((Delegate) cDisplayClass290.vessel.OnFlyByWire).GetInvocationList()).All<Delegate>((Func<Delegate, bool>) (d => d.Method.Name != "LunaOnVesselFlyByWire")))
        return;
      // ISSUE: reference to a compiler-generated field
      Vessel vessel1 = cDisplayClass290.vessel;
      // ISSUE: reference to a compiler-generated field
      vessel1.OnFlyByWire = (FlightInputCallback) Delegate.Combine((Delegate) vessel1.OnFlyByWire, (Delegate) this.FlyByWireDictionary[cDisplayClass290.vessel.id]);
    }

    public void RemoveVessel(Guid vesselId)
    {
      this.FlyByWireDictionary.TryRemove(vesselId, out FlightInputCallback _);
      VesselFlightStateSystem.CurrentFlightState.TryRemove(vesselId, out VesselFlightStateUpdate _);
      VesselFlightStateSystem.TargetFlightStateQueue.TryRemove(vesselId, out FlightStateQueue _);
      Vessel vessel = FlightGlobals.FindVessel(vesselId);
      if (!Object.op_Inequality((Object) vessel, (Object) null))
        return;
      this.TryRemoveCallback(vessel);
    }

    public void RemoveVessel(Vessel vesselToRemove)
    {
      if (Object.op_Equality((Object) vesselToRemove, (Object) null))
        return;
      this.TryRemoveCallback(vesselToRemove);
      this.FlyByWireDictionary.TryRemove(vesselToRemove.id, out FlightInputCallback _);
      VesselFlightStateSystem.CurrentFlightState.TryRemove(vesselToRemove.id, out VesselFlightStateUpdate _);
      VesselFlightStateSystem.TargetFlightStateQueue.TryRemove(vesselToRemove.id, out FlightStateQueue _);
    }

    public void AdjustExtraInterpolationTimes()
    {
      foreach (KeyValuePair<Guid, VesselFlightStateUpdate> keyValuePair in VesselFlightStateSystem.CurrentFlightState)
        keyValuePair.Value.AdjustExtraInterpolationTimes();
      using (IEnumerator<KeyValuePair<Guid, FlightStateQueue>> enumerator = VesselFlightStateSystem.TargetFlightStateQueue.GetEnumerator())
      {
label_12:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, FlightStateQueue> current = enumerator.Current;
          while (true)
          {
            VesselFlightStateUpdate result;
            if (current.Value.TryPeek(out result) && VesselFlightStateSystem.FlightStateUpdateIsTooOld(result))
              current.Value.TryDequeue(out VesselFlightStateUpdate _);
            else
              goto label_12;
          }
        }
      }
    }

    private static bool FlightStateUpdateIsTooOld(VesselFlightStateUpdate update)
    {
      double num = LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsEqualOrInThePast(update.SubspaceId) ? TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.VesselUpdatesMsInterval).TotalSeconds * 2.0 : double.MaxValue;
      return update.GameTimeStamp < TimeSyncSystem.UniversalTime - num;
    }

    private void LunaOnVesselFlyByWire(Guid id, FlightCtrlState st)
    {
      VesselFlightStateUpdate flightStateUpdate;
      if (!this.Enabled || !this.FlightStateSystemReady || !VesselFlightStateSystem.CurrentFlightState.TryGetValue(id, out flightStateUpdate))
        return;
      if (VesselCommon.IsSpectating)
        st.CopyFrom(flightStateUpdate.GetInterpolatedValue());
      else if (Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.situation > 8)
      {
        FlightCtrlState interpolatedValue = flightStateUpdate.GetInterpolatedValue();
        st.mainThrottle = interpolatedValue.mainThrottle;
        st.gearDown = interpolatedValue.gearDown;
        st.gearUp = interpolatedValue.gearUp;
        st.headlight = interpolatedValue.headlight;
        st.killRot = interpolatedValue.killRot;
      }
      else
        st.CopyFrom(flightStateUpdate.GetInterpolatedValue());
    }

    private void TryRemoveCallback(Vessel vesselToRemove)
    {
      if (!this.FlyByWireDictionary.ContainsKey(vesselToRemove.id) || !((IEnumerable<Delegate>) ((Delegate) vesselToRemove.OnFlyByWire).GetInvocationList()).All<Delegate>((Func<Delegate, bool>) (d => d.Method.Name != "LunaOnVesselFlyByWire")))
        return;
      Vessel vessel = vesselToRemove;
      vessel.OnFlyByWire = (FlightInputCallback) Delegate.Remove((Delegate) vessel.OnFlyByWire, (Delegate) this.FlyByWireDictionary[vesselToRemove.id]);
    }
  }
}
