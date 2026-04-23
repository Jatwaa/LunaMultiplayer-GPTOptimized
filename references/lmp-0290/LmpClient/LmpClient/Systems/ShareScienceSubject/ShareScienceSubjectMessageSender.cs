// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScienceSubject.ShareScienceSubjectMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.ShareScienceSubject
{
  public class ShareScienceSubjectMessageSender : 
    SubSystem<ShareScienceSubjectSystem>,
    IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendScienceSubjectMessage(ScienceSubject subject)
    {
      ShareProgressScienceSubjectMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressScienceSubjectMsgData>();
      newMessageData.ScienceSubject.Id = subject.id;
      ConfigNode configNode = ShareScienceSubjectMessageSender.ConvertScienceSubjectToConfigNode(subject);
      if (configNode == null)
        return;
      byte[] sourceArray = configNode.Serialize();
      int length = sourceArray.Length;
      newMessageData.ScienceSubject.NumBytes = length;
      if (newMessageData.ScienceSubject.Data.Length < length)
        newMessageData.ScienceSubject.Data = new byte[length];
      Array.Copy((Array) sourceArray, (Array) newMessageData.ScienceSubject.Data, length);
      this.SendMessage((IMessageData) newMessageData);
      LunaLog.Log("Science experiment \"" + subject.id + "\" sent");
    }

    private static ConfigNode ConvertScienceSubjectToConfigNode(ScienceSubject subject)
    {
      ConfigNode configNode = new ConfigNode();
      try
      {
        subject.Save(configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving science subject: {0}", (object) ex));
        return (ConfigNode) null;
      }
      return configNode;
    }
  }
}
