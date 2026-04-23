// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Interface.IMessageBase
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;

namespace LmpCommon.Message.Interface
{
  public interface IMessageBase
  {
    string ClassName { get; }

    IMessageData Data { get; }

    bool VersionMismatch { get; set; }

    NetDeliveryMethod NetDeliveryMethod { get; }

    int Channel { get; }

    void SetData(IMessageData data);

    IMessageData GetMessageData(ushort subType);

    void Serialize(NetOutgoingMessage lidgrenMsg);

    void Recycle();

    int GetMessageSize();
  }
}
