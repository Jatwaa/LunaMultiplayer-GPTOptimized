// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Interface.IMessageData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;

namespace LmpCommon.Message.Interface
{
  public interface IMessageData
  {
    string ClassName { get; }

    ushort MajorVersion { get; }

    ushort MinorVersion { get; }

    ushort BuildVersion { get; }

    long ReceiveTime { get; set; }

    ushort SubType { get; }

    long SentTime { get; set; }

    void Serialize(NetOutgoingMessage lidgrenMsg);

    void Deserialize(NetIncomingMessage lidgrenMsg);

    int GetMessageSize();
  }
}
