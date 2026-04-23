// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProtoSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Utilities;
using LmpClient.VesselUtilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProtoSystem : 
    MessageSystem<VesselProtoSystem, VesselProtoMessageSender, VesselProtoMessageHandler>
  {
    private static readonly HashSet<Guid> QueuedVesselsToSend = new HashSet<Guid>();
    public readonly HashSet<Guid> VesselsUnableToLoad = new HashSet<Guid>();

    public ConcurrentDictionary<Guid, VesselProtoQueue> VesselProtos { get; } = new ConcurrentDictionary<Guid, VesselProtoQueue>();

    public bool ProtoSystemReady => this.Enabled && FlightGlobals.ready && HighLogic.LoadedScene == 7 && Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && !VesselCommon.IsSpectating;

    public VesselProtoEvents VesselProtoEvents { get; } = new VesselProtoEvents();

    public VesselRemoveSystem VesselRemoveSystem => LmpClient.Base.System<VesselRemoveSystem>.Singleton;

    public override string SystemName { get; } = nameof (VesselProtoSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onFlightReady.Add(new EventVoid.OnEvent((object) this.VesselProtoEvents, __methodptr(FlightReady)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Add(new EventData<GameScenes>.OnEvent((object) this.VesselProtoEvents, __methodptr(OnSceneRequested)));
      // ISSUE: method pointer
      GameEvents.OnTriggeredDataTransmission.Add(new EventData<ScienceData, Vessel, bool>.OnEvent((object) this.VesselProtoEvents, __methodptr(TriggeredDataTransmission)));
      // ISSUE: method pointer
      GameEvents.OnExperimentStored.Add(new EventData<ScienceData>.OnEvent((object) this.VesselProtoEvents, __methodptr(ExperimentStored)));
      // ISSUE: method pointer
      ExperimentEvent.onExperimentReset.Add(new EventData<Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(ExperimentReset)));
      // ISSUE: method pointer
      PartEvent.onPartDecoupled.Add(new EventData<Part, float, Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartDecoupled)));
      // ISSUE: method pointer
      PartEvent.onPartUndocked.Add(new EventData<Part, DockedVesselInfo, Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartUndocked)));
      // ISSUE: method pointer
      PartEvent.onPartCoupled.Add(new EventData<Part, Part, Guid>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartCoupled)));
      // ISSUE: method pointer
      WarpEvent.onTimeWarpStopped.Add(new EventVoid.OnEvent((object) this.VesselProtoEvents, __methodptr(WarpStopped)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.CheckVesselsToLoad)));
      this.SetupRoutine(new RoutineDefinition(2500, RoutineExecution.Update, new Action(this.SendVesselDefinition)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onFlightReady.Remove(new EventVoid.OnEvent((object) this.VesselProtoEvents, __methodptr(FlightReady)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Remove(new EventData<GameScenes>.OnEvent((object) this.VesselProtoEvents, __methodptr(OnSceneRequested)));
      // ISSUE: method pointer
      GameEvents.OnTriggeredDataTransmission.Remove(new EventData<ScienceData, Vessel, bool>.OnEvent((object) this.VesselProtoEvents, __methodptr(TriggeredDataTransmission)));
      // ISSUE: method pointer
      GameEvents.OnExperimentStored.Remove(new EventData<ScienceData>.OnEvent((object) this.VesselProtoEvents, __methodptr(ExperimentStored)));
      // ISSUE: method pointer
      ExperimentEvent.onExperimentReset.Remove(new EventData<Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(ExperimentReset)));
      // ISSUE: method pointer
      PartEvent.onPartDecoupled.Remove(new EventData<Part, float, Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartDecoupled)));
      // ISSUE: method pointer
      PartEvent.onPartUndocked.Remove(new EventData<Part, DockedVesselInfo, Vessel>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartUndocked)));
      // ISSUE: method pointer
      PartEvent.onPartCoupled.Remove(new EventData<Part, Part, Guid>.OnEvent((object) this.VesselProtoEvents, __methodptr(PartCoupled)));
      // ISSUE: method pointer
      WarpEvent.onTimeWarpStopped.Remove(new EventVoid.OnEvent((object) this.VesselProtoEvents, __methodptr(WarpStopped)));
      this.VesselProtos.Clear();
      this.VesselsUnableToLoad.Clear();
      VesselProtoSystem.QueuedVesselsToSend.Clear();
    }

    private void SendVesselDefinition()
    {
      try
      {
        if (!this.ProtoSystemReady)
          return;
        if (FlightGlobals.ActiveVessel.parts.Count != FlightGlobals.ActiveVessel.protoVessel.protoPartSnapshots.Count)
          this.MessageSender.SendVesselMessage(FlightGlobals.ActiveVessel);
        foreach (Vessel secondaryVessel in VesselCommon.GetSecondaryVessels())
        {
          if (secondaryVessel.parts.Count != secondaryVessel.protoVessel.protoPartSnapshots.Count)
            this.MessageSender.SendVesselMessage(secondaryVessel);
        }
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error in SendVesselDefinition {0}", (object) ex));
      }
    }

    public void CheckVesselsToLoad()
    {
      if (HighLogic.LoadedScene < 5)
        return;
      try
      {
        foreach (KeyValuePair<Guid, VesselProtoQueue> vesselProto in this.VesselProtos)
        {
          VesselProto result;
          if (vesselProto.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
          {
            vesselProto.Value.TryDequeue(out VesselProto _);
            if (!this.VesselRemoveSystem.VesselWillBeKilled(result.VesselId))
            {
              bool forceReload = result.ForceReload;
              ProtoVessel protoVessel = result.CreateProtoVessel();
              vesselProto.Value.Recycle(result);
              if (protoVessel == null || protoVessel.HasInvalidParts(!this.VesselsUnableToLoad.Contains(result.VesselId)))
              {
                this.VesselsUnableToLoad.Add(result.VesselId);
              }
              else
              {
                this.VesselsUnableToLoad.Remove(result.VesselId);
                if (Object.op_Equality((Object) FlightGlobals.FindVessel(result.VesselId), (Object) null))
                {
                  if (VesselLoader.LoadVessel(protoVessel, forceReload))
                  {
                    LunaLog.Log(string.Format("[LMP]: Vessel {0} loaded", (object) protoVessel.vesselID));
                    VesselLoadEvent.onLmpVesselLoaded.Fire(protoVessel.vesselRef);
                  }
                }
                else if (VesselLoader.LoadVessel(protoVessel, forceReload))
                {
                  LunaLog.Log(string.Format("[LMP]: Vessel {0} reloaded", (object) protoVessel.vesselID));
                  VesselReloadEvent.onLmpVesselReloaded.Fire(protoVessel.vesselRef);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error in CheckVesselsToLoad {0}", (object) ex));
      }
    }

    public void DelayedSendVesselMessage(Guid vesselId, float delayInSec, bool forceReload = false)
    {
      if (VesselProtoSystem.QueuedVesselsToSend.Contains(vesselId))
        return;
      VesselProtoSystem.QueuedVesselsToSend.Add(vesselId);
      CoroutineUtil.StartDelayedRoutine("QueueVesselMessageAsPartsChanged", (Action) (() =>
      {
        VesselProtoSystem.QueuedVesselsToSend.Remove(vesselId);
        LunaLog.Log(string.Format("[LMP]: Sending delayed proto vessel {0}", (object) vesselId));
        this.MessageSender.SendVesselMessage(FlightGlobals.FindVessel(vesselId));
      }), delayInSec);
    }

    public void RemoveVessel(Guid vesselId) => this.VesselProtos.TryRemove(vesselId, out VesselProtoQueue _);
  }
}
