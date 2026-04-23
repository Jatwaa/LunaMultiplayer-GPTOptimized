// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Admin.AdminReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Admin
{
  public class AdminReplyMsgData : AdminBaseMsgData
  {
    public AdminResponse Response;

    internal AdminReplyMsgData()
    {
    }

    public override AdminMessageType AdminMessageType => AdminMessageType.Reply;

    public override string ClassName { get; } = nameof (AdminReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write((int) this.Response);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Response = (AdminResponse) lidgrenMsg.ReadInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4;
  }
}
