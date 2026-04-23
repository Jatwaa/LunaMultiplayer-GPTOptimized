// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFairingsSys.VesselFairing
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselFairingsSys
{
  public class VesselFairing
  {
    public double GameTime;
    public Guid VesselId;
    public uint PartFlightId;

    public void ProcessFairing()
    {
      if (!VesselCommon.DoVesselChecks(this.VesselId))
        return;
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.PartFlightId);
      if (protoPart == null)
        return;
      VesselFairing.ProcessFairingChange(protoPart);
    }

    private static void ProcessFairingChange(ProtoPartSnapshot protoPart)
    {
      ProtoPartModuleSnapshot moduleInProtoPart = protoPart.FindProtoPartModuleInProtoPart("ModuleProceduralFairing");
      moduleInProtoPart?.moduleValues.SetValue("fsm", "st_flight_deployed", false);
      moduleInProtoPart?.moduleValues.RemoveNodesStartWith("XSECTION");
      try
      {
        if (!(moduleInProtoPart?.moduleRef is ModuleProceduralFairing moduleRef))
          return;
        moduleRef.DeployFairing();
      }
      catch (Exception ex)
      {
      }
    }
  }
}
