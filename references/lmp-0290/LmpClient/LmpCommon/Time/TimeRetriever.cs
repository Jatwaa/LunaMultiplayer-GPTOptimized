// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.TimeRetriever
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Concurrent;

namespace LmpCommon.Time
{
  internal class TimeRetriever
  {
    private static readonly ConcurrentDictionary<TimeProvider, DateTime> TimeProviderLastRequests = new ConcurrentDictionary<TimeProvider, DateTime>()
    {
      [TimeProvider.Nist] = DateTime.MinValue,
      [TimeProvider.Microsoft] = DateTime.MinValue,
      [TimeProvider.Google] = DateTime.MinValue,
      [TimeProvider.NtpOrg] = DateTime.MinValue
    };

    internal static bool CanRequestTime(TimeProvider provider) => (DateTime.UtcNow - TimeRetriever.TimeProviderLastRequests[provider]).TotalSeconds > 5.0;

    internal static DateTime? GetTime(TimeProvider provider)
    {
      if (!TimeRetriever.CanRequestTime(provider))
        throw new Exception("Too many time requests!");
      DateTime? time;
      switch (provider)
      {
        case TimeProvider.Nist:
          time = new DateTime?(TimeRetrieverNtp.GetNtpTime("time-a.nist.gov"));
          break;
        case TimeProvider.Microsoft:
          time = new DateTime?(TimeRetrieverNtp.GetNtpTime("time.windows.com"));
          break;
        case TimeProvider.Google:
          time = new DateTime?(TimeRetrieverNtp.GetNtpTime("time.google.com"));
          break;
        case TimeProvider.NtpOrg:
          time = new DateTime?(TimeRetrieverNtp.GetNtpTime("pool.ntp.org"));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (provider), (object) provider, (string) null);
      }
      TimeRetriever.TimeProviderLastRequests[provider] = DateTime.UtcNow;
      return time;
    }
  }
}
