// Decompiled with JetBrains decompiler
// Type: LmpCommon.ConsoleUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Runtime.InteropServices;

namespace LmpCommon
{
  public static class ConsoleUtil
  {
    private const uint EnableQuickEdit = 64;
    private const int StdInputHandle = -10;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    public static bool DisableConsoleQuickEdit()
    {
      if (Common.PlatformIsWindows())
        return true;
      IntPtr stdHandle = ConsoleUtil.GetStdHandle(-10);
      uint lpMode;
      if (!ConsoleUtil.GetConsoleMode(stdHandle, out lpMode))
        return false;
      lpMode &= 4294967231U;
      return ConsoleUtil.SetConsoleMode(stdHandle, lpMode);
    }
  }
}
