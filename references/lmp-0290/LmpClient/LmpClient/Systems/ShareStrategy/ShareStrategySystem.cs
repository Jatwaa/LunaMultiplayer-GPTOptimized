// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareStrategy.ShareStrategySystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using Strategies;
using UnityEngine;

namespace LmpClient.Systems.ShareStrategy
{
  public class ShareStrategySystem : 
    ShareProgressBaseSystem<ShareStrategySystem, ShareStrategyMessageSender, ShareStrategyMessageHandler>
  {
    public readonly string[] OneTimeStrategies = new string[2]
    {
      "BailoutGrant",
      "researchIPsellout"
    };

    public override string SystemName { get; } = nameof (ShareStrategySystem);

    private ShareStrategyEvents ShareStrategiesEvents { get; } = new ShareStrategyEvents();

    protected override bool ShareSystemReady => Object.op_Inequality((Object) StrategySystem.Instance, (Object) null) && StrategySystem.Instance.Strategies.Count != 0 && Object.op_Inequality((Object) Funding.Instance, (Object) null) && Object.op_Inequality((Object) ResearchAndDevelopment.Instance, (Object) null) && Object.op_Inequality((Object) Reputation.Instance, (Object) null) && (double) Time.timeSinceLevelLoad > 1.0;

    protected override GameMode RelevantGameModes => GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      StrategyEvent.onStrategyActivated.Add(new EventData<Strategy>.OnEvent((object) this.ShareStrategiesEvents, __methodptr(StrategyActivated)));
      // ISSUE: method pointer
      StrategyEvent.onStrategyDeactivated.Add(new EventData<Strategy>.OnEvent((object) this.ShareStrategiesEvents, __methodptr(StrategyDeactivated)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      StrategyEvent.onStrategyActivated.Remove(new EventData<Strategy>.OnEvent((object) this.ShareStrategiesEvents, __methodptr(StrategyActivated)));
      // ISSUE: method pointer
      StrategyEvent.onStrategyDeactivated.Remove(new EventData<Strategy>.OnEvent((object) this.ShareStrategiesEvents, __methodptr(StrategyDeactivated)));
    }
  }
}
