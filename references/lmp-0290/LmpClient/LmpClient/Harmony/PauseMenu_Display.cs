// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.PauseMenu_Display
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (PauseMenu))]
  [HarmonyPatch("Display")]
  public class PauseMenu_Display
  {
    [HarmonyPostfix]
    private static void PostfixDisplay()
    {
      if (MainSystem.NetworkState < ClientState.Connected || !VesselCommon.IsSpectating || !PauseMenu.exists || !PauseMenu.isOpen)
        return;
      PauseMenu.canSaveAndExit = (ClearToSaveStatus) 0;
    }
  }
}
