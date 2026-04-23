// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ModApi.ModApiMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Events;
using LmpCommon;
using LmpCommon.Message.Data;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ModApi
{
  public class ModApiMessageHandler : SubSystem<ModApiSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ModMsgData data))
        return;
      byte[] numArray = Common.TrimArray<byte>(data.Data, data.NumBytes);
      ModApiEvent.onModMessageReceived.Fire(data.ModName, numArray);
    }
  }
}
