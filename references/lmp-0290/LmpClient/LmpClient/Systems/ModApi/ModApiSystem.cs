// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ModApi.ModApiSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Network;
using LmpCommon.Message.Data;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.ModApi
{
  public class ModApiSystem : MessageSystem<ModApiSystem, ModApiMessageSender, ModApiMessageHandler>
  {
    public override string SystemName { get; } = nameof (ModApiSystem);

    public override int ExecutionOrder => -2147483646;

    public void SendModMessage(string modName, byte[] messageData, bool relay) => this.SendModMessage(modName, messageData, messageData.Length, relay);

    public void SendModMessage(string modName, byte[] messageData, int numBytes, bool relay)
    {
      if (modName == null)
        return;
      if (messageData == null)
      {
        LunaLog.LogError("[LMP]: " + modName + " attemped to send a null Message");
      }
      else
      {
        ModMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ModMsgData>();
        if (newMessageData.Data.Length < numBytes)
          newMessageData.Data = new byte[numBytes];
        Array.Copy((Array) messageData, (Array) newMessageData.Data, numBytes);
        newMessageData.NumBytes = numBytes;
        newMessageData.Relay = relay;
        newMessageData.ModName = modName;
        this.MessageSender.SendMessage((IMessageData) newMessageData);
      }
    }
  }
}
