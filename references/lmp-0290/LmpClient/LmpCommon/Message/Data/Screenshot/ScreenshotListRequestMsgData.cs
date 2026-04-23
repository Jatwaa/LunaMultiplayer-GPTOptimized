// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Screenshot.ScreenshotListRequestMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using Lidgren.Network;
using LmpCommon.Message.Base;
using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Screenshot
{
  public class ScreenshotListRequestMsgData : ScreenshotBaseMsgData
  {
    public string FolderName;
    public int NumAlreadyOwnedPhotoIds;
    public long[] AlreadyOwnedPhotoIds = new long[0];

    internal ScreenshotListRequestMsgData()
    {
    }

    public override ScreenshotMessageType ScreenshotMessageType => ScreenshotMessageType.ListRequest;

    public override string ClassName { get; } = nameof (ScreenshotListRequestMsgData);

    internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
    {
      base.InternalSerialize(lidgrenMsg);
      lidgrenMsg.Write(this.FolderName);
      lidgrenMsg.Write(this.NumAlreadyOwnedPhotoIds);
      for (int index = 0; index < this.NumAlreadyOwnedPhotoIds; ++index)
        lidgrenMsg.Write(this.AlreadyOwnedPhotoIds[index]);
    }

    internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
    {
      base.InternalDeserialize(lidgrenMsg);
      this.FolderName = lidgrenMsg.ReadString();
      this.NumAlreadyOwnedPhotoIds = lidgrenMsg.ReadInt32();
      if (this.AlreadyOwnedPhotoIds.Length < this.NumAlreadyOwnedPhotoIds)
        this.AlreadyOwnedPhotoIds = new long[this.NumAlreadyOwnedPhotoIds];
      for (int index = 0; index < this.NumAlreadyOwnedPhotoIds; ++index)
        this.AlreadyOwnedPhotoIds[index] = lidgrenMsg.ReadInt64();
    }

    internal override int InternalGetMessageSize() => base.InternalGetMessageSize() + this.FolderName.GetByteCount() + 4 + 8 * this.NumAlreadyOwnedPhotoIds;
  }
}
