// Decompiled with JetBrains decompiler
// Type: LmpClient.LunaScreenMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace LmpClient
{
  public class LunaScreenMsg
  {
    private static readonly ConcurrentQueue<ScreenMessage> Queue = new ConcurrentQueue<ScreenMessage>();

    public static ScreenMessage PostScreenMessage(
      string text,
      float durationInSeconds,
      ScreenMessageStyle location)
    {
      return LunaScreenMsg.PostScreenMessage(text, durationInSeconds, location, Color.green);
    }

    public static ScreenMessage PostScreenMessage(
      string text,
      float durationInSeconds,
      ScreenMessageStyle location,
      Color color)
    {
      if (MainSystem.IsUnityThread)
        return ScreenMessages.PostScreenMessage(text, durationInSeconds, location, color);
      LunaScreenMsg.Queue.Enqueue(LunaScreenMsg.CreateMessage(text, durationInSeconds, location, color));
      return (ScreenMessage) null;
    }

    private static ScreenMessage CreateMessage(
      string text,
      float durationInSeconds,
      ScreenMessageStyle location,
      Color color)
    {
      return new ScreenMessage(text, durationInSeconds, location)
      {
        color = color
      };
    }

    public static void ProcessScreenMessages()
    {
      if (!MainSystem.IsUnityThread)
        throw new Exception("Cannot call ProcessScreenMessages from another thread that is not the Unity thread");
      ScreenMessage result;
      while (LunaScreenMsg.Queue.TryDequeue(out result))
        ScreenMessages.PostScreenMessage(result);
    }
  }
}
