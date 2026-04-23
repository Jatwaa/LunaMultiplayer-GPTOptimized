// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselActionGroupSys.VesselActionGroup
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.VesselUtilities;
using System;
using System.Globalization;
using UnityEngine;

namespace LmpClient.Systems.VesselActionGroupSys
{
  public class VesselActionGroup
  {
    public double GameTime;
    public Guid VesselId;
    public KSPActionGroup ActionGroup;
    public bool Value;

    public void ProcessActionGroup()
    {
      Vessel vessel = FlightGlobals.FindVessel(this.VesselId);
      if (Object.op_Equality((Object) vessel, (Object) null) || !VesselCommon.DoVesselChecks(this.VesselId) || this.ActionGroup == 16 && VesselCommon.IsSpectating && Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == vessel.id)
        return;
      if (vessel.ActionGroups != null && vessel.ActionGroups[this.ActionGroup] != this.Value)
        vessel.ActionGroups.ToggleGroup(this.ActionGroup);
      vessel.protoVessel?.actionGroups.SetValue(this.ActionGroup.ToString(), this.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", 0", false);
    }
  }
}
