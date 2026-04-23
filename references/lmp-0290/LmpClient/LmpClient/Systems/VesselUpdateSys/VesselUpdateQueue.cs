// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselUpdateSys.VesselUpdateQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;
using System;

namespace LmpClient.Systems.VesselUpdateSys
{
  public class VesselUpdateQueue : CachedConcurrentQueue<VesselUpdate, VesselUpdateMsgData>
  {
    protected override void AssignFromMessage(VesselUpdate value, VesselUpdateMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.Name = msgData.Name.Clone() as string;
      value.Type = msgData.Type.Clone() as string;
      value.DistanceTraveled = msgData.DistanceTraveled;
      value.Situation = msgData.Situation.Clone() as string;
      value.Landed = msgData.Landed;
      value.Splashed = msgData.Splashed;
      value.Persistent = msgData.Persistent;
      value.LandedAt = msgData.LandedAt.Clone() as string;
      value.DisplayLandedAt = msgData.DisplayLandedAt.Clone() as string;
      value.MissionTime = msgData.MissionTime;
      value.LaunchTime = msgData.LaunchTime;
      value.LastUt = msgData.LastUt;
      value.RefTransformId = msgData.RefTransformId;
      value.AutoClean = msgData.AutoClean;
      value.AutoCleanReason = msgData.AutoCleanReason.Clone() as string;
      value.WasControllable = msgData.WasControllable;
      value.Stage = msgData.Stage;
      Array.Copy((Array) msgData.Com, (Array) value.Com, 3);
    }
  }
}
