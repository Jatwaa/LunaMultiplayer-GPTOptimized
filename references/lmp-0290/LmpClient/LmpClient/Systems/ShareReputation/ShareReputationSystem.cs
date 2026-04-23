// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareReputation.ShareReputationSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;

namespace LmpClient.Systems.ShareReputation
{
  public class ShareReputationSystem : 
    ShareProgressBaseSystem<ShareReputationSystem, ShareReputationMessageSender, ShareReputationMessageHandler>
  {
    private float _lastReputation;

    public override string SystemName { get; } = nameof (ShareReputationSystem);

    private ShareReputationEvents ShareReputationEvents { get; } = new ShareReputationEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    public bool Reverting { get; set; }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnReputationChanged.Add(new EventData<float, TransactionReasons>.OnEvent((object) this.ShareReputationEvents, __methodptr(ReputationChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Add(new EventVoid.OnEvent((object) this.ShareReputationEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.ShareReputationEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareReputationEvents, __methodptr(LevelLoaded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnReputationChanged.Remove(new EventData<float, TransactionReasons>.OnEvent((object) this.ShareReputationEvents, __methodptr(ReputationChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Remove(new EventVoid.OnEvent((object) this.ShareReputationEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.ShareReputationEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareReputationEvents, __methodptr(LevelLoaded)));
      this._lastReputation = 0.0f;
      this.Reverting = false;
    }

    public override void SaveState()
    {
      base.SaveState();
      this._lastReputation = Reputation.Instance.reputation;
    }

    public override void RestoreState()
    {
      base.RestoreState();
      Reputation.Instance.SetReputation(this._lastReputation, (TransactionReasons) 0);
    }

    public void SetReputationWithoutTriggeringEvent(float reputation)
    {
      if (!this.CurrentGameModeIsRelevant)
        return;
      this.StartIgnoringEvents();
      Reputation.Instance.SetReputation(reputation, (TransactionReasons) 0);
      this.StopIgnoringEvents();
    }
  }
}
