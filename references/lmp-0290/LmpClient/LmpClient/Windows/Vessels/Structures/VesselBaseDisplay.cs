// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Vessels.Structures.VesselBaseDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Text;
using UnityEngine;

namespace LmpClient.Windows.Vessels.Structures
{
  internal abstract class VesselBaseDisplay
  {
    protected static GUIStyle ButtonStyle;
    protected static readonly StringBuilder StringBuilder = new StringBuilder();

    public static void SetStyles() => VesselBaseDisplay.ButtonStyle = new GUIStyle(GUI.skin.button);

    public void Print()
    {
      if (!this.Display)
        return;
      this.PrintDisplay();
    }

    public void Update(Vessel vessel)
    {
      if (!this.Display || !Object.op_Implicit((Object) vessel))
        return;
      this.UpdateDisplay(vessel);
    }

    protected abstract void PrintDisplay();

    protected abstract void UpdateDisplay(Vessel vessel);

    public abstract bool Display { get; set; }
  }
}
