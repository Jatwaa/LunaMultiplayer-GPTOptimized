// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselCrewSys.VesselCrewSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using System;

namespace LmpClient.Systems.VesselCrewSys
{
  public class VesselCrewSystem : LmpClient.Base.System<VesselCrewSystem>
  {
    private VesselCrewEvents VesselCrewEvents { get; } = new VesselCrewEvents();

    public override string SystemName { get; } = nameof (VesselCrewSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onCrewOnEva.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewEva)));
      // ISSUE: method pointer
      GameEvents.onVesselCrewWasModified.Add(new EventData<Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewModified)));
      // ISSUE: method pointer
      EvaEvent.onCrewEvaReady.Add(new EventData<Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(CrewEvaReady)));
      // ISSUE: method pointer
      EvaEvent.onCrewEvaBoarded.Add(new EventData<Guid, string, Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewBoard)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onCrewOnEva.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewEva)));
      // ISSUE: method pointer
      GameEvents.onVesselCrewWasModified.Remove(new EventData<Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewModified)));
      // ISSUE: method pointer
      EvaEvent.onCrewEvaReady.Remove(new EventData<Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(CrewEvaReady)));
      // ISSUE: method pointer
      EvaEvent.onCrewEvaBoarded.Remove(new EventData<Guid, string, Vessel>.OnEvent((object) this.VesselCrewEvents, __methodptr(OnCrewBoard)));
    }
  }
}
