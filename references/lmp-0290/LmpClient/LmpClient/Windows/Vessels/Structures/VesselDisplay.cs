// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public string VesselName { get; set; }

    public VesselDataDisplay Data { get; set; }

    public VesselLockDisplay Locks { get; set; }

    public VesselOrbitDisplay Orbit { get; set; }

    public VesselInterpolationDisplay Interpolation { get; set; }

    public VesselPositionDisplay Position { get; set; }

    public VesselVectorsDisplay Vectors { get; set; }

    public VesselDisplay(Guid vesselId)
    {
      this.VesselId = vesselId;
      this.Data = new VesselDataDisplay(this.VesselId);
      this.Locks = new VesselLockDisplay(this.VesselId);
      this.Orbit = new VesselOrbitDisplay(this.VesselId);
      this.Interpolation = new VesselInterpolationDisplay(this.VesselId);
      this.Position = new VesselPositionDisplay(this.VesselId);
      this.Vectors = new VesselVectorsDisplay(this.VesselId);
    }

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.VesselName = vessel.vesselName;
      this.Data.Update(vessel);
      this.Locks.Update(vessel);
      this.Orbit.Update(vessel);
      this.Interpolation.Update(vessel);
      this.Position.Update(vessel);
      this.Vectors.Update(vessel);
    }

    protected override void PrintDisplay()
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(this.VesselName, Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Reload", Array.Empty<GUILayoutOption>()))
        VesselLoader.LoadVessel(FlightGlobals.FindVessel(this.VesselId).protoVessel, true);
      GUILayout.EndHorizontal();
      this.Data.Display = GUILayout.Toggle(this.Data.Display, "Data", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Data.Print();
      this.Locks.Display = GUILayout.Toggle(this.Locks.Display, "Locks", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Locks.Print();
      this.Orbit.Display = GUILayout.Toggle(this.Orbit.Display, "Orbit", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Orbit.Print();
      this.Interpolation.Display = GUILayout.Toggle(this.Interpolation.Display, "Interpolation", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Interpolation.Print();
      this.Position.Display = GUILayout.Toggle(this.Position.Display, "Position", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Position.Print();
      this.Vectors.Display = GUILayout.Toggle(this.Vectors.Display, "Vectors", VesselBaseDisplay.ButtonStyle, Array.Empty<GUILayoutOption>());
      this.Vectors.Print();
      GUILayout.EndVertical();
    }
  }
}
