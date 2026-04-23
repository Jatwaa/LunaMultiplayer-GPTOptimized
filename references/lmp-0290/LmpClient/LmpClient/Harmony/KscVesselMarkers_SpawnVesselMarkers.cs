// Decompiled with JetBrains decompiler
// Type: LmpClient.Harmony.KscVesselMarkers_SpawnVesselMarkers
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI;
using KSP.UI.Screens;
using LmpClient.Systems.Lock;
using LmpClient.Systems.VesselRemoveSys;
using LmpCommon.Enums;
using System.Collections.Generic;

namespace LmpClient.Harmony
{
  [HarmonyPatch(typeof (KSCVesselMarkers))]
  [HarmonyPatch("SpawnVesselMarkers")]
  public class KscVesselMarkers_SpawnVesselMarkers
  {
    private static readonly List<KSCVesselMarker> MarkersToRemove = new List<KSCVesselMarker>();

    [HarmonyPostfix]
    private static void PostfixVesselMarkers()
    {
      if (MainSystem.NetworkState < ClientState.Connected)
        return;
      KscVesselMarkers_SpawnVesselMarkers.MarkersToRemove.Clear();
      List<KSCVesselMarker> kscVesselMarkerList = Traverse.Create((object) KSCVesselMarkers.fetch).Field("markers").GetValue<List<KSCVesselMarker>>();
      foreach (KSCVesselMarker kscVesselMarker in kscVesselMarkerList)
      {
        Vessel vessel = Traverse.Create((object) kscVesselMarker).Field("v").GetValue<Vessel>();
        if (LockSystem.LockQuery.ControlLockExists(vessel.id) || LmpClient.Base.System<VesselRemoveSystem>.Singleton.VesselWillBeKilled(vessel.id))
          KscVesselMarkers_SpawnVesselMarkers.MarkersToRemove.Add(kscVesselMarker);
      }
      foreach (KSCVesselMarker kscVesselMarker in KscVesselMarkers_SpawnVesselMarkers.MarkersToRemove)
      {
        kscVesselMarkerList.Remove(kscVesselMarker);
        ((AnchoredDialog) kscVesselMarker).Terminate();
      }
      InputLockManager.ClearControlLocks();
    }
  }
}
