// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.VesselFlightStateMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class VesselFlightStateMessageSender : SubSystem<VesselFlightStateSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendCurrentFlightState()
    {
      FlightCtrlState flightCtrlState = new FlightCtrlState();
      flightCtrlState.CopyFrom(FlightGlobals.ActiveVessel.ctrlState);
      VesselFlightStateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselFlightStateMsgData>();
      newMessageData.PingSec = NetworkStatistics.PingSec;
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.SubspaceId = System<WarpSystem>.Singleton.CurrentSubspace;
      newMessageData.VesselId = FlightGlobals.ActiveVessel.id;
      newMessageData.GearDown = flightCtrlState.gearDown;
      newMessageData.GearUp = flightCtrlState.gearUp;
      newMessageData.Headlight = flightCtrlState.headlight;
      newMessageData.KillRot = flightCtrlState.killRot;
      newMessageData.MainThrottle = flightCtrlState.mainThrottle;
      newMessageData.Pitch = flightCtrlState.pitch;
      newMessageData.PitchTrim = flightCtrlState.pitchTrim;
      newMessageData.Roll = flightCtrlState.roll;
      newMessageData.RollTrim = flightCtrlState.rollTrim;
      newMessageData.WheelSteer = flightCtrlState.wheelSteer;
      newMessageData.WheelSteerTrim = flightCtrlState.wheelSteerTrim;
      newMessageData.WheelThrottle = flightCtrlState.wheelThrottle;
      newMessageData.WheelThrottleTrim = flightCtrlState.wheelThrottleTrim;
      newMessageData.X = flightCtrlState.X;
      newMessageData.Y = flightCtrlState.Y;
      newMessageData.Yaw = flightCtrlState.yaw;
      newMessageData.YawTrim = flightCtrlState.yawTrim;
      newMessageData.Z = flightCtrlState.Z;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
