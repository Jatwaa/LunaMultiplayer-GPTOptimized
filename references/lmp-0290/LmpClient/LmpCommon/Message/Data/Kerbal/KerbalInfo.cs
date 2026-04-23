// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Kerbal.KerbalInfo
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Kerbal
{
  public class KerbalInfo
  {
    public string KerbalName;
    public int NumBytes;
    public byte[] KerbalData = new byte[0];

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.KerbalName);
      Common.ThreadSafeCompress((object) this, ref this.KerbalData, ref this.NumBytes);
      lidgrenMsg.Write(this.NumBytes);
      lidgrenMsg.Write(this.KerbalData, 0, this.NumBytes);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.KerbalName = lidgrenMsg.ReadString();
      this.NumBytes = lidgrenMsg.ReadInt32();
      if (this.KerbalData.Length < this.NumBytes)
        this.KerbalData = new byte[this.NumBytes];
      lidgrenMsg.ReadBytes(this.KerbalData, 0, this.NumBytes);
      Common.ThreadSafeDecompress((object) this, ref this.KerbalData, this.NumBytes, out this.NumBytes);
    }

    public int GetByteCount() => this.KerbalName.GetByteCount() + 4 + this.NumBytes;
  }
}
