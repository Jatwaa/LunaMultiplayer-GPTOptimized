// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUndockSys.VesselUndockEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.VesselUtilities;

namespace LmpClient.Systems.VesselUndockSys
{
  public class VesselUndockEvents : SubSystem<VesselUndockSystem>
  {
    public void UndockStart(Part part, DockedVesselInfo dockedInfo)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselUndockSystem>.System.IgnoreEvents || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(part.vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      LunaLog.Log(string.Format("Detected undock! Part: {0} Vessel: {1}", (object) part.partName, (object) part.vessel.id));
    }

    public void UndockComplete(Part part, DockedVesselInfo dockedInfo, Vessel originalVessel)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselUndockSystem>.System.IgnoreEvents || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(originalVessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(part.vessel.id, true, true);
      System<LockSystem>.Singleton.AcquireUpdateLock(part.vessel.id, true, true);
      System<VesselPositionSystem>.Singleton.MessageSender.SendVesselPositionUpdate(part.vessel, true);
      LunaLog.Log(string.Format("Undock complete! Part: {0} Vessel: {1}", (object) part, (object) originalVessel.id));
      SubSystem<VesselUndockSystem>.System.MessageSender.SendVesselUndock(originalVessel, part.flightID, dockedInfo, part.vessel.id);
    }
  }
}
