// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ModMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data
{
  public class ModMsgData : MessageData
  {
    public string ModName;
    public bool Relay;
    public bool Reliable;
    public int NumBytes;
    public byte[] Data = new byte[0];

    internal ModMsgData()
    {
    }

    public override string ClassName { get; } = nameof (ModMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.ModName);
      lidgrenMsg.Write(this.Relay);
      lidgrenMsg.Write(this.Reliable);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.Data, 0, this.NumBytes);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      this.ModName = lidgrenMsg.ReadString();
      this.Relay = lidgrenMsg.ReadBoolean();
      this.Reliable = lidgrenMsg.ReadBoolean();
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.Data, 0, this.NumBytes);
    }

    internal override int InternalGetMessageSize() => this.ModName.GetByteCount() + 1 + 1 + 4 + this.NumBytes;
  }
}
