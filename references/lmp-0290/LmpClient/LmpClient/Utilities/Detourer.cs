// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.Detourer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace LmpClient.Utilities
{
  public static class Detourer
  {
    private static readonly Dictionary<string, string> Detours = new Dictionary<string, string>();

    public static bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
    {
      if (source == (MethodInfo) null || destination == (MethodInfo) null)
      {
        LunaLog.LogError("[Detour] Source/Destination MethodInfo cannot be null");
        return false;
      }
      if (!Detourer.CheckDetouring(source, destination))
        return false;
      if (IntPtr.Size == 8)
        Detourer.Create64BitsDetour(source, destination);
      else
        Detourer.Create32BitsDetour(source, destination);
      return true;
    }

    private static bool CheckDetouring(MethodInfo source, MethodInfo destination)
    {
      string[] strArray1 = new string[5]
      {
        source.DeclaringType?.FullName,
        ".",
        source.Name,
        " @ 0x",
        null
      };
      IntPtr functionPointer = source.MethodHandle.GetFunctionPointer();
      ref IntPtr local1 = ref functionPointer;
      int num = IntPtr.Size * 2;
      string format1 = "X" + num.ToString();
      strArray1[4] = local1.ToString(format1);
      string key = string.Concat(strArray1);
      string[] strArray2 = new string[5]
      {
        destination.DeclaringType?.FullName,
        ".",
        destination.Name,
        " @ 0x",
        null
      };
      functionPointer = destination.MethodHandle.GetFunctionPointer();
      ref IntPtr local2 = ref functionPointer;
      num = IntPtr.Size * 2;
      string format2 = "X" + num.ToString();
      strArray2[4] = local2.ToString(format2);
      string str = string.Concat(strArray2);
      if (Detourer.Detours.ContainsKey(key))
      {
        if (str != Detourer.Detours[key])
          LunaLog.LogWarning("[Detour] Source method('" + key + "') was previously detoured to '" + Detourer.Detours[key] + "'");
        return false;
      }
      Detourer.Detours.Add(key, str);
      LunaLog.Log("[Detour] Detouring '" + key + "' to '" + str + "'");
      return true;
    }

    private static unsafe void Create32BitsDetour(MethodInfo source, MethodInfo destination)
    {
      int int32_1 = source.MethodHandle.GetFunctionPointer().ToInt32();
      int int32_2 = destination.MethodHandle.GetFunctionPointer().ToInt32();
      byte* numPtr1 = (byte*) int32_1;
      int* numPtr2 = (int*) (numPtr1 + 1);
      int num = int32_2 - int32_1 - 5;
      *numPtr1 = (byte) 233;
      *numPtr2 = num;
    }

    private static unsafe void Create64BitsDetour(MethodInfo source, MethodInfo destination)
    {
      long int64_1 = source.MethodHandle.GetFunctionPointer().ToInt64();
      long int64_2 = destination.MethodHandle.GetFunctionPointer().ToInt64();
      byte* numPtr1 = (byte*) int64_1;
      long* numPtr2 = (long*) (numPtr1 + 2);
      *numPtr1 = (byte) 72;
      numPtr1[1] = (byte) 184;
      *numPtr2 = int64_2;
      numPtr1[10] = byte.MaxValue;
      numPtr1[11] = (byte) 224;
    }
  }
}
