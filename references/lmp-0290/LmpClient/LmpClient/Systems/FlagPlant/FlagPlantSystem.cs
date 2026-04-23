// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.FlagPlant.FlagPlantSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.FlagPlant
{
  public class FlagPlantSystem : System<FlagPlantSystem>
  {
    private FlagPlantEvents FlagPlantEvents { get; } = new FlagPlantEvents();

    public override string SystemName { get; } = nameof (FlagPlantSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      // ISSUE: method pointer
      GameEvents.afterFlagPlanted.Add(new EventData<FlagSite>.OnEvent((object) this.FlagPlantEvents, __methodptr(AfterFlagPlanted)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.afterFlagPlanted.Remove(new EventData<FlagSite>.OnEvent((object) this.FlagPlantEvents, __methodptr(AfterFlagPlanted)));
    }
  }
}
