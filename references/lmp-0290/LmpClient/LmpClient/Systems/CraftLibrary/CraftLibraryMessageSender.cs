// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.CraftLibrary.CraftLibraryMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.CraftLibrary;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.CraftLibrary
{
  public class CraftLibraryMessageSender : SubSystem<CraftLibrarySystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<CraftLibraryCliMsg>(msg))));

    public void SendCraftMsg(CraftEntry craft)
    {
      CraftLibraryDataMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<CraftLibraryDataMsgData>();
      newMessageData.Craft.FolderName = craft.FolderName;
      newMessageData.Craft.CraftName = craft.CraftName;
      newMessageData.Craft.CraftType = craft.CraftType;
      newMessageData.Craft.NumBytes = craft.CraftNumBytes;
      if (newMessageData.Craft.Data.Length < craft.CraftNumBytes)
        newMessageData.Craft.Data = new byte[craft.CraftNumBytes];
      Array.Copy((Array) craft.CraftData, (Array) newMessageData.Craft.Data, craft.CraftNumBytes);
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendRequestFoldersMsg() => this.SendMessage((IMessageData) NetworkMain.CliMsgFactory.CreateNewMessageData<CraftLibraryFoldersRequestMsgData>());

    public void SendRequestCraftListMsg(string folderName)
    {
      CraftLibraryListRequestMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<CraftLibraryListRequestMsgData>();
      newMessageData.FolderName = folderName;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendRequestCraftMsg(CraftBasicEntry craft)
    {
      CraftLibraryDownloadRequestMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<CraftLibraryDownloadRequestMsgData>();
      newMessageData.CraftRequested.FolderName = craft.FolderName;
      newMessageData.CraftRequested.CraftName = craft.CraftName;
      newMessageData.CraftRequested.CraftType = craft.CraftType;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendDeleteCraftMsg(CraftBasicEntry craft)
    {
      CraftLibraryDeleteRequestMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<CraftLibraryDeleteRequestMsgData>();
      newMessageData.CraftToDelete.FolderName = craft.FolderName;
      newMessageData.CraftToDelete.CraftName = craft.CraftName;
      newMessageData.CraftToDelete.CraftType = craft.CraftType;
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
