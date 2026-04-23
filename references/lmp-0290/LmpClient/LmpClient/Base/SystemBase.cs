// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.SystemBase
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpCommon.Message;
using System.Threading.Tasks;

namespace LmpClient.Base
{
  public abstract class SystemBase
  {
    public static ClientMessageFactory MessageFactory { get; } = new ClientMessageFactory();

    public static TaskFactory TaskFactory { get; } = new TaskFactory();

    public static TaskFactory LongRunTaskFactory { get; } = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
  }
}
