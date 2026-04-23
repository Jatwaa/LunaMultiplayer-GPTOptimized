// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.CraftLibrary.CraftLibraryMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.CraftLibrary;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.CraftLibrary
{
  public class CraftLibraryMessageHandler : SubSystem<CraftLibrarySystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is CraftLibraryBaseMsgData data))
        return;
      switch (data.CraftMessageType)
      {
        case CraftMessageType.FoldersReply:
          CraftLibraryMessageHandler.HandleCraftFolders((CraftLibraryFoldersReplyMsgData) data);
          break;
        case CraftMessageType.ListReply:
          CraftLibraryMessageHandler.HandleCraftList((CraftLibraryListReplyMsgData) data);
          break;
        case CraftMessageType.DeleteRequest:
          CraftLibraryMessageHandler.DeleteCraft((CraftLibraryDeleteRequestMsgData) data);
          break;
        case CraftMessageType.CraftData:
          CraftLibraryMessageHandler.SaveNewCraft((CraftLibraryDataMsgData) data);
          break;
        case CraftMessageType.Notification:
          SubSystem<CraftLibrarySystem>.System.FoldersWithNewContent.Add(((CraftLibraryNotificationMsgData) data).FolderName);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static void HandleCraftFolders(CraftLibraryFoldersReplyMsgData foldersMsg)
    {
      SubSystem<CraftLibrarySystem>.System.CraftInfo.Clear();
      for (int index = 0; index < foldersMsg.NumFolders; ++index)
      {
        SubSystem<CraftLibrarySystem>.System.CraftInfo.TryAdd(foldersMsg.Folders[index], new ConcurrentDictionary<string, CraftBasicEntry>());
        SubSystem<CraftLibrarySystem>.System.CraftDownloaded.TryAdd(foldersMsg.Folders[index], new ConcurrentDictionary<string, CraftEntry>());
      }
    }

    private static void HandleCraftList(CraftLibraryListReplyMsgData listMsg)
    {
      ConcurrentDictionary<string, CraftBasicEntry> concurrentDictionary;
      if (!SubSystem<CraftLibrarySystem>.System.CraftInfo.TryGetValue(listMsg.FolderName, out concurrentDictionary))
        return;
      for (int index = 0; index < listMsg.PlayerCraftsCount; ++index)
      {
        CraftBasicEntry craftInfo = new CraftBasicEntry()
        {
          CraftName = listMsg.PlayerCrafts[index].CraftName,
          CraftType = listMsg.PlayerCrafts[index].CraftType,
          FolderName = listMsg.PlayerCrafts[index].FolderName
        };
        concurrentDictionary.AddOrUpdate(listMsg.PlayerCrafts[index].CraftName, craftInfo, (Func<string, CraftBasicEntry, CraftBasicEntry>) ((key, existingVal) => craftInfo));
      }
    }

    private static void DeleteCraft(CraftLibraryDeleteRequestMsgData deleteMsg)
    {
      ConcurrentDictionary<string, CraftBasicEntry> concurrentDictionary1;
      if (SubSystem<CraftLibrarySystem>.System.CraftInfo.TryGetValue(deleteMsg.CraftToDelete.FolderName, out concurrentDictionary1))
      {
        concurrentDictionary1.TryRemove(deleteMsg.CraftToDelete.CraftName, out CraftBasicEntry _);
        if (concurrentDictionary1.Count == 0)
          SubSystem<CraftLibrarySystem>.System.CraftInfo.TryRemove(deleteMsg.CraftToDelete.FolderName, out ConcurrentDictionary<string, CraftBasicEntry> _);
      }
      ConcurrentDictionary<string, CraftEntry> concurrentDictionary2;
      if (!SubSystem<CraftLibrarySystem>.System.CraftDownloaded.TryGetValue(deleteMsg.CraftToDelete.FolderName, out concurrentDictionary2))
        return;
      concurrentDictionary2.TryRemove(deleteMsg.CraftToDelete.CraftName, out CraftEntry _);
    }

    private static void SaveNewCraft(CraftLibraryDataMsgData craftMsg)
    {
      CraftEntry craft = new CraftEntry()
      {
        CraftName = craftMsg.Craft.CraftName,
        CraftType = craftMsg.Craft.CraftType,
        FolderName = craftMsg.Craft.FolderName,
        CraftNumBytes = craftMsg.Craft.NumBytes,
        CraftData = new byte[craftMsg.Craft.NumBytes]
      };
      Array.Copy((Array) craftMsg.Craft.Data, (Array) craft.CraftData, craftMsg.Craft.NumBytes);
      ConcurrentDictionary<string, CraftEntry> concurrentDictionary;
      if (SubSystem<CraftLibrarySystem>.System.CraftDownloaded.TryGetValue(craftMsg.Craft.FolderName, out concurrentDictionary))
        concurrentDictionary.AddOrUpdate(craftMsg.Craft.CraftName, craft, (Func<string, CraftEntry, CraftEntry>) ((key, existingVal) => craft));
      SubSystem<CraftLibrarySystem>.System.SaveCraftToDisk(craft);
    }
  }
}
