// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselInterpolationDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.VesselPositionSys;
using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselInterpolationDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public int Amount { get; set; }

    public float Percentage { get; set; }

    public double Duration { get; set; }

    public double ExtraInterpolationTime { get; set; }

    public double TimeDifference { get; set; }

    public VesselInterpolationDisplay(Guid vesselId) => this.VesselId = vesselId;

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.Amount = 0;
      this.Percentage = 0.0f;
      this.Duration = 0.0;
      this.ExtraInterpolationTime = 0.0;
      this.TimeDifference = 0.0;
      VesselPositionUpdate vesselPositionUpdate;
      if (!VesselPositionSystem.CurrentVesselUpdate.TryGetValue(this.VesselId, out vesselPositionUpdate) || vesselPositionUpdate.Target == null)
        return;
      PositionUpdateQueue positionUpdateQueue;
      if (VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(this.VesselId, out positionUpdateQueue))
        this.Amount = positionUpdateQueue.Count;
      this.Percentage = vesselPositionUpdate.LerpPercentage * 100f;
      TimeSpan timeSpan = TimeSpan.FromSeconds(vesselPositionUpdate.InterpolationDuration);
      this.Duration = timeSpan.TotalMilliseconds;
      timeSpan = TimeSpan.FromSeconds(vesselPositionUpdate.ExtraInterpolationTime);
      this.ExtraInterpolationTime = timeSpan.TotalMilliseconds;
      timeSpan = TimeSpan.FromSeconds(vesselPositionUpdate.TimeDifference);
      this.TimeDifference = timeSpan.TotalMilliseconds;
    }

    protected override void PrintDisplay()
    {
      VesselBaseDisplay.StringBuilder.Length = 0;
      VesselBaseDisplay.StringBuilder.Append("Amt: ").AppendLine(this.Amount.ToString()).Append("Duration: ").AppendLine(string.Format("{0:F0}ms", (object) this.Duration)).Append("TimeDiff: ").AppendLine(string.Format("{0:F0}ms", (object) this.TimeDifference)).Append("ExtraInterpolationTime: ").AppendLine(string.Format("{0:F0}ms", (object) this.ExtraInterpolationTime)).Append("Percentage: ").Append(string.Format("{0:F0}%", (object) this.Percentage));
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
    }
  }
}
