// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.ClientMessageFactory
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.Client.Base;
using System;

namespace LmpCommon.Message
{
  public class ClientMessageFactory : FactoryBase
  {
    protected internal override Type BaseMsgType => typeof (CliMsgBase<>);

    protected internal override Type HandledMessageTypes { get; } = typeof (ClientMessageType);
  }
}
