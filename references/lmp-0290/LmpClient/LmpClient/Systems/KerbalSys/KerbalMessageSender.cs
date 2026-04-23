// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KerbalSys.KerbalMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Kerbal;
using LmpCommon.Message.Interface;
using System;

namespace LmpClient.Systems.KerbalSys
{
  public class KerbalMessageSender : SubSystem<KerbalSystem>, IMessageSender
  {
    private static ConfigNode ConfigNode { get; } = new ConfigNode();

    public void SendMessage(IMessageData msg) => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<KerbalCliMsg>(msg))));

    public void SendKerbalsRequest() => SystemBase.TaskFactory.StartNew((Action) (() => NetworkSender.QueueOutgoingMessage((IMessageBase) NetworkMain.CliMsgFactory.CreateNew<KerbalCliMsg, KerbalsRequestMsgData>())));

    public void SendKerbalRemove(string kerbalName)
    {
      KerbalRemoveMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<KerbalRemoveMsgData>();
      newMessageData.KerbalName = kerbalName;
      this.SendMessage((IMessageData) newMessageData);
    }

    public void SendKerbal(ProtoCrewMember pcm)
    {
      if (pcm == null || VesselCommon.IsSpectating)
        return;
      KerbalMessageSender.ConfigNode.ClearData();
      pcm.Save(KerbalMessageSender.ConfigNode);
      byte[] kerbalBytes = KerbalMessageSender.ConfigNode.Serialize();
      if (kerbalBytes == null || kerbalBytes.Length == 0)
        LunaLog.LogError("[LMP]: Error sending kerbal - bytes are null or 0");
      else
        this.SendKerbalProtoMessage(pcm.name, kerbalBytes);
    }

    private void SendKerbalProtoMessage(string kerbalName, byte[] kerbalBytes)
    {
      if (kerbalBytes != null && (uint) kerbalBytes.Length > 0U)
      {
        KerbalProtoMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<KerbalProtoMsgData>();
        newMessageData.Kerbal.KerbalName = kerbalName;
        newMessageData.Kerbal.KerbalData = kerbalBytes;
        newMessageData.Kerbal.NumBytes = kerbalBytes.Length;
        this.SendMessage((IMessageData) newMessageData);
      }
      else
        LunaLog.LogError("[LMP]: Failed to create byte[] data for kerbal " + kerbalName);
    }
  }
}
