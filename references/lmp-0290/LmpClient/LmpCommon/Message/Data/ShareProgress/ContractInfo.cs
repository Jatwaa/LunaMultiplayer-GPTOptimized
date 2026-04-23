// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.ShareProgress.ContractInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using System;

namespace LmpCommon.Message.Data.ShareProgress
{
  public class ContractInfo
  {
    public Guid ContractGuid;
    public int NumBytes;
    public byte[] Data = new byte[0];

    public ContractInfo()
    {
    }

    public ContractInfo(ContractInfo copyFrom)
    {
      this.ContractGuid = copyFrom.ContractGuid;
      this.NumBytes = copyFrom.NumBytes;
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      Array.Copy((Array) copyFrom.Data, (Array) this.Data, this.NumBytes);
    }

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      GuidUtil.Serialize(this.ContractGuid, lidgrenMsg);
      Common.ThreadSafeCompress((object) this, ref this.Data, ref this.NumBytes);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.Data, 0, this.NumBytes);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.ContractGuid = GuidUtil.Deserialize(lidgrenMsg);
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.Data.Length < this.NumBytes)
        this.Data = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.Data, 0, this.NumBytes);
      Common.ThreadSafeDecompress((object) this, ref this.Data, this.NumBytes, out this.NumBytes);
    }

    public int GetByteCount() => GuidUtil.ByteSize + 4 + this.NumBytes;
  }
}
