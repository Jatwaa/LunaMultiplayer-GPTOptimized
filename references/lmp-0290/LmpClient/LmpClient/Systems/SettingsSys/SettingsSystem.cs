// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.SettingsSys.SettingsSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Network;
using LmpCommon.Enums;
using System.Text;

namespace LmpClient.Systems.SettingsSys
{
  public class SettingsSystem : 
    MessageSystem<SettingsSystem, SettingsMessageSender, SettingsMessageHandler>
  {
    private static readonly StringBuilder Builder = new StringBuilder();

    public static SettingStructure CurrentSettings { get; }

    public static SettingsServerStructure ServerSettings { get; private set; } = new SettingsServerStructure();

    public override string SystemName { get; } = nameof (SettingsSystem);

    static SettingsSystem() => SettingsSystem.CurrentSettings = SettingsReadSaveHandler.ReadSettings();

    protected override void OnDisabled()
    {
      base.OnDisabled();
      SettingsSystem.ServerSettings = new SettingsServerStructure();
    }

    public static void SaveSettings() => SettingsReadSaveHandler.SaveSettings(SettingsSystem.CurrentSettings);

    public static bool ValidateSettings()
    {
      SettingsSystem.Builder.Length = 0;
      bool flag = true;
      if (SettingsSystem.ServerSettings.TerrainQuality != TerrainQuality.Ignore && SettingsSystem.ServerSettings.TerrainQuality != (TerrainQuality) PQSCache.PresetList.presetIndex)
      {
        flag = false;
        SettingsSystem.Builder.Append(string.Format("Your terrain quality: {0} does not match the server quality: {1}.", (object) (TerrainQuality) PQSCache.PresetList.presetIndex, (object) SettingsSystem.ServerSettings.TerrainQuality));
      }
      if (!flag)
        NetworkConnection.Disconnect(SettingsSystem.Builder.ToString());
      return flag;
    }
  }
}
