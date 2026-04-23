// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareUpgradeableFacilities.ShareUpgradeableFacilitiesSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using Upgradeables;

namespace LmpClient.Systems.ShareUpgradeableFacilities
{
  public class ShareUpgradeableFacilitiesSystem : 
    ShareProgressBaseSystem<ShareUpgradeableFacilitiesSystem, ShareUpgradeableFacilitiesMessageSender, ShareUpgradeableFacilitiesMessageHandler>
  {
    public override string SystemName { get; } = nameof (ShareUpgradeableFacilitiesSystem);

    private ShareUpgradeableFacilitiesEvents ShareUpgradeableFacilitiesEvents { get; } = new ShareUpgradeableFacilitiesEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnKSCFacilityUpgrading.Add(new EventData<UpgradeableFacility, int>.OnEvent((object) this.ShareUpgradeableFacilitiesEvents, __methodptr(FacilityUpgraded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnKSCFacilityUpgrading.Remove(new EventData<UpgradeableFacility, int>.OnEvent((object) this.ShareUpgradeableFacilitiesEvents, __methodptr(FacilityUpgraded)));
    }
  }
}
