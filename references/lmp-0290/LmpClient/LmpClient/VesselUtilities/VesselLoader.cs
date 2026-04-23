// Decompiled with JetBrains decompiler
// Type: LmpClient.VesselUtilities.VesselLoader
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens.Flight;
using LmpClient.Extensions;
using LmpClient.Systems.VesselPositionSys;
using System;
using UnityEngine;

namespace LmpClient.VesselUtilities
{
  public class VesselLoader
  {
    public static bool LoadVessel(ProtoVessel vesselProto, bool forceReload)
    {
      try
      {
        return vesselProto.Validate() && VesselLoader.LoadVesselIntoGame(vesselProto, forceReload);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error loading vessel: {0}", (object) ex));
        return false;
      }
    }

    private static bool LoadVesselIntoGame(ProtoVessel vesselProto, bool forceReload)
    {
      if (HighLogic.CurrentGame?.flightState == null)
        return false;
      bool flag = Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && vesselProto.vesselID == FlightGlobals.ActiveVessel.id;
      Vessel vessel = FlightGlobals.FindVessel(vesselProto.vesselID);
      if (Object.op_Inequality((Object) vessel, (Object) null))
      {
        if (!forceReload && vessel.Parts.Count == vesselProto.protoPartSnapshots.Count && vessel.GetCrewCount() == vesselProto.GetVesselCrew().Count)
          return true;
        LunaLog.Log(string.Format("[LMP]: Reloading vessel {0}", (object) vesselProto.vesselID));
        if (flag)
          vessel.RemoveAllCrew();
        FlightGlobals.RemoveVessel(vessel);
        foreach (Component part in vessel.parts)
          Object.Destroy((Object) part.gameObject);
        Object.Destroy((Object) ((Component) vessel).gameObject);
      }
      else
        LunaLog.Log(string.Format("[LMP]: Loading vessel {0}", (object) vesselProto.vesselID));
      vesselProto.Load(HighLogic.CurrentGame.flightState);
      if (Object.op_Equality((Object) vesselProto.vesselRef, (Object) null))
      {
        LunaLog.Log(string.Format("[LMP]: Protovessel {0} failed to create a vessel!", (object) vesselProto.vesselID));
        return false;
      }
      LmpClient.Base.System<VesselPositionSystem>.Singleton.ForceUpdateVesselPosition(vesselProto.vesselRef.id);
      vesselProto.vesselRef.protoVessel = vesselProto;
      if (vesselProto.vesselRef.isEVA)
      {
        KerbalEVA moduleImplementing = vesselProto.vesselRef.FindPartModuleImplementing<KerbalEVA>();
        if (Object.op_Inequality((Object) moduleImplementing, (Object) null) && moduleImplementing.fsm != null && !moduleImplementing.fsm.Started)
          moduleImplementing.fsm?.StartFSM("Idle (Grounded)");
        vesselProto.vesselRef.GoOnRails();
      }
      if (vesselProto.vesselRef.situation > 4)
        vesselProto.vesselRef.orbitDriver.updateFromParameters();
      if (double.IsNaN(vesselProto.vesselRef.orbitDriver.pos.x))
      {
        LunaLog.Log(string.Format("[LMP]: Protovessel {0} has an invalid orbit", (object) vesselProto.vesselID));
        return false;
      }
      if (flag)
      {
        vesselProto.vesselRef.Load();
        vesselProto.vesselRef.RebuildCrewList();
        FlightGlobals.ForceSetActiveVessel(vesselProto.vesselRef);
        vesselProto.vesselRef.SpawnCrew();
        foreach (ProtoCrewMember protoCrewMember in vesselProto.vesselRef.GetVesselCrew())
        {
          ProtoCrewMember._Spawn(protoCrewMember);
          if (Object.op_Implicit((Object) protoCrewMember.KerbalRef))
            protoCrewMember.KerbalRef.state = (Kerbal.States) 3;
        }
        if (KerbalPortraitGallery.Instance.ActiveCrewItems.Count != vesselProto.vesselRef.GetCrewCount())
          KerbalPortraitGallery.Instance.StartReset(FlightGlobals.ActiveVessel);
      }
      return true;
    }
  }
}
