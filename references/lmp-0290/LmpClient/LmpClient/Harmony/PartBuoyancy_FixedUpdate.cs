// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.PartBuoyancy_FixedUpdate
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
  [HarmonyPatch(typeof (PartBuoyancy))]
  [HarmonyPatch("FixedUpdate")]
  public class PartBuoyancy_FixedUpdate
  {
    [HarmonyPrefix]
    private static bool PrefixCheckPartG(Part ___part) => MainSystem.NetworkState < ClientState.Connected || !Object.op_Implicit((Object) ___part) || !Object.op_Implicit((Object) ___part.vessel) || !___part.vessel.IsImmortal() && !System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(___part.vessel.id);
  }
}
