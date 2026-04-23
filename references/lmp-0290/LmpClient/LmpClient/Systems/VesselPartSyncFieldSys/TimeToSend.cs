// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.VesselPartSyncFieldSys.TimeToSend
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;

namespace LmpClient.Systems.VesselPartSyncFieldSys
{
  public class TimeToSend
  {
    private readonly int _intervalInMs;
    private DateTime _lastSendTime;

    public TimeToSend(int interval)
    {
      this._intervalInMs = interval;
      this._lastSendTime = DateTime.MinValue;
    }

    public bool ReadyToSend()
    {
      if (this._intervalInMs <= 0)
        return true;
      if (!(DateTime.UtcNow - this._lastSendTime > TimeSpan.FromMilliseconds((double) this._intervalInMs)))
        return false;
      this._lastSendTime = DateTime.UtcNow;
      return true;
    }
  }
}
