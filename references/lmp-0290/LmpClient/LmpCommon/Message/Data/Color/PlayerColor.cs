// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Color.PlayerColor
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Color
{
  public class PlayerColor
  {
    public string PlayerName;
    public float[] Color = new float[3];

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.PlayerName);
      for (int index = 0; index < 3; ++index)
        lidgrenMsg.Write(this.Color[index]);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.PlayerName = lidgrenMsg.ReadString();
      for (int index = 0; index < 3; ++index)
        this.Color[index] = lidgrenMsg.ReadFloat();
    }

    public int GetByteCount() => this.PlayerName.GetByteCount() + 12;
  }
}
