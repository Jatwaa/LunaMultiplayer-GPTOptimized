// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.OrbitRendererBase_OnUpdateCaption
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI.Screens.Mapview;
using LmpClient.Events;
using LmpCommon.Enums;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (OrbitRendererBase))]
  [HarmonyPatch("objectNode_OnUpdateCaption")]
  public class OrbitRendererBase_OnUpdateCaption
  {
    [HarmonyPostfix]
    private static void PostOnUpdateCaption(
      OrbitRendererBase __instance,
      MapNode n,
      MapNode.CaptionData data)
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      LabelEvent.onMapLabelProcessed.Fire(__instance.vessel, data);
    }
  }
}
