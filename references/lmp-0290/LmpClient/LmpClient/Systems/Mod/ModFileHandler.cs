// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Mod.ModFileHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Utilities;
using LmpClient.Windows.Mod;
using LmpCommon.ModFile.Structure;
using LmpCommon.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LmpClient.Systems.Mod
{
  public class ModFileHandler : SubSystem<ModSystem>
  {
    private static readonly StringBuilder Sb = new StringBuilder();

    public bool ParseModFile(ModControlStructure modFileData)
    {
      if (!LmpClient.Base.System<ModSystem>.Singleton.ModControl)
        return true;
      SubSystem<ModSystem>.System.ModControlData = modFileData;
      ModFileHandler.Sb.Length = 0;
      ModFileHandler.SaveCurrentModConfigurationFile();
      ModFileHandler.SetAllPathsToLowercase(modFileData);
      if (!ModFileHandler.CheckFilesAndExpansions(modFileData))
      {
        LunaLog.LogError("[LMP]: Mod check failed!");
        LunaLog.LogError(ModFileHandler.Sb.ToString());
        Window<ModWindow>.Singleton.Display = true;
        return false;
      }
      SubSystem<ModSystem>.System.AllowedParts = modFileData.AllowedParts;
      SubSystem<ModSystem>.System.AllowedResources = modFileData.AllowedResources;
      LunaLog.Log("[LMP]: Mod check passed!");
      return true;
    }

    private static void SetAllPathsToLowercase(ModControlStructure modFileInfo)
    {
      modFileInfo.MandatoryPlugins.ForEach((Action<DllFile>) (m => m.FilePath = m.FilePath.ToLower()));
      modFileInfo.OptionalPlugins.ForEach((Action<DllFile>) (m => m.FilePath = m.FilePath.ToLower()));
      modFileInfo.ForbiddenPlugins.ForEach((Action<ForbiddenDllFile>) (m => m.FilePath = m.FilePath.ToLower()));
    }

    private static void SaveCurrentModConfigurationFile() => LunaXmlSerializer.WriteToXmlFile((object) SubSystem<ModSystem>.System.ModControlData, CommonUtil.CombinePaths(MainSystem.KspPath, "GameData", "LunaMultiplayer", "Data", "LMPModControl.xml"));

    private static bool CheckFilesAndExpansions(ModControlStructure modInfo)
    {
      bool flag = true;
      List<string> installedExpansions = SubSystem<ModSystem>.System.GetInstalledExpansions();
      string[] array = modInfo.RequiredExpansions.Except<string>((IEnumerable<string>) installedExpansions).ToArray<string>();
      if (((IEnumerable<string>) array).Any<string>())
      {
        ModFileHandler.Sb.AppendLine("Missing " + string.Join(", ", array) + " expansion/s!");
        SubSystem<ModSystem>.System.MissingExpansions.AddRange((IEnumerable<string>) array);
        flag = false;
      }
      foreach (KeyValuePair<string, string> dll in SubSystem<ModSystem>.System.DllList)
        flag &= ModFileHandler.CheckExistingFile(modInfo, dll);
      foreach (DllFile mandatoryPlugin in modInfo.MandatoryPlugins)
        flag &= ModFileHandler.CheckMandatoryFile(mandatoryPlugin);
      foreach (MandatoryPart mandatoryPart in modInfo.MandatoryParts)
        flag &= ModFileHandler.CheckMandatoryPart(mandatoryPart);
      return flag;
    }

    private static bool CheckExistingFile(
      ModControlStructure modInfo,
      KeyValuePair<string, string> file)
    {
      ForbiddenDllFile forbiddenDllFile = modInfo.ForbiddenPlugins.FirstOrDefault<ForbiddenDllFile>((Func<ForbiddenDllFile, bool>) (f => f.FilePath == file.Key));
      if (forbiddenDllFile != null)
      {
        ModFileHandler.Sb.AppendLine("Banned file " + file.Key + " exists on client!");
        SubSystem<ModSystem>.System.ForbiddenFilesFound.Add(forbiddenDllFile);
        return false;
      }
      if (modInfo.AllowNonListedPlugins || !modInfo.MandatoryPlugins.All<DllFile>((Func<DllFile, bool>) (f => f.FilePath != file.Key)) || !modInfo.OptionalPlugins.All<DllFile>((Func<DllFile, bool>) (f => f.FilePath != file.Key)))
        return true;
      ModFileHandler.Sb.AppendLine("Server does not allow external plugins and file " + file.Key + " is neither optional nor mandatory!");
      SubSystem<ModSystem>.System.NonListedFilesFound.Add(file.Key);
      return false;
    }

    private static bool CheckMandatoryFile(DllFile item)
    {
      if (!SubSystem<ModSystem>.System.DllList.ContainsKey(item.FilePath))
      {
        ModFileHandler.Sb.AppendLine("Required file " + item.FilePath + " is missing!");
        SubSystem<ModSystem>.System.MandatoryFilesNotFound.Add(item);
        return false;
      }
      if (string.IsNullOrEmpty(item.Sha) || !(SubSystem<ModSystem>.System.DllList[item.FilePath] != item.Sha))
        return true;
      ModFileHandler.Sb.AppendLine("Required file " + item.FilePath + " does not match hash " + item.Sha + "!");
      SubSystem<ModSystem>.System.MandatoryFilesDifferentSha.Add(item);
      return false;
    }

    private static bool CheckMandatoryPart(MandatoryPart requiredPart)
    {
      if (((IEnumerable<AvailablePart>) PartLoader.LoadedPartsList).Any<AvailablePart>((Func<AvailablePart, bool>) (p => p.name == requiredPart.PartName)))
        return true;
      ModFileHandler.Sb.AppendLine("Required part " + requiredPart.PartName + " is missing!");
      SubSystem<ModSystem>.System.MandatoryPartsNotFound.Add(requiredPart);
      return false;
    }
  }
}
