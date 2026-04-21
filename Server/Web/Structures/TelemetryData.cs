using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Server.Context;
using Server.Settings.Structures;
using Server.System;

namespace Server.Web.Structures
{
    public class TelemetryData
    {
        public List<PlayerTelemetry> Players { get; } = new List<PlayerTelemetry>();
        public int TotalPlayers => ServerContext.PlayerCount;
        public long LastUpdate => _lastUpdate;

        private static readonly object _lock = new object();
        private static readonly ConcurrentDictionary<Guid, PlayerTelemetry> _players = new();
        private static long _lastUpdate;

        private static readonly HashSet<string> _bodies = new()
        {
            "Kerbin", "Mun", "Minmus", "Eve", "Duna", "Ike", "Jool", "Laythe", "Vall", "Tylo", "Bop", "Pol", "Dres", "Moho", "Sun"
        };

        public static void UpdatePlayer(Guid vesselId, string playerName, string vesselName, string vesselType,
            double lat, double lon, double alt, string body, double speed, double heading, double altitudeASL)
        {
            var telemetry = new PlayerTelemetry
            {
                VesselId = vesselId,
                PlayerName = playerName,
                VesselName = vesselName,
                VesselType = vesselType,
                Lat = lat,
                Lon = lon,
                Alt = alt,
                Body = body,
                Speed = speed,
                Heading = heading,
                AltitudeASL = altitudeASL,
                LastUpdate = DateTime.UtcNow.Ticks
            };

            _players[vesselId] = telemetry;
            _lastUpdate = DateTime.UtcNow.Ticks;
        }

        public static void RemovePlayer(Guid vesselId)
        {
            _players.TryRemove(vesselId, out _);
            _lastUpdate = DateTime.UtcNow.Ticks;
        }

        public static void RemoveInactive(TimeSpan maxAge)
        {
            var cutoff = DateTime.UtcNow.Ticks - maxAge.Ticks;
            var toRemove = _players.Where(p => p.Value.LastUpdate < cutoff).Select(p => p.Key).ToList();
            foreach (var id in toRemove)
            {
                _players.TryRemove(id, out _);
            }
        }

        public void Refresh()
        {
            // Evict entries from players that disconnected without an explicit RemovePlayer call.
            // Use 10× the primary update interval as a generous threshold.
            RemoveInactive(TimeSpan.FromMilliseconds(
                IntervalSettings.SettingsStore.VesselUpdatesMsInterval * 10));

            Players.Clear();

            foreach (var client in ServerContext.Clients.Values)
            {
                if (client.Authenticated && client.OwnedVesselId != Guid.Empty)
                {
                    if (_players.TryGetValue(client.OwnedVesselId, out var telemetry))
                    {
                        Players.Add(telemetry);
                    }
                    else
                    {
                        VesselStoreSystem.CurrentVessels.TryGetValue(client.OwnedVesselId, out var vessel);
                        if (vessel != null)
                        {
                            var lat = 0.0;
                            var lon = 0.0;
                            var alt = 0.0;

                            if (double.TryParse(vessel.Fields.GetSingle("lat")?.Value, out var latVal))
                                lat = latVal;
                            if (double.TryParse(vessel.Fields.GetSingle("lon")?.Value, out var lonVal))
                                lon = lonVal;
                            if (double.TryParse(vessel.Fields.GetSingle("alt")?.Value, out var altVal))
                                alt = altVal;

                            var body = vessel.GetOrbitingBodyName() ?? "Unknown";

                            var playerData = new PlayerTelemetry
                            {
                                VesselId = client.OwnedVesselId,
                                PlayerName = client.PlayerName,
                                VesselName = vessel.Fields.GetSingle("name")?.Value ?? "Unknown",
                                VesselType = vessel.Fields.GetSingle("type")?.Value ?? "Unknown",
                                Lat = lat,
                                Lon = lon,
                                Alt = alt,
                                Body = body,
                                LastUpdate = 0
                            };
                            Players.Add(playerData);
                        }
                    }
                }
            }

            _lastUpdate = DateTime.UtcNow.Ticks;
        }
    }

    public class PlayerTelemetry
    {
        public Guid VesselId { get; set; }
        public string PlayerName { get; set; } = "";
        public string VesselName { get; set; } = "";
        public string VesselType { get; set; } = "";
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Alt { get; set; }
        public string Body { get; set; } = "";
        public double Speed { get; set; }
        public double Heading { get; set; }
        public double AltitudeASL { get; set; }
        public long LastUpdate { get; set; }
    }
}