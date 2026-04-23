// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareContracts.ShareContractsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Systems.ShareCareer;
using LmpClient.Systems.ShareExperimentalParts;
using LmpClient.Systems.ShareFunds;
using LmpClient.Systems.ShareReputation;
using LmpClient.Systems.ShareScience;
using LmpCommon.Message.Data.ShareProgress;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.ShareContracts
{
  public class ShareContractsMessageHandler : SubSystem<ShareContractsSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ShareProgressBaseMsgData data) || data.ShareProgressMessageType != ShareProgressMessageType.ContractsUpdate || !(data is ShareProgressContractsMsgData contractsMsgData))
        return;
      ContractInfo[] contractInfos = ShareContractsMessageHandler.CopyContracts(contractsMsgData.Contracts);
      LunaLog.Log("Queue ContractsUpdate.");
      LmpClient.Base.System<ShareCareerSystem>.Singleton.QueueAction((Action) (() => ShareContractsMessageHandler.ContractUpdate(contractInfos)));
    }

    private static ContractInfo[] CopyContracts(ContractInfo[] contracts)
    {
      ContractInfo[] contractInfoArray = new ContractInfo[contracts.Length];
      for (int index = 0; index < contracts.Length; ++index)
        contractInfoArray[index] = new ContractInfo(contracts[index]);
      return contractInfoArray;
    }

    private static void ContractUpdate(ContractInfo[] contractInfos)
    {
      SubSystem<ShareContractsSystem>.System.StartIgnoringEvents();
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StartIgnoringEvents();
      LmpClient.Base.System<ShareExperimentalPartsSystem>.Singleton.StartIgnoringEvents();
      foreach (ContractInfo contractInfo in contractInfos)
      {
        ContractInfo cInfo = contractInfo;
        Contract contract = ShareContractsMessageHandler.ConvertByteArrayToContract(cInfo.Data, cInfo.NumBytes);
        if (contract != null)
        {
          int index = ContractSystem.Instance.Contracts.FindIndex((Predicate<Contract>) (c => c.ContractGuid == cInfo.ContractGuid));
          if (index != -1)
            ShareContractsMessageHandler.UpdateContract(index, contract);
          else
            ShareContractsMessageHandler.AddContract(contract);
        }
      }
      LmpClient.Base.System<ShareFundsSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareScienceSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareReputationSystem>.Singleton.StopIgnoringEvents(true);
      LmpClient.Base.System<ShareExperimentalPartsSystem>.Singleton.StopIgnoringEvents();
      GameEvents.Contract.onContractsListChanged.Fire();
      SubSystem<ShareContractsSystem>.System.StopIgnoringEvents();
    }

    private static Contract ConvertByteArrayToContract(byte[] data, int numBytes)
    {
      ConfigNode configNode;
      try
      {
        configNode = data.DeserializeToConfigNode(numBytes);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing contract configNode: {0}", (object) ex));
        return (Contract) null;
      }
      if (configNode == null)
      {
        LunaLog.LogError("[LMP]: Error, the contract configNode was null.");
        return (Contract) null;
      }
      Contract contract;
      try
      {
        string str = configNode.GetValue("type");
        configNode.RemoveValues("type");
        contract = Contract.Load((Contract) Activator.CreateInstance(ContractSystem.GetContractType(str)), configNode);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("[LMP]: Error while deserializing contract: {0}", (object) ex));
        return (Contract) null;
      }
      return contract;
    }

    private static void UpdateContract(int contractIndex, Contract incomingContract)
    {
      if (ContractSystem.Instance.Contracts[contractIndex].ContractState != incomingContract.ContractState)
      {
        switch (incomingContract.ContractState - 1)
        {
          case 0:
            ContractSystem.Instance.Contracts[contractIndex].Offer();
            break;
          case 2:
            ContractSystem.Instance.Contracts[contractIndex].Decline();
            break;
          case 3:
            ContractSystem.Instance.Contracts[contractIndex].Cancel();
            break;
          case 4:
            ContractSystem.Instance.Contracts[contractIndex].Accept();
            break;
          case 5:
            ContractSystem.Instance.Contracts[contractIndex].Complete();
            break;
          case 7:
            ContractSystem.Instance.Contracts[contractIndex].Fail();
            break;
          case 8:
            ContractSystem.Instance.Contracts[contractIndex].Withdraw();
            break;
        }
      }
      else
      {
        ContractSystem.Instance.Contracts[contractIndex].Unregister();
        ContractSystem.Instance.Contracts[contractIndex] = incomingContract;
        if (ContractSystem.Instance.Contracts[contractIndex].ContractState == 5)
          ContractSystem.Instance.Contracts[contractIndex].Register();
      }
      LunaLog.Log(string.Format("Contract update received - contract state changed on: {0} - {1}", (object) incomingContract.ContractGuid, (object) incomingContract.Title));
    }

    private static void AddContract(Contract incomingContract)
    {
      if (!incomingContract.IsFinished())
      {
        ContractSystem.Instance.Contracts.Add(incomingContract);
        int index = ContractSystem.Instance.Contracts.FindIndex((Predicate<Contract>) (c => c.ContractGuid == incomingContract.ContractGuid));
        ContractSystem.Instance.Contracts[index].OnStateChange.Fire(incomingContract.ContractState);
        switch (ContractSystem.Instance.Contracts[index].ContractState - 1)
        {
          case 0:
            GameEvents.Contract.onOffered.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 2:
            GameEvents.Contract.onDeclined.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 3:
            GameEvents.Contract.onCancelled.Fire(ContractSystem.Instance.Contracts[index]);
            GameEvents.Contract.onFailed.Fire(ContractSystem.Instance.Contracts[index]);
            GameEvents.Contract.onFinished.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 4:
            ContractSystem.Instance.Contracts[index].Register();
            GameEvents.Contract.onAccepted.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 5:
            GameEvents.Contract.onCompleted.Fire(ContractSystem.Instance.Contracts[index]);
            GameEvents.Contract.onFinished.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 7:
            GameEvents.Contract.onFailed.Fire(ContractSystem.Instance.Contracts[index]);
            GameEvents.Contract.onFinished.Fire(ContractSystem.Instance.Contracts[index]);
            break;
          case 8:
            GameEvents.Contract.onFinished.Fire(ContractSystem.Instance.Contracts[index]);
            break;
        }
      }
      else
      {
        incomingContract.Unregister();
        if (incomingContract.ContractState == 6 || incomingContract.ContractState == 7 || incomingContract.ContractState == 8 || incomingContract.ContractState == 4)
          ContractSystem.Instance.ContractsFinished.Add(incomingContract);
      }
      LunaLog.Log(string.Format("New contract received: {0} - {1}", (object) incomingContract.ContractGuid, (object) incomingContract.Title));
    }
  }
}
