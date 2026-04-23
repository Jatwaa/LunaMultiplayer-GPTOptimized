// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareUpgradeableFacilities.ShareUpgradeableFacilitiesEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using Upgradeables;

namespace LmpClient.Systems.ShareUpgradeableFacilities
{
  public class ShareUpgradeableFacilitiesEvents : SubSystem<ShareUpgradeableFacilitiesSystem>
  {
    public void FacilityUpgraded(UpgradeableFacility facility, int level)
    {
      if (SubSystem<ShareUpgradeableFacilitiesSystem>.System.IgnoreEvents)
        return;
      LunaLog.Log(string.Format("Facility {0} upgraded to level: {1}", (object) ((UpgradeableObject) facility).id, (object) level));
      SubSystem<ShareUpgradeableFacilitiesSystem>.System.MessageSender.SendFacilityUpgradeMessage(((UpgradeableObject) facility).id, level, (float) level / (float) ((UpgradeableObject) facility).MaxLevel);
    }
  }
}
