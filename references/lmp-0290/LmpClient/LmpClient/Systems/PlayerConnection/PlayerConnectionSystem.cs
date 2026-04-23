// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.PlayerConnection.PlayerConnectionSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.PlayerConnection
{
  public class PlayerConnectionSystem : 
    MessageSystem<PlayerConnectionSystem, PlayerConnectionMessageSender, PlayerConnectionMessageHandler>
  {
    public override string SystemName { get; } = nameof (PlayerConnectionSystem);
  }
}
