// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.MessageData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Interface;

namespace LmpCommon.Message.Base
{
  public abstract class MessageData : IMessageData
  {
    internal MessageData()
    {
    }

    public ushort MajorVersion { get; set; } = LmpVersioning.MajorVersion;

    public ushort MinorVersion { get; set; } = LmpVersioning.MinorVersion;

    public ushort BuildVersion { get; set; } = LmpVersioning.BuildVersion;

    public long ReceiveTime { get; set; }

    public long SentTime { get; set; }

    public virtual ushort SubType => 0;

    public abstract string ClassName { get; }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.SentTime);
      lidgrenMsg.Write(this.MajorVersion);
      lidgrenMsg.Write(this.MinorVersion);
      lidgrenMsg.Write(this.BuildVersion);
      lidgrenMsg.WritePadBits();
      this.InternalSerialize(lidgrenMsg);
    }

    internal abstract void InternalSerialize(NetOutgoingMessage lidgrenMsg);

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.SentTime = lidgrenMsg.ReadInt64();
      this.MajorVersion = lidgrenMsg.ReadUInt16();
      this.MinorVersion = lidgrenMsg.ReadUInt16();
      this.BuildVersion = lidgrenMsg.ReadUInt16();
      lidgrenMsg.SkipPadBits();
      this.InternalDeserialize(lidgrenMsg);
    }

    internal abstract void InternalDeserialize(NetIncomingMessage lidgrenMsg);

    public int GetMessageSize() => 14 + this.InternalGetMessageSize();

    internal abstract int InternalGetMessageSize();
  }
}
