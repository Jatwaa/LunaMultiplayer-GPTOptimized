// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselPartSyncFieldMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;
using System;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselPartSyncFieldMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public string ModuleName;
    public string FieldName;
    public PartSyncFieldType FieldType;
    public string StrValue;
    public bool BoolValue;
    public short ShortValue;
    public ushort UShortValue;
    public int IntValue;
    public uint UIntValue;
    public long LongValue;
    public ulong ULongValue;
    public float FloatValue;
    public double DoubleValue;
    public float[] VectorValue = new float[3];
    public float[] QuaternionValue = new float[4];

    internal VesselPartSyncFieldMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.PartSyncField;

    public override string ClassName { get; } = nameof (VesselPartSyncFieldMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartFlightId);
      lidgrenMsg.Write(this.ModuleName);
      lidgrenMsg.Write(this.FieldName);
      lidgrenMsg.Write((byte) this.FieldType);
      switch (this.FieldType)
      {
        case PartSyncFieldType.Boolean:
          lidgrenMsg.Write(this.BoolValue);
          break;
        case PartSyncFieldType.Short:
          lidgrenMsg.Write(this.ShortValue);
          break;
        case PartSyncFieldType.UShort:
          lidgrenMsg.Write(this.UShortValue);
          break;
        case PartSyncFieldType.Integer:
          lidgrenMsg.Write(this.IntValue);
          break;
        case PartSyncFieldType.UInteger:
          lidgrenMsg.Write(this.UIntValue);
          break;
        case PartSyncFieldType.Float:
          lidgrenMsg.Write(this.FloatValue);
          break;
        case PartSyncFieldType.Long:
          lidgrenMsg.Write(this.LongValue);
          break;
        case PartSyncFieldType.ULong:
          lidgrenMsg.Write(this.ULongValue);
          break;
        case PartSyncFieldType.Double:
          lidgrenMsg.Write(this.DoubleValue);
          break;
        case PartSyncFieldType.Vector2:
          for (int index = 0; index < 2; ++index)
            lidgrenMsg.Write(this.VectorValue[index]);
          break;
        case PartSyncFieldType.Vector3:
          for (int index = 0; index < 3; ++index)
            lidgrenMsg.Write(this.VectorValue[index]);
          break;
        case PartSyncFieldType.Quaternion:
          for (int index = 0; index < 4; ++index)
            lidgrenMsg.Write(this.QuaternionValue[index]);
          break;
        case PartSyncFieldType.String:
        case PartSyncFieldType.Object:
          lidgrenMsg.Write(this.StrValue);
          break;
        case PartSyncFieldType.Enum:
          lidgrenMsg.Write(this.IntValue);
          lidgrenMsg.Write(this.StrValue);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartFlightId = lidgrenMsg.ReadUInt32();
      this.ModuleName = lidgrenMsg.ReadString();
      this.FieldName = lidgrenMsg.ReadString();
      this.FieldType = (PartSyncFieldType) lidgrenMsg.ReadByte();
      switch (this.FieldType)
      {
        case PartSyncFieldType.Boolean:
          this.BoolValue = lidgrenMsg.ReadBoolean();
          break;
        case PartSyncFieldType.Short:
          this.ShortValue = lidgrenMsg.ReadInt16();
          break;
        case PartSyncFieldType.UShort:
          this.UShortValue = lidgrenMsg.ReadUInt16();
          break;
        case PartSyncFieldType.Integer:
          this.IntValue = lidgrenMsg.ReadInt32();
          break;
        case PartSyncFieldType.UInteger:
          this.UIntValue = lidgrenMsg.ReadUInt32();
          break;
        case PartSyncFieldType.Float:
          this.FloatValue = lidgrenMsg.ReadFloat();
          break;
        case PartSyncFieldType.Long:
          this.LongValue = lidgrenMsg.ReadInt64();
          break;
        case PartSyncFieldType.ULong:
          this.ULongValue = lidgrenMsg.ReadUInt64();
          break;
        case PartSyncFieldType.Double:
          this.DoubleValue = lidgrenMsg.ReadDouble();
          break;
        case PartSyncFieldType.Vector2:
          for (int index = 0; index < 2; ++index)
            this.VectorValue[index] = lidgrenMsg.ReadFloat();
          break;
        case PartSyncFieldType.Vector3:
          for (int index = 0; index < 3; ++index)
            this.VectorValue[index] = lidgrenMsg.ReadFloat();
          break;
        case PartSyncFieldType.Quaternion:
          for (int index = 0; index < 4; ++index)
            this.VectorValue[index] = lidgrenMsg.ReadFloat();
          break;
        case PartSyncFieldType.String:
        case PartSyncFieldType.Object:
          this.StrValue = lidgrenMsg.ReadString();
          break;
        case PartSyncFieldType.Enum:
          this.IntValue = lidgrenMsg.ReadInt32();
          this.StrValue = lidgrenMsg.ReadString();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = base.InternalGetMessageSize() + 4 + this.ModuleName.GetByteCount() + this.FieldName.GetByteCount();
      int messageSize;
      switch (this.FieldType)
      {
        case PartSyncFieldType.Boolean:
          messageSize = num + 1;
          break;
        case PartSyncFieldType.Short:
          messageSize = num + 2;
          break;
        case PartSyncFieldType.UShort:
          messageSize = num + 2;
          break;
        case PartSyncFieldType.Integer:
          messageSize = num + 4;
          break;
        case PartSyncFieldType.UInteger:
          messageSize = num + 4;
          break;
        case PartSyncFieldType.Float:
          messageSize = num + 4;
          break;
        case PartSyncFieldType.Long:
          messageSize = num + 8;
          break;
        case PartSyncFieldType.ULong:
          messageSize = num + 8;
          break;
        case PartSyncFieldType.Double:
          messageSize = num + 8;
          break;
        case PartSyncFieldType.Vector2:
          messageSize = num + 8;
          break;
        case PartSyncFieldType.Vector3:
          messageSize = num + 12;
          break;
        case PartSyncFieldType.Quaternion:
          messageSize = num + 16;
          break;
        case PartSyncFieldType.String:
          messageSize = num + this.StrValue.GetByteCount();
          break;
        case PartSyncFieldType.Enum:
          messageSize = num + (4 + this.StrValue.GetByteCount());
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return messageSize;
    }
  }
}
