// Decompiled with JetBrains decompiler
// Type: LmpClient.VesselUtilities.VesselCommon
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Network;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.VesselCoupleSys;
using LmpClient.Systems.VesselDecoupleSys;
using LmpClient.Systems.VesselFairingsSys;
using LmpClient.Systems.VesselFlightStateSys;
using LmpClient.Systems.VesselPartSyncCallSys;
using LmpClient.Systems.VesselPartSyncFieldSys;
using LmpClient.Systems.VesselPartSyncUiFieldSys;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Systems.VesselResourceSys;
using LmpClient.Systems.VesselUndockSys;
using LmpCommon.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.VesselUtilities
{
  public class VesselCommon
  {
    private static bool _isSpectating;

    public static float PositionAndFlightStateMessageOffsetSec(float targetPingSec) => Mathf.Clamp(NetworkStatistics.PingSec + targetPingSec, 0.25f, 2.5f);

    public static bool UpdateIsForOwnVessel(Guid vesselId) => !VesselCommon.IsSpectating && Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == vesselId;

    public static bool IsSpectating
    {
      get => HighLogic.LoadedScene == 7 && Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null) && VesselCommon._isSpectating;
      set => VesselCommon._isSpectating = value;
    }

    public static IEnumerable<Guid> GetControlledVesselIds() => LockSystem.LockQuery.GetAllControlLocks().Select<LockDefinition, Guid>((Func<LockDefinition, Guid>) (v => v.VesselId));

    public static void RemoveVesselFromSystems(Guid vesselId)
    {
      LmpClient.Base.System<VesselPositionSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselFlightStateSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselResourceSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselProtoSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselPartSyncFieldSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselPartSyncUiFieldSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselPartSyncCallSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselFairingsSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselCoupleSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselDecoupleSystem>.Singleton.RemoveVessel(vesselId);
      LmpClient.Base.System<VesselUndockSystem>.Singleton.RemoveVessel(vesselId);
    }

    public static bool PlayerVesselsNearby()
    {
      if (!Object.op_Inequality((Object) FlightGlobals.ActiveVessel, (Object) null))
        return false;
      for (int index = 0; index < FlightGlobals.VesselsLoaded.Count; ++index)
      {
        if (Object.op_Inequality((Object) FlightGlobals.VesselsLoaded[index], (Object) FlightGlobals.ActiveVessel))
          return true;
      }
      return false;
    }

    public static bool DoVesselChecks(Guid vesselId) => !LmpClient.Base.System<VesselRemoveSystem>.Singleton.VesselWillBeKilled(vesselId) && !LockSystem.LockQuery.ControlLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName) && !LockSystem.LockQuery.UnloadedUpdateLockBelongsToPlayer(vesselId, SettingsSystem.CurrentSettings.PlayerName);

    public static IEnumerable<Vessel> GetSecondaryVessels() => LockSystem.LockQuery.GetAllUpdateLocks(SettingsSystem.CurrentSettings.PlayerName).Select<LockDefinition, Vessel>((Func<LockDefinition, Vessel>) (l => ((IEnumerable<Vessel>) FlightGlobals.VesselsLoaded).FirstOrDefault<Vessel>((Func<Vessel, bool>) (v => Object.op_Implicit((Object) v) && v.id == l.VesselId)))).Where<Vessel>((Func<Vessel, bool>) (v =>
    {
      if (!Object.op_Implicit((Object) v))
        return false;
      return Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null) || Object.op_Inequality((Object) v, (Object) FlightGlobals.ActiveVessel);
    }));

    public static IEnumerable<Vessel> GetUnloadedSecondaryVessels() => LockSystem.LockQuery.GetAllUnloadedUpdateLocks(SettingsSystem.CurrentSettings.PlayerName).Select<LockDefinition, Vessel>((Func<LockDefinition, Vessel>) (l => ((IEnumerable<Vessel>) FlightGlobals.VesselsUnloaded).FirstOrDefault<Vessel>((Func<Vessel, bool>) (v => Object.op_Implicit((Object) v) && v.id == l.VesselId)))).Where<Vessel>((Func<Vessel, bool>) (v =>
    {
      if (!Object.op_Implicit((Object) v))
        return false;
      return Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null) || Object.op_Inequality((Object) v, (Object) FlightGlobals.ActiveVessel);
    }));
  }
}
