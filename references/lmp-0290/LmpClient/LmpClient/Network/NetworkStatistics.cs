// Decompiled with JetBrains decompiler
// Type: LmpClient.Network.NetworkStatistics
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message.Base;
using System;

namespace LmpClient.Network
{
  public class NetworkStatistics
  {
    public static volatile float PingSec;

    public static float AvgPingSec => NetworkMain.ClientConnection.ServerConnection.AverageRoundtripTime;

    public static int SentBytes => NetworkMain.ClientConnection.Statistics.SentBytes;

    public static int ReceivedBytes => NetworkMain.ClientConnection.Statistics.ReceivedBytes;

    public static float TimeOffset => NetworkMain.ClientConnection?.ServerConnection?.RemoteTimeOffset.GetValueOrDefault();

    public static int MessagesInCache => MessageStore.GetMessageCount((Type) null);

    public static int MessageDataInCache => MessageStore.GetMessageDataCount((Type) null);
  }
}
