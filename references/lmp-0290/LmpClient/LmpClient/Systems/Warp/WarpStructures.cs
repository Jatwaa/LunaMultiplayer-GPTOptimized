// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.SubspaceDisplayEntry
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections.Generic;

namespace LmpClient.Systems.Warp
{
  public class SubspaceDisplayEntry
  {
    public int SubspaceId { get; set; }

    public double SubspaceTime { get; set; }

    public List<string> Players { get; set; }
  }
}
