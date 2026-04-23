// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.ServerFilters
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpClient.Systems.SettingsSys
{
  [Serializable]
  public class ServerFilters
  {
    public bool HidePrivateServers { get; set; } = false;

    public bool HideFullServers { get; set; } = true;

    public bool HideEmptyServers { get; set; } = false;

    public bool DedicatedServersOnly { get; set; } = false;
  }
}
