// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.MotdCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.Motd;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class MotdCliMsg : CliMsgBase<MotdBaseMsgData>
  {
    internal MotdCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (MotdCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (MotdRequestMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.Motd;

    protected override int DefaultChannel => 12;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
