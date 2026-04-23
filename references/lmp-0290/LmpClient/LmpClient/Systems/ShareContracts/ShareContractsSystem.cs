// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareContracts.ShareContractsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.Lock;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;
using LmpCommon.Locks;

namespace LmpClient.Systems.ShareContracts
{
  public class ShareContractsSystem : 
    ShareProgressBaseSystem<ShareContractsSystem, ShareContractsMessageSender, ShareContractsMessageHandler>
  {
    public int DefaultContractGenerateIterations;

    public override string SystemName { get; } = nameof (ShareContractsSystem);

    private ShareContractsEvents ShareContractsEvents { get; } = new ShareContractsEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      ContractSystem.generateContractIterations = 0;
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) this.ShareContractsEvents, __methodptr(LockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) this.ShareContractsEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.ShareContractsEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.Contract.onAccepted.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractAccepted)));
      // ISSUE: method pointer
      GameEvents.Contract.onCancelled.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractCancelled)));
      // ISSUE: method pointer
      GameEvents.Contract.onCompleted.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractCompleted)));
      // ISSUE: method pointer
      GameEvents.Contract.onContractsListChanged.Add(new EventVoid.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractsListChanged)));
      // ISSUE: method pointer
      GameEvents.Contract.onContractsLoaded.Add(new EventVoid.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractsLoaded)));
      // ISSUE: method pointer
      GameEvents.Contract.onDeclined.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractDeclined)));
      // ISSUE: method pointer
      GameEvents.Contract.onFailed.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractFailed)));
      // ISSUE: method pointer
      GameEvents.Contract.onFinished.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractFinished)));
      // ISSUE: method pointer
      GameEvents.Contract.onOffered.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractOffered)));
      // ISSUE: method pointer
      GameEvents.Contract.onParameterChange.Add(new EventData<Contract, ContractParameter>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractParameterChanged)));
      // ISSUE: method pointer
      GameEvents.Contract.onRead.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractRead)));
      // ISSUE: method pointer
      GameEvents.Contract.onSeen.Add(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractSeen)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      ContractSystem.generateContractIterations = this.DefaultContractGenerateIterations;
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Remove(new EventData<LockDefinition>.OnEvent((object) this.ShareContractsEvents, __methodptr(LockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) this.ShareContractsEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.ShareContractsEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      GameEvents.Contract.onAccepted.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractAccepted)));
      // ISSUE: method pointer
      GameEvents.Contract.onCancelled.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractCancelled)));
      // ISSUE: method pointer
      GameEvents.Contract.onCompleted.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractCompleted)));
      // ISSUE: method pointer
      GameEvents.Contract.onContractsListChanged.Remove(new EventVoid.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractsListChanged)));
      // ISSUE: method pointer
      GameEvents.Contract.onContractsLoaded.Remove(new EventVoid.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractsLoaded)));
      // ISSUE: method pointer
      GameEvents.Contract.onDeclined.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractDeclined)));
      // ISSUE: method pointer
      GameEvents.Contract.onFailed.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractFailed)));
      // ISSUE: method pointer
      GameEvents.Contract.onFinished.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractFinished)));
      // ISSUE: method pointer
      GameEvents.Contract.onOffered.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractOffered)));
      // ISSUE: method pointer
      GameEvents.Contract.onParameterChange.Remove(new EventData<Contract, ContractParameter>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractParameterChanged)));
      // ISSUE: method pointer
      GameEvents.Contract.onRead.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractRead)));
      // ISSUE: method pointer
      GameEvents.Contract.onSeen.Remove(new EventData<Contract>.OnEvent((object) this.ShareContractsEvents, __methodptr(ContractSeen)));
    }

    public void TryGetContractLock()
    {
      if (LockSystem.LockQuery.ContractLockExists())
        return;
      System<LockSystem>.Singleton.AcquireContractLock();
    }
  }
}
