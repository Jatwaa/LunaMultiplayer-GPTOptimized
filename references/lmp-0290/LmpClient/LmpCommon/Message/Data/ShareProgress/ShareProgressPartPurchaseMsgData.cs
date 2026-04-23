// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ShareProgressPartPurchaseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ShareProgressPartPurchaseMsgData : ShareProgressBaseMsgData
  {
    public string TechId;
    public string PartName;

    internal ShareProgressPartPurchaseMsgData()
    {
    }

    public override ShareProgressMessageType ShareProgressMessageType => ShareProgressMessageType.PartPurchase;

    public override string ClassName { get; } = nameof (ShareProgressPartPurchaseMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.TechId);
      lidgrenMsg.Write(this.PartName);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.TechId = lidgrenMsg.ReadString();
      this.PartName = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.TechId.GetByteCount() + this.PartName.GetByteCount();
  }
}
