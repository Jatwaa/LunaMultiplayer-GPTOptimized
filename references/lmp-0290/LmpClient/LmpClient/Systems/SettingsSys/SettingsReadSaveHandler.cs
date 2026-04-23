// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.SettingsReadSaveHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Utilities;
using LmpCommon.Xml;
using System.IO;

namespace LmpClient.Systems.SettingsSys
{
  public static class SettingsReadSaveHandler
  {
    private const string SettingsFileName = "settings.xml";
    private const string BackupSettingsFileName = "settings_bkp.xml";

    private static string DataFolderPath => CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Data");

    private static string SettingsFilePath => CommonUtil.CombinePaths(SettingsReadSaveHandler.DataFolderPath, "settings.xml");

    private static string BackupSettingsFilePath => CommonUtil.CombinePaths(SettingsReadSaveHandler.DataFolderPath, "settings_bkp.xml");

    public static SettingStructure ReadSettings()
    {
      SettingsReadSaveHandler.CheckDataDirectory();
      SettingsReadSaveHandler.RestoreBackupIfNoSettings();
      if (!File.Exists(SettingsReadSaveHandler.SettingsFilePath))
        SettingsReadSaveHandler.CreateDefaultSettingsFile();
      if (!File.Exists(SettingsReadSaveHandler.BackupSettingsFilePath))
      {
        LunaLog.Log("[LMP]: Backing up settings file!");
        File.Copy(SettingsReadSaveHandler.SettingsFilePath, SettingsReadSaveHandler.BackupSettingsFilePath);
      }
      return LunaXmlSerializer.ReadXmlFromPath<SettingStructure>(SettingsReadSaveHandler.SettingsFilePath);
    }

    public static void SaveSettings(SettingStructure currentSettings)
    {
      SettingsReadSaveHandler.CheckDataDirectory();
      LunaXmlSerializer.WriteToXmlFile((object) currentSettings, SettingsReadSaveHandler.SettingsFilePath);
      File.Copy(SettingsReadSaveHandler.SettingsFilePath, SettingsReadSaveHandler.BackupSettingsFilePath, true);
    }

    private static void CheckDataDirectory()
    {
      if (Directory.Exists(SettingsReadSaveHandler.DataFolderPath))
        return;
      Directory.CreateDirectory(SettingsReadSaveHandler.DataFolderPath);
    }

    private static void CreateDefaultSettingsFile() => LunaXmlSerializer.WriteToXmlFile((object) new SettingStructure(), SettingsReadSaveHandler.SettingsFilePath);

    private static void RestoreBackupIfNoSettings()
    {
      if (!File.Exists(SettingsReadSaveHandler.BackupSettingsFilePath) || File.Exists(SettingsReadSaveHandler.SettingsFilePath))
        return;
      LunaLog.Log("[LMP]: Restoring player settings file!");
      File.Copy(SettingsReadSaveHandler.BackupSettingsFilePath, SettingsReadSaveHandler.SettingsFilePath);
    }
  }
}
