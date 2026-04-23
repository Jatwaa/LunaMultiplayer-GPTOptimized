// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.InstallChecker
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization;
using System;
using System.IO;
using System.Reflection;

namespace LmpClient.Utilities
{
  public class InstallChecker
  {
    private static string _currentPath = "";
    private static string _correctPath = "";

    public static bool IsCorrectlyInstalled()
    {
      string fullName = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).FullName;
      string path = CommonUtil.CombinePaths(new DirectoryInfo(MainSystem.KspPath).FullName, "GameData", "LunaMultiplayer", "Plugins", "LmpClient.dll");
      InstallChecker._currentPath = fullName;
      InstallChecker._correctPath = path;
      return File.Exists(path) || fullName == path;
    }

    public static void SpawnDialog()
    {
      if (InstallChecker.IsCorrectlyInstalled())
        return;
      LunaLog.Log("[InstallChecker] Mod '" + Assembly.GetExecutingAssembly().GetName().Name + "' is not correctly installed.");
      LunaLog.Log("[InstallChecker] LMP is Currently installed on '" + InstallChecker._currentPath + "', should be installed at '" + InstallChecker._correctPath + "'");
      PopupDialog.SpawnPopupDialog(new MultiOptionDialog(nameof (InstallChecker), LocalizationContainer.InstallDialogText.IncorrectInstall + "\n\n" + LocalizationContainer.InstallDialogText.CurrentLoc + " " + InstallChecker._currentPath + "\n\n" + LocalizationContainer.InstallDialogText.CorrectLoc + " " + InstallChecker._correctPath + "\n", LocalizationContainer.InstallDialogText.Title, HighLogic.UISkin, Array.Empty<DialogGUIBase>()), true, HighLogic.UISkin, true, "");
    }
  }
}
