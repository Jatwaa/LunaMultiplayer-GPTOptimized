// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.VesselPositionUpdate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselPositionSys.ExtensionMethods;
using LmpClient.Systems.Warp;
using LmpClient.VesselUtilities;
using LmpCommon;
using LmpCommon.Message.Data.Vessel;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPositionSys
{
  public class VesselPositionUpdate
  {
    private global::Vessel _vessel;

    public global::Vessel Vessel
    {
      get
      {
        if (Object.op_Equality((Object) this._vessel, (Object) null))
          this._vessel = FlightGlobals.FindVessel(this.VesselId);
        return this._vessel;
      }
    }

    public CelestialBody Body => VesselPositionUpdate.GetBody(this.BodyIndex);

    public VesselPositionUpdate Target { get; set; }

    public Guid VesselId { get; set; }

    public int BodyIndex { get; set; }

    public bool Landed { get; set; }

    public bool Splashed { get; set; }

    public double[] LatLonAlt { get; set; } = new double[3];

    public double[] VelocityVector { get; set; } = new double[3];

    public double[] NormalVector { get; set; } = new double[3];

    public double[] Orbit { get; set; } = new double[8];

    public float[] SrfRelRotation { get; set; } = new float[4];

    public float PingSec { get; set; }

    public float HeightFromTerrain { get; set; }

    public double GameTimeStamp { get; set; }

    public int SubspaceId { get; set; }

    public bool HackingGravity { get; set; }

    public global::Orbit KspOrbit { get; set; } = new global::Orbit();

    public Vector3d Velocity => new Vector3d(this.VelocityVector[0], this.VelocityVector[1], this.VelocityVector[2]);

    public Quaternion SurfaceRelRotation => new Quaternion(this.SrfRelRotation[0], this.SrfRelRotation[1], this.SrfRelRotation[2], this.SrfRelRotation[3]);

    public Vector3 Normal => Vector3d.op_Implicit(new Vector3d(this.NormalVector[0], this.NormalVector[1], this.NormalVector[2]));

    private double MaxInterpolationDuration => !LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsEqualOrInThePast(this.Target.SubspaceId) ? double.MaxValue : TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval).TotalSeconds * 2.0;

    private int MessageCount
    {
      get
      {
        PositionUpdateQueue positionUpdateQueue;
        return !VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(this.VesselId, out positionUpdateQueue) ? 0 : positionUpdateQueue.Count;
      }
    }

    public double TimeDifference { get; private set; }

    public double ExtraInterpolationTime { get; private set; }

    public bool InterpolationFinished => this.Target == null || (double) this.CurrentFrame >= (double) this.NumFrames;

    public double InterpolationDuration => LunaMath.Clamp(this.Target.GameTimeStamp - this.GameTimeStamp + this.ExtraInterpolationTime, 0.0, this.MaxInterpolationDuration);

    public float LerpPercentage => Mathf.Clamp01(this.CurrentFrame / (float) this.NumFrames);

    public float CurrentFrame { get; set; }

    public int NumFrames => (int) (this.InterpolationDuration / (double) Time.fixedDeltaTime) + 1;

    public VesselPositionUpdate()
    {
    }

    public VesselPositionUpdate(VesselPositionMsgData msgData)
    {
      this.VesselId = msgData.VesselId;
      this.BodyIndex = msgData.BodyIndex;
      this.SubspaceId = msgData.SubspaceId;
      this.PingSec = msgData.PingSec;
      this.HeightFromTerrain = msgData.HeightFromTerrain;
      this.Landed = msgData.Landed;
      this.Splashed = msgData.Splashed;
      this.GameTimeStamp = msgData.GameTime;
      this.HackingGravity = msgData.HackingGravity;
      Array.Copy((Array) msgData.SrfRelRotation, (Array) this.SrfRelRotation, 4);
      Array.Copy((Array) msgData.LatLonAlt, (Array) this.LatLonAlt, 3);
      Array.Copy((Array) msgData.VelocityVector, (Array) this.VelocityVector, 3);
      Array.Copy((Array) msgData.NormalVector, (Array) this.NormalVector, 3);
      Array.Copy((Array) msgData.Orbit, (Array) this.Orbit, 8);
    }

    public void CopyFrom(VesselPositionUpdate update)
    {
      this.VesselId = update.VesselId;
      this.BodyIndex = update.BodyIndex;
      this.SubspaceId = update.SubspaceId;
      this.PingSec = update.PingSec;
      this.HeightFromTerrain = update.HeightFromTerrain;
      this.Landed = update.Landed;
      this.Splashed = update.Splashed;
      this.GameTimeStamp = update.GameTimeStamp;
      this.HackingGravity = update.HackingGravity;
      Array.Copy((Array) update.SrfRelRotation, (Array) this.SrfRelRotation, 4);
      Array.Copy((Array) update.LatLonAlt, (Array) this.LatLonAlt, 3);
      Array.Copy((Array) update.VelocityVector, (Array) this.VelocityVector, 3);
      Array.Copy((Array) update.NormalVector, (Array) this.NormalVector, 3);
      Array.Copy((Array) update.Orbit, (Array) this.Orbit, 8);
    }

    public void UpdateVesselWithPositionData()
    {
      if (Object.op_Equality((Object) this.Body, (Object) null))
        return;
      PositionUpdateQueue positionUpdateQueue;
      VesselPositionUpdate result;
      if (this.InterpolationFinished && VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(this.VesselId, out positionUpdateQueue) && positionUpdateQueue.TryDequeue(out result))
      {
        if (this.Target == null)
        {
          this.GameTimeStamp = result.GameTimeStamp - TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval).TotalSeconds;
          this.PingSec = 0.0f;
          this.CopyFrom(result);
        }
        else
          this.CopyFrom(this.Target);
        this.CurrentFrame = 0.0f;
        if (this.Target != null)
        {
          this.Target.CopyFrom(result);
          positionUpdateQueue.Recycle(result);
        }
        else
          this.Target = result;
        this.AdjustExtraInterpolationTimes();
        this.InitializeOrbits();
      }
      if (this.Target == null)
        return;
      this.Vessel.SetVesselPosition(this, this.Target, this.LerpPercentage);
    }

    public void ApplyInterpolatedVesselUpdate()
    {
      try
      {
        this.UpdateVesselWithPositionData();
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("ApplyInterpolations: {0}", (object) ex));
      }
      finally
      {
        ++this.CurrentFrame;
      }
    }

    private void InitializeOrbits()
    {
      this.KspOrbit.SetOrbit(this.Orbit[0], this.Orbit[1], this.Orbit[2], this.Orbit[3] + VesselPositionUpdate.GetLanFixFactor(this.GameTimeStamp, this.SubspaceId, this.Vessel, this.Body), this.Orbit[4], this.Orbit[5], this.CalculateEpochTime(this.Orbit[6]), this.Body);
      double lanFixFactor = VesselPositionUpdate.GetLanFixFactor(this.Target.GameTimeStamp, this.Target.SubspaceId, this.Vessel, this.Target.Body);
      this.Target.KspOrbit.SetOrbit(this.Target.Orbit[0], this.Target.Orbit[1], this.Target.Orbit[2], this.Target.Orbit[3] + lanFixFactor, this.Target.Orbit[4], this.Target.Orbit[5], this.CalculateTargetEpochTime(this.Target.Orbit[6]), this.Target.Body);
      double anomalyFixFactor1 = VesselPositionUpdate.GetMeanAnomalyFixFactor(this.GameTimeStamp, this.SubspaceId, this.Vessel, this.KspOrbit);
      this.KspOrbit.SetOrbit(this.Orbit[0], this.Orbit[1], this.Orbit[2], this.Orbit[3] + lanFixFactor, this.Orbit[4], this.Orbit[5] + anomalyFixFactor1, this.Orbit[6], this.Body);
      double anomalyFixFactor2 = VesselPositionUpdate.GetMeanAnomalyFixFactor(this.Target.GameTimeStamp, this.Target.SubspaceId, this.Vessel, this.Target.KspOrbit);
      this.Target.KspOrbit.SetOrbit(this.Target.Orbit[0], this.Target.Orbit[1], this.Target.Orbit[2], this.Target.Orbit[3] + lanFixFactor, this.Target.Orbit[4], this.Target.Orbit[5] + anomalyFixFactor2, this.Target.Orbit[6], this.Target.Body);
    }

    private double CalculateTargetEpochTime(double targetEpoch) => this.SubspaceId == -1 || LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping || LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsInThePast(this.SubspaceId) ? targetEpoch : Planetarium.GetUniversalTime() + (this.Target.GameTimeStamp - this.GameTimeStamp);

    private double CalculateEpochTime(double currentEpoch) => this.SubspaceId == -1 || LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping || LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsInThePast(this.SubspaceId) ? currentEpoch : Planetarium.GetUniversalTime();

    private static double GetMeanAnomalyFixFactor(
      double timestamp,
      int subspaceId,
      global::Vessel vessel,
      global::Orbit orbit)
    {
      if (Object.op_Implicit((Object) vessel) && (vessel.situation >= 32 || vessel.loaded && subspaceId == -1))
        return 0.0;
      if (subspaceId == -1 && timestamp < TimeSyncSystem.UniversalTime)
        return (orbit.getObtAtUT(TimeSyncSystem.UniversalTime) - orbit.getObtAtUT(timestamp)) * orbit.meanMotion;
      if (!LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping && !LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsInThePast(subspaceId))
        return 0.0;
      double withGivenSubspace = LmpClient.Base.System<WarpSystem>.Singleton.GetTimeDifferenceWithGivenSubspace(subspaceId);
      return (orbit.getObtAtUT(TimeSyncSystem.UniversalTime) - orbit.getObtAtUT(TimeSyncSystem.UniversalTime - withGivenSubspace)) * orbit.meanMotion;
    }

    private static double GetLanFixFactor(
      double timestamp,
      int subspaceId,
      global::Vessel vessel,
      CelestialBody body)
    {
      if (Object.op_Implicit((Object) vessel) && vessel.situation >= 32 || body.SiderealDayLength() <= 0.0)
        return 0.0;
      if (subspaceId == -1 && timestamp < TimeSyncSystem.UniversalTime)
        return Math.Abs((TimeSyncSystem.UniversalTime - timestamp) * 360.0 / body.SiderealDayLength());
      return LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping || LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsInThePast(subspaceId) ? Math.Abs(LmpClient.Base.System<WarpSystem>.Singleton.GetTimeDifferenceWithGivenSubspace(subspaceId) * 360.0 / body.SiderealDayLength()) : 0.0;
    }

    public void AdjustExtraInterpolationTimes()
    {
      this.TimeDifference = TimeSyncSystem.UniversalTime - this.GameTimeStamp - (double) VesselCommon.PositionAndFlightStateMessageOffsetSec(this.PingSec);
      if (LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping || this.SubspaceId == -1)
      {
        if (this.TimeDifference > 0.0)
          this.CurrentFrame = float.MaxValue;
        else
          this.ExtraInterpolationTime = this.GetInterpolationFixFactor();
      }
      else
      {
        if (LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsInThePast(this.SubspaceId))
          this.TimeDifference -= Math.Abs(LmpClient.Base.System<WarpSystem>.Singleton.GetTimeDifferenceWithGivenSubspace(this.SubspaceId));
        this.ExtraInterpolationTime = (this.TimeDifference > 0.0 ? -1.0 : 1.0) * this.GetInterpolationFixFactor();
      }
    }

    private double GetInterpolationFixFactor()
    {
      double num1 = Math.Abs(Math.Abs(this.TimeDifference));
      double num2 = num1 / (double) Time.fixedDeltaTime;
      if (num2 < 1.0)
        return 0.0;
      if (num2 <= 2.0)
        return (double) Time.fixedDeltaTime;
      if (num2 <= 5.0)
        return (double) Time.fixedDeltaTime * 2.0;
      return num1 <= 2.5 ? (double) Time.fixedDeltaTime * num2 / 2.0 : (double) Time.fixedDeltaTime * num2;
    }

    private static CelestialBody GetBody(int bodyIndex)
    {
      try
      {
        return FlightGlobals.Bodies[bodyIndex];
      }
      catch (Exception ex)
      {
        return (CelestialBody) null;
      }
    }
  }
}
