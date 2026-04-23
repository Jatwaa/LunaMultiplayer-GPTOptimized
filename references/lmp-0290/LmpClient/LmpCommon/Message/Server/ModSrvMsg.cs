// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.ModSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data;
using LmpCommon.Message.Server.Base;

namespace LmpCommon.Message.Server
{
  public class ModSrvMsg : SrvMsgBase<ModMsgData>
  {
    internal ModSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (ModSrvMsg);

    public override ServerMessageType MessageType => ServerMessageType.Mod;

    protected override int DefaultChannel => !this.SendReliably() ? 0 : 15;

    public override NetDeliveryMethod NetDeliveryMethod => !this.SendReliably() ? NetDeliveryMethod.UnreliableSequenced : NetDeliveryMethod.ReliableOrdered;

    private bool SendReliably() => ((ModMsgData) this.Data).Reliable;
  }
}
