// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkReceiver
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpClient.Systems.Admin;
using LmpClient.Systems.Chat;
using LmpClient.Systems.CraftLibrary;
using LmpClient.Systems.Facility;
using LmpClient.Systems.Flag;
using LmpClient.Systems.Groups;
using LmpClient.Systems.Handshake;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.Lock;
using LmpClient.Systems.ModApi;
using LmpClient.Systems.Motd;
using LmpClient.Systems.PlayerColorSys;
using LmpClient.Systems.PlayerConnection;
using LmpClient.Systems.Scenario;
using LmpClient.Systems.Screenshot;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.ShareAchievements;
using LmpClient.Systems.ShareContracts;
using LmpClient.Systems.ShareExperimentalParts;
using LmpClient.Systems.ShareFunds;
using LmpClient.Systems.SharePurchaseParts;
using LmpClient.Systems.ShareReputation;
using LmpClient.Systems.ShareScience;
using LmpClient.Systems.ShareScienceSubject;
using LmpClient.Systems.ShareStrategy;
using LmpClient.Systems.ShareTechnology;
using LmpClient.Systems.ShareUpgradeableFacilities;
using LmpClient.Systems.Status;
using LmpClient.Systems.VesselActionGroupSys;
using LmpClient.Systems.VesselCoupleSys;
using LmpClient.Systems.VesselDecoupleSys;
using LmpClient.Systems.VesselFairingsSys;
using LmpClient.Systems.VesselFlightStateSys;
using LmpClient.Systems.VesselPartSyncCallSys;
using LmpClient.Systems.VesselPartSyncFieldSys;
using LmpClient.Systems.VesselPartSyncUiFieldSys;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Systems.VesselResourceSys;
using LmpClient.Systems.VesselUndockSys;
using LmpClient.Systems.VesselUpdateSys;
using LmpClient.Systems.Warp;
using LmpCommon.Enums;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using LmpCommon.Time;
using System;
using System.Threading;

namespace LmpClient.Network
{
  public class NetworkReceiver
  {
    public static void ReceiveMain()
    {
      LunaLog.Log("[LMP]: Receive thread started");
      try
      {
        while (!NetworkConnection.ResetRequested)
        {
          while (NetworkMain.ClientConnection.Status == NetPeerStatus.NotRunning)
            Thread.Sleep(50);
          NetIncomingMessage message;
          if (NetworkMain.ClientConnection.ReadMessage(out message))
          {
            switch (message.MessageType)
            {
              case NetIncomingMessageType.StatusChanged:
                if (message.ReadByte() == (byte) 7)
                {
                  NetworkConnection.Disconnect(message.ReadString());
                  break;
                }
                break;
              case NetIncomingMessageType.UnconnectedData:
                NetworkServerList.HandleServersList(message);
                break;
              case NetIncomingMessageType.Data:
                try
                {
                  IMessageBase msg = NetworkMain.SrvMsgFactory.Deserialize(message, LunaNetworkTime.UtcNow.Ticks);
                  if (msg != null)
                  {
                    NetworkReceiver.QueueMessageToSystem(msg as IServerMessageBase);
                    break;
                  }
                  break;
                }
                catch (Exception ex)
                {
                  LunaLog.LogError(string.Format("[LMP]: Error deserializing message! {0}", (object) ex));
                  break;
                }
              case NetIncomingMessageType.VerboseDebugMessage:
                LunaLog.Log("[Lidgren VERBOSE] " + message.ReadString());
                break;
              case NetIncomingMessageType.DebugMessage:
                LunaLog.Log("[Lidgren DEBUG] " + message.ReadString());
                break;
              case NetIncomingMessageType.WarningMessage:
                LunaLog.Log("[Lidgren WARNING] " + message.ReadString());
                break;
              case NetIncomingMessageType.NatIntroductionSuccess:
                NetworkServerList.HandleNatIntroduction(message);
                break;
              case NetIncomingMessageType.ConnectionLatencyUpdated:
                NetworkStatistics.PingSec = message.ReadFloat();
                break;
              default:
                LunaLog.Log(string.Format("[LMP]: LIDGREN: {0} -- {1}", (object) message.MessageType, (object) message.PeekString()));
                break;
            }
            NetworkMain.ClientConnection.Recycle(message);
          }
          else
            Thread.Sleep(SettingsSystem.CurrentSettings.SendReceiveMsInterval);
        }
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Receive thread error: {0}", (object) ex));
        NetworkMain.HandleDisconnectException(ex);
      }
      LunaLog.Log("[LMP]: Receive thread exited");
    }

    private static void QueueMessageToSystem(IServerMessageBase msg)
    {
      switch (msg.MessageType)
      {
        case ServerMessageType.Handshake:
          LmpClient.Base.System<HandshakeSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Settings:
          LmpClient.Base.System<SettingsSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Chat:
          LmpClient.Base.System<ChatSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.PlayerStatus:
          LmpClient.Base.System<StatusSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.PlayerColor:
          LmpClient.Base.System<PlayerColorSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Scenario:
          LmpClient.Base.System<ScenarioSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Kerbal:
          LmpClient.Base.System<KerbalSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Vessel:
          switch (((VesselBaseMsgData) msg.Data).VesselMessageType)
          {
            case VesselMessageType.Proto:
              LmpClient.Base.System<VesselProtoSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Remove:
              LmpClient.Base.System<VesselRemoveSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Position:
              LmpClient.Base.System<VesselPositionSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Flightstate:
              LmpClient.Base.System<VesselFlightStateSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Update:
              LmpClient.Base.System<VesselUpdateSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Resource:
              LmpClient.Base.System<VesselResourceSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Sync:
              return;
            case VesselMessageType.PartSyncField:
              LmpClient.Base.System<VesselPartSyncFieldSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.PartSyncUiField:
              LmpClient.Base.System<VesselPartSyncUiFieldSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.PartSyncCall:
              LmpClient.Base.System<VesselPartSyncCallSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.ActionGroup:
              LmpClient.Base.System<VesselActionGroupSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Fairing:
              LmpClient.Base.System<VesselFairingsSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Decouple:
              LmpClient.Base.System<VesselDecoupleSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Couple:
              LmpClient.Base.System<VesselCoupleSystem>.Singleton.EnqueueMessage(msg);
              return;
            case VesselMessageType.Undock:
              LmpClient.Base.System<VesselUndockSystem>.Singleton.EnqueueMessage(msg);
              return;
            default:
              return;
          }
        case ServerMessageType.CraftLibrary:
          LmpClient.Base.System<CraftLibrarySystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Flag:
          LmpClient.Base.System<FlagSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Motd:
          LmpClient.Base.System<MotdSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Warp:
          LmpClient.Base.System<WarpSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Lock:
          LmpClient.Base.System<LockSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Mod:
          LmpClient.Base.System<ModApiSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Admin:
          LmpClient.Base.System<AdminSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.PlayerConnection:
          LmpClient.Base.System<PlayerConnectionSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Groups:
          LmpClient.Base.System<GroupSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Facility:
          LmpClient.Base.System<FacilitySystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.Screenshot:
          LmpClient.Base.System<ScreenshotSystem>.Singleton.EnqueueMessage(msg);
          break;
        case ServerMessageType.ShareProgress:
          switch (((ShareProgressBaseMsgData) msg.Data).ShareProgressMessageType)
          {
            case ShareProgressMessageType.FundsUpdate:
              LmpClient.Base.System<ShareFundsSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.ScienceUpdate:
              LmpClient.Base.System<ShareScienceSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.ScienceSubjectUpdate:
              LmpClient.Base.System<ShareScienceSubjectSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.ReputationUpdate:
              LmpClient.Base.System<ShareReputationSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.TechnologyUpdate:
              LmpClient.Base.System<ShareTechnologySystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.ContractsUpdate:
              LmpClient.Base.System<ShareContractsSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.AchievementsUpdate:
              LmpClient.Base.System<ShareAchievementsSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.StrategyUpdate:
              LmpClient.Base.System<ShareStrategySystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.FacilityUpgrade:
              LmpClient.Base.System<ShareUpgradeableFacilitiesSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.PartPurchase:
              LmpClient.Base.System<SharePurchasePartsSystem>.Singleton.EnqueueMessage(msg);
              return;
            case ShareProgressMessageType.ExperimentalPart:
              LmpClient.Base.System<ShareExperimentalPartsSystem>.Singleton.EnqueueMessage(msg);
              return;
            default:
              return;
          }
        default:
          LunaLog.LogError(string.Format("[LMP]: Unhandled Message type {0}", (object) msg.MessageType));
          break;
      }
    }
  }
}
