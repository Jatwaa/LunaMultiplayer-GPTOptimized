// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselImmortalSys.VesselImmortalEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Locks;

namespace LmpClient.Systems.VesselImmortalSys
{
  public class VesselImmortalEvents : SubSystem<VesselImmortalSystem>
  {
    public void OnVesselChange(Vessel vessel) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);

    public void PartCountChanged(Vessel vessel) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);

    public void VesselGoOnRails(Vessel vessel) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);

    public void VesselGoOffRails(Vessel vessel) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);

    public void OnLockAcquire(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Control && lockDefinition.Type != LockType.Update && lockDefinition.Type != LockType.UnloadedUpdate)
        return;
      SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(FlightGlobals.FindVessel(lockDefinition.VesselId));
    }

    public void OnLockRelease(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.Control && lockDefinition.Type != LockType.Update && lockDefinition.Type != LockType.UnloadedUpdate)
        return;
      SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(FlightGlobals.FindVessel(lockDefinition.VesselId));
    }

    public void FinishSpectating() => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(FlightGlobals.ActiveVessel);

    public void StartSpectating() => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(FlightGlobals.ActiveVessel);

    public void VesselInitialized(Vessel vessel, bool fromShipAssembly) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);

    public void OnVesselCreated(Vessel vessel) => SubSystem<VesselImmortalSystem>.System.SetImmortalStateBasedOnLock(vessel);
  }
}
