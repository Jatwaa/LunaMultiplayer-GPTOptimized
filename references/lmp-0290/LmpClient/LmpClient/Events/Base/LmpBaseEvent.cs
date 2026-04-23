// Decompiled with JetBrains decompiler
// Type: LmpClient.Events.Base.LmpBaseEvent
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LmpClient.Events.Base
{
  public abstract class LmpBaseEvent
  {
    public static void Awake() => Parallel.ForEach<Type>(((IEnumerable<Type>) Assembly.GetExecutingAssembly().GetTypes()).Where<Type>((Func<Type, bool>) (myType => myType.IsClass && myType.IsSubclassOf(typeof (LmpBaseEvent)))), (Action<Type>) (lmpEventClass =>
    {
      FieldInfo[] array = ((IEnumerable<FieldInfo>) lmpEventClass.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public)).ToArray<FieldInfo>();
      if (!((IEnumerable<FieldInfo>) array).Any<FieldInfo>())
        return;
      foreach (FieldInfo fieldInfo in array)
      {
        object instance = Activator.CreateInstance(fieldInfo.FieldType, (object) fieldInfo.Name);
        fieldInfo.SetValue((object) null, instance);
      }
    }));
  }
}
