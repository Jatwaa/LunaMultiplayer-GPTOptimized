// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Screenshot.ScreenshotDataMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Screenshot
{
  public class ScreenshotDataMsgData : ScreenshotBaseMsgData
  {
    public ScreenshotInfo Screenshot = new ScreenshotInfo();

    internal ScreenshotDataMsgData()
    {
    }

    public override ScreenshotMessageType ScreenshotMessageType => ScreenshotMessageType.ScreenshotData;

    public override string ClassName { get; } = nameof (ScreenshotDataMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      this.Screenshot.Serialize(lidgrenMsg);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.Screenshot.Deserialize(lidgrenMsg);
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.Screenshot.GetByteCount();
  }
}
