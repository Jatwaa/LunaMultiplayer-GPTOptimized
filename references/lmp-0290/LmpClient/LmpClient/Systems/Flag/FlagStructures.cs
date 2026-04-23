// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Flag.ExtendedFlagInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon;
using LmpCommon.Message.Data.Flag;

namespace LmpClient.Systems.Flag
{
  public class ExtendedFlagInfo : FlagInfo
  {
    public string ShaSum => Common.CalculateSha256Hash(this.FlagData);

    public bool Loaded { get; set; }

    public ExtendedFlagInfo(FlagInfo flagInfo)
    {
      this.FlagData = Common.TrimArray<byte>(flagInfo.FlagData, flagInfo.NumBytes);
      this.Owner = flagInfo.Owner;
      this.FlagName = flagInfo.FlagName;
    }
  }
}
