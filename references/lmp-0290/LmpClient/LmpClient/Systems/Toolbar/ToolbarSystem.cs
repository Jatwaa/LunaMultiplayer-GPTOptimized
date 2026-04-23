// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Toolbar.ToolbarSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpClient.Utilities;

namespace LmpClient.Systems.Toolbar
{
  public class ToolbarSystem : System<ToolbarSystem>
  {
    public ToolbarSystem()
    {
      this.Enabled = true;
      // ISSUE: method pointer
      GameEvents.onGUIApplicationLauncherReady.Add(new EventVoid.OnEvent((object) this.ToolbarEvents, __methodptr(EnableToolBar)));
    }

    public ToolbarEvents ToolbarEvents { get; } = new ToolbarEvents();

    public override string SystemName { get; } = nameof (ToolbarSystem);

    public void HandleButtonClick()
    {
      if (!SettingsSystem.CurrentSettings.DisclaimerAccepted)
        DisclaimerDialog.SpawnDialog();
      else
        MainSystem.ToolbarShowGui = !MainSystem.ToolbarShowGui;
    }
  }
}
