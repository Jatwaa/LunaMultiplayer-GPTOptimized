// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ShipConstruction_FindVesselsLandedAt
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Systems.Lock;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ShipConstruction))]
  [HarmonyPatch("FindVesselsLandedAt")]
  [HarmonyPatch(new Type[] {typeof (FlightState), typeof (string)})]
  public class ShipConstruction_FindVesselsLandedAt
  {
    private static readonly List<ProtoVessel> ProtoVesselsToRemove = new List<ProtoVessel>();

    [HarmonyPostfix]
    private static void PostfixFindVesselsLandedAt(List<ProtoVessel> __result)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      __result.RemoveAll((Predicate<ProtoVessel>) (pv => LockSystem.LockQuery.ControlLockExists(pv.vesselID)));
    }
  }
}
