// Decompiled with JetBrains decompiler
// Type: LmpCommon.LmpVersioning
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Reflection;

namespace LmpCommon
{
  public class LmpVersioning
  {
    private static Version AssemblyVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;

    public static ushort MajorVersion { get; } = (ushort) LmpVersioning.AssemblyVersion.Major;

    public static ushort MinorVersion { get; } = (ushort) LmpVersioning.AssemblyVersion.Minor;

    public static ushort BuildVersion { get; } = (ushort) LmpVersioning.AssemblyVersion.Build;

    public static Version CurrentVersion { get; } = new Version(LmpVersioning.AssemblyVersion.ToString(3));

    public static bool IsCompatible(Version version) => version.Major == (int) LmpVersioning.MajorVersion && version.Minor == (int) LmpVersioning.MinorVersion;

    public static bool IsCompatible(string versionStr)
    {
      try
      {
        return LmpVersioning.IsCompatible(new Version(versionStr));
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool IsCompatible(int major, int minor, int build) => LmpVersioning.IsCompatible(new Version(major, minor, build));
  }
}
