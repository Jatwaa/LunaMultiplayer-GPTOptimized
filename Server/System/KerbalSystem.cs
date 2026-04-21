using LmpCommon.Message.Data.Kerbal;
using LmpCommon.Message.Server;
using Server.Client;
using Server.Context;
using Server.Log;
using Server.Properties;
using Server.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.System
{
    public class KerbalSystem
    {
        public static readonly string KerbalsPath = Path.Combine(ServerContext.UniverseDirectory, "Kerbals");

        /// <summary>
        /// Maps the four stock kerbals to their embedded resource content so that a corrupt
        /// or empty save file can be restored to the correct default (preserving the original trait).
        /// </summary>
        private static readonly Dictionary<string, Func<string>> DefaultKerbalResources =
            new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Jebediah Kerman"] = () => Resources.Jebediah_Kerman,
                ["Bill Kerman"]     = () => Resources.Bill_Kerman,
                ["Bob Kerman"]      = () => Resources.Bob_Kerman,
                ["Valentina Kerman"] = () => Resources.Valentina_Kerman,
            };

        public static void GenerateDefaultKerbals()
        {
            FileHandler.CreateFile(Path.Combine(KerbalsPath, "Jebediah Kerman.txt"), Resources.Jebediah_Kerman);
            FileHandler.CreateFile(Path.Combine(KerbalsPath, "Bill Kerman.txt"), Resources.Bill_Kerman);
            FileHandler.CreateFile(Path.Combine(KerbalsPath, "Bob Kerman.txt"), Resources.Bob_Kerman);
            FileHandler.CreateFile(Path.Combine(KerbalsPath, "Valentina Kerman.txt"), Resources.Valentina_Kerman);
        }

        public static void HandleKerbalProto(ClientStructure client, KerbalProtoMsgData data)
        {
            LunaLog.Debug($"Saving kerbal {data.Kerbal.KerbalName} from {client.PlayerName}");

            var path = Path.Combine(KerbalsPath, $"{data.Kerbal.KerbalName}.txt");
            FileHandler.WriteToFile(path, data.Kerbal.KerbalData, data.Kerbal.NumBytes);

            MessageQueuer.RelayMessage<KerbalSrvMsg>(client, data);
        }

        public static void HandleKerbalsRequest(ClientStructure client)
        {
            var kerbalFiles = FileHandler.GetFilesInPath(KerbalsPath);
            var kerbalsData = kerbalFiles.Select(k =>
            {
                var kerbalName = Path.GetFileNameWithoutExtension(k);
                var kerbalData = FileHandler.ReadFile(k);

                if (!KerbalFileIsValid(kerbalData))
                    kerbalData = RepairAndOverwrite(k, kerbalName);

                return new KerbalInfo
                {
                    KerbalData = kerbalData,
                    NumBytes = kerbalData.Length,
                    KerbalName = kerbalName
                };
            });
            LunaLog.Debug($"Sending {client.PlayerName} {kerbalFiles.Length} kerbals...");

            var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<KerbalReplyMsgData>();
            msgData.Kerbals = kerbalsData.ToArray();
            msgData.KerbalsCount = msgData.Kerbals.Length;

            MessageQueuer.SendToClient<KerbalSrvMsg>(client, msgData);
        }

        public static void HandleKerbalRemove(ClientStructure client, KerbalRemoveMsgData message)
        {
            var kerbalToRemove = message.KerbalName;

            LunaLog.Debug($"Removing kerbal {kerbalToRemove} from {client.PlayerName}");
            FileHandler.FileDelete(Path.Combine(KerbalsPath, $"{kerbalToRemove}.txt"));

            MessageQueuer.RelayMessage<KerbalSrvMsg>(client, message);
        }

        // -------------------------------------------------------------------------
        // Kerbal file validation and repair
        // -------------------------------------------------------------------------

        /// <summary>
        /// Returns true when <paramref name="data"/> decodes to valid UTF-8 text that
        /// contains the minimum fields KSP requires to construct a ProtoCrewMember.
        /// Binary-corrupted files, whitespace-only files, and nodes missing "name =" or
        /// "trait =" all fail this check.
        /// </summary>
        private static bool KerbalFileIsValid(byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            string text;
            try
            {
                text = Encoding.UTF8.GetString(data);
            }
            catch
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Both fields must be present and non-empty to avoid NullReferenceException
            // inside KerbalRoster.SetExperienceTrait during ProtoCrewMember construction.
            return text.Contains("name =") && text.Contains("trait =");
        }

        /// <summary>
        /// Rebuilds the kerbal file on disk and returns the repaired bytes ready to send.
        /// Stock kerbals (Jebediah, Bill, Bob, Valentina) are restored from their embedded
        /// default templates so the correct trait is preserved.  Any other kerbal is
        /// regenerated as a generic Available Pilot based on the Jebediah template format.
        /// </summary>
        private static byte[] RepairAndOverwrite(string path, string kerbalName)
        {
            string repairedContent;

            if (DefaultKerbalResources.TryGetValue(kerbalName, out var getResource))
            {
                repairedContent = getResource();
                LunaLog.Warning($"[Kerbal]: '{kerbalName}' save file was corrupt/empty — restored from embedded default (trait preserved)");
            }
            else
            {
                repairedContent = BuildGenericPilotTemplate(kerbalName);
                LunaLog.Warning($"[Kerbal]: '{kerbalName}' save file was corrupt/empty — regenerated as generic Available Pilot");
            }

            FileHandler.WriteToFile(path, repairedContent);
            return Encoding.UTF8.GetBytes(repairedContent);
        }

        /// <summary>
        /// Builds a minimal valid kerbal ConfigNode string for a non-default kerbal.
        /// Matches the field order and format used by the stock embedded templates.
        /// </summary>
        private static string BuildGenericPilotTemplate(string kerbalName)
        {
            return
                $"name = {kerbalName}\r\n" +
                "gender = Male\r\n" +
                "type = Crew\r\n" +
                "trait = Pilot\r\n" +
                "brave = 0.5\r\n" +
                "dumb = 0.5\r\n" +
                "badS = False\r\n" +
                "veteran = False\r\n" +
                "tour = False\r\n" +
                "state = Available\r\n" +
                "inactive = False\r\n" +
                "inactiveTimeEnd = 0\r\n" +
                "gExperienced = 0\r\n" +
                "outDueToG = False\r\n" +
                "ToD = 0\r\n" +
                "idx = -1\r\n" +
                "extraXP = 0\r\n" +
                "CAREER_LOG\r\n" +
                "{\r\n" +
                "\tflight = 0\r\n" +
                "}\r\n" +
                "FLIGHT_LOG\r\n" +
                "{\r\n" +
                "\tflight = 0\r\n" +
                "}\r\n";
        }
    }
}
