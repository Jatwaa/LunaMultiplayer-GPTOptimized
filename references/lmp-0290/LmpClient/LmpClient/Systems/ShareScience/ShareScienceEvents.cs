// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScience.ShareScienceEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareScience
{
  public class ShareScienceEvents : SubSystem<ShareScienceSystem>
  {
    public void ScienceChanged(float science, TransactionReasons reason)
    {
      if (SubSystem<ShareScienceSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareScienceSystem>.System.MessageSender.SendScienceMessage(science, reason.ToString());
    }

    public void RevertingDetected()
    {
      SubSystem<ShareScienceSystem>.System.Reverting = true;
      SubSystem<ShareScienceSystem>.System.StartIgnoringEvents();
    }

    public void RevertingToEditorDetected(EditorFacility data)
    {
      SubSystem<ShareScienceSystem>.System.Reverting = true;
      SubSystem<ShareScienceSystem>.System.StartIgnoringEvents();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (!SubSystem<ShareScienceSystem>.System.Reverting)
        return;
      SubSystem<ShareScienceSystem>.System.Reverting = false;
      SubSystem<ShareScienceSystem>.System.StopIgnoringEvents(true);
    }
  }
}
