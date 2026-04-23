// Decompiled with JetBrains decompiler
// Type: LmpCommon.Collection.ConcurrentHashSet`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections.Generic;
using System.Linq;

namespace LmpCommon.Collection
{
  public class ConcurrentHashSet<T>
  {
    private readonly object _lock = new object();
    private readonly HashSet<T> _hashSet = new HashSet<T>();

    public bool Add(T item)
    {
      lock (this._lock)
        return this._hashSet.Add(item);
    }

    public void Clear()
    {
      lock (this._lock)
        this._hashSet.Clear();
    }

    public bool Contains(T item)
    {
      lock (this._lock)
        return this._hashSet.Contains(item);
    }

    public bool Remove(T item)
    {
      lock (this._lock)
        return this._hashSet.Remove(item);
    }

    public int Count
    {
      get
      {
        lock (this._lock)
          return this._hashSet.Count;
      }
    }

    public T[] GetValues
    {
      get
      {
        lock (this._lock)
          return this._hashSet.ToArray<T>();
      }
    }
  }
}
