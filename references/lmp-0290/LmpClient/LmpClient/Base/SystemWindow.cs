// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.SystemWindow`2
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using System.Reflection;

namespace LmpClient.Base
{
  public abstract class SystemWindow<T, TS> : Window<T>
    where T : class, IWindow, new()
    where TS : class, ISystem
  {
    private bool _display;
    private static TS _system;

    public override bool Display
    {
      get => this._display && SystemWindow<T, TS>.System.Enabled;
      set => this._display = value;
    }

    protected static TS System
    {
      get
      {
        if ((object) SystemWindow<T, TS>._system == null)
          SystemWindow<T, TS>._system = typeof (TS).GetProperty("Singleton", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.GetValue((object) null, (object[]) null) as TS;
        return SystemWindow<T, TS>._system;
      }
    }
  }
}
