// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Admin.AdminSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;

namespace LmpClient.Systems.Admin
{
  public class AdminSystem : MessageSystem<AdminSystem, AdminMessageSender, AdminMessageHandler>
  {
    public string AdminPassword { get; set; } = string.Empty;

    public override string SystemName { get; } = nameof (AdminSystem);

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.AdminPassword = string.Empty;
    }
  }
}
