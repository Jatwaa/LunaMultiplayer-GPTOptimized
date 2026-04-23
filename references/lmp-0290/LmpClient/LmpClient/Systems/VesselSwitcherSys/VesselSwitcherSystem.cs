// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselSwitcherSys.VesselSwitcherSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections;
using UnityEngine;

namespace LmpClient.Systems.VesselSwitcherSys
{
  public class VesselSwitcherSystem : LmpClient.Base.System<VesselSwitcherSystem>
  {
    private static Vessel VesselToSwitchTo { get; set; }

    public override string SystemName { get; } = nameof (VesselSwitcherSystem);

    protected override void OnDisabled()
    {
      base.OnDisabled();
      VesselSwitcherSystem.VesselToSwitchTo = (Vessel) null;
    }

    public void SwitchToVessel(Vessel vessel)
    {
      if (!Object.op_Inequality((Object) vessel, (Object) null))
        return;
      VesselSwitcherSystem.VesselToSwitchTo = vessel;
      LunaLog.Log("[LMP]: Switching to vessel " + vessel.vesselName);
      MainSystem.Singleton.StartCoroutine(VesselSwitcherSystem.SwitchToVessel());
    }

    private static IEnumerator SwitchToVessel()
    {
      if (Object.op_Inequality((Object) VesselSwitcherSystem.VesselToSwitchTo, (Object) null))
      {
        int tries = 0;
        float zoom = FlightCamera.fetch.Distance;
        OrbitPhysicsManager.HoldVesselUnpack(1);
        while (!VesselSwitcherSystem.VesselToSwitchTo.loaded && tries < 100)
        {
          ++tries;
          yield return (object) new WaitForFixedUpdate();
        }
        LunaLog.Log(string.Format("Tries: {0} Loaded: {1}", (object) tries, (object) VesselSwitcherSystem.VesselToSwitchTo.loaded));
        if (!VesselSwitcherSystem.VesselToSwitchTo.loaded)
        {
          tries = 0;
          while (!VesselSwitcherSystem.VesselToSwitchTo.loaded && tries < 10)
          {
            ++tries;
            yield return (object) new WaitForSeconds(0.1f);
          }
        }
        if (!VesselSwitcherSystem.VesselToSwitchTo.loaded)
          VesselSwitcherSystem.VesselToSwitchTo.Load();
        FlightGlobals.ForceSetActiveVessel(VesselSwitcherSystem.VesselToSwitchTo);
        FlightCamera.fetch.SetDistance(zoom);
        VesselSwitcherSystem.VesselToSwitchTo = (Vessel) null;
      }
    }
  }
}
