// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.WarpMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Warp;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Warp
{
  public class WarpMessageHandler : SubSystem<WarpSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is WarpBaseMsgData data))
        return;
      switch (data.WarpMessageType)
      {
        case WarpMessageType.SubspacesReply:
          WarpSubspacesReplyMsgData subspacesReplyMsgData = (WarpSubspacesReplyMsgData) data;
          for (int index1 = 0; index1 < subspacesReplyMsgData.SubspaceCount; ++index1)
          {
            int subspaceKey = subspacesReplyMsgData.Subspaces[index1].SubspaceKey;
            WarpMessageHandler.AddSubspace(subspaceKey, subspacesReplyMsgData.Subspaces[index1].SubspaceTime);
            for (int index2 = 0; index2 < subspacesReplyMsgData.Subspaces[index1].PlayerCount; ++index2)
            {
              string player = subspacesReplyMsgData.Subspaces[index1].Players[index2];
              if (SubSystem<WarpSystem>.System.ClientSubspaceList.ContainsKey(player))
                SubSystem<WarpSystem>.System.ClientSubspaceList[player] = subspaceKey;
              else
                SubSystem<WarpSystem>.System.ClientSubspaceList.TryAdd(player, subspaceKey);
            }
          }
          WarpMessageHandler.AddSubspace(-1, 0.0);
          MainSystem.NetworkState = ClientState.WarpsubspacesSynced;
          break;
        case WarpMessageType.NewSubspace:
          WarpNewSubspaceMsgData newSubspaceMsgData = (WarpNewSubspaceMsgData) data;
          WarpMessageHandler.AddSubspace(newSubspaceMsgData.SubspaceKey, newSubspaceMsgData.ServerTimeDifference);
          if (!(newSubspaceMsgData.PlayerCreator == SettingsSystem.CurrentSettings.PlayerName))
            break;
          SubSystem<WarpSystem>.System.WaitingSubspaceIdFromServer = false;
          SubSystem<WarpSystem>.System.SkipSubspaceProcess = true;
          SubSystem<WarpSystem>.System.CurrentSubspace = newSubspaceMsgData.SubspaceKey;
          break;
        case WarpMessageType.ChangeSubspace:
          WarpChangeSubspaceMsgData changeSubspaceMsgData = (WarpChangeSubspaceMsgData) data;
          SubSystem<WarpSystem>.System.ClientSubspaceList[changeSubspaceMsgData.PlayerName] = changeSubspaceMsgData.Subspace;
          break;
        default:
          LunaLog.LogError(string.Format("[LMP]: Unhandled WARP_MESSAGE type: {0}", (object) data.WarpMessageType));
          break;
      }
    }

    private static void AddSubspace(int subspaceId, double subspaceTime)
    {
      if (SubSystem<WarpSystem>.System.Subspaces.ContainsKey(subspaceId))
        return;
      SubSystem<WarpSystem>.System.Subspaces.TryAdd(subspaceId, subspaceTime);
    }
  }
}
