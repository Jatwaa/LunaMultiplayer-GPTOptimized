// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncCallSys.VesselPartSyncCallMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;

namespace LmpClient.Systems.VesselPartSyncCallSys
{
  public class VesselPartSyncCallMessageSender : SubSystem<VesselPartSyncCallSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselPartSyncCallMsg(
      global::Vessel vessel,
      Part part,
      string moduleName,
      string methodName)
    {
      VesselPartSyncCallMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselPartSyncCallMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.PartFlightId = part.flightID;
      newMessageData.ModuleName = moduleName;
      newMessageData.MethodName = methodName;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
