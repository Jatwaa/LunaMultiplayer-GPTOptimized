// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Chat.ChatWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.Chat;
using LmpClient.Systems.PlayerColorSys;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Windows.Chat
{
  public class ChatWindow : Window<ChatWindow>
  {
    private static bool _display;
    private const float WindowHeight = 300f;
    private const float WindowWidth = 400f;
    private static Vector2 _chatScrollPos;
    private static GUIStyle _playerNameStyle;
    private static string _chatInputText = string.Empty;

    public override bool Display
    {
      get => base.Display && ChatWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set => base.Display = ChatWindow._display = value;
    }

    protected override bool Resizable => true;

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) (Screen.width / 10), (float) ((double) Screen.height / 2.0 - 150.0), 400f, 300f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      ChatWindow._chatScrollPos = new Vector2(0.0f, 0.0f);
      ChatWindow._playerNameStyle = new GUIStyle(GUI.skin.label)
      {
        fontStyle = (FontStyle) 0,
        stretchWidth = false,
        wordWrap = true
      };
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_ChatLock");
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display || !LmpClient.Base.System<ChatSystem>.Singleton.NewMessageReceived)
        return;
      LmpClient.Base.System<ChatSystem>.Singleton.NewMessageReceived = false;
    }

    protected override void DrawGui() => this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154308, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), LocalizationContainer.ChatWindowText.Title, Array.Empty<GUILayoutOption>()));

    public void ScrollToBottom() => ChatWindow._chatScrollPos.y = float.PositiveInfinity;

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
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_ChatLock");
          this.IsWindowLocked = true;
        }
        if (!flag && this.IsWindowLocked)
          this.RemoveWindowLock();
      }
      if (this.Display || !this.IsWindowLocked)
        return;
      this.RemoveWindowLock();
    }

    protected override void DrawWindowContent(int windowId)
    {
      bool pressedEnter = Event.current.type == 4 && !Event.current.shift && Event.current.character == '\n';
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUI.DragWindow(this.MoveRect);
      ChatWindow.DrawChatMessageBox();
      ChatWindow.DrawTextInput(pressedEnter);
      GUILayout.Space(5f);
      GUILayout.EndVertical();
    }

    private static void DrawChatMessageBox()
    {
      ChatWindow._chatScrollPos = GUILayout.BeginScrollView(ChatWindow._chatScrollPos, Array.Empty<GUILayoutOption>());
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      foreach (Tuple<string, string, string> chatMessage in (Queue<Tuple<string, string, string>>) LmpClient.Base.System<ChatSystem>.Singleton.ChatMessages)
      {
        ChatWindow._playerNameStyle.normal.textColor = LmpClient.Base.System<PlayerColorSystem>.Singleton.GetPlayerColor(chatMessage.Item1);
        GUILayout.Label(chatMessage.Item3, ChatWindow._playerNameStyle, Array.Empty<GUILayoutOption>());
      }
      GUILayout.EndVertical();
      GUILayout.EndScrollView();
    }

    private static void DrawTextInput(bool pressedEnter)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      int num;
      if (!pressedEnter)
        num = GUILayout.Button(LocalizationContainer.ChatWindowText.Send, new GUILayoutOption[1]
        {
          GUILayout.Width(100f)
        }) ? 1 : 0;
      else
        num = 1;
      if (num != 0)
      {
        if (!string.IsNullOrEmpty(ChatWindow._chatInputText))
          LmpClient.Base.System<ChatSystem>.Singleton.MessageSender.SendChatMsg(ChatWindow._chatInputText.Trim('\n'));
        ChatWindow._chatInputText = string.Empty;
      }
      else
        ChatWindow._chatInputText = GUILayout.TextArea(ChatWindow._chatInputText, Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
    }
  }
}
