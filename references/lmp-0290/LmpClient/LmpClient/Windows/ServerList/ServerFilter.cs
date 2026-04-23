// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.ServerList.ServerFilter
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using System;
using UnityEngine;

namespace LmpClient.Windows.ServerList
{
  public class ServerFilter
  {
    public static void DrawFilters()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      bool flag1 = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HideFullServers, LocalizationContainer.ServerListFiltersText.HideFullServers, Array.Empty<GUILayoutOption>());
      if (flag1 != SettingsSystem.CurrentSettings.ServerFilters.HideFullServers)
      {
        SettingsSystem.CurrentSettings.ServerFilters.HideFullServers = flag1;
        SettingsSystem.SaveSettings();
      }
      GUILayout.FlexibleSpace();
      bool flag2 = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers, LocalizationContainer.ServerListFiltersText.HideEmptyServers, Array.Empty<GUILayoutOption>());
      if (flag2 != SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers)
      {
        SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers = flag2;
        SettingsSystem.SaveSettings();
      }
      GUILayout.FlexibleSpace();
      bool flag3 = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers, LocalizationContainer.ServerListFiltersText.HidePrivateServers, Array.Empty<GUILayoutOption>());
      if (flag3 != SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers)
      {
        SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers = flag3;
        SettingsSystem.SaveSettings();
      }
      GUILayout.FlexibleSpace();
      bool flag4 = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly, LocalizationContainer.ServerListFiltersText.DedicatedServersOnly, Array.Empty<GUILayoutOption>());
      if (flag4 != SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly)
      {
        SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly = flag4;
        SettingsSystem.SaveSettings();
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
    }

    public static bool MatchesFilters(ServerInfo server) => (!SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers || !server.Password) && (!SettingsSystem.CurrentSettings.ServerFilters.HideFullServers || server.PlayerCount != server.MaxPlayers) && (!SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers || server.PlayerCount != 0) && (!SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly || server.DedicatedServer);
  }
}
