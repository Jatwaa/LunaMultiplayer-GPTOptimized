// Decompiled with JetBrains decompiler
// Type: LmpCommon.Locks.LockStore
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Concurrent;

namespace LmpCommon.Locks
{
  public class LockStore
  {
    private readonly object _asteroidCometSyncLock = new object();
    private readonly object _contractSyncLock = new object();

    internal LockDefinition ContractLock { get; set; }

    internal LockDefinition AsteroidCometLock { get; set; }

    internal ConcurrentDictionary<Guid, LockDefinition> UpdateLocks { get; } = new ConcurrentDictionary<Guid, LockDefinition>();

    internal ConcurrentDictionary<Guid, LockDefinition> UnloadedUpdateLocks { get; } = new ConcurrentDictionary<Guid, LockDefinition>();

    internal ConcurrentDictionary<Guid, LockDefinition> ControlLocks { get; } = new ConcurrentDictionary<Guid, LockDefinition>();

    internal ConcurrentDictionary<string, LockDefinition> KerbalLocks { get; } = new ConcurrentDictionary<string, LockDefinition>();

    internal ConcurrentDictionary<string, LockDefinition> SpectatorLocks { get; } = new ConcurrentDictionary<string, LockDefinition>();

    public void AddOrUpdateLock(LockDefinition lockDefinition)
    {
      LockDefinition safeLockDefinition = (LockDefinition) lockDefinition.Clone();
      switch (safeLockDefinition.Type)
      {
        case LockType.Contract:
          lock (this._contractSyncLock)
          {
            if (this.ContractLock == (LockDefinition) null)
            {
              this.ContractLock = new LockDefinition(LockType.Contract, safeLockDefinition.PlayerName);
              break;
            }
            this.ContractLock.PlayerName = safeLockDefinition.PlayerName;
            break;
          }
        case LockType.AsteroidComet:
          lock (this._asteroidCometSyncLock)
          {
            if (this.AsteroidCometLock == (LockDefinition) null)
            {
              this.AsteroidCometLock = new LockDefinition(LockType.AsteroidComet, safeLockDefinition.PlayerName);
              break;
            }
            this.AsteroidCometLock.PlayerName = safeLockDefinition.PlayerName;
            break;
          }
        case LockType.Kerbal:
          this.KerbalLocks.AddOrUpdate(safeLockDefinition.KerbalName, safeLockDefinition, (Func<string, LockDefinition, LockDefinition>) ((key, existingVal) =>
          {
            existingVal.PlayerName = safeLockDefinition.PlayerName;
            return existingVal;
          }));
          break;
        case LockType.Spectator:
          this.SpectatorLocks.AddOrUpdate(safeLockDefinition.PlayerName, safeLockDefinition, (Func<string, LockDefinition, LockDefinition>) ((key, existingVal) => safeLockDefinition));
          break;
        case LockType.UnloadedUpdate:
          this.UnloadedUpdateLocks.AddOrUpdate(safeLockDefinition.VesselId, safeLockDefinition, (Func<Guid, LockDefinition, LockDefinition>) ((key, existingVal) =>
          {
            existingVal.PlayerName = safeLockDefinition.PlayerName;
            return existingVal;
          }));
          break;
        case LockType.Update:
          this.UpdateLocks.AddOrUpdate(safeLockDefinition.VesselId, safeLockDefinition, (Func<Guid, LockDefinition, LockDefinition>) ((key, existingVal) =>
          {
            existingVal.PlayerName = safeLockDefinition.PlayerName;
            return existingVal;
          }));
          break;
        case LockType.Control:
          this.ControlLocks.AddOrUpdate(safeLockDefinition.VesselId, safeLockDefinition, (Func<Guid, LockDefinition, LockDefinition>) ((key, existingVal) =>
          {
            existingVal.PlayerName = safeLockDefinition.PlayerName;
            return existingVal;
          }));
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RemoveLock(LockDefinition lockDefinition)
    {
      LockDefinition lockDefinition1;
      switch (lockDefinition.Type)
      {
        case LockType.Contract:
          lock (this._contractSyncLock)
          {
            this.ContractLock = (LockDefinition) null;
            break;
          }
        case LockType.AsteroidComet:
          lock (this._asteroidCometSyncLock)
          {
            this.AsteroidCometLock = (LockDefinition) null;
            break;
          }
        case LockType.Kerbal:
          this.KerbalLocks.TryRemove(lockDefinition.KerbalName, out lockDefinition1);
          break;
        case LockType.Spectator:
          this.SpectatorLocks.TryRemove(lockDefinition.PlayerName, out lockDefinition1);
          break;
        case LockType.UnloadedUpdate:
          this.UnloadedUpdateLocks.TryRemove(lockDefinition.VesselId, out lockDefinition1);
          break;
        case LockType.Update:
          this.UpdateLocks.TryRemove(lockDefinition.VesselId, out lockDefinition1);
          break;
        case LockType.Control:
          this.ControlLocks.TryRemove(lockDefinition.VesselId, out lockDefinition1);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RemoveLock(LockType lockType, string playerName, Guid vesselId, string kerbalName)
    {
      LockDefinition lockDefinition;
      switch (lockType)
      {
        case LockType.Contract:
          lock (this._contractSyncLock)
          {
            this.ContractLock = (LockDefinition) null;
            break;
          }
        case LockType.AsteroidComet:
          lock (this._asteroidCometSyncLock)
          {
            this.AsteroidCometLock = (LockDefinition) null;
            break;
          }
        case LockType.Kerbal:
          this.KerbalLocks.TryRemove(kerbalName, out lockDefinition);
          break;
        case LockType.Spectator:
          this.SpectatorLocks.TryRemove(playerName, out lockDefinition);
          break;
        case LockType.UnloadedUpdate:
          this.UnloadedUpdateLocks.TryRemove(vesselId, out lockDefinition);
          break;
        case LockType.Update:
          this.UpdateLocks.TryRemove(vesselId, out lockDefinition);
          break;
        case LockType.Control:
          this.ControlLocks.TryRemove(vesselId, out lockDefinition);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void ClearAllLocks()
    {
      lock (this._asteroidCometSyncLock)
        this.AsteroidCometLock = (LockDefinition) null;
      lock (this._contractSyncLock)
        this.ContractLock = (LockDefinition) null;
      this.UpdateLocks.Clear();
      this.KerbalLocks.Clear();
      this.ControlLocks.Clear();
      this.SpectatorLocks.Clear();
      this.UnloadedUpdateLocks.Clear();
    }
  }
}
