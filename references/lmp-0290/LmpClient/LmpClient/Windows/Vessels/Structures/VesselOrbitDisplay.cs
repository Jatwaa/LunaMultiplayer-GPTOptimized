// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselOrbitDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselOrbitDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public Vessel Vessel { get; set; }

    public OrbitDriver.UpdateMode ObtDriverMode { get; set; }

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.Vessel = vessel;
      this.ObtDriverMode = vessel.orbitDriver.updateMode;
    }

    public VesselOrbitDisplay(Guid vesselId) => this.VesselId = vesselId;

    protected override void PrintDisplay()
    {
      if (!Object.op_Implicit((Object) this.Vessel))
        return;
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      VesselBaseDisplay.StringBuilder.Length = 0;
      VesselBaseDisplay.StringBuilder.Append("Update mode: ").AppendLine(this.ObtDriverMode.ToString());
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      OrbitDriver.UpdateMode obtDriverMode = this.ObtDriverMode;
      if (obtDriverMode != null)
      {
        if (obtDriverMode == 1 && GUILayout.Button("Set as track phys", Array.Empty<GUILayoutOption>()))
          this.Vessel.orbitDriver.SetOrbitMode((OrbitDriver.UpdateMode) 0);
      }
      else if (GUILayout.Button("Set as update", Array.Empty<GUILayoutOption>()))
        this.Vessel.orbitDriver.SetOrbitMode((OrbitDriver.UpdateMode) 1);
      GUILayout.EndHorizontal();
      VesselBaseDisplay.StringBuilder.Length = 0;
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Semi major axis: {0}", (object) this.Vessel.orbit.semiMajorAxis)).AppendLine(string.Format("Eccentricity: {0}", (object) this.Vessel.orbit.eccentricity)).AppendLine(string.Format("Inclination: {0}", (object) this.Vessel.orbit.inclination)).AppendLine(string.Format("LAN: {0}", (object) this.Vessel.orbit.LAN)).AppendLine(string.Format("Arg Periapsis: {0}", (object) this.Vessel.orbit.argumentOfPeriapsis)).AppendLine(string.Format("Mean anomaly: {0}", (object) this.Vessel.orbit.meanAnomaly)).AppendLine(string.Format("Mean anomaly at Epoch: {0}", (object) this.Vessel.orbit.meanAnomalyAtEpoch)).AppendLine(string.Format("Epoch: {0}", (object) this.Vessel.orbit.epoch)).Append(string.Format("ObT: {0}", (object) this.Vessel.orbit.ObT));
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
    }
  }
}
