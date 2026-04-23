// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Label.LabelSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using LmpClient.Base;
using LmpClient.Events;

namespace LmpClient.Systems.Label
{
  public class LabelSystem : System<LabelSystem>
  {
    private static LabelEvents LabelEvents { get; } = new LabelEvents();

    public override string SystemName { get; } = nameof (LabelSystem);

    protected override void OnEnabled()
    {
      // ISSUE: method pointer
      LabelEvent.onLabelProcessed.Add(new EventData<BaseLabel>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnLabelProcessed)));
      // ISSUE: method pointer
      LabelEvent.onMapLabelProcessed.Add(new EventData<Vessel, MapNode.CaptionData>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnMapLabelProcessed)));
      // ISSUE: method pointer
      LabelEvent.onMapWidgetTextProcessed.Add(new EventData<TrackingStationWidget>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnMapWidgetTextProcessed)));
    }

    protected override void OnDisabled()
    {
      // ISSUE: method pointer
      LabelEvent.onLabelProcessed.Remove(new EventData<BaseLabel>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnLabelProcessed)));
      // ISSUE: method pointer
      LabelEvent.onMapLabelProcessed.Remove(new EventData<Vessel, MapNode.CaptionData>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnMapLabelProcessed)));
      // ISSUE: method pointer
      LabelEvent.onMapWidgetTextProcessed.Remove(new EventData<TrackingStationWidget>.OnEvent((object) LabelSystem.LabelEvents, __methodptr(OnMapWidgetTextProcessed)));
    }
  }
}
