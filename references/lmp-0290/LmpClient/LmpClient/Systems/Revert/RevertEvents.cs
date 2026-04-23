// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Revert.RevertEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using System;
using UnityEngine;

namespace LmpClient.Systems.Revert
{
  public class RevertEvents : SubSystem<RevertSystem>
  {
    private static bool _revertingToLaunch;

    public void OnVesselChange(Vessel data)
    {
      if (RevertEvents._revertingToLaunch)
        RevertEvents._revertingToLaunch = false;
      else
        SubSystem<RevertSystem>.System.StartingVesselId = Guid.Empty;
    }

    public void VesselAssembled(Vessel vessel, ShipConstruct construct) => SubSystem<RevertSystem>.System.StartingVesselId = vessel.id;

    public void OnRevertToLaunch()
    {
      RevertEvents._revertingToLaunch = true;
      if (!Object.op_Implicit((Object) FlightGlobals.ActiveVessel))
        return;
      SubSystem<RevertSystem>.System.StartingVesselId = FlightGlobals.ActiveVessel.id;
    }

    public void GameSceneLoadRequested(GameScenes data)
    {
      if (data == 7 || !RevertEvents._revertingToLaunch)
        return;
      RevertEvents._revertingToLaunch = false;
    }
  }
}
