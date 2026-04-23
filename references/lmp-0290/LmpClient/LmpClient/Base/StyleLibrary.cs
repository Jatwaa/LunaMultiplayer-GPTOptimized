// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.StyleLibrary
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization;
using LmpClient.Utilities;
using LmpClient.Windows;
using UnityEngine;

namespace LmpClient.Base
{
  public abstract class StyleLibrary
  {
    public static GUISkin DefaultSkin;
    protected const int TitleHeight = 30;
    protected static GUISkin Skin;
    protected static GUIStyle RedFontButtonStyle;
    protected static GUIStyle CloseButtonStyle;
    protected static GUIStyle ResizeButtonStyle;
    protected static GUIStyle HyperlinkLabelStyle;
    protected static GUIStyle BoldRedLabelStyle;
    protected static GUIStyle BoldGreenLabelStyle;
    protected static GUIStyle StatusStyle;
    protected static GUIStyle BigLabelStyle;
    protected static GUIStyle ToggleButtonStyle;
    public static GUIStyle ToolTipStyle;
    protected GUILayoutOption[] LayoutOptions;
    protected GUILayoutOption[] TextAreaOptions;
    protected GUILayoutOption[] LabelOptions;
    protected Rect WindowRect;
    protected Rect MoveRect;
    protected Vector2 ScrollPos = new Vector2();
    protected static GUIContent SettingsIcon;
    protected static GUIContent SettingsBigIcon;
    protected static GUIContent ServerIcon;
    protected static GUIContent ServerBigIcon;
    protected static GUIContent SystemIcon;
    protected static GUIContent ConnectIcon;
    protected static GUIContent ConnectBigIcon;
    protected static GUIContent DebugIcon;
    protected static GUIContent DisconnectIcon;
    protected static GUIContent DisconnectBigIcon;
    protected static GUIContent LockIcon;
    protected static GUIContent SyncIcon;
    protected static Texture2D ResizeIcon;
    protected static Texture2D CloseIcon;
    protected static GUIContent RefreshIcon;
    protected static GUIContent RefreshBigIcon;
    protected static GUIContent UploadIcon;
    protected static GUIContent DeleteIcon;
    protected static GUIContent PlusIcon;
    protected static GUIContent SaveIcon;
    protected static Texture2D WaitIcon;
    protected static Texture2D WaitGiantIcon;
    protected static GUIContent KeyIcon;
    protected static GUIContent GlobeIcon;
    protected static GUIContent ChatIcon;
    protected static GUIContent ChatRedIcon;
    protected static GUIContent CameraIcon;
    protected static GUIContent CameraRedIcon;
    protected static GUIContent RocketIcon;
    protected static GUIContent RocketRedIcon;
    protected static GUIContent AdminIcon;
    protected static GUIContent KickIcon;
    protected static GUIContent KickBigIcon;
    protected static GUIContent BanIcon;
    protected static GUIContent BanBigIcon;
    protected static GUIContent DekesslerIcon;
    protected static GUIContent NukeIcon;
    protected static GUIContent DekesslerBigIcon;
    protected static GUIContent NukeBigIcon;
    protected static GUIContent RestartServerIcon;
    protected static GUIContent DownloadBigIcon;
    protected static GUIContent CycleFirstIcon;
    protected static GUIContent CyclePreviousIcon;
    protected static GUIContent CycleNextIcon;
    protected static GUIContent CycleLastIcon;

    protected bool Flash => (double) Time.time % 0.699999988079071 < 0.25;

    protected void InitializeStyles()
    {
      if (!Object.op_Equality((Object) StyleLibrary.Skin, (Object) null))
        return;
      StyleLibrary.Skin = Object.Instantiate<GUISkin>(HighLogic.Skin);
      StyleLibrary.SettingsIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "settings.png"), 16, 16), LocalizationContainer.ButtonTooltips.SettingsIcon);
      StyleLibrary.SettingsBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "settingsBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.SettingsIcon);
      StyleLibrary.ServerIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "server.png"), 16, 16), LocalizationContainer.ButtonTooltips.ServerIcon);
      StyleLibrary.ServerBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "serverBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.ServerIcon);
      StyleLibrary.SystemIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "system.png"), 16, 16), "SYSTEM");
      StyleLibrary.ConnectIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "connect.png"), 16, 16), LocalizationContainer.ButtonTooltips.ConnectIcon);
      StyleLibrary.ConnectBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "connectBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.ConnectIcon);
      StyleLibrary.DebugIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "debug.png"), 16, 16), "DEBUG");
      StyleLibrary.DisconnectIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "disconnect.png"), 16, 16), LocalizationContainer.ButtonTooltips.DisconnectIcon);
      StyleLibrary.DisconnectBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "disconnectBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.DisconnectIcon);
      StyleLibrary.LockIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "lock.png"), 16, 16), "LOCK");
      StyleLibrary.SyncIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "sync.png"), 16, 16), LocalizationContainer.ButtonTooltips.SyncIcon);
      StyleLibrary.ResizeIcon = WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "resize.png"), 16, 16);
      StyleLibrary.CloseIcon = WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "close_small.png"), 10, 10);
      StyleLibrary.RefreshIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "refresh.png"), 16, 16), LocalizationContainer.ButtonTooltips.RefreshIcon);
      StyleLibrary.RefreshBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "refreshBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.RefreshIcon);
      StyleLibrary.UploadIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "upload.png"), 16, 16), LocalizationContainer.ButtonTooltips.UploadIcon);
      StyleLibrary.DeleteIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "delete.png"), 16, 16), LocalizationContainer.ButtonTooltips.DeleteIcon);
      StyleLibrary.PlusIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "plus.png"), 16, 16), LocalizationContainer.ButtonTooltips.PlusIcon);
      StyleLibrary.SaveIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "save.png"), 16, 16), LocalizationContainer.ButtonTooltips.SaveIcon);
      StyleLibrary.WaitGiantIcon = WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "waitGiant.png"), 16, 16);
      StyleLibrary.WaitIcon = WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "wait.png"), 16, 16);
      StyleLibrary.KeyIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "key.png"), 16, 16), LocalizationContainer.ButtonTooltips.KeyIcon);
      StyleLibrary.GlobeIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "globe.png"), 16, 16), LocalizationContainer.ButtonTooltips.GlobeIcon);
      StyleLibrary.ChatIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "chatWhite.png"), 16, 16), LocalizationContainer.ButtonTooltips.ChatIcon);
      StyleLibrary.ChatRedIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "chatRed.png"), 16, 16), LocalizationContainer.ButtonTooltips.ChatIcon);
      StyleLibrary.CameraIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "camera.png"), 16, 16), LocalizationContainer.ButtonTooltips.CameraIcon);
      StyleLibrary.CameraRedIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "cameraRed.png"), 16, 16), LocalizationContainer.ButtonTooltips.CameraIcon);
      StyleLibrary.RocketIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "rocket.png"), 16, 16), LocalizationContainer.ButtonTooltips.RocketIcon);
      StyleLibrary.RocketRedIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "rocketRed.png"), 16, 16), LocalizationContainer.ButtonTooltips.RocketIcon);
      StyleLibrary.AdminIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "admin.png"), 16, 16), LocalizationContainer.ButtonTooltips.AdminIcon);
      StyleLibrary.KickIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "kick.png"), 16, 16), LocalizationContainer.ButtonTooltips.KickIcon);
      StyleLibrary.KickBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "kickBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.KickIcon);
      StyleLibrary.BanIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "ban.png"), 16, 16), LocalizationContainer.ButtonTooltips.BanIcon);
      StyleLibrary.BanBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "banBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.BanIcon);
      StyleLibrary.DekesslerIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "dekessler.png"), 16, 16), LocalizationContainer.ButtonTooltips.DekesslerIcon);
      StyleLibrary.NukeIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "nuke.png"), 16, 16), LocalizationContainer.ButtonTooltips.NukeIcon);
      StyleLibrary.DekesslerBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "dekesslerBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.DekesslerIcon);
      StyleLibrary.NukeBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "nukeBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.NukeIcon);
      StyleLibrary.RestartServerIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "restartServerBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.RestartServerIcon);
      StyleLibrary.DownloadBigIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "downloadBig.png"), 32, 32), LocalizationContainer.ButtonTooltips.DownloadIcon);
      StyleLibrary.CycleFirstIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "cycleFirstIcon.png"), 32, 32), LocalizationContainer.ButtonTooltips.CycleFirstIcon);
      StyleLibrary.CyclePreviousIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "cyclePreviousIcon.png"), 32, 32), LocalizationContainer.ButtonTooltips.CyclePreviousIcon);
      StyleLibrary.CycleNextIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "cycleNextIcon.png"), 32, 32), LocalizationContainer.ButtonTooltips.CycleNextIcon);
      StyleLibrary.CycleLastIcon = new GUIContent((Texture) WindowUtil.LoadIcon(CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Icons", "cycleLastIcon.png"), 32, 32), LocalizationContainer.ButtonTooltips.CycleLastIcon);
      StyleLibrary.RedFontButtonStyle = new GUIStyle(StyleLibrary.Skin.button)
      {
        normal = {
          textColor = Color.red
        },
        active = {
          textColor = Color.red
        },
        hover = {
          textColor = Color.red
        }
      };
      StyleLibrary.CloseButtonStyle = new GUIStyle(StyleLibrary.Skin.button)
      {
        padding = new RectOffset(2, 2, 2, 2),
        margin = new RectOffset(2, 2, 2, 2)
      };
      StyleLibrary.ResizeButtonStyle = new GUIStyle(StyleLibrary.Skin.button)
      {
        padding = new RectOffset(0, 0, 0, 0),
        border = new RectOffset(0, 0, 0, 0),
        normal = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        active = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        focused = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        hover = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        onNormal = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        onActive = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        onFocused = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        },
        onHover = new GUIStyleState()
        {
          background = StyleLibrary.ResizeIcon
        }
      };
      StyleLibrary.HyperlinkLabelStyle = new GUIStyle(StyleLibrary.Skin.button)
      {
        fontStyle = (FontStyle) 1,
        padding = new RectOffset(0, 0, 0, 0),
        border = new RectOffset(0, 0, 0, 0),
        normal = new GUIStyleState()
        {
          textColor = XKCDColors.KSPUnnamedCyan
        },
        active = new GUIStyleState(),
        focused = new GUIStyleState(),
        hover = new GUIStyleState(),
        onNormal = new GUIStyleState(),
        onActive = new GUIStyleState(),
        onFocused = new GUIStyleState(),
        onHover = new GUIStyleState()
      };
      StyleLibrary.BoldGreenLabelStyle = new GUIStyle(StyleLibrary.Skin.label)
      {
        fontStyle = (FontStyle) 1,
        normal = new GUIStyleState()
        {
          textColor = XKCDColors.KSPBadassGreen
        }
      };
      StyleLibrary.BoldRedLabelStyle = new GUIStyle(StyleLibrary.Skin.label)
      {
        fontStyle = (FontStyle) 1,
        normal = new GUIStyleState()
        {
          textColor = XKCDColors.KSPNotSoGoodOrange
        }
      };
      StyleLibrary.BigLabelStyle = new GUIStyle(StyleLibrary.Skin.label)
      {
        fontSize = 60,
        normal = {
          textColor = XKCDColors.KSPNotSoGoodOrange
        }
      };
      StyleLibrary.ToolTipStyle = new GUIStyle(StyleLibrary.Skin.box)
      {
        padding = new RectOffset(2, 2, 2, 2)
      };
      StyleLibrary.ToggleButtonStyle = StyleLibrary.Skin.button;
    }
  }
}
