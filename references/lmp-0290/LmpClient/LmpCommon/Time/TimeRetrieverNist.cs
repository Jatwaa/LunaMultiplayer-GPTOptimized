// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.TimeRetrieverNist
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace LmpCommon.Time
{
  internal static class TimeRetrieverNist
  {
    internal static DateTime? GetNistTime()
    {
      using (TcpClient tcpClient = new TcpClient("time.nist.gov", 13))
      {
        using (StreamReader streamReader = new StreamReader((Stream) tcpClient.GetStream()))
        {
          string end = streamReader.ReadToEnd();
          return !string.IsNullOrEmpty(end) && end.Length > 24 ? new DateTime?(DateTime.ParseExact(end.Substring(7, 17), "yy-MM-dd HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture)) : new DateTime?();
        }
      }
    }
  }
}
