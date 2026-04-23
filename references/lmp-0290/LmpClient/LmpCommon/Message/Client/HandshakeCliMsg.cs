// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.HandshakeCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.Handshake;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class HandshakeCliMsg : CliMsgBase<HandshakeBaseMsgData>
  {
    internal HandshakeCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (HandshakeCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (HandshakeRequestMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.Handshake;

    protected override int DefaultChannel => 1;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
