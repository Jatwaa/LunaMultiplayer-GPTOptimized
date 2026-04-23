// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.VectorUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpClient.Utilities
{
  public class VectorUtil
  {
    public static Vector3d LerpUnclamped(Vector3d from, Vector3d to, float t) => new Vector3d(from.x + (to.x - from.x) * (double) t, from.y + (to.y - from.y) * (double) t, from.z + (to.z - from.z) * (double) t);
  }
}
