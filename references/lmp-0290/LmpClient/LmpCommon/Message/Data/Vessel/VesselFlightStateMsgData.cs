// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselFlightStateMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselFlightStateMsgData : VesselBaseMsgData
  {
    public int SubspaceId;
    public float PingSec;
    public float MainThrottle;
    public float WheelThrottleTrim;
    public float X;
    public float Y;
    public float Z;
    public bool KillRot;
    public bool GearUp;
    public bool GearDown;
    public bool Headlight;
    public float WheelThrottle;
    public float Pitch;
    public float Roll;
    public float Yaw;
    public float PitchTrim;
    public float RollTrim;
    public float YawTrim;
    public float WheelSteer;
    public float WheelSteerTrim;

    internal VesselFlightStateMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Flightstate;

    public override string ClassName { get; } = nameof (VesselFlightStateMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.SubspaceId);
      lidgrenMsg.Write(this.PingSec);
      lidgrenMsg.Write(this.MainThrottle);
      lidgrenMsg.Write(this.WheelThrottle);
      lidgrenMsg.Write(this.WheelThrottleTrim);
      lidgrenMsg.Write(this.X);
      lidgrenMsg.Write(this.Y);
      lidgrenMsg.Write(this.Z);
      lidgrenMsg.Write(this.KillRot);
      lidgrenMsg.Write(this.GearUp);
      lidgrenMsg.Write(this.GearDown);
      lidgrenMsg.Write(this.Headlight);
      lidgrenMsg.Write(this.Pitch);
      lidgrenMsg.Write(this.Roll);
      lidgrenMsg.Write(this.Yaw);
      lidgrenMsg.Write(this.PitchTrim);
      lidgrenMsg.Write(this.RollTrim);
      lidgrenMsg.Write(this.YawTrim);
      lidgrenMsg.Write(this.WheelSteer);
      lidgrenMsg.Write(this.WheelSteerTrim);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.SubspaceId = lidgrenMsg.ReadInt32();
      this.PingSec = lidgrenMsg.ReadFloat();
      this.MainThrottle = lidgrenMsg.ReadFloat();
      this.WheelThrottle = lidgrenMsg.ReadFloat();
      this.WheelThrottleTrim = lidgrenMsg.ReadFloat();
      this.X = lidgrenMsg.ReadFloat();
      this.Y = lidgrenMsg.ReadFloat();
      this.Z = lidgrenMsg.ReadFloat();
      this.KillRot = lidgrenMsg.ReadBoolean();
      this.GearUp = lidgrenMsg.ReadBoolean();
      this.GearDown = lidgrenMsg.ReadBoolean();
      this.Headlight = lidgrenMsg.ReadBoolean();
      this.Pitch = lidgrenMsg.ReadFloat();
      this.Roll = lidgrenMsg.ReadFloat();
      this.Yaw = lidgrenMsg.ReadFloat();
      this.PitchTrim = lidgrenMsg.ReadFloat();
      this.RollTrim = lidgrenMsg.ReadFloat();
      this.YawTrim = lidgrenMsg.ReadFloat();
      this.WheelSteer = lidgrenMsg.ReadFloat();
      this.WheelSteerTrim = lidgrenMsg.ReadFloat();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + 60 + 4;
  }
}
