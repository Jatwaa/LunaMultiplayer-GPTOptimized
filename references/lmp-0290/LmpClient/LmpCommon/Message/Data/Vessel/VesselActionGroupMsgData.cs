// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselActionGroupMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselActionGroupMsgData : VesselBaseMsgData
  {
    public int ActionGroup;
    public string ActionGroupString;
    public bool Value;

    internal VesselActionGroupMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.ActionGroup;

    public override string ClassName { get; } = nameof (VesselActionGroupMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.ActionGroup);
      lidgrenMsg.Write(this.ActionGroupString);
      lidgrenMsg.Write(this.Value);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.ActionGroup = lidgrenMsg.ReadInt32();
      this.ActionGroupString = lidgrenMsg.ReadString();
      this.Value = lidgrenMsg.ReadBoolean();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 4 + this.ActionGroupString.GetByteCount() + 1;
  }
}
