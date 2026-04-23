// Decompiled with JetBrains decompiler
// Type: LmpClient.Extensions.OrbitDriverExtension
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using HarmonyLib;
using System.Reflection;

namespace LmpClient.Extensions
{
  public static class OrbitDriverExtension
  {
    private static readonly FieldInfo OrbitDriverReady = typeof (OrbitDriver).GetField("ready", AccessTools.all);
    private static readonly MethodInfo OrbitDriverStart = typeof (OrbitDriver).GetMethod("Start", AccessTools.all);

    public static bool Ready(this OrbitDriver driver) => (bool) OrbitDriverExtension.OrbitDriverReady.GetValue((object) driver);

    public static void ForceStart(this OrbitDriver driver)
    {
      if (driver.Ready())
        return;
      OrbitDriverExtension.OrbitDriverStart.Invoke((object) driver, (object[]) null);
    }
  }
}
