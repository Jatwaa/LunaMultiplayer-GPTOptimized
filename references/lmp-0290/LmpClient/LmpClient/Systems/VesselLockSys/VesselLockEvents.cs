// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselLockSys.VesselLockEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.VesselUtilities;
using LmpCommon.Locks;
using UnityEngine;

namespace LmpClient.Systems.VesselLockSys
{
  public class VesselLockEvents : SubSystem<VesselLockSystem>
  {
    public void OnVesselChange(Vessel vessel)
    {
      if (Object.op_Equality((Object) vessel, (Object) null) || LockSystem.LockQuery.GetControlLockOwner(vessel.id) == SettingsSystem.CurrentSettings.PlayerName)
        return;
      System<LockSystem>.Singleton.ReleasePlayerLocks(LockType.Control);
      if (LockSystem.LockQuery.ControlLockExists(vessel.id) && !LockSystem.LockQuery.ControlLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName))
      {
        SubSystem<VesselLockSystem>.System.StartSpectating(vessel.id);
      }
      else
      {
        if (VesselCommon.IsSpectating)
          System<VesselLockSystem>.Singleton.StopSpectating();
        if (FlightDriver.flightStarted)
          System<LockSystem>.Singleton.AcquireControlLock(vessel.id);
      }
    }

    public void LevelLoaded(GameScenes data)
    {
      if (data != 7)
      {
        System<VesselLockSystem>.Singleton.StopSpectating();
        System<LockSystem>.Singleton.ReleaseAllPlayerSpecifiedLocks(LockType.Control, LockType.Update, LockType.Kerbal);
      }
      if (data == 7 || data == 8)
      {
        foreach (Vessel vessel in FlightGlobals.Vessels)
        {
          if (!LockSystem.LockQuery.UnloadedUpdateLockExists(vessel.id))
            System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(vessel.id);
        }
      }
      else
        System<LockSystem>.Singleton.ReleaseAllPlayerSpecifiedLocks(LockType.UnloadedUpdate);
    }

    public void VesselLoaded(Vessel vessel)
    {
      if (LockSystem.LockQuery.UpdateLockExists(vessel.id) || VesselCommon.IsSpectating)
        return;
      System<LockSystem>.Singleton.AcquireUpdateLock(vessel.id);
    }

    public void LockAcquire(LockDefinition lockDefinition)
    {
      switch (lockDefinition.Type)
      {
        case LockType.Update:
          if (lockDefinition.PlayerName != SettingsSystem.CurrentSettings.PlayerName)
          {
            if (!LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(lockDefinition.VesselId, SettingsSystem.CurrentSettings.PlayerName))
              break;
            LockSystem.LockStore.RemoveLock(LockSystem.LockQuery.GetUnloadedUpdateLock(lockDefinition.VesselId));
            LockSystem.LockStore.AddOrUpdateLock(new LockDefinition(LockType.UnloadedUpdate, lockDefinition.PlayerName, lockDefinition.VesselId));
            break;
          }
          System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(lockDefinition.VesselId, true);
          System<LockSystem>.Singleton.AcquireKerbalLock(lockDefinition.VesselId, true);
          break;
        case LockType.Control:
          if (lockDefinition.PlayerName == SettingsSystem.CurrentSettings.PlayerName)
          {
            if (VesselCommon.IsSpectating)
              System<VesselLockSystem>.Singleton.StopSpectating();
            System<LockSystem>.Singleton.AcquireUpdateLock(lockDefinition.VesselId, true);
            System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(lockDefinition.VesselId, true);
            System<LockSystem>.Singleton.AcquireKerbalLock(lockDefinition.VesselId, true);
            VesselCommon.RemoveVesselFromSystems(lockDefinition.VesselId);
            break;
          }
          if (Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == lockDefinition.VesselId)
            SubSystem<VesselLockSystem>.System.StartSpectating(lockDefinition.VesselId);
          if (LockSystem.LockQuery.UpdateLockBelongsToPlayer(lockDefinition.VesselId, SettingsSystem.CurrentSettings.PlayerName))
          {
            LockSystem.LockStore.RemoveLock(LockSystem.LockQuery.GetUpdateLock(lockDefinition.VesselId));
            LockSystem.LockStore.AddOrUpdateLock(new LockDefinition(LockType.UnloadedUpdate, lockDefinition.PlayerName, lockDefinition.VesselId));
          }
          if (LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(lockDefinition.VesselId, SettingsSystem.CurrentSettings.PlayerName))
          {
            LockSystem.LockStore.RemoveLock(LockSystem.LockQuery.GetUnloadedUpdateLock(lockDefinition.VesselId));
            LockSystem.LockStore.AddOrUpdateLock(new LockDefinition(LockType.UnloadedUpdate, lockDefinition.PlayerName, lockDefinition.VesselId));
          }
          break;
      }
    }

    public void LockReleased(LockDefinition lockDefinition)
    {
      switch (lockDefinition.Type)
      {
        case LockType.UnloadedUpdate:
        case LockType.Update:
          if (HighLogic.LoadedScene < 7 || VesselCommon.IsSpectating)
            break;
          Vessel vessel = FlightGlobals.FindVessel(lockDefinition.VesselId);
          if (!Object.op_Inequality((Object) vessel, (Object) null))
            break;
          switch (lockDefinition.Type)
          {
            case LockType.UnloadedUpdate:
              System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(lockDefinition.VesselId);
              break;
            case LockType.Update:
              if (vessel.loaded)
              {
                System<LockSystem>.Singleton.AcquireUpdateLock(lockDefinition.VesselId);
                break;
              }
              break;
          }
          break;
        case LockType.Control:
          if (!VesselCommon.IsSpectating || !Object.op_Implicit((Object) FlightGlobals.ActiveVessel) || !(FlightGlobals.ActiveVessel.id == lockDefinition.VesselId))
            break;
          System<LockSystem>.Singleton.AcquireControlLock(lockDefinition.VesselId);
          break;
      }
    }

    public void VesselUnloading(Vessel vessel)
    {
      if (!LockSystem.LockQuery.UpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      System<LockSystem>.Singleton.ReleaseUpdateLock(vessel.id);
    }

    public void FlightStarted()
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) || LockSystem.LockQuery.ControlLockExists(FlightGlobals.ActiveVessel.id))
        return;
      System<LockSystem>.Singleton.AcquireControlLock(FlightGlobals.ActiveVessel.id);
    }
  }
}
