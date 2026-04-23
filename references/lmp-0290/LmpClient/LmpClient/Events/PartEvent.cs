// Decompiled with JetBrains decompiler
// Type: LmpClient.Events.PartEvent
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events.Base;
using System;

namespace LmpClient.Events
{
  public class PartEvent : LmpBaseEvent
  {
    public static EventData<Part, float> onPartDecoupling;
    public static EventData<Part, float, Vessel> onPartDecoupled;
    public static EventData<Part, DockedVesselInfo> onPartUndocking;
    public static EventData<Part, DockedVesselInfo, Vessel> onPartUndocked;
    public static EventData<Part, Part> onPartCoupling;
    public static EventData<Part, Part, Guid> onPartCoupled;
  }
}
