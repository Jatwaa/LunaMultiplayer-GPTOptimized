// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Scenario.ScenarioMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Scenario;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Scenario
{
  public class ScenarioMessageHandler : SubSystem<ScenarioSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ScenarioBaseMsgData data))
        return;
      switch (data.ScenarioMessageType)
      {
        case ScenarioMessageType.Data:
          ScenarioMessageHandler.QueueAllReceivedScenarios(data);
          break;
        case ScenarioMessageType.Proto:
          ScenarioProtoMsgData scenarioProtoMsgData = (ScenarioProtoMsgData) data;
          ScenarioMessageHandler.QueueScenarioBytes(scenarioProtoMsgData.ScenarioData.Module, scenarioProtoMsgData.ScenarioData.Data, scenarioProtoMsgData.ScenarioData.NumBytes);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static void QueueAllReceivedScenarios(ScenarioBaseMsgData msgData)
    {
      ScenarioDataMsgData scenarioDataMsgData = (ScenarioDataMsgData) msgData;
      for (int index = 0; index < scenarioDataMsgData.ScenarioCount; ++index)
        ScenarioMessageHandler.QueueScenarioBytes(scenarioDataMsgData.ScenariosData[index].Module, scenarioDataMsgData.ScenariosData[index].Data, scenarioDataMsgData.ScenariosData[index].NumBytes);
      if (MainSystem.NetworkState >= ClientState.ScenariosSynced)
        return;
      MainSystem.NetworkState = ClientState.ScenariosSynced;
    }

    private static void QueueScenarioBytes(
      string scenarioModule,
      byte[] scenarioData,
      int numBytes)
    {
      ConfigNode configNode = scenarioData.DeserializeToConfigNode(numBytes);
      if (configNode != null)
        SubSystem<ScenarioSystem>.System.ScenarioQueue.Enqueue(new ScenarioEntry()
        {
          ScenarioModule = scenarioModule,
          ScenarioNode = configNode
        });
      else
        LunaLog.LogError("[LMP]: Scenario data has been lost for " + scenarioModule);
    }
  }
}
