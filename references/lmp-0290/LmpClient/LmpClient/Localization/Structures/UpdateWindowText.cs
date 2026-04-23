// Decompiled with JetBrains decompiler
// Type: LmpClient.Localization.Structures.UpdateWindowText
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Localization.Structures
{
  public class UpdateWindowText
  {
    public string Title { get; set; } = "New update available";

    public string Text { get; set; } = "There is a new version of LMP available for download";

    public string StillCompatible { get; set; } = "Your current LMP version is compatible with the latest version";

    public string NotCompatible { get; set; } = "Your current LMP version is not compatible with the latest version";

    public string Changelog { get; set; } = nameof (Changelog);

    public string CurrentVersion { get; set; } = "Your current version:";

    public string LatestVersion { get; set; } = "Latest version:";
  }
}
