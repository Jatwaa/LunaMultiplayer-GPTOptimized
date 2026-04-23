// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.ShareExperimentalParts.ShareExperimentalPartsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Events;
using LmpClient.Systems.ShareProgress;
using LmpCommon.Enums;

namespace LmpClient.Systems.ShareExperimentalParts
{
  public class ShareExperimentalPartsSystem : 
    ShareProgressBaseSystem<ShareExperimentalPartsSystem, ShareExperimentalPartsMessageSender, ShareExperimentalPartsMessageHandler>
  {
    public override string SystemName { get; } = nameof (ShareExperimentalPartsSystem);

    private ShareExperimentalPartsEvents ShareExperimentalPartsEvents { get; } = new ShareExperimentalPartsEvents();

    protected override bool ShareSystemReady => true;

    protected override GameMode RelevantGameModes => GameMode.Career;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.CurrentGameModeIsRelevant)
        return;
      // ISSUE: method pointer
      ExperimentalPartEvent.onExperimentalPartRemoved.Add(new EventData<AvailablePart, int>.OnEvent((object) this.ShareExperimentalPartsEvents, __methodptr(ExperimentalPartRemoved)));
      // ISSUE: method pointer
      ExperimentalPartEvent.onExperimentalPartAdded.Add(new EventData<AvailablePart, int>.OnEvent((object) this.ShareExperimentalPartsEvents, __methodptr(ExperimentalPartAdded)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      ExperimentalPartEvent.onExperimentalPartRemoved.Remove(new EventData<AvailablePart, int>.OnEvent((object) this.ShareExperimentalPartsEvents, __methodptr(ExperimentalPartRemoved)));
      // ISSUE: method pointer
      ExperimentalPartEvent.onExperimentalPartAdded.Remove(new EventData<AvailablePart, int>.OnEvent((object) this.ShareExperimentalPartsEvents, __methodptr(ExperimentalPartAdded)));
    }
  }
}
