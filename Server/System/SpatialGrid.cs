using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Numerics;
using Server.Client;
using Server.Context;

namespace Server.System
{
    public class SpatialGrid
    {
        // Cell size: 10 km (matches previous interest radius)
        private const float CellSize = 10000f;
        // Sub-cell size: 1 km (10x10x10 subcells per cell)
        private const float SubCellSize = 1000f;
        private const int SubCellsPerCell = 10; // CellSize / SubCellSize

        private static readonly SpatialGrid Instance = new SpatialGrid();
        public static SpatialGrid InstanceProperty => Instance;

        // Maps vessel ID -> its current cell and sub-cell coordinates
        private readonly ConcurrentDictionary<Guid, (int cellX, int cellY, int cellZ, int subX, int subY, int subZ)> _vesselLocation =
            new ConcurrentDictionary<Guid, (int, int, int, int, int, int)>();

        // Maps (cellX,cellY,cellZ,subX,subY,subZ) -> set of vessel IDs in that sub-cell
        private readonly ConcurrentDictionary<(int,int,int,int,int,int), HashSet<Guid>> _subcellVessels =
            new ConcurrentDictionary<(int,int,int,int,int,int), HashSet<Guid>>();

        // Maps (cellX,cellY,cellZ) -> set of vessel IDs in that cell (for low-detail interest if needed)
        private readonly ConcurrentDictionary<(int,int,int), HashSet<Guid>> _cellVessels =
            new ConcurrentDictionary<(int,int,int), HashSet<Guid>>();

        public void UpdateVesselPosition(Guid vesselId, Vector3 position)
        {
            var (newCellX, newCellY, newCellZ, newSubX, newSubY, newSubZ) = GetCellAndSubCell(position);

            // Try to get old location
            if (_vesselLocation.TryRemove(vesselId, out var oldLoc))
            {
                var (oldCellX, oldCellY, oldCellZ, oldSubX, oldSubY, oldSubZ) = oldLoc;

                // Remove from old sub-cell
                RemoveVesselFromSubCell(vesselId, oldCellX, oldCellY, oldCellZ, oldSubX, oldSubY, oldSubZ);

                // If after removal the old sub-cell becomes empty, we could clean up but not required.
                // Update cell-level set
                RemoveVesselFromCell(vesselId, oldCellX, oldCellY, oldCellZ);
            }

            // Add to new sub-cell
            AddVesselToSubCell(vesselId, newCellX, newCellY, newCellZ, newSubX, newSubY, newSubZ);
            AddVesselToCell(vesselId, newCellX, newCellY, newCellZ);

            _vesselLocation[vesselId] = (newCellX, newCellY, newCellZ, newSubX, newSubY, newSubZ);
        }

        /// <summary>
        /// Returns clients whose owned vessel is within the same or adjacent sub-cell (3x3x3) of the given position.
        /// This provides full-detail interest.
        /// </summary>
        public IEnumerable<ClientStructure> GetInterestedClientsFullDetail(Vector3 position)
        {
            var (cellX, cellY, cellZ, subX, subY, subZ) = GetCellAndSubCell(position);
            var interestedVessels = new HashSet<Guid>();

            // Check current sub-cell and neighboring sub-cells (offset -1..1)
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        int sx = subX + dx;
                        int sy = subY + dy;
                        int sz = subZ + dz;
                        // Wrap within cell: if sub index goes out of [0, SubCellsPerCell-1], adjust cell coordinates
                        int cx = cellX;
                        int cy = cellY;
                        int cz = cellZ;
                        if (sx < 0) { sx += SubCellsPerCell; cx--; }
                        else if (sx >= SubCellsPerCell) { sx -= SubCellsPerCell; cx++; }
                        if (sy < 0) { sy += SubCellsPerCell; cy--; }
                        else if (sy >= SubCellsPerCell) { sy -= SubCellsPerCell; cy++; }
                        if (sz < 0) { sz += SubCellsPerCell; cz--; }
                        else if (sz >= SubCellsPerCell) { sz -= SubCellsPerCell; cz++; }

                        var key = (cx, cy, cz, sx, sy, sz);
                        if (_subcellVessels.TryGetValue(key, out var vessels))
                        {
                            foreach (var v in vessels)
                                interestedVessels.Add(v);
                        }
                    }
                }
            }

            // Yield clients whose owned vessel is in the interested set
            foreach (var client in ServerContext.Clients.Values)
            {
                if (client.OwnedVesselId != Guid.Empty && interestedVessels.Contains(client.OwnedVesselId))
                {
                    yield return client;
                }
            }
        }

        /// <summary>
        /// Returns clients whose owned vessel is within the same cell (10km³) of the given position.
        /// This can be used for low-detail interest if needed.
        /// </summary>
        public IEnumerable<ClientStructure> GetInterestedClientsCellDetail(Vector3 position)
        {
            var (cellX, cellY, cellZ, _, _, _) = GetCellAndSubCell(position);
            if (_cellVessels.TryGetValue((cellX, cellY, cellZ), out var vessels))
            {
                foreach (var client in ServerContext.Clients.Values)
                {
                    if (client.OwnedVesselId != Guid.Empty && vessels.Contains(client.OwnedVesselId))
                    {
                        yield return client;
                    }
                }
            }
        }

        private (int cellX, int cellY, int cellZ, int subX, int subY, int subZ) GetCellAndSubCell(Vector3 pos)
        {
            int cellX = (int)Math.Floor(pos.X / CellSize);
            int cellY = (int)Math.Floor(pos.Y / CellSize);
            int cellZ = (int)Math.Floor(pos.Z / CellSize);

            float offsetX = pos.X - cellX * CellSize;
            float offsetY = pos.Y - cellY * CellSize;
            float offsetZ = pos.Z - cellZ * CellSize;

            int subX = (int)Math.Floor(offsetX / SubCellSize);
            int subY = (int)Math.Floor(offsetY / SubCellSize);
            int subZ = (int)Math.Floor(offsetZ / SubCellSize);

            // Clamp just in case of floating errors at boundaries
            subX = Math.Clamp(subX, 0, SubCellsPerCell - 1);
            subY = Math.Clamp(subY, 0, SubCellsPerCell - 1);
            subZ = Math.Clamp(subZ, 0, SubCellsPerCell - 1);

            return (cellX, cellY, cellZ, subX, subY, subZ);
        }

        private void AddVesselToSubCell(Guid vesselId, int cellX, int cellY, int cellZ, int subX, int subY, int subZ)
        {
            var key = (cellX, cellY, cellZ, subX, subY, subZ);
            _subcellVessels.AddOrUpdate(key,
                new HashSet<Guid> { vesselId },
                (k, oldSet) =>
                {
                    lock (oldSet)
                    {
                        oldSet.Add(vesselId);
                        return oldSet;
                    }
                });
        }

        private void RemoveVesselFromSubCell(Guid vesselId, int cellX, int cellY, int cellZ, int subX, int subY, int subZ)
        {
            var key = (cellX, cellY, cellZ, subX, subY, subZ);
            if (_subcellVessels.TryGetValue(key, out var vessels))
            {
                lock (vessels)
                {
                    vessels.Remove(vesselId);
                    // Optional: clean up empty sets to prevent memory leak
                    if (vessels.Count == 0)
                    {
                        _subcellVessels.TryRemove(key, out _);
                    }
                }
            }
        }

        private void AddVesselToCell(Guid vesselId, int cellX, int cellY, int cellZ)
        {
            var key = (cellX, cellY, cellZ);
            _cellVessels.AddOrUpdate(key,
                new HashSet<Guid> { vesselId },
                (k, oldSet) =>
                {
                    lock (oldSet)
                    {
                        oldSet.Add(vesselId);
                        return oldSet;
                    }
                });
        }

        private void RemoveVesselFromCell(Guid vesselId, int cellX, int cellY, int cellZ)
        {
            var key = (cellX, cellY, cellZ);
            if (_cellVessels.TryGetValue(key, out var vessels))
            {
                lock (vessels)
                {
                    vessels.Remove(vesselId);
                    if (vessels.Count == 0)
                    {
                        _cellVessels.TryRemove(key, out _);
                    }
                }
            }
        }

        public void RemoveVessel(Guid vesselId)
        {
            if (_vesselLocation.TryRemove(vesselId, out var loc))
            {
                var (cellX, cellY, cellZ, subX, subY, subZ) = loc;
                RemoveVesselFromSubCell(vesselId, cellX, cellY, cellZ, subX, subY, subZ);
                RemoveVesselFromCell(vesselId, cellX, cellY, cellZ);
            }
        }
    }
}
