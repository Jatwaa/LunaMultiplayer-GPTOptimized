// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.VesselSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class VesselSrvMsg : SrvMsgBase<VesselBaseMsgData>
  {
    internal VesselSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (VesselSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 0] = typeof (VesselProtoMsgData),
      [(ushort) 1] = typeof (VesselRemoveMsgData),
      [(ushort) 2] = typeof (VesselPositionMsgData),
      [(ushort) 3] = typeof (VesselFlightStateMsgData),
      [(ushort) 4] = typeof (VesselUpdateMsgData),
      [(ushort) 5] = typeof (VesselResourceMsgData),
      [(ushort) 7] = typeof (VesselPartSyncFieldMsgData),
      [(ushort) 8] = typeof (VesselPartSyncUiFieldMsgData),
      [(ushort) 9] = typeof (VesselPartSyncCallMsgData),
      [(ushort) 10] = typeof (VesselActionGroupMsgData),
      [(ushort) 11] = typeof (VesselFairingMsgData),
      [(ushort) 12] = typeof (VesselDecoupleMsgData),
      [(ushort) 13] = typeof (VesselCoupleMsgData),
      [(ushort) 14] = typeof (VesselUndockMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Vessel;

    protected override int DefaultChannel => !this.IsUnreliableMessage() ? 8 : 0;

    public override NetDeliveryMethod NetDeliveryMethod => !this.IsUnreliableMessage() ? NetDeliveryMethod.ReliableOrdered : NetDeliveryMethod.UnreliableSequenced;

    private bool IsUnreliableMessage() => this.Data.SubType == (ushort) 2 || this.Data.SubType == (ushort) 3 || this.Data.SubType == (ushort) 4 || this.Data.SubType == (ushort) 5;
  }
}
