// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.ContractSystem_OnAwake
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Contracts;
using HarmonyLib;
using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.Systems.ShareContracts;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (ContractSystem))]
  [HarmonyPatch("OnAwake")]
  public class ContractSystem_OnAwake
  {
    [HarmonyPostfix]
    private static void PostFixConstructor(ContractSystem __instance)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      if (System<ShareContractsSystem>.Singleton.DefaultContractGenerateIterations == 0)
        System<ShareContractsSystem>.Singleton.DefaultContractGenerateIterations = ContractSystem.generateContractIterations;
      ContractSystem.generateContractIterations = LockSystem.LockQuery.ContractLockBelongsToPlayer(SettingsSystem.CurrentSettings.PlayerName) ? System<ShareContractsSystem>.Singleton.DefaultContractGenerateIterations : 0;
    }
  }
}
