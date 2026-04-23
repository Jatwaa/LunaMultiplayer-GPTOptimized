// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Screenshot.ScreenshotMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Screenshot;
using LmpCommon.Message.Interface;
using LmpCommon.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UniLinq;

namespace LmpClient.Systems.Screenshot
{
  public class ScreenshotMessageSender : SubSystem<ScreenshotSystem>, IMessageSender
  {
    public static readonly Dictionary<string, DateTime> RequestedImages = new Dictionary<string, DateTime>();

    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<ScreenshotCliMsg>(msg))));

    public void SendScreenshot(byte[] data)
    {
      ScreenshotDataMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ScreenshotDataMsgData>();
      newMessageData.Screenshot.DateTaken = LunaNetworkTime.UtcNow.ToBinary();
      newMessageData.Screenshot.NumBytes = data.Length;
      if (newMessageData.Screenshot.Data.Length < newMessageData.Screenshot.NumBytes)
        newMessageData.Screenshot.Data = new byte[newMessageData.Screenshot.NumBytes];
      Array.Copy((Array) data, (Array) newMessageData.Screenshot.Data, newMessageData.Screenshot.NumBytes);
      this.SendMessage((IMessageData) newMessageData);
    }

    public void RequestFolders() => this.SendMessage((IMessageData) NetworkMain.CliMsgFactory.CreateNewMessageData<ScreenshotFoldersRequestMsgData>());

    public void RequestMiniatures(string folderName)
    {
      ScreenshotListRequestMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ScreenshotListRequestMsgData>();
      newMessageData.FolderName = folderName;
      ConcurrentDictionary<long, LmpClient.Systems.Screenshot.Screenshot> concurrentDictionary;
      if (SubSystem<ScreenshotSystem>.System.MiniatureImages.TryGetValue(folderName, out concurrentDictionary))
      {
        newMessageData.AlreadyOwnedPhotoIds = Enumerable.ToArray<long>((IEnumerable<long>) concurrentDictionary.Keys);
        newMessageData.NumAlreadyOwnedPhotoIds = concurrentDictionary.Count;
      }
      else
        newMessageData.NumAlreadyOwnedPhotoIds = 0;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void RequestImage(string folderName, long dateTaken)
    {
      if (!ScreenshotMessageSender.RequestedImages.ContainsKey(string.Format("folderName_{0}", (object) dateTaken)) || ScreenshotMessageSender.RequestedImages[string.Format("folderName_{0}", (object) dateTaken)] < DateTime.UtcNow - TimeSpan.FromSeconds(30.0))
      {
        ScreenshotDownloadRequestMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<ScreenshotDownloadRequestMsgData>();
        newMessageData.FolderName = folderName;
        newMessageData.DateTaken = dateTaken;
        this.SendMessage((IMessageData) newMessageData);
      }
      if (!ScreenshotMessageSender.RequestedImages.ContainsKey(string.Format("folderName_{0}", (object) dateTaken)))
        ScreenshotMessageSender.RequestedImages.Add(string.Format("folderName_{0}", (object) dateTaken), LunaComputerTime.UtcNow);
      else
        ScreenshotMessageSender.RequestedImages[string.Format("folderName_{0}", (object) dateTaken)] = LunaComputerTime.UtcNow;
    }
  }
}
