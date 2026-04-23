// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.CommonUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LmpClient.Utilities
{
  public class CommonUtil
  {
    private static readonly Random Rnd = new Random();
    private static string _processId;
    public static string OutputLogFilePath = CommonUtil.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "Squad", "Kerbal Space Program", "output_log.txt");

    public static string ProcessId
    {
      get
      {
        string processId = CommonUtil._processId;
        if (processId != null)
          return processId;
        int id = Process.GetCurrentProcess().Id;
        return CommonUtil._processId = id.ToString();
      }
    }

    public static string CombinePaths(params string[] paths) => paths != null ? ((IEnumerable<string>) paths).Aggregate<string>(new Func<string, string, string>(Path.Combine)) : throw new ArgumentNullException(nameof (paths));

    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
      bool flag = list1.Except<T>(list2).Any<T>();
      return !list2.Except<T>(list1).Any<T>() && !flag;
    }

    public static void Reserve20Mb()
    {
      byte[] buffer = new byte[20971520];
      CommonUtil.Rnd.NextBytes(buffer);
    }
  }
}
