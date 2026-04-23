// Decompiled with JetBrains decompiler
// Type: LmpClient.LunaLog
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace LmpClient
{
  public class LunaLog
  {
    private static readonly ConcurrentQueue<LunaLog.LogEntry> Queue = new ConcurrentQueue<LunaLog.LogEntry>();

    public static void LogWarning(string message)
    {
      string text = message.Contains("[LMP]") ? message : "[LMP]: " + message;
      if (MainSystem.IsUnityThread)
        Debug.LogWarning((object) text);
      else
        LunaLog.Queue.Enqueue(new LunaLog.LogEntry(LunaLog.LogType.Warning, text));
    }

    public static void LogError(string message)
    {
      string text = message.Contains("[LMP]") ? message : "[LMP]: " + message;
      if (MainSystem.IsUnityThread)
        Debug.LogError((object) text);
      else
        LunaLog.Queue.Enqueue(new LunaLog.LogEntry(LunaLog.LogType.Error, text));
    }

    public static void Log(string message)
    {
      string text = message.StartsWith("[LMP]") ? message : "[LMP]: " + message;
      if (MainSystem.IsUnityThread)
        Debug.Log((object) text);
      else
        LunaLog.Queue.Enqueue(new LunaLog.LogEntry(LunaLog.LogType.Info, text));
    }

    public static void ProcessLogMessages()
    {
      if (!MainSystem.IsUnityThread)
        throw new Exception("Cannot call ProcessLogMessages from another thread that is not the Unity thread");
      LunaLog.LogEntry result;
      while (LunaLog.Queue.TryDequeue(out result))
      {
        switch (result.Type)
        {
          case LunaLog.LogType.Error:
            Debug.LogError((object) result.Text);
            continue;
          case LunaLog.LogType.Warning:
            Debug.LogWarning((object) result.Text);
            continue;
          case LunaLog.LogType.Info:
            Debug.Log((object) result.Text);
            continue;
          default:
            continue;
        }
      }
    }

    private enum LogType
    {
      Error,
      Warning,
      Info,
    }

    private class LogEntry
    {
      public LunaLog.LogType Type { get; }

      public string Text { get; }

      public LogEntry(LunaLog.LogType type, string text)
      {
        this.Type = type;
        this.Text = text;
      }
    }
  }
}
