// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.System`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using LmpClient.Events;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Base
{
  public abstract class System<T> : SystemBase, ISystem where T : ISystem, new()
  {
    private bool _enabled;

    public static T Singleton { get; } = new T();

    public abstract string SystemName { get; }

    public virtual int ExecutionOrder { get; } = 0;

    private List<RoutineDefinition> FixedUpdateRoutines { get; } = new List<RoutineDefinition>();

    private List<RoutineDefinition> UpdateRoutines { get; } = new List<RoutineDefinition>();

    private List<RoutineDefinition> LateUpdateRoutines { get; } = new List<RoutineDefinition>();

    protected System()
    {
      EventData<ClientState> networkStatusChanged = NetworkEvent.onNetworkStatusChanged;
      LmpClient.Base.System<T> system = this;
      // ISSUE: virtual method pointer
      EventData<ClientState>.OnEvent onEvent = new EventData<ClientState>.OnEvent((object) system, __vmethodptr(system, NetworkEventHandler));
      networkStatusChanged.Add(onEvent);
    }

    protected virtual ClientState EnableStage => ClientState.Running;

    protected virtual void NetworkEventHandler(ClientState data)
    {
      if (data <= ClientState.Disconnected)
        this.Enabled = false;
      if (data != this.EnableStage)
        return;
      this.Enabled = true;
    }

    protected virtual bool AlwaysEnabled { get; } = false;

    protected void SetupRoutine(RoutineDefinition routine)
    {
      if (routine == null)
        LunaLog.LogError("[LMP]: Cannot set a null routine!");
      else if (routine.Execution == RoutineExecution.FixedUpdate && this.FixedUpdateRoutines.All<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name != routine.Name)))
        this.FixedUpdateRoutines.Add(routine);
      else if (routine.Execution == RoutineExecution.Update && this.UpdateRoutines.All<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name != routine.Name)))
        this.UpdateRoutines.Add(routine);
      else if (routine.Execution == RoutineExecution.LateUpdate && this.LateUpdateRoutines.All<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name != routine.Name)))
        this.LateUpdateRoutines.Add(routine);
      else
        LunaLog.LogError("[LMP]: Routine " + routine.Name + " already defined");
    }

    protected void ChangeRoutineExecutionInterval(
      RoutineExecution execution,
      string routineName,
      int newIntervalInMs)
    {
      RoutineDefinition routineDefinition;
      switch (execution)
      {
        case RoutineExecution.FixedUpdate:
          routineDefinition = this.FixedUpdateRoutines.FirstOrDefault<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name == routineName));
          break;
        case RoutineExecution.Update:
          routineDefinition = this.UpdateRoutines.FirstOrDefault<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name == routineName));
          break;
        case RoutineExecution.LateUpdate:
          routineDefinition = this.LateUpdateRoutines.FirstOrDefault<RoutineDefinition>((Func<RoutineDefinition, bool>) (r => r.Name == routineName));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (execution), (object) execution, (string) null);
      }
      if (routineDefinition != null)
        routineDefinition.IntervalInMs = newIntervalInMs;
      else
        LunaLog.LogError(string.Format("[LMP]: Routine {0}/{1} not defined", (object) execution, (object) routineName));
    }

    public virtual bool Enabled
    {
      get => this.AlwaysEnabled || this._enabled;
      set
      {
        if (this.AlwaysEnabled)
          return;
        if (!this._enabled & value)
        {
          this._enabled = true;
          this.OnEnabled();
        }
        else if (this._enabled && !value)
        {
          this._enabled = false;
          this.OnDisabled();
          this.RemoveRoutines();
        }
      }
    }

    protected virtual void OnEnabled()
    {
    }

    protected virtual void OnDisabled()
    {
    }

    protected virtual void RemoveRoutines()
    {
      this.UpdateRoutines.Clear();
      this.FixedUpdateRoutines.Clear();
      this.LateUpdateRoutines.Clear();
    }

    public void FixedUpdate()
    {
      for (int index = 0; index < this.FixedUpdateRoutines.Count; ++index)
        this.FixedUpdateRoutines[index]?.RunRoutine();
    }

    public void Update()
    {
      for (int index = 0; index < this.UpdateRoutines.Count; ++index)
        this.UpdateRoutines[index]?.RunRoutine();
    }

    public void LateUpdate()
    {
      for (int index = 0; index < this.LateUpdateRoutines.Count; ++index)
        this.LateUpdateRoutines[index]?.RunRoutine();
    }
  }
}
