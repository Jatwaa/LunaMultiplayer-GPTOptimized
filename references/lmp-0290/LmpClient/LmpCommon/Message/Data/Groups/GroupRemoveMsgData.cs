// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Groups.GroupRemoveMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Groups
{
  public class GroupRemoveMsgData : GroupBaseMsgData
  {
    public string GroupName;

    internal GroupRemoveMsgData()
    {
    }

    public override GroupMessageType GroupMessageType => GroupMessageType.RemoveGroup;

    public override string ClassName { get; } = nameof (GroupRemoveMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.GroupName);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.GroupName = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.GroupName.GetByteCount();
  }
}
