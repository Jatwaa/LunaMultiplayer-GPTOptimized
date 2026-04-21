using LmpCommon.Locks;
using LmpCommon.Message.Data.Lock;
using LmpCommon.Message.Server;
using Server.Client;
using Server.Context;
using Server.Log;
using Server.Server;
using System.Linq;

namespace Server.System
{
    public class LockSystemSender
    {
        public static void SendAllLocks(ClientStructure client)
        {
            var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<LockListReplyMsgData>();
            msgData.Locks = LockSystem.LockQuery.GetAllLocks().ToArray();
            msgData.LocksCount = msgData.Locks.Length;

            MessageQueuer.SendToClient<LockSrvMsg>(client, msgData);
        }

        public static void ReleaseAndSendLockReleaseMessage(ClientStructure client, LockDefinition lockDefinition)
        {
            var lockReleaseResult = LockSystem.ReleaseLock(lockDefinition);
            if (lockReleaseResult)
            {
                var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<LockReleaseMsgData>();
                msgData.Lock = lockDefinition;
                msgData.LockResult = true;

                MessageQueuer.RelayMessage<LockSrvMsg>(client, msgData);
                LunaLog.Debug($"{lockDefinition.PlayerName} released lock {lockDefinition}");
                LockTraceLog.Release(lockDefinition.PlayerName, lockDefinition);
            }
            else
            {
                // Capture who actually holds the lock before we send fallback data
                var currentHolder = CurrentHolder(lockDefinition);
                SendStoredLockData(client, lockDefinition);
                LunaLog.Debug($"{lockDefinition.PlayerName} failed to release lock {lockDefinition}");
                LockTraceLog.FailRelease(lockDefinition.PlayerName, lockDefinition, currentHolder);
            }
        }

        public static void SendLockAcquireMessage(ClientStructure client, LockDefinition lockDefinition, bool force)
        {
            // Snapshot who holds this lock BEFORE the acquire so we can detect steals
            var priorHolder = CurrentHolder(lockDefinition);

            if (LockSystem.AcquireLock(lockDefinition, force, out var repeatedAcquire))
            {
                var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<LockAcquireMsgData>();
                msgData.Lock = lockDefinition;
                msgData.Force = force;

                MessageQueuer.SendToAllClients<LockSrvMsg>(msgData);

                // Just log if we actually changed the value. Clients re-send acquires until confirmed.
                if (!repeatedAcquire)
                {
                    bool wasStolen = priorHolder != null && priorHolder != lockDefinition.PlayerName;
                    LunaLog.Debug($"{lockDefinition.PlayerName} acquired lock {lockDefinition}");
                    LockTraceLog.Acquire(lockDefinition.PlayerName, lockDefinition, wasStolen);
                }
            }
            else
            {
                SendStoredLockData(client, lockDefinition);
                LunaLog.Debug($"{lockDefinition.PlayerName} failed to acquire lock {lockDefinition}");
                LockTraceLog.FailAcquire(lockDefinition.PlayerName, lockDefinition, priorHolder ?? "none");
            }
        }

        /// <summary>
        /// Whenever a release/acquire lock fails, call this method to relay the correct lock definition to the player
        /// </summary>
        private static void SendStoredLockData(ClientStructure client, LockDefinition lockDefinition)
        {
            var storedLockDef = LockSystem.LockQuery.GetLock(lockDefinition.Type, lockDefinition.PlayerName, lockDefinition.VesselId, lockDefinition.KerbalName);
            if (storedLockDef != null)
            {
                var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<LockAcquireMsgData>();
                msgData.Lock = storedLockDef;
                MessageQueuer.SendToClient<LockSrvMsg>(client, msgData);
            }
        }

        /// <summary>Returns the player name currently holding this lock, or null if nobody holds it.</summary>
        private static string CurrentHolder(LockDefinition lockDef)
        {
            return lockDef.Type switch
            {
                LockType.Control       => LockSystem.LockQuery.GetControlLock(lockDef.VesselId)?.PlayerName,
                LockType.Update        => LockSystem.LockQuery.GetUpdateLock(lockDef.VesselId)?.PlayerName,
                LockType.UnloadedUpdate=> LockSystem.LockQuery.GetUnloadedUpdateLock(lockDef.VesselId)?.PlayerName,
                LockType.Kerbal        => LockSystem.LockQuery.GetLock(LockType.Kerbal, null, default, lockDef.KerbalName)?.PlayerName,
                _                      => null
            };
        }
    }
}
