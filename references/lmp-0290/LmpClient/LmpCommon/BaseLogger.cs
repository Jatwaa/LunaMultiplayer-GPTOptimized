// Decompiled with JetBrains decompiler
// Type: LmpCommon.BaseLogger
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Enums;
using System;

namespace LmpCommon
{
  public class BaseLogger
  {
    protected virtual LogLevels LogLevel => LogLevels.Debug;

    protected virtual bool UseUtcTime => false;

    protected virtual void AfterPrint(string line)
    {
    }

    private void WriteLog(LogLevels level, string type, string message)
    {
      if (level > this.LogLevel)
        return;
      string line = this.UseUtcTime ? string.Format("[{0:HH:mm:ss}][{1}]: {2}", (object) DateTime.UtcNow, (object) type, (object) message) : string.Format("[{0:HH:mm:ss}][{1}]: {2}", (object) DateTime.Now, (object) type, (object) message);
      Console.WriteLine(line);
      this.AfterPrint(line);
    }

    public void NetworkVerboseDebug(string message)
    {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.Blue;
      this.WriteLog(LogLevels.VerboseNetworkDebug, "VerboseNetwork", message);
      Console.ResetColor();
    }

    public void NetworkDebug(string message)
    {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.Cyan;
      this.WriteLog(LogLevels.NetworkDebug, nameof (NetworkDebug), message);
      Console.ResetColor();
    }

    public void Debug(string message)
    {
      Console.ForegroundColor = ConsoleColor.Green;
      this.WriteLog(LogLevels.Debug, nameof (Debug), message);
      Console.ResetColor();
    }

    public void Warning(string message)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      this.WriteLog(LogLevels.Normal, nameof (Warning), message);
      Console.ResetColor();
    }

    public void Info(string message)
    {
      Console.BackgroundColor = ConsoleColor.Yellow;
      Console.ForegroundColor = ConsoleColor.Red;
      this.WriteLog(LogLevels.Normal, nameof (Info), message);
      Console.ResetColor();
    }

    public void Normal(string message)
    {
      Console.ForegroundColor = ConsoleColor.Gray;
      this.WriteLog(LogLevels.Normal, "LMP", message);
      Console.ResetColor();
    }

    public void Error(string message)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      this.WriteLog(LogLevels.Normal, nameof (Error), message);
      Console.ResetColor();
    }

    public void Fatal(string message)
    {
      Console.BackgroundColor = ConsoleColor.DarkRed;
      Console.ForegroundColor = ConsoleColor.Red;
      this.WriteLog(LogLevels.Normal, nameof (Fatal), message);
      Console.ResetColor();
    }

    public void ChatMessage(string message)
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      this.WriteLog(LogLevels.Normal, "Chat", message);
      Console.ResetColor();
    }
  }
}
