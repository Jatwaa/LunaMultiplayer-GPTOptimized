// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KerbalSys.KerbalEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Localization;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpClient.VesselUtilities;
using UnityEngine;

namespace LmpClient.Systems.KerbalSys
{
  public class KerbalEvents : SubSystem<KerbalSystem>
  {
    public void ValidationBeforeAssembly(bool validationResult)
    {
      VesselCrewManifest newShipManifest = FlightDriver.newShipManifest;
      if (newShipManifest == null)
        return;
      foreach (ProtoCrewMember protoCrewMember in newShipManifest.GetAllCrew(false))
      {
        if (protoCrewMember != null)
        {
          if (validationResult)
          {
            SubSystem<KerbalSystem>.System.SetKerbalStatusWithoutTriggeringEvent(protoCrewMember, (ProtoCrewMember.RosterStatus) 1);
            SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(protoCrewMember);
            System<LockSystem>.Singleton.AcquireKerbalLock(protoCrewMember.name, true);
          }
          else
          {
            SubSystem<KerbalSystem>.System.SetKerbalStatusWithoutTriggeringEvent(protoCrewMember, (ProtoCrewMember.RosterStatus) 0);
            SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(protoCrewMember);
          }
        }
      }
    }

    public void StatusChange(
      ProtoCrewMember kerbal,
      ProtoCrewMember.RosterStatus previousStatus,
      ProtoCrewMember.RosterStatus newStatus)
    {
      if (previousStatus == newStatus)
        return;
      if (LockSystem.LockQuery.KerbalLockExists(kerbal.name) && !LockSystem.LockQuery.KerbalLockBelongsToPlayer(kerbal.name, SettingsSystem.CurrentSettings.PlayerName))
      {
        SubSystem<KerbalSystem>.System.SetKerbalStatusWithoutTriggeringEvent(kerbal, previousStatus);
      }
      else
      {
        SubSystem<KerbalSystem>.System.SetKerbalStatusWithoutTriggeringEvent(kerbal, newStatus);
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(kerbal);
        SubSystem<KerbalSystem>.System.RefreshCrewDialog();
      }
    }

    public void TypeChange(
      ProtoCrewMember kerbal,
      ProtoCrewMember.KerbalType previousType,
      ProtoCrewMember.KerbalType newType)
    {
      if (previousType == newType)
        return;
      if (LockSystem.LockQuery.KerbalLockExists(kerbal.name) && !LockSystem.LockQuery.KerbalLockBelongsToPlayer(kerbal.name, SettingsSystem.CurrentSettings.PlayerName))
      {
        LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.KerbalNotYours, 5f, (ScreenMessageStyle) 0);
        SubSystem<KerbalSystem>.System.SetKerbalTypeWithoutTriggeringEvent(kerbal, (ProtoCrewMember.KerbalType) 0);
      }
      else
      {
        SubSystem<KerbalSystem>.System.SetKerbalTypeWithoutTriggeringEvent(kerbal, newType);
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(kerbal);
        SubSystem<KerbalSystem>.System.RefreshCrewDialog();
      }
    }

    public void KerbalLevelUp(ProtoCrewMember kerbal)
    {
      if (LockSystem.LockQuery.KerbalLockExists(kerbal.name) && !LockSystem.LockQuery.KerbalLockBelongsToPlayer(kerbal.name, SettingsSystem.CurrentSettings.PlayerName))
        return;
      SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(kerbal);
      SubSystem<KerbalSystem>.System.RefreshCrewDialog();
    }

    public void ReturningToEditor(EditorFacility data)
    {
      if (Object.op_Equality((Object) FlightGlobals.ActiveVessel, (Object) null) || VesselCommon.IsSpectating)
        return;
      foreach (ProtoCrewMember protoCrewMember in FlightGlobals.ActiveVessel.GetVesselCrew())
      {
        SubSystem<KerbalSystem>.System.SetKerbalStatusWithoutTriggeringEvent(protoCrewMember, (ProtoCrewMember.RosterStatus) 0);
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(protoCrewMember);
      }
    }

    public void OnVesselTerminated(ProtoVessel terminatedVessel)
    {
      if (terminatedVessel == null)
        return;
      foreach (ProtoCrewMember protoCrewMember in terminatedVessel.GetVesselCrew())
      {
        System<KerbalSystem>.Singleton.SetKerbalStatusWithoutTriggeringEvent(protoCrewMember, (ProtoCrewMember.RosterStatus) 3);
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(protoCrewMember);
        System<LockSystem>.Singleton.ReleaseKerbalLock(protoCrewMember.name, 1f);
      }
    }

    public void OnVesselRecovered(ProtoVessel recoveredVessel)
    {
      if (recoveredVessel == null)
        return;
      foreach (ProtoCrewMember protoCrewMember in recoveredVessel.GetVesselCrew())
      {
        System<KerbalSystem>.Singleton.SetKerbalStatusWithoutTriggeringEvent(protoCrewMember, (ProtoCrewMember.RosterStatus) 0);
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(protoCrewMember);
        System<LockSystem>.Singleton.ReleaseKerbalLock(protoCrewMember.name, 1f);
      }
    }

    public void OnVesselWillDestroy(Vessel dyingVessel)
    {
      if (Object.op_Equality((Object) dyingVessel, (Object) null))
        return;
      foreach (ProtoCrewMember pcm in dyingVessel.GetVesselCrew())
      {
        SubSystem<KerbalSystem>.System.MessageSender.SendKerbal(pcm);
        System<LockSystem>.Singleton.ReleaseKerbalLock(pcm.name, 0.5f);
      }
    }

    public void OnVesselLoaded(Vessel data)
    {
      if (Object.op_Inequality((Object) SubSystem<KerbalSystem>.System.AstronautComplex, (Object) null))
        SubSystem<KerbalSystem>.System.RefreshCrewDialog();
      HighLogic.CurrentGame.Updated();
    }
  }
}
