// Decompiled with JetBrains decompiler
// Type: LmpClient.ModuleStore.Patching.TestModule
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using LmpClient.Events;
using System.Reflection;

namespace LmpClient.ModuleStore.Patching
{
  internal class TestModule : PartModule
  {
    public static readonly MethodInfo AfterMethodCallMethodInfo = typeof (TestModule).GetMethod("AfterMethodCall", AccessTools.all);

    private void AfterMethodCall() => PartModuleEvent.onPartModuleMethodCalling.Fire((PartModule) this, "METHODNAME");
  }
}
