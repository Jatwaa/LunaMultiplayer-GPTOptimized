// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.KerbalRoster_SackAvailable
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.KerbalSys;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (KerbalRoster))]
  [HarmonyPatch("SackAvailable")]
  public class KerbalRoster_SackAvailable
  {
    [HarmonyPrefix]
    private static bool PrefixSackAvailable(KerbalRoster __instance, ProtoCrewMember ap)
    {
      if (MainSystem.NetworkState < ClientState.Connected || SettingsSystem.ServerSettings.AllowSackKerbals)
        return true;
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.SackingKerbalsNotAllowed, 10f, (ScreenMessageStyle) 0);
      System<KerbalSystem>.Singleton.RefreshCrewDialog();
      return false;
    }
  }
}
