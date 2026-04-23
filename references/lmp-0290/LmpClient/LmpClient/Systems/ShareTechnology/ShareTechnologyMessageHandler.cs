// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareTechnology.ShareTechnologyMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.ShareTechnology
{
  public class ShareTechnologyMessageHandler : SubSystem<ShareTechnologySystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.TechnologyUpdate || !(data is ShareProgressTechnologyMsgData technologyMsgData))
        return;
      TechNodeInfo tech = new TechNodeInfo(technologyMsgData.TechNode);
      LunaLog.Log("Queue TechnologyResearch with: " + tech.Id);
      SubSystem<ShareTechnologySystem>.System.QueueAction((Action) (() => ShareTechnologyMessageHandler.TechnologyResearch(tech)));
    }

    private static void TechnologyResearch(TechNodeInfo tech)
    {
      SubSystem<ShareTechnologySystem>.System.StartIgnoringEvents();
      ProtoTechNode protoTechNode = ((IEnumerable<ProtoTechNode>) AssetBase.RnDTechTree.GetTreeTechs()).ToList<ProtoTechNode>().Find((Predicate<ProtoTechNode>) (n => n.techID == tech.Id));
      ResearchAndDevelopment.Instance.UnlockProtoTechNode(protoTechNode);
      if (Object.op_Implicit((Object) RDController.Instance) && Object.op_Implicit((Object) RDController.Instance.partList))
      {
        RDController.Instance.partList.Refresh();
        RDController.Instance.UpdatePanel();
      }
      ResearchAndDevelopment.RefreshTechTreeUI();
      if (Object.op_Implicit((Object) EditorPartList.Instance))
        EditorPartList.Instance.Refresh();
      SubSystem<ShareTechnologySystem>.System.StopIgnoringEvents();
      LunaLog.Log("TechnologyResearch received - technology researched: " + tech.Id);
    }
  }
}
