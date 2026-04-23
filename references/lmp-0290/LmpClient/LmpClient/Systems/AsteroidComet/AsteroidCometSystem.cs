// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.AsteroidComet.AsteroidCometSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Extensions;
using LmpClient.Systems.Lock;
using LmpCommon.Locks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Systems.AsteroidComet
{
  public class AsteroidCometSystem : LmpClient.Base.System<AsteroidCometSystem>
  {
    public AsteroidCometEvents AsteroidCometEvents { get; } = new AsteroidCometEvents();

    public bool AsteroidSystemReady => this.Enabled && HighLogic.LoadedScene >= 7;

    public override string SystemName { get; } = nameof (AsteroidCometSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) this.AsteroidCometEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Add(new EventData<GameScenes>.OnEvent((object) this.AsteroidCometEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      TrackingEvent.onStartTrackingAsteroidOrComet.Add(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(StartTrackingCometOrAsteroid)));
      // ISSUE: method pointer
      TrackingEvent.onStopTrackingAsteroidOrComet.Add(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(StopTrackingCometOrAsteroid)));
      // ISSUE: method pointer
      GameEvents.onNewVesselCreated.Add(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(NewVesselCreated)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) this.AsteroidCometEvents, __methodptr(LockReleased)));
      // ISSUE: method pointer
      GameEvents.onLevelWasLoadedGUIReady.Remove(new EventData<GameScenes>.OnEvent((object) this.AsteroidCometEvents, __methodptr(LevelLoaded)));
      // ISSUE: method pointer
      TrackingEvent.onStartTrackingAsteroidOrComet.Remove(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(StartTrackingCometOrAsteroid)));
      // ISSUE: method pointer
      TrackingEvent.onStopTrackingAsteroidOrComet.Remove(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(StopTrackingCometOrAsteroid)));
      // ISSUE: method pointer
      GameEvents.onNewVesselCreated.Remove(new EventData<Vessel>.OnEvent((object) this.AsteroidCometEvents, __methodptr(NewVesselCreated)));
    }

    public void TryGetCometAsteroidLock()
    {
      if (LockSystem.LockQuery.AsteroidCometLockExists())
        return;
      LmpClient.Base.System<LockSystem>.Singleton.AcquireAsteroidLock();
    }

    public int GetAsteroidCount() => ((IEnumerable<Vessel>) FlightGlobals.Vessels).Count<Vessel>((Func<Vessel, bool>) (v => v.IsAsteroid()));

    public int GetCometCount() => ((IEnumerable<Vessel>) FlightGlobals.Vessels).Count<Vessel>((Func<Vessel, bool>) (v => v.IsComet()));
  }
}
