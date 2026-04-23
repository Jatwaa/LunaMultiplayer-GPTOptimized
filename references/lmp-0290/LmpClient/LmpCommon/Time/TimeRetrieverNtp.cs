// Decompiled with JetBrains decompiler
// Type: LmpCommon.Time.TimeRetrieverNtp
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace LmpCommon.Time
{
  internal static class TimeRetrieverNtp
  {
    private const int DaysTo1900 = 693595;
    private const long TicksTo1900 = 599266080000000000;
    private const byte NtpDataLength = 48;
    private static byte[] _ntpData = new byte[48];
    private static IPEndPoint _serverAddress;

    internal static DateTime GetNtpTime(string server)
    {
      TimeRetrieverNtp.InitializeStructure();
      TimeRetrieverNtp._serverAddress = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], 123);
      long num1 = Stopwatch.GetTimestamp();
      using (UdpClient udpClient = new UdpClient(TimeRetrieverNtp._serverAddress.AddressFamily))
      {
        udpClient.Connect(TimeRetrieverNtp._serverAddress);
        udpClient.Send(TimeRetrieverNtp._ntpData, TimeRetrieverNtp._ntpData.Length);
        num1 = Stopwatch.GetTimestamp();
        TimeRetrieverNtp._ntpData = udpClient.Receive(ref TimeRetrieverNtp._serverAddress);
        num1 = Stopwatch.GetTimestamp() - num1;
      }
      long num2 = num1 * 10000000L / Stopwatch.Frequency;
      return new DateTime(599266080000000000L + (((long) TimeRetrieverNtp._ntpData[40] << 24 | (long) TimeRetrieverNtp._ntpData[41] << 16 | (long) TimeRetrieverNtp._ntpData[42] << 8 | (long) TimeRetrieverNtp._ntpData[43]) * 10000000L + (((long) TimeRetrieverNtp._ntpData[44] << 24 | (long) TimeRetrieverNtp._ntpData[45] << 16 | (long) TimeRetrieverNtp._ntpData[46] << 8 | (long) TimeRetrieverNtp._ntpData[47]) * 10000000L >> 32)) + num2 / 2L);
    }

    private static void InitializeStructure()
    {
      TimeRetrieverNtp._ntpData[0] = (byte) 27;
      for (int index = 1; index < 48; ++index)
        TimeRetrieverNtp._ntpData[index] = (byte) 0;
    }
  }
}
