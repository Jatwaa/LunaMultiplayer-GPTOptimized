// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncFieldQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Vessel;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncFieldQueue : 
    CachedConcurrentQueue<VesselPartSyncField, VesselPartSyncFieldMsgData>
  {
    protected override void AssignFromMessage(
      VesselPartSyncField value,
      VesselPartSyncFieldMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.PartFlightId = msgData.PartFlightId;
      value.ModuleName = msgData.ModuleName.Clone() as string;
      value.FieldName = msgData.FieldName.Clone() as string;
      value.FieldType = msgData.FieldType;
      switch (value.FieldType)
      {
        case PartSyncFieldType.Boolean:
          value.BoolValue = msgData.BoolValue;
          break;
        case PartSyncFieldType.Integer:
          value.IntValue = msgData.IntValue;
          break;
        case PartSyncFieldType.Float:
          value.FloatValue = msgData.FloatValue;
          break;
        case PartSyncFieldType.Double:
          value.DoubleValue = msgData.DoubleValue;
          break;
        case PartSyncFieldType.Vector2:
          value.Vector2Value = new Vector2(msgData.VectorValue[0], msgData.VectorValue[1]);
          break;
        case PartSyncFieldType.Vector3:
          value.Vector3Value = new Vector3(msgData.VectorValue[0], msgData.VectorValue[1], msgData.VectorValue[2]);
          break;
        case PartSyncFieldType.Quaternion:
          value.QuaternionValue = new Quaternion(msgData.QuaternionValue[0], msgData.QuaternionValue[1], msgData.QuaternionValue[2], msgData.QuaternionValue[3]);
          break;
        case PartSyncFieldType.String:
        case PartSyncFieldType.Object:
          value.StrValue = msgData.StrValue.Clone() as string;
          break;
        case PartSyncFieldType.Enum:
          value.IntValue = msgData.IntValue;
          value.StrValue = msgData.StrValue.Clone() as string;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
