// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.CelestialBodyExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using UnityEngine;

namespace LmpClient.Extensions
{
  public static class CelestialBodyExtension
  {
    public static double SiderealDayLength(this CelestialBody body)
    {
      if (Object.op_Equality((Object) body, (Object) null) || body.orbit == null || !body.solarRotationPeriod)
        return 0.0;
      double num = 2.0 * Math.PI * Math.Sqrt(Math.Pow(Math.Abs(body.orbit.semiMajorAxis), 3.0) / body.orbit.referenceBody.gravParameter);
      return body.rotationPeriod * num / (num + body.rotationPeriod);
    }
  }
}
