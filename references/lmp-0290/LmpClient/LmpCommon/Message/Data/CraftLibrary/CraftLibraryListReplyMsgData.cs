// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.CraftLibrary.CraftLibraryListReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.CraftLibrary
{
  public class CraftLibraryListReplyMsgData : CraftLibraryBaseMsgData
  {
    public string FolderName;
    public int PlayerCraftsCount;
    public CraftBasicInfo[] PlayerCrafts = new CraftBasicInfo[0];

    internal CraftLibraryListReplyMsgData()
    {
    }

    public override CraftMessageType CraftMessageType => CraftMessageType.ListReply;

    public override string ClassName { get; } = nameof (CraftLibraryListReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FolderName);
      lidgrenMsg.Write(this.PlayerCraftsCount);
      for (int index = 0; index < this.PlayerCraftsCount; ++index)
        this.PlayerCrafts[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FolderName = lidgrenMsg.ReadString();
      this.PlayerCraftsCount = lidgrenMsg.ReadInt32();
      if (this.PlayerCrafts.Length < this.PlayerCraftsCount)
        this.PlayerCrafts = new CraftBasicInfo[this.PlayerCraftsCount];
      for (int index = 0; index < this.PlayerCraftsCount; ++index)
      {
        if (this.PlayerCrafts[index] == null)
          this.PlayerCrafts[index] = new CraftBasicInfo();
        this.PlayerCrafts[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.PlayerCraftsCount; ++index)
        num += this.PlayerCrafts[index].GetByteCount();
      return base.InternalGetMessageSize() + this.FolderName.GetByteCount() + 4 + num;
    }
  }
}
