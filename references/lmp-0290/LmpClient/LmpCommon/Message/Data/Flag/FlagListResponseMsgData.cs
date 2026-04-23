// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Flag.FlagListResponseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Flag
{
  public class FlagListResponseMsgData : FlagBaseMsgData
  {
    public int FlagCount;
    public FlagInfo[] FlagFiles = new FlagInfo[0];

    internal FlagListResponseMsgData()
    {
    }

    public override FlagMessageType FlagMessageType => FlagMessageType.ListResponse;

    public override string ClassName { get; } = nameof (FlagListResponseMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FlagCount);
      for (int index = 0; index < this.FlagCount; ++index)
        this.FlagFiles[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FlagCount = lidgrenMsg.ReadInt32();
      if (this.FlagFiles.Length < this.FlagCount)
        this.FlagFiles = new FlagInfo[this.FlagCount];
      for (int index = 0; index < this.FlagCount; ++index)
      {
        if (this.FlagFiles[index] == null)
          this.FlagFiles[index] = new FlagInfo();
        this.FlagFiles[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.FlagCount; ++index)
        num += this.FlagFiles[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
