// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareAchievements.ShareAchievementsMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;
using UnityEngine;

namespace LmpClient.Systems.ShareAchievements
{
  public class ShareAchievementsMessageSender : SubSystem<ShareAchievementsSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendAchievementsMessage(ProgressNode achievement)
    {
      ProgressNode node = ProgressTracking.Instance.FindNode(new string[1]
      {
        achievement.Id
      });
      if (node == null)
      {
        Traverse<CelestialBody> traverse = new Traverse((object) achievement).Field<CelestialBody>("body");
        string str = Object.op_Implicit((Object) traverse.Value) ? traverse.Value.name : (string) null;
        if (str != null)
          node = ProgressTracking.Instance.FindNode(new string[1]
          {
            str
          });
      }
      if (node == null)
        return;
      ConfigNode configNode = ShareAchievementsMessageSender.ConvertAchievementToConfigNode(node);
      if (configNode == null)
        return;
      ShareProgressAchievementsMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressAchievementsMsgData>();
      newMessageData.Id = node.Id;
      newMessageData.Data = configNode.Serialize();
      newMessageData.NumBytes = newMessageData.Data.Length;
      SubSystem<ShareAchievementsSystem>.System.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    private static ConfigNode ConvertAchievementToConfigNode(ProgressNode achievement)
    {
      ConfigNode configNode = new ConfigNode(achievement.Id);
      try
      {
        achievement.Save(configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving achievement: {0}", (object) ex));
        return (ConfigNode) null;
      }
      return configNode;
    }
  }
}
