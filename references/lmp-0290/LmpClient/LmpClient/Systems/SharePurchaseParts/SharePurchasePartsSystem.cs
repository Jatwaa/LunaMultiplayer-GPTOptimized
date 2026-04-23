// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SharePurchaseParts.SharePurchasePartsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;

namespace LmpClient.Systems.SharePurchaseParts
{
  public class SharePurchasePartsSystem : 
    ShareProgressBaseSystem<SharePurchasePartsSystem, SharePurchasePartsMessageSender, SharePurchasePartsMessageHandler>
  {
    public override string SystemName { get; } = nameof (SharePurchasePartsSystem);

    private SharePurchasePartsEvents SharePurchasePartsEvents { get; } = new SharePurchasePartsEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant || HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch)
        return;
      // ISSUE: method pointer
      GameEvents.OnPartPurchased.Add(new EventData<AvailablePart>.OnEvent((object) this.SharePurchasePartsEvents, __methodptr(PartPurchased)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnPartPurchased.Remove(new EventData<AvailablePart>.OnEvent((object) this.SharePurchasePartsEvents, __methodptr(PartPurchased)));
    }
  }
}
