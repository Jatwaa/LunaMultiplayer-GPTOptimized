// Decompiled with JetBrains decompiler
// Type: LmpCommon.Enums.ClientMessageType
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpCommon.Enums
{
  public enum ClientMessageType
  {
    Handshake = 0,
    Settings = 1,
    Chat = 3,
    PlayerStatus = 4,
    PlayerColor = 5,
    Scenario = 6,
    Kerbal = 7,
    Vessel = 8,
    CraftLibrary = 9,
    Flag = 10, // 0x0000000A
    Motd = 11, // 0x0000000B
    Warp = 12, // 0x0000000C
    Lock = 13, // 0x0000000D
    Mod = 14, // 0x0000000E
    Admin = 15, // 0x0000000F
    Groups = 16, // 0x00000010
    Facility = 17, // 0x00000011
    Screenshot = 18, // 0x00000012
    ShareProgress = 19, // 0x00000013
  }
}
