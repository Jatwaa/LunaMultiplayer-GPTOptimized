// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareProgress.ShareProgressBaseSystem`3
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;

namespace LmpClient.Systems.ShareProgress
{
  public abstract class ShareProgressBaseSystem<T, TS, TH> : MessageSystem<T, TS, TH>
    where T : LmpClient.Base.System<T>, new()
    where TS : class, IMessageSender, new()
    where TH : class, IMessageHandler, new()
  {
    private Queue<Action> _actionQueue;

    public bool IgnoreEvents { get; protected set; }

    protected abstract GameMode RelevantGameModes { get; }

    protected bool CurrentGameModeIsRelevant => (uint) (SettingsSystem.ServerSettings.GameMode & this.RelevantGameModes) > 0U;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      this.IgnoreEvents = false;
      this._actionQueue = new Queue<Action>();
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(this.RunQueue)));
    }

    protected abstract bool ShareSystemReady { get; }

    public virtual void SaveState()
    {
    }

    public virtual void RestoreState()
    {
    }

    public void StartIgnoringEvents()
    {
      this.SaveState();
      this.IgnoreEvents = true;
    }

    public void StopIgnoringEvents(bool restoreOldValue = false)
    {
      if (restoreOldValue)
        this.RestoreState();
      this.IgnoreEvents = false;
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
