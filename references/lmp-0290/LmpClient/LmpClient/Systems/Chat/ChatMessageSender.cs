// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Chat.ChatMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.Chat
{
  public class ChatMessageSender : SubSystem<ChatSystem>, IMessageSender
  {
    public void SendMessage(IMessageData messageData) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ChatCliMsg>(messageData))));

    public void SendChatMsg(string text, bool relay = true)
    {
      ChatMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ChatMsgData>();
      newMessageData.From = SettingsSystem.CurrentSettings.PlayerName;
      newMessageData.Text = text;
      newMessageData.Relay = relay;
      SubSystem<ChatSystem>.System.MessageSender.SendMessage((IMessageData) newMessageData);
    }
  }
}
