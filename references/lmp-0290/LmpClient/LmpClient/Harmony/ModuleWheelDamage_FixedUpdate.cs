// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Harmony.ModuleWheelDamage_FixedUpdate
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpCommon.Enums;
using ModuleWheels;

namespace LmpClient.ModuleStore.Harmony
{
  [HarmonyPatch(typeof (ModuleWheelDamage))]
  [HarmonyPatch("FixedUpdate")]
  public class ModuleWheelDamage_FixedUpdate
  {
    [HarmonyPrefix]
    private static bool PrefixFixedUpdate(ModuleWheelDamage __instance) => MainSystem.NetworkState < ClientState.Connected || (double) ((PartModule) __instance).part.crashTolerance != double.PositiveInfinity;
  }
}
