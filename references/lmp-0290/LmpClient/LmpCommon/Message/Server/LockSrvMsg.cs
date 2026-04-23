// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.LockSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Lock;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class LockSrvMsg : SrvMsgBase<LockBaseMsgData>
  {
    internal LockSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (LockSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (LockListReplyMsgData),
      [(ushort) 2] = typeof (LockAcquireMsgData),
      [(ushort) 3] = typeof (LockReleaseMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Lock;

    protected override int DefaultChannel => 14;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
