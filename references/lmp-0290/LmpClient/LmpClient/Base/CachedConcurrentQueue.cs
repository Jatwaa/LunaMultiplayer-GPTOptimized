// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.CachedConcurrentQueue`2
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Interface;
using System.Collections.Concurrent;

namespace LmpClient.Base
{
  public abstract class CachedConcurrentQueue<T, TD>
    where T : new()
    where TD : IMessageData
  {
    protected static readonly ConcurrentBag<T> Cache = new ConcurrentBag<T>();
    protected ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();

    public static int CacheSize => CachedConcurrentQueue<T, TD>.Cache.Count;

    public int Count => this.Queue.Count;

    public bool IsEmpty => this.Queue.IsEmpty;

    public virtual void Enqueue(TD msgData)
    {
      T result;
      if (CachedConcurrentQueue<T, TD>.Cache.TryTake(out result))
      {
        this.AssignFromMessage(result, msgData);
        this.Queue.Enqueue(result);
      }
      else
      {
        T obj = new T();
        this.AssignFromMessage(obj, msgData);
        this.Queue.Enqueue(obj);
      }
    }

    public virtual bool TryDequeue(out T result) => this.Queue.TryDequeue(out result);

    public virtual bool TryPeek(out T result) => this.Queue.TryPeek(out result);

    public virtual void Clear()
    {
      do
        ;
      while (!this.Queue.IsEmpty && this.Queue.TryDequeue(out T _));
    }

    public virtual void Recycle(T item)
    {
      if ((object) item == null)
        return;
      CachedConcurrentQueue<T, TD>.Cache.Add(item);
    }

    protected abstract void AssignFromMessage(T value, TD msgData);
  }
}
