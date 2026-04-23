// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ShareProgressScienceSubjectMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ShareProgressScienceSubjectMsgData : ShareProgressBaseMsgData
  {
    public ScienceSubjectInfo ScienceSubject = new ScienceSubjectInfo();

    internal ShareProgressScienceSubjectMsgData()
    {
    }

    public override ShareProgressMessageType ShareProgressMessageType => ShareProgressMessageType.ScienceSubjectUpdate;

    public override string ClassName { get; } = nameof (ShareProgressScienceSubjectMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.ScienceSubject.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ScienceSubject.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.ScienceSubject.GetByteCount();
  }
}
