// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.AssemblyExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LmpClient.Extensions
{
  public static class AssemblyExtension
  {
    public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
      if (assembly == (Assembly) null)
        throw new ArgumentNullException(nameof (assembly));
      try
      {
        return (IEnumerable<Type>) assembly.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        return ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (t => t != (Type) null));
      }
    }
  }
}
