// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareAchievements.ShareAchievementsEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareAchievements
{
  public class ShareAchievementsEvents : SubSystem<ShareAchievementsSystem>
  {
    public void AchievementReached(ProgressNode progressNode)
    {
      if (SubSystem<ShareAchievementsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareAchievementsSystem>.System.MessageSender.SendAchievementsMessage(progressNode);
      LunaLog.Log("Achievement reached: " + progressNode.Id);
    }

    public void AchievementCompleted(ProgressNode progressNode)
    {
      if (SubSystem<ShareAchievementsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareAchievementsSystem>.System.MessageSender.SendAchievementsMessage(progressNode);
      LunaLog.Log("Achievement completed: " + progressNode.Id);
    }

    public void AchievementAchieved(ProgressNode progressNode)
    {
    }

    public void RevertingDetected()
    {
      SubSystem<ShareAchievementsSystem>.System.Reverting = true;
      SubSystem<ShareAchievementsSystem>.System.StartIgnoringEvents();
    }

    public void RevertingToEditorDetected(EditorFacility data)
    {
      SubSystem<ShareAchievementsSystem>.System.Reverting = true;
      SubSystem<ShareAchievementsSystem>.System.StartIgnoringEvents();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (!SubSystem<ShareAchievementsSystem>.System.Reverting)
        return;
      SubSystem<ShareAchievementsSystem>.System.Reverting = false;
      SubSystem<ShareAchievementsSystem>.System.StopIgnoringEvents(true);
    }
  }
}
