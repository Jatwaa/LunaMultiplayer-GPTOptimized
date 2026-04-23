// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareCareer.ShareCareerSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.ShareCareer
{
  public class ShareCareerSystem : LmpClient.Base.System<ShareCareerSystem>
  {
    private Queue<Action> _actionQueue;

    public override string SystemName { get; } = nameof (ShareCareerSystem);

    protected bool ShareSystemReady => Object.op_Inequality((Object) ContractSystem.Instance, (Object) null) && Object.op_Inequality((Object) Funding.Instance, (Object) null) && Object.op_Inequality((Object) ResearchAndDevelopment.Instance, (Object) null) && Object.op_Inequality((Object) Reputation.Instance, (Object) null) && (double) Time.timeSinceLevelLoad > 1.0 && HighLogic.LoadedScene >= 5 && HighLogic.LoadedScene <= 8;

    protected override void OnEnabled()
    {
      if (SettingsSystem.ServerSettings.GameMode != GameMode.Career)
        return;
      base.OnEnabled();
      this._actionQueue = new Queue<Action>();
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.RunQueue)));
    }

    public void QueueAction(Action action)
    {
      this._actionQueue.Enqueue(action);
      this.RunQueue();
    }

    private void RunQueue()
    {
      while (this._actionQueue.Count > 0 && this.ShareSystemReady)
      {
        Action action = this._actionQueue.Dequeue();
        if (action != null)
          action();
      }
    }
  }
}
