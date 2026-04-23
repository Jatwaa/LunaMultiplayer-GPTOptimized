// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ShareProgressExperimentalPartMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ShareProgressExperimentalPartMsgData : ShareProgressBaseMsgData
  {
    public string PartName;
    public int Count;

    internal ShareProgressExperimentalPartMsgData()
    {
    }

    public override ShareProgressMessageType ShareProgressMessageType => ShareProgressMessageType.ExperimentalPart;

    public override string ClassName { get; } = nameof (ShareProgressExperimentalPartMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PartName);
      lidgrenMsg.Write(this.Count);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PartName = lidgrenMsg.ReadString();
      this.Count = lidgrenMsg.ReadInt32();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PartName.GetByteCount() + 4;
  }
}
