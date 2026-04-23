// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.SetingsSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Settings;
using LmpCommon.Message.Server.Base;
using System;
using System.Collections.Generic;

namespace LmpCommon.Message.Server
{
  public class SetingsSrvMsg : SrvMsgBase<SettingsBaseMsgData>
  {
    internal SetingsSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (SetingsSrvMsg);

    protected override Dictionary<ushort, Type> SubTypeDictionary { get; } = new Dictionary<ushort, Type>()
    {
      [(ushort) 1] = typeof (SettingsReplyMsgData)
    };

    public override ServerMessageType MessageType => ServerMessageType.Settings;

    protected override int DefaultChannel => 2;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
