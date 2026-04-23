// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkMain
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpClient.Windows.BannedParts;
using LmpCommon.Message;
using LmpCommon.Message.Interface;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LmpClient.Network
{
  public class NetworkMain
  {
    public const int MaxMtuSize = 8191;

    public static ClientMessageFactory CliMsgFactory { get; } = new ClientMessageFactory();

    public static ServerMessageFactory SrvMsgFactory { get; } = new ServerMessageFactory();

    public static MasterServerMessageFactory MstSrvMsgFactory { get; } = new MasterServerMessageFactory();

    private static Task ReceiveThread { get; set; }

    private static Task SendThread { get; set; }

    public static NetPeerConfiguration Config { get; } = new NetPeerConfiguration("LMP")
    {
      UseMessageRecycling = true,
      ReceiveBufferSize = 500000,
      SendBufferSize = 500000,
      SuppressUnreliableUnorderedAcks = true,
      PingInterval = (float) TimeSpan.FromMilliseconds((double) SettingsSystem.CurrentSettings.HearbeatMsInterval).TotalSeconds,
      ConnectionTimeout = SettingsSystem.CurrentSettings.TimeoutSeconds,
      MaximumHandshakeAttempts = SettingsSystem.CurrentSettings.ConnectionTries,
      ResendHandshakeInterval = (float) SettingsSystem.CurrentSettings.MsBetweenConnectionTries / 1000f,
      MaximumTransmissionUnit = SettingsSystem.CurrentSettings.Mtu,
      AutoExpandMTU = SettingsSystem.CurrentSettings.AutoExpandMtu,
      LocalAddress = IPAddress.IPv6Any,
      DualStack = true
    };

    public static NetClient ClientConnection { get; private set; }

    public static void DeleteAllTheControlLocksSoTheSpaceCentreBugGoesAway()
    {
      LunaLog.Log(string.Format("[LMP]: Clearing {0} control locks", (object) InputLockManager.lockStack.Count));
      InputLockManager.ClearControlLocks();
    }

    public static void RandomizeBadConnectionValues()
    {
      Random random = new Random();
      LunaNetworkTime.SimulatedMsTimeOffset = (float) random.Next(-500, 500);
      NetworkMain.Config.SimulatedMinimumLatency = (float) random.Next(50, 250) / 1000f;
      NetworkMain.Config.SimulatedRandomLatency = (float) random.Next(10, 250) / 1000f;
      NetworkMain.Config.SimulatedDuplicatesChance = (float) random.Next(10, 50) / 1000f;
      NetworkMain.Config.SimulatedLoss = (float) random.Next(10, 30) / 1000f;
    }

    public static void ResetBadConnectionValues()
    {
      LunaNetworkTime.SimulatedMsTimeOffset = 0.0f;
      NetworkMain.Config.SimulatedMinimumLatency = 0.0f;
      NetworkMain.Config.SimulatedRandomLatency = 0.0f;
      NetworkMain.Config.SimulatedDuplicatesChance = 0.0f;
      NetworkMain.Config.SimulatedLoss = 0.0f;
    }

    public static void ResetConnectionStaticsAndQueues() => NetworkSender.OutgoingMessages = new ConcurrentQueue<IMessageBase>();

    public static void AwakeNetworkSystem()
    {
      NetworkMain.Config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
      NetworkMain.Config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
      NetworkMain.Config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
      NetworkMain.Config.EnableMessageType(NetIncomingMessageType.DebugMessage);
      if (!Socket.OSSupportsIPv6)
      {
        NetworkMain.Config.LocalAddress = IPAddress.Any;
        NetworkMain.Config.DualStack = false;
      }
      NetworkMain.ClientConnection = new NetClient(NetworkMain.Config.Clone());
    }

    public static void ResetNetworkSystem()
    {
      NetworkConnection.ResetRequested = true;
      Window<BannedPartsResourcesWindow>.Singleton.Display = false;
      if (NetworkMain.ClientConnection.Status > NetPeerStatus.NotRunning)
      {
        NetworkMain.ClientConnection.Shutdown("Disconnected");
        Thread.Sleep(1000);
      }
      if (NetworkMain.SendThread != null && !NetworkMain.SendThread.IsCompleted)
        NetworkMain.SendThread.Wait(1000);
      if (NetworkMain.ReceiveThread != null && !NetworkMain.ReceiveThread.IsCompleted)
        NetworkMain.ReceiveThread.Wait(1000);
      NetworkConnection.ResetRequested = false;
      NetworkMain.ReceiveThread = SystemBase.LongRunTaskFactory.StartNew(new Action(NetworkReceiver.ReceiveMain));
      NetworkMain.SendThread = SystemBase.LongRunTaskFactory.StartNew(new Action(NetworkSender.SendMain));
    }

    public static void HandleDisconnectException(Exception e)
    {
      if (e.InnerException != null)
      {
        LunaLog.LogError(string.Format("[LMP]: Connection error: {0}, {1}", (object) e.Message, (object) e.InnerException));
        NetworkConnection.Disconnect("Connection error: " + e.Message + ", " + e.InnerException.Message);
      }
      else
      {
        LunaLog.LogError(string.Format("[LMP]: Connection error: {0}", (object) e));
        NetworkConnection.Disconnect("Connection error: " + e.Message);
      }
    }
  }
}
