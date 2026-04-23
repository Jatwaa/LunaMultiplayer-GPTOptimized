// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.WindowsHandler
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

namespace LmpClient.Windows
{
  public static class WindowsHandler
  {
    private static IWindow[] Windows = new IWindow[0];

    public static void FillUpWindowsList()
    {
      List<IWindow> windowList = new List<IWindow>();
      foreach (Type type in Assembly.GetExecutingAssembly().GetLoadableTypes().Where<Type>((Func<Type, bool>) (t => t.IsClass && typeof (IWindow).IsAssignableFrom(t) && !t.IsAbstract)).ToArray<Type>())
      {
        try
        {
          if (type.GetProperty("Singleton", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.GetValue((object) null, (object[]) null) is IWindow window)
            windowList.Add(window);
        }
        catch (Exception ex)
        {
          LunaLog.LogError("Exception loading window type " + type.FullName + ": " + ex.Message);
        }
      }
      WindowsHandler.Windows = windowList.ToArray();
    }

    public static void Update()
    {
      for (int index = 0; index < WindowsHandler.Windows.Length; ++index)
      {
        try
        {
          Profiler.BeginSample(WindowsHandler.Windows[index].WindowName);
          WindowsHandler.Windows[index].Update();
          Profiler.EndSample();
        }
        catch (Exception ex)
        {
          MainSystem.Singleton.HandleException(ex, "WindowsHandler-Update");
        }
      }
    }

    public static void OnGui()
    {
      for (int index = 0; index < WindowsHandler.Windows.Length; ++index)
      {
        try
        {
          Profiler.BeginSample(WindowsHandler.Windows[index].WindowName);
          WindowsHandler.Windows[index].OnGui();
          Profiler.EndSample();
        }
        catch (Exception ex)
        {
          MainSystem.Singleton.HandleException(ex, "WindowsHandler-OnGui");
        }
      }
    }
  }
}
