// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KscScene.KscSceneEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using KSP.UI;
using KSP.UI.Screens;
using LmpClient.Base;
using LmpClient.Systems.VesselUpdateSys;
using LmpCommon.Locks;
using System.Reflection;
using UnityEngine;

namespace LmpClient.Systems.KscScene
{
  public class KscSceneEvents : SubSystem<KscSceneSystem>
  {
    private static readonly MethodInfo ClearVesselMarkers = typeof (KSCVesselMarkers).GetMethod(nameof (ClearVesselMarkers), AccessTools.all);

    public void OnLockAcquire(LockDefinition lockdefinition)
    {
      SubSystem<KscSceneSystem>.System.RefreshTrackingStationVessels();
      KscSceneEvents.RefreshMarkers();
    }

    public void OnLockRelease(LockDefinition lockdefinition)
    {
      SubSystem<KscSceneSystem>.System.RefreshTrackingStationVessels();
      KscSceneEvents.RefreshMarkers();
    }

    public void OnSceneRequested(GameScenes requestedScene)
    {
      if (requestedScene <= 5)
        return;
      KscSceneEvents.ClearMarkers();
    }

    public void LevelLoaded(GameScenes data)
    {
      if (data != 5)
        return;
      KscSceneEvents.ClearMarkers();
      KscSceneEvents.RefreshMarkers();
    }

    public void OnVesselCreated(Vessel vessel)
    {
      SubSystem<KscSceneSystem>.System.RefreshTrackingStationVessels();
      KscSceneEvents.RefreshMarkers();
    }

    public void OnVesselRename(GameEvents.HostedFromToAction<Vessel, string> pair)
    {
      if (HighLogic.LoadedScene != 8)
        return;
      ((Object) pair.host).name = pair.to;
      new VesselUpdateMessageSender().SendVesselUpdate(pair.host);
    }

    public void VesselInitialized(Vessel vessel, bool fromShipAssembly)
    {
      SubSystem<KscSceneSystem>.System.RefreshTrackingStationVessels();
      KscSceneEvents.RefreshMarkers();
    }

    private static void ClearMarkers()
    {
      if (Object.op_Implicit((Object) KSCVesselMarkers.fetch))
        KscSceneEvents.ClearVesselMarkers?.Invoke((object) KSCVesselMarkers.fetch, (object[]) null);
      foreach (KSCVesselMarker kscVesselMarker in Object.FindObjectsOfType<KSCVesselMarker>())
      {
        ((AnchoredDialog) kscVesselMarker).Terminate();
        Object.DestroyImmediate((Object) kscVesselMarker);
      }
    }

    private static void RefreshMarkers()
    {
      if (!Object.op_Implicit((Object) KSCVesselMarkers.fetch) || HighLogic.LoadedScene != 5)
        return;
      KSCVesselMarkers.fetch.RefreshMarkers();
    }
  }
}
