// Decompiled with JetBrains decompiler
// Type: LmpClient.Events.ExperimentalPartEvent
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events.Base;

namespace LmpClient.Events
{
  public class ExperimentalPartEvent : LmpBaseEvent
  {
    public static EventData<AvailablePart, int> onExperimentalPartAdded;
    public static EventData<AvailablePart, int> onExperimentalPartRemoved;
  }
}
