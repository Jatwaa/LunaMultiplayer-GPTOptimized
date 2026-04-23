// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Groups.GroupMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.Groups;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Groups
{
  public class GroupMessageHandler : SubSystem<GroupSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is GroupBaseMsgData data1))
        return;
      switch (data1.GroupMessageType)
      {
        case GroupMessageType.ListResponse:
          GroupListResponseMsgData listResponseMsgData = (GroupListResponseMsgData) data1;
          for (int index = 0; index < listResponseMsgData.GroupsCount; ++index)
            SubSystem<GroupSystem>.System.Groups.TryAdd(listResponseMsgData.Groups[index].Name, listResponseMsgData.Groups[index]);
          break;
        case GroupMessageType.RemoveGroup:
          SubSystem<GroupSystem>.System.Groups.TryRemove(((GroupRemoveMsgData) data1).GroupName, out Group _);
          break;
        case GroupMessageType.GroupUpdate:
          GroupUpdateMsgData data = (GroupUpdateMsgData) data1;
          SubSystem<GroupSystem>.System.Groups.AddOrUpdate(data.Group.Name, data.Group, (Func<string, Group, Group>) ((key, existingVal) => data.Group));
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
