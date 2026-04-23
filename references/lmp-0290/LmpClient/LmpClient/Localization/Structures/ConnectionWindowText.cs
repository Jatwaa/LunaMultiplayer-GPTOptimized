// Decompiled with JetBrains decompiler
// Type: LmpClient.Localization.Structures.ConnectionWindowText
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Localization.Structures
{
  public class ConnectionWindowText
  {
    public string PlayerName { get; set; } = "Player Name:";

    public string Name { get; set; } = "Name:";

    public string Address { get; set; } = "Address:";

    public string Port { get; set; } = "Port:";

    public string Password { get; set; } = "Password:";

    public string CustomServers { get; set; } = "Custom servers:";

    public string Connect { get; set; } = nameof (Connect);

    public string Settings { get; set; } = nameof (Settings);

    public string Servers { get; set; } = "Server List";
  }
}
