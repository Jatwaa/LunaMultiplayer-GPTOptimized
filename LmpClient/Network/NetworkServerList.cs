using Lidgren.Network;
using LmpClient.Systems.Ping;
using LmpCommon;
using LmpCommon.Message.Data.MasterServer;
using LmpCommon.Message.MasterServer;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LmpClient.Network
{
    public class NetworkServerList
    {
        public static string Password { get; set; } = string.Empty;
        public static ConcurrentDictionary<long, ServerInfo> Servers { get; } = new ConcurrentDictionary<long, ServerInfo>();

        private static bool receivedNATIntroductionSuccessResponse = false;
        private static IPEndPoint expectedNatEndpoint = null;
        private static long pendingNatServerId = -1;

        /// <summary>
        /// Sends a request for the server list to the master servers
        /// </summary>
        public static void RequestServers()
        {
            Servers.Clear();
            var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<MsRequestServersMsgData>();
            var requestMsg = NetworkMain.MstSrvMsgFactory.CreateNew<MainMstSrvMsg>(msgData);
            NetworkSender.QueueOutgoingMessage(requestMsg);
        }

        /// <summary>
        /// Handles a server list response from the master servers
        /// </summary>
        public static void HandleServersList(NetIncomingMessage msg)
        {
            try
            {
                var msgDeserialized = NetworkMain.MstSrvMsgFactory.Deserialize(msg, LunaNetworkTime.UtcNow.Ticks);

                //Sometimes we receive other type of unconnected messages.
                //Therefore we assert that the received message data is of MsReplyServersMsgData
                if (msgDeserialized.Data is MsReplyServersMsgData data)
                {
                    //Filter servers with different version
                    if (!LmpVersioning.IsCompatible(data.ServerVersion))
                    {
                        LunaLog.Log($"[LMP]: Server list: skipping server '{data.ServerName}' — version {data.ServerVersion} not compatible with local {LmpVersioning.CurrentVersion}");
                        return;
                    }

                    var server = new ServerInfo
                    {
                        Id = data.Id,
                        InternalEndpoint = data.InternalEndpoint,
                        InternalEndpoint6 = data.InternalEndpoint6,
                        ExternalEndpoint = data.ExternalEndpoint,
                        Description = data.Description,
                        Country = data.Country,
                        Website = data.Website,
                        WebsiteText = data.WebsiteText,
                        Password = data.Password,
                        Cheats = data.Cheats,
                        ServerName = data.ServerName,
                        MaxPlayers = data.MaxPlayers,
                        WarpMode = data.WarpMode,
                        TerrainQuality = data.TerrainQuality,
                        PlayerCount = data.PlayerCount,
                        GameMode = data.GameMode,
                        ModControl = data.ModControl,
                        DedicatedServer = data.DedicatedServer,
                        RainbowEffect = data.RainbowEffect,
                        VesselUpdatesSendMsInterval = data.VesselUpdatesSendMsInterval,
                        ServerVersion = data.ServerVersion
                    };

                    Array.Copy(data.Color, server.Color, 3);

                    Servers.AddOrUpdate(data.Id, server, (l, info) => MergeServerInfos(info, server));
                    PingSystem.QueuePing(data.Id);
                }
            }
            catch (Exception e)
            {
                LunaLog.LogError($"[LMP]: Invalid server list reply msg: {e}");
            }
        }

        /// <summary>
        /// This merges two ServerInfo objects.
        /// It copies over data from the old object if it is empty/null/invalid in the new object.
        /// </summary>
        /// <returns>A merged ServerInfo object</returns>
        private static ServerInfo MergeServerInfos(ServerInfo existing, ServerInfo fresh)
        {
            // Some masterservers have broken country detection, so keep the existing value if any.
            if (string.IsNullOrEmpty(fresh.Country) && !string.IsNullOrEmpty(existing.Country))
                fresh.Country = existing.Country;

            return fresh;
        }

        /// <summary>
        /// Send a request to the master server to introduce us and do the NAT punchtrough to the selected server
        /// </summary>
        public static void IntroduceToServer(long serverId)
        {
            if (Servers.TryGetValue(serverId, out var serverInfo))
            {
                // CRITICAL: Never access NetworkMain.ClientConnection.Socket here.
                // Socket is null until the client has been started (which happens in the send thread).
                // The reference 0.29.0 never touches Socket; it always sends the real IPv6 address.
                // LunaNetUtils.GetOwnInternalIPv6Address() returns IPv6Loopback when no suitable IPv6
                // exists, and the master server already skips IPv6 introduction when loopback is sent.
                var lanV4 = ServerIsInLocalLan(serverInfo.ExternalEndpoint);
                var lanV6 = ServerIsInLocalLan(serverInfo.InternalEndpoint6);
                LunaLog.Log($"[LMP]: Connection decision for '{serverInfo.ServerName}' ({serverId}): " +
                            $"IPv4 LAN={lanV4}, IPv6 LAN={lanV6}, External={serverInfo.ExternalEndpoint}, " +
                            $"Internal={serverInfo.InternalEndpoint}, Internal6={serverInfo.InternalEndpoint6}");

                if (lanV4 || lanV6)
                {
                    LunaLog.Log("[LMP]: Server is in LAN. Skipping NAT punch");
                    var endpoints = new List<IPEndPoint>();
                    if (lanV6 && !serverInfo.InternalEndpoint6.Address.Equals(IPAddress.IPv6Loopback))
                        endpoints.Add(serverInfo.InternalEndpoint6);
                    if (lanV4 && !serverInfo.InternalEndpoint.Address.Equals(IPAddress.Loopback))
                        endpoints.Add(serverInfo.InternalEndpoint);
                    if (endpoints.Count == 0)
                    {
                        LunaLog.LogWarning("[LMP]: LAN detected but no valid internal endpoint; falling back to external");
                        endpoints.Add(serverInfo.ExternalEndpoint);
                    }
                    LunaLog.Log($"[LMP]: Connecting via LAN to {string.Join(", ", endpoints)}");
                    NetworkConnection.ConnectToServerCore(endpoints.ToArray(), Password);
                }
                else
                {
                    try
                    {
                        receivedNATIntroductionSuccessResponse = false;
                        expectedNatEndpoint = serverInfo.ExternalEndpoint;
                        pendingNatServerId = serverId;

                        // Ensure client is started so Port reflects the real bound port.
                        // The send thread will also start it before sending, but we read Port here
                        // to build the introduction message.
                        if (NetworkMain.ClientConnection.Status == NetPeerStatus.NotRunning)
                        {
                            LunaLog.Log("[LMP]: Client not running — starting it now for NAT introduction");
                            NetworkMain.ClientConnection.Start();
                            var spinCount = 0;
                            while (NetworkMain.ClientConnection.Status != NetPeerStatus.Running && spinCount < 200)
                            {
                                Thread.Sleep(10);
                                spinCount++;
                            }
                            if (NetworkMain.ClientConnection.Status != NetPeerStatus.Running)
                            {
                                LunaLog.LogError("[LMP]: Client failed to start within 2 seconds — cannot request NAT introduction");
                                MainSystem.Singleton.Status = "Network client failed to start";
                                return;
                            }
                        }

                        var localPort = NetworkMain.ClientConnection.Port;
                        if (localPort == 0)
                        {
                            LunaLog.LogWarning("[LMP]: Client bound port is 0 — this will break NAT punchthrough. Retrying Start()...");
                            NetworkMain.ClientConnection.Start();
                            Thread.Sleep(100);
                            localPort = NetworkMain.ClientConnection.Port;
                        }

                        var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<MsIntroductionMsgData>();
                        msgData.Id = serverId;
                        msgData.Token = MainSystem.UniqueIdentifier;
                        msgData.InternalEndpoint = new IPEndPoint(LunaNetUtils.GetOwnInternalIPv4Address(), localPort);
                        // Always send the real IPv6 address (GetOwnInternalIPv6Address returns loopback
                        // when none is available). The master server skips IPv6 introduction when it
                        // sees loopback, so this is safe.
                        msgData.InternalEndpoint6 = new IPEndPoint(LunaNetUtils.GetOwnInternalIPv6Address(), localPort);

                        var introduceMsg = NetworkMain.MstSrvMsgFactory.CreateNew<MainMstSrvMsg>(msgData);

                        MainSystem.Singleton.Status = "Requesting NAT introduction";
                        LunaLog.Log($"[LMP]: Selected connection method = NAT_Punch. " +
                                    $"Token: {MainSystem.UniqueIdentifier}, " +
                                    $"LocalPort: {localPort}, " +
                                    $"Internal Endpoint: {msgData.InternalEndpoint}, " +
                                    $"Internal Endpoint v6: {msgData.InternalEndpoint6}, " +
                                    $"Expected External: {expectedNatEndpoint}");
                        NetworkSender.QueueOutgoingMessage(introduceMsg);
                    }
                    catch (Exception e)
                    {
                        LunaLog.LogError($"[LMP]: Error connecting to server: {e}");
                    }
                }
            }
            else
            {
                LunaLog.LogError($"[LMP]: Cannot introduce to server {serverId} — not found in server list");
            }
        }

        /// <summary>
        /// Returns true if the server is running in a local LAN
        /// </summary>
        private static bool ServerIsInLocalLan(IPEndPoint serverEndPoint)
        {
            if (serverEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var ownNetwork = LunaNetUtils.GetOwnInternalIPv6Network();
                if (ownNetwork == null)
                    return false;

                // For IPv6, we strip both addresses down to the subnet portion (likely the first 64 bits) and compare them.
                // Because we only receive Global Unique Addresses from GetOwnInternalIPv6Network() (which are globally
                // unique, as the name suggests and the RFCs define), those being equal should mean both are on the same network.
                var ownBytes = ownNetwork.Address.GetAddressBytes();
                var serverBytes = serverEndPoint.Address.GetAddressBytes();
                // TODO IPv6: We currently assume an on-link prefix length of 64 bits, which is the most common case
                // and standardized as per the RFCs. UnicastIPAddressInformation.PrefixLength is not implemented yet,
                // and also wouldn't be reliable (hosts often assign their address as /128). A possible solution could be
                // checking whether serverEndPoint matches any configured on-link/no-gateway route.
                Array.Resize(ref ownBytes, 8);
                Array.Resize(ref serverBytes, 8);
                if (ownBytes.SequenceEqual(serverBytes))
                    return true;
            }
            else
            {
                return Equals(LunaNetUtils.GetOwnExternalIpAddress(), serverEndPoint.Address);
            }
            return false;
        }

        /// <summary>
        /// Returns true when two IPAddresses are the same, treating IPv4-mapped IPv6
        /// addresses as equal to their IPv4 counterpart.
        /// </summary>
        private static bool SameIpAddress(IPAddress a, IPAddress b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;

            // If both are IPv4-mapped IPv6, or one is IPv4 and the other is mapped IPv6,
            // compare the underlying IPv4 addresses.
            var a4 = a.IsIPv4MappedToIPv6 ? a.MapToIPv4() : a;
            var b4 = b.IsIPv4MappedToIPv6 ? b.MapToIPv4() : b;
            return a4.Equals(b4);
        }

        /// <summary>
        /// We received a NAT punchthrough response from the server, so connect to it.
        /// Called on the Lidgren receive thread.
        /// </summary>
        public static void HandleNatIntroductionSuccess(NetIncomingMessage msg)
        {
            var token = msg.ReadString();
            if (MainSystem.UniqueIdentifier != token)
            {
                LunaLog.LogError($"[LMP]: Incorrect client identifier in NAT introduction success response from {msg.SenderEndPoint}. Expected token: {MainSystem.UniqueIdentifier}, got: {token}");
                return;
            }

            var sender = msg.SenderEndPoint;
            if (expectedNatEndpoint != null && !SameIpAddress(expectedNatEndpoint.Address, sender.Address))
            {
                LunaLog.LogWarning($"[LMP]: NAT introduction success from unexpected endpoint {sender}. Expected {expectedNatEndpoint.Address}. Proceeding anyway.");
            }
            else
            {
                LunaLog.Log($"[LMP]: NAT introduction success from {sender}. Token: {MainSystem.UniqueIdentifier}");
            }

            receivedNATIntroductionSuccessResponse = true;

            // Log the current connection state so we can diagnose stale-connection issues
            var client = NetworkMain.ClientConnection;
            if (client != null)
            {
                LunaLog.Log($"[LMP]: Pre-connect state — peer status: {client.Status}, connection count: {client.ConnectionsCount}, " +
                            $"current network state: {MainSystem.NetworkState}");
            }

            NetworkConnection.ConnectToServerCore(new []{ sender }, Password);
        }
    }
}
