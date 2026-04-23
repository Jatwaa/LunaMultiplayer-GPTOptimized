// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Flag.FlagEvents
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.SettingsSys;

namespace LmpClient.Systems.Flag
{
  public class FlagEvents : SubSystem<FlagSystem>
  {
    public void OnFlagSelect(string data) => FlagEvents.HandleFlagChangeEvent(data);

    public void OnMissionFlagSelect(string data) => FlagEvents.HandleFlagChangeEvent(data);

    private static void HandleFlagChangeEvent(string flagUrl)
    {
      SettingsSystem.CurrentSettings.SelectedFlag = flagUrl;
      SettingsSystem.SaveSettings();
      SubSystem<FlagSystem>.System.SendFlag(flagUrl);
    }
  }
}
