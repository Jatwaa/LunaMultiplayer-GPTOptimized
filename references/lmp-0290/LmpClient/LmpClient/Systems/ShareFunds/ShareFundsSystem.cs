// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareFunds.ShareFundsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using System;

namespace LmpClient.Systems.ShareFunds
{
  public class ShareFundsSystem : 
    ShareProgressBaseSystem<ShareFundsSystem, ShareFundsMessageSender, ShareFundsMessageHandler>
  {
    private double _lastFunds;

    public override string SystemName { get; } = nameof (ShareFundsSystem);

    private ShareFundsEvents ShareFundsEvents { get; } = new ShareFundsEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    public bool Reverting { get; set; }

    public Tuple<Guid, float> CurrentShipCost { get; set; }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnFundsChanged.Add(new EventData<double, TransactionReasons>.OnEvent((object) this.ShareFundsEvents, __methodptr(FundsChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Add(new EventVoid.OnEvent((object) this.ShareFundsEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.ShareFundsEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareFundsEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselSwitching.Add(new EventData<Vessel, Vessel>.OnEvent((object) this.ShareFundsEvents, __methodptr(VesselSwitching)));
      // ISSUE: method pointer
      VesselAssemblyEvent.onAssembledVessel.Add(new EventData<Vessel, ShipConstruct>.OnEvent((object) this.ShareFundsEvents, __methodptr(VesselAssembled)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnFundsChanged.Remove(new EventData<double, TransactionReasons>.OnEvent((object) this.ShareFundsEvents, __methodptr(FundsChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Remove(new EventVoid.OnEvent((object) this.ShareFundsEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.ShareFundsEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareFundsEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.onVesselSwitching.Remove(new EventData<Vessel, Vessel>.OnEvent((object) this.ShareFundsEvents, __methodptr(VesselSwitching)));
      // ISSUE: method pointer
      VesselAssemblyEvent.onAssembledVessel.Remove(new EventData<Vessel, ShipConstruct>.OnEvent((object) this.ShareFundsEvents, __methodptr(VesselAssembled)));
      this._lastFunds = 0.0;
      this.Reverting = false;
    }

    public override void SaveState()
    {
      base.SaveState();
      this._lastFunds = Funding.Instance.Funds;
    }

    public override void RestoreState()
    {
      base.RestoreState();
      Funding.Instance.SetFunds(this._lastFunds, (TransactionReasons) 0);
    }

    public void SetFundsWithoutTriggeringEvent(double funds)
    {
      if (!this.CurrentGameModeIsRelevant)
        return;
      this.StartIgnoringEvents();
      Funding.Instance.SetFunds(funds, (TransactionReasons) 0);
      this.StopIgnoringEvents();
    }
  }
}
