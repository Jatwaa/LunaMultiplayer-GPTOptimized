// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Warp.WarpNewSubspaceMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Warp
{
  public class WarpNewSubspaceMsgData : WarpBaseMsgData
  {
    public string PlayerCreator;
    public int SubspaceKey;
    public double ServerTimeDifference;

    internal WarpNewSubspaceMsgData()
    {
    }

    public override WarpMessageType WarpMessageType => WarpMessageType.NewSubspace;

    public override string ClassName { get; } = nameof (WarpNewSubspaceMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.PlayerCreator);
      lidgrenMsg.Write(this.SubspaceKey);
      lidgrenMsg.Write(this.ServerTimeDifference);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.PlayerCreator = lidgrenMsg.ReadString();
      this.SubspaceKey = lidgrenMsg.ReadInt32();
      this.ServerTimeDifference = lidgrenMsg.ReadDouble();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.PlayerCreator.GetByteCount() + 4 + 8;
  }
}
