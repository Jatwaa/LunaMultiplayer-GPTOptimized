using LmpCommon.Time;
using Server.Context;
using Server.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Web.Structures
{
    public class MapData
    {
        public DateTime StartTime { get; set; }
        public List<PlayerCraft> Players { get; } = new List<PlayerCraft>();
        public long ServerTime { get; set; }

        public void Refresh()
        {
            Players.Clear();
            ServerTime = LunaNetworkTime.UtcNow.Ticks;

            foreach (var client in ServerContext.Clients.Values)
            {
                if (client.Authenticated && client.OwnedVesselId != Guid.Empty)
                {
                    var craft = new PlayerCraft
                    {
                        PlayerName = client.PlayerName,
                        VesselId = client.OwnedVesselId,
                        Lat = 0,
                        Lon = 0,
                        Alt = 0,
                        Body = "Unknown"
                    };

                    if (VesselStoreSystem.CurrentVessels.TryGetValue(client.OwnedVesselId, out var vessel))
                    {
                        var latStr = vessel.Fields.GetSingle("lat")?.Value;
                        var lonStr = vessel.Fields.GetSingle("lon")?.Value;
                        var altStr = vessel.Fields.GetSingle("alt")?.Value;

                        if (double.TryParse(latStr, out var lat)) craft.Lat = lat;
                        if (double.TryParse(lonStr, out var lon)) craft.Lon = lon;
                        if (double.TryParse(altStr, out var alt)) craft.Alt = alt;

                        craft.Body = vessel.GetOrbitingBodyName();
                        craft.VesselName = vessel.Fields.GetSingle("name")?.Value ?? "Unknown";
                        craft.VesselType = vessel.Fields.GetSingle("type")?.Value ?? "Unknown";
                    }

                    Players.Add(craft);
                }
            }
        }
    }

    public class PlayerCraft
    {
        public string PlayerName { get; set; }
        public Guid VesselId { get; set; }
        public string VesselName { get; set; }
        public string VesselType { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Alt { get; set; }
        public string Body { get; set; }
    }
}