// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.ShareProgressCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data.ShareProgress;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Client
{
  public class ShareProgressCliMsg : CliMsgBase<ShareProgressBaseMsgData>
  {
    internal ShareProgressCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (ShareProgressCliMsg);

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

    public override ClientMessageType MessageType => ClientMessageType.ShareProgress;

    protected override int DefaultChannel => 20;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
