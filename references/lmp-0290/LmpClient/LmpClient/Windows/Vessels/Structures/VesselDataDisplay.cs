// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselDataDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpClient.Systems.VesselImmortalSys;
using System;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal class VesselDataDisplay : VesselBaseDisplay
  {
    public override bool Display { get; set; }

    public Guid VesselId { get; set; }

    public Vessel Vessel { get; set; }

    public VesselDataDisplay(Guid vesselId) => this.VesselId = vesselId;

    protected override void UpdateDisplay(Vessel vessel)
    {
      this.VesselId = vessel.id;
      this.Vessel = vessel;
    }

    protected override void PrintDisplay()
    {
      if (!Object.op_Implicit((Object) this.Vessel))
        return;
      GUILayout.Label(string.Format("Stage: {0}", (object) this.Vessel.currentStage), Array.Empty<GUILayoutOption>());
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("Immortal: {0}", (object) this.Vessel.IsImmortal()), Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (this.Vessel.IsImmortal())
      {
        if (GUILayout.Button("Make mortal", Array.Empty<GUILayoutOption>()))
          this.Vessel.SetImmortal(false);
      }
      else if (GUILayout.Button("Make immortal", Array.Empty<GUILayoutOption>()))
        this.Vessel.SetImmortal(true);
      if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
        LmpClient.Base.System<VesselImmortalSystem>.Singleton.SetImmortalStateBasedOnLock(this.Vessel);
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("Loaded: {0}", (object) this.Vessel.loaded), Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (this.Vessel.loaded)
      {
        if (GUILayout.Button("Unload", Array.Empty<GUILayoutOption>()))
          FlightGlobals.FindVessel(this.VesselId).vesselRanges = VesselDataDisplay.UnloadRanges;
      }
      else if (GUILayout.Button("Load", Array.Empty<GUILayoutOption>()))
        FlightGlobals.FindVessel(this.VesselId).vesselRanges = VesselDataDisplay.LoadRanges;
      if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
        FlightGlobals.FindVessel(this.VesselId).vesselRanges = PhysicsGlobals.Instance.VesselRangesDefault;
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
      GUILayout.Label(string.Format("Packed: {0}", (object) this.Vessel.packed), Array.Empty<GUILayoutOption>());
      GUILayout.FlexibleSpace();
      if (this.Vessel.packed)
      {
        if (GUILayout.Button("Unpack", Array.Empty<GUILayoutOption>()))
          FlightGlobals.FindVessel(this.VesselId).vesselRanges = VesselDataDisplay.UnPackRanges;
      }
      else if (GUILayout.Button("Pack", Array.Empty<GUILayoutOption>()))
        FlightGlobals.FindVessel(this.VesselId).vesselRanges = VesselDataDisplay.PackRanges;
      if (GUILayout.Button("Reset", Array.Empty<GUILayoutOption>()))
        FlightGlobals.FindVessel(this.VesselId).vesselRanges = PhysicsGlobals.Instance.VesselRangesDefault;
      GUILayout.EndHorizontal();
    }

    public static VesselRanges PackRanges { get; } = new VesselRanges()
    {
      escaping = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.escaping)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      flying = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.flying)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      landed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.landed)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      orbit = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      prelaunch = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      splashed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      },
      subOrbital = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = 0.0f,
        unpack = (float) int.MaxValue
      }
    };

    public static VesselRanges UnPackRanges { get; } = new VesselRanges()
    {
      escaping = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.escaping)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      flying = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.flying)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      landed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.landed)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      orbit = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      prelaunch = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      splashed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      },
      subOrbital = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        pack = (float) int.MaxValue,
        unpack = 0.0f
      }
    };

    public static VesselRanges LoadRanges { get; } = new VesselRanges()
    {
      escaping = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.escaping)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      flying = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.flying)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      landed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.landed)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      orbit = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      prelaunch = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      splashed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      },
      subOrbital = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = 0.0f,
        unload = (float) int.MaxValue
      }
    };

    public static VesselRanges UnloadRanges { get; } = new VesselRanges()
    {
      escaping = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.escaping)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      flying = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.flying)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      landed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.landed)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      orbit = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      prelaunch = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      splashed = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      },
      subOrbital = new VesselRanges.Situation(PhysicsGlobals.Instance.VesselRangesDefault.orbit)
      {
        load = (float) int.MaxValue,
        unload = 0.0f
      }
    };
  }
}
