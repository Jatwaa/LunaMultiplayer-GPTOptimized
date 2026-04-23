// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.CraftLibrary.CraftEntry
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Enums;

namespace LmpClient.Systems.CraftLibrary
{
  public class CraftEntry
  {
    public string FolderName { get; set; }

    public CraftType CraftType { get; set; }

    public string CraftName { get; set; }

    public int CraftNumBytes { get; set; }

    public byte[] CraftData { get; set; }
  }
}
