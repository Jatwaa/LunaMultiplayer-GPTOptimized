// Decompiled with JetBrains decompiler
// Type: LmpCommon.Locks.LockQuery
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpCommon.Locks
{
  public class LockQuery
  {
    private LockStore LockStore { get; }

    public LockQuery(LockStore lockStore) => this.LockStore = lockStore;

    public bool LockBelongsToPlayer(
      LockType type,
      Guid vesselId,
      string kerbalName,
      string playerName)
    {
      switch (type)
      {
        case LockType.Contract:
          return this.LockStore.ContractLock?.PlayerName == playerName;
        case LockType.AsteroidComet:
          return this.LockStore.AsteroidCometLock?.PlayerName == playerName;
        case LockType.Kerbal:
          LockDefinition lockDefinition1;
          if (this.LockStore.KerbalLocks.TryGetValue(kerbalName, out lockDefinition1))
            return lockDefinition1.Type == LockType.Kerbal && lockDefinition1.KerbalName == kerbalName && lockDefinition1.PlayerName == playerName;
          break;
        case LockType.Spectator:
          return this.SpectatorLockExists(playerName);
        case LockType.UnloadedUpdate:
          LockDefinition lockDefinition2;
          if (this.LockStore.UnloadedUpdateLocks.TryGetValue(vesselId, out lockDefinition2))
            return lockDefinition2.Type == LockType.UnloadedUpdate && lockDefinition2.VesselId == vesselId && lockDefinition2.PlayerName == playerName;
          break;
        case LockType.Update:
          LockDefinition lockDefinition3;
          if (this.LockStore.UpdateLocks.TryGetValue(vesselId, out lockDefinition3))
            return lockDefinition3.Type == LockType.Update && lockDefinition3.VesselId == vesselId && lockDefinition3.PlayerName == playerName;
          break;
        case LockType.Control:
          LockDefinition lockDefinition4;
          if (this.LockStore.ControlLocks.TryGetValue(vesselId, out lockDefinition4))
            return lockDefinition4.Type == LockType.Control && lockDefinition4.VesselId == vesselId && lockDefinition4.PlayerName == playerName;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type), (object) type, (string) null);
      }
      return false;
    }

    public bool LockExists(LockType type, Guid vesselId, string kerbalName)
    {
      switch (type)
      {
        case LockType.Contract:
          return this.LockStore.ContractLock != (LockDefinition) null;
        case LockType.AsteroidComet:
          return this.LockStore.AsteroidCometLock != (LockDefinition) null;
        case LockType.Kerbal:
          return this.LockStore.KerbalLocks.ContainsKey(kerbalName);
        case LockType.UnloadedUpdate:
          return this.LockStore.UnloadedUpdateLocks.ContainsKey(vesselId);
        case LockType.Update:
          return this.LockStore.UpdateLocks.ContainsKey(vesselId);
        case LockType.Control:
          return this.LockStore.ControlLocks.ContainsKey(vesselId);
        default:
          throw new ArgumentOutOfRangeException(nameof (type), (object) type, (string) null);
      }
    }

    private string GetLockOwner(LockType type, Guid vesselId, string kerbalName)
    {
      switch (type)
      {
        case LockType.Contract:
          return this.LockStore.ContractLock?.PlayerName;
        case LockType.AsteroidComet:
          return this.LockStore.AsteroidCometLock?.PlayerName;
        case LockType.Kerbal:
          LockDefinition lockDefinition1;
          if (this.LockStore.KerbalLocks.TryGetValue(kerbalName, out lockDefinition1))
            return lockDefinition1.PlayerName;
          break;
        case LockType.UnloadedUpdate:
          LockDefinition lockDefinition2;
          if (this.LockStore.UnloadedUpdateLocks.TryGetValue(vesselId, out lockDefinition2))
            return lockDefinition2.PlayerName;
          break;
        case LockType.Update:
          LockDefinition lockDefinition3;
          if (this.LockStore.UpdateLocks.TryGetValue(vesselId, out lockDefinition3))
            return lockDefinition3.PlayerName;
          break;
        case LockType.Control:
          LockDefinition lockDefinition4;
          if (this.LockStore.ControlLocks.TryGetValue(vesselId, out lockDefinition4))
            return lockDefinition4.PlayerName;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type), (object) type, (string) null);
      }
      return (string) null;
    }

    public IEnumerable<LockDefinition> GetAllPlayerLocks(string playerName)
    {
      List<LockDefinition> allPlayerLocks = new List<LockDefinition>();
      allPlayerLocks.AddRange(this.GetAllKerbalLocks(playerName));
      allPlayerLocks.AddRange(this.GetAllControlLocks(playerName));
      allPlayerLocks.AddRange(this.GetAllUpdateLocks(playerName));
      allPlayerLocks.AddRange(this.GetAllUnloadedUpdateLocks(playerName));
      LockDefinition lockDefinition;
      if (this.LockStore.SpectatorLocks.TryGetValue(playerName, out lockDefinition))
        allPlayerLocks.Add(lockDefinition);
      if (this.AsteroidCometLockBelongsToPlayer(playerName))
        allPlayerLocks.Add(this.LockStore.AsteroidCometLock);
      if (this.ContractLockBelongsToPlayer(playerName))
        allPlayerLocks.Add(this.LockStore.ContractLock);
      return (IEnumerable<LockDefinition>) allPlayerLocks;
    }

    public IEnumerable<LockDefinition> GetAllLocks()
    {
      List<LockDefinition> allLocks = new List<LockDefinition>();
      allLocks.AddRange(this.GetAllKerbalLocks());
      allLocks.AddRange(this.GetAllControlLocks());
      allLocks.AddRange(this.GetAllUpdateLocks());
      allLocks.AddRange(this.GetAllUnloadedUpdateLocks());
      allLocks.AddRange((IEnumerable<LockDefinition>) this.LockStore.SpectatorLocks.Values);
      if (this.LockStore.AsteroidCometLock != (LockDefinition) null)
        allLocks.Add(this.LockStore.AsteroidCometLock);
      if (this.LockStore.ContractLock != (LockDefinition) null)
        allLocks.Add(this.LockStore.ContractLock);
      return (IEnumerable<LockDefinition>) allLocks;
    }

    public bool LockExists(LockDefinition lockDefinition)
    {
      switch (lockDefinition.Type)
      {
        case LockType.Contract:
          return this.LockStore.ContractLock != (LockDefinition) null;
        case LockType.AsteroidComet:
          return this.LockStore.AsteroidCometLock != (LockDefinition) null;
        case LockType.Kerbal:
          return this.LockStore.SpectatorLocks.ContainsKey(lockDefinition.KerbalName);
        case LockType.Spectator:
          return this.LockStore.SpectatorLocks.ContainsKey(lockDefinition.PlayerName);
        case LockType.UnloadedUpdate:
          return this.LockStore.UnloadedUpdateLocks.ContainsKey(lockDefinition.VesselId);
        case LockType.Update:
          return this.LockStore.UpdateLocks.ContainsKey(lockDefinition.VesselId);
        case LockType.Control:
          return this.LockStore.ControlLocks.ContainsKey(lockDefinition.VesselId);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public LockDefinition GetLock(
      LockType lockType,
      string playerName,
      Guid vesselId,
      string kerbalName)
    {
      LockDefinition lockDefinition;
      switch (lockType)
      {
        case LockType.Contract:
          return this.LockStore.ContractLock;
        case LockType.AsteroidComet:
          return this.LockStore.AsteroidCometLock;
        case LockType.Kerbal:
          this.LockStore.KerbalLocks.TryGetValue(kerbalName, out lockDefinition);
          break;
        case LockType.Spectator:
          this.LockStore.SpectatorLocks.TryGetValue(playerName, out lockDefinition);
          break;
        case LockType.UnloadedUpdate:
          this.LockStore.UnloadedUpdateLocks.TryGetValue(vesselId, out lockDefinition);
          break;
        case LockType.Update:
          this.LockStore.UpdateLocks.TryGetValue(vesselId, out lockDefinition);
          break;
        case LockType.Control:
          this.LockStore.ControlLocks.TryGetValue(vesselId, out lockDefinition);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return lockDefinition;
    }

    public bool CanRecoverOrTerminateTheVessel(Guid vesselId, string playerName) => !this.ControlLockExists(vesselId) || this.ControlLockBelongsToPlayer(vesselId, playerName);

    public bool AsteroidCometLockExists() => this.LockExists(LockType.AsteroidComet, Guid.Empty, string.Empty);

    public bool AsteroidCometLockBelongsToPlayer(string playerName) => this.LockBelongsToPlayer(LockType.AsteroidComet, Guid.Empty, string.Empty, playerName);

    public string AsteroidCometLockOwner() => this.GetLockOwner(LockType.AsteroidComet, Guid.Empty, string.Empty);

    public LockDefinition AsteroidCometLock() => this.GetLock(LockType.AsteroidComet, string.Empty, Guid.Empty, string.Empty);

    public bool ContractLockExists() => this.LockExists(LockType.Contract, Guid.Empty, string.Empty);

    public bool ContractLockBelongsToPlayer(string playerName) => this.LockBelongsToPlayer(LockType.Contract, Guid.Empty, string.Empty, playerName);

    public string ContractLockOwner() => this.GetLockOwner(LockType.Contract, Guid.Empty, string.Empty);

    public LockDefinition ContractLock() => this.GetLock(LockType.Contract, string.Empty, Guid.Empty, string.Empty);

    public bool ControlLockExists(Guid vesselId) => this.LockExists(LockType.Control, vesselId, string.Empty);

    public bool ControlLockBelongsToPlayer(Guid vesselId, string playerName) => this.LockBelongsToPlayer(LockType.Control, vesselId, string.Empty, playerName);

    public LockDefinition GetControlLock(Guid vesselId) => this.LockStore.ControlLocks[vesselId];

    public string GetControlLockOwner(Guid vesselId) => this.GetLockOwner(LockType.Control, vesselId, string.Empty);

    public IEnumerable<LockDefinition> GetAllControlLocks(string playerName) => this.LockStore.ControlLocks.Where<KeyValuePair<Guid, LockDefinition>>((Func<KeyValuePair<Guid, LockDefinition>, bool>) (v => v.Value.PlayerName == playerName)).Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));

    public IEnumerable<LockDefinition> GetAllControlLocks() => this.LockStore.ControlLocks.Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));

    public bool KerbalLockExists(string kerbalName) => this.LockExists(LockType.Kerbal, Guid.Empty, kerbalName);

    public bool KerbalLockBelongsToPlayer(string kerbalName, string playerName) => this.LockBelongsToPlayer(LockType.Kerbal, Guid.Empty, kerbalName, playerName);

    public LockDefinition GetKerbalLock(string kerbalName) => this.LockStore.KerbalLocks[kerbalName];

    public string GetKerbalLockOwner(string kerbalName) => this.GetLockOwner(LockType.Kerbal, Guid.Empty, kerbalName);

    public IEnumerable<LockDefinition> GetAllKerbalLocks(string playerName) => this.LockStore.KerbalLocks.Where<KeyValuePair<string, LockDefinition>>((Func<KeyValuePair<string, LockDefinition>, bool>) (v => v.Value.PlayerName == playerName)).Select<KeyValuePair<string, LockDefinition>, LockDefinition>((Func<KeyValuePair<string, LockDefinition>, LockDefinition>) (v => v.Value));

    public IEnumerable<LockDefinition> GetAllKerbalLocks() => this.LockStore.KerbalLocks.Select<KeyValuePair<string, LockDefinition>, LockDefinition>((Func<KeyValuePair<string, LockDefinition>, LockDefinition>) (v => v.Value));

    public bool SpectatorLockExists(string playerName) => this.LockStore.SpectatorLocks.ContainsKey(playerName);

    public LockDefinition GetSpectatorLock(string playerName)
    {
      LockDefinition lockDefinition;
      return this.LockStore.SpectatorLocks.TryGetValue(playerName, out lockDefinition) ? lockDefinition : (LockDefinition) null;
    }

    public bool UnloadedUpdateLockExists(Guid vesselId) => this.LockExists(LockType.UnloadedUpdate, vesselId, string.Empty);

    public bool UnloadedUpdateLockBelongsToPlayer(Guid vesselId, string playerName) => this.LockBelongsToPlayer(LockType.UnloadedUpdate, vesselId, string.Empty, playerName);

    public LockDefinition GetUnloadedUpdateLock(Guid vesselId) => this.LockStore.UnloadedUpdateLocks[vesselId];

    public string GetUnloadedUpdateLockOwner(Guid vesselId) => this.GetLockOwner(LockType.UnloadedUpdate, vesselId, string.Empty);

    public IEnumerable<LockDefinition> GetAllUnloadedUpdateLocks(
      string playerName)
    {
      return this.LockStore.UnloadedUpdateLocks.Where<KeyValuePair<Guid, LockDefinition>>((Func<KeyValuePair<Guid, LockDefinition>, bool>) (v => v.Value.PlayerName == playerName)).Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));
    }

    public IEnumerable<LockDefinition> GetAllUnloadedUpdateLocks() => this.LockStore.UnloadedUpdateLocks.Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));

    public bool UpdateLockExists(Guid vesselId) => this.LockExists(LockType.Update, vesselId, string.Empty);

    public bool UpdateLockBelongsToPlayer(Guid vesselId, string playerName) => this.LockBelongsToPlayer(LockType.Update, vesselId, string.Empty, playerName);

    public LockDefinition GetUpdateLock(Guid vesselId) => this.LockStore.UpdateLocks[vesselId];

    public string GetUpdateLockOwner(Guid vesselId) => this.GetLockOwner(LockType.Update, vesselId, string.Empty);

    public IEnumerable<LockDefinition> GetAllUpdateLocks(string playerName) => this.LockStore.UpdateLocks.Where<KeyValuePair<Guid, LockDefinition>>((Func<KeyValuePair<Guid, LockDefinition>, bool>) (v => v.Value.PlayerName == playerName)).Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));

    public IEnumerable<LockDefinition> GetAllUpdateLocks() => this.LockStore.UpdateLocks.Select<KeyValuePair<Guid, LockDefinition>, LockDefinition>((Func<KeyValuePair<Guid, LockDefinition>, LockDefinition>) (v => v.Value));
  }
}
