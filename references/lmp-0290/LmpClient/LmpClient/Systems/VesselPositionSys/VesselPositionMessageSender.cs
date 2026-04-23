// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.VesselPositionMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpClient.Utilities;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPositionSys
{
  public class VesselPositionMessageSender : SubSystem<VesselPositionSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselPositionUpdate(global::Vessel vessel, bool doOrbitDriverReadyCheck = false)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      if (doOrbitDriverReadyCheck && !vessel.orbitDriver.Ready())
      {
        CoroutineUtil.StartConditionRoutine(nameof (SendVesselPositionUpdate), (Action) (() => this.SendVesselPositionUpdate(vessel)), (Func<bool>) (() => vessel.orbitDriver.Ready()), 10f);
      }
      else
      {
        VesselPositionMsgData messageFromVessel = VesselPositionMessageSender.CreateMessageFromVessel(vessel);
        if (messageFromVessel == null)
          return;
        this.SendMessage((IMessageData) messageFromVessel);
      }
    }

    public static VesselPositionMsgData CreateMessageFromVessel(global::Vessel vessel)
    {
      if (!VesselPositionMessageSender.OrbitParametersAreOk(vessel))
        return (VesselPositionMsgData) null;
      VesselPositionMsgData newMessageData = SystemBase.MessageFactory.CreateNewMessageData<VesselPositionMsgData>();
      newMessageData.PingSec = NetworkStatistics.PingSec;
      newMessageData.SubspaceId = LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspace;
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      try
      {
        newMessageData.VesselId = vessel.id;
        newMessageData.BodyIndex = vessel.mainBody.flightGlobalsIndex;
        newMessageData.Landed = vessel.Landed;
        newMessageData.Splashed = vessel.Splashed;
        VesselPositionMessageSender.SetSrfRelRotation(vessel, newMessageData);
        VesselPositionMessageSender.SetLatLonAlt(vessel, newMessageData);
        VesselPositionMessageSender.SetVelocityVector(vessel, newMessageData);
        VesselPositionMessageSender.SetNormalVector(vessel, newMessageData);
        VesselPositionMessageSender.SetOrbit(vessel, newMessageData);
        newMessageData.HeightFromTerrain = vessel.heightFromTerrain;
        double num;
        if (MainSystem.BodiesGees.TryGetValue(vessel.mainBody, out num))
          newMessageData.HackingGravity = Math.Abs(num - vessel.mainBody.GeeASL) > 0.0001;
        newMessageData.HackingGravity = false;
        return newMessageData;
      }
      catch (Exception ex)
      {
        LunaLog.Log(string.Format("[LMP]: Failed to get vessel position update, exception: {0}", (object) ex));
      }
      return (VesselPositionMsgData) null;
    }

    private static void SetOrbit(global::Vessel vessel, VesselPositionMsgData msgData)
    {
      msgData.Orbit[0] = vessel.orbit.inclination;
      msgData.Orbit[1] = vessel.orbit.eccentricity;
      msgData.Orbit[2] = vessel.orbit.semiMajorAxis;
      msgData.Orbit[3] = vessel.orbit.LAN;
      msgData.Orbit[4] = vessel.orbit.argumentOfPeriapsis;
      msgData.Orbit[5] = vessel.orbit.meanAnomalyAtEpoch;
      msgData.Orbit[6] = vessel.orbit.epoch;
      msgData.Orbit[7] = (double) vessel.orbit.referenceBody.flightGlobalsIndex;
    }

    private static void SetVelocityVector(global::Vessel vessel, VesselPositionMsgData msgData)
    {
      Vector3 vector3 = Quaternion.op_Multiply(Quaternion.Inverse(vessel.mainBody.bodyTransform.rotation), Vector3d.op_Implicit(vessel.srf_velocity));
      msgData.VelocityVector[0] = (double) vector3.x;
      msgData.VelocityVector[1] = (double) vector3.y;
      msgData.VelocityVector[2] = (double) vector3.z;
    }

    private static void SetNormalVector(global::Vessel vessel, VesselPositionMsgData msgData)
    {
      msgData.NormalVector[0] = (double) vessel.terrainNormal.x;
      msgData.NormalVector[1] = (double) vessel.terrainNormal.y;
      msgData.NormalVector[2] = (double) vessel.terrainNormal.z;
    }

    private static void SetLatLonAlt(global::Vessel vessel, VesselPositionMsgData msgData)
    {
      msgData.LatLonAlt[0] = vessel.latitude;
      msgData.LatLonAlt[1] = vessel.longitude;
      msgData.LatLonAlt[2] = vessel.altitude;
    }

    private static void SetSrfRelRotation(global::Vessel vessel, VesselPositionMsgData msgData)
    {
      msgData.SrfRelRotation[0] = vessel.srfRelRotation.x;
      msgData.SrfRelRotation[1] = vessel.srfRelRotation.y;
      msgData.SrfRelRotation[2] = vessel.srfRelRotation.z;
      msgData.SrfRelRotation[3] = vessel.srfRelRotation.w;
    }

    private static bool OrbitParametersAreOk(global::Vessel vessel) => !double.IsNaN(vessel.orbit.inclination) && !double.IsNaN(vessel.orbit.eccentricity) && !double.IsNaN(vessel.orbit.semiMajorAxis) && !double.IsNaN(vessel.orbit.LAN) && !double.IsNaN(vessel.orbit.argumentOfPeriapsis) && !double.IsNaN(vessel.orbit.meanAnomalyAtEpoch) && !double.IsNaN(vessel.orbit.epoch) && !double.IsNaN((double) vessel.orbit.referenceBody.flightGlobalsIndex);
  }
}
