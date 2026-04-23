// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.DestructibleBuilding_OnCollisionEnter
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Extensions;
using LmpCommon.Enums;
using UnityEngine;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (DestructibleBuilding))]
  [HarmonyPatch("OnCollisionEnter")]
  public class DestructibleBuilding_OnCollisionEnter
  {
    [HarmonyPrefix]
    private static bool PrefixOnCollisionEnter(Collision c)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return true;
      Vessel vessel = GameObjectExtension.GetComponentUpwards<Part>(c.gameObject)?.vessel;
      return !Object.op_Inequality((Object) vessel, (Object) null) || !vessel.IsImmortal();
    }
  }
}
