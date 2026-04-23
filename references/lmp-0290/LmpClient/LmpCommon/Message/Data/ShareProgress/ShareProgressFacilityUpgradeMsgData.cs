// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ShareProgressFacilityUpgradeMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ShareProgressFacilityUpgradeMsgData : ShareProgressBaseMsgData
  {
    public string FacilityId;
    public int Level;
    public float NormLevel;

    internal ShareProgressFacilityUpgradeMsgData()
    {
    }

    public override ShareProgressMessageType ShareProgressMessageType => ShareProgressMessageType.FacilityUpgrade;

    public override string ClassName { get; } = nameof (ShareProgressFacilityUpgradeMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FacilityId);
      lidgrenMsg.Write(this.Level);
      lidgrenMsg.Write(this.NormLevel);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FacilityId = lidgrenMsg.ReadString();
      this.Level = lidgrenMsg.ReadInt32();
      this.NormLevel = lidgrenMsg.ReadFloat();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.FacilityId.GetByteCount() + 4 + 4;
  }
}
