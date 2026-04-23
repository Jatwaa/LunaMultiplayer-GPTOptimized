// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Lock.LockMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Events;
using LmpCommon.Enums;
using LmpCommon.Locks;
using LmpCommon.Message.Data.Lock;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.Lock
{
  public class LockMessageHandler : SubSystem<LockSystem>, IMessageHandler
  {
    private static readonly List<LockDefinition> LocksToRemove = new List<LockDefinition>();

    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is LockBaseMsgData data))
        return;
      switch (data.LockMessageType)
      {
        case LockMessageType.ListReply:
          LockListReplyMsgData listReplyMsgData = (LockListReplyMsgData) data;
          for (int index = 0; index < listReplyMsgData.LocksCount; ++index)
            LockSystem.LockStore.AddOrUpdateLock(listReplyMsgData.Locks[index]);
          if (MainSystem.NetworkState >= ClientState.LocksSynced)
            break;
          MainSystem.NetworkState = ClientState.LocksSynced;
          break;
        case LockMessageType.Acquire:
          LockAcquireMsgData lockAcquireMsgData = (LockAcquireMsgData) data;
          LockSystem.LockStore.AddOrUpdateLock(lockAcquireMsgData.Lock);
          LockEvent.onLockAcquire.Fire(lockAcquireMsgData.Lock);
          break;
        case LockMessageType.Release:
          LockReleaseMsgData lockReleaseMsgData = (LockReleaseMsgData) data;
          LockSystem.LockStore.RemoveLock(lockReleaseMsgData.Lock);
          LockEvent.onLockRelease.Fire(lockReleaseMsgData.Lock);
          break;
      }
    }
  }
}
