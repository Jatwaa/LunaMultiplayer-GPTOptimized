// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkServerList
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpClient.Systems.Ping;
using LmpCommon;
using LmpCommon.Message.Data.MasterServer;
using LmpCommon.Message.Interface;
using LmpCommon.Message.MasterServer;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LmpClient.Network
{
  public class NetworkServerList
  {
    public static string Password { get; set; } = string.Empty;

    public static ConcurrentDictionary<long, ServerInfo> Servers { get; } = new ConcurrentDictionary<long, ServerInfo>();

    public static void RequestServers()
    {
      NetworkServerList.Servers.Clear();
      NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.MstSrvMsgFactory.CreateNew<MainMstSrvMsg>((IMessageData) NetworkMain.CliMsgFactory.CreateNewMessageData<MsRequestServersMsgData>()));
    }

    public static void HandleServersList(NetIncomingMessage msg)
    {
      try
      {
        if (!(NetworkMain.MstSrvMsgFactory.Deserialize(msg, LunaNetworkTime.UtcNow.Ticks).Data is MsReplyServersMsgData data))
          return;
        if (!LmpVersioning.IsCompatible(data.ServerVersion))
          return;
        ServerInfo server = new ServerInfo()
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
        Array.Copy((Array) data.Color, (Array) server.Color, 3);
        NetworkServerList.Servers.AddOrUpdate(data.Id, server, (Func<long, ServerInfo, ServerInfo>) ((l, info) => NetworkServerList.MergeServerInfos(info, server)));
        PingSystem.QueuePing(data.Id);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Invalid server list reply msg: {0}", (object) ex));
      }
    }

    private static ServerInfo MergeServerInfos(ServerInfo existing, ServerInfo fresh)
    {
      if (string.IsNullOrEmpty(fresh.Country) && !string.IsNullOrEmpty(existing.Country))
        fresh.Country = existing.Country;
      return fresh;
    }

    public static void IntroduceToServer(long serverId)
    {
      ServerInfo serverInfo;
      if (!NetworkServerList.Servers.TryGetValue(serverId, out serverInfo))
        return;
      if (NetworkServerList.ServerIsInLocalLan(serverInfo.ExternalEndpoint) || NetworkServerList.ServerIsInLocalLan(serverInfo.InternalEndpoint6))
      {
        LunaLog.Log("Server is in LAN. Skipping NAT punch");
        List<IPEndPoint> ipEndPointList = new List<IPEndPoint>();
        if (!serverInfo.InternalEndpoint6.Address.Equals((object) IPAddress.IPv6Loopback))
          ipEndPointList.Add(serverInfo.InternalEndpoint6);
        if (!serverInfo.InternalEndpoint.Address.Equals((object) IPAddress.Loopback))
          ipEndPointList.Add(serverInfo.InternalEndpoint);
        NetworkConnection.ConnectToServer(ipEndPointList.ToArray(), NetworkServerList.Password);
      }
      else
      {
        try
        {
          MsIntroductionMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<MsIntroductionMsgData>();
          newMessageData.Id = serverId;
          newMessageData.Token = MainSystem.UniqueIdentifier;
          newMessageData.InternalEndpoint = new IPEndPoint(LunaNetUtils.GetOwnInternalIPv4Address(), NetworkMain.ClientConnection.Port);
          newMessageData.InternalEndpoint6 = new IPEndPoint(LunaNetUtils.GetOwnInternalIPv6Address(), NetworkMain.ClientConnection.Port);
          MainMstSrvMsg message = NetworkMain.MstSrvMsgFactory.CreateNew<MainMstSrvMsg>((IMessageData) newMessageData);
          MainSystem.Singleton.Status = string.Empty;
          LunaLog.Log("[LMP]: Sending NAT introduction to master servers. Token: " + MainSystem.UniqueIdentifier);
          NetworkSender.QueueOutgoingMessage((IMessageBase) message);
        }
        catch (Exception ex)
        {
          LunaLog.LogError(string.Format("[LMP]: Error connecting to server: {0}", (object) ex));
        }
      }
    }

    private static bool ServerIsInLocalLan(IPEndPoint serverEndPoint)
    {
      UnicastIPAddressInformation internalIpv6Network = LunaNetUtils.GetOwnInternalIPv6Network();
      if (internalIpv6Network != null && serverEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
      {
        byte[] addressBytes1 = internalIpv6Network.Address.GetAddressBytes();
        byte[] addressBytes2 = serverEndPoint.Address.GetAddressBytes();
        Array.Resize<byte>(ref addressBytes1, 8);
        Array.Resize<byte>(ref addressBytes2, 8);
        if (addressBytes1 == addressBytes2)
          return true;
      }
      return object.Equals((object) LunaNetUtils.GetOwnExternalIpAddress(), (object) serverEndPoint.Address);
    }

    public static void HandleNatIntroduction(NetIncomingMessage msg)
    {
      if (MainSystem.UniqueIdentifier == msg.ReadString())
      {
        LunaLog.Log(string.Format("[LMP]: Nat introduction success against {0}. Token: {1}", (object) msg.SenderEndPoint, (object) MainSystem.UniqueIdentifier));
        NetworkConnection.ConnectToServer(new IPEndPoint[1]
        {
          msg.SenderEndPoint
        }, NetworkServerList.Password);
      }
      else
        LunaLog.LogError(string.Format("[LMP]: Nat introduction failed against {0}. Token: {1}", (object) msg.SenderEndPoint, (object) MainSystem.UniqueIdentifier));
    }
  }
}
