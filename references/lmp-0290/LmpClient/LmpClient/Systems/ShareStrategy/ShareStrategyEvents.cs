// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareStrategy.ShareStrategyEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using Strategies;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Systems.ShareStrategy
{
  public class ShareStrategyEvents : SubSystem<ShareStrategySystem>
  {
    public void StrategyActivated(Strategy strategy)
    {
      if (SubSystem<ShareStrategySystem>.System.IgnoreEvents || ((IEnumerable<string>) SubSystem<ShareStrategySystem>.System.OneTimeStrategies).Contains<string>(strategy.Config.Name))
        return;
      LunaLog.Log(string.Format("Relaying strategy activation: {0} - with factor: {1}", (object) strategy.Config.Name, (object) strategy.Factor));
      SubSystem<ShareStrategySystem>.System.MessageSender.SendStrategyMessage(strategy);
    }

    public void StrategyDeactivated(Strategy strategy)
    {
      if (SubSystem<ShareStrategySystem>.System.IgnoreEvents || ((IEnumerable<string>) SubSystem<ShareStrategySystem>.System.OneTimeStrategies).Contains<string>(strategy.Config.Name))
        return;
      LunaLog.Log(string.Format("Relaying strategy deactivation: {0} - with factor: {1}", (object) strategy.Config.Name, (object) strategy.Factor));
      SubSystem<ShareStrategySystem>.System.MessageSender.SendStrategyMessage(strategy);
    }
  }
}
