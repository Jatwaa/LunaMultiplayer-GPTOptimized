// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SharePurchaseParts.SharePurchasePartsEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.SharePurchaseParts
{
  public class SharePurchasePartsEvents : SubSystem<SharePurchasePartsSystem>
  {
    public void PartPurchased(AvailablePart part)
    {
      if (SubSystem<SharePurchasePartsSystem>.System.IgnoreEvents)
        return;
      ProtoTechNode techState = ResearchAndDevelopment.Instance.GetTechState(part.TechRequired);
      if (techState == null)
        return;
      LunaLog.Log("Relaying part purchased on tech: " + techState.techID + "; part: " + part.name);
      SubSystem<SharePurchasePartsSystem>.System.MessageSender.SendPartPurchasedMessage(techState.techID, part.name);
    }
  }
}
