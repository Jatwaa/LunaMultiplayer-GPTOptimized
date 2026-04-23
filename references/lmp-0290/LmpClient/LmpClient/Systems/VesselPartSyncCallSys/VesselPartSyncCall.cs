// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncCallSys.VesselPartSyncCall
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncCallSys
{
  public class VesselPartSyncCall
  {
    public double GameTime;
    public Guid VesselId;
    public uint PartFlightId;
    public string ModuleName;
    public string MethodName;

    public void ProcessPartMethodCallSync()
    {
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !vessel.loaded || !VesselCommon.DoVesselChecks(this.VesselId))
        return;
      ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.PartFlightId);
      if (protoPart == null)
        return;
      ProtoPartModuleSnapshot moduleInProtoPart = protoPart.FindProtoPartModuleInProtoPart(this.ModuleName);
      if (moduleInProtoPart != null && Object.op_Inequality((Object) moduleInProtoPart.moduleRef, (Object) null))
      {
        ((object) moduleInProtoPart.moduleRef).GetType().GetMethod(this.MethodName, AccessTools.all)?.Invoke((object) moduleInProtoPart.moduleRef, (object[]) null);
        PartModuleEvent.onPartModuleMethodProcessed.Fire(moduleInProtoPart, this.MethodName);
      }
    }
  }
}
