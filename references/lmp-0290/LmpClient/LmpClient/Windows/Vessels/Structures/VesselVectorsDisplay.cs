// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselVectorsDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Text;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselVectorsDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public Vessel Vessel { get; set; }

    public VesselVectorsDisplay(Guid vesselId) => this.VesselId = vesselId;

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
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Forward vector: {0}", (object) this.Vessel.GetFwdVector()));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Up vector: {0}", (object) this.Vessel.upAxis));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Srf Rotation: {0}", (object) this.Vessel.srfRelRotation));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Vessel Rotation: {0}", (object) ((Component) this.Vessel).transform.rotation));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("Vessel Local Rotation: {0}", (object) ((Component) this.Vessel).transform.localRotation));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("mainBody Rotation: {0}", (object) this.Vessel.mainBody.rotation));
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("mainBody Transform Rotation: {0}", (object) this.Vessel.mainBody.bodyTransform.rotation));
      StringBuilder stringBuilder1 = VesselBaseDisplay.StringBuilder;
      Vector3 srfVelocity = (object) this.Vessel.GetSrfVelocity();
      Vector3 vector3 = this.Vessel.GetSrfVelocity();
      // ISSUE: variable of a boxed type
      __Boxed<float> magnitude1 = (ValueType) ((Vector3) ref vector3).magnitude;
      string str1 = string.Format("Surface Velocity: {0}, |v|: {1}", (object) srfVelocity, (object) magnitude1);
      stringBuilder1.AppendLine(str1);
      StringBuilder stringBuilder2 = VesselBaseDisplay.StringBuilder;
      Vector3 obtVelocity = (object) this.Vessel.GetObtVelocity();
      vector3 = this.Vessel.GetObtVelocity();
      // ISSUE: variable of a boxed type
      __Boxed<float> magnitude2 = (ValueType) ((Vector3) ref vector3).magnitude;
      string str2 = string.Format("Orbital Velocity: {0}, |v|: {1}", (object) obtVelocity, (object) magnitude2);
      stringBuilder2.AppendLine(str2);
      if (Object.op_Inequality((Object) this.Vessel.orbitDriver, (Object) null) && this.Vessel.orbitDriver.orbit != null)
      {
        StringBuilder stringBuilder3 = VesselBaseDisplay.StringBuilder;
        Vector3d frameVel1 = (object) this.Vessel.orbitDriver.orbit.GetFrameVel();
        Vector3d frameVel2 = this.Vessel.orbitDriver.orbit.GetFrameVel();
        // ISSUE: variable of a boxed type
        __Boxed<double> magnitude3 = (ValueType) ((Vector3d) ref frameVel2).magnitude;
        string str3 = string.Format("Frame Velocity: {0}, |v|: {1}", (object) frameVel1, (object) magnitude3);
        stringBuilder3.AppendLine(str3);
      }
      VesselBaseDisplay.StringBuilder.AppendLine(string.Format("CoM offset vector: {0}", (object) this.Vessel.CoM));
      VesselBaseDisplay.StringBuilder.Append(string.Format("Angular Velocity: {0}, |v|: {1}", (object) this.Vessel.angularVelocity, (object) ((Vector3) ref this.Vessel.angularVelocity).magnitude));
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
    }
  }
}
