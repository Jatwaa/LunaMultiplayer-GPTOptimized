// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.LunaComputerTime
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpCommon.Time
{
  public class LunaComputerTime
  {
    public static DateTime Now => LunaComputerTime.UtcNow.ToLocalTime();

    public static float SimulatedMinutesTimeOffset { get; set; }

    public static DateTime UtcNow => DateTime.UtcNow + TimeSpan.FromMinutes((double) LunaComputerTime.SimulatedMinutesTimeOffset);
  }
}
