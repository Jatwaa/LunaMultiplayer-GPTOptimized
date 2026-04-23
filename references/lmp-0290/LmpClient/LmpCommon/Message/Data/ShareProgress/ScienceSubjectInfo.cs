// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ScienceSubjectInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using System;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ScienceSubjectInfo
  {
    public string Id;
    public int NumBytes;
    public byte[] Data = new byte[0];

    public ScienceSubjectInfo()
    {
    }

    public ScienceSubjectInfo(ScienceSubjectInfo copyFrom)
    {
      this.Id = string.Copy(copyFrom.Id);
      this.NumBytes = copyFrom.NumBytes;
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      Array.Copy((Array) copyFrom.Data, (Array) this.Data, this.NumBytes);
    }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.Id);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.Data, 0, this.NumBytes);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.Id = lidgrenMsg.ReadString();
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.Data, 0, this.NumBytes);
    }

    public int GetByteCount() => this.Id.GetByteCount() + 4 + this.NumBytes;
  }
}
