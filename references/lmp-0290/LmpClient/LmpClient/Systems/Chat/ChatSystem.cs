// Decompiled with JetBrains decompiler
// Type: LmpClient.Systems.Chat.ChatSystem
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using LmpClient.Base;
using LmpClient.Systems.SettingsSys;
using LmpClient.Windows.Chat;
using System;
using System.Collections.Concurrent;

namespace LmpClient.Systems.Chat
{
  public class ChatSystem : MessageSystem<ChatSystem, ChatMessageSender, ChatMessageHandler>
  {
    public bool NewMessageReceived { get; set; }

    public bool SendEventHandled { get; set; } = true;

    public LimitedQueue<Tuple<string, string, string>> ChatMessages { get; private set; }

    public ConcurrentQueue<Tuple<string, string, string>> NewChatMessages { get; private set; } = new ConcurrentQueue<Tuple<string, string, string>>();

    public override string SystemName { get; } = nameof (ChatSystem);

    protected override bool ProcessMessagesInUnityThread => false;

    protected override void OnEnabled()
    {
      base.OnEnabled();
      this.ChatMessages = new LimitedQueue<Tuple<string, string, string>>(SettingsSystem.CurrentSettings.ChatBuffer);
      this.SetupRoutine(new RoutineDefinition(100, RoutineExecution.Update, new Action(this.ProcessReceivedMessages)));
    }

    protected override void OnDisabled()
    {
      base.OnDisabled();
      this.NewMessageReceived = false;
      this.SendEventHandled = true;
      this.ChatMessages.Clear();
      this.NewChatMessages = new ConcurrentQueue<Tuple<string, string, string>>();
    }

    private void ProcessReceivedMessages()
    {
      if (!this.Enabled)
        return;
      Tuple<string, string, string> result;
      while (this.NewChatMessages.TryDequeue(out result))
      {
        this.NewMessageReceived = true;
        if (!Window<ChatWindow>.Singleton.Display)
          LunaScreenMsg.PostScreenMessage(result.Item1 + ": " + result.Item2, 5f, (ScreenMessageStyle) 1);
        else
          Window<ChatWindow>.Singleton.ScrollToBottom();
        this.ChatMessages.Enqueue(result);
      }
    }

    public void PrintToChat(string text) => this.NewChatMessages.Enqueue(new Tuple<string, string, string>(SettingsSystem.ServerSettings.ConsoleIdentifier, text, SettingsSystem.ServerSettings.ConsoleIdentifier + ": " + text));

    public void PmMessageServer(string message) => this.MessageSender.SendChatMsg(message, false);
  }
}
