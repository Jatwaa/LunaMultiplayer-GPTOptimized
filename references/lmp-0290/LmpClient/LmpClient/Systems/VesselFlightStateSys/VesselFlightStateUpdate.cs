// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.VesselFlightStateUpdate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpClient.VesselUtilities;
using LmpCommon;
using LmpCommon.Message.Data.Vessel;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class VesselFlightStateUpdate
  {
    public VesselFlightStateUpdate Target { get; set; }

    public FlightCtrlState InterpolatedCtrlState { get; set; } = new FlightCtrlState();

    public FlightCtrlState CtrlState { get; set; } = new FlightCtrlState();

    public double GameTimeStamp { get; set; }

    public int SubspaceId { get; set; }

    public Guid VesselId { get; set; }

    public float PingSec { get; set; }

    private double MaxInterpolationDuration => !LmpClient.Base.System<WarpSystem>.Singleton.SubspaceIsEqualOrInThePast(this.Target.SubspaceId) ? double.MaxValue : TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval).TotalSeconds * 2.0;

    private int MessageCount
    {
      get
      {
        FlightStateQueue flightStateQueue;
        return !VesselFlightStateSystem.TargetFlightStateQueue.TryGetValue(this.VesselId, out flightStateQueue) ? 0 : flightStateQueue.Count;
      }
    }

    public double TimeDifference { get; private set; }

    public double ExtraInterpolationTime { get; private set; }

    public bool InterpolationFinished => this.Target == null || (double) this.LerpPercentage >= 1.0;

    public double InterpolationDuration => LunaMath.Clamp(this.Target.GameTimeStamp - this.GameTimeStamp + this.ExtraInterpolationTime, 0.0, this.MaxInterpolationDuration);

    public float LerpPercentage { get; set; } = 1f;

    public VesselFlightStateUpdate()
    {
    }

    public VesselFlightStateUpdate(VesselFlightStateMsgData msgData)
    {
      this.VesselId = msgData.VesselId;
      this.GameTimeStamp = msgData.GameTime;
      this.SubspaceId = msgData.SubspaceId;
      this.PingSec = msgData.PingSec;
      this.CtrlState.CopyFrom(msgData);
    }

    public void CopyFrom(VesselFlightStateUpdate update)
    {
      this.VesselId = update.VesselId;
      this.GameTimeStamp = update.GameTimeStamp;
      this.SubspaceId = update.SubspaceId;
      this.PingSec = update.PingSec;
      this.CtrlState.CopyFrom(update.CtrlState);
    }

    public void CopyFrom(global::Vessel vessel)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      this.CtrlState.CopyFrom(vessel.ctrlState);
    }

    public FlightCtrlState GetInterpolatedValue()
    {
      if (!VesselCommon.IsSpectating && Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == this.VesselId)
        return FlightGlobals.ActiveVessel.ctrlState;
      FlightStateQueue flightStateQueue;
      VesselFlightStateUpdate result;
      if (this.InterpolationFinished && VesselFlightStateSystem.TargetFlightStateQueue.TryGetValue(this.VesselId, out flightStateQueue) && flightStateQueue.TryDequeue(out result))
      {
        if (this.Target == null)
        {
          this.GameTimeStamp = result.GameTimeStamp - TimeSpan.FromMilliseconds((double) SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval).TotalSeconds;
          this.CopyFrom(FlightGlobals.FindVessel(this.VesselId));
        }
        else
        {
          this.GameTimeStamp = this.Target.GameTimeStamp;
          this.SubspaceId = this.Target.SubspaceId;
          this.CtrlState.CopyFrom(this.Target.CtrlState);
        }
        this.LerpPercentage = 0.0f;
        if (this.Target != null)
        {
          this.Target.CopyFrom(result);
          VesselFlightStateSystem.TargetFlightStateQueue[this.VesselId].Recycle(result);
        }
        else
          this.Target = result;
        this.AdjustExtraInterpolationTimes();
      }
      if (this.Target == null)
        return this.InterpolatedCtrlState;
      this.InterpolatedCtrlState.Lerp(this.CtrlState, this.Target.CtrlState, this.LerpPercentage);
      this.LerpPercentage += Time.fixedDeltaTime / (float) this.InterpolationDuration;
      return this.InterpolatedCtrlState;
    }

    public void AdjustExtraInterpolationTimes()
    {
      this.TimeDifference = TimeSyncSystem.UniversalTime - this.GameTimeStamp - (double) VesselCommon.PositionAndFlightStateMessageOffsetSec(this.PingSec);
      if (LmpClient.Base.System<WarpSystem>.Singleton.CurrentlyWarping || this.SubspaceId == -1)
      {
        if (this.TimeDifference > 0.0)
          this.LerpPercentage = 1f;
        this.ExtraInterpolationTime = (double) Time.fixedDeltaTime;
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
  }
}
