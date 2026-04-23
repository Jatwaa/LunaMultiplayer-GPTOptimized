// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.GroupCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.Groups;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class GroupCliMsg : CliMsgBase<GroupBaseMsgData>
  {
    internal GroupCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (GroupCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (GroupListRequestMsgData),
      [(ushort) 1] = typeof (GroupListResponseMsgData),
      [(ushort) 2] = typeof (GroupCreateMsgData),
      [(ushort) 3] = typeof (GroupRemoveMsgData),
      [(ushort) 4] = typeof (GroupUpdateMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.Groups;

    protected override int DefaultChannel => 17;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
