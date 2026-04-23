// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.KerbalSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Kerbal;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class KerbalSrvMsg : SrvMsgBase<KerbalBaseMsgData>
  {
    internal KerbalSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (KerbalSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (KerbalReplyMsgData),
      [(ushort) 2] = typeof (KerbalProtoMsgData),
      [(ushort) 3] = typeof (KerbalRemoveMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Kerbal;

    protected override int DefaultChannel => 7;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
