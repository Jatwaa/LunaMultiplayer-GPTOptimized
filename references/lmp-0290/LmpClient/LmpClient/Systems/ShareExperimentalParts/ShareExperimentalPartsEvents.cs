// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareExperimentalParts.ShareExperimentalPartsEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.ShareExperimentalParts
{
  public class ShareExperimentalPartsEvents : SubSystem<ShareExperimentalPartsSystem>
  {
    public void ExperimentalPartRemoved(AvailablePart part, int count)
    {
      if (SubSystem<ShareExperimentalPartsSystem>.System.IgnoreEvents)
        return;
      LunaLog.Log(string.Format("Relaying experimental part added: part: {0} count: {1}", (object) part.name, (object) count));
      SubSystem<ShareExperimentalPartsSystem>.System.MessageSender.SendExperimentalPartMessage(part.name, count);
    }

    public void ExperimentalPartAdded(AvailablePart part, int count)
    {
      if (SubSystem<ShareExperimentalPartsSystem>.System.IgnoreEvents)
        return;
      LunaLog.Log(string.Format("Relaying experimental part removed: part: {0} count: {1}", (object) part.name, (object) count));
      SubSystem<ShareExperimentalPartsSystem>.System.MessageSender.SendExperimentalPartMessage(part.name, count);
    }
  }
}
