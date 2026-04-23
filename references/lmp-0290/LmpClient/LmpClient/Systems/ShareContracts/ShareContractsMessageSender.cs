// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareContracts.ShareContractsMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;

namespace LmpClient.Systems.ShareContracts
{
  public class ShareContractsMessageSender : SubSystem<ShareContractsSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ShareProgressCliMsg>(msg))));

    public void SendContractMessage(Contract[] contracts)
    {
      List<ContractInfo> contractInfoList = new List<ContractInfo>();
      foreach (Contract contract in contracts)
      {
        ConfigNode configNode = ShareContractsMessageSender.ConvertContractToConfigNode(contract);
        if (configNode != null)
        {
          byte[] numArray = configNode.Serialize();
          int length = numArray.Length;
          contractInfoList.Add(new ContractInfo()
          {
            ContractGuid = contract.ContractGuid,
            Data = numArray,
            NumBytes = length
          });
        }
        else
          break;
      }
      ShareProgressContractsMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ShareProgressContractsMsgData>();
      newMessageData.Contracts = contractInfoList.ToArray();
      newMessageData.ContractCount = newMessageData.Contracts.Length;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void SendContractMessage(Contract contract) => this.SendContractMessage(new Contract[1]
    {
      contract
    });

    private static ConfigNode ConvertContractToConfigNode(Contract contract)
    {
      ConfigNode configNode = new ConfigNode();
      try
      {
        contract.Save(configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while saving contract: {0}", (object) ex));
        return (ConfigNode) null;
      }
      return configNode;
    }
  }
}
