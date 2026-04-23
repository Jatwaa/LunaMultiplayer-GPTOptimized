// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareContracts.ShareContractsEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using Contracts.Templates;
using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Locks;

namespace LmpClient.Systems.ShareContracts
{
  public class ShareContractsEvents : SubSystem<ShareContractsSystem>
  {
    public void LockAcquire(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Contract || !(lockDefinition.PlayerName == SettingsSystem.CurrentSettings.PlayerName))
        return;
      ContractSystem.generateContractIterations = System<ShareContractsSystem>.Singleton.DefaultContractGenerateIterations;
    }

    public void LockReleased(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Contract)
        return;
      SubSystem<ShareContractsSystem>.System.TryGetContractLock();
    }

    public void LevelLoaded(GameScenes data) => SubSystem<ShareContractsSystem>.System.TryGetContractLock();

    public void ContractAccepted(Contract contract)
    {
      if (SubSystem<ShareContractsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      LunaLog.Log(string.Format("Contract accepted: {0}", (object) contract.ContractGuid));
    }

    public void ContractCancelled(Contract contract)
    {
      if (SubSystem<ShareContractsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      LunaLog.Log(string.Format("Contract cancelled: {0}", (object) contract.ContractGuid));
    }

    public void ContractCompleted(Contract contract)
    {
      if (SubSystem<ShareContractsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      LunaLog.Log(string.Format("Contract completed: {0}", (object) contract.ContractGuid));
    }

    public void ContractsListChanged() => LunaLog.Log("Contract list changed.");

    public void ContractsLoaded() => LunaLog.Log("Contracts loaded.");

    public void ContractDeclined(Contract contract)
    {
      if (SubSystem<ShareContractsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      LunaLog.Log(string.Format("Contract declined: {0}", (object) contract.ContractGuid));
    }

    public void ContractFailed(Contract contract)
    {
      if (SubSystem<ShareContractsSystem>.System.IgnoreEvents)
        return;
      SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      LunaLog.Log(string.Format("Contract failed: {0}", (object) contract.ContractGuid));
    }

    public void ContractFinished(Contract contract)
    {
    }

    public void ContractOffered(Contract contract)
    {
      if (!LockSystem.LockQuery.ContractLockBelongsToPlayer(SettingsSystem.CurrentSettings.PlayerName))
      {
        contract.Withdraw();
        contract.Kill();
      }
      else if (((object) contract).GetType() == typeof (RecoverAsset))
      {
        contract.Withdraw();
        contract.Kill();
      }
      else
      {
        LunaLog.Log(string.Format("Contract offered: {0} - {1}", (object) contract.ContractGuid, (object) contract.Title));
        SubSystem<ShareContractsSystem>.System.MessageSender.SendContractMessage(contract);
      }
    }

    public void ContractParameterChanged(Contract contract, ContractParameter contractParameter) => LunaLog.Log(string.Format("Contract parameter changed on:{0}", (object) contract.ContractGuid));

    public void ContractRead(Contract contract) => LunaLog.Log(string.Format("Contract read:{0}", (object) contract.ContractGuid));

    public void ContractSeen(Contract contract) => LunaLog.Log(string.Format("Contract seen:{0}", (object) contract.ContractGuid));
  }
}
