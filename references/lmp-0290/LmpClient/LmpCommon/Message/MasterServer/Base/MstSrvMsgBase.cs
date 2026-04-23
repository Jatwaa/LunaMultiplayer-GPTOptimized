// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.MasterServer.Base.MstSrvMsgBase`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.Interface;

namespace LmpCommon.Message.MasterServer.Base
{
  public abstract class MstSrvMsgBase<T> : MessageBase<T>, IMasterServerMessageBase, IMessageBase
    where T : class, IMessageData
  {
    protected override ushort MessageTypeId => (ushort) this.MessageType;

    public abstract MasterServerMessageType MessageType { get; }
  }
}
