// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.PlayerConnectionSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.PlayerConnection;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class PlayerConnectionSrvMsg : SrvMsgBase<PlayerConnectionBaseMsgData>
  {
    internal PlayerConnectionSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (PlayerConnectionSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (PlayerConnectionJoinMsgData),
      [(ushort) 1] = typeof (PlayerConnectionLeaveMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.PlayerConnection;

    protected override int DefaultChannel => 17;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
