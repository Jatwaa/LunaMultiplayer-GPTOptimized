// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.FlagPlant.FlagPlantEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.VesselProtoSys;
using System;

namespace LmpClient.Systems.FlagPlant
{
  public class FlagPlantEvents : SubSystem<FlagPlantSystem>
  {
    public void AfterFlagPlanted(FlagSite data)
    {
      if (((PartModule) data).vessel.id == Guid.Empty)
        ((PartModule) data).vessel.id = Guid.NewGuid();
      LmpClient.Base.System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(((PartModule) data).vessel, true);
    }
  }
}
