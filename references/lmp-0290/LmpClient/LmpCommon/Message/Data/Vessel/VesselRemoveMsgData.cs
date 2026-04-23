// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Vessel.VesselRemoveMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Vessel
{
  public class VesselRemoveMsgData : VesselBaseMsgData
  {
    public bool AddToKillList;
    public bool KillOnReceive;

    internal VesselRemoveMsgData()
    {
    }

    public override VesselMessageType VesselMessageType => VesselMessageType.Remove;

    public override string ClassName { get; } = nameof (VesselRemoveMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.AddToKillList);
      lidgrenMsg.Write(this.KillOnReceive);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.AddToKillList = lidgrenMsg.ReadBoolean();
      this.KillOnReceive = lidgrenMsg.ReadBoolean();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + 2;
  }
}
