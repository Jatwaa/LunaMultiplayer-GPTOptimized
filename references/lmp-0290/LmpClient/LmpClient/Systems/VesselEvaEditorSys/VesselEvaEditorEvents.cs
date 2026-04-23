// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselEvaEditorSys.VesselEvaEditorEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.Lock;
using LmpClient.Systems.VesselProtoSys;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.VesselUtilities;
using UnityEngine;

namespace LmpClient.Systems.VesselEvaEditorSys
{
  public class VesselEvaEditorEvents : SubSystem<VesselEvaEditorSystem>
  {
    public void EVAConstructionModePartAttached(Vessel vessel, Part part)
    {
      if (VesselCommon.IsSpectating)
        return;
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel);
    }

    public void EVAConstructionModePartDetached(Vessel vessel, Part part)
    {
      if (VesselCommon.IsSpectating)
        return;
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel);
    }

    public void VesselCreated(Vessel vessel)
    {
      if (!SubSystem<VesselEvaEditorSystem>.System.DetachingPart)
        return;
      System<LockSystem>.Singleton.AcquireUpdateLock(vessel.id, true, true);
      System<LockSystem>.Singleton.AcquireUnloadedUpdateLock(vessel.id, true, true);
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel);
    }

    public void OnDroppingPart() => SubSystem<VesselEvaEditorSystem>.System.DetachingPart = true;

    public void OnDroppedPart() => SubSystem<VesselEvaEditorSystem>.System.DetachingPart = false;

    public void OnAttachingPart(Part part)
    {
      if (!Object.op_Implicit((Object) part.vessel))
        return;
      System<VesselRemoveSystem>.Singleton.MessageSender.SendVesselRemove(part.vessel);
    }
  }
}
