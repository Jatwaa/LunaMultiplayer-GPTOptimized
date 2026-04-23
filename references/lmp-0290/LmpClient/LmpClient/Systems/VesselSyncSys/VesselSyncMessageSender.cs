// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselSyncSys.VesselSyncMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselProtoSys;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.VesselSyncSys
{
  public class VesselSyncMessageSender : SubSystem<VesselSyncSystem>, IMessageSender
  {
    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselsSyncMsg()
    {
      VesselSyncMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselSyncMsgData>();
      newMessageData.GameTime = TimeSyncSystem.UniversalTime;
      Guid[] array = Enumerable.ToArray<Guid>(Enumerable.Distinct<Guid>(Enumerable.Union<Guid>(Enumerable.Select<global::Vessel, Guid>(Enumerable.Where<global::Vessel>((IEnumerable<global::Vessel>) FlightGlobals.Vessels, (Func<global::Vessel, bool>) (v => Object.op_Inequality((Object) v, (Object) null))), (Func<global::Vessel, Guid>) (v => v.id)), (IEnumerable<Guid>) LmpClient.Base.System<VesselProtoSystem>.Singleton.VesselsUnableToLoad)));
      newMessageData.VesselsCount = array.Length;
      for (int index = 0; index < newMessageData.VesselIds.Length; ++index)
        newMessageData.VesselIds[index] = Guid.Empty;
      if (newMessageData.VesselIds.Length < newMessageData.VesselsCount)
        newMessageData.VesselIds = new Guid[newMessageData.VesselsCount];
      Array.Copy((Array) array, (Array) newMessageData.VesselIds, newMessageData.VesselsCount);
      this.SendMessage((IMessageData) newMessageData);
    }
  }
}
