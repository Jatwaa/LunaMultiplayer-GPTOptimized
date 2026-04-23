// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Network.NetworkSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Localization;
using LmpClient.Network;
using LmpClient.Systems.Flag;
using LmpClient.Systems.Handshake;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.Lock;
using LmpClient.Systems.PlayerColorSys;
using LmpClient.Systems.PlayerConnection;
using LmpClient.Systems.Scenario;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.Status;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselSyncSys;
using LmpClient.Systems.Warp;
using LmpClient.Utilities;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;

namespace LmpClient.Systems.Network
{
  public class NetworkSystem : LmpClient.Base.System<NetworkSystem>
  {
    private static DateTime _lastStateTime = DateTime.MinValue;

    public static bool DisplayDisconnectMessage { get; set; }

    public NetworkSystem()
    {
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.NetworkUpdate)));
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(NetworkSystem.ProcessNetworkStatusChanges)));
      this.SetupRoutine(new RoutineDefinition(1000, RoutineExecution.Update, new Action(NetworkSystem.ShowDisconnectMessage)));
    }

    public static ClientState? NetworkStatus { private get; set; }

    public override string SystemName { get; } = nameof (NetworkSystem);

    protected override bool AlwaysEnabled => true;

    public override int ExecutionOrder => int.MinValue;

    private static void ProcessNetworkStatusChanges()
    {
      if (!NetworkSystem.NetworkStatus.HasValue)
        return;
      NetworkEvent.onNetworkStatusChanged.Fire(NetworkSystem.NetworkStatus.Value);
      NetworkSystem.NetworkStatus = new ClientState?();
    }

    private void NetworkUpdate()
    {
      switch (MainSystem.NetworkState)
      {
        case ClientState.DisconnectRequested:
          break;
        case ClientState.Disconnected:
          break;
        case ClientState.Connecting:
          this.ChangeRoutineExecutionInterval(RoutineExecution.Update, nameof (NetworkUpdate), 0);
          break;
        case ClientState.Connected:
          LmpClient.Base.System<HandshakeSystem>.Singleton.Enabled = true;
          MainSystem.Singleton.Status = "Connected";
          MainSystem.NetworkState = ClientState.Handshaking;
          LmpClient.Base.System<HandshakeSystem>.Singleton.MessageSender.SendHandshakeRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.Handshaking:
          MainSystem.Singleton.Status = "Waiting for handshake response";
          if (!NetworkSystem.ConnectionIsStuck(60000))
            break;
          LunaLog.Log("[LMP]: Failed to get a handshake response after 60 seconds. Sending the handshake again...");
          MainSystem.NetworkState = ClientState.Connected;
          break;
        case ClientState.Handshaked:
          MainSystem.Singleton.Status = "Handshaking successful";
          LmpClient.Base.System<SettingsSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingSettings;
          LmpClient.Base.System<SettingsSystem>.Singleton.MessageSender.SendSettingsRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingSettings:
          MainSystem.Singleton.Status = "Syncing settings";
          if (!NetworkSystem.ConnectionIsStuck())
            break;
          MainSystem.NetworkState = ClientState.Handshaked;
          break;
        case ClientState.SettingsSynced:
          MainSystem.Singleton.Status = "Settings synced";
          if (!SettingsSystem.ValidateSettings())
            break;
          LmpClient.Base.System<KerbalSystem>.Singleton.Enabled = true;
          LmpClient.Base.System<VesselProtoSystem>.Singleton.Enabled = true;
          LmpClient.Base.System<VesselSyncSystem>.Singleton.Enabled = true;
          LmpClient.Base.System<VesselSyncSystem>.Singleton.MessageSender.SendVesselsSyncMsg();
          MainSystem.NetworkState = ClientState.SyncingKerbals;
          LmpClient.Base.System<KerbalSystem>.Singleton.MessageSender.SendKerbalsRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingKerbals:
          MainSystem.Singleton.Status = "Syncing kerbals";
          if (!NetworkSystem.ConnectionIsStuck(10000))
            break;
          MainSystem.NetworkState = ClientState.SettingsSynced;
          break;
        case ClientState.KerbalsSynced:
          MainSystem.Singleton.Status = "Kerbals synced";
          LmpClient.Base.System<WarpSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingWarpsubspaces;
          LmpClient.Base.System<WarpSystem>.Singleton.MessageSender.SendWarpSubspacesRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingWarpsubspaces:
          MainSystem.Singleton.Status = "Syncing warp subspaces";
          if (!NetworkSystem.ConnectionIsStuck())
            break;
          MainSystem.NetworkState = ClientState.KerbalsSynced;
          break;
        case ClientState.WarpsubspacesSynced:
          MainSystem.Singleton.Status = "Warp subspaces synced";
          LmpClient.Base.System<PlayerColorSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingColors;
          LmpClient.Base.System<PlayerColorSystem>.Singleton.MessageSender.SendColorsRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingColors:
          MainSystem.Singleton.Status = "Syncing player colors";
          if (!NetworkSystem.ConnectionIsStuck())
            break;
          MainSystem.NetworkState = ClientState.WarpsubspacesSynced;
          break;
        case ClientState.ColorsSynced:
          MainSystem.Singleton.Status = "Player colors synced";
          LmpClient.Base.System<FlagSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingFlags;
          LmpClient.Base.System<FlagSystem>.Singleton.MessageSender.SendFlagsRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingFlags:
          MainSystem.Singleton.Status = "Syncing flags";
          if (!NetworkSystem.ConnectionIsStuck(10000))
            break;
          MainSystem.NetworkState = ClientState.ColorsSynced;
          break;
        case ClientState.FlagsSynced:
          MainSystem.Singleton.Status = "Flags synced";
          LmpClient.Base.System<StatusSystem>.Singleton.Enabled = true;
          LmpClient.Base.System<PlayerConnectionSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingPlayers;
          LmpClient.Base.System<StatusSystem>.Singleton.MessageSender.SendPlayersRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingPlayers:
          MainSystem.Singleton.Status = "Syncing players";
          if (!NetworkSystem.ConnectionIsStuck())
            break;
          MainSystem.NetworkState = ClientState.FlagsSynced;
          break;
        case ClientState.PlayersSynced:
          MainSystem.Singleton.Status = "Players synced";
          LmpClient.Base.System<ScenarioSystem>.Singleton.Enabled = true;
          MainSystem.NetworkState = ClientState.SyncingScenarios;
          LmpClient.Base.System<ScenarioSystem>.Singleton.MessageSender.SendScenariosRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingScenarios:
          MainSystem.Singleton.Status = "Syncing scenarios";
          if (!NetworkSystem.ConnectionIsStuck(10000))
            break;
          MainSystem.NetworkState = ClientState.PlayersSynced;
          break;
        case ClientState.ScenariosSynced:
          MainSystem.Singleton.Status = "Scenarios synced";
          MainSystem.NetworkState = ClientState.SyncingLocks;
          LmpClient.Base.System<LockSystem>.Singleton.Enabled = true;
          LmpClient.Base.System<LockSystem>.Singleton.MessageSender.SendLocksRequest();
          NetworkSystem._lastStateTime = LunaComputerTime.UtcNow;
          break;
        case ClientState.SyncingLocks:
          MainSystem.Singleton.Status = "Syncing locks";
          if (!NetworkSystem.ConnectionIsStuck())
            break;
          MainSystem.NetworkState = ClientState.ScenariosSynced;
          break;
        case ClientState.LocksSynced:
          MainSystem.Singleton.Status = "Starting";
          MainSystem.Singleton.StartGame = true;
          MainSystem.NetworkState = ClientState.Starting;
          break;
        case ClientState.Starting:
          this.ChangeRoutineExecutionInterval(RoutineExecution.Update, nameof (NetworkUpdate), 1500);
          MainSystem.Singleton.Status = "Running";
          CommonUtil.Reserve20Mb();
          LunaLog.Log("[LMP]: All systems up and running. Поехали!");
          if (HighLogic.LoadedScene != 5)
            break;
          MainSystem.NetworkState = ClientState.Running;
          NetworkMain.DeleteAllTheControlLocksSoTheSpaceCentreBugGoesAway();
          break;
        case ClientState.Running:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static bool ConnectionIsStuck(int maxIdleMiliseconds = 2000)
    {
      if ((LunaComputerTime.UtcNow - NetworkSystem._lastStateTime).TotalMilliseconds <= (double) maxIdleMiliseconds)
        return false;
      LunaLog.LogWarning(string.Format("Connection got stuck while connecting after waiting {0} ms, resending last request!", (object) maxIdleMiliseconds));
      return true;
    }

    private static void ShowDisconnectMessage()
    {
      if (HighLogic.LoadedScene < 5)
        NetworkSystem.DisplayDisconnectMessage = false;
      if (!NetworkSystem.DisplayDisconnectMessage)
        return;
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.Disconected, 2f, (ScreenMessageStyle) 0);
    }
  }
}
