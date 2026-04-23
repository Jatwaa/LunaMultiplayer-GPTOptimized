// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Ping.PingSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Network;
using LmpCommon;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace LmpClient.Systems.Ping
{
  public class PingSystem : LmpClient.Base.System<PingSystem>
  {
    private const float PingTimeoutInSec = 7.5f;
    private static readonly HashSet<(long, bool)> RunningPings = new HashSet<(long, bool)>();

    public PingSystem() => this.SetupRoutine(new RoutineDefinition(100, RoutineExecution.Update, new Action(PingSystem.PerformPings)));

    private static ConcurrentBag<long> PingQueue { get; } = new ConcurrentBag<long>();

    protected override bool AlwaysEnabled => true;

    public override string SystemName { get; } = nameof (PingSystem);

    public static void QueuePing(long id) => PingSystem.PingQueue.Add(id);

    private static void PerformPings()
    {
      long result;
      while (PingSystem.PingQueue.TryTake(out result))
      {
        bool[] flagArray = new bool[2]{ true, false };
        foreach (bool ipv6 in flagArray)
        {
          if (!PingSystem.RunningPings.Contains((result, ipv6)))
          {
            PingSystem.RunningPings.Add((result, ipv6));
            MainSystem.Singleton.StartCoroutine(PingSystem.PingUpdate(result, ipv6));
          }
        }
      }
    }

    private static IEnumerator PingUpdate(long serverId, bool ipv6)
    {
      ServerInfo serverInfo;
      if (NetworkServerList.Servers.TryGetValue(serverId, out serverInfo))
      {
        IPAddress host;
        if (ipv6)
        {
          host = serverInfo.InternalEndpoint6.Address;
          if (host.Equals((object) IPAddress.IPv6Loopback))
          {
            serverInfo.Ping6 = int.MaxValue;
            serverInfo.DisplayedPing6 = "X";
            PingSystem.RunningPings.Remove((serverId, ipv6));
            yield break;
          }
        }
        else
        {
          host = serverInfo.ExternalEndpoint.Address;
          if (host.Equals((object) IPAddress.Loopback))
          {
            serverInfo.Ping = int.MaxValue;
            serverInfo.DisplayedPing = "X";
            PingSystem.RunningPings.Remove((serverId, ipv6));
            yield break;
          }
        }
        UnityEngine.Ping ping = new UnityEngine.Ping(host.ToString());
        for (float elapsedSecs = 0.0f; !ping.isDone && (double) elapsedSecs < 7.5; elapsedSecs += Time.deltaTime)
          yield return (object) null;
        bool finished = ping.isDone;
        int result = finished ? ping.time : int.MaxValue;
        ping.DestroyPing();
        ServerInfo server;
        if (NetworkServerList.Servers.TryGetValue(serverId, out server))
        {
          if (ipv6)
          {
            server.Ping6 = result;
            server.DisplayedPing6 = finished ? result.ToString() : "∞";
          }
          else
          {
            server.Ping = result;
            server.DisplayedPing = finished ? result.ToString() : "∞";
          }
        }
        PingSystem.RunningPings.Remove((serverId, ipv6));
        host = (IPAddress) null;
        ping = (UnityEngine.Ping) null;
        server = (ServerInfo) null;
      }
    }
  }
}
