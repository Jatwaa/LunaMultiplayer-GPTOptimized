// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Screenshot.ScreenshotNotificationMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Screenshot
{
  public class ScreenshotNotificationMsgData : ScreenshotBaseMsgData
  {
    public string FolderName;

    internal ScreenshotNotificationMsgData()
    {
    }

    public override ScreenshotMessageType ScreenshotMessageType => ScreenshotMessageType.Notification;

    public override string ClassName { get; } = nameof (ScreenshotNotificationMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FolderName);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FolderName = lidgrenMsg.ReadString();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.FolderName.GetByteCount();
  }
}
