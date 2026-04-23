// Decompiled with JetBrains decompiler
// Type: LmpClient.Localization.Structures.DisclaimerDialogText
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Localization.Structures
{
  public class DisclaimerDialogText
  {
    public string Text { get; set; } = "Luna Multi Player (LMP) shares the following personally identifiable information with the master server and any server you connect to.\na) Your player name you connect with.\nb) Your player token (A randomly generated string to authenticate you).\nc) Your IP address is logged on the server console.\n\nLMP does not contact any other computer than the server you are connecting to and the master server.\nIn order to use LMP, you must allow it to use this info\n\nFor more information - see the KSP addon rules\n";

    public string Title { get; set; } = "LunaMultiplayer - Disclaimer";

    public string Accept { get; set; } = nameof (Accept);

    public string Decline { get; set; } = nameof (Decline);
  }
}
