// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareFunds.ShareFundsEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using System;

namespace LmpClient.Systems.ShareFunds
{
  public class ShareFundsEvents : SubSystem<ShareFundsSystem>
  {
    public void FundsChanged(double funds, TransactionReasons reason)
    {
      if (SubSystem<ShareFundsSystem>.System.IgnoreEvents)
        return;
      LunaLog.Log(string.Format("Funds changed to: {0} reason: {1}", (object) funds, (object) reason));
      SubSystem<ShareFundsSystem>.System.MessageSender.SendFundsMessage(funds, reason.ToString());
    }

    public void RevertingDetected()
    {
      SubSystem<ShareFundsSystem>.System.Reverting = true;
      SubSystem<ShareFundsSystem>.System.StartIgnoringEvents();
    }

    public void RevertingToEditorDetected(EditorFacility data)
    {
      SubSystem<ShareFundsSystem>.System.Reverting = true;
      if (SubSystem<ShareFundsSystem>.System.CurrentShipCost != null)
      {
        Funding.Instance.AddFunds((double) SubSystem<ShareFundsSystem>.System.CurrentShipCost.Item2, (TransactionReasons) 32);
        SubSystem<ShareFundsSystem>.System.CurrentShipCost = (Tuple<Guid, float>) null;
      }
      SubSystem<ShareFundsSystem>.System.StartIgnoringEvents();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (!SubSystem<ShareFundsSystem>.System.Reverting)
        return;
      SubSystem<ShareFundsSystem>.System.Reverting = false;
      SubSystem<ShareFundsSystem>.System.StopIgnoringEvents(true);
    }

    public void VesselSwitching(Vessel data0, Vessel data1) => SubSystem<ShareFundsSystem>.System.CurrentShipCost = (Tuple<Guid, float>) null;

    public void VesselAssembled(Vessel vessel, ShipConstruct construct)
    {
      float num1;
      float num2;
      SubSystem<ShareFundsSystem>.System.CurrentShipCost = new Tuple<Guid, float>(vessel.id, construct.GetShipCosts(ref num1, ref num2));
    }
  }
}
