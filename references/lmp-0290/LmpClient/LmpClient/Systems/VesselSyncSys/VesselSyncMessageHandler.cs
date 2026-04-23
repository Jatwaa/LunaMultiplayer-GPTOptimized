// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselSyncSys.VesselSyncMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.VesselSyncSys
{
  public class VesselSyncMessageHandler : SubSystem<VesselSyncSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg) => throw new Exception("Vessel SYNC messages are not handled on client-side!");
  }
}
