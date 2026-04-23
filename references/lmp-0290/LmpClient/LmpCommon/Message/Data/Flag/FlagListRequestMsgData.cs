// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Data.Flag.FlagListRequestMsgData
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Types;

namespace LmpCommon.Message.Data.Flag
{
  public class FlagListRequestMsgData : FlagBaseMsgData
  {
    internal FlagListRequestMsgData()
    {
    }

    public override FlagMessageType FlagMessageType => FlagMessageType.ListRequest;

    public override string ClassName { get; } = nameof (FlagListRequestMsgData);
  }
}
