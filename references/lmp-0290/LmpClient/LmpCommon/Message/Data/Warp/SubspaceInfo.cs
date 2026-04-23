// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Warp.SubspaceInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Warp
{
  public class SubspaceInfo
  {
    public int SubspaceKey;
    public double SubspaceTime;
    public int PlayerCount;
    public string[] Players = new string[0];

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.SubspaceKey);
      lidgrenMsg.Write(this.SubspaceTime);
      lidgrenMsg.Write(this.PlayerCount);
      for (int index = 0; index < this.PlayerCount; ++index)
        lidgrenMsg.Write(this.Players[index]);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.SubspaceKey = lidgrenMsg.ReadInt32();
      this.SubspaceTime = lidgrenMsg.ReadDouble();
      this.PlayerCount = lidgrenMsg.ReadInt32();
      if (this.Players.Length < this.PlayerCount)
        this.Players = new string[this.PlayerCount];
      for (int index = 0; index < this.PlayerCount; ++index)
        this.Players[index] = lidgrenMsg.ReadString();
    }

    public int GetByteCount() => 16 + this.Players.GetByteCount(this.PlayerCount);
  }
}
