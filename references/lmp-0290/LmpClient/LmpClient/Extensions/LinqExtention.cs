// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.LinqExtention
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;

namespace LmpClient.Extensions
{
  public static class LinqExtention
  {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      HashSet<TKey> seenKeys = new HashSet<TKey>();
      foreach (TSource source1 in source)
      {
        TSource element = source1;
        if (seenKeys.Add(keySelector(element)))
          yield return element;
        element = default (TSource);
      }
    }
  }
}
