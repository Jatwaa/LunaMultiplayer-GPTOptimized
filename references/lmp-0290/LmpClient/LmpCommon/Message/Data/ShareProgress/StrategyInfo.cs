// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.StrategyInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using System;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class StrategyInfo
  {
    public string Name;
    public int NumBytes;
    public byte[] Data = new byte[0];

    public StrategyInfo()
    {
    }

    public StrategyInfo(StrategyInfo copyFrom)
    {
      this.Name = string.Copy(copyFrom.Name);
      this.NumBytes = copyFrom.NumBytes;
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      Array.Copy((Array) copyFrom.Data, (Array) this.Data, this.NumBytes);
    }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.Name);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.Data, 0, this.NumBytes);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.Name = lidgrenMsg.ReadString();
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.Data, 0, this.NumBytes);
    }

    public int GetByteCount() => this.Name.GetByteCount() + 4 + this.NumBytes;
  }
}
