// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkConnection
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpClient.Base;
using LmpClient.ModuleStore.Patching;
using LmpClient.Systems.Network;
using LmpCommon;
using LmpCommon.Enums;
using LmpCommon.Message.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UniLinq;

namespace LmpClient.Network
{
  public class NetworkConnection
  {
    private static readonly object DisconnectLock = new object();
    public static volatile bool ResetRequested;

    public static void Disconnect(string reason = "unknown")
    {
      lock (NetworkConnection.DisconnectLock)
      {
        if (MainSystem.NetworkState <= ClientState.Disconnected)
          return;
        MainSystem.NetworkState = ClientState.DisconnectRequested;
        LunaLog.Log("[LMP]: Disconnected, reason: " + reason);
        if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight)
          MainSystem.Singleton.ForceQuit = true;
        else
          NetworkSystem.DisplayDisconnectMessage = true;
        MainSystem.Singleton.Status = "Disconnected: " + reason;
        NetworkMain.ClientConnection.Disconnect(reason);
        NetworkMain.ClientConnection.Shutdown(reason);
        NetworkMain.ResetConnectionStaticsAndQueues();
      }
    }

    public static void ConnectToServer(string hostname, int port, string password)
    {
      IPEndPoint[] array = Enumerable.ToArray<IPEndPoint>(Enumerable.Select<IPAddress, IPEndPoint>((IEnumerable<IPAddress>) LunaNetUtils.CreateAddressFromString(hostname), (Func<IPAddress, IPEndPoint>) (addr => new IPEndPoint(addr, port))));
      if (array.Length == 0)
      {
        MainSystem.Singleton.Status = "Hostname resolution failed, check for typos";
        LunaLog.LogError("[LMP]: Hostname resolution failed, check for typos");
        NetworkConnection.Disconnect("Hostname resolution failed");
      }
      NetworkConnection.ConnectToServer(array, password);
    }

    public static void ConnectToServer(IPEndPoint[] endpoints, string password)
    {
      if (MainSystem.NetworkState > ClientState.Disconnected || endpoints == null || endpoints.Length == 0)
        return;
      MainSystem.NetworkState = ClientState.Connecting;
      SystemBase.TaskFactory.StartNew((Action) (() =>
      {
        while (!PartModuleRunner.Ready)
        {
          MainSystem.Singleton.Status = "Patching part modules (runs on every restart). " + PartModuleRunner.GetPercentage() + "%";
          Thread.Sleep(50);
        }
        foreach (IPEndPoint endpoint in endpoints)
        {
          if (endpoint != null)
          {
            MainSystem.Singleton.Status = string.Format("Connecting to {0}:{1}", (object) endpoint.Address, (object) endpoint.Port);
            LunaLog.Log(string.Format("[LMP]: Connecting to {0} port {1}", (object) endpoint.Address, (object) endpoint.Port));
            try
            {
              NetClient clientConnection = NetworkMain.ClientConnection;
              if (clientConnection.Status == NetPeerStatus.NotRunning)
              {
                LunaLog.Log("[LMP]: Starting client");
                clientConnection.Start();
              }
              while (clientConnection.Status != NetPeerStatus.Running)
                Thread.Sleep(50);
              NetOutgoingMessage message = clientConnection.CreateMessage(password.GetByteCount());
              message.Write(password);
              NetConnection netConnection = clientConnection.Connect(endpoint, message);
              if (netConnection == null)
              {
                LunaLog.LogError("[LMP]: Invalid connection state, connected without connection");
                clientConnection.Disconnect("Invalid state");
                break;
              }
              clientConnection.FlushSendQueue();
              while (netConnection.Status == NetConnectionStatus.InitiatedConnect || netConnection.Status == NetConnectionStatus.None)
                Thread.Sleep(50);
              if (clientConnection.ConnectionStatus == NetConnectionStatus.Connected)
              {
                LunaLog.Log(string.Format("[LMP]: Connected to {0}:{1}", (object) endpoint.Address, (object) endpoint.Port));
                MainSystem.NetworkState = ClientState.Connected;
                break;
              }
              LunaLog.Log(string.Format("[LMP]: Initial connection timeout to {0}:{1}", (object) endpoint.Address, (object) endpoint.Port));
              clientConnection.Disconnect("Initial connection timeout");
            }
            catch (Exception ex)
            {
              NetworkMain.HandleDisconnectException(ex);
            }
          }
        }
        if (MainSystem.NetworkState >= ClientState.Connected)
          return;
        NetworkConnection.Disconnect(MainSystem.NetworkState == ClientState.Connecting ? "Initial connection timeout" : "Cancelled connection");
      }));
    }
  }
}
