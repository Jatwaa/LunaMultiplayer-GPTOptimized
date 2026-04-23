// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselDecoupleSys.VesselDecouple
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.Lock;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselDecoupleSys
{
  public class VesselDecouple
  {
    public double GameTime;
    public Guid VesselId;
    public uint PartFlightId;
    public float BreakForce;
    public Guid NewVesselId;

    public void ProcessDecouple()
    {
      if (!VesselCommon.DoVesselChecks(this.VesselId))
        return;
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.PartFlightId);
      if (protoPart == null || !Object.op_Implicit((Object) protoPart.partRef))
        return;
      LmpClient.Base.System<VesselDecoupleSystem>.Singleton.ManuallyDecouplingVesselId = protoPart.partRef.vessel.id;
      LmpClient.Base.System<VesselDecoupleSystem>.Singleton.IgnoreEvents = true;
      protoPart.partRef.decouple(this.BreakForce);
      protoPart.partRef.vessel.id = this.NewVesselId;
      LmpClient.Base.System<LockSystem>.Singleton.FireVesselLocksEvents(this.NewVesselId);
      protoPart.partRef.vessel.SetImmortal(true);
      LmpClient.Base.System<VesselPositionSystem>.Singleton.ForceUpdateVesselPosition(this.NewVesselId);
      LmpClient.Base.System<VesselDecoupleSystem>.Singleton.IgnoreEvents = false;
      LmpClient.Base.System<VesselDecoupleSystem>.Singleton.ManuallyDecouplingVesselId = Guid.Empty;
    }
  }
}
