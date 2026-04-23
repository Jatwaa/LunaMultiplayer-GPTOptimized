// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselProtoSys.VesselProtoMessageSender
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Base.Interface;
using LmpClient.Extensions;
using LmpClient.Network;
using LmpClient.Systems.TimeSync;
using LmpClient.Systems.VesselRemoveSys;
using LmpClient.Utilities;
using LmpClient.VesselUtilities;
using LmpCommon.Message.Client;
using LmpCommon.Message.Data.Vessel;
using LmpCommon.Message.Interface;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselProtoSys
{
  public class VesselProtoMessageSender : SubSystem<VesselProtoSystem>, IMessageSender
  {
    private static readonly byte[] VesselSerializedBytes = new byte[10240000];
    private static readonly object VesselArraySyncLock = new object();

    public void SendMessage(IMessageData msg) => NetworkSender.QueueOutgoingMessage((IMessageBase) SystemBase.MessageFactory.CreateNew<VesselCliMsg>(msg));

    public void SendVesselMessage(global::Vessel vessel, bool forceReload = false)
    {
      if (Object.op_Equality((Object) vessel, (Object) null) || vessel.state == 2 || LmpClient.Base.System<VesselRemoveSystem>.Singleton.VesselWillBeKilled(vessel.id))
        return;
      if (!Object.op_Implicit((Object) vessel.orbitDriver))
        LunaLog.LogWarning(string.Format("Cannot send vessel {0} - {1}. It's orbit driver is null!", (object) vessel.vesselName, (object) vessel.id));
      else if (vessel.orbitDriver.Ready())
      {
        vessel.protoVessel = vessel.BackupVessel();
        this.SendVesselMessage(vessel.protoVessel, forceReload);
      }
      else
        CoroutineUtil.StartConditionRoutine(nameof (SendVesselMessage), (Action) (() => this.SendVesselMessage(vessel)), (Func<bool>) (() => vessel.orbitDriver.Ready()), 10f);
    }

    private void SendVesselMessage(ProtoVessel protoVessel, bool forceReload)
    {
      if (protoVessel == null || protoVessel.vesselID == Guid.Empty)
        return;
      SystemBase.TaskFactory.StartNew((Action) (() => this.PrepareAndSendProtoVessel(protoVessel, forceReload)));
    }

    private void PrepareAndSendProtoVessel(ProtoVessel protoVessel, bool forceReload)
    {
      if (protoVessel.vesselID == Guid.Empty)
        return;
      lock (VesselProtoMessageSender.VesselArraySyncLock)
      {
        int numBytes;
        VesselSerializer.SerializeVesselToArray(protoVessel, VesselProtoMessageSender.VesselSerializedBytes, out numBytes);
        if (numBytes > 0)
        {
          VesselProtoMsgData newMessageData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselProtoMsgData>();
          newMessageData.GameTime = TimeSyncSystem.UniversalTime;
          newMessageData.VesselId = protoVessel.vesselID;
          newMessageData.NumBytes = numBytes;
          newMessageData.ForceReload = forceReload;
          if (newMessageData.Data.Length < numBytes)
            Array.Resize<byte>(ref newMessageData.Data, numBytes);
          Array.Copy((Array) VesselProtoMessageSender.VesselSerializedBytes, 0, (Array) newMessageData.Data, 0, numBytes);
          this.SendMessage((IMessageData) newMessageData);
        }
        else if (protoVessel.vesselType == 0)
        {
          LunaLog.Log(string.Format("Serialization of debris vessel: {0} name: {1} failed. Adding to kill list", (object) protoVessel.vesselID, (object) protoVessel.vesselName));
          LmpClient.Base.System<VesselRemoveSystem>.Singleton.KillVessel(protoVessel.vesselID, true, "Serialization of debris failed");
        }
      }
    }
  }
}
