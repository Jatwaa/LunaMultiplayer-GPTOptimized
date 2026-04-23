// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Handshake.HandshakeMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.Mod;
using LmpClient.Systems.TimeSync;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Handshake;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using LmpCommon.ModFile;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Handshake
{
  public class HandshakeMessageHandler : SubSystem<HandshakeSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is HandshakeBaseMsgData data))
        return;
      if (data.HandshakeMessageType != HandshakeMessageType.Reply)
        throw new ArgumentOutOfRangeException();
      this.HandleHandshakeReplyReceivedMessage((HandshakeReplyMsgData) data);
    }

    public void HandleHandshakeReplyReceivedMessage(HandshakeReplyMsgData data)
    {
      TimeSyncSystem.ServerStartTime = data.ServerStartTime;
      if (data.Response == HandshakeReply.HandshookSuccessfully)
      {
        LmpClient.Base.System<ModSystem>.Singleton.Clear();
        LmpClient.Base.System<ModSystem>.Singleton.ModControl = data.ModControl;
        if (LmpClient.Base.System<ModSystem>.Singleton.ModControl)
        {
          if (LmpClient.Base.System<ModSystem>.Singleton.ModFileHandler.ParseModFile(ModFileParser.ReadModFileFromString(data.ModFileData)))
          {
            LunaLog.Log("[LMP]: Handshake successful");
            MainSystem.NetworkState = ClientState.Handshaked;
          }
          else
          {
            LunaLog.LogError("[LMP]: Failed to pass mod validation");
            NetworkConnection.Disconnect("[LMP]: Failed mod validation");
          }
        }
        else
        {
          LunaLog.Log("[LMP]: Handshake successful");
          MainSystem.NetworkState = ClientState.Handshaked;
        }
      }
      else
      {
        string str = "Handshake failure: " + data.Reason;
        LunaLog.Log(str);
        NetworkConnection.Disconnect(str);
      }
    }
  }
}
