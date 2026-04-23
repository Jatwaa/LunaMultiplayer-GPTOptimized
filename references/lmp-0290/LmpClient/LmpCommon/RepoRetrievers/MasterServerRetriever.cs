// Decompiled with JetBrains decompiler
// Type: LmpCommon.RepoRetrievers.MasterServerRetriever
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
  public static class MasterServerRetriever
  {
    private static readonly ConcurrentHashSet<IPEndPoint> MasterServersEndpoints = new ConcurrentHashSet<IPEndPoint>();
    private static readonly TimeSpan MaxRequestInterval = TimeSpan.FromMinutes(15.0);
    private static DateTime _lastRequestTime = DateTime.MinValue;

    public static ConcurrentHashSet<IPEndPoint> MasterServers
    {
      get
      {
        if (MasterServerRetriever._lastRequestTime == DateTime.MinValue)
        {
          MasterServerRetriever.RefreshMasterServersList();
          MasterServerRetriever._lastRequestTime = LunaComputerTime.UtcNow;
        }
        else if (LunaComputerTime.UtcNow - MasterServerRetriever._lastRequestTime > MasterServerRetriever.MaxRequestInterval)
        {
          Task.Run(new Action(MasterServerRetriever.RefreshMasterServersList));
          MasterServerRetriever._lastRequestTime = LunaComputerTime.UtcNow;
        }
        return MasterServerRetriever.MasterServersEndpoints;
      }
    }

    private static void RefreshMasterServersList()
    {
      try
      {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(GithubCertification.MyRemoteCertificateValidationCallback);
        using (WebClient webClient = new WebClient())
        {
          using (Stream stream = webClient.OpenRead(RepoConstants.MasterServersListUrl))
          {
            using (StreamReader streamReader = new StreamReader(stream))
            {
              string[] array = ((IEnumerable<string>) streamReader.ReadToEnd().Trim().Split('\n')).Where<string>((Func<string, bool>) (s => !s.StartsWith("#") && s.Contains(":") && !string.IsNullOrEmpty(s))).ToArray<string>();
              MasterServerRetriever.MasterServersEndpoints.Clear();
              foreach (string endpoint in array)
              {
                try
                {
                  MasterServerRetriever.MasterServersEndpoints.Add(LunaNetUtils.CreateEndpointFromString(endpoint));
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
