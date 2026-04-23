// Decompiled with JetBrains decompiler
// Type: LmpCommon.Message.Base.IpEndpointUtil
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System.Net;

namespace LmpCommon.Message.Base
{
  public static class IpEndpointUtil
  {
    public static int GetByteCount(this IPEndPoint endPointToCheck) => 7;
  }
}
