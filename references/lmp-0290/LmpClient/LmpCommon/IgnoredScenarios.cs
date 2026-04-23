// Decompiled with JetBrains decompiler
// Type: LmpCommon.IgnoredScenarios
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections.Generic;

namespace LmpCommon
{
  public class IgnoredScenarios
  {
    public static List<string> IgnoreReceive { get; } = new List<string>()
    {
      "ScenarioDiscoverableObjects",
      "ScenarioCustomWaypoints"
    };

    public static List<string> IgnoreSend { get; } = new List<string>()
    {
      "ScenarioNewGameIntro",
      "ScenarioDiscoverableObjects",
      "ScenarioCustomWaypoints",
      "ContractSystem",
      "Funding",
      "ProgressTracking",
      "Reputation",
      "ResearchAndDevelopment",
      "ScenarioDestructibles",
      "ScenarioUpgradeableFacilities",
      "StrategySystem"
    };
  }
}
