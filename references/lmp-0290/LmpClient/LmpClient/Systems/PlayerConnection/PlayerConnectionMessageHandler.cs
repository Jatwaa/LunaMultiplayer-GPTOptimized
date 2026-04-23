// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerConnection.PlayerConnectionMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.Status;
using LmpClient.Systems.Warp;
using LmpCommon.Message.Data.PlayerConnection;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System.Collections.Concurrent;

namespace LmpClient.Systems.PlayerConnection
{
  public class PlayerConnectionMessageHandler : SubSystem<PlayerConnectionSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is PlayerConnectionBaseMsgData data))
        return;
      string playerName = data.PlayerName;
      switch (data.PlayerConnectionMessageType)
      {
        case PlayerConnectionMessageType.Join:
          LunaScreenMsg.PostScreenMessage(playerName + " has joined the server", 3f, (ScreenMessageStyle) 0);
          break;
        case PlayerConnectionMessageType.Leave:
          LmpClient.Base.System<WarpSystem>.Singleton.RemovePlayer(playerName);
          LmpClient.Base.System<StatusSystem>.Singleton.RemovePlayer(playerName);
          LunaScreenMsg.PostScreenMessage(playerName + " has left the server", 3f, (ScreenMessageStyle) 0);
          break;
      }
    }
  }
}
