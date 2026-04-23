// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightResultsDialog_SetupGUI
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Dialogs;
using LmpClient.Base;
using LmpClient.Systems.Revert;
using LmpClient.VesselUtilities;
using LmpCommon.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightResultsDialog))]
  [HarmonyPatch("SetupGUI")]
  public class FlightResultsDialog_SetupGUI
  {
    [HarmonyPostfix]
    private static void PostfixSetupGUI(Button ___Btn_revLaunch, Button ___Btn_revEditor)
    {
      if (MainSystem.NetworkState < ClientState.Connected || Object.op_Implicit((Object) FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.id == System<RevertSystem>.Singleton.StartingVesselId && !VesselCommon.IsSpectating)
        return;
      ((UnityEventBase) ___Btn_revLaunch.onClick).RemoveAllListeners();
      ((UnityEventBase) ___Btn_revEditor.onClick).RemoveAllListeners();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ((UnityEvent) ___Btn_revLaunch.onClick).AddListener(FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9__0_0 = new UnityAction((object) FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CPostfixSetupGUI\u003Eb__0_0))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ((UnityEvent) ___Btn_revEditor.onClick).AddListener(FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9__0_1 ?? (FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9__0_1 = new UnityAction((object) FlightResultsDialog_SetupGUI.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CPostfixSetupGUI\u003Eb__0_1))));
    }
  }
}
