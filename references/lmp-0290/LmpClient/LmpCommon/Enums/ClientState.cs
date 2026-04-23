// Decompiled with JetBrains decompiler
// Type: LmpCommon.Enums.ClientState
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpCommon.Enums
{
  public enum ClientState
  {
    DisconnectRequested = -1, // 0xFFFFFFFF
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
    Handshaking = 3,
    Handshaked = 6,
    SyncingSettings = 7,
    SettingsSynced = 8,
    SyncingKerbals = 9,
    KerbalsSynced = 10, // 0x0000000A
    SyncingWarpsubspaces = 11, // 0x0000000B
    WarpsubspacesSynced = 12, // 0x0000000C
    SyncingColors = 13, // 0x0000000D
    ColorsSynced = 14, // 0x0000000E
    SyncingFlags = 16, // 0x00000010
    FlagsSynced = 17, // 0x00000011
    SyncingPlayers = 18, // 0x00000012
    PlayersSynced = 19, // 0x00000013
    SyncingScenarios = 20, // 0x00000014
    ScenariosSynced = 21, // 0x00000015
    SyncingLocks = 24, // 0x00000018
    LocksSynced = 25, // 0x00000019
    Starting = 35, // 0x00000023
    Running = 36, // 0x00000024
  }
}
