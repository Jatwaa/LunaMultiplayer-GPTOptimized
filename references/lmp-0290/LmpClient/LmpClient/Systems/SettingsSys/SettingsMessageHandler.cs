// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.SettingsMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using CommNet;
using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Settings;
using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Systems.SettingsSys
{
  public class SettingsMessageHandler : SubSystem<SettingsSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is SettingsReplyMsgData data))
        return;
      SettingsSystem.ServerSettings.WarpMode = data.WarpMode;
      SettingsSystem.ServerSettings.GameMode = data.GameMode;
      SettingsSystem.ServerSettings.TerrainQuality = data.TerrainQuality;
      SettingsSystem.ServerSettings.AllowCheats = data.AllowCheats;
      SettingsSystem.ServerSettings.AllowAdmin = data.AllowAdmin;
      SettingsSystem.ServerSettings.AllowSackKerbals = data.AllowSackKerbals;
      SettingsSystem.ServerSettings.MaxNumberOfAsteroids = data.MaxNumberOfAsteroids;
      SettingsSystem.ServerSettings.MaxNumberOfComets = data.MaxNumberOfComets;
      SettingsSystem.ServerSettings.ConsoleIdentifier = data.ConsoleIdentifier;
      SettingsSystem.ServerSettings.SafetyBubbleDistance = data.SafetyBubbleDistance;
      SettingsSystem.ServerSettings.MaxVesselParts = data.MaxVesselParts;
      SettingsSystem.ServerSettings.VesselUpdatesMsInterval = data.VesselUpdatesMsInterval;
      SettingsSystem.ServerSettings.SecondaryVesselUpdatesMsInterval = data.SecondaryVesselUpdatesMsInterval;
      SettingsSystem.ServerSettings.GameDifficulty = data.GameDifficulty;
      SettingsSystem.ServerSettings.MinScreenshotIntervalMs = data.MinScreenshotIntervalMs;
      SettingsSystem.ServerSettings.MaxScreenshotWidth = data.MaxScreenshotWidth;
      SettingsSystem.ServerSettings.MaxScreenshotHeight = data.MaxScreenshotHeight;
      SettingsSystem.ServerSettings.MinCraftLibraryRequestIntervalMs = data.MinScreenshotIntervalMs;
      SettingsSystem.ServerSettings.PrintMotdInChat = data.PrintMotdInChat;
      SettingsSystem.ServerSettings.ServerParameters = GameParameters.GetDefaultParameters(MainSystem.Singleton.ConvertGameMode(SettingsSystem.ServerSettings.GameMode), (GameParameters.Preset) SettingsSystem.ServerSettings.GameDifficulty);
      if (SettingsSystem.ServerSettings.GameDifficulty == GameDifficulty.Custom)
      {
        SettingsSystem.ServerSettings.ServerParameters = new GameParameters()
        {
          preset = (GameParameters.Preset) 4,
          Difficulty = {
            AllowOtherLaunchSites = data.AllowOtherLaunchSites,
            AllowStockVessels = data.AllowStockVessels,
            AutoHireCrews = data.AutoHireCrews,
            BypassEntryPurchaseAfterResearch = data.BypassEntryPurchaseAfterResearch,
            IndestructibleFacilities = data.IndestructibleFacilities,
            MissingCrewsRespawn = data.MissingCrewsRespawn,
            ReentryHeatScale = data.ReentryHeatScale,
            ResourceAbundance = data.ResourceAbundance,
            RespawnTimer = data.RespawnTimer,
            EnableCommNet = data.EnableCommNet
          },
          Career = {
            FundsGainMultiplier = data.FundsGainMultiplier,
            FundsLossMultiplier = data.FundsLossMultiplier,
            RepGainMultiplier = data.RepGainMultiplier,
            RepLossMultiplier = data.RepLossMultiplier,
            RepLossDeclined = data.RepLossDeclined,
            ScienceGainMultiplier = data.ScienceGainMultiplier,
            StartingFunds = data.StartingFunds,
            StartingReputation = data.StartingReputation,
            StartingScience = data.StartingScience
          },
          Flight = {
            CanRestart = data.CanRevert,
            CanLeaveToEditor = data.CanRevert
          }
        };
        SettingsSystem.ServerSettings.ServerAdvancedParameters = new GameParameters.AdvancedParams()
        {
          ActionGroupsAlways = data.ActionGroupsAlways,
          GKerbalLimits = data.GKerbalLimits,
          GPartLimits = data.GPartLimits,
          KerbalGToleranceMult = data.KerbalGToleranceMult,
          PressurePartLimits = data.PressurePartLimits,
          AllowNegativeCurrency = data.AllowNegativeCurrency,
          EnableKerbalExperience = data.EnableKerbalExperience,
          ImmediateLevelUp = data.ImmediateLevelUp,
          ResourceTransferObeyCrossfeed = data.ResourceTransferObeyCrossfeed,
          BuildingImpactDamageMult = data.BuildingImpactDamageMult,
          PartUpgradesInCareer = data.PartUpgradesInCareerAndSandbox,
          PartUpgradesInSandbox = data.PartUpgradesInCareerAndSandbox,
          EnableFullSASInSandbox = data.EnableFullSASInSandbox
        };
        SettingsSystem.ServerSettings.ServerCommNetParameters = new CommNetParams()
        {
          requireSignalForControl = data.RequireSignalForControl,
          DSNModifier = data.DsnModifier,
          rangeModifier = data.RangeModifier,
          occlusionMultiplierVac = data.OcclusionMultiplierVac,
          occlusionMultiplierAtm = data.OcclusionMultiplierAtm,
          enableGroundStations = data.EnableGroundStations,
          plasmaBlackout = data.PlasmaBlackout
        };
      }
      SettingsSystem.ServerSettings.ServerParameters.Flight.CanQuickLoad = false;
      MainSystem.NetworkState = ClientState.SettingsSynced;
    }
  }
}
