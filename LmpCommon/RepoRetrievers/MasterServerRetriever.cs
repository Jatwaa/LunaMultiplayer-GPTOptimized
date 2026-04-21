using LmpCommon.Collection;
using LmpCommon.Time;
using LmpGlobal;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable SYSLIB0014

namespace LmpCommon.RepoRetrievers
{
    /// <summary>
    /// This class retrieves the ips of master servers that are stored in:
    /// <see cref="RepoConstants.MasterServersListUrl"/>
    /// </summary>
    public static class MasterServerRetriever
    {
        /// <summary>
        /// Hardcoded fallback list used when the GitHub fetch fails (e.g. TLS issues in KSP's Mono runtime).
        /// Keep in sync with the GitHub file when community master servers change.
        /// </summary>
        private static readonly string[] FallbackMasterServers =
        {
            "ms.lmp.dasskelett.dev:8700",
            "lmp.nightshade.fun:8700",
        };

        private static readonly ConcurrentHashSet<IPEndPoint> MasterServersEndpoints = new ConcurrentHashSet<IPEndPoint>();
        public static ConcurrentHashSet<IPEndPoint> MasterServers
        {
            get
            {
                if (_lastRequestTime == DateTime.MinValue)
                {
                    //Run syncronously if it's the first time
                    RefreshMasterServersList();
                    _lastRequestTime = LunaComputerTime.UtcNow;
                }
                else if (LunaComputerTime.UtcNow - _lastRequestTime > MaxRequestInterval)
                {
                    _ = Task.Run(RefreshMasterServersList);
                    _lastRequestTime = LunaComputerTime.UtcNow;
                }

                return MasterServersEndpoints;
            }
        }

        private static readonly TimeSpan MaxRequestInterval = TimeSpan.FromMinutes(15);
        private static DateTime _lastRequestTime = DateTime.MinValue;

        private static void RefreshMasterServersList()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = GithubCertification.MyRemoteCertificateValidationCallback;
                using (var client = new WebClient())
                using (var stream = client.OpenRead(RepoConstants.MasterServersListUrl))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();
                        var servers = content
                            .Trim()
                            .Split('\n')
                            .Where(s => !s.StartsWith("#") && s.Contains(":") && !string.IsNullOrEmpty(s))
                            .ToArray();

                        MasterServersEndpoints.Clear();

                        foreach (var server in servers)
                        {
                            try
                            {
                                var endpoint = LunaNetUtils.CreateEndpointFromString(server);
                                if (endpoint != null)
                                    MasterServersEndpoints.Add(endpoint);
                            }
                            catch (Exception)
                            {
                                //Ignore the bad server
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Ignored
            }

            // If the GitHub fetch produced nothing (network error, TLS failure in Mono, etc.),
            // fall back to the hardcoded list so the client can still reach master servers.
            if (MasterServersEndpoints.Count == 0)
            {
                foreach (var server in FallbackMasterServers)
                {
                    try
                    {
                        var endpoint = LunaNetUtils.CreateEndpointFromString(server);
                        if (endpoint != null)
                            MasterServersEndpoints.Add(endpoint);
                    }
                    catch (Exception)
                    {
                        //Ignore
                    }
                }
            }
        }
    }
}
