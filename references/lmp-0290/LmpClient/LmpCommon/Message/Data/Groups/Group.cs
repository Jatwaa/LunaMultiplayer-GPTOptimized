// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Groups.Group
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpCommon.Message.Data.Groups
{
  [Serializable]
  public class Group
  {
    public string Name;
    public string Owner;
    public int MembersCount;
    public string[] Members = new string[0];
    public int InvitedCount;
    public string[] Invited = new string[0];

    public Group Clone()
    {
      if (!(this.MemberwiseClone() is Group group))
        return (Group) null;
      group.Name = this.Name.Clone() as string;
      group.Owner = this.Owner.Clone() as string;
      group.Members = ((IEnumerable<string>) this.Members).Select<string, string>((Func<string, string>) (m => m.Clone() as string)).ToArray<string>();
      group.Invited = ((IEnumerable<string>) this.Invited).Select<string, string>((Func<string, string>) (m => m.Clone() as string)).ToArray<string>();
      return group;
    }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.Name);
      lidgrenMsg.Write(this.Owner);
      lidgrenMsg.Write(this.MembersCount);
      for (int index = 0; index < this.MembersCount; ++index)
        lidgrenMsg.Write(this.Members[index]);
      lidgrenMsg.Write(this.InvitedCount);
      for (int index = 0; index < this.InvitedCount; ++index)
        lidgrenMsg.Write(this.Invited[index]);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.Name = lidgrenMsg.ReadString();
      this.Owner = lidgrenMsg.ReadString();
      this.MembersCount = lidgrenMsg.ReadInt32();
      if (this.Members.Length < this.MembersCount)
        this.Members = new string[this.MembersCount];
      for (int index = 0; index < this.MembersCount; ++index)
        this.Members[index] = lidgrenMsg.ReadString();
      this.InvitedCount = lidgrenMsg.ReadInt32();
      if (this.Invited.Length < this.InvitedCount)
        this.Invited = new string[this.InvitedCount];
      for (int index = 0; index < this.InvitedCount; ++index)
        this.Invited[index] = lidgrenMsg.ReadString();
    }

    public int GetByteCount() => this.Name.GetByteCount() + this.Owner.GetByteCount() + 4 + this.Members.GetByteCount(this.MembersCount) + 4 + this.Invited.GetByteCount(this.InvitedCount);
  }
}
