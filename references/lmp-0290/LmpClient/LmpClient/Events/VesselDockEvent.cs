// Decompiled with JetBrains decompiler
// Type: LmpClient.Events.VesselDockEvent
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events.Base;

namespace LmpClient.Events
{
  public class VesselDockEvent : LmpBaseEvent
  {
    public static EventData<Vessel, Vessel> onDocking;
    public static EventData<Vessel, Vessel> onDockingComplete;
  }
}
