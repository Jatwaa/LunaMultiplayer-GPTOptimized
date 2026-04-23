// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselResourceSys.VesselResource
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Data.Vessel;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselResourceSys
{
  public class VesselResource
  {
    public double GameTime;
    public Guid VesselId;
    public int ResourcesCount;
    public VesselResourceInfo[] Resources = new VesselResourceInfo[0];

    public void ProcessVesselResource()
    {
      global::Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !VesselCommon.DoVesselChecks(vessel.id))
        return;
      this.UpdateVesselFields(vessel);
    }

    private void UpdateVesselFields(global::Vessel vessel)
    {
      if (vessel.protoVessel == null)
        return;
      for (int index = 0; index < this.ResourcesCount; ++index)
      {
        ProtoPartSnapshot protoPart = vessel.protoVessel.GetProtoPart(this.Resources[index].PartFlightId);
        ProtoPartResourceSnapshot resourceInProtoPart = protoPart.FindResourceInProtoPart(this.Resources[index].ResourceName);
        if (resourceInProtoPart != null)
        {
          resourceInProtoPart.amount = this.Resources[index].Amount;
          resourceInProtoPart.flowState = this.Resources[index].FlowState;
          if (resourceInProtoPart.resourceRef == null)
          {
            if (Object.op_Inequality((Object) protoPart.partRef, (Object) null))
            {
              PartResource resource = protoPart.partRef.FindResource(resourceInProtoPart.resourceName);
              resource.amount = this.Resources[index].Amount;
              resource.flowState = this.Resources[index].FlowState;
            }
          }
          else
          {
            resourceInProtoPart.resourceRef.amount = this.Resources[index].Amount;
            resourceInProtoPart.resourceRef.flowState = this.Resources[index].FlowState;
          }
        }
      }
    }
  }
}
