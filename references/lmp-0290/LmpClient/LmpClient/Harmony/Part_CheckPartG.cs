// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.Part_CheckPartG
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
  [HarmonyPatch(typeof (Part))]
  [HarmonyPatch("_CheckPartG")]
  public class Part_CheckPartG
  {
    [HarmonyPrefix]
    private static bool PrefixCheckPartG(Part p) => MainSystem.NetworkState < ClientState.Connected || !Object.op_Implicit((Object) p.vessel) || !p.vessel.IsImmortal() && !System<VesselPositionSystem>.Singleton.VesselHavePositionUpdatesQueued(p.vessel.id);
  }
}
