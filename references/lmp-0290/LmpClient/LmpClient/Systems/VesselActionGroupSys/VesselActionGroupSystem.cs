// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselActionGroupSys.VesselActionGroupSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.TimeSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LmpClient.Systems.VesselActionGroupSys
{
  public class VesselActionGroupSystem : 
    MessageSystem<VesselActionGroupSystem, VesselActionGroupMessageSender, VesselActionGroupMessageHandler>
  {
    public ConcurrentDictionary<Guid, VesselActionGroupQueue> VesselActionGroups { get; } = new ConcurrentDictionary<Guid, VesselActionGroupQueue>();

    public static VesselActionGroupEvents VesselActionGroupEvents { get; } = new VesselActionGroupEvents();

    protected override bool ProcessMessagesInUnityThread => false;

    public override string SystemName { get; } = nameof (VesselActionGroupSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.SetupRoutine(new RoutineDefinition(500, RoutineExecution.Update, new Action(this.ProcessVesselActionGroups)));
      // ISSUE: method pointer
      ActionGroupEvent.onActionGroupFired.Add(new EventData<Vessel, KSPActionGroup, bool>.OnEvent((object) VesselActionGroupSystem.VesselActionGroupEvents, __methodptr(ActionGroupFired)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.VesselActionGroups.Clear();
      // ISSUE: method pointer
      ActionGroupEvent.onActionGroupFired.Remove(new EventData<Vessel, KSPActionGroup, bool>.OnEvent((object) VesselActionGroupSystem.VesselActionGroupEvents, __methodptr(ActionGroupFired)));
    }

    private void ProcessVesselActionGroups()
    {
      using (IEnumerator<KeyValuePair<Guid, VesselActionGroupQueue>> enumerator = this.VesselActionGroups.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, VesselActionGroupQueue> current = enumerator.Current;
          while (true)
          {
            VesselActionGroup result;
            if (current.Value.TryPeek(out result) && result.GameTime <= TimeSyncSystem.UniversalTime)
            {
              current.Value.TryDequeue(out result);
              result.ProcessActionGroup();
              current.Value.Recycle(result);
            }
            else
              goto label_5;
          }
        }
      }
    }
  }
}
