using LmpCommon.Time;
using Server.Command.Command;
using Server.Context;
using Server.Log;
using Server.Plugin;
using Server.Server;
using Server.Settings.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Client
{
    public class ClientMainThread
    {
        public static async Task ThreadMainAsync()
        {
            try
            {
                const int BacklogWarnThreshold = 50; // warn when any client's queue exceeds this many messages

                while (ServerContext.ServerRunning)
                {
                    //Check timers
                    NukeCommand.CheckTimer();
                    DekesslerCommand.CheckTimer();

                    LmpPluginHandler.FireOnUpdate(); //Run plugin update

                    // Detect send-queue backlog so server ops can spot congestion early
                    foreach (var client in ServerContext.Clients.Values)
                    {
                        var depth = client.SendMessageQueue.Count;
                        if (depth > BacklogWarnThreshold)
                            LunaLog.Warning($"Send queue backlog for '{client.PlayerName}': {depth} messages pending");
                    }

                    await Task.Delay(IntervalSettings.SettingsStore.MainTimeTick);
                }
            }
            catch (Exception e)
            {
                LunaLog.Error($"Fatal error thrown, exception: {e}");
                ServerContext.Shutdown("Fatal error server side");
            }

            try
            {
                var disconnectTime = LunaNetworkTime.UtcNow.Ticks;
                var sendingMessages = true;
                while (sendingMessages)
                {
                    if (LunaNetworkTime.UtcNow.Ticks - disconnectTime > TimeSpan.FromSeconds(5).Ticks)
                    {
                        LunaLog.Debug($"Shutting down with {ServerContext.PlayerCount} Players, " +
                                      $"{ServerContext.Clients.Count} connected Clients");
                        break;
                    }
                    sendingMessages = ClientRetriever.GetAuthenticatedClients().Any(c => c.SendMessageQueue.Count > 0);

                    await Task.Delay(IntervalSettings.SettingsStore.MainTimeTick);
                }
                LidgrenServer.ShutdownLidgrenServer();
            }
            catch (Exception e)
            {
                LunaLog.Fatal($"Fatal error thrown during shutdown, exception: {e}");
                throw;
            }
        }
    }
}
