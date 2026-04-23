// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.PlayerStatus.PlayerStatusReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.PlayerStatus
{
  public class PlayerStatusReplyMsgData : PlayerStatusBaseMsgData
  {
    public int PlayerStatusCount;
    public PlayerStatusInfo[] PlayerStatus = new PlayerStatusInfo[0];

    internal PlayerStatusReplyMsgData()
    {
    }

    public override PlayerStatusMessageType PlayerStatusMessageType => PlayerStatusMessageType.Reply;

    public override string ClassName { get; } = nameof (PlayerStatusReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PlayerStatusCount);
      for (int index = 0; index < this.PlayerStatusCount; ++index)
        this.PlayerStatus[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerStatusCount = lidgrenMsg.ReadInt32();
      if (this.PlayerStatus.Length < this.PlayerStatusCount)
        this.PlayerStatus = new PlayerStatusInfo[this.PlayerStatusCount];
      for (int index = 0; index < this.PlayerStatusCount; ++index)
      {
        if (this.PlayerStatus[index] == null)
          this.PlayerStatus[index] = new PlayerStatusInfo();
        this.PlayerStatus[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.PlayerStatusCount; ++index)
        num += this.PlayerStatus[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
