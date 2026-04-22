using LmpClient.Localization;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpClient.Windows.Options;
using LmpClient.Windows.ServerList;
using LmpCommon;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Windows.Connection
{
    public partial class ConnectionWindow
    {
        private static Vector2 _favScrollPos;

        protected override void DrawWindowContent(int windowId)
        {
            GUI.DragWindow(MoveRect);

            GUILayout.BeginVertical();
            DrawPlayerNameSection();
            DrawTopButtons();
            DrawFavoriteServers();
            DrawCustomServers();

            GUILayout.Label(MainSystem.Singleton.Status, StatusStyle);
            GUILayout.EndVertical();
        }

        private static void DrawFavoriteServers()
        {
            var favorites = SettingsSystem.CurrentSettings.FavoriteServers;
            if (favorites.Count == 0) return;

            GUILayout.Label(LocalizationContainer.ConnectionWindowText.FavoriteServers);
            _favScrollPos = GUILayout.BeginScrollView(_favScrollPos, GUILayout.Width(WindowWidth - 5), GUILayout.MaxHeight(120));

            foreach (var fav in favorites)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(fav.Name, GUILayout.ExpandWidth(true));
                if (MainSystem.NetworkState <= ClientState.Disconnected)
                {
                    if (GUILayout.Button(LocalizationContainer.ConnectionWindowText.Connect, GUILayout.Width(70)))
                    {
                        NetworkConnection.ConnectToServer(fav.Address, fav.Port, string.Empty);
                    }
                }
                if (GUILayout.Button("✕", GUILayout.Width(25)))
                {
                    SettingsSystem.CurrentSettings.FavoriteServers.Remove(fav);
                    SettingsSystem.SaveSettings();
                    GUILayout.EndHorizontal();
                    break;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void DrawCustomServers()
        {
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.CustomServers);
            GUILayout.BeginVertical();

            ScrollPos = GUILayout.BeginScrollView(ScrollPos, GUILayout.Width(WindowWidth - 5),
                GUILayout.Height(WindowHeight - 100));

            if (GUILayout.Button(PlusIcon))
            {
                SettingsSystem.CurrentSettings.Servers.Insert(0, new ServerEntry());
                SettingsSystem.SaveSettings();
            }

            for (var serverPos = 0; serverPos < SettingsSystem.CurrentSettings.Servers.Count; serverPos++)
            {
                GUILayout.BeginHorizontal();
                var selected = GUILayout.Toggle(SelectedIndex == serverPos,
                    SettingsSystem.CurrentSettings.Servers[serverPos].Name, ToggleButtonStyle);
                if (GUILayout.Button(DeleteIcon, GUILayout.Width(35)))
                {
                    SettingsSystem.CurrentSettings.Servers.RemoveAt(SelectedIndex);
                    SettingsSystem.SaveSettings();
                }
                else
                {
                    GUILayout.EndHorizontal();
                    if (selected)
                    {
                        DrawServerEntry(serverPos);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawServerEntry(int serverPos)
        {
            SelectedIndex = serverPos;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.Name, LabelOptions);
            var newServerName = GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Name);
            if (newServerName != SettingsSystem.CurrentSettings.Servers[serverPos].Name)
            {
                SettingsSystem.CurrentSettings.Servers[serverPos].Name = newServerName;
                SettingsSystem.SaveSettings();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.Address, LabelOptions);
            var newAddress = GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Address);
            if (newAddress != SettingsSystem.CurrentSettings.Servers[serverPos].Address)
            {
                SettingsSystem.CurrentSettings.Servers[serverPos].Address = newAddress;
                SettingsSystem.SaveSettings();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.Port, LabelOptions);
            var port = GUILayout.TextArea(SettingsSystem.CurrentSettings.Servers[serverPos].Port.ToString());
            if (int.TryParse(port, out var parsedPort))
            {
                if (parsedPort != SettingsSystem.CurrentSettings.Servers[serverPos].Port)
                {
                    SettingsSystem.CurrentSettings.Servers[serverPos].Port = parsedPort;
                    SettingsSystem.SaveSettings();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.Password, LabelOptions);
            var newPassword = GUILayout.PasswordField(SettingsSystem.CurrentSettings.Servers[serverPos].Password, '*', 30);
            if (newPassword != SettingsSystem.CurrentSettings.Servers[serverPos].Password)
            {
                SettingsSystem.CurrentSettings.Servers[serverPos].Password = newPassword;
                SettingsSystem.SaveSettings();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void DrawTopButtons()
        {
            GUILayout.BeginHorizontal();
            if (MainSystem.NetworkState <= ClientState.Disconnected)
            {
                GUI.enabled = SettingsSystem.CurrentSettings.Servers.Count > SelectedIndex && SelectedIndex >= 0;
                if (GUILayout.Button(ConnectBigIcon))
                {
                    NetworkConnection.ConnectToServer(
                        SettingsSystem.CurrentSettings.Servers[SelectedIndex].Address,
                        SettingsSystem.CurrentSettings.Servers[SelectedIndex].Port,
                        SettingsSystem.CurrentSettings.Servers[SelectedIndex].Password);
                }
            }
            else
            {
                if (GUILayout.Button(DisconnectBigIcon))
                {
                    NetworkConnection.Disconnect("Cancelled connection to server");
                }
            }

            GUI.enabled = true;
            OptionsWindow.Singleton.Display = GUILayout.Toggle(OptionsWindow.Singleton.Display, SettingsBigIcon, ToggleButtonStyle);

            ServerListWindow.Singleton.Display = GUILayout.Toggle(ServerListWindow.Singleton.Display, ServerBigIcon, ToggleButtonStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawPlayerNameSection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ConnectionWindowText.PlayerName, LabelOptions);
            GUI.enabled = MainSystem.NetworkState <= ClientState.Disconnected;
            var newPlayerName = GUILayout.TextArea(SettingsSystem.CurrentSettings.PlayerName, 32); // Max 32 characters
            if (newPlayerName != SettingsSystem.CurrentSettings.PlayerName)
            {
                SettingsSystem.CurrentSettings.PlayerName = newPlayerName.Trim().Replace("\n", "");
                SettingsSystem.SaveSettings();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
    }
}
