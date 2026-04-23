// Decompiled with JetBrains decompiler
// Type: LmpClient.Base.MessageSystem`3
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base.Interface;
using LmpClient.Network;
using LmpCommon.Message.Interface;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Base
{
  public abstract class MessageSystem<T, TS, TH> : LmpClient.Base.System<T>
    where T : LmpClient.Base.System<T>, new()
    where TS : class, IMessageSender, new()
    where TH : class, IMessageHandler, new()
  {
    protected virtual bool ProcessMessagesInUnityThread => true;

    public TS MessageSender { get; } = new TS();

    public TH MessageHandler { get; } = new TH();

    public virtual void EnqueueMessage(IServerMessageBase msg)
    {
      if (this.ProcessMessagesInUnityThread)
        this.MessageHandler.IncomingMessages.Enqueue(msg);
      else
        SystemBase.TaskFactory.StartNew((Action) (() => this.HandleMessage(msg)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      if (!this.ProcessMessagesInUnityThread)
        return;
      this.MessageHandler.IncomingMessages = new ConcurrentQueue<IServerMessageBase>();
    }

    protected override void OnEnabled()
    {
      base.OnEnabled();
      if (!this.ProcessMessagesInUnityThread)
        return;
      this.SetupRoutine(new RoutineDefinition(0, RoutineExecution.Update, new Action(this.ReadAndHandleAllReceivedMessages)));
    }

    private void ReadAndHandleAllReceivedMessages()
    {
      IServerMessageBase result;
      while (this.MessageHandler.IncomingMessages.TryDequeue(out result))
        this.HandleMessage(result);
    }

    private void HandleMessage(IServerMessageBase msg)
    {
      try
      {
        this.MessageHandler.HandleMessage(msg);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(string.Format("Error handling message type {0}. Details: {1}", (object) msg.Data.GetType(), (object) ex));
        NetworkConnection.Disconnect(string.Format("Error handling message type {0}. Details: {1}", (object) msg.Data.GetType(), (object) ex));
      }
      finally
      {
        msg.Recycle();
      }
    }
  }
}
