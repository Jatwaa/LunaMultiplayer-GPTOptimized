// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Lock.LockReleaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Locks;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Lock
{
  public class LockReleaseMsgData : LockBaseMsgData
  {
    public LockDefinition Lock = new LockDefinition();
    public bool LockResult;

    internal LockReleaseMsgData()
    {
    }

    public override LockMessageType LockMessageType => LockMessageType.Release;

    public override string ClassName { get; } = nameof (LockReleaseMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.LockResult);
      lidgrenMsg.WritePadBits();
      this.Lock.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.LockResult = lidgrenMsg.ReadBoolean();
      lidgrenMsg.SkipPadBits();
      this.Lock.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Lock.GetByteCount() + 1;
  }
}
