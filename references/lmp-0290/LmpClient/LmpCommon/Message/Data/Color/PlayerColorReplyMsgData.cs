// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Color.PlayerColorReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Color
{
  public class PlayerColorReplyMsgData : PlayerColorBaseMsgData
  {
    public int PlayerColorsCount;
    public PlayerColor[] PlayersColors = new PlayerColor[0];

    internal PlayerColorReplyMsgData()
    {
    }

    public override PlayerColorMessageType PlayerColorMessageType => PlayerColorMessageType.Reply;

    public override string ClassName { get; } = nameof (PlayerColorReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PlayerColorsCount);
      for (int index = 0; index < this.PlayerColorsCount; ++index)
        this.PlayersColors[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerColorsCount = lidgrenMsg.ReadInt32();
      if (this.PlayersColors.Length < this.PlayerColorsCount)
        this.PlayersColors = new PlayerColor[this.PlayerColorsCount];
      for (int index = 0; index < this.PlayerColorsCount; ++index)
      {
        if (this.PlayersColors[index] == null)
          this.PlayersColors[index] = new PlayerColor();
        this.PlayersColors[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.PlayerColorsCount; ++index)
        num += this.PlayersColors[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
