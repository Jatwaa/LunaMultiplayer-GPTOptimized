// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareTechnology.ShareTechnologyMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.ShareTechnology
{
  public class ShareTechnologyMessageSender : SubSystem<ShareTechnologySystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendTechnologyMessage(RDTech tech)
    {
      ShareProgressTechnologyMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressTechnologyMsgData>();
      newMessageData.TechNode.Id = tech.techID;
      ConfigNode configNode = ShareTechnologyMessageSender.ConvertTechNodeToConfigNode(tech);
      if (configNode == null)
        return;
      byte[] sourceArray = configNode.Serialize();
      int length = sourceArray.Length;
      newMessageData.TechNode.NumBytes = length;
      if (newMessageData.TechNode.Data.Length < length)
        newMessageData.TechNode.Data = new byte[length];
      Array.Copy((Array) sourceArray, (Array) newMessageData.TechNode.Data, length);
      this.SendMessage((IMessageData) newMessageData);
    }

    private static ConfigNode ConvertTechNodeToConfigNode(RDTech techNode)
    {
      ConfigNode configNode = new ConfigNode();
      try
      {
        configNode.AddValue("id", techNode.techID);
        configNode.AddValue("state", (object) techNode.state);
        configNode.AddValue("cost", techNode.scienceCost);
        foreach (AvailablePart availablePart in techNode.partsPurchased)
          configNode.AddValue("part", availablePart.name);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving tech node: {0}", (object) ex));
        return (ConfigNode) null;
      }
      return configNode;
    }
  }
}
