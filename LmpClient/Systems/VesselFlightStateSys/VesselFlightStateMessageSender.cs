using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using LmpCommon.Time;
using System;

namespace LmpClient.Systems.VesselFlightStateSys
{
    public class VesselFlightStateMessageSender : SubSystem<VesselFlightStateSystem>, IMessageSender
    {
        // ── Delta suppression ─────────────────────────────────────────────────
        // We only transmit a new flight-state message when the control inputs
        // actually changed.  When SAS / MechJeb / autopilot is holding a fixed
        // attitude the state is bitwise-identical on every frame — without this
        // guard we were sending 10-20 redundant messages per second.
        //
        // Even when inputs are frozen we still send a "heartbeat" every
        // ForceSendIntervalMs so late-joining clients pick up the current state.

        private const float  FlightStateEpsilon   = 0.0005f; // ~0.05 % — below any perceivable change
        private const int    ForceSendIntervalMs  = 5000;    // always send at least once per 5 s

        private struct Snapshot
        {
            public float MainThrottle, Pitch, PitchTrim, Roll, RollTrim;
            public float Yaw, YawTrim, WheelThrottle, WheelThrottleTrim;
            public float WheelSteer, WheelSteerTrim, X, Y, Z;
            public bool  GearDown, GearUp, Headlight, KillRot;
        }

        private static Snapshot  _last;
        private static bool      _lastIsSet;
        private static DateTime  _lastForceSent = DateTime.MinValue;

        // Called when the active vessel changes so we force an immediate send.
        public static void ResetDelta() { _lastIsSet = false; }

        // ─────────────────────────────────────────────────────────────────────

        public void SendMessage(IMessageData msg)
        {
            NetworkSender.QueueOutgoingMessage(MessageFactory.CreateNew<VesselCliMsg>(msg));
        }

        public void SendCurrentFlightState()
        {
            var flightState = new FlightCtrlState();
            flightState.CopyFrom(FlightGlobals.ActiveVessel.ctrlState);

            // Skip send if nothing changed, unless the heartbeat interval has elapsed.
            bool forceHeartbeat = (LunaComputerTime.UtcNow - _lastForceSent).TotalMilliseconds >= ForceSendIntervalMs;
            if (_lastIsSet && !forceHeartbeat && !StateChanged(flightState))
                return;

            // Capture snapshot for next comparison.
            _last = new Snapshot
            {
                MainThrottle      = flightState.mainThrottle,
                Pitch             = flightState.pitch,
                PitchTrim         = flightState.pitchTrim,
                Roll              = flightState.roll,
                RollTrim          = flightState.rollTrim,
                Yaw               = flightState.yaw,
                YawTrim           = flightState.yawTrim,
                WheelThrottle     = flightState.wheelThrottle,
                WheelThrottleTrim = flightState.wheelThrottleTrim,
                WheelSteer        = flightState.wheelSteer,
                WheelSteerTrim    = flightState.wheelSteerTrim,
                X                 = flightState.X,
                Y                 = flightState.Y,
                Z                 = flightState.Z,
                GearDown          = flightState.gearDown,
                GearUp            = flightState.gearUp,
                Headlight         = flightState.headlight,
                KillRot           = flightState.killRot,
            };
            _lastIsSet     = true;
            _lastForceSent = LunaComputerTime.UtcNow;

            var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselFlightStateMsgData>();
            msgData.PingSec           = NetworkStatistics.PingSec;
            msgData.GameTime          = TimeSyncSystem.UniversalTime;
            msgData.SubspaceId        = WarpSystem.Singleton.CurrentSubspace;
            msgData.VesselId          = FlightGlobals.ActiveVessel.id;
            msgData.GearDown          = flightState.gearDown;
            msgData.GearUp            = flightState.gearUp;
            msgData.Headlight         = flightState.headlight;
            msgData.KillRot           = flightState.killRot;
            msgData.MainThrottle      = flightState.mainThrottle;
            msgData.Pitch             = flightState.pitch;
            msgData.PitchTrim         = flightState.pitchTrim;
            msgData.Roll              = flightState.roll;
            msgData.RollTrim          = flightState.rollTrim;
            msgData.WheelSteer        = flightState.wheelSteer;
            msgData.WheelSteerTrim    = flightState.wheelSteerTrim;
            msgData.WheelThrottle     = flightState.wheelThrottle;
            msgData.WheelThrottleTrim = flightState.wheelThrottleTrim;
            msgData.X                 = flightState.X;
            msgData.Y                 = flightState.Y;
            msgData.Yaw               = flightState.yaw;
            msgData.YawTrim           = flightState.yawTrim;
            msgData.Z                 = flightState.Z;

            SendMessage(msgData);
        }

        private static bool StateChanged(FlightCtrlState s)
        {
            var l = _last;
            return Math.Abs(s.mainThrottle      - l.MainThrottle)      > FlightStateEpsilon
                || Math.Abs(s.pitch             - l.Pitch)             > FlightStateEpsilon
                || Math.Abs(s.pitchTrim         - l.PitchTrim)         > FlightStateEpsilon
                || Math.Abs(s.roll              - l.Roll)              > FlightStateEpsilon
                || Math.Abs(s.rollTrim          - l.RollTrim)          > FlightStateEpsilon
                || Math.Abs(s.yaw               - l.Yaw)               > FlightStateEpsilon
                || Math.Abs(s.yawTrim           - l.YawTrim)           > FlightStateEpsilon
                || Math.Abs(s.wheelThrottle     - l.WheelThrottle)     > FlightStateEpsilon
                || Math.Abs(s.wheelThrottleTrim - l.WheelThrottleTrim) > FlightStateEpsilon
                || Math.Abs(s.wheelSteer        - l.WheelSteer)        > FlightStateEpsilon
                || Math.Abs(s.wheelSteerTrim    - l.WheelSteerTrim)    > FlightStateEpsilon
                || Math.Abs(s.X                 - l.X)                 > FlightStateEpsilon
                || Math.Abs(s.Y                 - l.Y)                 > FlightStateEpsilon
                || Math.Abs(s.Z                 - l.Z)                 > FlightStateEpsilon
                || s.gearDown  != l.GearDown  || s.gearUp  != l.GearUp
                || s.headlight != l.Headlight || s.killRot != l.KillRot;
        }
    }
}
