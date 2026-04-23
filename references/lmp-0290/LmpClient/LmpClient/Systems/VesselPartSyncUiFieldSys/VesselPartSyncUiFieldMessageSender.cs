// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncUiFieldSys.VesselPartSyncUiFieldMessageSender
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

namespace LmpClient.Systems.VesselPartSyncUiFieldSys
{
  public class VesselPartSyncUiFieldMessageSender : 
    SubSystem<VesselPartSyncUiFieldSystem>,
    IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselPartSyncUiFieldBoolMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      bool value)
    {
      VesselPartSyncUiFieldMsgData baseMsg = VesselPartSyncUiFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Boolean;
      baseMsg.BoolValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncUiFieldIntMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      int value)
    {
      VesselPartSyncUiFieldMsgData baseMsg = VesselPartSyncUiFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Integer;
      baseMsg.IntValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    public void SendVesselPartSyncUiFieldFloatMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field,
      float value)
    {
      VesselPartSyncUiFieldMsgData baseMsg = VesselPartSyncUiFieldMessageSender.GetBaseMsg(vessel, part, moduleName, field);
      baseMsg.FieldType = PartSyncFieldType.Float;
      baseMsg.FloatValue = value;
      this.SendMessage((IMessageData) baseMsg);
    }

    private static VesselPartSyncUiFieldMsgData GetBaseMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string field)
    {
      VesselPartSyncUiFieldMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselPartSyncUiFieldMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = part.flightID;
      newMessageData.ModuleName = moduleName;
      newMessageData.FieldName = field;
      return newMessageData;
    }
  }
}
