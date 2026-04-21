using System;
using System.Collections.Generic;
using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Server;
using Server.Context;
using Server.Log;
using Server.Server;

namespace Server.Web.Structures
{
    public class ChatData
    {
        public List<ChatMessageEntry> Messages { get; } = new List<ChatMessageEntry>();
        public const int MaxMessages = 100;

        private static readonly object _lock = new object();
        private static readonly Queue<ChatMessageEntry> _messageHistory = new Queue<ChatMessageEntry>();

        public void AddMessage(string from, string text)
        {
            lock (_lock)
            {
                var msg = new ChatMessageEntry { From = from, Text = text, Timestamp = DateTime.UtcNow };
                _messageHistory.Enqueue(msg);

                while (_messageHistory.Count > MaxMessages)
                    _messageHistory.Dequeue();
            }
        }

        public void Refresh()
        {
            Messages.Clear();
            lock (_lock)
            {
                Messages.AddRange(_messageHistory);
            }
        }

        public static void Broadcast(string from, string text)
        {
            var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<ChatMsgData>();
            msgData.From = from;
            msgData.Text = text;
            msgData.Relay = true;

            LunaLog.ChatMessage($"{from}: {text}");
            MessageQueuer.SendToAllClients<ChatSrvMsg>(msgData);
        }
    }

    public class ChatMessageEntry
    {
        public string From { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}