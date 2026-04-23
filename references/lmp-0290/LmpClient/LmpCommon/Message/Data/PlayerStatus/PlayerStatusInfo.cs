// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.PlayerStatus.PlayerStatusInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.PlayerStatus
{
  public class PlayerStatusInfo
  {
    public string PlayerName;
    public string VesselText;
    public string StatusText;

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.PlayerName);
      lidgrenMsg.Write(this.VesselText);
      lidgrenMsg.Write(this.StatusText);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.PlayerName = lidgrenMsg.ReadString();
      this.VesselText = lidgrenMsg.ReadString();
      this.StatusText = lidgrenMsg.ReadString();
    }

    public int GetByteCount() => this.PlayerName.GetByteCount() + this.VesselText.GetByteCount() + this.StatusText.GetByteCount();
  }
}
