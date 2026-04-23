// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselPositionDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.SafetyBubble;
using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselPositionDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public Vessel Vessel { get; set; }

    public VesselPositionDisplay(Guid vesselId) => this.VesselId = vesselId;

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.Vessel = vessel;
    }

    protected override void PrintDisplay()
    {
      if (!Object.op_Implicit((Object) this.Vessel))
        return;
      VesselBaseDisplay.StringBuilder.Length = 0;
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Situation: {0}", (object) this.Vessel.situation)).AppendLine(string.Format("Orbit Pos: {0}", (object) this.Vessel.orbit.pos)).AppendLine(string.Format("Transform Pos: {0}", (object) this.Vessel.vesselTransform.position)).AppendLine(string.Format("Com Pos: {0}", (object) this.Vessel.CoM)).AppendLine(string.Format("ComD Pos: {0}", (object) this.Vessel.CoMD)).AppendLine(string.Format("Lat,Lon,Alt: {0},{1},{2}", (object) this.Vessel.latitude, (object) this.Vessel.longitude, (object) this.Vessel.altitude));
      double num1;
      double num2;
      double num3;
      this.Vessel.mainBody.GetLatLonAlt(Vector3d.op_Implicit(this.Vessel.vesselTransform.position), ref num1, ref num2, ref num3);
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Current Lat,Lon,Alt: {0},{1},{2}", (object) num1, (object) num2, (object) num3));
      this.Vessel.mainBody.GetLatLonAltOrbital(this.Vessel.orbit.pos, ref num1, ref num2, ref num3);
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Orbital Lat,Lon,Alt: {0},{1},{2}", (object) num1, (object) num2, (object) num3));
      VesselBaseDisplay.StringBuilder.Append(string.Format("Inside safety bubble: {0}", (object) LmpClient.Base.System<SafetyBubbleSystem>.Singleton.IsInSafetyBubble(this.Vessel)));
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
    }
  }
}
