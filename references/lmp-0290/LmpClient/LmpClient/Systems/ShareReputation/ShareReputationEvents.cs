// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareReputation.ShareReputationEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareReputation
{
  public class ShareReputationEvents : SubSystem<ShareReputationSystem>
  {
    public void ReputationChanged(float reputation, TransactionReasons reason)
    {
      if (SubSystem<ShareReputationSystem>.System.IgnoreEvents)
        return;
      LunaLog.Log(string.Format("Reputation changed to: {0} reason: {1}", (object) reputation, (object) reason));
      SubSystem<ShareReputationSystem>.System.MessageSender.SendReputationMsg(reputation, reason.ToString());
    }

    public void RevertingDetected()
    {
      SubSystem<ShareReputationSystem>.System.Reverting = true;
      SubSystem<ShareReputationSystem>.System.StartIgnoringEvents();
    }

    public void RevertingToEditorDetected(EditorFacility data)
    {
      SubSystem<ShareReputationSystem>.System.Reverting = true;
      SubSystem<ShareReputationSystem>.System.StartIgnoringEvents();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (!SubSystem<ShareReputationSystem>.System.Reverting)
        return;
      SubSystem<ShareReputationSystem>.System.Reverting = false;
      SubSystem<ShareReputationSystem>.System.StopIgnoringEvents(true);
    }
  }
}
