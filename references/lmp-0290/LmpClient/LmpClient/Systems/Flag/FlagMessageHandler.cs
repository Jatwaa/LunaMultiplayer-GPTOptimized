// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Flag.FlagMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Flag;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Flag
{
  public class FlagMessageHandler : SubSystem<FlagSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is FlagBaseMsgData data))
        return;
      switch (data.FlagMessageType)
      {
        case FlagMessageType.ListResponse:
          FlagListResponseMsgData listResponseMsgData = (FlagListResponseMsgData) data;
          for (int index = 0; index < listResponseMsgData.FlagCount; ++index)
          {
            ExtendedFlagInfo extendedFlagInfo1 = new ExtendedFlagInfo(listResponseMsgData.FlagFiles[index]);
            SubSystem<FlagSystem>.System.ServerFlags.TryAdd(extendedFlagInfo1.FlagName, extendedFlagInfo1);
          }
          MainSystem.NetworkState = ClientState.FlagsSynced;
          break;
        case FlagMessageType.FlagData:
          ExtendedFlagInfo extendedFlagInfo = new ExtendedFlagInfo(((FlagDataMsgData) data).Flag);
          SubSystem<FlagSystem>.System.ServerFlags.AddOrUpdate(extendedFlagInfo.FlagName, extendedFlagInfo, (Func<string, ExtendedFlagInfo, ExtendedFlagInfo>) ((key, existingVal) => extendedFlagInfo));
          break;
      }
    }
  }
}
