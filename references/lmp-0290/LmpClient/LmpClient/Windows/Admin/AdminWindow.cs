// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Admin.AdminWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.Admin;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.Status;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Windows.Admin
{
  public class AdminWindow : SystemWindow<AdminWindow, AdminSystem>
  {
    private const float WindowHeight = 300f;
    private const float WindowWidth = 400f;
    private const float ConfirmationWindowHeight = 50f;
    private const float ConfirmationWindowWidth = 350f;
    private static Rect _confirmationWindowRect;
    private static GUILayoutOption[] _confirmationLayoutOptions;
    private static string _selectedPlayer;
    private static bool _banMode;
    private static string _reason = string.Empty;
    private static bool _display;

    protected override void DrawWindowContent(int windowId)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.AdminWindowText.Password, Array.Empty<GUILayoutOption>());
      LmpClient.Base.System<AdminSystem>.Singleton.AdminPassword = GUILayout.PasswordField(LmpClient.Base.System<AdminSystem>.Singleton.AdminPassword, '*', 30, new GUILayoutOption[1]
      {
        GUILayout.Width(200f)
      });
      GUILayout.EndHorizontal();
      GUILayout.Space(5f);
      GUI.enabled = !string.IsNullOrEmpty(LmpClient.Base.System<AdminSystem>.Singleton.AdminPassword);
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, Array.Empty<GUILayoutOption>());
      foreach (string key in (IEnumerable<string>) LmpClient.Base.System<StatusSystem>.Singleton.PlayerStatusList.Keys)
      {
        if (!(key == SettingsSystem.CurrentSettings.PlayerName))
          AdminWindow.DrawPlayerLine(key);
      }
      GUILayout.EndScrollView();
      GUILayout.Space(5f);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      if (GUILayout.Button(StyleLibrary.DekesslerBigIcon, Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<AdminSystem>.Singleton.MessageSender.SendDekesslerMsg();
      if (GUILayout.Button(StyleLibrary.NukeBigIcon, Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<AdminSystem>.Singleton.MessageSender.SendNukeMsg();
      if (GUILayout.Button(StyleLibrary.RestartServerIcon, Array.Empty<GUILayoutOption>()))
      {
        LmpClient.Base.System<AdminSystem>.Singleton.MessageSender.SendServerRestartMsg();
        this.Display = false;
      }
      GUI.enabled = true;
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    private static void DrawPlayerLine(string playerName)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(playerName, Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (GUILayout.Button(StyleLibrary.BanIcon, Array.Empty<GUILayoutOption>()))
      {
        AdminWindow._selectedPlayer = playerName;
        AdminWindow._banMode = true;
      }
      if (GUILayout.Button(StyleLibrary.KickIcon, Array.Empty<GUILayoutOption>()))
      {
        AdminWindow._selectedPlayer = playerName;
        AdminWindow._banMode = false;
      }
      GUILayout.EndHorizontal();
    }

    public void DrawConfirmationDialog(int windowId)
    {
      this.DrawCloseButton((Action) (() =>
      {
        AdminWindow._selectedPlayer = (string) null;
        AdminWindow._reason = string.Empty;
      }), AdminWindow._confirmationWindowRect);
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      GUILayout.Label(AdminWindow._banMode ? LocalizationContainer.AdminWindowText.BanText : LocalizationContainer.AdminWindowText.KickText, this.LabelOptions);
      GUILayout.Space(20f);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(LocalizationContainer.AdminWindowText.Reason, this.LabelOptions);
      AdminWindow._reason = GUILayout.TextField(AdminWindow._reason, (int) byte.MaxValue, new GUILayoutOption[1]
      {
        GUILayout.Width((float) byte.MaxValue)
      });
      GUILayout.EndHorizontal();
      GUILayout.Space(20f);
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (AdminWindow._banMode)
      {
        if (GUILayout.Button(StyleLibrary.BanBigIcon, new GUILayoutOption[1]
        {
          GUILayout.Width((float) byte.MaxValue)
        }))
        {
          LmpClient.Base.System<AdminSystem>.Singleton.MessageSender.SendBanPlayerMsg(AdminWindow._selectedPlayer, AdminWindow._reason);
          AdminWindow._selectedPlayer = (string) null;
          AdminWindow._reason = string.Empty;
        }
      }
      else if (GUILayout.Button(StyleLibrary.KickBigIcon, new GUILayoutOption[1]
      {
        GUILayout.Width((float) byte.MaxValue)
      }))
      {
        LmpClient.Base.System<AdminSystem>.Singleton.MessageSender.SendKickPlayerMsg(AdminWindow._selectedPlayer, AdminWindow._reason);
        AdminWindow._selectedPlayer = (string) null;
        AdminWindow._reason = string.Empty;
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && AdminWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5 && SettingsSystem.ServerSettings.AllowAdmin;
      set => base.Display = AdminWindow._display = value;
    }

    protected override void DrawGui()
    {
      if (this.Display)
      {
        // ISSUE: method pointer
        this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154327, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.AdminWindowText.Title, this.LayoutOptions));
        if (string.IsNullOrEmpty(AdminWindow._selectedPlayer))
          return;
        // ISSUE: method pointer
        AdminWindow._confirmationWindowRect = this.FixWindowPos(GUILayout.Window(1664154328, AdminWindow._confirmationWindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawConfirmationDialog)), LocalizationContainer.AdminWindowText.ConfirmDialogTitle, AdminWindow._confirmationLayoutOptions));
      }
      else
      {
        AdminWindow._reason = string.Empty;
        AdminWindow._selectedPlayer = (string) null;
      }
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) ((double) Screen.width / 2.0 - 200.0), (float) ((double) Screen.height / 2.0 - 150.0), 400f, 300f);
      this.MoveRect = new Rect(0.0f, 0.0f, 10000f, 40f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(400f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(400f);
      this.LayoutOptions[2] = GUILayout.MinHeight(300f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(300f);
      AdminWindow._confirmationLayoutOptions = new GUILayoutOption[4];
      AdminWindow._confirmationLayoutOptions[0] = GUILayout.MinWidth(350f);
      AdminWindow._confirmationLayoutOptions[1] = GUILayout.MaxWidth(350f);
      AdminWindow._confirmationLayoutOptions[2] = GUILayout.MinHeight(50f);
      AdminWindow._confirmationLayoutOptions[3] = GUILayout.MaxHeight(50f);
      this.ScrollPos = new Vector2();
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_AdminLock");
    }

    public override void CheckWindowLock()
    {
      if (this.Display)
      {
        if (MainSystem.NetworkState < ClientState.Running || HighLogic.LoadedSceneIsFlight)
        {
          this.RemoveWindowLock();
          return;
        }
        Vector2 vector2 = Vector2.op_Implicit(Input.mousePosition);
        vector2.y = (float) Screen.height - vector2.y;
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2) || !string.IsNullOrEmpty(AdminWindow._selectedPlayer) && ((Rect) ref AdminWindow._confirmationWindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_AdminLock");
          this.IsWindowLocked = true;
        }
        if (!flag && this.IsWindowLocked)
          this.RemoveWindowLock();
      }
      if (this.Display || !this.IsWindowLocked)
        return;
      this.RemoveWindowLock();
    }
  }
}
