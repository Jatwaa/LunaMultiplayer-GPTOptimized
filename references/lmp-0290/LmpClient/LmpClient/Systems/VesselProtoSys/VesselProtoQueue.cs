// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProtoQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;
using System;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProtoQueue : CachedConcurrentQueue<VesselProto, VesselProtoMsgData>
  {
    protected override void AssignFromMessage(VesselProto value, VesselProtoMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.NumBytes = msgData.NumBytes;
      value.ForceReload = msgData.ForceReload;
      if (value.RawData.Length < msgData.NumBytes)
        value.RawData = new byte[msgData.NumBytes];
      Array.Copy((Array) msgData.Data, (Array) value.RawData, msgData.NumBytes);
    }
  }
}
