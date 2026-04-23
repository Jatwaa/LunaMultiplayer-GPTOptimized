// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SystemsHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using LmpClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Profiling;

namespace LmpClient.Systems
{
  public static class SystemsHandler
  {
    private static ISystem[] _systems = new ISystem[0];

    public static void FillUpSystemsList()
    {
      List<ISystem> source = new List<ISystem>();
      foreach (Type type in Assembly.GetExecutingAssembly().GetLoadableTypes().Where<Type>((Func<Type, bool>) (t => t.IsClass && typeof (ISystem).IsAssignableFrom(t) && !t.IsAbstract)).ToArray<Type>())
      {
        try
        {
          if (type.GetProperty("Singleton", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.GetValue((object) null, (object[]) null) is ISystem system)
            source.Add(system);
        }
        catch (Exception ex)
        {
          LunaLog.LogError("Exception loading system type " + type.FullName + ": " + ex.Message);
        }
      }
      SystemsHandler._systems = source.OrderBy<ISystem, int>((Func<ISystem, int>) (s => s.ExecutionOrder)).ToArray<ISystem>();
    }

    public static void FixedUpdate()
    {
      for (int index = 0; index < SystemsHandler._systems.Length; ++index)
      {
        try
        {
          Profiler.BeginSample(SystemsHandler._systems[index].SystemName);
          SystemsHandler._systems[index].FixedUpdate();
          Profiler.EndSample();
        }
        catch (Exception ex)
        {
          MainSystem.Singleton.HandleException(ex, "SystemHandler-FixedUpdate");
        }
      }
    }

    public static void Update()
    {
      for (int index = 0; index < SystemsHandler._systems.Length; ++index)
      {
        try
        {
          Profiler.BeginSample(SystemsHandler._systems[index].SystemName);
          SystemsHandler._systems[index].Update();
          Profiler.EndSample();
        }
        catch (Exception ex)
        {
          MainSystem.Singleton.HandleException(ex, "SystemHandler-Update");
        }
      }
    }

    public static void LateUpdate()
    {
      for (int index = 0; index < SystemsHandler._systems.Length; ++index)
      {
        try
        {
          Profiler.BeginSample(SystemsHandler._systems[index].SystemName);
          SystemsHandler._systems[index].LateUpdate();
          Profiler.EndSample();
        }
        catch (Exception ex)
        {
          MainSystem.Singleton.HandleException(ex, "SystemHandler-Update");
        }
      }
    }
  }
}
