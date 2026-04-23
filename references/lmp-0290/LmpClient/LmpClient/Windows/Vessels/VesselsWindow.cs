// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.VesselsWindow
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Windows.Vessels.Structures;
using LmpCommon.Enums;
using LmpCommon.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LmpClient.Windows.Vessels
{
  public class VesselsWindow : Window<VesselsWindow>
  {
    private static bool _display;
    private const float WindowHeight = 500f;
    private const float WindowWidth = 600f;
    private DateTime _lastUpdateTime = DateTime.MinValue;
    private static VesselDisplay _activeVesselDisplayStore;
    private static readonly Dictionary<Guid, VesselDisplay> VesselDisplayStore = new Dictionary<Guid, VesselDisplay>();
    private static bool _fastUpdate = false;
    private const int SlowUpdateInterval = 1000;
    private const int FastUpdateInterval = 50;

    protected override void DrawWindowContent(int windowId)
    {
      GUI.DragWindow(this.MoveRect);
      this.ScrollPos = GUILayout.BeginScrollView(this.ScrollPos, new GUILayoutOption[2]
      {
        GUILayout.Width(600f),
        GUILayout.Height(500f)
      });
      VesselsWindow.PrintVessels();
      GUILayout.EndScrollView();
    }

    private static void PrintVessels()
    {
      VesselsWindow._fastUpdate = GUILayout.Toggle(VesselsWindow._fastUpdate, "Fast Update", Array.Empty<GUILayoutOption>());
      VesselFilter.DrawFilters();
      if (VesselsWindow._activeVesselDisplayStore != null)
      {
        GUILayout.Label("Active vessel:", Array.Empty<GUILayoutOption>());
        GUILayout.BeginVertical(StyleLibrary.Skin.box, Array.Empty<GUILayoutOption>());
        VesselsWindow._activeVesselDisplayStore.Display = GUILayout.Toggle(VesselsWindow._activeVesselDisplayStore.Display, VesselsWindow._activeVesselDisplayStore.VesselId.ToString(), Array.Empty<GUILayoutOption>());
        VesselsWindow._activeVesselDisplayStore.Print();
        GUILayout.EndVertical();
        GUILayout.Label("Other vessels:", Array.Empty<GUILayoutOption>());
      }
      else
        GUILayout.Label("Vessels:", Array.Empty<GUILayoutOption>());
      foreach (KeyValuePair<Guid, VesselDisplay> keyValuePair in VesselsWindow.VesselDisplayStore)
      {
        GUILayout.BeginVertical(StyleLibrary.Skin.box, Array.Empty<GUILayoutOption>());
        keyValuePair.Value.Display = GUILayout.Toggle(keyValuePair.Value.Display, keyValuePair.Value.VesselId.ToString(), Array.Empty<GUILayoutOption>());
        keyValuePair.Value.Print();
        GUILayout.EndVertical();
      }
    }

    public override bool Display
    {
      get => base.Display && VesselsWindow._display && MainSystem.NetworkState >= ClientState.Running && HighLogic.LoadedScene >= 5;
      set => base.Display = VesselsWindow._display = value;
    }

    public override void Update()
    {
      base.Update();
      if (!this.Display || !TimeUtil.IsInInterval(ref this._lastUpdateTime, VesselsWindow._fastUpdate ? 50 : 1000))
        return;
      if (Object.op_Implicit((Object) FlightGlobals.ActiveVessel))
      {
        if (VesselsWindow._activeVesselDisplayStore == null)
          VesselsWindow._activeVesselDisplayStore = new VesselDisplay(FlightGlobals.ActiveVessel.id);
        VesselsWindow._activeVesselDisplayStore.Update(FlightGlobals.ActiveVessel);
      }
      else
        VesselsWindow._activeVesselDisplayStore = (VesselDisplay) null;
      List<Guid> list = VesselsWindow.VesselDisplayStore.Keys.Except<Guid>(((IEnumerable<Vessel>) FlightGlobals.Vessels).Select<Vessel, Guid>((Func<Vessel, Guid>) (v => v.id))).ToList<Guid>();
      for (int index = 0; index < FlightGlobals.Vessels.Count; ++index)
      {
        Vessel vessel = FlightGlobals.Vessels[index];
        if (Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) vessel) || !VesselFilter.MatchesFilters(vessel))
        {
          list.Add(vessel.id);
        }
        else
        {
          if (!VesselsWindow.VesselDisplayStore.ContainsKey(vessel.id))
            VesselsWindow.VesselDisplayStore.Add(vessel.id, new VesselDisplay(vessel.id));
          VesselsWindow.VesselDisplayStore[vessel.id].Update(vessel);
        }
      }
      foreach (Guid key in list)
        VesselsWindow.VesselDisplayStore.Remove(key);
    }

    protected override void DrawGui()
    {
      GUI.skin = StyleLibrary.DefaultSkin;
      // ISSUE: method pointer
      this.WindowRect = this.FixWindowPos(GUILayout.Window(1664154329, this.WindowRect, new GUI.WindowFunction((object) this, __methodptr(DrawContent)), "Vessels", StyleLibrary.Skin.window, this.LayoutOptions));
    }

    public override void SetStyles()
    {
      this.WindowRect = new Rect((float) Screen.width - 650f, (float) ((double) Screen.height / 2.0 - 250.0), 600f, 500f);
      this.MoveRect = new Rect(0.0f, 0.0f, (float) int.MaxValue, 30f);
      this.LayoutOptions = new GUILayoutOption[4];
      this.LayoutOptions[0] = GUILayout.MinWidth(600f);
      this.LayoutOptions[1] = GUILayout.MaxWidth(600f);
      this.LayoutOptions[2] = GUILayout.MinHeight(500f);
      this.LayoutOptions[3] = GUILayout.MaxHeight(500f);
      this.TextAreaOptions = new GUILayoutOption[1];
      this.TextAreaOptions[0] = GUILayout.ExpandWidth(true);
      VesselBaseDisplay.SetStyles();
    }

    public override void RemoveWindowLock()
    {
      if (!this.IsWindowLocked)
        return;
      this.IsWindowLocked = false;
      InputLockManager.RemoveControlLock("LMP_VesselsWindowsLock");
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
          InputLockManager.SetControlLock((ControlTypes) 900719925474097919L, "LMP_VesselsWindowsLock");
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
