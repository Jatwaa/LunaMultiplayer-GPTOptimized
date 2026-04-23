// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselResourceSys.VesselResourceQueue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselResourceSys
{
  public class VesselResourceQueue : CachedConcurrentQueue<VesselResource, VesselResourceMsgData>
  {
    protected override void AssignFromMessage(VesselResource value, VesselResourceMsgData msgData)
    {
      value.GameTime = msgData.GameTime;
      value.VesselId = msgData.VesselId;
      value.ResourcesCount = msgData.ResourcesCount;
      if (value.Resources.Length < msgData.ResourcesCount)
        value.Resources = new VesselResourceInfo[msgData.ResourcesCount];
      for (int index = 0; index < msgData.ResourcesCount; ++index)
      {
        if (value.Resources[index] == null)
          value.Resources[index] = new VesselResourceInfo();
        value.Resources[index].Amount = msgData.Resources[index].Amount;
        value.Resources[index].FlowState = msgData.Resources[index].FlowState;
        value.Resources[index].PartFlightId = msgData.Resources[index].PartFlightId;
        value.Resources[index].ResourceName = msgData.Resources[index].ResourceName.Clone() as string;
      }
    }
  }
}
