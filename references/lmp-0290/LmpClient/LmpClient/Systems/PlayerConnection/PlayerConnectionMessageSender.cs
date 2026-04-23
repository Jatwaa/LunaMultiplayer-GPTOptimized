// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerConnection.PlayerConnectionMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.PlayerConnection
{
  public class PlayerConnectionMessageSender : SubSystem<PlayerConnectionSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => throw new Exception("We don't send this messages!");
  }
}
