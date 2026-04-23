// Decompiled with JetBrains decompiler
// Type: LmpCommon.Pointer.Ptr`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpCommon.Pointer
{
  public class Ptr<T>
  {
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;

    public Ptr(Func<T> g, Action<T> s)
    {
      this._getter = g;
      this._setter = s;
    }

    public T Deref
    {
      get => this._getter();
      set => this._setter(value);
    }
  }
}
