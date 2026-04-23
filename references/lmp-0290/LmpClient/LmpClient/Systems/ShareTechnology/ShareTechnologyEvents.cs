// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareTechnology.ShareTechnologyEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareTechnology
{
  public class ShareTechnologyEvents : SubSystem<ShareTechnologySystem>
  {
    public void TechnologyResearched(
      GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> data)
    {
      if (SubSystem<ShareTechnologySystem>.System.IgnoreEvents || data.target > 0)
        return;
      LunaLog.Log("Relaying researched technology: " + data.host.techID);
      SubSystem<ShareTechnologySystem>.System.MessageSender.SendTechnologyMessage(data.host);
    }
  }
}
