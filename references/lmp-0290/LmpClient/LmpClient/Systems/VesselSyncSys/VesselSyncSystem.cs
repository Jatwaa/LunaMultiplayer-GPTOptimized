// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselSyncSys.VesselSyncSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using System;
using UnityEngine;

namespace LmpClient.Systems.VesselSyncSys
{
  public class VesselSyncSystem : 
    MessageSystem<VesselSyncSystem, VesselSyncMessageSender, VesselSyncMessageHandler>
  {
    public bool UpdateSystemReady => this.Enabled && (double) Time.timeSinceLevelLoad > 1.0;

    public override string SystemName { get; } = nameof (VesselSyncSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(10000, RoutineExecution.Update, new Action(this.SendCurrentVesselIds)));
    }

    private void SendCurrentVesselIds()
    {
      if (!this.UpdateSystemReady)
        return;
      this.MessageSender.SendVesselsSyncMsg();
    }
  }
}
