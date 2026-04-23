// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareExperimentalParts.ShareExperimentalPartsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.ShareCareer;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.ShareExperimentalParts
{
  public class ShareExperimentalPartsMessageHandler : 
    SubSystem<ShareExperimentalPartsSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.ExperimentalPart || !(data is ShareProgressExperimentalPartMsgData experimentalPartMsgData))
        return;
      string partName = string.Copy(experimentalPartMsgData.PartName);
      int count = experimentalPartMsgData.Count;
      LunaLog.Log(string.Format("Queue ExperimentalPart: part {0} count {1}", (object) partName, (object) count));
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => ShareExperimentalPartsMessageHandler.ExperimentalPart(partName, count)));
    }

    private static void ExperimentalPart(string partName, int count)
    {
      SubSystem<ShareExperimentalPartsSystem>.System.StartIgnoringEvents();
      AvailablePart partInfoByName = PartLoader.getPartInfoByName(partName);
      if (partInfoByName != null)
      {
        Dictionary<AvailablePart, int> dictionary = Traverse.Create((object) ResearchAndDevelopment.Instance).Field<Dictionary<AvailablePart, int>>("experimentalPartsStock").Value;
        int num;
        if (dictionary.TryGetValue(partInfoByName, out num))
        {
          if (count == 0)
            dictionary.Remove(partInfoByName);
          else if (num != count)
            dictionary[partInfoByName] = count;
        }
        else if (count > 0)
          dictionary.Add(partInfoByName, count);
      }
      if (Object.op_Implicit((Object) RDController.Instance) && Object.op_Implicit((Object) RDController.Instance.partList))
      {
        RDController.Instance.partList.Refresh();
        RDController.Instance.UpdatePanel();
      }
      if (Object.op_Implicit((Object) EditorPartList.Instance))
        EditorPartList.Instance.Refresh();
      SubSystem<ShareExperimentalPartsSystem>.System.StopIgnoringEvents();
      LunaLog.Log(string.Format("Experimental part received part: {0} count {1}", (object) partName, (object) count));
    }
  }
}
