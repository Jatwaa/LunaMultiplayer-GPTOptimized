// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiFieldQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Vessel;
using System;

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiFieldQueue : 
    CachedConcurrentQueue<VesselPartSyncUiField, VesselPartSyncUiFieldMsgData>
  {
    protected override void AssignFromMessage(
      VesselPartSyncUiField value,
      VesselPartSyncUiFieldMsgData msgData)
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
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
