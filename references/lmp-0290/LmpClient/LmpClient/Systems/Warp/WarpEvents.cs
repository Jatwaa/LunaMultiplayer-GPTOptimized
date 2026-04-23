// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.WarpEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.VesselUtilities;

namespace LmpClient.Systems.Warp
{
  public class WarpEvents : SubSystem<WarpSystem>
  {
    public void OnTimeWarpChanged()
    {
      if (TimeWarp.CurrentRateIndex <= 0)
        return;
      if (!SubSystem<WarpSystem>.System.WarpValidation() || VesselCommon.IsSpectating)
        TimeWarp.SetRate(0, true, true);
      if (SubSystem<WarpSystem>.System.CurrentSubspace != -1)
        SubSystem<WarpSystem>.System.CurrentSubspace = -1;
    }

    public void OnSceneChanged(GameScenes data)
    {
      if (!SubSystem<WarpSystem>.System.Enabled || SubSystem<WarpSystem>.System.SyncedToLastSubspace || !HighLogic.LoadedSceneIsGame)
        return;
      SubSystem<WarpSystem>.System.CurrentSubspace = SubSystem<WarpSystem>.System.LatestSubspace;
      SubSystem<WarpSystem>.System.SyncedToLastSubspace = true;
      SubSystem<WarpSystem>.System.ProcessNewSubspace();
    }
  }
}
