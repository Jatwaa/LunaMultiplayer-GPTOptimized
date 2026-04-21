using LunaConfigNode;
using Server.Context;
using Server.Log;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.System
{
    /// <summary>
    /// Here we keep a copy of all the player vessels in <see cref="Vessel"/> format and we also save them to files at a specified rate
    /// </summary>
    public static class VesselStoreSystem
    {
        public const string VesselFileFormat = ".txt";
        public const string OwnerFileFormat  = ".owner";
        public static string VesselsPath = Path.Combine(ServerContext.UniverseDirectory, "Vessels");

        public static ConcurrentDictionary<Guid, Vessel.Classes.Vessel> CurrentVessels
            = new ConcurrentDictionary<Guid, Vessel.Classes.Vessel>();

        /// <summary>
        /// Maps vessel GUID → the player name that first sent a proto for it.
        /// Used to validate remove/revert requests so only the owner can delete their craft.
        /// Persisted to {vesselId}.owner files alongside the vessel .txt files.
        /// </summary>
        public static readonly ConcurrentDictionary<Guid, string> VesselOwners
            = new ConcurrentDictionary<Guid, string>();

        private static readonly object BackupLock = new object();

        public static bool VesselExists(Guid vesselId) => CurrentVessels.ContainsKey(vesselId);

        // ── Ownership API ────────────────────────────────────────────────────

        /// <summary>
        /// Records the first player to send a proto for <paramref name="vesselId"/> as its owner.
        /// Subsequent calls for the same vessel are ignored — first sender wins.
        /// </summary>
        public static void SetOwnerIfAbsent(Guid vesselId, string playerName)
        {
            if (VesselOwners.TryAdd(vesselId, playerName))
            {
                // Persist so ownership survives server restarts
                _ = Task.Run(() =>
                {
                    lock (BackupLock)
                    {
                        FileHandler.WriteToFile(
                            Path.Combine(VesselsPath, $"{vesselId}{OwnerFileFormat}"),
                            playerName);
                    }
                });
                LunaLog.Debug($"Vessel {vesselId} owner set to '{playerName}'.");
            }
        }

        /// <summary>Returns the recorded owner of a vessel, or <c>null</c> if unknown.</summary>
        public static string GetOwner(Guid vesselId)
            => VesselOwners.TryGetValue(vesselId, out var owner) ? owner : null;

        // ── Remove ───────────────────────────────────────────────────────────

        /// <summary>
        /// Removes a vessel from the store and deletes its files from disk.
        /// </summary>
        public static void RemoveVessel(Guid vesselId)
        {
            CurrentVessels.TryRemove(vesselId, out _);
            VesselOwners.TryRemove(vesselId, out _);

            _ = Task.Run(() =>
            {
                lock (BackupLock)
                {
                    FileHandler.FileDelete(Path.Combine(VesselsPath, $"{vesselId}{VesselFileFormat}"));

                    var ownerFile = Path.Combine(VesselsPath, $"{vesselId}{OwnerFileFormat}");
                    if (File.Exists(ownerFile))
                        FileHandler.FileDelete(ownerFile);
                }
            });
        }

        /// <summary>
        /// Returns a vessel in the standard KSP format
        /// </summary>
        public static string GetVesselInConfigNodeFormat(Guid vesselId)
        {
            return CurrentVessels.TryGetValue(vesselId, out var vessel) ?
                vessel.ToString() : null;
        }

        /// <summary>
        /// Load the stored vessels (and their ownership records) into the dictionaries.
        /// </summary>
        public static void LoadExistingVessels()
        {
            ChangeExistingVesselFormats();
            lock (BackupLock)
            {
                // Load vessel data
                foreach (var file in Directory.GetFiles(VesselsPath).Where(f => Path.GetExtension(f) == VesselFileFormat))
                {
                    if (!Guid.TryParse(Path.GetFileNameWithoutExtension(file), out var vesselId))
                        continue;

                    var vesselText = FileHandler.ReadFileText(file);

                    // Skip vessels with unresolved localization-key names or missing name fields.
                    // These were saved in an incomplete state and cause FlightIntegrator /
                    // Part.Update crashes on every client that loads them.
                    if (!VesselNameIsValid(vesselText, vesselId, file))
                    {
                        LunaLog.Warning($"Skipping vessel file {Path.GetFileName(file)} — invalid or unresolved vessel name. Delete the file to suppress this warning.");
                        continue;
                    }

                    try
                    {
                        CurrentVessels.TryAdd(vesselId, new Vessel.Classes.Vessel(vesselText));
                    }
                    catch (Exception ex)
                    {
                        LunaLog.Error($"Failed to parse vessel file {Path.GetFileName(file)}: {ex.Message} — skipping.");
                    }
                }

                // Load ownership records (.owner files written by SetOwnerIfAbsent)
                foreach (var file in Directory.GetFiles(VesselsPath).Where(f => Path.GetExtension(f) == OwnerFileFormat))
                {
                    if (!Guid.TryParse(Path.GetFileNameWithoutExtension(file), out var vesselId))
                        continue;

                    var ownerName = FileHandler.ReadFileText(file)?.Trim();
                    if (!string.IsNullOrEmpty(ownerName))
                        VesselOwners.TryAdd(vesselId, ownerName);
                }

                LunaLog.Normal($"Loaded {CurrentVessels.Count} vessels, {VesselOwners.Count} ownership records.");
            }
        }

        /// <summary>
        /// Returns false when the vessel text has a missing, empty, or unresolved
        /// localization-key name (e.g. #autoLOC_8005483).
        /// </summary>
        private static bool VesselNameIsValid(string vesselText, Guid vesselId, string filePath)
        {
            if (string.IsNullOrWhiteSpace(vesselText)) return false;

            const string namePrefix = "name = ";
            var nameStart = vesselText.IndexOf(namePrefix, StringComparison.Ordinal);
            if (nameStart < 0) return false;

            nameStart += namePrefix.Length;
            var nameEnd = vesselText.IndexOf('\n', nameStart);
            var vesselName = (nameEnd >= 0
                ? vesselText.Substring(nameStart, nameEnd - nameStart)
                : vesselText.Substring(nameStart)).Trim();

            if (string.IsNullOrEmpty(vesselName)) return false;
            if (vesselName.StartsWith("#autoLOC_", StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }

        /// <summary>
        /// Transform OLD Xml vessels into the new format
        /// TODO: Remove this for next version
        /// </summary>
        public static void ChangeExistingVesselFormats()
        {
            lock (BackupLock)
            {
                foreach (var file in Directory.GetFiles(VesselsPath).Where(f => Path.GetExtension(f) == ".xml"))
                {
                    if (Guid.TryParse(Path.GetFileNameWithoutExtension(file), out var vesselId))
                    {
                        var vesselAsCfgNode = XmlConverter.ConvertToConfigNode(FileHandler.ReadFileText(file));
                        FileHandler.WriteToFile(file.Replace(".xml", ".txt"), vesselAsCfgNode);
                    }
                    FileHandler.FileDelete(file);
                }
            }
        }

        /// <summary>
        /// Actually performs the backup of the vessels to file
        /// </summary>
        public static void BackupVessels()
        {
            lock (BackupLock)
            {
                var vesselsInCfgNode = CurrentVessels.ToArray();
                foreach (var vessel in vesselsInCfgNode)
                {
                    FileHandler.WriteToFile(Path.Combine(VesselsPath, $"{vessel.Key}{VesselFileFormat}"), vessel.Value.ToString());
                }
            }
        }

        /// <summary>
        /// Writes one vessel to disk so live patches (orbit, IDENT, position fields) are reflected in the Vessels folder without waiting for <see cref="BackupVessels"/>.
        /// </summary>
        public static void PersistVesselToFile(Guid vesselId)
        {
            if (!CurrentVessels.TryGetValue(vesselId, out var vessel)) return;

            lock (BackupLock)
            {
                FileHandler.WriteToFile(Path.Combine(VesselsPath, $"{vesselId}{VesselFileFormat}"), vessel.ToString());
            }
        }
    }
}
