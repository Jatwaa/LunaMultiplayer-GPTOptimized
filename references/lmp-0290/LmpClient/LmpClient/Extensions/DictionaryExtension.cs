// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.DictionaryExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;

namespace LmpClient.Extensions
{
  public static class DictionaryExtension
  {
    public static TU GetOrAdd<T, TU>(this Dictionary<T, TU> dict, T key, Func<TU> create)
    {
      TU orAdd;
      if (!dict.TryGetValue(key, out orAdd))
      {
        orAdd = create();
        dict[key] = orAdd;
      }
      return orAdd;
    }
  }
}
