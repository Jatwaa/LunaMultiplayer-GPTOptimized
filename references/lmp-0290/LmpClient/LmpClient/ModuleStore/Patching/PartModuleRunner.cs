// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Patching.PartModuleRunner
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LmpClient.ModuleStore.Patching
{
  public class PartModuleRunner
  {
    private static Task _awakeTask;
    private static double _percentage;

    public static bool Ready => PartModuleRunner._awakeTask.IsCompleted;

    public static void Awake()
    {
      if (PartModuleRunner._awakeTask != null)
        return;
      PartModuleRunner._awakeTask = Task.Run((Action) (() =>
      {
        Type[] partModules = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (a => ((IEnumerable<Type>) a.GetTypes()).Where<Type>((Func<Type, bool>) (myType => myType.IsClass && myType.IsSubclassOf(typeof (PartModule)))))).ToArray<Type>();
        Parallel.ForEach<Type>((IEnumerable<Type>) partModules, (Action<Type>) (partModule =>
        {
          try
          {
            PartModulePatcher.PatchFieldsAndMethods(partModule);
            PartModuleRunner.IncreasePercentage(1.0 / (double) partModules.Length);
          }
          catch (Exception ex)
          {
            LunaLog.LogError("Exception patching module " + partModule.Name + " from assembly " + partModule.Assembly.GetName().Name + ": " + ex.Message);
          }
        }));
      }));
      PartModuleRunner._awakeTask.ConfigureAwait(false);
    }

    public static string GetPercentage() => (Interlocked.CompareExchange(ref PartModuleRunner._percentage, 0.0, 0.0) * 100.0).ToString("0.##");

    private static double IncreasePercentage(double value)
    {
      double num1 = PartModuleRunner._percentage;
      double comparand;
      double num2;
      do
      {
        comparand = num1;
        num2 = comparand + value;
        num1 = Interlocked.CompareExchange(ref PartModuleRunner._percentage, num2, comparand);
      }
      while (num1 != comparand);
      return num2;
    }
  }
}
