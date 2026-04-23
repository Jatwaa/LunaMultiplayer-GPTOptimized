// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Screenshot.ScreenshotMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpCommon.Message.Data.Screenshot;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Screenshot
{
  public class ScreenshotMessageHandler : SubSystem<ScreenshotSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is ScreenshotBaseMsgData data))
        return;
      switch (data.ScreenshotMessageType)
      {
        case ScreenshotMessageType.FoldersReply:
          ScreenshotFoldersReplyMsgData foldersReplyMsgData = (ScreenshotFoldersReplyMsgData) data;
          for (int index = 0; index < foldersReplyMsgData.NumFolders; ++index)
          {
            SubSystem<ScreenshotSystem>.System.DownloadedImages.TryAdd(foldersReplyMsgData.Folders[index], new ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>());
            SubSystem<ScreenshotSystem>.System.MiniatureImages.TryAdd(foldersReplyMsgData.Folders[index], new ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot>());
          }
          break;
        case ScreenshotMessageType.ListReply:
          ScreenshotListReplyMsgData listReplyMsgData = (ScreenshotListReplyMsgData) data;
          ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary1;
          if (!SubSystem<ScreenshotSystem>.System.MiniatureImages.TryGetValue(listReplyMsgData.FolderName, out concurrentDictionary1))
            break;
          for (int index = 0; index < listReplyMsgData.NumScreenshots; ++index)
          {
            LmpClient.Systems.Screenshot.Screenshot miniImage = ScreenshotMessageHandler.CreateImage(listReplyMsgData.Screenshots[index].DateTaken, (int) listReplyMsgData.Screenshots[index].Width, (int) listReplyMsgData.Screenshots[index].Height, listReplyMsgData.Screenshots[index].Data, listReplyMsgData.Screenshots[index].NumBytes);
            concurrentDictionary1.AddOrUpdate(listReplyMsgData.Screenshots[index].DateTaken, miniImage, (Func<long, LmpClient.Systems.Screenshot.Screenshot, LmpClient.Systems.Screenshot.Screenshot>) ((key, existingVal) => miniImage));
          }
          break;
        case ScreenshotMessageType.ScreenshotData:
          ScreenshotDataMsgData screenshotDataMsgData = (ScreenshotDataMsgData) data;
          LmpClient.Systems.Screenshot.Screenshot image = ScreenshotMessageHandler.CreateImage(screenshotDataMsgData.Screenshot.DateTaken, (int) screenshotDataMsgData.Screenshot.Width, (int) screenshotDataMsgData.Screenshot.Height, screenshotDataMsgData.Screenshot.Data, screenshotDataMsgData.Screenshot.NumBytes);
          ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary2;
          if (!SubSystem<ScreenshotSystem>.System.DownloadedImages.TryGetValue(screenshotDataMsgData.Screenshot.FolderName, out concurrentDictionary2))
            break;
          concurrentDictionary2.AddOrUpdate(screenshotDataMsgData.Screenshot.DateTaken, image, (Func<long, LmpClient.Systems.Screenshot.Screenshot, LmpClient.Systems.Screenshot.Screenshot>) ((key, existingVal) => image));
          break;
        case ScreenshotMessageType.Notification:
          SubSystem<ScreenshotSystem>.System.FoldersWithNewContent.Add(((ScreenshotNotificationMsgData) data).FolderName);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static LmpClient.Systems.Screenshot.Screenshot CreateImage(
      long dateTaken,
      int width,
      int height,
      byte[] data,
      int numBytes)
    {
      LmpClient.Systems.Screenshot.Screenshot image = new LmpClient.Systems.Screenshot.Screenshot()
      {
        DateTaken = dateTaken,
        Width = width,
        Height = height,
        Data = new byte[numBytes]
      };
      Array.Copy((Array) data, (Array) image.Data, numBytes);
      return image;
    }
  }
}
