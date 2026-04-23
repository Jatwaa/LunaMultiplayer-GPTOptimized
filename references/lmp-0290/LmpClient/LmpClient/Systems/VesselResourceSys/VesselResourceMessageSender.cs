// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselResourceSys.VesselResourceMessageSender
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
using System.Collections.Generic;

namespace LmpClient.Systems.VesselResourceSys
{
  public class VesselResourceMessageSender : SubSystem<VesselResourceSystem>, IMessageSender
  {
    private static readonly List<VesselResourceInfo> Resources = new List<VesselResourceInfo>();

    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselResources(global::Vessel vessel)
    {
      int index1 = 0;
      VesselResourceMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselResourceMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      newMessageData.VesselId = vessel.id;
      for (int index2 = 0; index2 < vessel.protoVessel.protoPartSnapshots.Count; ++index2)
      {
        if (vessel.protoVessel.protoPartSnapshots[index2]?.resources != null)
        {
          for (int index3 = 0; index3 < vessel.protoVessel.protoPartSnapshots[index2].resources.Count; ++index3)
          {
            PartResource resourceRef = vessel.protoVessel.protoPartSnapshots[index2].resources[index3]?.resourceRef;
            if (resourceRef != null)
            {
              if (VesselResourceMessageSender.Resources.Count > index1)
              {
                VesselResourceMessageSender.Resources[index1].ResourceName = resourceRef.resourceName;
                VesselResourceMessageSender.Resources[index1].PartFlightId = vessel.protoVessel.protoPartSnapshots[index2].flightID;
                VesselResourceMessageSender.Resources[index1].Amount = resourceRef.amount;
                VesselResourceMessageSender.Resources[index1].FlowState = resourceRef.flowState;
              }
              else
                VesselResourceMessageSender.Resources.Add(new VesselResourceInfo()
                {
                  ResourceName = resourceRef.resourceName,
                  PartFlightId = vessel.protoVessel.protoPartSnapshots[index2].flightID,
                  Amount = resourceRef.amount,
                  FlowState = resourceRef.flowState
                });
              ++index1;
            }
          }
        }
      }
      newMessageData.ResourcesCount = index1;
      if (newMessageData.Resources.Length < index1)
        newMessageData.Resources = new VesselResourceInfo[index1];
      for (int index4 = 0; index4 < index1; ++index4)
      {
        if (newMessageData.Resources[index4] == null)
          newMessageData.Resources[index4] = new VesselResourceInfo(VesselResourceMessageSender.Resources[index4]);
        else
          newMessageData.Resources[index4].CopyFrom(VesselResourceMessageSender.Resources[index4]);
      }
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
