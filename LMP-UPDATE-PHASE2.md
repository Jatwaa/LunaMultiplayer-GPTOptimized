# LMP Update – Phase 2: Optimizing Tight Player Clusters

This document outlines a set of incremental optimizations aimed at reducing bandwidth and CPU load when many players congregate in the same Area‑of‑Interest (AOI) cell (e.g., a crowded space‑station, large fleet battle, or busy launch pad).  
Each item can be implemented independently; later items build on earlier ones.

---

## 1. Hierarchical / Multi‑Resolution Interest Management
- **Sub‑cell partitioning**: Divide each 10 km³ AOI cell into smaller sub‑cells (e.g., 1 km³).  
  - Clients receive full‑detail updates only for vessels in their own sub‑cell and the 8 neighboring sub‑cells.  
  - Vessels farther away are still visible but at reduced fidelity (see #2).  
- **Dynamic Octree / Quadtree**: Build a spatial tree where leaf size shrinks as local vessel density grows.  
  - The server can quickly compute the smallest leaf that guarantees a bound on positional error and send only the needed detail.

## 2. Adaptive Update‑Rate & Data‑LOD (Level‑of‑Detail)
| Distance from client (or leaf size) | Update rate | Payload (example) |
|-------------------------------------|------------|-------------------|
| ≤ 2 km (same sub‑cell)              | 10 Hz      | Full (pos, vel, rot, part‑sync) |
| 2 km – 5 km (adjacent sub‑cells)    | 5 Hz       | Pos + vel only |
| > 5 km (same cell, farther)         | 1 Hz       | Pos only (quaternion optional) |
| Outside cell (still relevant “blip”)| 0.2 Hz     | Vessel ID + coarse bounding sphere |

- The server decides the rate/LOD based on client‑to‑vessel distance (or leaf size).  
- Clients extrapolate (dead‑reckon) between updates; lower rates tolerate larger prediction error before a corrective snap.

## 3. Quantization & Bit‑Packing of Vessel State
- **Position**: 16‑bit fixed‑point per axis within cell bounds → 6 bytes (vs 24 bytes for three doubles).  
- **Velocity / Angular Velocity**: 12‑bit signed → 3 bytes each.  
- **Rotation**: Store three quaternion components (4th derived) with 10‑bit each → ~4 bytes.  
- **Part‑sync fields**: Only send changed fields (delta compression) and pack booleans into bits.  
- Target: reduce a typical vessel update from ~200 bytes to **30‑50 bytes**.

## 4. Delta Compression + Ack‑Based Resend
- Keep last known state per vessel per client.  
- Transmit only fields that differ from the last acknowledged state.  
- Use small NACK/ACK messages or a timeout‑based resend to recover lost updates.  
- Cuts bandwidth dramatically for slowly‑moving or idle vessels.

## 5. Interest‑Based Culling of Non‑Essential Objects
- Tag low‑priority objects (debris, EVA kerbals, small probes).  
- For remote clients:  
  - Send at a much lower rate (e.g., 0.5 Hz) with position only, **or**  
  - Omit entirely beyond a distance threshold (e.g., > 3 km).  
- Particularly useful in crowded launch pads with hundreds of small parts.

## 6. Server‑Side Batching & UDP MTU Optimization
- Pack as many vessel updates as possible into a single UDP packet (≤ ~1400 bytes after IP/UDP headers).  
- Include a per‑packet sequence number so the client can detect loss and request a retransmission of the whole batch if needed.  
- Reduces per‑packet overhead and system‑call count.

## 7. Client‑Side Interpolation Buffer (Small Render Delay)
- Introduce a fixed 50‑100 ms render buffer: render the vessel at a slightly delayed simulation time.  
- Gives the client extra time to receive the next update, reducing visible snaps under jitter.  
- Adds a tiny, constant input lag imperceptible in KSP but greatly improves visual smoothness.

## 8. Load‑Sharding / Zone Servers (Future Scale‑Out)
- Split the solar system into zones (inner planets, outer planets, major moons).  
- Each zone runs its own physics server; vessels crossing zone boundaries are handed off via a lightweight zone‑transfer message.  
- Keeps physics CPU per instance low while supporting thousands of players globally.

---

### Implementation Roadmap (suggested order)

1. **Sub‑cell Spatial Grid** – modify `SpatialGrid` to track sub‑cells and provide filtered interested‑client lists.  
2. **LOD selection helper** – function that returns an enum (`FULL`, `POS_ONLY`, `NONE`) based on distance or sub‑cell depth.  
3. **Quantization helpers** – static methods for converting floats/ doubles to reduced‑bit integers and back.  
4. **Delta tracking in `MessageReceiver`** – store last sent `VesselPositionMsgData` per (client, vessel) and send only diffs.  
5. **Apply quantization & LOD** to the serialization of `VesselPositionMsgData` (and optionally `VesselPartSyncFieldMsgData`).  
6. **Implement UDP batching** in `MessageQueuer` (collect updates per tick, send as one packet when MTU or time threshold reached).  
7. **Add client interpolation buffer** – small delay before applying predicted position to the vessel’s transform.  
8. **Optional: Zone server architecture** – for when player counts exceed a single‑server threshold.

--- 

*This document is a living artifact; as each item is implemented, mark it as completed and update the estimates of supported player counts accordingly.*