// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.CraftLibrary.CraftLibraryDataMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.CraftLibrary
{
  public class CraftLibraryDataMsgData : CraftLibraryBaseMsgData
  {
    public CraftInfo Craft = new CraftInfo();

    internal CraftLibraryDataMsgData()
    {
    }

    public override CraftMessageType CraftMessageType => CraftMessageType.CraftData;

    public override string ClassName { get; } = nameof (CraftLibraryDataMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.Craft.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Craft.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Craft.GetByteCount();
  }
}
