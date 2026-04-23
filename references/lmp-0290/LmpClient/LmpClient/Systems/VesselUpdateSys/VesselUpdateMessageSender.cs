// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUpdateSys.VesselUpdateMessageSender
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
using UnityEngine;

namespace LmpClient.Systems.VesselUpdateSys
{
  public class VesselUpdateMessageSender : SubSystem<VesselUpdateSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselUpdate(global::Vessel vessel)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      VesselUpdateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselUpdateMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      newMessageData.Name = vessel.vesselName;
      newMessageData.Type = vessel.vesselType.ToString();
      newMessageData.DistanceTraveled = vessel.distanceTraveled;
      newMessageData.Situation = vessel.situation.ToString();
      newMessageData.Landed = vessel.Landed;
      newMessageData.LandedAt = vessel.landedAt;
      newMessageData.DisplayLandedAt = vessel.displaylandedAt;
      newMessageData.Splashed = vessel.Splashed;
      newMessageData.MissionTime = vessel.missionTime;
      newMessageData.LaunchTime = vessel.launchTime;
      newMessageData.LastUt = vessel.lastUT;
      newMessageData.Persistent = vessel.isPersistent;
      newMessageData.RefTransformId = vessel.referenceTransformId;
      newMessageData.AutoClean = vessel.AutoClean;
      newMessageData.AutoCleanReason = vessel.AutoCleanReason;
      newMessageData.WasControllable = vessel.IsControllable;
      newMessageData.Stage = vessel.currentStage;
      newMessageData.Com[0] = vessel.localCoM.x;
      newMessageData.Com[1] = vessel.localCoM.y;
      newMessageData.Com[2] = vessel.localCoM.z;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
