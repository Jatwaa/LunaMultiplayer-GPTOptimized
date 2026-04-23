// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselDecoupleSys.VesselDecoupleEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.VesselPositionSys;
using LmpClient.VesselUtilities;
using UnityEngine;

namespace LmpClient.Systems.VesselDecoupleSys
{
  public class VesselDecoupleEvents : SubSystem<VesselDecoupleSystem>
  {
    public void DecoupleStart(Part part, float breakForce)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselDecoupleSystem>.System.IgnoreEvents || !Object.op_Implicit((Object) part) || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(part.vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      LunaLog.Log(string.Format("Detected decouple! Part: {0} Vessel: {1}", (object) part.partName, (object) part.vessel.id));
    }

    public void DecoupleComplete(Part part, float breakForce, Vessel originalVessel)
    {
      if (VesselCommon.IsSpectating || SubSystem<VesselDecoupleSystem>.System.IgnoreEvents || !Object.op_Implicit((Object) part) || !Object.op_Implicit((Object) originalVessel) || !LockSystem.LockQuery.UpdateLockBelongsToPlayer(originalVessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return;
      System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(part.vessel.id, true, true);
      System<LockSystem>.Singleton.AcquireUpdateLock(part.vessel.id, true, true);
      System<VesselPositionSystem>.Singleton.MessageSender.SendVesselPositionUpdate(part.vessel, true);
      LunaLog.Log(string.Format("Decouple complete! Part: {0} Vessel: {1}", (object) part.partName, (object) part.vessel.id));
      SubSystem<VesselDecoupleSystem>.System.MessageSender.SendVesselDecouple(originalVessel, part.flightID, breakForce, part.vessel.id);
    }
  }
}
