// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselUpdateMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselUpdateMsgData : VesselBaseMsgData
  {
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

    internal VesselUpdateMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Update;

    public override string ClassName { get; } = nameof (VesselUpdateMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.Name);
      lidgrenMsg.Write(this.Type);
      lidgrenMsg.Write(this.DistanceTraveled);
      lidgrenMsg.Write(this.Situation);
      lidgrenMsg.Write(this.Landed);
      lidgrenMsg.Write(this.Splashed);
      lidgrenMsg.Write(this.Persistent);
      lidgrenMsg.Write(this.LandedAt);
      lidgrenMsg.Write(this.DisplayLandedAt);
      lidgrenMsg.Write(this.MissionTime);
      lidgrenMsg.Write(this.LaunchTime);
      lidgrenMsg.Write(this.LastUt);
      lidgrenMsg.Write(this.RefTransformId);
      lidgrenMsg.Write(this.AutoClean);
      lidgrenMsg.Write(this.AutoCleanReason);
      lidgrenMsg.Write(this.WasControllable);
      lidgrenMsg.Write(this.Stage);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.Com[index]);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Name = lidgrenMsg.ReadString();
      this.Type = lidgrenMsg.ReadString();
      this.DistanceTraveled = lidgrenMsg.ReadDouble();
      this.Situation = lidgrenMsg.ReadString();
      this.Landed = lidgrenMsg.ReadBoolean();
      this.Splashed = lidgrenMsg.ReadBoolean();
      this.Persistent = lidgrenMsg.ReadBoolean();
      this.LandedAt = lidgrenMsg.ReadString();
      this.DisplayLandedAt = lidgrenMsg.ReadString();
      this.MissionTime = lidgrenMsg.ReadDouble();
      this.LaunchTime = lidgrenMsg.ReadDouble();
      this.LastUt = lidgrenMsg.ReadDouble();
      this.RefTransformId = lidgrenMsg.ReadUInt32();
      this.AutoClean = lidgrenMsg.ReadBoolean();
      this.AutoCleanReason = lidgrenMsg.ReadString();
      this.WasControllable = lidgrenMsg.ReadBoolean();
      this.Stage = lidgrenMsg.ReadInt32();
      for (int index = 0; index < 3; ++index)
        this.Com[index] = lidgrenMsg.ReadFloat();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 32 + 5 + 4 + 4 + this.Name.GetByteCount() + this.Type.GetByteCount() + this.Situation.GetByteCount() + this.LandedAt.GetByteCount() + this.DisplayLandedAt.GetByteCount() + this.AutoCleanReason.GetByteCount();
  }
}
