// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncCallSys.VesselPartSyncCallEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Extensions;
using UnityEngine;

namespace LmpClient.Systems.VesselPartSyncCallSys
{
  public class VesselPartSyncCallEvents : SubSystem<VesselPartSyncCallSystem>
  {
    private static bool CallIsValid(PartModule module)
    {
      Vessel vessel = module.vessel;
      if (Object.op_Equality((Object) vessel, (Object) null) || !vessel.loaded || vessel.protoVessel == null)
        return false;
      Part part = module.part;
      return !Object.op_Equality((Object) part, (Object) null) && !part.vessel.IsImmortal();
    }

    public void PartModuleMethodCalled(PartModule module, string methodName)
    {
      if (!VesselPartSyncCallEvents.CallIsValid(module))
        return;
      LunaLog.Log(string.Format("Part sync method {0} in module {1} from part {2} was called.", (object) methodName, (object) module.moduleName, (object) module.part.flightID));
      SubSystem<VesselPartSyncCallSystem>.System.MessageSender.SendVesselPartSyncCallMsg(module.vessel, module.part, module.moduleName, methodName);
    }
  }
}
