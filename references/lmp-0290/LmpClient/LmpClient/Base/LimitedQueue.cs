// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.LimitedQueue`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Collections.Generic;

namespace LmpClient.Base
{
  public class LimitedQueue<T> : Queue<T>
  {
    public int Limit { get; set; }

    public LimitedQueue(int limit)
      : base(limit)
    {
      this.Limit = limit;
    }

    public new void Enqueue(T item)
    {
      while (this.Count >= this.Limit)
        this.Dequeue();
      base.Enqueue(item);
    }
  }
}
