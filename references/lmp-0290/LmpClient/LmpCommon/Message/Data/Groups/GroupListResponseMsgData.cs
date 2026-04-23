// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Groups.GroupListResponseMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Groups
{
  public class GroupListResponseMsgData : GroupBaseMsgData
  {
    public int GroupsCount;
    public Group[] Groups = new Group[0];

    internal GroupListResponseMsgData()
    {
    }

    public override GroupMessageType GroupMessageType => GroupMessageType.ListResponse;

    public override string ClassName { get; } = nameof (GroupListResponseMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.GroupsCount);
      for (int index = 0; index < this.GroupsCount; ++index)
        this.Groups[index].Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.GroupsCount = lidgrenMsg.ReadInt32();
      if (this.Groups.Length < this.GroupsCount)
        this.Groups = new Group[this.GroupsCount];
      for (int index = 0; index < this.GroupsCount; ++index)
      {
        if (this.Groups[index] == null)
          this.Groups[index] = new Group();
        this.Groups[index].Deserialize(lidgrenMsg);
      }
    }

    internal override int InternalGetMessageSize()
    {
      int num = 0;
      for (int index = 0; index < this.GroupsCount; ++index)
        num += this.Groups[index].GetByteCount();
      return base.InternalGetMessageSize() + 4 + num;
    }
  }
}
