// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselRemoveSys.VesselRemoveSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Localization;
using LmpClient.Utilities;
using LmpClient.VesselUtilities;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.VesselRemoveSys
{
  public class VesselRemoveSystem : 
    MessageSystem<VesselRemoveSystem, VesselRemoveMessageSender, VesselRemoveMessageHandler>
  {
    private const int MaxTimeToKeepVesselsInRemoveListMs = 2500;

    private VesselRemoveEvents VesselRemoveEvents { get; } = new VesselRemoveEvents();

    public ConcurrentDictionary<Guid, DateTime> RemovedVessels { get; } = new ConcurrentDictionary<Guid, DateTime>();

    public override string SystemName { get; } = nameof (VesselRemoveSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.OnVesselRecoveryRequested.Add(new EventData<Vessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselRecovering)));
      // ISSUE: method pointer
      GameEvents.onVesselRecovered.Add(new EventData<ProtoVessel, bool>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselRecovered)));
      // ISSUE: method pointer
      GameEvents.onVesselTerminated.Add(new EventData<ProtoVessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselTerminated)));
      // ISSUE: method pointer
      GameEvents.onVesselWillDestroy.Add(new EventData<Vessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselWillDestroy)));
      // ISSUE: method pointer
      RevertEvent.onRevertedToLaunch.Add(new EventVoid.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnRevertToLaunch)));
      // ISSUE: method pointer
      RevertEvent.onReturnedToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnRevertToEditor)));
      this.SetupRoutine(new RoutineDefinition(2500, RoutineExecution.Update, new Action(this.FlushRemovedVessels)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.ClearSystem();
      // ISSUE: method pointer
      GameEvents.OnVesselRecoveryRequested.Remove(new EventData<Vessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselRecovering)));
      // ISSUE: method pointer
      GameEvents.onVesselRecovered.Remove(new EventData<ProtoVessel, bool>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselRecovered)));
      // ISSUE: method pointer
      GameEvents.onVesselTerminated.Remove(new EventData<ProtoVessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselTerminated)));
      // ISSUE: method pointer
      GameEvents.onVesselWillDestroy.Remove(new EventData<Vessel>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnVesselWillDestroy)));
      // ISSUE: method pointer
      RevertEvent.onRevertedToLaunch.Remove(new EventVoid.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnRevertToLaunch)));
      // ISSUE: method pointer
      RevertEvent.onReturnedToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.VesselRemoveEvents, __methodptr(OnRevertToEditor)));
    }

    public void ClearSystem() => this.RemovedVessels.Clear();

    public bool VesselWillBeKilled(Guid vesselId) => this.RemovedVessels.ContainsKey(vesselId);

    public void DelayedKillVessel(
      Guid vesselId,
      bool addToKilledList,
      string reason,
      int delayInMs)
    {
      CoroutineUtil.StartDelayedRoutine(nameof (DelayedKillVessel), (Action) (() =>
      {
        LunaLog.Log(string.Format("Delayed attempt to kill vessel {0}", (object) vesselId));
        this.KillVessel(vesselId, addToKilledList, reason);
      }), (float) TimeSpan.FromMilliseconds((double) delayInMs).TotalSeconds);
    }

    public void KillVessel(Guid vesselId, bool addToKilledList, string reason)
    {
      VesselCommon.RemoveVesselFromSystems(vesselId);
      VesselRemoveEvent.onLmpVesselRemoved.Fire(vesselId);
      if (addToKilledList)
        this.RemovedVessels.TryAdd(vesselId, LunaNetworkTime.UtcNow);
      VesselRemoveSystem.KillVessel(FlightGlobals.FindVessel(vesselId), reason);
    }

    private static void KillVessel(Vessel killVessel, string reason)
    {
      if (Object.op_Equality((Object) killVessel, (Object) null) || killVessel.state == 2)
        return;
      LunaLog.Log(string.Format("[LMP]: Killing vessel {0}. Reason: {1}", (object) killVessel.id, (object) reason));
      VesselRemoveSystem.SwitchVesselIfKillingActiveVessel(killVessel);
      try
      {
        Guid? id1 = FlightGlobals.fetch.VesselTarget?.GetVessel().id;
        Guid id2 = killVessel.id;
        if (id1.HasValue && (!id1.HasValue || id1.GetValueOrDefault() == id2))
          FlightGlobals.fetch.SetVesselTarget((ITargetable) null, false);
        FlightGlobals.RemoveVessel(killVessel);
        foreach (Component part in killVessel.parts)
          Object.Destroy((Object) part.gameObject);
        Object.Destroy((Object) ((Component) killVessel).gameObject);
        HighLogic.CurrentGame.flightState.protoVessels.RemoveAll((Predicate<ProtoVessel>) (v => v == null || v.vesselID == killVessel.id));
        if (!Object.op_Implicit((Object) KSCVesselMarkers.fetch))
          return;
        KSCVesselMarkers.fetch.RefreshMarkers();
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error destroying vessel: {0}", (object) ex));
      }
    }

    private void FlushRemovedVessels()
    {
      foreach (Guid key in Enumerable.Select<KeyValuePair<Guid, DateTime>, Guid>(Enumerable.Where<KeyValuePair<Guid, DateTime>>((IEnumerable<KeyValuePair<Guid, DateTime>>) this.RemovedVessels, (Func<KeyValuePair<Guid, DateTime>, bool>) (v => LunaNetworkTime.UtcNow - v.Value > TimeSpan.FromMilliseconds(2500.0))), (Func<KeyValuePair<Guid, DateTime>, Guid>) (v => v.Key)))
        this.RemovedVessels.TryRemove(key, out DateTime _);
    }

    private static void SwitchVesselIfKillingActiveVessel(Vessel killVessel)
    {
      if (!Object.op_Implicit((Object) FlightGlobals.ActiveVessel) || !(FlightGlobals.ActiveVessel.id == killVessel.id))
        return;
      FlightGlobals.fetch.SetVesselTarget((ITargetable) null, false);
      Vessel controllableVessel = FlightGlobals.FindNearestControllableVessel(killVessel);
      if (Object.op_Inequality((Object) controllableVessel, (Object) null))
        FlightGlobals.ForceSetActiveVessel(controllableVessel);
      else
        HighLogic.LoadScene((GameScenes) 5);
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.SpectatingRemoved, 10f, (ScreenMessageStyle) 0);
    }
  }
}
