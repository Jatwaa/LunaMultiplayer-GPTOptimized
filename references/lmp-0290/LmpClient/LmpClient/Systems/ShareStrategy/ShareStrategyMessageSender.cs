// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareStrategy.ShareStrategyMessageSender
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
using Strategies;
using System;

namespace LmpClient.Systems.ShareStrategy
{
  public class ShareStrategyMessageSender : SubSystem<ShareStrategySystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendStrategyMessage(Strategy strategy)
    {
      ShareProgressStrategyMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressStrategyMsgData>();
      newMessageData.Strategy.Name = strategy.Config.Name;
      ConfigNode configNode = ShareStrategyMessageSender.ConvertStrategyToConfigNode(strategy);
      if (configNode == null)
        return;
      byte[] sourceArray = configNode.Serialize();
      int length = sourceArray.Length;
      newMessageData.Strategy.NumBytes = length;
      if (newMessageData.Strategy.Data.Length < length)
        newMessageData.Strategy.Data = new byte[length];
      Array.Copy((Array) sourceArray, (Array) newMessageData.Strategy.Data, length);
      this.SendMessage((IMessageData) newMessageData);
    }

    private static ConfigNode ConvertStrategyToConfigNode(Strategy strategy)
    {
      ConfigNode configNode = new ConfigNode();
      try
      {
        strategy.Save(configNode);
        configNode.AddValue("isActive", strategy.IsActive);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving strategy: {0}", (object) ex));
        return (ConfigNode) null;
      }
      return configNode;
    }
  }
}
