// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselActionGroupSys.VesselActionGroupEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;

namespace LmpClient.Systems.VesselActionGroupSys
{
  public class VesselActionGroupEvents : SubSystem<VesselActionGroupSystem>
  {
    public void ActionGroupFired(Vessel vessel, KSPActionGroup actionGroup, bool value)
    {
      if (LockSystem.LockQuery.UpdateLockExists(vessel.id) && !LockSystem.LockQuery.UpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      SubSystem<VesselActionGroupSystem>.System.MessageSender.SendVesselActionGroup(FlightGlobals.ActiveVessel, actionGroup, value);
    }
  }
}
