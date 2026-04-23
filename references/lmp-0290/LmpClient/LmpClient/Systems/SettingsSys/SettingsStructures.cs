// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.SettingStructure
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.PlayerColorSys;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.SettingsSys
{
  [Serializable]
  public class SettingStructure
  {
    public string Language { get; set; } = "English";

    public string PlayerName { get; set; } = "Player";

    public int ConnectionTries { get; set; } = 3;

    public int InitialConnectionMsTimeout { get; set; } = 5000;

    public int SendReceiveMsInterval { get; set; } = 3;

    public int MsBetweenConnectionTries { get; set; } = 3000;

    public int HearbeatMsInterval { get; set; } = 2000;

    public bool DisclaimerAccepted { get; set; } = false;

    public Color PlayerColor { get; set; } = PlayerColorSystem.GenerateRandomColor();

    public string SelectedFlag { get; set; } = "Squad/Flags/default";

    public List<ServerEntry> Servers { get; set; } = new List<ServerEntry>();

    public int InitialConnectionSyncTimeRequests { get; set; } = 10;

    public bool RevertEnabled { get; set; }

    public int MaxGroupsPerPlayer { get; set; } = 1;

    public bool IgnoreSyncChecks { get; set; } = false;

    public int Mtu { get; set; } = 1408;

    public int ChatBuffer { get; set; } = 30;

    public bool AutoExpandMtu { get; set; } = false;

    public float TimeoutSeconds { get; set; } = 15f;

    public ServerFilters ServerFilters { get; set; } = new ServerFilters();

    public string CustomMasterServer { get; set; } = "";

    public bool Debug1 { get; set; } = false;

    public bool Debug2 { get; set; } = false;

    public bool Debug3 { get; set; } = false;

    public bool Debug4 { get; set; } = false;

    public bool Debug5 { get; set; } = false;

    public bool Debug6 { get; set; } = false;

    public bool Debug7 { get; set; } = false;

    public bool Debug8 { get; set; } = false;

    public bool Debug9 { get; set; } = false;
  }
}
