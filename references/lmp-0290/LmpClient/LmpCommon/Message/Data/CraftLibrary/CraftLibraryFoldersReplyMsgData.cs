// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.CraftLibrary.CraftLibraryFoldersReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.CraftLibrary
{
  public class CraftLibraryFoldersReplyMsgData : CraftLibraryBaseMsgData
  {
    public int NumFolders;
    public string[] Folders = new string[0];

    internal CraftLibraryFoldersReplyMsgData()
    {
    }

    public override CraftMessageType CraftMessageType => CraftMessageType.FoldersReply;

    public override string ClassName { get; } = nameof (CraftLibraryFoldersReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.NumFolders);
      for (int index = 0; index < this.NumFolders; ++index)
        lidgrenMsg.Write(this.Folders[index]);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.NumFolders = lidgrenMsg.ReadInt32();
      if (this.Folders.Length < this.NumFolders)
        this.Folders = new string[this.NumFolders];
      for (int index = 0; index < this.NumFolders; ++index)
        this.Folders[index] = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.NumFolders; ++index)
        num += this.Folders[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
