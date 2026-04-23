// Decompiled with JetBrains decompiler
// Type: LmpClient.Windows.Status.StatusTexts
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Systems.TimeSync;
using LmpClient.Systems.Warp;
using System.Text;

namespace LmpClient.Windows.Status
{
  public class StatusTexts
  {
    public const string DebugBtnTxt = "Debug";
    public const string SystemsBtnTxt = "Systems";
    public const string VesselsBtnTxt = "Vessels";
    public const string WarpingLabelTxt = "WARPING";
    public const string Debug1BtnTxt = "D1";
    public const string Debug2BtnTxt = "D2";
    public const string Debug3BtnTxt = "D3";
    public const string Debug4BtnTxt = "D4";
    public const string Debug5BtnTxt = "D5";
    public const string Debug6BtnTxt = "D6";
    public const string Debug7BtnTxt = "D7";
    public const string Debug8BtnTxt = "D8";
    public const string Debug9BtnTxt = "D9";
    private const string NegativeDeltaTimePrefix = " (-";
    private const string PositiveDeltaTimePrefix = " (+";
    private const string CloseDeltaTime = ")";
    private static readonly StringBuilder StringBuilder = new StringBuilder();

    public static string GetTimeLabel(SubspaceDisplayEntry currentEntry)
    {
      StatusTexts.StringBuilder.Length = 0;
      double subspaceTime = LmpClient.Base.System<WarpSystem>.Singleton.GetSubspaceTime(currentEntry.SubspaceId);
      StatusTexts.StringBuilder.Append(KSPUtil.PrintDateCompact(subspaceTime, true, true));
      if (LmpClient.Base.System<WarpSystem>.Singleton.CurrentSubspace != currentEntry.SubspaceId)
        StatusTexts.AppendDeltaTime(subspaceTime);
      return StatusTexts.StringBuilder.ToString();
    }

    private static void AppendDeltaTime(double subspaceTime)
    {
      double universalTime = TimeSyncSystem.UniversalTime;
      if (subspaceTime < universalTime)
        StatusTexts.StringBuilder.Append(" (-").Append(KSPUtil.PrintTimeCompact(universalTime - subspaceTime, false));
      else
        StatusTexts.StringBuilder.Append(" (+").Append(KSPUtil.PrintTimeCompact(subspaceTime - universalTime, false));
      StatusTexts.StringBuilder.Append(")");
    }
  }
}
