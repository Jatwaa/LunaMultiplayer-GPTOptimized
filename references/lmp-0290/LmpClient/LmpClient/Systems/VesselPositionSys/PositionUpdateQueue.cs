// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPositionSys.PositionUpdateQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;
using System;

namespace LmpClient.Systems.VesselPositionSys
{
  public class PositionUpdateQueue : 
    CachedConcurrentQueue<VesselPositionUpdate, VesselPositionMsgData>
  {
    protected override void AssignFromMessage(
      VesselPositionUpdate value,
      VesselPositionMsgData msgData)
    {
      value.VesselId = msgData.VesselId;
      value.SubspaceId = msgData.SubspaceId;
      value.BodyIndex = msgData.BodyIndex;
      value.HeightFromTerrain = msgData.HeightFromTerrain;
      value.PingSec = msgData.PingSec;
      value.Landed = msgData.Landed;
      value.Splashed = msgData.Splashed;
      value.GameTimeStamp = msgData.GameTime;
      value.HackingGravity = msgData.HackingGravity;
      Array.Copy((Array) msgData.SrfRelRotation, (Array) value.SrfRelRotation, 4);
      Array.Copy((Array) msgData.LatLonAlt, (Array) value.LatLonAlt, 3);
      Array.Copy((Array) msgData.VelocityVector, (Array) value.VelocityVector, 3);
      Array.Copy((Array) msgData.NormalVector, (Array) value.NormalVector, 3);
      Array.Copy((Array) msgData.Orbit, (Array) value.Orbit, 8);
    }
  }
}
