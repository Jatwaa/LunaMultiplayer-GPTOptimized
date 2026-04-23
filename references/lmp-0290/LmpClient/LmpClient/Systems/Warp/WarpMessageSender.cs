// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.WarpMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.TimeSync;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Warp;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Warp
{
  public class WarpMessageSender : SubSystem<WarpSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<WarpCliMsg>(msg))));

    public void SendWarpSubspacesRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<WarpCliMsg, WarpSubspacesRequestMsgData>())));

    public void SendChangeSubspaceMsg(int subspaceId)
    {
      WarpChangeSubspaceMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<WarpChangeSubspaceMsgData>();
      newMessageData.PlayerName = SettingsSystem.CurrentSettings.PlayerName;
      newMessageData.Subspace = subspaceId;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendNewSubspace()
    {
      WarpNewSubspaceMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<WarpNewSubspaceMsgData>();
      newMessageData.ServerTimeDifference = TimeSyncSystem.UniversalTime - TimeSyncSystem.ServerClockSec;
      newMessageData.PlayerCreator = SettingsSystem.CurrentSettings.PlayerName;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
