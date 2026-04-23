// Decompiled with JetBrains decompiler
// Type: LmpCommon.RepoRetrievers.BannedIpsRetriever
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Collection;
using LmpCommon.Time;
using LmpGlobal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;

namespace LmpCommon.RepoRetrievers
{
  public static class BannedIpsRetriever
  {
    private static readonly ConcurrentHashSet<IPAddress> PrivBannedIPs = new ConcurrentHashSet<IPAddress>();
    private static readonly TimeSpan MaxRequestInterval = TimeSpan.FromMinutes(10.0);
    private static DateTime _lastRequestTime = DateTime.MinValue;

    private static ConcurrentHashSet<IPAddress> BannedIps
    {
      get
      {
        if (BannedIpsRetriever._lastRequestTime == DateTime.MinValue)
        {
          BannedIpsRetriever.RefreshBannedIps();
          BannedIpsRetriever._lastRequestTime = LunaComputerTime.UtcNow;
        }
        else if (LunaComputerTime.UtcNow - BannedIpsRetriever._lastRequestTime > BannedIpsRetriever.MaxRequestInterval)
        {
          Task.Run((Action) (() => BannedIpsRetriever.RefreshBannedIps()));
          BannedIpsRetriever._lastRequestTime = LunaComputerTime.UtcNow;
        }
        return BannedIpsRetriever.PrivBannedIPs;
      }
    }

    public static bool IsBanned(IPEndPoint endpoint) => BannedIpsRetriever.BannedIps.Contains(endpoint.Address);

    private static void RefreshBannedIps()
    {
      try
      {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(GithubCertification.MyRemoteCertificateValidationCallback);
        using (WebClient webClient = new WebClient())
        {
          using (Stream stream = webClient.OpenRead(RepoConstants.BannedIpListUrl))
          {
            using (StreamReader streamReader = new StreamReader(stream))
            {
              string[] array = ((IEnumerable<string>) streamReader.ReadToEnd().Trim().Split('\n')).Where<string>((Func<string, bool>) (s => !s.StartsWith("#") && !string.IsNullOrEmpty(s))).ToArray<string>();
              BannedIpsRetriever.PrivBannedIPs.Clear();
              foreach (string ipString in array)
              {
                try
                {
                  IPAddress address;
                  if (!IPAddress.TryParse(ipString, out address))
                    BannedIpsRetriever.PrivBannedIPs.Add(address);
                }
                catch (Exception ex)
                {
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
    }
  }
}
