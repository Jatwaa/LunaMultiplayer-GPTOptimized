// Decompiled with JetBrains decompiler
// Type: LmpClient.Localization.LocalizationContainer
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Localization.Structures;
using LmpClient.Utilities;
using LmpCommon.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LmpClient.Localization
{
  public static class LocalizationContainer
  {
    public static readonly List<string> Languages = new List<string>();
    public static AdminWindowText AdminWindowText = new AdminWindowText();
    public static BannedPartsResourcesWindowText BannedPartsResourcesWindowText = new BannedPartsResourcesWindowText();
    public static ChatWindowText ChatWindowText = new ChatWindowText();
    public static ConnectionWindowText ConnectionWindowText = new ConnectionWindowText();
    public static CraftLibraryWindowText CraftLibraryWindowText = new CraftLibraryWindowText();
    public static ModWindowText ModWindowText = new ModWindowText();
    public static OptionsWindowText OptionsWindowText = new OptionsWindowText();
    public static ServerListWindowText ServerListWindowText = new ServerListWindowText();
    public static StatusWindowText StatusWindowText = new StatusWindowText();
    public static DisclaimerDialogText DisclaimerDialogText = new DisclaimerDialogText();
    public static InstallDialogText InstallDialogText = new InstallDialogText();
    public static ScreenshotWindowText ScreenshotWindowText = new ScreenshotWindowText();
    public static ScreenText ScreenText = new ScreenText();
    public static ButtonTooltips ButtonTooltips = new ButtonTooltips();
    public static UpdateWindowText UpdateWindowText = new UpdateWindowText();
    public static CompatibleDialogText CompatibleDialogText = new CompatibleDialogText();
    public static RevertDialogText RevertDialogText = new RevertDialogText();
    public static ServerListFiltersText ServerListFiltersText = new ServerListFiltersText();
    private static readonly string LocalizationFolder = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Localization");

    public static string CurrentLanguage { get; set; } = "English";

    public static string GetCurrentLanguageAsText() => LocalizationContainer.CurrentLanguage.Replace("_", " ");

    public static void LoadLanguages()
    {
      LocalizationContainer.Languages.Clear();
      LocalizationContainer.Languages.AddRange(((IEnumerable<string>) Directory.GetDirectories(LocalizationContainer.LocalizationFolder)).Select<string, string>((Func<string, string>) (d => new DirectoryInfo(d).Name)));
    }

    public static string GetNextLanguage()
    {
      for (int index = 0; index < LocalizationContainer.Languages.Count; ++index)
      {
        if (LocalizationContainer.CurrentLanguage == LocalizationContainer.Languages[index])
          return index + 1 == LocalizationContainer.Languages.Count ? LocalizationContainer.Languages[0] : LocalizationContainer.Languages[index + 1];
      }
      return LocalizationContainer.Languages[0];
    }

    public static void LoadLanguage(string language)
    {
      LocalizationContainer.CurrentLanguage = language;
      if (!Directory.Exists(LocalizationContainer.LocalizationFolder))
        Directory.CreateDirectory(LocalizationContainer.LocalizationFolder);
      if (!Directory.Exists(CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language)))
        Directory.CreateDirectory(CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language));
      LocalizationContainer.LoadWindowTexts<AdminWindowText>(language, ref LocalizationContainer.AdminWindowText);
      LocalizationContainer.LoadWindowTexts<BannedPartsResourcesWindowText>(language, ref LocalizationContainer.BannedPartsResourcesWindowText);
      LocalizationContainer.LoadWindowTexts<ChatWindowText>(language, ref LocalizationContainer.ChatWindowText);
      LocalizationContainer.LoadWindowTexts<ConnectionWindowText>(language, ref LocalizationContainer.ConnectionWindowText);
      LocalizationContainer.LoadWindowTexts<CraftLibraryWindowText>(language, ref LocalizationContainer.CraftLibraryWindowText);
      LocalizationContainer.LoadWindowTexts<ModWindowText>(language, ref LocalizationContainer.ModWindowText);
      LocalizationContainer.LoadWindowTexts<OptionsWindowText>(language, ref LocalizationContainer.OptionsWindowText);
      LocalizationContainer.LoadWindowTexts<ServerListWindowText>(language, ref LocalizationContainer.ServerListWindowText);
      LocalizationContainer.LoadWindowTexts<StatusWindowText>(language, ref LocalizationContainer.StatusWindowText);
      LocalizationContainer.LoadWindowTexts<DisclaimerDialogText>(language, ref LocalizationContainer.DisclaimerDialogText);
      LocalizationContainer.LoadWindowTexts<InstallDialogText>(language, ref LocalizationContainer.InstallDialogText);
      LocalizationContainer.LoadWindowTexts<ScreenshotWindowText>(language, ref LocalizationContainer.ScreenshotWindowText);
      LocalizationContainer.LoadWindowTexts<ScreenText>(language, ref LocalizationContainer.ScreenText);
      LocalizationContainer.LoadWindowTexts<ButtonTooltips>(language, ref LocalizationContainer.ButtonTooltips);
      LocalizationContainer.LoadWindowTexts<UpdateWindowText>(language, ref LocalizationContainer.UpdateWindowText);
      LocalizationContainer.LoadWindowTexts<CompatibleDialogText>(language, ref LocalizationContainer.CompatibleDialogText);
      LocalizationContainer.LoadWindowTexts<RevertDialogText>(language, ref LocalizationContainer.RevertDialogText);
      LocalizationContainer.LoadWindowTexts<ServerListFiltersText>(language, ref LocalizationContainer.ServerListFiltersText);
    }

    public static void RegenerateTranslations()
    {
      string language1 = LocalizationContainer.CurrentLanguage.Clone() as string;
      foreach (string language2 in LocalizationContainer.Languages)
      {
        LocalizationContainer.LoadLanguage(language2);
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.AdminWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "AdminWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.BannedPartsResourcesWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "BannedPartsResourcesWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ChatWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ChatWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ConnectionWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ConnectionWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.CraftLibraryWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "CraftLibraryWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ModWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ModWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.OptionsWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "OptionsWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ServerListWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ServerListWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.StatusWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "StatusWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.DisclaimerDialogText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "DisclaimerDialogText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.InstallDialogText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "InstallDialogText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ScreenshotWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ScreenshotWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ScreenText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ScreenText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ButtonTooltips, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ButtonTooltips.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.UpdateWindowText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "UpdateWindowText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.CompatibleDialogText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "CompatibleDialogText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.RevertDialogText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "RevertDialogText.xml"));
        LunaXmlSerializer.WriteToXmlFile((object) LocalizationContainer.ServerListFiltersText, CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language2, "ServerListFiltersText.xml"));
      }
      LocalizationContainer.LoadLanguage(language1);
    }

    private static void LoadWindowTexts<T>(string language, ref T classToReplace) where T : class, new()
    {
      try
      {
        string path = CommonUtil.CombinePaths(LocalizationContainer.LocalizationFolder, language, classToReplace.GetType().Name + ".xml");
        if (!File.Exists(path))
          LunaXmlSerializer.WriteToXmlFile((object) new T(), path);
        classToReplace = LunaXmlSerializer.ReadXmlFromPath<T>(path);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error reading '{0}.xml' for language '{1}' Details: {2}", (object) classToReplace.GetType().Name, (object) language, (object) ex));
      }
    }
  }
}
