// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.QuickSaveLoad_QuickLoad
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Localization;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (QuickSaveLoad))]
  [HarmonyPatch("quickLoad")]
  public class QuickSaveLoad_QuickLoad
  {
    [HarmonyPrefix]
    private static bool PrefixQuickLoad()
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return true;
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.CannotLoadGames, 5f, (ScreenMessageStyle) 0);
      return false;
    }
  }
}
