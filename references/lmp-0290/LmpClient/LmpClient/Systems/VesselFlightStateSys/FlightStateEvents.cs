// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselFlightStateSys.FlightStateEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.VesselFlightStateSys
{
  public class FlightStateEvents : SubSystem<VesselFlightStateSystem>
  {
    public void OnVesselPack(Vessel vessel) => SubSystem<VesselFlightStateSystem>.System.RemoveVessel(vessel);

    public void OnVesselUnpack(Vessel vessel) => SubSystem<VesselFlightStateSystem>.System.AddVesselToSystem(vessel);

    public void OnStartSpectating() => SubSystem<VesselFlightStateSystem>.System.AddVesselToSystem(FlightGlobals.ActiveVessel);

    public void OnFinishedSpectating() => SubSystem<VesselFlightStateSystem>.System.RemoveVessel(FlightGlobals.ActiveVessel);

    public void WarpStopped() => SubSystem<VesselFlightStateSystem>.System.AdjustExtraInterpolationTimes();
  }
}
