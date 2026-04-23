// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Lock.LockAcquireMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Locks;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Lock
{
  public class LockAcquireMsgData : LockBaseMsgData
  {
    public LockDefinition Lock = new LockDefinition();
    public bool Force;

    internal LockAcquireMsgData()
    {
    }

    public override LockMessageType LockMessageType => LockMessageType.Acquire;

    public override string ClassName { get; } = nameof (LockAcquireMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.Force);
      this.Lock.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Force = lidgrenMsg.ReadBoolean();
      this.Lock.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Lock.GetByteCount() + 1;
  }
}
