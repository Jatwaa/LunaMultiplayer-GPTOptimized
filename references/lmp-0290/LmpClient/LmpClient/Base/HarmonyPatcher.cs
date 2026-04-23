// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.HarmonyPatcher
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using System.Reflection;

namespace LmpClient.Base
{
  public static class HarmonyPatcher
  {
    public static Harmony HarmonyInstance = new Harmony("LunaMultiplayer");

    public static void Awake() => HarmonyPatcher.HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
  }
}
