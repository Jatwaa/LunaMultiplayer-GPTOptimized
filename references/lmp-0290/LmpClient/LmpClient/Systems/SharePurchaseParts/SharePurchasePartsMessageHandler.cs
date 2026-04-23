// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SharePurchaseParts.SharePurchasePartsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.ShareCareer;
using LmpClient.Systems.ShareFunds;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace LmpClient.Systems.SharePurchaseParts
{
  public class SharePurchasePartsMessageHandler : 
    SubSystem<SharePurchasePartsSystem>,
    IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.PartPurchase || !(data is ShareProgressPartPurchaseMsgData partPurchaseMsgData))
        return;
      string techId = string.Copy(partPurchaseMsgData.TechId);
      string partName = string.Copy(partPurchaseMsgData.PartName);
      LunaLog.Log("Queue PartPurchase with: " + techId + " part " + partName);
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => SharePurchasePartsMessageHandler.PartPurchase(techId, partName)));
    }

    private static void PartPurchase(string techId, string partName)
    {
      SubSystem<SharePurchasePartsSystem>.System.StartIgnoringEvents();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StartIgnoringEvents();
      ProtoTechNode techState = ResearchAndDevelopment.Instance.GetTechState(techId);
      AvailablePart partInfoByName1 = PartLoader.getPartInfoByName(partName);
      if (techState != null && partInfoByName1 != null)
      {
        techState.partsPurchased.Add(partInfoByName1);
        GameEvents.OnPartPurchased.Fire(partInfoByName1);
        string identicalParts = partInfoByName1.identicalParts;
        char[] chArray = new char[1]{ ',' };
        foreach (string str in identicalParts.Split(chArray))
        {
          if (!string.IsNullOrEmpty(str))
          {
            AvailablePart partInfoByName2 = PartLoader.getPartInfoByName(str.Replace('_', '.').Trim());
            if (partInfoByName2 != null)
            {
              partInfoByName2.costsFunds = false;
              techState.partsPurchased.Add(partInfoByName2);
              GameEvents.OnPartPurchased.Fire(partInfoByName2);
              partInfoByName2.costsFunds = true;
            }
          }
        }
      }
      if (Object.op_Implicit((Object) RDController.Instance) && Object.op_Implicit((Object) RDController.Instance.partList))
      {
        RDController.Instance.partList.Refresh();
        RDController.Instance.UpdatePanel();
      }
      if (Object.op_Implicit((Object) EditorPartList.Instance))
        EditorPartList.Instance.Refresh();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StopIgnoringEvents();
      SubSystem<SharePurchasePartsSystem>.System.StopIgnoringEvents();
      LunaLog.Log("Part purchase received tech: " + techId + " part: " + partName);
    }
  }
}
