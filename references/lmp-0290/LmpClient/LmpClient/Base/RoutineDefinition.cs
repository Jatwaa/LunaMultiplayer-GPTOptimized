// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.RoutineDefinition
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace LmpClient.Base
{
  public class RoutineDefinition
  {
    private readonly Stopwatch _stopwatch = new Stopwatch();

    public string Name => this.Method.Method.Name;

    public int IntervalInMs { get; set; }

    public Action Method { private get; set; }

    public string MethodName { get; }

    public RoutineExecution Execution { get; set; }

    private RoutineDefinition() => this._stopwatch.Start();

    public RoutineDefinition(int intervalInMs, RoutineExecution execution, Action method)
      : this()
    {
      this.IntervalInMs = intervalInMs;
      this.Execution = execution;
      this.Method = method;
      this.MethodName = method.Method.Name;
    }

    public void RunRoutine()
    {
      if (this.IntervalInMs > 0 && this._stopwatch.ElapsedMilliseconds <= (long) this.IntervalInMs)
        return;
      Profiler.BeginSample(this.MethodName);
      this.Method();
      this._stopwatch.Reset();
      this._stopwatch.Start();
      Profiler.EndSample();
    }
  }
}
