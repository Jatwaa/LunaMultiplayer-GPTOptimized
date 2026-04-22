using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using UnityEngine;

namespace LmpClient.Windows.ServerList
{
    public class ServerFilter
    {
        private static string _searchText = "";

        public static void DrawFilters()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ServerListFiltersText.Search, GUILayout.Width(50));
            _searchText = GUILayout.TextField(_searchText, GUILayout.Width(180));
            GUILayout.FlexibleSpace();
            var hideFullServers = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HideFullServers, LocalizationContainer.ServerListFiltersText.HideFullServers);
            if (hideFullServers != SettingsSystem.CurrentSettings.ServerFilters.HideFullServers)
            {
                SettingsSystem.CurrentSettings.ServerFilters.HideFullServers = hideFullServers;
                SettingsSystem.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            var hideEmptyServers = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers, LocalizationContainer.ServerListFiltersText.HideEmptyServers);
            if (hideEmptyServers != SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers)
            {
                SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers = hideEmptyServers;
                SettingsSystem.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            var hidePrivateServers = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers, LocalizationContainer.ServerListFiltersText.HidePrivateServers);
            if (hidePrivateServers != SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers)
            {
                SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers = hidePrivateServers;
                SettingsSystem.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            var dedicatedServersOnly = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly, LocalizationContainer.ServerListFiltersText.DedicatedServersOnly);
            if (dedicatedServersOnly != SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly)
            {
                SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly = dedicatedServersOnly;
                SettingsSystem.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            var favoritesOnly = GUILayout.Toggle(SettingsSystem.CurrentSettings.ServerFilters.FavoritesOnly, LocalizationContainer.ServerListFiltersText.FavoritesOnly);
            if (favoritesOnly != SettingsSystem.CurrentSettings.ServerFilters.FavoritesOnly)
            {
                SettingsSystem.CurrentSettings.ServerFilters.FavoritesOnly = favoritesOnly;
                SettingsSystem.SaveSettings();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static bool MatchesFilters(ServerInfo server)
        {
            if (SettingsSystem.CurrentSettings.ServerFilters.HidePrivateServers && server.Password)
                return false;

            if (SettingsSystem.CurrentSettings.ServerFilters.HideFullServers && server.PlayerCount == server.MaxPlayers)
                return false;

            if (SettingsSystem.CurrentSettings.ServerFilters.HideEmptyServers && server.PlayerCount == 0)
                return false;

            if (SettingsSystem.CurrentSettings.ServerFilters.DedicatedServersOnly && !server.DedicatedServer)
                return false;

            if (SettingsSystem.CurrentSettings.ServerFilters.FavoritesOnly && !ServerListWindow.IsFavorite(server))
                return false;

            if (!string.IsNullOrEmpty(_searchText))
            {
                var search = _searchText.ToLowerInvariant();
                var nameMatch = server.ServerName != null && server.ServerName.ToLowerInvariant().Contains(search);
                var descMatch = server.Description != null && server.Description.ToLowerInvariant().Contains(search);
                if (!nameMatch && !descMatch)
                    return false;
            }

            return true;
        }
    }
}
