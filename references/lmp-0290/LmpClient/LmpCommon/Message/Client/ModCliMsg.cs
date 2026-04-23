// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Client.ModCliMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Client.Base;
using LmpCommon.Message.Data;

namespace LmpCommon.Message.Client
{
  public class ModCliMsg : CliMsgBase<ModMsgData>
  {
    internal ModCliMsg()
    {
    }

    public override string ClassName { get; } = nameof (ModCliMsg);

    public override ClientMessageType MessageType => ClientMessageType.Mod;

    protected override int DefaultChannel => !this.SendReliably() ? 0 : 15;

    public override NetDeliveryMethod NetDeliveryMethod => !this.SendReliably() ? NetDeliveryMethod.UnreliableSequenced : NetDeliveryMethod.ReliableOrdered;

    private bool SendReliably() => ((ModMsgData) this.Data).Reliable;
  }
}
