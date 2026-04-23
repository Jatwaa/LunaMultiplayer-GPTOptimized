// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.Interface.IMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Base.Interface
{
  public interface IMessageHandler
  {
    ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; }

    void HandleMessage(IServerMessageBase msg);
  }
}
