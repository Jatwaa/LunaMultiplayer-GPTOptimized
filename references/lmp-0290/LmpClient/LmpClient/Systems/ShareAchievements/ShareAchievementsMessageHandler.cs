// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareAchievements.ShareAchievementsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Systems.ShareFunds;
using LmpClient.Systems.ShareReputation;
using LmpClient.Systems.ShareScience;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ShareAchievements
{
  public class ShareAchievementsMessageHandler : SubSystem<ShareAchievementsSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.AchievementsUpdate || !(data is ShareProgressAchievementsMsgData achievementsMsgData))
        return;
      ProgressNode incomingAchievement = ShareAchievementsMessageHandler.ConvertByteArrayToAchievement(achievementsMsgData.Data, achievementsMsgData.NumBytes, achievementsMsgData.Id);
      LunaLog.Log("Queue AchievementsUpdate");
      SubSystem<ShareAchievementsSystem>.System.QueueAction((Action) (() => ShareAchievementsMessageHandler.AchievementUpdate(incomingAchievement)));
    }

    private static void AchievementUpdate(ProgressNode incomingAchievement)
    {
      if (incomingAchievement == null)
        return;
      SubSystem<ShareAchievementsSystem>.System.StartIgnoringEvents();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StartIgnoringEvents();
      int num = -1;
      for (int index = 0; index < ProgressTracking.Instance.achievementTree.Count; ++index)
      {
        if (!(ProgressTracking.Instance.achievementTree[index].Id != incomingAchievement.Id))
        {
          num = index;
          break;
        }
      }
      if (num != -1)
      {
        if (!ProgressTracking.Instance.achievementTree[num].IsReached && incomingAchievement.IsReached)
          ProgressTracking.Instance.achievementTree[num].Reach();
        if (!ProgressTracking.Instance.achievementTree[num].IsComplete && incomingAchievement.IsComplete)
          ProgressTracking.Instance.achievementTree[num].Complete();
        LunaLog.Log("Achievement was updated: " + incomingAchievement.Id);
      }
      else
      {
        ProgressTracking.Instance.achievementTree.AddNode(incomingAchievement);
        LunaLog.Log("Achievement was added: " + incomingAchievement.Id);
      }
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StopIgnoringEvents(true);
      SubSystem<ShareAchievementsSystem>.System.StopIgnoringEvents();
    }

    private static ProgressNode ConvertByteArrayToAchievement(
      byte[] data,
      int numBytes,
      string progressNodeId)
    {
      ConfigNode configNode;
      try
      {
        configNode = data.DeserializeToConfigNode(numBytes);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing achievement configNode: {0}", (object) ex));
        return (ProgressNode) null;
      }
      if (configNode == null)
      {
        LunaLog.LogError("[LMP]: Error, the achievement configNode was null.");
        return (ProgressNode) null;
      }
      ProgressNode achievement;
      try
      {
        achievement = new ProgressNode(progressNodeId, false);
        achievement.Load(configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing achievement: {0}", (object) ex));
        return (ProgressNode) null;
      }
      return achievement;
    }
  }
}
