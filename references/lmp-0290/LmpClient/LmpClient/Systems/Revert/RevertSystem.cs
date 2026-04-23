// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Revert.RevertSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using System;

namespace LmpClient.Systems.Revert
{
  public class RevertSystem : LmpClient.Base.System<RevertSystem>
  {
    public static RevertEvents RevertEvents { get; } = new RevertEvents();

    public Guid StartingVesselId { get; set; } = Guid.Empty;

    public override string SystemName { get; } = nameof (RevertSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      VesselAssemblyEvent.onAssembledVessel.Add(new EventData<Vessel, ShipConstruct>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(VesselAssembled)));
      // ISSUE: method pointer
      GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      RevertEvent.onRevertedToLaunch.Add(new EventVoid.OnEvent((object) RevertSystem.RevertEvents, __methodptr(OnRevertToLaunch)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Add(new EventData<GameScenes>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(GameSceneLoadRequested)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      VesselAssemblyEvent.onAssembledVessel.Remove(new EventData<Vessel, ShipConstruct>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(VesselAssembled)));
      // ISSUE: method pointer
      GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(OnVesselChange)));
      // ISSUE: method pointer
      RevertEvent.onRevertedToLaunch.Remove(new EventVoid.OnEvent((object) RevertSystem.RevertEvents, __methodptr(OnRevertToLaunch)));
      // ISSUE: method pointer
      GameEvents.onGameSceneLoadRequested.Remove(new EventData<GameScenes>.OnEvent((object) RevertSystem.RevertEvents, __methodptr(GameSceneLoadRequested)));
    }
  }
}
