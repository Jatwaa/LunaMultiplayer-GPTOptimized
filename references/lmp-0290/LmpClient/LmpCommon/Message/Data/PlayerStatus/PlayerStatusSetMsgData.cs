// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.PlayerStatus.PlayerStatusSetMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.PlayerStatus
{
  public class PlayerStatusSetMsgData : PlayerStatusBaseMsgData
  {
    public PlayerStatusInfo PlayerStatus = new PlayerStatusInfo();

    internal PlayerStatusSetMsgData()
    {
    }

    public override PlayerStatusMessageType PlayerStatusMessageType => PlayerStatusMessageType.Set;

    public override string ClassName { get; } = nameof (PlayerStatusSetMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.PlayerStatus.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerStatus.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PlayerStatus.GetByteCount();
  }
}
