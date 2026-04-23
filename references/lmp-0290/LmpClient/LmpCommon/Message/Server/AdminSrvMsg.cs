// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.AdminSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Admin;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class AdminSrvMsg : SrvMsgBase<AdminBaseMsgData>
  {
    internal AdminSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (AdminSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (AdminReplyMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Admin;

    protected override int DefaultChannel => 16;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
