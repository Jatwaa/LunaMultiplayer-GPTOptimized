// Decompiled with JetBrains decompiler
// Type: LmpClient.Events.LabelEvent
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using KSP.UI.Screens;
using KSP.UI.Screens.Mapview;
using LmpClient.Events.Base;

namespace LmpClient.Events
{
  public class LabelEvent : LmpBaseEvent
  {
    public static EventData<BaseLabel> onLabelProcessed;
    public static EventData<Vessel, MapNode.CaptionData> onMapLabelProcessed;
    public static EventData<TrackingStationWidget> onMapWidgetTextProcessed;
  }
}
