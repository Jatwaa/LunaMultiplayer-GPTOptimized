// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.ServerList.ServerListWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Network;
using LmpCommon;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LmpClient.Windows.ServerList
{
  public class ServerListWindow : Window<ServerListWindow>
  {
    private static readonly float[] HeaderGridSize = new float[15];
    private static readonly Dictionary<string, PropertyInfo> OrderByPropertyDictionary = new Dictionary<string, PropertyInfo>();
    protected float WindowHeight = (float) Screen.height * 0.95f;
    protected float WindowWidth = (float) Screen.width * 0.95f;
    protected float ServerDetailWindowHeight = 50f;
    protected float ServerDetailWindowWidth = 350f;
    private static bool _display;
    private static readonly List<ServerInfo> DisplayedServers = new List<ServerInfo>();
    private static Vector2 _verticalScrollPosition;
    private static Vector2 _horizontalScrollPosition;
    private static Rect _serverDetailWindowRect;
    private static GUILayoutOption[] _serverDetailLayoutOptions;
    private static long _selectedServerId;
    private static string _orderBy = "PlayerCount";
    private static bool _ascending;
    private static GUIStyle _headerServerLine;
    private static GUIStyle _evenServerLine;
    private static GUIStyle _oddServerLine;
    private static GUIStyle _labelStyle;
    private static GUIStyle _kspLabelStyle;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      if (GUILayout.Button(StyleLibrary.RefreshBigIcon, Array.Empty<GUILayoutOption>()))
        NetworkServerList.RequestServers();
      ServerFilter.DrawFilters();
      this.DrawServersGrid();
      GUILayout.EndVertical();
    }

    private void DrawServersGrid()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      ServerListWindow._verticalScrollPosition = GUILayout.BeginScrollView(ServerListWindow._verticalScrollPosition, Array.Empty<GUILayoutOption>());
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      ServerListWindow._horizontalScrollPosition = GUILayout.BeginScrollView(ServerListWindow._horizontalScrollPosition, Array.Empty<GUILayoutOption>());
      ServerListWindow.DrawGridHeader();
      this.DrawServerList();
      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndScrollView();
      GUILayout.EndHorizontal();
    }

    private static void DrawGridHeader()
    {
      GUILayout.BeginHorizontal(ServerListWindow._headerServerLine, Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.Width(25f)
      });
      if (GUILayout.Button(ServerListWindow._ascending ? "▲" : "▼", Array.Empty<GUILayoutOption>()))
        ServerListWindow._ascending = !ServerListWindow._ascending;
      Rect lastRect;
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[0] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(30f)
      });
      if (GUILayout.Button(StyleLibrary.KeyIcon, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Password";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[1] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(30f)
      });
      if (GUILayout.Button(StyleLibrary.GlobeIcon, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Country";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[2] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(50f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Dedicated, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Dedicated";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[3] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(65f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Ping, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Ping";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[4] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(65f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Ping6, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Ping6";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[5] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(50f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Players, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "PlayerCount";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[6] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(85f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.MaxPlayers, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "MaxPlayers";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[7] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(85f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Mode, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "GameMode";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[8] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(75f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.WarpMode, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "WarpMode";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[9] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(50f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Terrain, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "TerrainQuality";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[10] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(50f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Cheats, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Cheats";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[11] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(220f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Name, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "ServerName";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double num;
        if ((double) ((Rect) ref lastRect).width <= 220.0)
        {
          num = 220.0;
        }
        else
        {
          lastRect = GUILayoutUtility.GetLastRect();
          num = (double) ((Rect) ref lastRect).width;
        }
        headerGridSize[12] = (float) num;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(150f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Website, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "WebsiteText";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double width = (double) ((Rect) ref lastRect).width;
        headerGridSize[13] = (float) width;
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(600f)
      });
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Description, Array.Empty<GUILayoutOption>()))
        ServerListWindow._orderBy = "Description";
      if (Event.current.type == 7)
      {
        float[] headerGridSize = ServerListWindow.HeaderGridSize;
        lastRect = GUILayoutUtility.GetLastRect();
        double num;
        if ((double) ((Rect) ref lastRect).width <= 600.0)
        {
          num = 600.0;
        }
        else
        {
          lastRect = GUILayoutUtility.GetLastRect();
          num = (double) ((Rect) ref lastRect).width;
        }
        headerGridSize[14] = (float) num;
      }
      GUILayout.EndHorizontal();
      GUILayout.EndHorizontal();
    }

    private void DrawServerList()
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (ServerListWindow.DisplayedServers == null || !ServerListWindow.DisplayedServers.Any<ServerInfo>())
      {
        GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
        GUILayout.FlexibleSpace();
        GUILayout.Label(LocalizationContainer.ServerListWindowText.NoServers, StyleLibrary.BigLabelStyle, Array.Empty<GUILayoutOption>());
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
      }
      else
      {
        GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
        for (int index = 0; index < ServerListWindow.DisplayedServers.Count; ++index)
        {
          ServerInfo displayedServer = ServerListWindow.DisplayedServers[index];
          GUILayout.BeginHorizontal(index % 2 != 0 ? ServerListWindow._oddServerLine : ServerListWindow._evenServerLine, Array.Empty<GUILayoutOption>());
          this.DrawServerEntry(displayedServer);
          GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();
    }

    private void DrawServerEntry(ServerInfo currentEntry)
    {
      ColorEffect.StartPaintingServer(currentEntry);
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[0])
      });
      if (GUILayout.Button("▶", Array.Empty<GUILayoutOption>()))
      {
        if (currentEntry.Password)
        {
          ServerListWindow._selectedServerId = currentEntry.Id;
        }
        else
        {
          NetworkServerList.IntroduceToServer(currentEntry.Id);
          this.Display = false;
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[1])
      });
      if (currentEntry.Password)
        GUILayout.Label(StyleLibrary.KeyIcon, ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
        {
          GUILayout.MinWidth(ServerListWindow.HeaderGridSize[1])
        });
      else
        GUILayout.Label("", ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
        {
          GUILayout.MinWidth(ServerListWindow.HeaderGridSize[1])
        });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[2])
      });
      GUILayout.Label(new GUIContent(currentEntry.Country ?? ""), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[2])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[3])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) currentEntry.DedicatedServer)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[3])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[4])
      });
      GUILayout.Label(new GUIContent(currentEntry.DisplayedPing ?? ""), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[4])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[5])
      });
      GUILayout.Label(new GUIContent(currentEntry.DisplayedPing6 ?? ""), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[5])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[6])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) currentEntry.PlayerCount)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[6])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[7])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) currentEntry.MaxPlayers)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[7])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[8])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) (GameMode) currentEntry.GameMode)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[8])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[9])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) (WarpMode) currentEntry.WarpMode)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[9])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[10])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) (TerrainQuality) currentEntry.TerrainQuality)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[10])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[11])
      });
      GUILayout.Label(new GUIContent(string.Format("{0}", (object) currentEntry.Cheats)), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[11])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[12])
      });
      GUILayout.Label(new GUIContent(currentEntry.ServerName ?? ""), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[12])
      });
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[13])
      });
      if (!string.IsNullOrEmpty(currentEntry.Website))
      {
        if (GUILayout.Button(new GUIContent(currentEntry.WebsiteText), ServerListWindow.GetCorrectHyperlinkLabelStyle(currentEntry), new GUILayoutOption[1]
        {
          GUILayout.MinWidth(ServerListWindow.HeaderGridSize[13])
        }))
          Application.OpenURL(currentEntry.Website);
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[14])
      });
      GUILayout.Label(new GUIContent(currentEntry.Description ?? ""), ServerListWindow.GetCorrectLabelStyle(currentEntry), new GUILayoutOption[1]
      {
        GUILayout.MinWidth(ServerListWindow.HeaderGridSize[14])
      });
      GUILayout.EndHorizontal();
      ColorEffect.StopPaintingServer();
    }

    public void DrawServerDetailsContent(int windowId)
    {
      this.DrawCloseButton((Action) (() => ServerListWindow._selectedServerId = 0L), ServerListWindow._serverDetailWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.ServerListWindowText.Password, this.LabelOptions);
      NetworkServerList.Password = GUILayout.PasswordField(NetworkServerList.Password, '*', 30, new GUILayoutOption[1]
      {
        GUILayout.Width(200f)
      });
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (GUILayout.Button(LocalizationContainer.ServerListWindowText.Connect, Array.Empty<GUILayoutOption>()))
      {
        NetworkServerList.IntroduceToServer(ServerListWindow._selectedServerId);
        ServerListWindow._selectedServerId = 0L;
        this.Display = false;
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && ServerListWindow._display && MainSystem.ToolbarShowGui && MainSystem.NetworkState == ClientState.Disconnected && HighLogic.LoadedScene == 2;
      set
      {
        if (!ServerListWindow._display & value)
          NetworkServerList.RequestServers();
        base.Display = ServerListWindow._display = value;
      }
    }

    protected override bool Resizable => true;

    public ServerListWindow()
    {
      foreach (PropertyInfo property in typeof (ServerInfo).GetProperties())
        ServerListWindow.OrderByPropertyDictionary.Add(property.Name, property);
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) Screen.width * 0.025f, (float) Screen.height * 0.025f, this.WindowWidth, this.WindowHeight);
      ServerListWindow._serverDetailWindowRect = new Rect((float) Screen.width * 0.025f, (float) Screen.height * 0.025f, this.WindowWidth, this.WindowHeight);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      ServerListWindow._headerServerLine = new GUIStyle()
      {
        normal = {
          background = new Texture2D(1, 1)
        }
      };
      ServerListWindow._headerServerLine.normal.background.SetPixel(0, 0, new Color(0.04f, 0.04f, 0.04f, 0.9f));
      ServerListWindow._headerServerLine.normal.background.Apply();
      ServerListWindow._headerServerLine.onNormal.background = new Texture2D(1, 1);
      ServerListWindow._headerServerLine.onNormal.background.SetPixel(0, 0, new Color(0.04f, 0.04f, 0.04f, 0.9f));
      ServerListWindow._headerServerLine.onNormal.background.Apply();
      ServerListWindow._evenServerLine = new GUIStyle()
      {
        normal = {
          background = new Texture2D(1, 1)
        }
      };
      ServerListWindow._evenServerLine.normal.background.SetPixel(0, 0, new Color(0.12f, 0.12f, 0.15f, 0.9f));
      ServerListWindow._evenServerLine.normal.background.Apply();
      ServerListWindow._evenServerLine.onNormal.background = new Texture2D(1, 1);
      ServerListWindow._evenServerLine.onNormal.background.SetPixel(0, 0, new Color(0.12f, 0.12f, 0.15f, 0.9f));
      ServerListWindow._evenServerLine.onNormal.background.Apply();
      ServerListWindow._oddServerLine = new GUIStyle()
      {
        normal = {
          background = new Texture2D(1, 1)
        }
      };
      ServerListWindow._oddServerLine.normal.background.SetPixel(0, 0, new Color(0.18f, 0.18f, 0.22f, 0.9f));
      ServerListWindow._oddServerLine.normal.background.Apply();
      ServerListWindow._oddServerLine.onNormal.background = new Texture2D(1, 1);
      ServerListWindow._oddServerLine.onNormal.background.SetPixel(0, 0, new Color(0.18f, 0.18f, 0.22f, 0.9f));
      ServerListWindow._oddServerLine.onNormal.background.Apply();
      ServerListWindow._kspLabelStyle = new GUIStyle(StyleLibrary.Skin.label)
      {
        alignment = (TextAnchor) 4
      };
      ServerListWindow._labelStyle = new GUIStyle(StyleLibrary.Skin.label)
      {
        alignment = (TextAnchor) 4,
        normal = GUI.skin.label.normal
      };
      ServerListWindow._serverDetailLayoutOptions = new GUILayoutOption[4];
      ServerListWindow._serverDetailLayoutOptions[0] = GUILayout.MinWidth(this.ServerDetailWindowWidth);
      ServerListWindow._serverDetailLayoutOptions[1] = GUILayout.MaxWidth(this.ServerDetailWindowWidth);
      ServerListWindow._serverDetailLayoutOptions[2] = GUILayout.MinHeight(this.ServerDetailWindowHeight);
      ServerListWindow._serverDetailLayoutOptions[3] = GUILayout.MaxHeight(this.ServerDetailWindowHeight);
      this.LabelOptions = new GUILayoutOption[1];
      this.LabelOptions[0] = GUILayout.Width(100f);
    }

    protected override void DrawGui()
    {
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154318, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.ServerListWindowText.Title, Array.Empty<GUILayoutOption>()));
      if ((ulong) ServerListWindow._selectedServerId <= 0UL)
        return;
      // ISSUE: method pointer
      ServerListWindow._serverDetailWindowRect = this.FixWindowPos(GUILayout.Window(1664154319, ServerListWindow._serverDetailWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawServerDetailsContent)), LocalizationContainer.ServerListWindowText.ServerDetailTitle, ServerListWindow._serverDetailLayoutOptions));
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display)
        return;
      ServerListWindow.DisplayedServers.Clear();
      ServerListWindow.DisplayedServers.AddRange(ServerListWindow._ascending ? (IEnumerable<ServerInfo>) NetworkServerList.Servers.Values.OrderBy<ServerInfo, object>((Func<ServerInfo, object>) (s => ServerListWindow.OrderByPropertyDictionary[ServerListWindow._orderBy].GetValue((object) s, (object[]) null))) : NetworkServerList.Servers.Values.OrderByDescending<ServerInfo, object>((Func<ServerInfo, object>) (s => ServerListWindow.OrderByPropertyDictionary[ServerListWindow._orderBy].GetValue((object) s, (object[]) null))).Where<ServerInfo>(new Func<ServerInfo, bool>(ServerFilter.MatchesFilters)));
    }

    private static GUIStyle GetCorrectLabelStyle(ServerInfo server) => server.DedicatedServer ? ServerListWindow._labelStyle : ServerListWindow._kspLabelStyle;

    private static GUIStyle GetCorrectHyperlinkLabelStyle(ServerInfo server) => server.DedicatedServer ? ServerListWindow._labelStyle : StyleLibrary.HyperlinkLabelStyle;
  }
}
