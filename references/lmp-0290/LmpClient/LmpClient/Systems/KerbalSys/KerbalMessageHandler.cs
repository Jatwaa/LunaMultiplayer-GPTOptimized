// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.KerbalSys.KerbalMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Kerbal;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System.Collections.Concurrent;

namespace LmpClient.Systems.KerbalSys
{
  public class KerbalMessageHandler : SubSystem<KerbalSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is KerbalBaseMsgData data))
        return;
      switch (data.KerbalMessageType)
      {
        case KerbalMessageType.Reply:
          KerbalMessageHandler.HandleKerbalReply(data as KerbalReplyMsgData);
          break;
        case KerbalMessageType.Proto:
          KerbalMessageHandler.HandleKerbalProto(data as KerbalProtoMsgData);
          break;
        case KerbalMessageType.Remove:
          SubSystem<KerbalSystem>.System.KerbalsToRemove.Enqueue(((KerbalRemoveMsgData) data).KerbalName);
          break;
        default:
          LunaLog.LogError("[LMP]: Invalid Kerbal message type");
          break;
      }
    }

    private static void HandleKerbalProto(KerbalProtoMsgData messageData) => KerbalMessageHandler.ProcessKerbal(messageData.Kerbal.KerbalData, messageData.Kerbal.NumBytes);

    private static void ProcessKerbal(byte[] kerbalData, int numBytes)
    {
      ConfigNode configNode = kerbalData.DeserializeToConfigNode(numBytes);
      if (configNode != null)
        SubSystem<KerbalSystem>.System.KerbalsToProcess.Enqueue(configNode);
      else
        LunaLog.LogError("[LMP]: Failed to load kerbal!");
    }

    private static void HandleKerbalReply(KerbalReplyMsgData messageData)
    {
      for (int index = 0; index < messageData.KerbalsCount; ++index)
        KerbalMessageHandler.ProcessKerbal(messageData.Kerbals[index].KerbalData, messageData.Kerbals[index].NumBytes);
      LunaLog.Log("[LMP]: Kerbals Synced!");
      MainSystem.NetworkState = ClientState.KerbalsSynced;
    }
  }
}
