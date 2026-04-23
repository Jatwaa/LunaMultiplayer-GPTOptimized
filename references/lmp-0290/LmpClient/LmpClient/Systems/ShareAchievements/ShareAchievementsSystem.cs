// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareAchievements.ShareAchievementsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.ShareAchievements
{
  public class ShareAchievementsSystem : 
    ShareProgressBaseSystem<ShareAchievementsSystem, ShareAchievementsMessageSender, ShareAchievementsMessageHandler>
  {
    private ConfigNode _lastAchievements;

    public override string SystemName { get; } = nameof (ShareAchievementsSystem);

    private ShareAchievementsEvents ShareAchievementsEvents { get; } = new ShareAchievementsEvents();

    protected override bool ShareSystemReady => Object.op_Inequality((Object) ProgressTracking.Instance, (Object) null);

    protected override GameMode RelevantGameModes => GameMode.Career;

    public bool Reverting { get; set; }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnProgressReached.Add(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementReached)));
      // ISSUE: method pointer
      GameEvents.OnProgressComplete.Add(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementCompleted)));
      // ISSUE: method pointer
      GameEvents.OnProgressAchieved.Add(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementAchieved)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Add(new EventVoid.OnEvent((object) this.ShareAchievementsEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(LevelLoaded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnProgressReached.Remove(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementReached)));
      // ISSUE: method pointer
      GameEvents.OnProgressComplete.Remove(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementCompleted)));
      // ISSUE: method pointer
      GameEvents.OnProgressAchieved.Remove(new EventData<ProgressNode>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(AchievementAchieved)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Remove(new EventVoid.OnEvent((object) this.ShareAchievementsEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareAchievementsEvents, __methodptr(LevelLoaded)));
      this._lastAchievements = (ConfigNode) null;
      this.Reverting = false;
    }

    public override void SaveState()
    {
      base.SaveState();
      this._lastAchievements = new ConfigNode();
      if (!Object.op_Implicit((Object) ProgressTracking.Instance))
        return;
      ProgressTracking.Instance.achievementTree.Save(this._lastAchievements);
    }

    public override void RestoreState()
    {
      base.RestoreState();
      if (Object.op_Equality((Object) ProgressTracking.Instance, (Object) null))
      {
        ConfigNode node1 = Traverse.Create((object) ((IEnumerable<ProtoScenarioModule>) HighLogic.CurrentGame.scenarios).FirstOrDefault<ProtoScenarioModule>((Func<ProtoScenarioModule, bool>) (s => s.moduleName == "ProgressTracking"))).Field<ConfigNode>("moduleValues").Value.GetNode("Progress");
        node1.ClearNodes();
        foreach (ConfigNode node2 in this._lastAchievements.GetNodes())
          node1.AddNode(node2);
      }
      else
        ProgressTracking.Instance.achievementTree.Load(this._lastAchievements);
    }
  }
}
