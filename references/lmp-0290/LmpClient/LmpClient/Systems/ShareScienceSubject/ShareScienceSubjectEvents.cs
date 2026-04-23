// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScienceSubject.ShareScienceSubjectEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareScienceSubject
{
  public class ShareScienceSubjectEvents : SubSystem<ShareScienceSubjectSystem>
  {
    public void ScienceRecieved(
      float dataAmount,
      ScienceSubject subject,
      ProtoVessel source,
      bool reverseEngineered)
    {
      if (SubSystem<ShareScienceSubjectSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareScienceSubjectSystem>.System.MessageSender.SendScienceSubjectMessage(subject);
    }

    public void RevertingDetected()
    {
      SubSystem<ShareScienceSubjectSystem>.System.Reverting = true;
      SubSystem<ShareScienceSubjectSystem>.System.StartIgnoringEvents();
    }

    public void RevertingToEditorDetected(EditorFacility data)
    {
      SubSystem<ShareScienceSubjectSystem>.System.Reverting = true;
      SubSystem<ShareScienceSubjectSystem>.System.StartIgnoringEvents();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (!SubSystem<ShareScienceSubjectSystem>.System.Reverting)
        return;
      SubSystem<ShareScienceSubjectSystem>.System.Reverting = false;
      SubSystem<ShareScienceSubjectSystem>.System.StopIgnoringEvents(true);
    }
  }
}
