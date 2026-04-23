// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerColorSys.PlayerColorMessageHandler
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Systems.Lock;
using LmpClient.Windows.Status;
using LmpCommon.Enums;
using LmpCommon.Locks;
using LmpCommon.Message.Data.Color;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Types;
using System;
using System.Collections.Concurrent;
using UniLinq;
using UnityEngine;

namespace LmpClient.Systems.PlayerColorSys
{
  public class PlayerColorMessageHandler : SubSystem<PlayerColorSystem>, IMessageHandler
  {
    public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

    public void HandleMessage(IServerMessageBase msg)
    {
      if (!(msg.Data is PlayerColorBaseMsgData data))
        return;
      switch (data.PlayerColorMessageType)
      {
        case PlayerColorMessageType.Reply:
          PlayerColorReplyMsgData colorReplyMsgData = (PlayerColorReplyMsgData) data;
          SubSystem<PlayerColorSystem>.System.PlayerColors.Clear();
          for (int index = 0; index < colorReplyMsgData.PlayerColorsCount; ++index)
          {
            SubSystem<PlayerColorSystem>.System.PlayerColors.Add(colorReplyMsgData.PlayersColors[index].PlayerName, new UnityEngine.Color(colorReplyMsgData.PlayersColors[index].Color[0], colorReplyMsgData.PlayersColors[index].Color[1], colorReplyMsgData.PlayersColors[index].Color[2]));
            Window<StatusWindow>.Singleton.ColorEventHandled = false;
          }
          MainSystem.NetworkState = ClientState.ColorsSynced;
          break;
        case PlayerColorMessageType.Set:
          PlayerColorSetMsgData playerColorSetMsgData = (PlayerColorSetMsgData) data;
          string playerName = playerColorSetMsgData.PlayerColor.PlayerName;
          float[] color = playerColorSetMsgData.PlayerColor.Color;
          LunaLog.Log(string.Format("[LMP]: Color Message, Name: {0} , color: {1}", (object) playerName, (object) color));
          SubSystem<PlayerColorSystem>.System.PlayerColors[playerName] = new UnityEngine.Color(color[0], color[1], color[2]);
          this.UpdateVesselColors(playerName);
          Window<StatusWindow>.Singleton.ColorEventHandled = false;
          break;
      }
    }

    public void UpdateVesselColors(string playerName)
    {
      foreach (Vessel vessel in Enumerable.Where<Vessel>(Enumerable.Select<LockDefinition, Vessel>(LockSystem.LockQuery.GetAllControlLocks(playerName), (Func<LockDefinition, Vessel>) (l => FlightGlobals.FindVessel(l.VesselId))), (Func<Vessel, bool>) (v => Object.op_Inequality((Object) v, (Object) null))))
        SubSystem<PlayerColorSystem>.System.SetVesselOrbitColor(vessel);
    }
  }
}
