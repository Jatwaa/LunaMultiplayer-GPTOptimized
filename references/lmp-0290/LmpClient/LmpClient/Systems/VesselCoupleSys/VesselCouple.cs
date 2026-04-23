// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCoupleSys.VesselCouple
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Extensions;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Vessel;
using System;
using System.Reflection;
using UnityEngine;

namespace LmpClient.Systems.VesselCoupleSys
{
  public class VesselCouple
  {
    private static readonly MethodInfo GrappleMethod = typeof (ModuleGrappleNode).GetMethod("Grapple", AccessTools.all);
    private static readonly FieldInfo KerbalSeatField = typeof (KerbalEVA).GetField("kerbalSeat", AccessTools.all);
    public double GameTime;
    public Guid VesselId;
    public Guid CoupledVesselId;
    public uint PartFlightId;
    public uint CoupledPartFlightId;
    public CoupleTrigger Trigger;
    private static bool _activeVesselIsWeakVessel;
    private static bool _activeVesselIsDominantVessel;
    private static global::Vessel _dominantVessel;
    private static global::Vessel _weakVessel;

    public bool ProcessCouple()
    {
      VesselCouple._activeVesselIsWeakVessel = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == this.CoupledVesselId;
      VesselCouple._activeVesselIsDominantVessel = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == this.VesselId;
      bool flag = VesselCouple.ProcessCoupleInternal(this.VesselId, this.CoupledVesselId, this.PartFlightId, this.CoupledPartFlightId, this.Trigger);
      VesselCouple.AfterCouplingEvent();
      if (!flag)
        LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(this.CoupledVesselId, false, "Killing coupled vessel during a undetected coupling");
      return flag;
    }

    public static bool ProcessCouple(VesselCoupleMsgData msgData)
    {
      VesselCouple._activeVesselIsWeakVessel = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == msgData.CoupledVesselId;
      VesselCouple._activeVesselIsDominantVessel = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == msgData.VesselId;
      bool flag = VesselCouple.ProcessCoupleInternal(msgData.VesselId, msgData.CoupledVesselId, msgData.PartFlightId, msgData.CoupledPartFlightId, (CoupleTrigger) msgData.Trigger);
      VesselCouple.AfterCouplingEvent();
      if (!flag)
        LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(msgData.CoupledVesselId, false, "Killing coupled vessel during a undetected coupling");
      return flag;
    }

    private static bool ProcessCoupleInternal(
      Guid vesselId,
      Guid coupledVesselId,
      uint partFlightId,
      uint coupledPartFlightId,
      CoupleTrigger trigger)
    {
      if (!VesselCommon.DoVesselChecks(vesselId))
        return false;
      bool flag = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && (FlightGlobals.ActiveVessel.id == vesselId || FlightGlobals.ActiveVessel.id == coupledVesselId);
      VesselCouple._dominantVessel = FlightGlobals.FindVessel(vesselId);
      if (Object.op_Equality((Object) VesselCouple._dominantVessel, (Object) null))
        return false;
      if (!VesselCouple._dominantVessel.loaded & flag)
        VesselCouple._dominantVessel.Load();
      VesselCouple._weakVessel = FlightGlobals.FindVessel(coupledVesselId);
      if (Object.op_Equality((Object) VesselCouple._weakVessel, (Object) null))
        return false;
      if (!VesselCouple._weakVessel.loaded & flag)
        VesselCouple._weakVessel.Load();
      ProtoPartSnapshot protoPart1 = VesselCouple._dominantVessel.protoVessel.GetProtoPart(partFlightId);
      ProtoPartSnapshot protoPart2 = VesselCouple._weakVessel.protoVessel.GetProtoPart(coupledPartFlightId);
      if (protoPart1 == null || protoPart2 == null || !Object.op_Implicit((Object) protoPart1.partRef) || !Object.op_Implicit((Object) protoPart2.partRef))
        return false;
      LmpClient.Base.System<VesselCoupleSystem>.Singleton.IgnoreEvents = true;
      switch (trigger)
      {
        case CoupleTrigger.DockingNode:
          ModuleDockingNode moduleImplementing1 = protoPart2.partRef.FindModuleImplementing<ModuleDockingNode>();
          if (Object.op_Implicit((Object) moduleImplementing1))
          {
            ModuleDockingNode moduleImplementing2 = protoPart1.partRef.FindModuleImplementing<ModuleDockingNode>();
            if (Object.op_Implicit((Object) moduleImplementing2))
              moduleImplementing1.DockToVessel(moduleImplementing2);
            break;
          }
          break;
        case CoupleTrigger.GrappleNode:
          ModuleGrappleNode moduleImplementing3 = protoPart2.partRef.FindModuleImplementing<ModuleGrappleNode>();
          if (Object.op_Implicit((Object) moduleImplementing3))
          {
            VesselCouple.GrappleMethod.Invoke((object) moduleImplementing3, new object[2]
            {
              (object) protoPart2,
              (object) protoPart1
            });
            break;
          }
          break;
        case CoupleTrigger.Kerbal:
          KerbalEVA moduleImplementing4 = protoPart2.partRef.FindModuleImplementing<KerbalEVA>();
          if (Object.op_Implicit((Object) moduleImplementing4) && Object.op_Implicit((Object) (VesselCouple.KerbalSeatField.GetValue((object) moduleImplementing4) as KerbalSeat)))
          {
            moduleImplementing4.fsm.RunEvent(moduleImplementing4.On_seatBoard);
            break;
          }
          break;
        case CoupleTrigger.Other:
          protoPart2.partRef.Couple(protoPart1.partRef);
          break;
      }
      LmpClient.Base.System<VesselCoupleSystem>.Singleton.IgnoreEvents = false;
      return true;
    }

    private static void AfterCouplingEvent()
    {
      if (VesselCouple._activeVesselIsWeakVessel && Object.op_Implicit((Object) VesselCouple._dominantVessel))
      {
        FlightGlobals.ForceSetActiveVessel(VesselCouple._dominantVessel);
        FlightInputHandler.SetNeutralControls();
      }
      if (!VesselCouple._activeVesselIsDominantVessel)
        return;
      VesselCouple._dominantVessel.MakeActive();
      FlightInputHandler.SetNeutralControls();
    }
  }
}
