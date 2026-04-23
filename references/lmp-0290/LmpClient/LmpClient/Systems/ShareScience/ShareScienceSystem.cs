// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScience.ShareScienceSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Systems.ShareScience
{
  public class ShareScienceSystem : 
    ShareProgressBaseSystem<ShareScienceSystem, ShareScienceMessageSender, ShareScienceMessageHandler>
  {
    private float _lastScience;

    public override string SystemName { get; } = nameof (ShareScienceSystem);

    private ShareScienceEvents ShareScienceEvents { get; } = new ShareScienceEvents();

    protected override bool ShareSystemReady => Object.op_Inequality((Object) ResearchAndDevelopment.Instance, (Object) null);

    protected override GameMode RelevantGameModes => GameMode.Science | GameMode.Career;

    public bool Reverting { get; set; }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnScienceChanged.Add(new EventData<float, TransactionReasons>.OnEvent((object) this.ShareScienceEvents, __methodptr(ScienceChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Add(new EventVoid.OnEvent((object) this.ShareScienceEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.ShareScienceEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareScienceEvents, __methodptr(LevelLoaded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnScienceChanged.Remove(new EventData<float, TransactionReasons>.OnEvent((object) this.ShareScienceEvents, __methodptr(ScienceChanged)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Remove(new EventVoid.OnEvent((object) this.ShareScienceEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.ShareScienceEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareScienceEvents, __methodptr(LevelLoaded)));
      this.Reverting = false;
      this._lastScience = 0.0f;
    }

    public override void SaveState()
    {
      base.SaveState();
      this._lastScience = ResearchAndDevelopment.Instance.Science;
    }

    public override void RestoreState()
    {
      base.RestoreState();
      ResearchAndDevelopment.Instance.SetScience(this._lastScience, (TransactionReasons) 0);
    }

    public void SetScienceWithoutTriggeringEvent(float science)
    {
      if (!this.CurrentGameModeIsRelevant)
        return;
      this.StartIgnoringEvents();
      ResearchAndDevelopment.Instance.SetScience(science, (TransactionReasons) 0);
      this.StopIgnoringEvents();
    }
  }
}
