// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselPositionMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselPositionMsgData : VesselBaseMsgData
  {
    public int BodyIndex;
    public int SubspaceId;
    public float PingSec;
    public float HeightFromTerrain;
    public bool Landed;
    public bool Splashed;
    public bool HackingGravity;
    public double[] LatLonAlt = new double[3];
    public double[] VelocityVector = new double[3];
    public double[] NormalVector = new double[3];
    public float[] SrfRelRotation = new float[4];
    public double[] Orbit = new double[8];

    internal VesselPositionMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Position;

    public override string ClassName { get; } = nameof (VesselPositionMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.BodyIndex);
      lidgrenMsg.Write(this.SubspaceId);
      lidgrenMsg.Write(this.PingSec);
      lidgrenMsg.Write(this.HeightFromTerrain);
      lidgrenMsg.Write(this.Landed);
      lidgrenMsg.Write(this.Splashed);
      lidgrenMsg.Write(this.HackingGravity);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.LatLonAlt[index]);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.VelocityVector[index]);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.NormalVector[index]);
      for (int index = 0; index < 4; ++index)
        lidgrenMsg.Write(this.SrfRelRotation[index]);
      for (int index = 0; index < 8; ++index)
        lidgrenMsg.Write(this.Orbit[index]);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.BodyIndex = lidgrenMsg.ReadInt32();
      this.SubspaceId = lidgrenMsg.ReadInt32();
      this.PingSec = lidgrenMsg.ReadFloat();
      this.HeightFromTerrain = lidgrenMsg.ReadFloat();
      this.Landed = lidgrenMsg.ReadBoolean();
      this.Splashed = lidgrenMsg.ReadBoolean();
      this.HackingGravity = lidgrenMsg.ReadBoolean();
      for (int index = 0; index < 3; ++index)
        this.LatLonAlt[index] = lidgrenMsg.ReadDouble();
      for (int index = 0; index < 3; ++index)
        this.VelocityVector[index] = lidgrenMsg.ReadDouble();
      for (int index = 0; index < 3; ++index)
        this.NormalVector[index] = lidgrenMsg.ReadDouble();
      for (int index = 0; index < 4; ++index)
        this.SrfRelRotation[index] = lidgrenMsg.ReadFloat();
      for (int index = 0; index < 8; ++index)
        this.Orbit[index] = lidgrenMsg.ReadDouble();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 8 + 8 + 3 + 72 + 16 + 64;
  }
}
