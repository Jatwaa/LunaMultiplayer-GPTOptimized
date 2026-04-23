// Decompiled with JetBrains decompiler
// Type: LmpClient.Localization.Structures.ModWindowText
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Localization.Structures
{
  public class ModWindowText
  {
    public string Title { get; set; } = "LunaMultiplayer - Failed Mod Validation";

    public string MandatoryModsNotFound { get; set; } = "Mandatory mods not found:";

    public string MissingExpansions { get; set; } = "Missing expansions:";

    public string MandatoryModsDifferentShaFound { get; set; } = "Mandatory mods with different SHA found:";

    public string Link { get; set; } = nameof (Link);

    public string ForbiddenFilesFound { get; set; } = "Forbidden mods found:";

    public string NonListedFilesFound { get; set; } = "Non listed and forbidden mods found:";

    public string MandatoryPartsNotFound { get; set; } = "Mandatory parts not found:";

    public string ForbiddenPartsFound { get; set; } = "Forbidden parts found:";
  }
}
