// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.PlayerColorSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Color;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class PlayerColorSrvMsg : SrvMsgBase<PlayerColorBaseMsgData>
  {
    internal PlayerColorSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (PlayerColorSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (PlayerColorReplyMsgData),
      [(ushort) 2] = typeof (PlayerColorSetMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.PlayerColor;

    protected override int DefaultChannel => 5;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
