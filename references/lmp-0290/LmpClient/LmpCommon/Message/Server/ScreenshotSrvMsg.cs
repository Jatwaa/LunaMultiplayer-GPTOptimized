// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.ScreenshotSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Screenshot;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class ScreenshotSrvMsg : SrvMsgBase<ScreenshotBaseMsgData>
  {
    internal ScreenshotSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (ScreenshotSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (ScreenshotFoldersReplyMsgData),
      [(ushort) 3] = typeof (ScreenshotListReplyMsgData),
      [(ushort) 5] = typeof (ScreenshotDataMsgData),
      [(ushort) 6] = typeof (ScreenshotNotificationMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Screenshot;

    protected override int DefaultChannel => 20;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
