// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.Window`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using LmpClient.Events;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using UnityEngine;

namespace LmpClient.Base
{
  public abstract class Window<T> : StyleLibrary, IWindow where T : IWindow, new()
  {
    private readonly GUIContent _tipContent = new GUIContent();
    private double _tipTime;
    private bool _display;
    protected bool ResizingWindow;

    public static T Singleton { get; } = new T();

    public string WindowName { get; } = typeof (T).Name;

    private string Tooltip { get; set; }

    public virtual bool Display
    {
      get => this._display && SettingsSystem.CurrentSettings.DisclaimerAccepted;
      set
      {
        if (!this._display & value)
        {
          this._display = true;
          this.OnDisplay();
        }
        else
        {
          if (!this._display || value)
            return;
          this._display = false;
          this.OnHide();
        }
      }
    }

    protected Window()
    {
      EventData<ClientState> networkStatusChanged = NetworkEvent.onNetworkStatusChanged;
      Window<T> window = this;
      // ISSUE: virtual method pointer
      EventData<ClientState>.OnEvent onEvent = new EventData<ClientState>.OnEvent((object) window, __vmethodptr(window, NetworkEventHandler));
      networkStatusChanged.Add(onEvent);
    }

    protected virtual void NetworkEventHandler(ClientState data)
    {
    }

    public virtual void OnDisplay()
    {
    }

    public virtual void OnHide() => this.RemoveWindowLock();

    public bool Initialized { get; set; }

    public bool IsWindowLocked { get; set; }

    public virtual void Update()
    {
      if (!this.Display || !this.Resizable)
        return;
      if (Input.GetMouseButtonUp(0))
        this.ResizingWindow = false;
      if (this.ResizingWindow)
      {
        ((Rect) ref this.WindowRect).width = (float) ((double) Input.mousePosition.x - (double) ((Rect) ref this.WindowRect).x + 10.0);
        ((Rect) ref this.WindowRect).height = (float) ((double) Screen.height - (double) Input.mousePosition.y - (double) ((Rect) ref this.WindowRect).y + 10.0);
      }
    }

    public void OnGui()
    {
      if (!this.Initialized)
      {
        this.InitializeStyles();
        this.SetStyles();
        this.Initialized = true;
      }
      this.CheckWindowLock();
      if (!this.Display)
        return;
      GUI.skin = StyleLibrary.Skin;
      this.DrawGui();
    }

    protected abstract void DrawGui();

    public virtual void CheckWindowLock()
    {
    }

    public virtual void RemoveWindowLock()
    {
    }

    public abstract void SetStyles();

    protected virtual bool Resizable { get; } = false;

    protected void DrawContent(int windowId)
    {
      this.DrawCloseButton(new Action(this.OnCloseButton), this.WindowRect);
      if (this.Resizable && GUI.RepeatButton(new Rect(((Rect) ref this.WindowRect).width - 15f, ((Rect) ref this.WindowRect).height - 15f, 10f, 10f), (Texture) StyleLibrary.ResizeIcon, StyleLibrary.ResizeButtonStyle))
        this.ResizingWindow = true;
      this.DrawWindowContent(windowId);
      if (!string.IsNullOrEmpty(this.Tooltip))
      {
        if ((double) Time.unscaledTime - this._tipTime > 0.349999994039536)
        {
          this._tipContent.text = this.Tooltip;
          Vector2 vector2 = StyleLibrary.ToolTipStyle.CalcSize(this._tipContent);
          vector2.x += 8f;
          vector2.y += 4f;
          GUILayout.BeginArea(GUIUtility.ScreenToGUIRect(new Rect(Mouse.screenPos.x, Mouse.screenPos.y - vector2.y, vector2.x, vector2.y)));
          GUILayout.Label(this.Tooltip, StyleLibrary.ToolTipStyle, Array.Empty<GUILayoutOption>());
          GUILayout.EndArea();
        }
      }
      else
        this._tipTime = (double) Time.unscaledTime;
      if (Event.current.type != 7)
        return;
      this.Tooltip = GUI.tooltip;
    }

    protected abstract void DrawWindowContent(int windowId);

    protected virtual void OnCloseButton() => this.Display = false;

    protected void DrawCloseButton(Action closeAction, Rect rect)
    {
      Color backgroundColor = GUI.backgroundColor;
      GUI.backgroundColor = Color.red;
      if (GUI.Button(new Rect(((Rect) ref rect).width - 25f, 4f, 20f, 20f), (Texture) StyleLibrary.CloseIcon, StyleLibrary.CloseButtonStyle))
        closeAction();
      GUI.backgroundColor = backgroundColor;
    }

    protected void DrawRefreshButton(Action refreshAction)
    {
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (GUILayout.Button(StyleLibrary.RefreshIcon, Array.Empty<GUILayoutOption>()))
        refreshAction();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
    }

    protected void DrawWaitIcon(bool small)
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.Label(small ? (Texture) StyleLibrary.WaitIcon : (Texture) StyleLibrary.WaitGiantIcon, Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
    }

    protected Rect FixWindowPos(Rect inputRect)
    {
      float num1 = (float) (0.0 - 0.75 * (double) ((Rect) ref inputRect).width);
      float num2 = (float) Screen.width - 0.25f * ((Rect) ref inputRect).width;
      float num3 = (float) (Screen.height - 20);
      if ((double) ((Rect) ref inputRect).x < (double) num1)
        ((Rect) ref inputRect).x = num1;
      if ((double) ((Rect) ref inputRect).x > (double) num2)
        ((Rect) ref inputRect).x = num2;
      if ((double) ((Rect) ref inputRect).y < 0.0)
        ((Rect) ref inputRect).y = 0.0f;
      if ((double) ((Rect) ref inputRect).y > (double) num3)
        ((Rect) ref inputRect).y = num3;
      return inputRect;
    }
  }
}
