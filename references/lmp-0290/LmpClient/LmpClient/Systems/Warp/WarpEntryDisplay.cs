// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Warp.WarpEntryDisplay
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LmpClient.Systems.Warp
{
  public class WarpEntryDisplay : SubSystem<WarpSystem>
  {
    public List<SubspaceDisplayEntry> GetSubspaceDisplayEntries()
    {
      if (SettingsSystem.ServerSettings.WarpMode == WarpMode.Subspace)
        WarpEntryDisplay.FillSubspaceDisplayEntriesSubspace();
      else
        WarpEntryDisplay.FillSubspaceDisplayEntriesNoneSubspace();
      return SubSystem<WarpSystem>.System.SubspaceEntries;
    }

    private static void FillSubspaceDisplayEntriesNoneSubspace()
    {
      if (SubSystem<WarpSystem>.System.SubspaceEntries.Count == 1 && SubSystem<WarpSystem>.System.ClientSubspaceList.Keys.Count == SubSystem<WarpSystem>.System.SubspaceEntries[0].Players.Count)
        return;
      SubSystem<WarpSystem>.System.SubspaceEntries.Clear();
      List<string> stringList = new List<string>()
      {
        SettingsSystem.CurrentSettings.PlayerName
      };
      stringList.AddRange((IEnumerable<string>) SubSystem<WarpSystem>.System.ClientSubspaceList.Keys);
      stringList.Sort(new Comparison<string>(WarpEntryDisplay.PlayerSorter));
      SubSystem<WarpSystem>.System.SubspaceEntries.Add(new SubspaceDisplayEntry()
      {
        Players = stringList,
        SubspaceId = 0,
        SubspaceTime = 0.0
      });
    }

    private static void FillSubspaceDisplayEntriesSubspace()
    {
      if (!WarpEntryDisplay.PlayersInSubspacesHaveChanged())
        return;
      SubSystem<WarpSystem>.System.SubspaceEntries.Clear();
      IEnumerable<IGrouping<int, KeyValuePair<string, int>>> groupings = SubSystem<WarpSystem>.System.ClientSubspaceList.GroupBy<KeyValuePair<string, int>, int>((Func<KeyValuePair<string, int>, int>) (s => s.Value));
      SubspaceDisplayEntry subspaceDisplayEntry1 = (SubspaceDisplayEntry) null;
      foreach (IGrouping<int, KeyValuePair<string, int>> source in groupings)
      {
        SubspaceDisplayEntry subspaceDisplayEntry2 = new SubspaceDisplayEntry()
        {
          SubspaceTime = SubSystem<WarpSystem>.System.GetSubspaceTime(source.Key),
          SubspaceId = source.Key,
          Players = source.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (u => u.Key)).ToList<string>()
        };
        if (subspaceDisplayEntry2.SubspaceId == -1)
          subspaceDisplayEntry1 = subspaceDisplayEntry2;
        else
          SubSystem<WarpSystem>.System.SubspaceEntries.Add(subspaceDisplayEntry2);
      }
      SubSystem<WarpSystem>.System.SubspaceEntries = SubSystem<WarpSystem>.System.SubspaceEntries.OrderByDescending<SubspaceDisplayEntry, double>((Func<SubspaceDisplayEntry, double>) (s => s.SubspaceTime)).ToList<SubspaceDisplayEntry>();
      if (subspaceDisplayEntry1 != null)
        SubSystem<WarpSystem>.System.SubspaceEntries.Insert(0, subspaceDisplayEntry1);
    }

    private static bool PlayersInSubspacesHaveChanged()
    {
      if (SubSystem<WarpSystem>.System.SubspaceEntries.Count + 1 != SubSystem<WarpSystem>.System.Subspaces.Count || SubSystem<WarpSystem>.System.SubspaceEntries.Sum<SubspaceDisplayEntry>((Func<SubspaceDisplayEntry, int>) (s => s.Players.Count)) != SubSystem<WarpSystem>.System.ClientSubspaceList.Keys.Count)
        return true;
      for (int index1 = 0; index1 < SubSystem<WarpSystem>.System.SubspaceEntries.Count; ++index1)
      {
        for (int index2 = 0; index2 < SubSystem<WarpSystem>.System.SubspaceEntries[index1].Players.Count; ++index2)
        {
          string player = SubSystem<WarpSystem>.System.SubspaceEntries[index1].Players[index2];
          int subspaceId = SubSystem<WarpSystem>.System.SubspaceEntries[index1].SubspaceId;
          int num;
          if (!SubSystem<WarpSystem>.System.ClientSubspaceList.TryGetValue(player, out num) || num != subspaceId)
            return true;
        }
      }
      return false;
    }

    private static int PlayerSorter(string lhs, string rhs)
    {
      string playerName = SettingsSystem.CurrentSettings.PlayerName;
      return lhs == playerName ? -1 : (rhs == playerName ? 1 : string.CompareOrdinal(lhs, rhs));
    }
  }
}
