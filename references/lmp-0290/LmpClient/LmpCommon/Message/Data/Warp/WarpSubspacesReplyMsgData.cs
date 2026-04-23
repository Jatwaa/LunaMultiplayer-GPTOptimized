// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Warp.WarpSubspacesReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Warp
{
  public class WarpSubspacesReplyMsgData : WarpBaseMsgData
  {
    public int SubspaceCount;
    public SubspaceInfo[] Subspaces = new SubspaceInfo[0];

    internal WarpSubspacesReplyMsgData()
    {
    }

    public override WarpMessageType WarpMessageType => WarpMessageType.SubspacesReply;

    public override string ClassName { get; } = nameof (WarpSubspacesReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.SubspaceCount);
      for (int index = 0; index < this.SubspaceCount; ++index)
        this.Subspaces[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.SubspaceCount = lidgrenMsg.ReadInt32();
      if (this.Subspaces.Length < this.SubspaceCount)
        this.Subspaces = new SubspaceInfo[this.SubspaceCount];
      for (int index = 0; index < this.SubspaceCount; ++index)
      {
        if (this.Subspaces[index] == null)
          this.Subspaces[index] = new SubspaceInfo();
        this.Subspaces[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.SubspaceCount; ++index)
        num += this.Subspaces[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
