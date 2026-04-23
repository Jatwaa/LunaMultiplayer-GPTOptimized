// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Kerbal.KerbalProtoMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Kerbal
{
  public class KerbalProtoMsgData : KerbalBaseMsgData
  {
    public KerbalInfo Kerbal = new KerbalInfo();

    internal KerbalProtoMsgData()
    {
    }

    public override KerbalMessageType KerbalMessageType => KerbalMessageType.Proto;

    public override string ClassName { get; } = nameof (KerbalProtoMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.Kerbal.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Kerbal.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Kerbal.GetByteCount();
  }
}
