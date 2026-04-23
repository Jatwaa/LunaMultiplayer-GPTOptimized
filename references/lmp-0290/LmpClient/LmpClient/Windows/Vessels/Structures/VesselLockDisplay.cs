// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselLockDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselLockDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public string ControlLockOwner { get; set; }

    public string UpdateLockOwner { get; set; }

    public string UnloadedUpdateLockOwner { get; set; }

    public VesselLockDisplay(Guid vesselId) => this.VesselId = vesselId;

    public bool PlayerOwnsAnyLock() => this.ControlLockOwner == SettingsSystem.CurrentSettings.PlayerName || this.UpdateLockOwner == SettingsSystem.CurrentSettings.PlayerName || this.UnloadedUpdateLockOwner == SettingsSystem.CurrentSettings.PlayerName;

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.ControlLockOwner = LockSystem.LockQuery.GetControlLockOwner(this.VesselId);
      this.UpdateLockOwner = LockSystem.LockQuery.GetUpdateLockOwner(this.VesselId);
      this.UnloadedUpdateLockOwner = LockSystem.LockQuery.GetUnloadedUpdateLockOwner(this.VesselId);
    }

    protected override void PrintDisplay()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      VesselBaseDisplay.StringBuilder.Length = 0;
      VesselBaseDisplay.StringBuilder.Append("Control: ").AppendLine(this.ControlLockOwner).Append("Update: ").AppendLine(this.UpdateLockOwner).Append("UnlUpdate: ").Append(this.UnloadedUpdateLockOwner);
      GUILayout.Label(VesselBaseDisplay.StringBuilder.ToString(), Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (this.PlayerOwnsAnyLock() && GUILayout.Button("Release", Array.Empty<GUILayoutOption>()))
      {
        Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
        LmpClient.Base.System<LockSystem>.Singleton.ReleaseAllVesselLocks(Object.op_Implicit((Object) vessel) ? (IEnumerable<string>) Enumerable.ToArray<string>(Enumerable.Select<ProtoCrewMember, string>((IEnumerable<ProtoCrewMember>) vessel.GetVesselCrew(), (Func<ProtoCrewMember, string>) (c => c.name))) : (IEnumerable<string>) (string[]) null, this.VesselId);
      }
      GUILayout.EndHorizontal();
    }
  }
}
