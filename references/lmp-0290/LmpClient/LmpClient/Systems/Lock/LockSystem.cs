// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Lock.LockSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpCommon.Locks;
using LmpCommon.Message.Data.Lock;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Systems.Lock
{
  public class LockSystem : MessageSystem<LockSystem, LockMessageSender, LockMessageHandler>
  {
    public static LockStore LockStore { get; } = new LockStore();

    public static LockQuery LockQuery { get; } = new LockQuery(LockSystem.LockStore);

    public override string SystemName { get; } = nameof (LockSystem);

    protected override void OnDisabled()
    {
      base.OnDisabled();
      LockSystem.LockStore.ClearAllLocks();
    }

    private void AcquireLock(LockDefinition lockDefinition, bool force = false, bool immediate = false)
    {
      LockAcquireMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<LockAcquireMsgData>();
      newMessageData.Lock = lockDefinition;
      newMessageData.Force = force;
      this.MessageSender.SendMessage((IMessageData) newMessageData);
      if (!(force & immediate))
        return;
      LockSystem.LockStore.AddOrUpdateLock(lockDefinition);
    }

    public void AcquireControlLock(Guid vesselId, bool force = false, bool immediate = false)
    {
      if (LockSystem.LockQuery.ControlLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.AcquireLock(new LockDefinition(LockType.Control, SettingsSystem.CurrentSettings.PlayerName, vesselId), force, immediate);
    }

    public void AcquireKerbalLock(string kerbalName, bool force = false, bool immediate = false)
    {
      if (LockSystem.LockQuery.KerbalLockBelongsToPlayer(kerbalName, SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.AcquireLock(new LockDefinition(LockType.Kerbal, SettingsSystem.CurrentSettings.PlayerName, kerbalName), force, immediate);
    }

    public void AcquireKerbalLock(Vessel vessel, bool force = false, bool immediate = false)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      foreach (ProtoCrewMember protoCrewMember in vessel.GetVesselCrew())
      {
        if (protoCrewMember != null && !LockSystem.LockQuery.KerbalLockBelongsToPlayer(protoCrewMember.name, SettingsSystem.CurrentSettings.PlayerName))
          this.AcquireLock(new LockDefinition(LockType.Kerbal, SettingsSystem.CurrentSettings.PlayerName, protoCrewMember.name), force, immediate);
      }
    }

    public void AcquireKerbalLock(Guid vesselId, bool force = false, bool immediate = false) => this.AcquireKerbalLock(FlightGlobals.FindVessel(vesselId), force, immediate);

    public void AcquireUpdateLock(Guid vesselId, bool force = false, bool immediate = false)
    {
      if (LockSystem.LockQuery.UpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.AcquireLock(new LockDefinition(LockType.Update, SettingsSystem.CurrentSettings.PlayerName, vesselId), force, immediate);
    }

    public void AcquireUnloadedUpdateLock(Guid vesselId, bool force = false, bool immediate = false)
    {
      if (LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.AcquireLock(new LockDefinition(LockType.UnloadedUpdate, SettingsSystem.CurrentSettings.PlayerName, vesselId), force, immediate);
    }

    public void AcquireSpectatorLock()
    {
      if (LockSystem.LockQuery.SpectatorLockExists(SettingsSystem.CurrentSettings.PlayerName))
        return;
      this.AcquireLock(new LockDefinition(LockType.Spectator, SettingsSystem.CurrentSettings.PlayerName));
    }

    public void AcquireAsteroidLock() => this.AcquireLock(new LockDefinition(LockType.AsteroidComet, SettingsSystem.CurrentSettings.PlayerName));

    public void AcquireContractLock() => this.AcquireLock(new LockDefinition(LockType.Contract, SettingsSystem.CurrentSettings.PlayerName));

    private void ReleaseLock(LockDefinition lockDefinition)
    {
      LockReleaseMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<LockReleaseMsgData>();
      newMessageData.Lock = lockDefinition;
      LockEvent.onLockRelease.Fire(lockDefinition);
      LockSystem.LockStore.RemoveLock(lockDefinition);
      this.MessageSender.SendMessage((IMessageData) newMessageData);
    }

    public void ReleaseUpdateLock(Guid vesselId) => this.ReleaseLock(new LockDefinition(LockType.Update, SettingsSystem.CurrentSettings.PlayerName, vesselId));

    public void ReleaseKerbalLock(string kerbalName, float delayInSec)
    {
      if ((double) delayInSec > 0.0)
        CoroutineUtil.StartDelayedRoutine(nameof (ReleaseKerbalLock), (Action) (() => this.ReleaseLock(new LockDefinition(LockType.Kerbal, SettingsSystem.CurrentSettings.PlayerName, kerbalName))), delayInSec);
      else
        this.ReleaseLock(new LockDefinition(LockType.Kerbal, SettingsSystem.CurrentSettings.PlayerName, kerbalName));
    }

    public void ReleaseSpectatorLock() => this.ReleaseLock(new LockDefinition(LockType.Spectator, SettingsSystem.CurrentSettings.PlayerName));

    public void ReleaseAllVesselLocks(
      IEnumerable<string> crewNames,
      Guid vesselId,
      float delayInSec = 0.0f)
    {
      if ((double) delayInSec > 0.0)
        CoroutineUtil.StartDelayedRoutine(nameof (ReleaseAllVesselLocks), (Action) (() => this.ReleaseAllVesselLocksImpl(crewNames, vesselId)), delayInSec);
      else
        this.ReleaseAllVesselLocksImpl(crewNames, vesselId);
    }

    public void ReleaseControlLocksExcept(Guid vesselId)
    {
      foreach (LockDefinition lockDefinition in LockSystem.LockQuery.GetAllControlLocks(SettingsSystem.CurrentSettings.PlayerName).Where<LockDefinition>((Func<LockDefinition, bool>) (v => v.VesselId != vesselId)).ToArray<LockDefinition>())
        this.ReleaseLock(lockDefinition);
    }

    public void ReleaseAllPlayerSpecifiedLocks(params LockType[] lockTypes)
    {
      foreach (LockDefinition allPlayerLock in LockSystem.LockQuery.GetAllPlayerLocks(SettingsSystem.CurrentSettings.PlayerName))
      {
        if (((IEnumerable<LockType>) lockTypes).Contains<LockType>(allPlayerLock.Type))
          this.ReleaseLock(allPlayerLock);
      }
    }

    public void ReleasePlayerLocks(LockType type)
    {
      string playerName = SettingsSystem.CurrentSettings.PlayerName;
      IEnumerable<LockDefinition> source;
      switch (type)
      {
        case LockType.Contract:
          LockDefinition[] lockDefinitionArray1;
          if (!(LockSystem.LockQuery.ContractLockOwner() == playerName))
            lockDefinitionArray1 = new LockDefinition[0];
          else
            lockDefinitionArray1 = new LockDefinition[1]
            {
              LockSystem.LockQuery.ContractLock()
            };
          source = (IEnumerable<LockDefinition>) lockDefinitionArray1;
          break;
        case LockType.AsteroidComet:
          LockDefinition[] lockDefinitionArray2;
          if (!(LockSystem.LockQuery.AsteroidCometLockOwner() == playerName))
            lockDefinitionArray2 = new LockDefinition[0];
          else
            lockDefinitionArray2 = new LockDefinition[1]
            {
              LockSystem.LockQuery.AsteroidCometLock()
            };
          source = (IEnumerable<LockDefinition>) lockDefinitionArray2;
          break;
        case LockType.Kerbal:
          source = LockSystem.LockQuery.GetAllKerbalLocks(SettingsSystem.CurrentSettings.PlayerName);
          break;
        case LockType.Spectator:
          LockDefinition[] lockDefinitionArray3;
          if (!LockSystem.LockQuery.SpectatorLockExists(SettingsSystem.CurrentSettings.PlayerName))
            lockDefinitionArray3 = new LockDefinition[0];
          else
            lockDefinitionArray3 = new LockDefinition[1]
            {
              LockSystem.LockQuery.GetSpectatorLock(SettingsSystem.CurrentSettings.PlayerName)
            };
          source = (IEnumerable<LockDefinition>) lockDefinitionArray3;
          break;
        case LockType.UnloadedUpdate:
          source = LockSystem.LockQuery.GetAllUnloadedUpdateLocks(SettingsSystem.CurrentSettings.PlayerName);
          break;
        case LockType.Update:
          source = LockSystem.LockQuery.GetAllUpdateLocks(SettingsSystem.CurrentSettings.PlayerName);
          break;
        case LockType.Control:
          source = LockSystem.LockQuery.GetAllControlLocks(SettingsSystem.CurrentSettings.PlayerName);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type), (object) type, (string) null);
      }
      foreach (LockDefinition lockDefinition in source.ToArray<LockDefinition>())
        this.ReleaseLock(lockDefinition);
    }

    public void FireVesselLocksEvents(Guid vesselId)
    {
      if (LockSystem.LockQuery.ControlLockExists(vesselId))
        LockEvent.onLockAcquire.Fire(LockSystem.LockQuery.GetControlLock(vesselId));
      if (LockSystem.LockQuery.UpdateLockExists(vesselId))
        LockEvent.onLockAcquire.Fire(LockSystem.LockQuery.GetUpdateLock(vesselId));
      if (!LockSystem.LockQuery.UnloadedUpdateLockExists(vesselId))
        return;
      LockEvent.onLockAcquire.Fire(LockSystem.LockQuery.GetUnloadedUpdateLock(vesselId));
    }

    private void ReleaseAllVesselLocksImpl(IEnumerable<string> crewNames, Guid vesselId)
    {
      if (LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        this.ReleaseLock(new LockDefinition(LockType.UnloadedUpdate, SettingsSystem.CurrentSettings.PlayerName, vesselId));
      if (LockSystem.LockQuery.UpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        this.ReleaseLock(new LockDefinition(LockType.Update, SettingsSystem.CurrentSettings.PlayerName, vesselId));
      if (LockSystem.LockQuery.ControlLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName))
        this.ReleaseLock(new LockDefinition(LockType.Control, SettingsSystem.CurrentSettings.PlayerName, vesselId));
      if (crewNames == null)
        return;
      foreach (string crewName in crewNames)
      {
        if (LockSystem.LockQuery.KerbalLockBelongsToPlayer(crewName, SettingsSystem.CurrentSettings.PlayerName))
          this.ReleaseLock(new LockDefinition(LockType.Kerbal, SettingsSystem.CurrentSettings.PlayerName, crewName));
      }
    }
  }
}
