// Decompiled with JetBrains decompiler
// Type: LmpCommon.Common
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using CachedQuickLz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LmpCommon
{
  public class Common
  {
    public static void ThreadSafeCompress(object lockObj, ref byte[] data, ref int numBytes)
    {
      lock (lockObj)
      {
        if (CachedQlz.IsCompressed(data, numBytes))
          return;
        CachedQlz.Compress(ref data, ref numBytes);
      }
    }

    public static void ThreadSafeDecompress(
      object lockObj,
      ref byte[] data,
      int length,
      out int numBytes)
    {
      lock (lockObj)
      {
        if (CachedQlz.IsCompressed(data, length))
          CachedQlz.Decompress(ref data, out numBytes);
        else
          numBytes = length;
      }
    }

    public static T[] TrimArray<T>(T[] array, int size)
    {
      T[] destinationArray = new T[size];
      Array.Copy((Array) array, (Array) destinationArray, size);
      return destinationArray;
    }

    public static bool PlatformIsWindows() => Environment.OSVersion.Platform == PlatformID.Win32NT;

    public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
    {
      if (!(list1 is T[] objArray1))
        objArray1 = list1.ToArray<T>();
      T[] objArray2 = objArray1;
      if (!(list2 is T[] objArray3))
        objArray3 = list2.ToArray<T>();
      T[] objArray4 = objArray3;
      if (objArray2.Length != objArray4.Length)
        return false;
      Dictionary<T, int> dictionary = new Dictionary<T, int>();
      foreach (T key in objArray2)
      {
        if (dictionary.ContainsKey(key))
          dictionary[key]++;
        else
          dictionary.Add(key, 1);
      }
      foreach (T key in objArray4)
      {
        if (!dictionary.ContainsKey(key))
          return false;
        dictionary[key]--;
      }
      return dictionary.Values.All<int>((Func<int, bool>) (c => c == 0));
    }

    public string CalculateSha256StringHash(string input) => Common.CalculateSha256Hash(Encoding.UTF8.GetBytes(input));

    public static string CalculateSha256FileHash(string fileName) => Common.CalculateSha256Hash(File.ReadAllBytes(fileName));

    public static string CalculateSha256Hash(byte[] data)
    {
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return BitConverter.ToString(shA256Managed.ComputeHash(data));
    }

    public static string ConvertConfigStringToGuidString(string configNodeString)
    {
      if (configNodeString == null || configNodeString.Length != 32)
        return (string) null;
      return string.Join("-", new string[5]
      {
        configNodeString.Substring(0, 8),
        configNodeString.Substring(8, 4),
        configNodeString.Substring(12, 4),
        configNodeString.Substring(16, 4),
        configNodeString.Substring(20)
      });
    }

    public static Guid ConvertConfigStringToGuid(string configNodeString)
    {
      try
      {
        return new Guid(Common.ConvertConfigStringToGuidString(configNodeString));
      }
      catch (Exception ex)
      {
        return Guid.Empty;
      }
    }
  }
}
