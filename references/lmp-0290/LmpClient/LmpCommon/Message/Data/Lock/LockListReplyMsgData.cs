// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Lock.LockListReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Locks;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Lock
{
  public class LockListReplyMsgData : LockBaseMsgData
  {
    public int LocksCount;
    public LockDefinition[] Locks = new LockDefinition[0];

    internal LockListReplyMsgData()
    {
    }

    public override LockMessageType LockMessageType => LockMessageType.ListReply;

    public override string ClassName { get; } = nameof (LockListReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.LocksCount);
      for (int index = 0; index < this.LocksCount; ++index)
        this.Locks[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.LocksCount = lidgrenMsg.ReadInt32();
      if (this.Locks.Length < this.LocksCount)
        this.Locks = new LockDefinition[this.LocksCount];
      for (int index = 0; index < this.LocksCount; ++index)
      {
        if (this.Locks[index] == (LockDefinition) null)
          this.Locks[index] = new LockDefinition();
        this.Locks[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.LocksCount; ++index)
        num += this.Locks[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
