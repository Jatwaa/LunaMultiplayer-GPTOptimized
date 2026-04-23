// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Settings.SettingsReplyMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Settings
{
  public class SettingsReplyMsgData : SettingsBaseMsgData
  {
    public WarpMode WarpMode;
    public GameMode GameMode;
    public TerrainQuality TerrainQuality;
    public bool AllowCheats;
    public bool AllowAdmin;
    public bool AllowSackKerbals;
    public int MaxNumberOfAsteroids;
    public int MaxNumberOfComets;
    public string ConsoleIdentifier;
    public GameDifficulty GameDifficulty;
    public float SafetyBubbleDistance;
    public int MaxVesselParts;
    public int VesselUpdatesMsInterval;
    public int SecondaryVesselUpdatesMsInterval;
    public bool AllowOtherLaunchSites;
    public bool AllowStockVessels;
    public bool CanRevert;
    public bool AutoHireCrews;
    public bool BypassEntryPurchaseAfterResearch;
    public bool IndestructibleFacilities;
    public bool MissingCrewsRespawn;
    public float ReentryHeatScale;
    public float ResourceAbundance;
    public float FundsGainMultiplier;
    public float FundsLossMultiplier;
    public float RepGainMultiplier;
    public float RepLossMultiplier;
    public float RepLossDeclined;
    public float ScienceGainMultiplier;
    public float StartingFunds;
    public float StartingReputation;
    public float StartingScience;
    public float RespawnTimer;
    public bool EnableCommNet;
    public bool EnableKerbalExperience;
    public bool ImmediateLevelUp;
    public bool ResourceTransferObeyCrossfeed;
    public float BuildingImpactDamageMult;
    public bool PartUpgradesInCareerAndSandbox;
    public bool EnableFullSASInSandbox;
    public bool RequireSignalForControl;
    public float DsnModifier;
    public float RangeModifier;
    public float OcclusionMultiplierVac;
    public float OcclusionMultiplierAtm;
    public bool EnableGroundStations;
    public bool PlasmaBlackout;
    public bool ActionGroupsAlways;
    public bool GKerbalLimits;
    public bool GPartLimits;
    public bool PressurePartLimits;
    public float KerbalGToleranceMult;
    public bool AllowNegativeCurrency;
    public int MinScreenshotIntervalMs;
    public int MaxScreenshotWidth;
    public int MaxScreenshotHeight;
    public int MinCraftLibraryRequestIntervalMs;
    public bool PrintMotdInChat;

    internal SettingsReplyMsgData()
    {
    }

    public override SettingsMessageType SettingsMessageType => SettingsMessageType.Reply;

    public override string ClassName { get; } = nameof (SettingsReplyMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write((int) this.WarpMode);
      lidgrenMsg.Write((int) this.GameMode);
      lidgrenMsg.Write((int) this.TerrainQuality);
      lidgrenMsg.Write(this.AllowCheats);
      lidgrenMsg.Write(this.AllowAdmin);
      lidgrenMsg.Write(this.AllowSackKerbals);
      lidgrenMsg.Write(this.MaxNumberOfAsteroids);
      lidgrenMsg.Write(this.MaxNumberOfComets);
      lidgrenMsg.Write(this.ConsoleIdentifier);
      lidgrenMsg.Write((int) this.GameDifficulty);
      lidgrenMsg.Write(this.SafetyBubbleDistance);
      lidgrenMsg.Write(this.MaxVesselParts);
      lidgrenMsg.Write(this.VesselUpdatesMsInterval);
      lidgrenMsg.Write(this.SecondaryVesselUpdatesMsInterval);
      lidgrenMsg.Write(this.AllowOtherLaunchSites);
      lidgrenMsg.Write(this.AllowStockVessels);
      lidgrenMsg.Write(this.CanRevert);
      lidgrenMsg.Write(this.AutoHireCrews);
      lidgrenMsg.Write(this.BypassEntryPurchaseAfterResearch);
      lidgrenMsg.Write(this.IndestructibleFacilities);
      lidgrenMsg.Write(this.MissingCrewsRespawn);
      lidgrenMsg.Write(this.ReentryHeatScale);
      lidgrenMsg.Write(this.ResourceAbundance);
      lidgrenMsg.Write(this.FundsGainMultiplier);
      lidgrenMsg.Write(this.FundsLossMultiplier);
      lidgrenMsg.Write(this.RepGainMultiplier);
      lidgrenMsg.Write(this.RepLossMultiplier);
      lidgrenMsg.Write(this.RepLossDeclined);
      lidgrenMsg.Write(this.ScienceGainMultiplier);
      lidgrenMsg.Write(this.StartingFunds);
      lidgrenMsg.Write(this.StartingReputation);
      lidgrenMsg.Write(this.StartingScience);
      lidgrenMsg.Write(this.RespawnTimer);
      lidgrenMsg.Write(this.EnableCommNet);
      lidgrenMsg.Write(this.EnableKerbalExperience);
      lidgrenMsg.Write(this.ImmediateLevelUp);
      lidgrenMsg.Write(this.ResourceTransferObeyCrossfeed);
      lidgrenMsg.Write(this.BuildingImpactDamageMult);
      lidgrenMsg.Write(this.PartUpgradesInCareerAndSandbox);
      lidgrenMsg.Write(this.EnableFullSASInSandbox);
      lidgrenMsg.Write(this.RequireSignalForControl);
      lidgrenMsg.Write(this.DsnModifier);
      lidgrenMsg.Write(this.RangeModifier);
      lidgrenMsg.Write(this.OcclusionMultiplierVac);
      lidgrenMsg.Write(this.OcclusionMultiplierAtm);
      lidgrenMsg.Write(this.EnableGroundStations);
      lidgrenMsg.Write(this.PlasmaBlackout);
      lidgrenMsg.Write(this.ActionGroupsAlways);
      lidgrenMsg.Write(this.GKerbalLimits);
      lidgrenMsg.Write(this.GPartLimits);
      lidgrenMsg.Write(this.PressurePartLimits);
      lidgrenMsg.Write(this.KerbalGToleranceMult);
      lidgrenMsg.Write(this.AllowNegativeCurrency);
      lidgrenMsg.Write(this.MinScreenshotIntervalMs);
      lidgrenMsg.Write(this.MaxScreenshotWidth);
      lidgrenMsg.Write(this.MaxScreenshotHeight);
      lidgrenMsg.Write(this.MinCraftLibraryRequestIntervalMs);
      lidgrenMsg.Write(this.PrintMotdInChat);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.WarpMode = (WarpMode) lidgrenMsg.ReadInt32();
      this.GameMode = (GameMode) lidgrenMsg.ReadInt32();
      this.TerrainQuality = (TerrainQuality) lidgrenMsg.ReadInt32();
      this.AllowCheats = lidgrenMsg.ReadBoolean();
      this.AllowAdmin = lidgrenMsg.ReadBoolean();
      this.AllowSackKerbals = lidgrenMsg.ReadBoolean();
      this.MaxNumberOfAsteroids = lidgrenMsg.ReadInt32();
      this.MaxNumberOfComets = lidgrenMsg.ReadInt32();
      this.ConsoleIdentifier = lidgrenMsg.ReadString();
      this.GameDifficulty = (GameDifficulty) lidgrenMsg.ReadInt32();
      this.SafetyBubbleDistance = lidgrenMsg.ReadFloat();
      this.MaxVesselParts = lidgrenMsg.ReadInt32();
      this.VesselUpdatesMsInterval = lidgrenMsg.ReadInt32();
      this.SecondaryVesselUpdatesMsInterval = lidgrenMsg.ReadInt32();
      this.AllowOtherLaunchSites = lidgrenMsg.ReadBoolean();
      this.AllowStockVessels = lidgrenMsg.ReadBoolean();
      this.CanRevert = lidgrenMsg.ReadBoolean();
      this.AutoHireCrews = lidgrenMsg.ReadBoolean();
      this.BypassEntryPurchaseAfterResearch = lidgrenMsg.ReadBoolean();
      this.IndestructibleFacilities = lidgrenMsg.ReadBoolean();
      this.MissingCrewsRespawn = lidgrenMsg.ReadBoolean();
      this.ReentryHeatScale = lidgrenMsg.ReadFloat();
      this.ResourceAbundance = lidgrenMsg.ReadFloat();
      this.FundsGainMultiplier = lidgrenMsg.ReadFloat();
      this.FundsLossMultiplier = lidgrenMsg.ReadFloat();
      this.RepGainMultiplier = lidgrenMsg.ReadFloat();
      this.RepLossMultiplier = lidgrenMsg.ReadFloat();
      this.RepLossDeclined = lidgrenMsg.ReadFloat();
      this.ScienceGainMultiplier = lidgrenMsg.ReadFloat();
      this.StartingFunds = lidgrenMsg.ReadFloat();
      this.StartingReputation = lidgrenMsg.ReadFloat();
      this.StartingScience = lidgrenMsg.ReadFloat();
      this.RespawnTimer = lidgrenMsg.ReadFloat();
      this.EnableCommNet = lidgrenMsg.ReadBoolean();
      this.EnableKerbalExperience = lidgrenMsg.ReadBoolean();
      this.ImmediateLevelUp = lidgrenMsg.ReadBoolean();
      this.ResourceTransferObeyCrossfeed = lidgrenMsg.ReadBoolean();
      this.BuildingImpactDamageMult = lidgrenMsg.ReadFloat();
      this.PartUpgradesInCareerAndSandbox = lidgrenMsg.ReadBoolean();
      this.EnableFullSASInSandbox = lidgrenMsg.ReadBoolean();
      this.RequireSignalForControl = lidgrenMsg.ReadBoolean();
      this.DsnModifier = lidgrenMsg.ReadFloat();
      this.RangeModifier = lidgrenMsg.ReadFloat();
      this.OcclusionMultiplierVac = lidgrenMsg.ReadFloat();
      this.OcclusionMultiplierAtm = lidgrenMsg.ReadFloat();
      this.EnableGroundStations = lidgrenMsg.ReadBoolean();
      this.PlasmaBlackout = lidgrenMsg.ReadBoolean();
      this.ActionGroupsAlways = lidgrenMsg.ReadBoolean();
      this.GKerbalLimits = lidgrenMsg.ReadBoolean();
      this.GPartLimits = lidgrenMsg.ReadBoolean();
      this.PressurePartLimits = lidgrenMsg.ReadBoolean();
      this.KerbalGToleranceMult = lidgrenMsg.ReadFloat();
      this.AllowNegativeCurrency = lidgrenMsg.ReadBoolean();
      this.MinScreenshotIntervalMs = lidgrenMsg.ReadInt32();
      this.MaxScreenshotWidth = lidgrenMsg.ReadInt32();
      this.MaxScreenshotHeight = lidgrenMsg.ReadInt32();
      this.MinCraftLibraryRequestIntervalMs = lidgrenMsg.ReadInt32();
      this.PrintMotdInChat = lidgrenMsg.ReadBoolean();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + 4 + 4 + 4 + 24 + 36 + 76 + this.ConsoleIdentifier.GetByteCount();
  }
}
