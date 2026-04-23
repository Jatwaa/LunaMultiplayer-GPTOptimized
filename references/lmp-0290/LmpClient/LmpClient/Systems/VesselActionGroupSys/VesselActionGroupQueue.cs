// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselActionGroupSys.VesselActionGroupQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselActionGroupSys
{
  public class VesselActionGroupQueue : 
    CachedConcurrentQueue<VesselActionGroup, VesselActionGroupMsgData>
  {
    protected override void AssignFromMessage(
      VesselActionGroup value,
      VesselActionGroupMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.ActionGroup = (KSPActionGroup) msgData.ActionGroup;
      value.Value = msgData.Value;
    }
  }
}
