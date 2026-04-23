// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.UniverseConverter
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Extensions;
using LmpCommon;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LmpClient.Utilities
{
  public class UniverseConverter
  {
    private static string SavesFolder { get; } = CommonUtil.CombinePaths(MainSystem.KspPath, "saves");

    public static void GenerateUniverse(string saveName)
    {
      string path1 = CommonUtil.CombinePaths(MainSystem.KspPath, "Universe");
      if (Directory.Exists(path1))
        Directory.Delete(path1, true);
      string path2 = CommonUtil.CombinePaths(UniverseConverter.SavesFolder, saveName);
      if (!Directory.Exists(path2))
      {
        LunaLog.Log("[LMP]: Failed to generate a LMP universe for '" + saveName + "', Save directory doesn't exist");
        LunaScreenMsg.PostScreenMessage("Failed to generate a LMP universe for '" + saveName + "', Save directory doesn't exist", 5f, (ScreenMessageStyle) 0);
      }
      else
      {
        string path3 = CommonUtil.CombinePaths(path2, "persistent.sfs");
        if (!File.Exists(path3))
        {
          LunaLog.Log("[LMP]: Failed to generate a LMP universe for '" + saveName + "', persistent.sfs doesn't exist");
          LunaScreenMsg.PostScreenMessage("Failed to generate a LMP universe for '" + saveName + "', persistent.sfs doesn't exist", 5f, (ScreenMessageStyle) 0);
        }
        else
        {
          Directory.CreateDirectory(path1);
          string path4 = CommonUtil.CombinePaths(path1, "Vessels");
          Directory.CreateDirectory(path4);
          string path5 = CommonUtil.CombinePaths(path1, "Scenarios");
          Directory.CreateDirectory(path5);
          string path6 = CommonUtil.CombinePaths(path1, "Kerbals");
          Directory.CreateDirectory(path6);
          ConfigNode configNode1 = ConfigNode.Load(path3);
          if (configNode1 == null)
          {
            LunaLog.Log("[LMP]: Failed to generate a LMP universe for '" + saveName + "', failed to load persistent data");
            LunaScreenMsg.PostScreenMessage("Failed to generate a LMP universe for '" + saveName + "', failed to load persistent data", 5f, (ScreenMessageStyle) 0);
          }
          else
          {
            ConfigNode node1 = configNode1.GetNode("GAME");
            if (node1 == null)
            {
              LunaLog.Log("[LMP]: Failed to generate a LMP universe for '" + saveName + "', failed to load game data");
              LunaScreenMsg.PostScreenMessage("Failed to generate a LMP universe for '" + saveName + "', failed to load game data", 5f, (ScreenMessageStyle) 0);
            }
            else
            {
              ConfigNode node2 = node1.GetNode("FLIGHTSTATE");
              if (node2 == null)
              {
                LunaLog.Log("[LMP]: Failed to generate a LMP universe for '" + saveName + "', failed to load flight state data");
                LunaScreenMsg.PostScreenMessage("Failed to generate a LMP universe for '" + saveName + "', failed to load flight state data", 5f, (ScreenMessageStyle) 0);
              }
              else
              {
                File.WriteAllText(CommonUtil.CombinePaths(path1, "Subspace.txt"), "0:" + node2.GetValue("UT"));
                ConfigNode[] nodes1 = node2.GetNodes("VESSEL");
                if (nodes1 != null)
                {
                  foreach (ConfigNode node3 in nodes1)
                  {
                    string guidString = Common.ConvertConfigStringToGuidString(node3.GetValue("pid"));
                    LunaLog.Log("[LMP]: Saving vessel " + guidString + ", Name: " + node3.GetValue("name"));
                    File.WriteAllText(CommonUtil.CombinePaths(path4, guidString + ".txt"), Encoding.UTF8.GetString(node3.Serialize()));
                  }
                }
                ConfigNode[] nodes2 = node1.GetNodes("SCENARIO");
                if (nodes2 != null)
                {
                  foreach (ConfigNode node4 in nodes2)
                  {
                    string str = node4.GetValue("name");
                    if (!string.IsNullOrEmpty(str))
                    {
                      LunaLog.Log("[LMP]: Saving scenario: " + str);
                      File.WriteAllText(CommonUtil.CombinePaths(path5, str + ".txt"), Encoding.UTF8.GetString(node4.Serialize()));
                    }
                  }
                }
                ConfigNode[] nodes3 = node1.GetNode("ROSTER").GetNodes("KERBAL");
                if (nodes3 != null)
                {
                  foreach (ConfigNode configNode2 in nodes3)
                  {
                    string str = configNode2.GetValue("name");
                    LunaLog.Log("[LMP]: Saving kerbal: " + str);
                    configNode2.Save(CommonUtil.CombinePaths(path6, str + ".txt"));
                  }
                }
                LunaLog.Log("[LMP]: Generated KSP_folder/Universe from " + saveName);
                LunaScreenMsg.PostScreenMessage("Generated KSP_folder/Universe from " + saveName, 5f, (ScreenMessageStyle) 0);
              }
            }
          }
        }
      }
    }

    public static IEnumerable<string> GetSavedNames()
    {
      List<string> savedNames = new List<string>();
      foreach (string directory in Directory.GetDirectories(UniverseConverter.SavesFolder))
      {
        string str1 = directory;
        if ((int) directory[directory.Length - 1] == (int) Path.DirectorySeparatorChar)
          str1 = directory.Substring(0, directory.Length - 2);
        string str2 = str1.Substring(str1.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        int num;
        if (str2.ToLower() != "training" && str2.ToLower() != "scenarios")
          num = File.Exists(CommonUtil.CombinePaths(directory, "persistent.sfs")) ? 1 : 0;
        else
          num = 0;
        if (num != 0)
          savedNames.Add(str2);
      }
      return (IEnumerable<string>) savedNames;
    }
  }
}
