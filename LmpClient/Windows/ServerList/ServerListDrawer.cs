using LmpClient.Localization;
using LmpClient.Network;
using LmpClient.Systems.SettingsSys;
using LmpCommon;
using LmpCommon.Enums;
using System.Linq;
using UnityEngine;

namespace LmpClient.Windows.ServerList
{
    public partial class ServerListWindow
    {
        // Index 0 = connect, 1 = favorite, 2 = password, 3 = country, 4 = dedicated
        // 5 = ping, 6 = ping6, 7 = players, 8 = maxplayers, 9 = mode
        // 10 = warpmode, 11 = terrain, 12 = cheats, 13 = name, 14 = website, 15 = description
        private static readonly float[] HeaderGridSize = new float[16];

        #region Servers grid

        protected override void DrawWindowContent(int windowId)
        {
            GUILayout.BeginVertical();
            GUI.DragWindow(MoveRect);

            if (GUILayout.Button(RefreshBigIcon))
            {
                NetworkServerList.RequestServers();
            }
            ServerFilter.DrawFilters();
            DrawServersGrid();
            GUILayout.EndVertical();
        }

        private void DrawServersGrid()
        {
            // Header scroll — horizontal only, synced with content scroll
            var headerScroll = new Vector2(_horizontalScrollPosition.x, 0);
            headerScroll = GUILayout.BeginScrollView(headerScroll, false, false, GUIStyle.none, GUIStyle.none);
            DrawGridHeader();
            GUILayout.EndScrollView();
            _horizontalScrollPosition.x = headerScroll.x;

            // Content: vertical scroll only; horizontal is handled by outer container
            GUILayout.BeginHorizontal();
            _verticalScrollPosition = GUILayout.BeginScrollView(_verticalScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            GUILayout.BeginHorizontal();
            _horizontalScrollPosition = GUILayout.BeginScrollView(_horizontalScrollPosition, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none);
            DrawServerList();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Sets the sort column. If the same column is clicked again, toggles asc/desc.
        /// If a different column is clicked, switches to that column in ascending order.
        /// </summary>
        private static void SetSort(string column)
        {
            if (_orderBy == column)
                _ascending = !_ascending;
            else
            {
                _orderBy = column;
                _ascending = true;
            }
        }

        private static void DrawGridHeader()
        {
            GUILayout.BeginHorizontal(_headerServerLine);

            GUILayout.BeginHorizontal(GUILayout.Width(25));
            if (GUILayout.Button(_ascending ? "▲" : "▼"))
            {
                _ascending = !_ascending;
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[0] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            // Favorite column header
            GUILayout.BeginHorizontal(GUILayout.Width(30));
            GUILayout.Label(LocalizationContainer.ServerListWindowText.Favorite, GUILayout.Width(30));
            if (Event.current.type == EventType.Repaint) HeaderGridSize[1] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(30));
            if (GUILayout.Button(KeyIcon))
            {
                SetSort("Password");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[2] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(30));
            if (GUILayout.Button(GlobeIcon))
            {
                SetSort("Country");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[3] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(50));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Dedicated))
            {
                SetSort("DedicatedServer");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[4] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(65));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Ping))
            {
                SetSort("Ping");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[5] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(65));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Ping6))
            {
                SetSort("Ping6");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[6] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(50));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Players))
            {
                SetSort("PlayerCount");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[7] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(85));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.MaxPlayers))
            {
                SetSort("MaxPlayers");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[8] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(85));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Mode))
            {
                SetSort("GameMode");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[9] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(75));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.WarpMode))
            {
                SetSort("WarpMode");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[10] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(50));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Terrain))
            {
                SetSort("TerrainQuality");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[11] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(50));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Cheats))
            {
                SetSort("Cheats");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[12] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(220));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Name))
            {
                SetSort("ServerName");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[13] = GUILayoutUtility.GetLastRect().width > 220 ? GUILayoutUtility.GetLastRect().width : 220;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(150));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Website))
            {
                SetSort("WebsiteText");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[14] = GUILayoutUtility.GetLastRect().width;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(600));
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Description))
            {
                SetSort("Description");
            }
            if (Event.current.type == EventType.Repaint) HeaderGridSize[15] = GUILayoutUtility.GetLastRect().width > 600 ? GUILayoutUtility.GetLastRect().width : 600;
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        private void DrawServerList()
        {
            GUILayout.BeginHorizontal();

            if (DisplayedServers == null || !DisplayedServers.Any())
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label(LocalizationContainer.ServerListWindowText.NoServers, BigLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginVertical();

                for (var i = 0; i < DisplayedServers.Count; i++)
                {
                    var currentEntry = DisplayedServers[i];

                    GUILayout.BeginHorizontal(i % 2 != 0 ? _oddServerLine : _evenServerLine);
                    DrawServerEntry(currentEntry);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawServerEntry(ServerInfo currentEntry)
        {
            ColorEffect.StartPaintingServer(currentEntry);

            // Connect button
            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[0]));
            if (GUILayout.Button("▶"))
            {
                if (currentEntry.Password)
                {
                    _selectedServerId = currentEntry.Id;
                }
                else
                {
                    NetworkServerList.IntroduceToServer(currentEntry.Id);
                    Display = false;
                }
            }
            GUILayout.EndHorizontal();

            // Favorite toggle
            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[1]));
            var isFav = IsFavorite(currentEntry);
            if (GUILayout.Button(isFav ? "★" : "☆", GUILayout.MinWidth(HeaderGridSize[1])))
            {
                ToggleFavorite(currentEntry);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[2]));
            if (currentEntry.Password)
                GUILayout.Label(KeyIcon, GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[2]));
            else
                GUILayout.Label("", GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[2]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[3]));
            GUILayout.Label(new GUIContent($"{currentEntry.Country}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[3]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[4]));
            GUILayout.Label(new GUIContent($"{currentEntry.DedicatedServer}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[4]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[5]));
            GUILayout.Label(new GUIContent($"{currentEntry.DisplayedPing}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[5]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[6]));
            GUILayout.Label(new GUIContent($"{currentEntry.DisplayedPing6}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[6]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[7]));
            GUILayout.Label(new GUIContent($"{currentEntry.PlayerCount}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[7]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[8]));
            GUILayout.Label(new GUIContent($"{currentEntry.MaxPlayers}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[8]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[9]));
            GUILayout.Label(new GUIContent($"{(GameMode)currentEntry.GameMode}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[9]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[10]));
            GUILayout.Label(new GUIContent($"{(WarpMode)currentEntry.WarpMode}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[10]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[11]));
            GUILayout.Label(new GUIContent($"{(TerrainQuality)currentEntry.TerrainQuality}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[11]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[12]));
            GUILayout.Label(new GUIContent($"{currentEntry.Cheats}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[12]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[13]));
            GUILayout.Label(new GUIContent($"{currentEntry.ServerName}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[13]));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[14]));
            if (!string.IsNullOrEmpty(currentEntry.Website))
            {
                if (GUILayout.Button(new GUIContent(currentEntry.WebsiteText), GetCorrectHyperlinkLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[14])))
                {
                    Application.OpenURL(currentEntry.Website);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MinWidth(HeaderGridSize[15]));
            GUILayout.Label(new GUIContent($"{currentEntry.Description}"), GetCorrectLabelStyle(currentEntry), GUILayout.MinWidth(HeaderGridSize[15]));
            GUILayout.EndHorizontal();

            ColorEffect.StopPaintingServer();
        }

        #endregion

        #region Server details dialog

        public void DrawServerDetailsContent(int windowId)
        {
            //Always draw close button first
            DrawCloseButton(() => _selectedServerId = 0, _serverDetailWindowRect);

            GUILayout.BeginVertical();
            GUI.DragWindow(MoveRect);

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocalizationContainer.ServerListWindowText.Password, LabelOptions);
            NetworkServerList.Password = GUILayout.PasswordField(NetworkServerList.Password, '*', 30, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Connect))
            {
                NetworkServerList.IntroduceToServer(_selectedServerId);
                _selectedServerId = 0;
                Display = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        #endregion
    }
}
