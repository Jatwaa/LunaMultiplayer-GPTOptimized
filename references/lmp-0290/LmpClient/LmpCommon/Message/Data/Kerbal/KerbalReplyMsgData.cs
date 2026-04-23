// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Kerbal.KerbalReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Kerbal
{
  public class KerbalReplyMsgData : KerbalBaseMsgData
  {
    public int KerbalsCount;
    public KerbalInfo[] Kerbals = new KerbalInfo[0];

    internal KerbalReplyMsgData()
    {
    }

    public override KerbalMessageType KerbalMessageType => KerbalMessageType.Reply;

    public override string ClassName { get; } = nameof (KerbalReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.KerbalsCount);
      for (int index = 0; index < this.KerbalsCount; ++index)
        this.Kerbals[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.KerbalsCount = lidgrenMsg.ReadInt32();
      if (this.Kerbals.Length < this.KerbalsCount)
        this.Kerbals = new KerbalInfo[this.KerbalsCount];
      for (int index = 0; index < this.KerbalsCount; ++index)
      {
        if (this.Kerbals[index] == null)
          this.Kerbals[index] = new KerbalInfo();
        this.Kerbals[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.KerbalsCount; ++index)
        num += this.Kerbals[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
