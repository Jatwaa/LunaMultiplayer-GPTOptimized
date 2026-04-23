// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.VesselPartSyncFieldMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpCommon.Enums;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class VesselPartSyncFieldMessageSender : 
    SubSystem<VesselPartSyncFieldSystem>,
    IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselPartSyncFieldBoolMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      bool value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Boolean;
      baseMsg.BoolValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldShortMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      short value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Short;
      baseMsg.ShortValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldUshortMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      ushort value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.UShort;
      baseMsg.UShortValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldIntMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      int value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Integer;
      baseMsg.IntValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldUIntMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      uint value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.UInteger;
      baseMsg.UIntValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldFloatMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      float value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Float;
      baseMsg.FloatValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldLongMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      long value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Long;
      baseMsg.LongValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldULongMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      ulong value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.ULong;
      baseMsg.ULongValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldDoubleMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      double value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Double;
      baseMsg.DoubleValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldVector2Msg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      Vector2 value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Vector2;
      baseMsg.VectorValue[0] = value.x;
      baseMsg.VectorValue[1] = value.y;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldVector3Msg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      Vector3 value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Vector3;
      baseMsg.VectorValue[0] = value.x;
      baseMsg.VectorValue[1] = value.y;
      baseMsg.VectorValue[2] = value.z;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldQuaternionMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      Quaternion value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Quaternion;
      baseMsg.QuaternionValue[0] = value.x;
      baseMsg.QuaternionValue[1] = value.y;
      baseMsg.QuaternionValue[2] = value.z;
      baseMsg.QuaternionValue[3] = value.w;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldStringMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      string value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.String;
      baseMsg.StrValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldObjectMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      object value)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.String;
      baseMsg.StrValue = value.ToString();
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncFieldEnumMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      int value,
      string valueStr)
    {
      VesselPartSyncFieldMsgData baseMsg = VesselPartSyncFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Enum;
      baseMsg.IntValue = value;
      baseMsg.StrValue = valueStr;
      this.SendMessage((IMessageData) baseMsg);
    }

    private static VesselPartSyncFieldMsgData GetBaseMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field)
    {
      VesselPartSyncFieldMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselPartSyncFieldMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = part.flightID;
      newMessageData.ModuleName = moduleName;
      newMessageData.FieldName = field;
      return newMessageData;
    }
  }
}
