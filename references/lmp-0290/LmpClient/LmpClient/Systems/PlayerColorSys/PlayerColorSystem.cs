// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerColorSys.PlayerColorSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Events;
using LmpClient.Systems.Lock;
using LmpClient.Systems.SettingsSys;
using LmpCommon.Locks;
using System.Collections.Generic;
using UnityEngine;

namespace LmpClient.Systems.PlayerColorSys
{
  public class PlayerColorSystem : 
    MessageSystem<PlayerColorSystem, PlayerColorMessageSender, PlayerColorMessageHandler>
  {
    public Color DefaultColor { get; } = XKCDColors.KSPNeutralUIGrey;

    public Dictionary<string, Color> PlayerColors { get; } = new Dictionary<string, Color>();

    public PlayerColorEvents PlayerColorEvents { get; } = new PlayerColorEvents();

    public override string SystemName { get; } = nameof (PlayerColorSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.MessageSender.SendPlayerColorToServer();
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Add(new EventData<Vessel>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnVesselCreated)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Add(new EventData<Vessel, bool>.OnEvent((object) this.PlayerColorEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.OnMapEntered.Add(new EventVoid.OnEvent((object) this.PlayerColorEvents, __methodptr(MapEntered)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Add(new EventData<LockDefinition>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Add(new EventData<LockDefinition>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnLockRelease)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      // ISSUE: method pointer
      GameEvents.onVesselCreate.Remove(new EventData<Vessel>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnVesselCreated)));
      // ISSUE: method pointer
      VesselInitializeEvent.onVesselInitialized.Remove(new EventData<Vessel, bool>.OnEvent((object) this.PlayerColorEvents, __methodptr(VesselInitialized)));
      // ISSUE: method pointer
      GameEvents.OnMapEntered.Remove(new EventVoid.OnEvent((object) this.PlayerColorEvents, __methodptr(MapEntered)));
      // ISSUE: method pointer
      LockEvent.onLockAcquire.Remove(new EventData<LockDefinition>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnLockAcquire)));
      // ISSUE: method pointer
      LockEvent.onLockRelease.Remove(new EventData<LockDefinition>.OnEvent((object) this.PlayerColorEvents, __methodptr(OnLockRelease)));
      this.PlayerColors.Clear();
    }

    public void SetVesselOrbitColor(Vessel vessel)
    {
      if (Object.op_Equality((Object) vessel, (Object) null))
        return;
      string controlLockOwner = LockSystem.LockQuery.GetControlLockOwner(vessel.id);
      PlayerColorSystem.SetOrbitColor(vessel, controlLockOwner == null ? this.DefaultColor : this.GetPlayerColor(controlLockOwner));
    }

    public static Color GenerateRandomColor() => Color.HSVToRGB((float) ((double) Random.Range(0, 24) * 0.6180340051651 % 1.0), Random.Range(0.8f, 0.99f), 0.99f);

    public Color GetPlayerColor(string playerName)
    {
      if (string.IsNullOrEmpty(playerName))
        return this.DefaultColor;
      return playerName == SettingsSystem.CurrentSettings.PlayerName ? SettingsSystem.CurrentSettings.PlayerColor : (this.PlayerColors.ContainsKey(playerName) ? this.PlayerColors[playerName] : this.DefaultColor);
    }

    private static void SetOrbitColor(Vessel vessel, Color colour)
    {
      if (!Object.op_Inequality((Object) vessel, (Object) null) || !Object.op_Inequality((Object) vessel.orbitDriver, (Object) null))
        return;
      vessel.orbitDriver.orbitColor = colour;
      if (Object.op_Implicit((Object) vessel.orbitDriver.Renderer))
        vessel.orbitDriver.Renderer.SetColor(colour);
    }
  }
}
