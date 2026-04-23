// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScienceSubject.ShareScienceSubjectSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.ShareScienceSubject
{
  public class ShareScienceSubjectSystem : 
    ShareProgressBaseSystem<ShareScienceSubjectSystem, ShareScienceSubjectMessageSender, ShareScienceSubjectMessageHandler>
  {
    private Dictionary<string, ScienceSubject> _lastScienceSubjects = new Dictionary<string, ScienceSubject>();
    private static Dictionary<string, ScienceSubject> _scienceSubjects;

    public override string SystemName { get; } = nameof (ShareScienceSubjectSystem);

    private ShareScienceSubjectEvents ShareScienceSubjectEvents { get; } = new ShareScienceSubjectEvents();

    public Dictionary<string, ScienceSubject> ScienceSubjects
    {
      get
      {
        if (ShareScienceSubjectSystem._scienceSubjects == null)
          ShareScienceSubjectSystem._scienceSubjects = Traverse.Create((object) ResearchAndDevelopment.Instance).Field("scienceSubjects").GetValue<Dictionary<string, ScienceSubject>>();
        return ShareScienceSubjectSystem._scienceSubjects;
      }
    }

    protected override bool ShareSystemReady => Object.op_Inequality((Object) ResearchAndDevelopment.Instance, (Object) null);

    protected override GameMode RelevantGameModes => GameMode.Science | GameMode.Career;

    public bool Reverting { get; set; }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      GameEvents.OnScienceRecieved.Add(new EventData<float, ScienceSubject, ProtoVessel, bool>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(ScienceRecieved)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Add(new EventVoid.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Add(new EventData<EditorFacility>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(LevelLoaded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.OnScienceRecieved.Remove(new EventData<float, ScienceSubject, ProtoVessel, bool>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(ScienceRecieved)));
      // ISSUE: method pointer
      RevertEvent.onRevertingToLaunch.Remove(new EventVoid.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(RevertingDetected)));
      // ISSUE: method pointer
      RevertEvent.onReturningToEditor.Remove(new EventData<EditorFacility>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(RevertingToEditorDetected)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareScienceSubjectEvents, __methodptr(LevelLoaded)));
      this.Reverting = false;
      this._lastScienceSubjects.Clear();
      ShareScienceSubjectSystem._scienceSubjects = (Dictionary<string, ScienceSubject>) null;
    }

    public override void SaveState()
    {
      base.SaveState();
      this._lastScienceSubjects = this.ScienceSubjects;
    }

    public override void RestoreState()
    {
      base.RestoreState();
      Traverse.Create((object) ResearchAndDevelopment.Instance).Field("scienceSubjects").SetValue((object) this._lastScienceSubjects);
    }
  }
}
