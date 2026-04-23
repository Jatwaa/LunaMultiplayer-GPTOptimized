// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using LmpCommon.Enums;
using LmpCommon.Message.Interface;
using LmpCommon.RepoRetrievers;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace LmpClient.Network
{
  public class NetworkSender
  {
    public static ConcurrentQueue<IMessageBase> OutgoingMessages { get; set; } = new ConcurrentQueue<IMessageBase>();

    public static void SendMain()
    {
      LunaLog.Log("[LMP]: Send thread started");
      try
      {
        while (!NetworkConnection.ResetRequested)
        {
          IMessageBase result;
          if (NetworkSender.OutgoingMessages.Count > 0 && NetworkSender.OutgoingMessages.TryDequeue(out result))
            NetworkSender.SendNetworkMessage(result);
          else
            Thread.Sleep(SettingsSystem.CurrentSettings.SendReceiveMsInterval);
        }
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Send thread error: {0}", (object) ex));
      }
      LunaLog.Log("[LMP]: Send thread exited");
    }

    public static void QueueOutgoingMessage(IMessageBase message) => NetworkSender.OutgoingMessages.Enqueue(message);

    private static void SendNetworkMessage(IMessageBase message)
    {
      message.Data.SentTime = LunaNetworkTime.UtcNow.Ticks;
      try
      {
        if (message is IMasterServerMessageBase)
        {
          if (NetworkMain.ClientConnection.Status == NetPeerStatus.NotRunning)
          {
            LunaLog.Log("Starting client to send unconnected message");
            NetworkMain.ClientConnection.Start();
          }
          while (NetworkMain.ClientConnection.Status != NetPeerStatus.Running)
          {
            LunaLog.Log("Waiting for client to start up to send unconnected message");
            Thread.Sleep(50);
          }
          IPEndPoint[] ipEndPointArray;
          if (string.IsNullOrEmpty(SettingsSystem.CurrentSettings.CustomMasterServer))
            ipEndPointArray = MasterServerRetriever.MasterServers.GetValues;
          else
            ipEndPointArray = new IPEndPoint[1]
            {
              LunaNetUtils.CreateEndpointFromString(SettingsSystem.CurrentSettings.CustomMasterServer)
            };
          foreach (IPEndPoint recipient in ipEndPointArray)
          {
            NetOutgoingMessage message1 = NetworkMain.ClientConnection.CreateMessage(message.GetMessageSize());
            message.Serialize(message1);
            NetworkMain.ClientConnection.SendUnconnectedMessage(message1, recipient);
          }
          NetworkMain.ClientConnection.FlushSendQueue();
        }
        else
        {
          if (NetworkMain.ClientConnection == null || NetworkMain.ClientConnection.Status == NetPeerStatus.NotRunning || MainSystem.NetworkState < ClientState.Connected)
            return;
          NetOutgoingMessage message2 = NetworkMain.ClientConnection.CreateMessage(message.GetMessageSize());
          message.Serialize(message2);
          int num = (int) NetworkMain.ClientConnection.SendMessage(message2, message.NetDeliveryMethod, message.Channel);
          NetworkMain.ClientConnection.FlushSendQueue();
        }
        message.Recycle();
      }
      catch (Exception ex)
      {
        NetworkMain.HandleDisconnectException(ex);
      }
    }
  }
}
