// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.MasterServerMessageFactory
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.MasterServer.Base;
using System;

namespace LmpCommon.Message
{
  public class MasterServerMessageFactory : FactoryBase
  {
    protected internal override Type HandledMessageTypes { get; } = typeof (ServerMessageType);

    protected internal override Type BaseMsgType => typeof (MstSrvMsgBase<>);
  }
}
