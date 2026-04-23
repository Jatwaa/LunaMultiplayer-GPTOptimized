// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Color.PlayerColorSetMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Color
{
  public class PlayerColorSetMsgData : PlayerColorBaseMsgData
  {
    public PlayerColor PlayerColor = new PlayerColor();

    internal PlayerColorSetMsgData()
    {
    }

    public override PlayerColorMessageType PlayerColorMessageType => PlayerColorMessageType.Set;

    public override string ClassName { get; } = nameof (PlayerColorSetMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.PlayerColor.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerColor.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PlayerColor.GetByteCount();
  }
}
