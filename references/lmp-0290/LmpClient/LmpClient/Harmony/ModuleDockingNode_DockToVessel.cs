// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ModuleDockingNode_DockToVessel
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ModuleDockingNode))]
  [HarmonyPatch("DockToVessel")]
  public class ModuleDockingNode_DockToVessel
  {
    [HarmonyPrefix]
    private static void PrefixDockToVessel(ModuleDockingNode __instance, ModuleDockingNode node) => VesselDockEvent.onDocking.Fire(((PartModule) __instance).vessel, ((PartModule) node).vessel);

    [HarmonyPostfix]
    private static void PostfixDockToVessel(ModuleDockingNode __instance, ModuleDockingNode node) => VesselDockEvent.onDockingComplete.Fire(((PartModule) __instance).vessel, ((PartModule) node).vessel);
  }
}
