// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.AsteroidComet.AsteroidCometEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Extensions;
using LmpClient.Systems.VesselProtoSys;
using LmpCommon.Locks;

namespace LmpClient.Systems.AsteroidComet
{
  public class AsteroidCometEvents : SubSystem<AsteroidCometSystem>
  {
    public void LockReleased(LockDefinition lockDefinition)
    {
      if (lockDefinition.Type != LockType.AsteroidComet)
        return;
      SubSystem<AsteroidCometSystem>.System.TryGetCometAsteroidLock();
    }

    public void LevelLoaded(GameScenes data) => SubSystem<AsteroidCometSystem>.System.TryGetCometAsteroidLock();

    public void StartTrackingCometOrAsteroid(Vessel potato)
    {
      LunaLog.Log(string.Format("Started to track comet/asteroid {0}", (object) potato.id));
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(potato, true);
    }

    public void StopTrackingCometOrAsteroid(Vessel potato)
    {
      LunaLog.Log(string.Format("Stopped to track comet/asteroid {0}", (object) potato.id));
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(potato, true);
    }

    public void NewVesselCreated(Vessel vessel)
    {
      if (!vessel.IsCometOrAsteroid())
        return;
      System<VesselProtoSystem>.Singleton.MessageSender.SendVesselMessage(vessel, true);
    }
  }
}
