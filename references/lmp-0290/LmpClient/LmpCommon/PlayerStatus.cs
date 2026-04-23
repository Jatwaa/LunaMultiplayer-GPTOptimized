// Decompiled with JetBrains decompiler
// Type: LmpCommon.PlayerStatus
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

namespace LmpCommon
{
  public class PlayerStatus
  {
    private string _statusText;
    private string _vesselText;

    public string PlayerName { get; set; }

    public string StatusText
    {
      get => this._statusText;
      set
      {
        this._statusText = value;
        this.DisplayText = !string.IsNullOrEmpty(this._vesselText) ? this._statusText + " (" + this._vesselText + ")" : this._statusText;
      }
    }

    public string VesselText
    {
      get => this._vesselText;
      set
      {
        this._vesselText = value;
        this.DisplayText = !string.IsNullOrEmpty(this._vesselText) ? this._statusText + " (" + this._vesselText + ")" : this._statusText;
      }
    }

    public string DisplayText { get; private set; }
  }
}
