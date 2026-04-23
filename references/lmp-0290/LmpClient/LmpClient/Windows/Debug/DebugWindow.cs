// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Debug.DebugWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LmpClient.Windows.Debug
{
  public class DebugWindow : Window<DebugWindow>
  {
    private static readonly StringBuilder StringBuilder = new StringBuilder();
    private static readonly List<Tuple<Guid, string>> VesselProtoStoreData = new List<Tuple<Guid, string>>();
    private const float DisplayUpdateInterval = 0.2f;
    private const float WindowHeight = 400f;
    private const float WindowWidth = 650f;
    private static bool _displayFast;
    private static string _subspaceText;
    private static string _timeText;
    private static string _connectionText;
    private static float _lastUpdateTime;
    private static bool _displaySubspace;
    private static bool _displayTimes;
    private static bool _displayConnectionQueue;
    private static bool _display;

    protected override void DrawWindowContent(int windowId)
    {
      GUI.DragWindow(this.MoveRect);
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, new GUILayoutOption[2]
      {
        GUILayout.Width(650f),
        GUILayout.Height(400f)
      });
      DebugWindow.DrawDebugButtons();
      GUILayout.EndScrollView();
    }

    private static void DrawDebugButtons()
    {
      GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
      DebugWindow._displayFast = GUILayout.Toggle(DebugWindow._displayFast, "Fast debug update", Array.Empty<GUILayoutOption>());
      DebugWindow._displaySubspace = GUILayout.Toggle(DebugWindow._displaySubspace, "Display subspace statistics", Array.Empty<GUILayoutOption>());
      if (DebugWindow._displaySubspace)
        GUILayout.Label(DebugWindow._subspaceText, Array.Empty<GUILayoutOption>());
      DebugWindow._displayTimes = GUILayout.Toggle(DebugWindow._displayTimes, "Display time clocks", Array.Empty<GUILayoutOption>());
      if (DebugWindow._displayTimes)
        GUILayout.Label(DebugWindow._timeText, Array.Empty<GUILayoutOption>());
      DebugWindow._displayConnectionQueue = GUILayout.Toggle(DebugWindow._displayConnectionQueue, "Display connection statistics", Array.Empty<GUILayoutOption>());
      if (DebugWindow._displayConnectionQueue)
        GUILayout.Label(DebugWindow._connectionText, Array.Empty<GUILayoutOption>());
      GUILayout.EndVertical();
    }

    public override bool Display
    {
      get => base.Display && DebugWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set => base.Display = DebugWindow._display = value;
    }

    public override void Update()
    {
      base.Update();
      if ((!this.Display || (double) UnityEngine.Time.realtimeSinceStartup - (double) DebugWindow._lastUpdateTime <= 0.200000002980232) && !DebugWindow._displayFast)
        return;
      DebugWindow._lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
      if (DebugWindow._displaySubspace)
      {
        DebugWindow.StringBuilder.AppendLine(string.Format("Warp rate: {0}x.", (object) Math.Round((double) UnityEngine.Time.timeScale, 3)));
        DebugWindow.StringBuilder.AppendLine(string.Format("Current subspace: {0}.", (object) LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspace));
        DebugWindow.StringBuilder.AppendLine(string.Format("Current subspace time: {0}s.", (object) LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTime));
        DebugWindow.StringBuilder.AppendLine(string.Format("Current subspace time difference: {0}s.", (object) LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspaceTimeDifference));
        DebugWindow.StringBuilder.AppendLine(string.Format("Current Error: {0}ms.", (object) Math.Round(TimeSyncSystem.CurrentErrorSec * 1000.0, 0)));
        DebugWindow.StringBuilder.AppendLine(string.Format("Current universe time: {0} UT", (object) Math.Round(TimeSyncSystem.UniversalTime, 3)));
        DebugWindow._subspaceText = DebugWindow.StringBuilder.ToString();
        DebugWindow.StringBuilder.Length = 0;
      }
      TimeSpan timeSpan;
      if (DebugWindow._displayTimes)
      {
        DebugWindow.StringBuilder.AppendLine(string.Format("Server start time: {0:yyyy-MM-dd HH-mm-ss.ffff}", (object) new DateTime(TimeSyncSystem.ServerStartTime)));
        DebugWindow.StringBuilder.AppendLine(string.Format("Computer clock time (UTC): {0:HH:mm:ss.fff}", (object) DateTime.UtcNow));
        DebugWindow.StringBuilder.AppendLine(string.Format("Computer clock offset (minutes): {0}", (object) LunaComputerTime.SimulatedMinutesTimeOffset));
        DebugWindow.StringBuilder.AppendLine(string.Format("Computer clock time + offset: {0:HH:mm:ss.fff}", (object) LunaComputerTime.UtcNow));
        DebugWindow.StringBuilder.AppendLine(string.Format("Computer <-> NTP clock difference: {0}ms.", (object) LunaNetworkTime.TimeDifference.TotalMilliseconds));
        DebugWindow.StringBuilder.AppendLine(string.Format("NTP clock offset: {0}ms.", (object) LunaNetworkTime.SimulatedMsTimeOffset));
        StringBuilder stringBuilder = DebugWindow.StringBuilder;
        timeSpan = LunaNetworkTime.TimeDifference;
        string str = string.Format("Total Difference: {0}ms.", (object) (timeSpan.TotalMilliseconds + (double) LunaNetworkTime.SimulatedMsTimeOffset));
        stringBuilder.AppendLine(str);
        DebugWindow.StringBuilder.AppendLine(string.Format("NTP clock time (UTP): {0:HH:mm:ss.fff}", (object) LunaNetworkTime.UtcNow));
        DebugWindow._timeText = DebugWindow.StringBuilder.ToString();
        DebugWindow.StringBuilder.Length = 0;
      }
      if (DebugWindow._displayConnectionQueue)
      {
        StringBuilder stringBuilder1 = DebugWindow.StringBuilder;
        timeSpan = TimeSpan.FromSeconds((double) NetworkStatistics.PingSec);
        string str1 = string.Format("Ping: {0}ms.", (object) timeSpan.TotalMilliseconds);
        stringBuilder1.AppendLine(str1);
        StringBuilder stringBuilder2 = DebugWindow.StringBuilder;
        timeSpan = TimeSpan.FromSeconds((double) NetworkStatistics.AvgPingSec);
        string str2 = string.Format("Average Ping: {0}ms.", (object) timeSpan.TotalMilliseconds);
        stringBuilder2.AppendLine(str2);
        StringBuilder stringBuilder3 = DebugWindow.StringBuilder;
        timeSpan = TimeSpan.FromSeconds((double) NetworkStatistics.TimeOffset);
        string str3 = string.Format("TimeOffset: {0}ms.", (object) timeSpan.TotalMilliseconds);
        stringBuilder3.AppendLine(str3);
        DebugWindow.StringBuilder.AppendLine(string.Format("Messages in cache: {0}.", (object) NetworkStatistics.MessagesInCache));
        DebugWindow.StringBuilder.AppendLine(string.Format("Message data in cache: {0}.", (object) NetworkStatistics.MessageDataInCache));
        DebugWindow.StringBuilder.AppendLine(string.Format("Sent bytes: {0}.", (object) NetworkStatistics.SentBytes));
        DebugWindow.StringBuilder.AppendLine(string.Format("Received bytes: {0}.\n", (object) NetworkStatistics.ReceivedBytes));
        DebugWindow._connectionText = DebugWindow.StringBuilder.ToString();
        DebugWindow.StringBuilder.Length = 0;
      }
    }

    protected override void DrawGui()
    {
      GUI.skin = StyleLibrary.DefaultSkin;
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154309, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), "Debug", this.LayoutOptions));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) Screen.width - 700f, (float) ((double) Screen.height / 2.0 - 200.0), 650f, 400f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(650f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(650f);
      this.LayoutOptions[2] = GUILayout.MinHeight(400f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(400f);
      this.TextAreaOptions = new GUILayoutOption[1];
      this.TextAreaOptions[0] = GUILayout.ExpandWidth(true);
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_DebugLock");
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
        bool flag = ((Rect) ref this.WindowRect).Contains(vector2);
        if (flag && !this.IsWindowLocked)
        {
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_DebugLock");
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
