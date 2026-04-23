// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareStrategy.ShareStrategyMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Systems.ShareFunds;
using LmpClient.Systems.ShareReputation;
using LmpClient.Systems.ShareScience;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using Strategies;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using UnityEngine;

namespace LmpClient.Systems.ShareStrategy
{
  public class ShareStrategyMessageHandler : SubSystem<ShareStrategySystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.StrategyUpdate || !(data is ShareProgressStrategyMsgData progressStrategyMsgData))
        return;
      StrategyInfo strategy = new StrategyInfo(progressStrategyMsgData.Strategy);
      LunaLog.Log("Queue StrategyUpdate with: " + strategy.Name);
      SubSystem<ShareStrategySystem>.System.QueueAction((Action) (() => ShareStrategyMessageHandler.StrategyUpdate(strategy)));
    }

    private static void StrategyUpdate(StrategyInfo strategyInfo)
    {
      ConfigNode configNode = ShareStrategyMessageHandler.ConvertByteArrayToConfigNode(strategyInfo.Data, strategyInfo.NumBytes);
      if (configNode == null)
        return;
      float num = float.Parse(configNode.GetValue("factor"), (IFormatProvider) CultureInfo.InvariantCulture);
      bool flag = bool.Parse(configNode.GetValue("isActive"));
      SubSystem<ShareStrategySystem>.System.StartIgnoringEvents();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StartIgnoringEvents();
      int index = StrategySystem.Instance.Strategies.FindIndex((Predicate<Strategy>) (s => s.Config.Name == strategyInfo.Name));
      if (index != -1)
      {
        if (flag)
        {
          StrategySystem.Instance.Strategies[index].Factor = num;
          StrategySystem.Instance.Strategies[index].Activate();
          LunaLog.Log(string.Format("StrategyUpdate received - strategy activated: {0}  - with factor: {1}", (object) strategyInfo.Name, (object) num));
        }
        else
        {
          StrategySystem.Instance.Strategies[index].Factor = num;
          StrategySystem.Instance.Strategies[index].Deactivate();
          LunaLog.Log(string.Format("StrategyUpdate received - strategy deactivated: {0}  - with factor: {1}", (object) strategyInfo.Name, (object) num));
        }
      }
      if (Object.op_Implicit((Object) Administration.Instance))
        Administration.Instance.RedrawPanels();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StopIgnoringEvents(true);
      GameEvents.Contract.onContractsListChanged.Fire();
      SubSystem<ShareStrategySystem>.System.StopIgnoringEvents();
    }

    private static ConfigNode ConvertByteArrayToConfigNode(byte[] data, int numBytes)
    {
      ConfigNode configNode;
      try
      {
        configNode = data.DeserializeToConfigNode(numBytes);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing strategy configNode: {0}", (object) ex));
        return (ConfigNode) null;
      }
      if (configNode == null)
      {
        LunaLog.LogError("[LMP]: Error, the strategy configNode was null.");
        return (ConfigNode) null;
      }
      if (configNode.HasValue("isActive"))
        return configNode;
      LunaLog.LogError("[LMP]: Error, the strategy configNode is invalid (isActive missing).");
      return (ConfigNode) null;
    }
  }
}
