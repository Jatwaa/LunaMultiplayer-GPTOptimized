// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Mod.ModSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Expansions;
using LmpClient.Localization;
using LmpClient.Utilities;
using LmpCommon;
using LmpCommon.ModFile.Structure;
using LmpCommon.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LmpClient.Systems.Mod
{
  public class ModSystem : LmpClient.Base.System<ModSystem>
  {
    private static readonly FieldInfo ExpansionsInfo = typeof (ExpansionsLoader).GetField("expansionsInfo", BindingFlags.Static | BindingFlags.NonPublic);

    public Dictionary<string, string> DllList { get; } = new Dictionary<string, string>();

    public bool ModControl { get; set; } = true;

    public ModControlStructure ModControlData { get; set; }

    public List<string> AllowedParts { get; set; } = new List<string>();

    public List<string> AllowedResources { get; set; } = new List<string>();

    public List<string> MissingExpansions { get; } = new List<string>();

    public List<ForbiddenDllFile> ForbiddenFilesFound { get; } = new List<ForbiddenDllFile>();

    public List<string> NonListedFilesFound { get; } = new List<string>();

    public List<DllFile> MandatoryFilesNotFound { get; } = new List<DllFile>();

    public List<DllFile> MandatoryFilesDifferentSha { get; } = new List<DllFile>();

    public List<MandatoryPart> MandatoryPartsNotFound { get; } = new List<MandatoryPart>();

    public ModFileHandler ModFileHandler { get; } = new ModFileHandler();

    public override string SystemName { get; } = nameof (ModSystem);

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.Clear();
    }

    public override int ExecutionOrder => -2147483647;

    public void Clear()
    {
      this.ModControl = true;
      this.AllowedParts.Clear();
      this.AllowedResources.Clear();
      this.MissingExpansions.Clear();
      this.ForbiddenFilesFound.Clear();
      this.NonListedFilesFound.Clear();
      this.MandatoryFilesNotFound.Clear();
      this.MandatoryFilesDifferentSha.Clear();
      this.MandatoryPartsNotFound.Clear();
      this.ModControlData = (ModControlStructure) null;
    }

    public void BuildDllFileList()
    {
      this.DllList.Clear();
      foreach (string modFile in ModSystem.GetModFiles())
      {
        string sha256FileHash = Common.CalculateSha256FileHash(modFile);
        this.DllList.Add(ModSystem.GetRelativePath(modFile), sha256FileHash);
      }
    }

    public void GenerateModControlFile(bool appendSha)
    {
      ModControlStructure objectToSerialize = new ModControlStructure()
      {
        RequiredExpansions = this.GetInstalledExpansions()
      };
      objectToSerialize.AllowedParts.AddRange(((IEnumerable<AvailablePart>) PartLoader.LoadedPartsList).Select<AvailablePart, string>((Func<AvailablePart, string>) (p => p.name)));
      objectToSerialize.AllowedResources.AddRange(((IEnumerable) PartResourceLibrary.Instance.resourceDefinitions).Cast<PartResourceDefinition>().Select<PartResourceDefinition, string>((Func<PartResourceDefinition, string>) (r => r.name)));
      foreach (string modFile in ModSystem.GetModFiles())
        objectToSerialize.OptionalPlugins.Add(new DllFile()
        {
          FilePath = ModSystem.GetRelativePath(modFile),
          Sha = appendSha ? Common.CalculateSha256FileHash(modFile) : string.Empty,
          Text = Path.GetFileNameWithoutExtension(modFile) + ". Version: " + FileVersionInfo.GetVersionInfo(modFile).FileVersion
        });
      LunaXmlSerializer.WriteToXmlFile((object) objectToSerialize, CommonUtil.CombinePaths(MainSystem.KspPath, "LMPModControl.xml"));
      LunaScreenMsg.PostScreenMessage(LocalizationContainer.ScreenText.ModFileGenerated, 5f, (ScreenMessageStyle) 0);
    }

    public void CheckCommonStockParts()
    {
      int num1 = 0;
      int num2 = 0;
      ModControlStructure modFile = new ModControlStructure();
      modFile.SetDefaultAllowedParts();
      modFile.SetDefaultAllowedResources();
      LunaLog.Log("[LMP]: Missing parts start");
      foreach (AvailablePart availablePart in ((IEnumerable<AvailablePart>) PartLoader.LoadedPartsList).Where<AvailablePart>((Func<AvailablePart, bool>) (p => !modFile.AllowedParts.Contains(p.name))))
      {
        ++num1;
        LunaLog.Log("[LMP]: Missing part: '" + availablePart.name + "'");
      }
      LunaLog.Log("[LMP]: Missing parts end");
      LunaLog.Log("[LMP]: Missing resources start");
      foreach (string str in ((IEnumerable) PartResourceLibrary.Instance.resourceDefinitions).Cast<PartResourceDefinition>().Select<PartResourceDefinition, string>((Func<PartResourceDefinition, string>) (r => r.name)).Where<string>((Func<string, bool>) (r => !modFile.AllowedResources.Contains(r))))
      {
        ++num2;
        LunaLog.Log("[LMP]: Missing resource: '" + str + "'");
      }
      LunaLog.Log("[LMP]: Missing resources end");
      if (num1 > 0 && num2 <= 0)
        LunaScreenMsg.PostScreenMessage(string.Format("{0} missing part(s) from Common.dll printed to log ({1} total)", (object) num1, (object) PartLoader.LoadedPartsList.Count), 5f, (ScreenMessageStyle) 0);
      else if (num1 <= 0 && num2 <= 0)
        LunaScreenMsg.PostScreenMessage("No missing parts/resources from Common.dll", 5f, (ScreenMessageStyle) 0);
      else if (num1 <= 0 && num2 > 0)
        LunaScreenMsg.PostScreenMessage(string.Format("{0} missing resources from Common.dll printed to log ({1} total)", (object) num2, (object) PartResourceLibrary.Instance.resourceDefinitions.Count), 5f, (ScreenMessageStyle) 0);
      else
        LunaScreenMsg.PostScreenMessage(string.Format("{0} missing part(s) from Common.dll printed to log ({1} total). ", (object) num1, (object) PartLoader.LoadedPartsList.Count) + string.Format("{0} missing resources from Common.dll printed to log ({1} total)", (object) num2, (object) PartResourceLibrary.Instance.resourceDefinitions.Count), 5f, (ScreenMessageStyle) 0);
    }

    public IEnumerable<string> GetBannedPartsFromPartNames(IEnumerable<string> partNames) => partNames.Where<string>((Func<string, bool>) (p => !this.ModControlData.AllowedParts.Contains(p))).ToList<string>().Distinct<string>();

    public IEnumerable<string> GetBannedResourcesFromResourceNames(
      IEnumerable<string> resourceNames)
    {
      return resourceNames.Where<string>((Func<string, bool>) (r => !this.ModControlData.AllowedResources.Contains(r))).ToList<string>().Distinct<string>();
    }

    public List<string> GetInstalledExpansions()
    {
      object obj = ModSystem.ExpansionsInfo?.GetValue((object) ExpansionsLoader.Instance);
      return obj != null && obj.GetType().GetProperty("Keys")?.GetValue(obj, (object[]) null) is ICollection<string> source ? source.ToList<string>() : (List<string>) null;
    }

    private static IEnumerable<string> GetModFiles()
    {
      string gameDataDir = CommonUtil.CombinePaths(MainSystem.KspPath, "GameData");
      string[] strArray1 = Directory.GetDirectories(gameDataDir);
      for (int index1 = 0; index1 < strArray1.Length; ++index1)
      {
        string modDirectory = strArray1[index1];
        string relPathFolder = modDirectory.Substring(modDirectory.ToLower().IndexOf("gamedata", StringComparison.Ordinal) + 9).Replace("\\", "/");
        if (!relPathFolder.StartsWith("squad", StringComparison.OrdinalIgnoreCase) && !relPathFolder.StartsWith("lunamultiplayer", StringComparison.OrdinalIgnoreCase))
        {
          string[] filesInModFolder = Directory.GetFiles(modDirectory, "*.dll", SearchOption.AllDirectories);
          string[] strArray2 = filesInModFolder;
          for (int index2 = 0; index2 < strArray2.Length; ++index2)
          {
            string file = strArray2[index2];
            yield return file;
            file = (string) null;
          }
          strArray2 = (string[]) null;
          relPathFolder = (string) null;
          filesInModFolder = (string[]) null;
          modDirectory = (string) null;
        }
      }
      strArray1 = (string[]) null;
    }

    private static string GetRelativePath(string file) => file.ToLowerInvariant().Substring(file.ToLowerInvariant().IndexOf("gamedata", StringComparison.Ordinal) + 9).Replace('\\', '/');
  }
}
