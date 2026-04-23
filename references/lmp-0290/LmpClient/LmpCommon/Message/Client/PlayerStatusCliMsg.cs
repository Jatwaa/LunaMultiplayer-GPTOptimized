// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.PlayerStatusCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.PlayerStatus;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class PlayerStatusCliMsg : CliMsgBase<PlayerStatusBaseMsgData>
  {
    internal PlayerStatusCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (PlayerStatusCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (PlayerStatusRequestMsgData),
      [(ushort) 2] = typeof (PlayerStatusSetMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.PlayerStatus;

    protected override int DefaultChannel => 4;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
