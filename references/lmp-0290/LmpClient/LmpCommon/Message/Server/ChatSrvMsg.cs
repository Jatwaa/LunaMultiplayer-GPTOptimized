// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Server.ChatSrvMsg
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Enums;
using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Server.Base;

namespace LmpCommon.Message.Server
{
  public class ChatSrvMsg : SrvMsgBase<ChatMsgData>
  {
    internal ChatSrvMsg()
    {
    }

    public override string ClassName { get; } = nameof (ChatSrvMsg);

    public override ServerMessageType MessageType => ServerMessageType.Chat;

    protected override int DefaultChannel => 3;

    public override NetDeliveryMethod NetDeliveryMethod => NetDeliveryMethod.ReliableOrdered;
  }
}
