// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.LunaNetworkTime
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Threading;

namespace LmpCommon.Time
{
  public class LunaNetworkTime
  {
    private static readonly Timer Timer = new Timer((TimerCallback) (_ => LunaNetworkTime.RefreshTimeDifference()), (object) null, 0, 30000);
    private const int TimeSyncIntervalMs = 30000;

    public static DateTime Now => LunaNetworkTime.UtcNow.ToLocalTime();

    public static TimeSpan TimeDifference { get; private set; } = TimeSpan.Zero;

    public static float SimulatedMsTimeOffset { get; set; } = 0.0f;

    public static DateTime UtcNow => LunaComputerTime.UtcNow + LunaNetworkTime.TimeDifference.Negate() + TimeSpan.FromMilliseconds((double) LunaNetworkTime.SimulatedMsTimeOffset);

    private static void RefreshTimeDifference()
    {
      bool createdNew;
      using (Mutex mutex = new Mutex(true, "LunaTimeMutex", out createdNew))
      {
        if (createdNew || mutex.WaitOne(10))
        {
          try
          {
            DateTime? time = TimeRetriever.GetTime(TimeProvider.Google);
            if (time.HasValue)
              LunaNetworkTime.TimeDifference = LunaComputerTime.UtcNow - time.Value;
          }
          catch (Exception ex)
          {
          }
          Thread.Sleep(5000);
          mutex.ReleaseMutex();
        }
        else
          LunaNetworkTime.Timer.Change(5500, 30000);
      }
    }
  }
}
