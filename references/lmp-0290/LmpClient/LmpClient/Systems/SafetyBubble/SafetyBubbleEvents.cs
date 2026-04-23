// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SafetyBubble.SafetyBubbleEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;
using LmpClient.VesselUtilities;
using System;
using UnityEngine;

namespace LmpClient.Systems.SafetyBubble
{
  public class SafetyBubbleEvents : SubSystem<SafetyBubbleSystem>
  {
    public void FlightReady()
    {
      if (VesselCommon.IsSpectating || Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null) || !FlightGlobals.ActiveVessel.vesselSpawning || (double) SettingsSystem.ServerSettings.SafetyBubbleDistance <= 0.0)
        return;
      if (SubSystem<SafetyBubbleSystem>.System.IsInSafetyBubble(FlightGlobals.ActiveVessel) && FlightGlobals.ActiveVessel.situation == 4)
        SubSystem<SafetyBubbleSystem>.System.DrawSafetyBubble();
      if (!FlightGlobals.ActiveVessel.vesselSpawning)
        return;
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.SafetyBubble, 10f, (ScreenMessageStyle) 0);
      CoroutineUtil.StartDelayedRoutine(nameof (SafetyBubbleEvents), (Action) (() => LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CheckParts, 15f, (ScreenMessageStyle) 0, Color.red)), 25f);
    }
  }
}
