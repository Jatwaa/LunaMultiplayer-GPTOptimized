// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Admin.AdminBanKickMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Admin
{
  public abstract class AdminBanKickMsgData : AdminBaseMsgData
  {
    public string PlayerName;
    public string Reason;

    internal AdminBanKickMsgData()
    {
    }

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PlayerName);
      lidgrenMsg.Write(this.Reason);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerName = lidgrenMsg.ReadString();
      this.Reason = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PlayerName.GetByteCount() + this.Reason.GetByteCount();
  }
}
