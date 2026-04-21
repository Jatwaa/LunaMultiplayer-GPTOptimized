using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lidgren.Network;
using LmpCommon.Message.Interface;
using LmpCommon.Time;
using Server.Client;
using Server.Context;
using Server.Log;
using Server.Server;

namespace Server.System
{
    public class MessageBatcher
    {
        private static readonly MessageBatcher Instance = new MessageBatcher();
        public static MessageBatcher InstanceProperty => Instance;

        private const int MaxBatchSize = 1400;
        private const int MaxBatchMessages = 50;
        private const int BatchTimeoutMs = 10;

        private readonly ConcurrentDictionary<NetConnection, Batch> _clientBatches =
            new ConcurrentDictionary<NetConnection, Batch>();

        public void Enqueue(ClientStructure client, IServerMessageBase message)
        {
            var batch = _clientBatches.GetOrAdd(client.Connection, _ => new Batch());

            lock (batch)
            {
                batch.Messages.Add(message);
                batch.SizeBytes += message.GetMessageSize();
                batch.LastEnqueueTime = ServerContext.ServerClock.ElapsedMilliseconds;

                if (batch.Messages.Count >= MaxBatchMessages || batch.SizeBytes >= MaxBatchSize ||
                    ServerContext.ServerClock.ElapsedMilliseconds - batch.LastEnqueueTime > BatchTimeoutMs)
                {
                    FlushBatchLocked(batch, client);
                }
            }
        }

        public void FlushClient(ClientStructure client)
        {
            if (_clientBatches.TryRemove(client.Connection, out var batch))
            {
                lock (batch)
                {
                    if (batch.Messages.Count > 0)
                    {
                        FlushBatchLocked(batch, client);
                    }
                }
            }
        }

        public void FlushAll()
        {
            foreach (var kvp in _clientBatches)
            {
                var batch = kvp.Value;
                lock (batch)
                {
                    if (batch.Messages.Count > 0 && ServerContext.Clients.TryGetValue(kvp.Key.RemoteEndPoint, out var client))
                    {
                        FlushBatchLocked(batch, client);
                    }
                }
            }
        }

        private void FlushBatchLocked(Batch batch, ClientStructure client)
        {
            if (batch.Messages.Count == 0) return;

            try
            {
                var outmsg = LidgrenServer.Server.CreateMessage();
                var first = true;
                var sentCount = 0;

                for (int i = batch.Messages.Count - 1; i >= 0 && sentCount < MaxBatchMessages; i--)
                {
                    var msg = batch.Messages[i];
                    if (msg?.Data == null) continue;

                    var msgSize = msg.GetMessageSize();
                    if (!first && outmsg.LengthBytes + msgSize > MaxBatchSize) break;

                    msg.Data.SentTime = LunaNetworkTime.UtcNow.Ticks;
                    msg.Serialize(outmsg);
                    first = false;
                    sentCount++;
                }

                if (outmsg.LengthBytes > 0)
                {
                    client.LastSendTime = ServerContext.ServerClock.ElapsedMilliseconds;
                    client.BytesSent += outmsg.LengthBytes;

                    LidgrenServer.Server.SendMessage(outmsg, client.Connection, NetDeliveryMethod.Unreliable, 1);
                }

                batch.Messages.Clear();
                batch.SizeBytes = 0;
                batch.LastEnqueueTime = ServerContext.ServerClock.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                LunaLog.Error($"Batch send error: {ex.Message}");
            }
        }

        public void ClearClientBatch(NetConnection connection)
        {
            _clientBatches.TryRemove(connection, out _);
        }

        private class Batch
        {
            public List<IServerMessageBase> Messages { get; } = new List<IServerMessageBase>();
            public int SizeBytes;
            public long LastEnqueueTime;
        }
    }
}