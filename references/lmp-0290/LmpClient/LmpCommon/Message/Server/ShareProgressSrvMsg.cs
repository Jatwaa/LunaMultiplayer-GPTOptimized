// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.ShareProgressSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class ShareProgressSrvMsg : SrvMsgBase<ShareProgressBaseMsgData>
  {
    internal ShareProgressSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (ShareProgressSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (ShareProgressFundsMsgData),
      [(ushort) 1] = typeof (ShareProgressScienceMsgData),
      [(ushort) 2] = typeof (ShareProgressScienceSubjectMsgData),
      [(ushort) 3] = typeof (ShareProgressReputationMsgData),
      [(ushort) 4] = typeof (ShareProgressTechnologyMsgData),
      [(ushort) 5] = typeof (ShareProgressContractsMsgData),
      [(ushort) 6] = typeof (ShareProgressAchievementsMsgData),
      [(ushort) 7] = typeof (ShareProgressStrategyMsgData),
      [(ushort) 8] = typeof (ShareProgressFacilityUpgradeMsgData),
      [(ushort) 9] = typeof (ShareProgressPartPurchaseMsgData),
      [(ushort) 10] = typeof (ShareProgressExperimentalPartMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.ShareProgress;

    protected override int DefaultChannel => 21;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
