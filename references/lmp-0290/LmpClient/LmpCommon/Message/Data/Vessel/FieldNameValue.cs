// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.FieldNameValue
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;

namespace LmpCommon.Message.Data.Vessel
{
  public class FieldNameValue
  {
    public string FieldName;
    public string Value;

    public void Serialize(NetOutgoingMessage lidgrenMsg)
    {
      lidgrenMsg.Write(this.FieldName);
      lidgrenMsg.Write(this.Value);
    }

    public void Deserialize(NetIncomingMessage lidgrenMsg)
    {
      this.FieldName = lidgrenMsg.ReadString();
      this.Value = lidgrenMsg.ReadString();
    }

    public int GetByteCount() => this.FieldName.GetByteCount() + this.Value.GetByteCount();
  }
}
