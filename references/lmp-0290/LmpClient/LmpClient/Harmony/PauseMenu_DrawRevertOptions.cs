// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.PauseMenu_DrawRevertOptions
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Localization;
using LmpClient.Systems.Revert;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (PauseMenu))]
  [HarmonyPatch("drawStockRevertOptions")]
  public class PauseMenu_DrawRevertOptions
  {
    [HarmonyPostfix]
    private static void PostfixDrawStockRevertOptions(
      PopupDialog dialog,
      List<DialogGUIBase> options)
    {
      if (MainSystem.NetworkState < ClientState.Connected || Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == LmpClient.Base.System<RevertSystem>.Singleton.StartingVesselId && !VesselCommon.IsSpectating)
        return;
      foreach (DialogGUIBase option in options)
      {
        if (option is DialogGUILabel dialogGuiLabel)
          ((DialogGUIBase) dialogGuiLabel).OptionText = LocalizationContainer.RevertDialogText.CannotRevertText;
        if (option is DialogGUIButton dialogGuiButton)
          ((DialogGUIBase) dialogGuiButton).OptionInteractableCondition = (Func<bool>) (() => false);
      }
    }
  }
}
