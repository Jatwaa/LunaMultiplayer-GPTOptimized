// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Screenshot.ScreenshotDownloadRequestMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Screenshot
{
  public class ScreenshotDownloadRequestMsgData : ScreenshotBaseMsgData
  {
    public string FolderName;
    public long DateTaken;

    internal ScreenshotDownloadRequestMsgData()
    {
    }

    public override ScreenshotMessageType ScreenshotMessageType => ScreenshotMessageType.DownloadRequest;

    public override string ClassName { get; } = nameof (ScreenshotDownloadRequestMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FolderName);
      lidgrenMsg.Write(this.DateTaken);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FolderName = lidgrenMsg.ReadString();
      this.DateTaken = lidgrenMsg.ReadInt64();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.FolderName.GetByteCount() + 8;
  }
}
