// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareTechnology.ShareTechnologySystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Systems.ShareTechnology
{
  public class ShareTechnologySystem : 
    ShareProgressBaseSystem<ShareTechnologySystem, ShareTechnologyMessageSender, ShareTechnologyMessageHandler>
  {
    public override string SystemName { get; } = nameof (ShareTechnologySystem);

    private ShareTechnologyEvents ShareTechnologyEvents { get; } = new ShareTechnologyEvents();

    protected override bool ShareSystemReady => Object.op_Inequality((Object) ResearchAndDevelopment.Instance, (Object) null);

    protected override GameMode RelevantGameModes => GameMode.Science | GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnTechnologyResearched.Add(new EventData<GameEvents.HostTargetAction<RDTech, RDTech.OperationResult>>.OnEvent((object) this.ShareTechnologyEvents, __methodptr(TechnologyResearched)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnTechnologyResearched.Remove(new EventData<GameEvents.HostTargetAction<RDTech, RDTech.OperationResult>>.OnEvent((object) this.ShareTechnologyEvents, __methodptr(TechnologyResearched)));
    }
  }
}
