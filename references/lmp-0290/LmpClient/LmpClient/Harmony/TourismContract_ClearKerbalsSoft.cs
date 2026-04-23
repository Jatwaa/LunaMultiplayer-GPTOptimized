// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.TourismContract_ClearKerbalsSoft
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using FinePrint.Contracts;
using HarmonyLib;
using LmpClient.Base;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.ShareContracts;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (TourismContract))]
  [HarmonyPatch("ClearKerbalsSoft")]
  public class TourismContract_ClearKerbalsSoft
  {
    [HarmonyPostfix]
    private static void PostfixClearKerbalsSoft(TourismContract __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected || System<ShareContractsSystem>.Singleton.IgnoreEvents)
        return;
      foreach (string tourist in __instance.Tourists)
      {
        if (!HighLogic.CurrentGame.CrewRoster.Exists(tourist))
          System<KerbalSystem>.Singleton.MessageSender.SendKerbalRemove(tourist);
      }
    }
  }
}
