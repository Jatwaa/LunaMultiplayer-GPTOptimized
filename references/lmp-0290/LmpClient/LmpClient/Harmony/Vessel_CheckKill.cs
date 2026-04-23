// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Vessel_CheckKill
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.Systems.VesselPositionSys;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (Vessel))]
  [HarmonyPatch("CheckKill")]
  public class Vessel_CheckKill
  {
    [HarmonyPrefix]
    private static bool PrefixCheckKill(Vessel __instance) => MainSystem.NetworkState < ClientState.Connected || !Object.op_Implicit((Object) __instance) || !__instance.IsImmortal() && !System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(__instance.id);
  }
}
