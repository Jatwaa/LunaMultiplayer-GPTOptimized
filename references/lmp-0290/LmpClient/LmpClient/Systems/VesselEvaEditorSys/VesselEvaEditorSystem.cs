// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselEvaEditorSys.VesselEvaEditorSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;

namespace LmpClient.Systems.VesselEvaEditorSys
{
  public class VesselEvaEditorSystem : System<VesselEvaEditorSystem>
  {
    public VesselEvaEditorEvents VesselEvaEditorEventsEvents { get; } = new VesselEvaEditorEvents();

    public bool DetachingPart { get; set; }

    public override string SystemName { get; } = nameof (VesselEvaEditorSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.onNewVesselCreated.Add(new EventData<Vessel>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(VesselCreated)));
      // ISSUE: method pointer
      EVAConstructionEvent.onAttachingPart.Add(new EventData<Part>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(OnAttachingPart)));
      // ISSUE: method pointer
      EVAConstructionEvent.onDroppingPart.Add(new EventVoid.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(OnDroppingPart)));
      // ISSUE: method pointer
      EVAConstructionEvent.onDroppedPart.Add(new EventVoid.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(OnDroppedPart)));
      // ISSUE: method pointer
      GameEvents.OnEVAConstructionModePartAttached.Add(new EventData<Vessel, Part>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(EVAConstructionModePartAttached)));
      // ISSUE: method pointer
      GameEvents.OnEVAConstructionModePartDetached.Add(new EventData<Vessel, Part>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(EVAConstructionModePartDetached)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onNewVesselCreated.Remove(new EventData<Vessel>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(VesselCreated)));
      // ISSUE: method pointer
      EVAConstructionEvent.onDroppingPart.Remove(new EventVoid.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(OnDroppingPart)));
      // ISSUE: method pointer
      EVAConstructionEvent.onDroppedPart.Remove(new EventVoid.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(OnDroppedPart)));
      // ISSUE: method pointer
      GameEvents.OnEVAConstructionModePartAttached.Remove(new EventData<Vessel, Part>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(EVAConstructionModePartAttached)));
      // ISSUE: method pointer
      GameEvents.OnEVAConstructionModePartDetached.Remove(new EventData<Vessel, Part>.OnEvent((object) this.VesselEvaEditorEventsEvents, __methodptr(EVAConstructionModePartDetached)));
      this.DetachingPart = false;
    }
  }
}
