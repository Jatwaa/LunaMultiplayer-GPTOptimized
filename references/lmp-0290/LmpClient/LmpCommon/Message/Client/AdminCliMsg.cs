// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.AdminCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.Admin;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class AdminCliMsg : CliMsgBase<AdminBaseMsgData>
  {
    internal AdminCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (AdminCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (AdminBanMsgData),
      [(ushort) 2] = typeof (AdminKickMsgData),
      [(ushort) 3] = typeof (AdminDekesslerMsgData),
      [(ushort) 4] = typeof (AdminNukeMsgData),
      [(ushort) 5] = typeof (AdminRestartServerMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.Admin;

    protected override int DefaultChannel => 16;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
