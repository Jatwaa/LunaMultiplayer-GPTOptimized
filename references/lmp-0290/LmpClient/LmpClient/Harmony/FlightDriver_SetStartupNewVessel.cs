// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.FlightDriver_SetStartupNewVessel
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.Mod;
using LmpClient.Systems.SettingsSys;
using LmpClient.Windows.BannedParts;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (FlightDriver))]
  [HarmonyPatch("setStartupNewVessel")]
  public class FlightDriver_SetStartupNewVessel
  {
    [HarmonyPrefix]
    private static bool PrefixSetStartupNewVessel()
    {
      if (MainSystem.NetworkState < ClientState.Connected || string.IsNullOrEmpty(FlightDriver.newShipToLoadPath))
        return true;
      ConfigNode configNode = ConfigNode.Load(FlightDriver.newShipToLoadPath);
      string vesselName = configNode.GetValue("ship");
      List<string> list1 = ((IEnumerable<ConfigNode>) configNode.GetNodes("PART")).Select<ConfigNode, string>((Func<ConfigNode, string>) (n => n.GetValue("part").Substring(0, n.GetValue("part").IndexOf('_')))).ToList<string>();
      List<string> list2 = ((IEnumerable<ConfigNode>) configNode.GetNodes("PART")).SelectMany<ConfigNode, string>((Func<ConfigNode, IEnumerable<string>>) (p => ((IEnumerable<ConfigNode>) p.GetNodes("RESOURCE")).Select<ConfigNode, string>((Func<ConfigNode, string>) (r => r.GetValue("name"))))).ToList<string>();
      int partCount = ((IEnumerable<ConfigNode>) configNode.GetNodes("PART")).Count<ConfigNode>();
      if (LmpClient.Base.System<ModSystem>.Singleton.ModControl)
      {
        string[] array1 = LmpClient.Base.System<ModSystem>.Singleton.GetBannedPartsFromPartNames(list1.Distinct<string>()).ToArray<string>();
        string[] array2 = LmpClient.Base.System<ModSystem>.Singleton.GetBannedResourcesFromResourceNames(list2.Distinct<string>()).ToArray<string>();
        if (((IEnumerable<string>) array1).Any<string>() || ((IEnumerable<string>) array2).Any<string>())
        {
          if (((IEnumerable<string>) array1).Any<string>())
            LunaLog.LogError("Vessel " + vesselName + " Contains the following banned parts: " + string.Join(", ", array1));
          if (((IEnumerable<string>) array2).Any<string>())
            LunaLog.LogError("Vessel " + vesselName + " Contains the following banned resources: " + string.Join(", ", array2));
          Window<BannedPartsResourcesWindow>.Singleton.DisplayBannedPartsResourcesDialog(vesselName, array1, array2);
          HighLogic.LoadScene((GameScenes) 5);
          VesselAssemblyEvent.onVesselValidationBeforAssembly.Fire(false);
          return false;
        }
      }
      if (partCount > SettingsSystem.ServerSettings.MaxVesselParts)
      {
        LunaLog.LogError(string.Format("Vessel {0} has {1} parts and the max allowed in the server is: {2}", (object) vesselName, (object) partCount, (object) SettingsSystem.ServerSettings.MaxVesselParts));
        Window<BannedPartsResourcesWindow>.Singleton.DisplayBannedPartsResourcesDialog(vesselName, new string[0], new string[0], partCount);
        HighLogic.LoadScene((GameScenes) 5);
        VesselAssemblyEvent.onVesselValidationBeforAssembly.Fire(false);
        return false;
      }
      VesselAssemblyEvent.onVesselValidationBeforAssembly.Fire(true);
      return true;
    }
  }
}
