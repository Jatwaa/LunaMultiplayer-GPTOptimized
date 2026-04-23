// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.WarpSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Warp;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class WarpSrvMsg : SrvMsgBase<WarpBaseMsgData>
  {
    internal WarpSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (WarpSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 2] = typeof (WarpSubspacesReplyMsgData),
      [(ushort) 3] = typeof (WarpNewSubspaceMsgData),
      [(ushort) 4] = typeof (WarpChangeSubspaceMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Warp;

    protected override int DefaultChannel => 13;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
