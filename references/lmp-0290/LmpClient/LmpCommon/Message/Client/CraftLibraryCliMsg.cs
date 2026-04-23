// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.CraftLibraryCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.CraftLibrary;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class CraftLibraryCliMsg : CliMsgBase<CraftLibraryBaseMsgData>
  {
    internal CraftLibraryCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (CraftLibraryCliMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (CraftLibraryFoldersRequestMsgData),
      [(ushort) 2] = typeof (CraftLibraryListRequestMsgData),
      [(ushort) 4] = typeof (CraftLibraryDownloadRequestMsgData),
      [(ushort) 5] = typeof (CraftLibraryDeleteRequestMsgData),
      [(ushort) 6] = typeof (CraftLibraryDataMsgData)
    };

    public override ClientMessageType MessageType => ClientMessageType.CraftLibrary;

    protected override int DefaultChannel => 9;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
