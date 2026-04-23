// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.ScreenshotCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.Screenshot;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class ScreenshotCliMsg : CliMsgBase<ScreenshotBaseMsgData>
  {
    internal ScreenshotCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (ScreenshotCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (ScreenshotFoldersRequestMsgData),
      [(ushort) 2] = typeof (ScreenshotListRequestMsgData),
      [(ushort) 4] = typeof (ScreenshotDownloadRequestMsgData),
      [(ushort) 5] = typeof (ScreenshotDataMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.Screenshot;

    protected override int DefaultChannel => 19;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
