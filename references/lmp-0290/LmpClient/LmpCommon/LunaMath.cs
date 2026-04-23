// Decompiled with JetBrains decompiler
// Type: LmpCommon.LunaMath
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpCommon
{
  public class LunaMath
  {
    public static double Lerp(double from, double to, float t) => from * (1.0 - (double) t) + to * (double) t;

    public static double LerpUnclamped(double from, double to, double t) => from + (to - from) * t;

    public static float Lerp(float v0, float v1, float t) => (float) ((1.0 - (double) t) * (double) v0 + (double) t * (double) v1);

    public static float LerpUnclamped(float from, float to, float t) => from + (to - from) * t;

    public static bool Lerp(bool v0, bool v1, float t) => (double) t < 0.5 ? v0 : v1;

    public static double LerpAngleDeg(double from, double to, float t, double wrapAngle)
    {
      double num = LunaMath.LerpAngleDeg(from, to, t);
      if (num > wrapAngle)
        num -= 360.0;
      if (num <= -wrapAngle)
        num += 360.0;
      if (num <= wrapAngle - 360.0)
        num += 360.0;
      return num;
    }

    public static double LerpAngleDegAbs(double from, double to, float t, double wrapAngle)
    {
      double num = LunaMath.LerpAngleDeg(from, to, t, wrapAngle);
      while (num < 0.0)
        num += wrapAngle;
      while (num > wrapAngle)
        num -= wrapAngle;
      return num;
    }

    public static double LerpAngleDeg(double from, double to, float t)
    {
      double num = LunaMath.Repeat(to - from, 360.0);
      if (num > 180.0)
        num -= 360.0;
      return from + num * (double) t;
    }

    public static double LerpAngleDegAbs(double from, double to, float t)
    {
      double num = LunaMath.LerpAngleDeg(from, to, t);
      while (num < 0.0)
        num += 360.0;
      while (num > 360.0)
        num -= 360.0;
      return num;
    }

    public static double LerpAngleRad(double from, double to, float t, double wrapAngle)
    {
      double num = LunaMath.LerpAngleRad(from, to, t);
      if (num > wrapAngle)
        num -= 2.0 * Math.PI;
      if (num <= -wrapAngle)
        num += 2.0 * Math.PI;
      if (num <= wrapAngle - 2.0 * Math.PI)
        num += 2.0 * Math.PI;
      return num;
    }

    public static double LerpAngleRad(double from, double to, float t)
    {
      double num = LunaMath.Repeat(to - from, 2.0 * Math.PI);
      if (num > Math.PI)
        num -= 2.0 * Math.PI;
      return from + num * (double) t;
    }

    public static double Clamp01(double value) => value < 0.0 ? 0.0 : (value <= 1.0 ? value : 1.0);

    public static double Clamp(double value, double min, double max)
    {
      if (value < min)
        value = min;
      else if (value > max)
        value = max;
      return value;
    }

    public static double SafeDivision(double numerator, double denominator) => denominator == 0.0 ? 0.0 : numerator / denominator;

    private static double Repeat(double t, double length) => t - Math.Floor(t / length) * length;
  }
}
