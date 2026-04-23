// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.CraftLibrarySrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.CraftLibrary;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class CraftLibrarySrvMsg : SrvMsgBase<CraftLibraryBaseMsgData>
  {
    internal CraftLibrarySrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (CraftLibrarySrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (CraftLibraryFoldersReplyMsgData),
      [(ushort) 3] = typeof (CraftLibraryListReplyMsgData),
      [(ushort) 5] = typeof (CraftLibraryDeleteRequestMsgData),
      [(ushort) 6] = typeof (CraftLibraryDataMsgData),
      [(ushort) 7] = typeof (CraftLibraryNotificationMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.CraftLibrary;

    protected override int DefaultChannel => 9;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
