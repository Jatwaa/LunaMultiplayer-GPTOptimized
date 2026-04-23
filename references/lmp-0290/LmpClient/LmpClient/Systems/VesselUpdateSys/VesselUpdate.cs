// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUpdateSys.VesselUpdate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselUpdateSys
{
  public class VesselUpdate
  {
    public double GameTime;
    public Guid VesselId;
    public string Name;
    public string Type;
    public double DistanceTraveled;
    public string Situation;
    public bool Landed;
    public bool Splashed;
    public bool Persistent;
    public string LandedAt;
    public string DisplayLandedAt;
    public double MissionTime;
    public double LaunchTime;
    public double LastUt;
    public uint RefTransformId;
    public bool AutoClean;
    public string AutoCleanReason;
    public bool WasControllable;
    public int Stage;
    public float[] Com = new float[3];

    public void ProcessVesselUpdate()
    {
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !VesselCommon.DoVesselChecks(vessel.id))
        return;
      int currentStage = vessel.currentStage;
      this.UpdateVesselFields(vessel);
      this.UpdateProtoVesselValues(vessel.protoVessel);
      if (Object.op_Implicit((Object) vessel.orbitDriver) && !vessel.loaded)
      {
        if (vessel.situation < 8 && vessel.orbitDriver.updateMode != 2)
          vessel.orbitDriver.SetOrbitMode((OrbitDriver.UpdateMode) 2);
        else if (vessel.situation >= 8 && vessel.orbitDriver.updateMode != 1)
          vessel.orbitDriver.SetOrbitMode((OrbitDriver.UpdateMode) 1);
      }
      if (currentStage == this.Stage)
        return;
      vessel.currentStage = this.Stage;
      if (Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) vessel))
        StageManager.RecalculateVesselStaging(vessel);
    }

    private void UpdateVesselFields(Vessel vessel)
    {
      vessel.vesselName = this.Name;
      vessel.vesselType = (VesselType) Enum.Parse(typeof (VesselType), this.Type);
      vessel.distanceTraveled = this.DistanceTraveled;
      vessel.protoVessel.situation = (Vessel.Situations) Enum.Parse(typeof (Vessel.Situations), this.Situation);
      vessel.situation = (Vessel.Situations) Enum.Parse(typeof (Vessel.Situations), this.Situation);
      vessel.Landed = this.Landed;
      vessel.Splashed = this.Splashed;
      vessel.landedAt = this.LandedAt;
      vessel.displaylandedAt = this.DisplayLandedAt;
      vessel.missionTime = this.MissionTime;
      vessel.launchTime = this.LaunchTime;
      vessel.lastUT = this.LastUt;
      vessel.isPersistent = this.Persistent;
      vessel.referenceTransformId = this.RefTransformId;
      if (this.AutoClean)
        vessel.SetAutoClean(this.AutoCleanReason);
      vessel.currentStage = this.Stage;
      vessel.localCoM.x = this.Com[0];
      vessel.localCoM.y = this.Com[1];
      vessel.localCoM.z = this.Com[2];
      for (int index = 0; index < 17; ++index)
      {
        if ((KSPActionGroup) (1 << index) == 16 && VesselCommon.IsSpectating && Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == vessel.id && vessel.ActionGroups[(KSPActionGroup) 16])
        {
          vessel.ActionGroups.ToggleGroup((KSPActionGroup) 16);
          vessel.ActionGroups.groups[index] = false;
        }
      }
    }

    private void UpdateProtoVesselValues(ProtoVessel protoVessel)
    {
      if (protoVessel == null)
        return;
      protoVessel.vesselName = this.Name;
      protoVessel.vesselType = (VesselType) Enum.Parse(typeof (VesselType), this.Type);
      protoVessel.distanceTraveled = this.DistanceTraveled;
      protoVessel.situation = (Vessel.Situations) Enum.Parse(typeof (Vessel.Situations), this.Situation);
      protoVessel.landed = this.Landed;
      protoVessel.landedAt = this.LandedAt;
      protoVessel.displaylandedAt = this.DisplayLandedAt;
      protoVessel.splashed = this.Splashed;
      protoVessel.missionTime = this.MissionTime;
      protoVessel.launchTime = this.LaunchTime;
      protoVessel.lastUT = this.LastUt;
      protoVessel.persistent = this.Persistent;
      protoVessel.refTransform = this.RefTransformId;
      protoVessel.autoClean = this.AutoClean;
      protoVessel.autoCleanReason = this.AutoCleanReason;
      protoVessel.wasControllable = this.WasControllable;
      protoVessel.stage = this.Stage;
      protoVessel.CoM.x = this.Com[0];
      protoVessel.CoM.y = this.Com[1];
      protoVessel.CoM.z = this.Com[2];
    }
  }
}
