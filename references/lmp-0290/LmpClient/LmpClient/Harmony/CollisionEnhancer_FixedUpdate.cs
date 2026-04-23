// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.CollisionEnhancer_FixedUpdate
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
  [HarmonyPatch(typeof (CollisionEnhancer))]
  [HarmonyPatch("FixedUpdate")]
  public class CollisionEnhancer_FixedUpdate
  {
    [HarmonyPrefix]
    private static bool PrefixFixedUpdate(CollisionEnhancer __instance) => MainSystem.NetworkState < ClientState.Connected || !Object.op_Implicit((Object) __instance.part) || !Object.op_Implicit((Object) __instance.part.vessel) || !__instance.part.vessel.IsImmortal() && !System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(__instance.part.vessel.id);
  }
}
