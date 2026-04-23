// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.MasterServer.MainMstSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.MasterServer;
using LmpCommon.Message.MasterServer.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.MasterServer
{
  public class MainMstSrvMsg : MstSrvMsgBase<MsBaseMsgData>
  {
    internal MainMstSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (MainMstSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (MsRegisterServerMsgData),
      [(ushort) 1] = typeof (MsRequestServersMsgData),
      [(ushort) 2] = typeof (MsReplyServersMsgData),
      [(ushort) 3] = typeof (MsIntroductionMsgData)
    };

    public override MasterServerMessageType MessageType => MasterServerMessageType.Main;

    protected override int DefaultChannel => 1;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
