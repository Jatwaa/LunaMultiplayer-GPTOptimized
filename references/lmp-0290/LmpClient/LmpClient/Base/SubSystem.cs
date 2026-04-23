// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.SubSystem`1
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using System.Reflection;

namespace LmpClient.Base
{
  public abstract class SubSystem<T> : SystemBase where T : class, ISystem, new()
  {
    private static T _system;

    protected static T System
    {
      get
      {
        if ((object) SubSystem<T>._system == null)
          SubSystem<T>._system = typeof (T).GetProperty("Singleton", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.GetValue((object) null, (object[]) null) as T;
        return SubSystem<T>._system;
      }
    }
  }
}
