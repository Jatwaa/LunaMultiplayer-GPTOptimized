// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Flag.FlagDataMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Flag
{
  public class FlagDataMsgData : FlagBaseMsgData
  {
    public FlagInfo Flag = new FlagInfo();

    internal FlagDataMsgData()
    {
    }

    public override FlagMessageType FlagMessageType => FlagMessageType.FlagData;

    public override string ClassName { get; } = nameof (FlagDataMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.Flag.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Flag.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Flag.GetByteCount();
  }
}
