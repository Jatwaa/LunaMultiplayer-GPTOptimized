// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareScienceSubject.ShareScienceSubjectMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.ShareScienceSubject
{
  public class ShareScienceSubjectMessageHandler : 
    SubSystem<ShareScienceSubjectSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.ScienceSubjectUpdate || !(data is ShareProgressScienceSubjectMsgData scienceSubjectMsgData))
        return;
      ScienceSubjectInfo subject = new ScienceSubjectInfo(scienceSubjectMsgData.ScienceSubject);
      LunaLog.Log("Queue Science subject: " + subject.Id);
      SubSystem<ShareScienceSubjectSystem>.System.QueueAction((Action) (() => ShareScienceSubjectMessageHandler.NewScienceSubject(subject)));
    }

    private static void NewScienceSubject(ScienceSubjectInfo subject)
    {
      SubSystem<ShareScienceSubjectSystem>.System.StartIgnoringEvents();
      Dictionary<string, ScienceSubject> scienceSubjects = SubSystem<ShareScienceSubjectSystem>.System.ScienceSubjects;
      ScienceSubject scienceSubject1 = ShareScienceSubjectMessageHandler.ConvertByteArrayToScienceSubject(subject.Data, subject.NumBytes);
      ScienceSubject scienceSubject2;
      if (!scienceSubjects.TryGetValue(subject.Id, out scienceSubject2))
      {
        scienceSubjects.Add(scienceSubject1.id, scienceSubject1);
      }
      else
      {
        scienceSubject2.dataScale = scienceSubject1.dataScale;
        scienceSubject2.scientificValue = scienceSubject1.scientificValue;
        scienceSubject2.subjectValue = scienceSubject1.subjectValue;
        scienceSubject2.science = scienceSubject1.science;
        scienceSubject2.scienceCap = scienceSubject1.scienceCap;
      }
      SubSystem<ShareScienceSubjectSystem>.System.StopIgnoringEvents();
      LunaLog.Log("Science subject received: " + subject.Id);
    }

    private static ScienceSubject ConvertByteArrayToScienceSubject(
      byte[] data,
      int numBytes)
    {
      ConfigNode configNode = new ConfigNode("Science");
      try
      {
        configNode.AddData(data.DeserializeToConfigNode(numBytes));
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing science subject configNode: {0}", (object) ex));
        return (ScienceSubject) null;
      }
      return new ScienceSubject(configNode);
    }
  }
}
