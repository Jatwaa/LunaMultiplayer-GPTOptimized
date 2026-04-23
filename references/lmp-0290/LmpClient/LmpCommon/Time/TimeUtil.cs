// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.TimeUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpCommon.Time
{
  public static class TimeUtil
  {
    public static double SecondsToMilliseconds(double seconds) => seconds * 1000.0;

    public static double MillisecondsToSeconds(double milliseconds) => milliseconds / 1000.0;

    public static long SecondsToTicks(double seconds) => (long) (seconds * 10000000.0);

    public static double TicksToSeconds(double ticks) => ticks / 10000000.0;

    public static bool IsInInterval(ref DateTime lastRequest, int intervalInMs)
    {
      if (!(LunaComputerTime.UtcNow - lastRequest > TimeSpan.FromMilliseconds((double) intervalInMs)))
        return false;
      lastRequest = DateTime.UtcNow;
      return true;
    }
  }
}
