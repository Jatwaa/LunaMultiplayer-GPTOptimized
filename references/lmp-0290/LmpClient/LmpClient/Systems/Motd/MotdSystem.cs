// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Motd.MotdSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.Motd
{
  public class MotdSystem : MessageSystem<MotdSystem, MotdMessageSender, MotdMessageHandler>
  {
    public override string SystemName { get; } = nameof (MotdSystem);

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.MessageSender.SendMotdRequest();
    }
  }
}
