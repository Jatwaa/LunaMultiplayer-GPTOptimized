// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Groups.GroupSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Message.Data.Groups;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Systems.Groups
{
  public class GroupSystem : MessageSystem<GroupSystem, GroupMessageSender, GroupMessageHandler>
  {
    public ConcurrentDictionary<string, Group> Groups { get; } = new ConcurrentDictionary<string, Group>();

    public override string SystemName { get; } = nameof (GroupSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.Groups.Clear();
    }

    public void JoinGroup(string groupName)
    {
      Group group1;
      if (!this.Groups.TryGetValue(groupName, out group1) || !((IEnumerable<string>) group1.Members).All<string>((Func<string, bool>) (m => m != SettingsSystem.CurrentSettings.PlayerName)) || !((IEnumerable<string>) group1.Invited).All<string>((Func<string, bool>) (m => m != SettingsSystem.CurrentSettings.PlayerName)))
        return;
      Group group2 = group1.Clone();
      List<string> stringList = new List<string>((IEnumerable<string>) group2.Invited)
      {
        SettingsSystem.CurrentSettings.PlayerName
      };
      group2.Invited = stringList.ToArray();
      GroupUpdateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<GroupUpdateMsgData>();
      newMessageData.Group = group2;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void CreateGroup(string groupName)
    {
      if (this.Groups.ContainsKey(groupName))
        return;
      GroupCreateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<GroupCreateMsgData>();
      newMessageData.GroupName = groupName;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void RemoveGroup(string groupName)
    {
      Group group;
      if (!this.Groups.TryGetValue(groupName, out group) || !(group.Owner == SettingsSystem.CurrentSettings.PlayerName))
        return;
      GroupRemoveMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<GroupRemoveMsgData>();
      newMessageData.GroupName = groupName;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void AddMember(string groupName, string username)
    {
      Group group1;
      if (!this.Groups.TryGetValue(groupName, out group1) || !(group1.Owner == SettingsSystem.CurrentSettings.PlayerName))
        return;
      Group group2 = group1.Clone();
      List<string> stringList1 = new List<string>((IEnumerable<string>) group2.Members)
      {
        username
      };
      group2.Members = stringList1.ToArray();
      List<string> stringList2 = new List<string>(((IEnumerable<string>) group2.Invited).Except<string>((IEnumerable<string>) new string[1]
      {
        username
      }));
      group2.Invited = stringList2.ToArray();
      GroupUpdateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<GroupUpdateMsgData>();
      newMessageData.Group = group2;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void RemoveMember(string groupName, string username)
    {
      Group group1;
      if (!this.Groups.TryGetValue(groupName, out group1) || !(group1.Owner == SettingsSystem.CurrentSettings.PlayerName))
        return;
      Group group2 = group1.Clone();
      List<string> stringList1 = new List<string>(((IEnumerable<string>) group2.Members).Except<string>((IEnumerable<string>) new string[1]
      {
        username
      }))
      {
        username
      };
      group2.Members = stringList1.ToArray();
      List<string> stringList2 = new List<string>(((IEnumerable<string>) group2.Invited).Except<string>((IEnumerable<string>) new string[1]
      {
        username
      }));
      group2.Invited = stringList2.ToArray();
      GroupUpdateMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<GroupUpdateMsgData>();
      newMessageData.Group = group2;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }
  }
}
