// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerColorSys.PlayerColorEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Locks;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.PlayerColorSys
{
  public class PlayerColorEvents : SubSystem<PlayerColorSystem>
  {
    public void OnVesselCreated(Vessel vessel) => SubSystem<PlayerColorSystem>.System.SetVesselOrbitColor(vessel);

    public void OnLockAcquire(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Control)
        return;
      PlayerColorEvents.UpdateVesselColorsFromLockVesselId(lockDefinition.VesselId);
    }

    public void OnLockRelease(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Control)
        return;
      PlayerColorEvents.UpdateVesselColorsFromLockVesselId(lockDefinition.VesselId);
    }

    public void MapEntered()
    {
      foreach (Vessel vessel in Enumerable.Where<Vessel>((IEnumerable<Vessel>) FlightGlobals.Vessels, (Func<Vessel, bool>) (v => Object.op_Inequality((Object) v, (Object) null) && Object.op_Implicit((Object) v.orbitDriver) && Object.op_Implicit((Object) v.orbitDriver.Renderer))))
        SubSystem<PlayerColorSystem>.System.SetVesselOrbitColor(vessel);
    }

    private static void UpdateVesselColorsFromLockVesselId(Guid vesselId) => SubSystem<PlayerColorSystem>.System.SetVesselOrbitColor(FlightGlobals.FindVessel(vesselId));

    public void VesselInitialized(Vessel vessel, bool fromShipAssembly) => SubSystem<PlayerColorSystem>.System.SetVesselOrbitColor(vessel);
  }
}
