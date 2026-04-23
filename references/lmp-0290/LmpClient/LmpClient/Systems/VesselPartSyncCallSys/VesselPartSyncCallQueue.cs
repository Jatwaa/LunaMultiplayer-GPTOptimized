// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncCallSys.VesselPartSyncCallQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselPartSyncCallSys
{
  public class VesselPartSyncCallQueue : 
    CachedConcurrentQueue<VesselPartSyncCall, VesselPartSyncCallMsgData>
  {
    protected override void AssignFromMessage(
      VesselPartSyncCall value,
      VesselPartSyncCallMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.PartFlightId = msgData.PartFlightId;
      value.ModuleName = msgData.ModuleName.Clone() as string;
      value.MethodName = msgData.MethodName.Clone() as string;
    }
  }
}
