// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KerbalSys.KerbalSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI;
using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Events;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using UnityEngine;

namespace LmpClient.Systems.KerbalSys
{
  public class KerbalSystem : MessageSystem<KerbalSystem, KerbalMessageSender, KerbalMessageHandler>
  {
    private static AstronautComplex _astronautComplex;
    private static readonly FieldInfo KerbalStatusField = typeof (ProtoCrewMember).GetField("_rosterStatus", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo KerbalTypeField = typeof (ProtoCrewMember).GetField("_type", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo CreateAvailableList = typeof (AstronautComplex).GetMethod(nameof (CreateAvailableList), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo CreateAssignedList = typeof (AstronautComplex).GetMethod(nameof (CreateAssignedList), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo CreateKiaList = typeof (AstronautComplex).GetMethod(nameof (CreateKiaList), BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo InitiateGui = typeof (AstronautComplex).GetMethod("InitiateGUI", BindingFlags.Instance | BindingFlags.NonPublic);

    public ConcurrentQueue<string> KerbalsToRemove { get; private set; } = new ConcurrentQueue<string>();

    public ConcurrentQueue<ConfigNode> KerbalsToProcess { get; private set; } = new ConcurrentQueue<ConfigNode>();

    public bool KerbalSystemReady => this.Enabled && HighLogic.CurrentGame?.CrewRoster != null;

    public KerbalEvents KerbalEvents { get; } = new KerbalEvents();

    public AstronautComplex AstronautComplex
    {
      get
      {
        if (Object.op_Equality((Object) KerbalSystem._astronautComplex, (Object) null))
          KerbalSystem._astronautComplex = Object.FindObjectOfType<AstronautComplex>();
        return KerbalSystem._astronautComplex;
      }
    }

    public override string SystemName { get; } = nameof (KerbalSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.RemoveQueuedKerbals)));
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.LoadKerbals)));
      // ISSUE: method pointer
      VesselAssemblyEvent.onVesselValidationBeforAssembly.Add(new EventData<bool>.OnEvent((object) this.KerbalEvents, __methodptr(ValidationBeforeAssembly)));
      // ISSUE: method pointer
      GameEvents.onKerbalLevelUp.Add(new EventData<ProtoCrewMember>.OnEvent((object) this.KerbalEvents, __methodptr(KerbalLevelUp)));
      // ISSUE: method pointer
      GameEvents.onKerbalStatusChange.Add(new EventData<ProtoCrewMember, ProtoCrewMember.RosterStatus, ProtoCrewMember.RosterStatus>.OnEvent((object) this.KerbalEvents, __methodptr(StatusChange)));
      // ISSUE: method pointer
      GameEvents.onKerbalTypeChange.Add(new EventData<ProtoCrewMember, ProtoCrewMember.KerbalType, ProtoCrewMember.KerbalType>.OnEvent((object) this.KerbalEvents, __methodptr(TypeChange)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.KerbalEvents, __methodptr(ReturningToEditor)));
      // ISSUE: method pointer
      RemoveEvent.onLmpTerminatedVessel.Add(new EventData<ProtoVessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselTerminated)));
      // ISSUE: method pointer
      RemoveEvent.onLmpRecoveredVessel.Add(new EventData<ProtoVessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselRecovered)));
      // ISSUE: method pointer
      RemoveEvent.onLmpDestroyVessel.Add(new EventData<Vessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselWillDestroy)));
      // ISSUE: method pointer
      VesselLoadEvent.onLmpVesselLoaded.Add(new EventData<Vessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselLoaded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.KerbalsToRemove = new ConcurrentQueue<string>();
      this.KerbalsToProcess = new ConcurrentQueue<ConfigNode>();
      // ISSUE: method pointer
      VesselAssemblyEvent.onVesselValidationBeforAssembly.Remove(new EventData<bool>.OnEvent((object) this.KerbalEvents, __methodptr(ValidationBeforeAssembly)));
      // ISSUE: method pointer
      GameEvents.onKerbalStatusChange.Remove(new EventData<ProtoCrewMember, ProtoCrewMember.RosterStatus, ProtoCrewMember.RosterStatus>.OnEvent((object) this.KerbalEvents, __methodptr(StatusChange)));
      // ISSUE: method pointer
      GameEvents.onKerbalTypeChange.Remove(new EventData<ProtoCrewMember, ProtoCrewMember.KerbalType, ProtoCrewMember.KerbalType>.OnEvent((object) this.KerbalEvents, __methodptr(TypeChange)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.KerbalEvents, __methodptr(ReturningToEditor)));
      // ISSUE: method pointer
      RemoveEvent.onLmpTerminatedVessel.Remove(new EventData<ProtoVessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselTerminated)));
      // ISSUE: method pointer
      RemoveEvent.onLmpRecoveredVessel.Remove(new EventData<ProtoVessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselRecovered)));
      // ISSUE: method pointer
      RemoveEvent.onLmpDestroyVessel.Remove(new EventData<Vessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselWillDestroy)));
      // ISSUE: method pointer
      VesselLoadEvent.onLmpVesselLoaded.Remove(new EventData<Vessel>.OnEvent((object) this.KerbalEvents, __methodptr(OnVesselLoaded)));
    }

    public void LoadKerbalsIntoGame() => this.ProcessKerbalQueue();

    public void SetKerbalStatusWithoutTriggeringEvent(
      ProtoCrewMember crew,
      ProtoCrewMember.RosterStatus newStatus)
    {
      if (crew == null)
        return;
      KerbalSystem.KerbalStatusField?.SetValue((object) crew, (object) newStatus);
    }

    public void SetKerbalTypeWithoutTriggeringEvent(
      ProtoCrewMember crew,
      ProtoCrewMember.KerbalType newType)
    {
      if (crew == null)
        return;
      KerbalSystem.KerbalTypeField?.SetValue((object) crew, (object) newType);
    }

    private void RemoveQueuedKerbals()
    {
      if (!this.KerbalSystemReady)
        return;
      bool flag = false;
      string result;
      while (this.KerbalsToRemove.TryDequeue(out result))
      {
        HighLogic.CurrentGame.CrewRoster.Remove(result);
        flag = true;
      }
      if (flag)
        this.RefreshCrewDialog();
    }

    private void LoadKerbals()
    {
      if (!this.KerbalSystemReady || HighLogic.LoadedScene < 5)
        return;
      this.ProcessKerbalQueue();
    }

    private void ProcessKerbalQueue()
    {
      bool flag = false;
      ConfigNode result;
      while (this.KerbalsToProcess.TryDequeue(out result))
      {
        this.LoadKerbal(result);
        flag = true;
      }
      if (!flag)
        return;
      this.RefreshCrewDialog();
    }

    public void RefreshCrewDialog()
    {
      if (Object.op_Inequality((Object) CrewAssignmentDialog.Instance, (Object) null))
      {
        ((BaseCrewAssignmentDialog) CrewAssignmentDialog.Instance).RefreshCrewLists(((BaseCrewAssignmentDialog) CrewAssignmentDialog.Instance).GetManifest(true), false, true, (Func<PartCrewManifest, bool>) null);
        ((BaseCrewAssignmentDialog) CrewAssignmentDialog.Instance).ButtonClear();
        ((BaseCrewAssignmentDialog) CrewAssignmentDialog.Instance).ButtonFill();
      }
      if (!Object.op_Inequality((Object) this.AstronautComplex, (Object) null))
        return;
      KerbalSystem.InitiateGui.Invoke((object) this.AstronautComplex, (object[]) null);
      KerbalSystem.CreateAvailableList.Invoke((object) this.AstronautComplex, (object[]) null);
      KerbalSystem.CreateAssignedList.Invoke((object) this.AstronautComplex, (object[]) null);
      KerbalSystem.CreateKiaList.Invoke((object) this.AstronautComplex, (object[]) null);
    }

    private void LoadKerbal(ConfigNode crewNode)
    {
      ProtoCrewMember protoCrew = new ProtoCrewMember(HighLogic.CurrentGame.Mode, crewNode, (ProtoCrewMember.KerbalType) 0);
      if (string.IsNullOrEmpty(protoCrew.name))
        LunaLog.LogError("[LMP]: protoName is blank!");
      else if (!HighLogic.CurrentGame.CrewRoster.Exists(protoCrew.name))
        HighLogic.CurrentGame.CrewRoster.AddCrewMember(protoCrew);
      else
        this.UpdateKerbalData(crewNode, protoCrew);
    }

    private void UpdateKerbalData(ConfigNode crewNode, ProtoCrewMember protoCrew)
    {
      ConfigNode node1 = crewNode.GetNode("CAREER_LOG");
      if (node1 != null)
      {
        HighLogic.CurrentGame.CrewRoster[protoCrew.name].careerLog.Entries.Clear();
        HighLogic.CurrentGame.CrewRoster[protoCrew.name].careerLog.Load(node1);
      }
      else
        LunaLog.Log("[LMP]: Career log node for " + protoCrew.name + " is empty!");
      ConfigNode node2 = crewNode.GetNode("FLIGHT_LOG");
      if (node2 != null)
      {
        HighLogic.CurrentGame.CrewRoster[protoCrew.name].flightLog.Entries.Clear();
        HighLogic.CurrentGame.CrewRoster[protoCrew.name].flightLog.Load(node2);
      }
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].courage = protoCrew.courage;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].experience = protoCrew.experience;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].experienceLevel = protoCrew.experienceLevel;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].experienceTrait = protoCrew.experienceTrait;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].gExperienced = protoCrew.gExperienced;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].gIncrement = protoCrew.gIncrement;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].geeForce = protoCrew.geeForce;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].gender = protoCrew.gender;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].hasToured = protoCrew.hasToured;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].isBadass = protoCrew.isBadass;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].inactiveTimeEnd = protoCrew.inactiveTimeEnd;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].outDueToG = protoCrew.outDueToG;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].seat = protoCrew.seat;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].seatIdx = protoCrew.seatIdx;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].stupidity = protoCrew.stupidity;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].trait = protoCrew.trait;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].UTaR = protoCrew.UTaR;
      HighLogic.CurrentGame.CrewRoster[protoCrew.name].veteran = protoCrew.veteran;
      this.SetKerbalTypeWithoutTriggeringEvent(HighLogic.CurrentGame.CrewRoster[protoCrew.name], protoCrew.type);
      this.SetKerbalStatusWithoutTriggeringEvent(HighLogic.CurrentGame.CrewRoster[protoCrew.name], protoCrew.rosterStatus);
    }
  }
}
