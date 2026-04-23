// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Scenario.ScenarioMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Scenario;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Systems.Scenario
{
  public class ScenarioMessageSender : SubSystem<ScenarioSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ScenarioCliMsg>(msg))));

    public void SendScenariosRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<ScenarioCliMsg, ScenarioRequestMsgData>())));

    public void SendScenarioModuleData(List<string> scenarioNames, List<byte[]> scenarioData)
    {
      ScenarioDataMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ScenarioDataMsgData>();
      ScenarioInfo[] array = scenarioNames.Select<string, ScenarioInfo>((Func<string, int, ScenarioInfo>) ((t, i) => new ScenarioInfo()
      {
        Data = scenarioData[i],
        NumBytes = scenarioData[i].Length,
        Module = t
      })).ToArray<ScenarioInfo>();
      newMessageData.ScenariosData = array;
      newMessageData.ScenarioCount = array.Length;
      LunaLog.Log(string.Format("[LMP]: Sending {0} scenario modules", (object) newMessageData.ScenarioCount));
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
