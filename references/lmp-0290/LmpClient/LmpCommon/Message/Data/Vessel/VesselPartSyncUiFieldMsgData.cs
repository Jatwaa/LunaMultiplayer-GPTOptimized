// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselPartSyncUiFieldMsgData
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
  public class VesselPartSyncUiFieldMsgData : VesselBaseMsgData
  {
    public uint PartFlightId;
    public string ModuleName;
    public string FieldName;
    public PartSyncFieldType FieldType;
    public bool BoolValue;
    public int IntValue;
    public float FloatValue;

    internal VesselPartSyncUiFieldMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.PartSyncUiField;

    public override string ClassName { get; } = nameof (VesselPartSyncUiFieldMsgData);

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
        case PartSyncFieldType.Integer:
          lidgrenMsg.Write(this.IntValue);
          break;
        case PartSyncFieldType.Float:
          lidgrenMsg.Write(this.FloatValue);
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
        case PartSyncFieldType.Integer:
          this.IntValue = lidgrenMsg.ReadInt32();
          break;
        case PartSyncFieldType.Float:
          this.FloatValue = lidgrenMsg.ReadFloat();
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
        case PartSyncFieldType.Integer:
          messageSize = num + 4;
          break;
        case PartSyncFieldType.Float:
          messageSize = num + 4;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return messageSize;
    }
  }
}
