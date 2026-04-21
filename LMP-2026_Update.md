# LunaMultiplayer — 2026 Update Log

Full record of every code change applied in this update cycle, what was broken before, what was changed, and what the expected outcome is. Changes are grouped by theme. The final section carries forward the outstanding items from `Code_audit.md`.

---

## 1. Web Server & Telemetry

### 1.1 NullReferenceException on `/map/`

| | |
|---|---|
| **File** | `uhttpsharp/HttpClient.cs` |
| **Line** | 258 |

**Before**
```csharp
if (index == handlers.Count)
{
    return null;          // ← null passed as "next" delegate
}
```
**After**
```csharp
if (index == handlers.Count)
{
    return _ => Task.CompletedTask;   // safe no-op at end of chain
}
```
**Root cause** — `RequestHandlersAggregateExtensions.Aggregate()` returned `null` when it reached the last handler in the pipeline. Any handler that called `next()` at the tail of the chain invoked a null delegate, producing `System.NullReferenceException` on every HTTP request that fell through all routes. Every page on the web dashboard was broken.

**Expected result** — No more NullReferenceException. All HTTP routes resolve cleanly, including `/map/`, `/api/telemetry`, `/api/chat`, and `/`.

---

### 1.2 Trailing-slash path resolution (`/map/` returns 404)

| | |
|---|---|
| **File** | `uhttpsharp/Handlers/FileHandler.cs` |
| **Method** | `Handle()` |

**Before**
```csharp
var requestPath = context.Request.Uri.OriginalString.TrimStart('/');
// "map/" → Path.Combine(root, "map/") → directory → File.Exists = false → silent return
if (!File.Exists(path))
    return;   // Response never set
```
**After**
```csharp
var requestPath = context.Request.Uri.OriginalString.TrimStart('/').TrimEnd('/');
if (string.IsNullOrEmpty(requestPath))
    requestPath = "index.html";

if (!Path.HasExtension(requestPath))
    requestPath += ".html";         // "map" → "map.html"

if (!File.Exists(path))
{
    await next().ConfigureAwait(false);   // pass through rather than silent return
    return;
}
```
**Root cause** — The `HttpRouter` correctly matched `"map"` as a route segment but forwarded the full URI `/map/` to `FileHandler`. The trailing slash made `Path.Combine` produce a directory path; `File.Exists` returned false; the handler returned without setting `context.Response`, leaving it null for downstream handlers (including `CompressionHandler`).

**Expected result** — `http://localhost:8900/map/` and `http://localhost:8900/map` both serve `map.html` correctly.

---

### 1.3 Telemetry `Refresh()` adds wrong object

| | |
|---|---|
| **File** | `Server/Web/Structures/TelemetryData.cs` |
| **Line** | 106 |

**Before**
```csharp
Players.Add(telemetry);   // stale cache object, not the newly built one
```
**After**
```csharp
Players.Add(playerData);  // the VesselStoreSystem fallback entry we just created
```
**Root cause** — In the fallback branch (when a player has an `OwnedVesselId` but no live telemetry cache entry), the code built a `playerData` object from `VesselStoreSystem` then accidentally added the unrelated `telemetry` variable from the outer scope, which was `null` or an unrelated entry.

**Expected result** — Players whose vessel is known to the store but has not yet sent a live position update appear in the telemetry API with static vessel info rather than causing a null entry.

---

### 1.4 Live position data never reached telemetry

| | |
|---|---|
| **Files** | `Server/Message/VesselMsgReader.cs`, `Server/Web/Structures/TelemetryData.cs` |

**Before** — `TelemetryData.UpdatePlayer()` existed but had **zero callers**. `client.OwnedVesselId` was declared but **never assigned** anywhere in the server. Result: `TelemetryData.Refresh()` skipped every client because `client.OwnedVesselId == Guid.Empty`, so `Players` was always empty and `/api/telemetry` always returned an empty list.

**After** — `VesselMsgReader.HandleMessage()` Position case:
```csharp
// Track which vessel this client is piloting
client.OwnedVesselId = posData.VesselId;

// Compute surface speed from velocity vector
var speed = Math.Sqrt(vx*vx + vy*vy + vz*vz);

// Best-effort vessel name/type lookup
VesselStoreSystem.CurrentVessels.TryGetValue(posData.VesselId, out var stored);
var vName = stored?.Fields.GetSingle("name")?.Value ?? "Unknown";
var vType = stored?.Fields.GetSingle("type")?.Value ?? "Unknown";

TelemetryData.UpdatePlayer(
    posData.VesselId, client.PlayerName, vName, vType,
    posData.LatLonAlt[0], posData.LatLonAlt[1], posData.LatLonAlt[2],
    posData.BodyName ?? "Unknown", speed, 0, posData.HeightFromTerrain);
```
**Expected result** — As soon as a player sends their first position tick the map dashboard shows them. Vessel name, body, lat/lon/alt, and surface speed all populate from live wire data.

---

### 1.5 `TotalPlayers` counted the wrong dictionary

| | |
|---|---|
| **File** | `Server/Web/Structures/TelemetryData.cs` |

**Before**
```csharp
public int TotalPlayers => _players.Count;  // static dict, was always 0 before 1.4 fix
```
**After**
```csharp
public int TotalPlayers => ServerContext.PlayerCount;  // ClientRetriever.GetAuthenticatedClients().Length
```
**Root cause** — Even after 1.4 was fixed, `TotalPlayers` pointed at `_players` (the live-update cache). A newly authenticated player who hasn't sent a position yet would still show as 0. `ServerContext.PlayerCount` is updated the moment the handshake completes — the same value the server logs in `"Online Players: N"` debug lines.

**Expected result** — GUI and map dashboard show the correct online count immediately on join, not only after the first position message.

---

### 1.6 Stale telemetry entries never evicted

| | |
|---|---|
| **File** | `Server/Web/Structures/TelemetryData.cs` |

**Before** — `_players` (static `ConcurrentDictionary`) grew forever. Players who crashed or disconnected without calling `RemovePlayer` would leave ghost entries indefinitely.

**After** — `Refresh()` calls `RemoveInactive()` at the top with a threshold of 10 × the primary vessel update interval (~300 ms at the new defaults):
```csharp
RemoveInactive(TimeSpan.FromMilliseconds(
    IntervalSettings.SettingsStore.VesselUpdatesMsInterval * 10));
```
**Expected result** — Ghost vessels disappear from the map within a few seconds of a player disconnecting, even on unclean exits.

---

### 1.7 Chat POST not implemented

| | |
|---|---|
| **File** | `Server/Web/Handlers/ChatRestController.cs` |

**Before**
```csharp
public Task<ChatData> Create(IHttpRequest request)
    => throw new HttpException(HttpResponseCode.MethodNotAllowed, "POST not implemented");
```
**After**
```csharp
public Task<ChatData> Create(IHttpRequest request)
{
    string text;
    if (!request.Post.Parsed.TryGetByName("text", out text) || string.IsNullOrWhiteSpace(text))
        throw new HttpException(HttpResponseCode.BadRequest, "Missing 'text' field");

    WebServer.ChatData.AddMessage("[Server]", text);
    ChatData.Broadcast("[Server]", text);
    return Task.FromResult(WebServer.ChatData);
}
```
**Expected result** — Messages sent from the GUI chat box via `POST /api/chat` are broadcast over Lidgren to all connected KSP clients and appear in the in-game chat, and are reflected back to the map dashboard chat panel.

---

## 2. Server GUI (`LmpServerGUI`)

### 2.1 KSP-themed redesign

| | |
|---|---|
| **File** | `LmpServerGUI/Program.cs` |

**Before** — Plain Windows Forms layout with duplicate control positioning, mixed async/sync stdout reading, no player count display, and a flat grey appearance.

**After** — Full rebuild:
- **KSP colour palette** — near-black `#0D0F14` background, `#5CDD3E` green for running/online states, `#F0A030` orange section headers, `#7CB8FF` blue labels, Consolas monospace throughout.
- **Layout** — Title + live status dot, server path row, Start/Stop/Map buttons, `◈ COMM CHANNEL` chat section with list + input, `◈ SERVER LOG` console with clear button.
- **Fixed layout bugs** — `btnClearLog` was positioned twice in the old code (at `(498,75)` then overridden to `(204,75)`), colliding with `btnOpenChat`.
- **Async-only log** — Removed the old `StandardOutput.Peek()` polling loop that conflicted with `BeginOutputReadLine()`; now uses only the async event handlers.

**Expected result** — GUI is immediately recognisable to KSP players, all controls are reachable, log output is reliable.

---

### 2.2 Player count display

| | |
|---|---|
| **File** | `LmpServerGUI/Program.cs` |

**Before** — No player count anywhere in the GUI.

**After** — `PLAYERS: N` label in the title bar, colour-coded (green when > 0, dim grey when empty). Polls `/api/telemetry` for `TotalPlayers` every 5 seconds via a dedicated timer.

**Expected result** — Server operator sees online player count at a glance without opening the map.

---

### 2.3 Chat integration

| | |
|---|---|
| **File** | `LmpServerGUI/Program.cs` |

**Before** — Chat send used a fire-and-forget `.Result` call (blocking UI thread). Polling rebuilt the entire list on every tick.

**After** — Full async `SendChatAsync` / `PollChatAsync`, only rebuilds list when message count changes, timestamps show local time, `[Server]` messages styled distinctly.

**Expected result** — Chat messages typed in the GUI appear in-game on all connected clients (via the newly implemented POST endpoint in 1.7).

---

### 2.4 Graceful server shutdown (Ctrl+C instead of `exit`)

| | |
|---|---|
| **File** | `LmpServerGUI/Program.cs` |

**Before** — Stop button wrote `"exit"` to stdin, then killed the process after 5 seconds. The `Exited` event handler always showed **"CRASHED"** regardless of how the process ended.

**After**
```csharp
private static bool SendCtrlC(int processId)
{
    if (!AttachConsole((uint)processId)) return false;
    SetConsoleCtrlHandler(IntPtr.Zero, true);   // suppress in this process
    try   { return GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0); }
    finally { FreeConsole(); SetConsoleCtrlHandler(IntPtr.Zero, false); }
}
```
- `_stopping` flag (`volatile bool`) set before initiating stop, cleared in the `Exited` handler.
- `Exited` handler branches: `_stopping == true` → **OFFLINE** (dim); `_stopping == false` → **CRASHED** (red).
- 8-second grace window before force-kill.
- `GracefulStop()` shared by both the Stop button and `OnFormClosing`.
- `CheckProcess()` timer only fires the CRASHED path when `_stopping` is false, preventing races.

**Expected result** — Stop button triggers the server's normal `CancelKeyPress` / `SIGINT` shutdown path (runs backups, flushes buffers, disconnects clients cleanly). Status shows **OFFLINE** on clean stop and **CRASHED** only on unexpected exit.

---

## 3. Map Dashboard (`map.html`)

| | |
|---|---|
| **File** | `Server/Web/Pages/map.html` |

**Before** — Basic dark table with player name, status, body, lat/lon, speed. Generic dark theme, no vessel type icons, no altitude column, no chat.

**After** — Mission-control layout:
- Matches KSP colour palette (same variables as GUI).
- **Telemetry panel** — vessel type emoji icons (🚀 Ship, 🛸 Probe, 🚗 Rover, 🛰 Station, etc.), altitude column, speed column, live green dot per player.
- **Comm channel panel** — live chat sidebar, polls `/api/chat` every 2 s, send on Enter or button, `[Server]` messages styled in orange.
- Telemetry refreshes every 1.5 s.
- Vessel count and last-update time in the top bar.

**Expected result** — Anyone browsing `http://localhost:8900/map/` sees a real-time vessel tracker and can read server chat without needing the GUI.

---

## 4. Network Performance

### 4.1 Position update rate

| | |
|---|---|
| **File** | `Server/Settings/Definition/IntervalSettingsDefinition.cs` |

| Setting | Before | After | Effect |
|---|---|---|---|
| `VesselUpdatesMsInterval` | 50 ms | **30 ms** | 20 Hz → 33 Hz nearby vessel updates |
| `SecondaryVesselUpdatesMsInterval` | 150 ms | **80 ms** | Uncontrolled / distant vessels update faster |
| `SendReceiveThreadTickMs` | 5 ms | **2 ms** | Reduces idle-queue latency by up to 3 ms per direction |
| `MainTimeTick` | 5 ms | **2 ms** | Main loop processes timer events more promptly |
| `GcMinutesInterval` | 15 min | **0 (off)** | Eliminates forced stop-the-world GC pause (100–200 ms every 15 min) |

**Expected result** — Other players' vessels move visibly smoother (33 snapshots/sec vs 20). Jitter from server-side latency floor drops from ~25 ms to ~10 ms. No more periodic 150+ ms freeze from forced GC.

---

### 4.2 MTU auto-expansion

| | |
|---|---|
| **File** | `Server/Settings/Definition/ConnectionSettingsDefinition.cs` |

**Before**
```csharp
public bool AutoExpandMtu { get; set; } = false;
```
**After**
```csharp
public bool AutoExpandMtu { get; set; } = true;
```
**Root cause** — The default 1408-byte MTU (Lidgren's `kDefaultMTU`) means large vessel proto messages (full part trees on complex craft) are fragmented across multiple UDP datagrams. Loss of any single fragment drops the entire reassembled message, triggering a resend or missing state update.

**Expected result** — Lidgren negotiates the largest MTU supported between server and client (typically 1472–8192 bytes on LAN, 1472 on internet). Large vessel syncs send in fewer packets, reducing fragmentation loss and sync time when players first join or load a vessel.

---

### 4.3 Send-queue backlog detection

| | |
|---|---|
| **File** | `Server/Client/ClientMainThread.cs` |

**Before** — No visibility into per-client send-queue depth. A client whose connection couldn't keep up with send rate would silently fall further and further behind.

**After** — Main loop checks `client.SendMessageQueue.Count` every tick and logs a warning when any client exceeds 50 queued messages:
```csharp
if (depth > BacklogWarnThreshold)
    LunaLog.Warning($"Send queue backlog for '{client.PlayerName}': {depth} messages pending");
```
**Expected result** — Operators see early warning in the server log when a specific player is causing send-side congestion (e.g. on a slow connection or if the update rate is set too aggressively for their bandwidth).

---

## 5. Outstanding Items from `Code_audit.md`

These items were identified in the original audit and have **not yet been implemented**. They are documented here for the next development cycle.

---

### 5.1 🔴 UDP Amplification — Master Server

| | |
|---|---|
| **File** | `LmpMasterServer/Lidgren/MasterServer.cs` |
| **Priority** | Critical |

**Problem** — The master server responds to unverified UDP packets without rate-limiting or source validation. An attacker can spoof a victim's IP, send small packets to the master server, and cause it to flood the victim with large responses (classic UDP amplification DDoS vector).

**Required fix** — Add per-IP rate limiting on `NetIncomingMessageType.UnconnectedData` handlers. Validate that the requesting IP is reachable (challenge-response handshake) before reflecting any data.

**Expected result** — Master server cannot be weaponised as a DDoS amplifier; also reduces unnecessary outbound bandwidth.

---

### 5.2 🔴 Memory Leak in Message Pooling

| | |
|---|---|
| **File** | `Server/Server/MessageReceiver.cs` |
| **Priority** | Critical |

**Problem** — Pooled messages are not always returned to the pool. Under load, allocations accumulate, causing GC pressure and eventual out-of-memory conditions. GcSystem was added as a workaround (now disabled — see 4.1).

**Required fix** — Audit every `message.Recycle()` call site. Ensure messages are recycled in all error paths (try/finally blocks). Add pool depth monitoring so leaks surface quickly.

**Expected result** — Stable memory usage under sustained load; GC pauses remain short without forced collection.

---

### 5.3 🔴 Area of Interest (AoI) — MMO Scale

| | |
|---|---|
| **Files** | `Server/System/SpatialGrid.cs`, `Server/Message/VesselMsgReader.cs` |
| **Priority** | Critical for > 20 simultaneous players |

**Problem** — The current `SpatialGrid` relays every position update to all "interested" clients within a fixed 3 km radius. As player count grows, the number of relay operations scales as O(n²). Beyond ~20 players in the same area, bandwidth and CPU saturate.

**Required fix** — Implement a configurable interest radius in `SpatialGrid.GetInterestedClientsFullDetail()`. Add a LOD tier: clients within 3 km get every update, clients within 30 km get every 3rd update, clients beyond 30 km get no position updates (only periodic proto syncs). Expose the radii as server settings.

**Expected result** — Server can support 50–100+ simultaneous vessels in the same area without bandwidth saturation.

---

### 5.4 🔴 Predicted Movement / Dead Reckoning

| | |
|---|---|
| **Files** | `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs`, `VesselPositionUpdate.cs`, `ExtensionMethods/VesselPositioner.cs` |
| **Priority** | Critical for perceived smoothness |

**Problem** — The client interpolates between two received snapshots using a linear orbit Lerp. A dead-reckoning block exists in `HandleVesselUpdates()` and computes `PredictedPosition` / `PredictionError`, but `PredictedPosition` is **never read by the actual positioning code** in `VesselPositioner.SetVesselPosition()`. The orbit Lerp has a hardcoded `blendFactor = 0.1f` (also unused by the orbiter). Result: at 30 ms update rate there is always at least one snapshot of visual lag on other vessels.

**Required fix**
1. Wire `PredictedPosition` into `VesselPositioner.SetVesselPosition()` as the primary position source.
2. Use the orbit Lerp only to correct drift between the dead-reckoned position and the authoritative snapshot.
3. Make `blendFactor` adaptive: `Mathf.Clamp(PredictionError * errorGain, minBlend, maxBlend)` so vessels snap quickly after large corrections and glide smoothly during normal flight.

**Expected result** — Other vessels appear to move continuously between updates rather than stepping. Perceived lag drops from ~30 ms (snapshot interval) to near zero under stable network conditions.

---

### 5.5 🟡 Delta Compression

| | |
|---|---|
| **Files** | `LmpCommon/Message/Data/Vessel/VesselPositionMsgData.cs` and related |
| **Priority** | Medium — required before scaling beyond ~10 simultaneous vessels |

**Problem** — Every position message sends the full state: lat, lon, alt, velocity (3), normal (3), rotation (4), orbit (8), body name. At 30 ms intervals per vessel that is ~300 bytes × 33 Hz = ~10 KB/s per vessel. With 20 vessels nearby, a single client receives ~200 KB/s of position data alone.

**Required fix** — Track the last acknowledged state per vessel per client. Send only fields that changed by more than a configurable epsilon. Use fixed-point encoding for angles and positions (e.g. lat/lon in microdegrees as `int32`, alt as `int32` cm). Estimated savings: 60–80% bandwidth reduction.

**Expected result** — Position update bandwidth per client stays roughly constant regardless of player count, enabling true MMO-scale scenarios.

---

### 5.6 🟡 Server List Allocation Pressure — Master Server

| | |
|---|---|
| **File** | `LmpMasterServer/Lidgren/MasterServer.cs` |
| **Priority** | Medium |

**Problem** — The master server reconstructs full server-list objects on every client poll. With many polling clients this generates heavy short-lived allocation pressure.

**Required fix** — Cache the serialised server list and invalidate only when a server registers or deregisters. Serve the cached bytes directly.

**Expected result** — Master server GC pressure reduced; stays responsive under many simultaneous list requests.

---

### 5.7 🟡 Process Termination Resilience — Master Server

| | |
|---|---|
| **File** | `LmpMasterServer/Lidgren/MasterServer.cs` |
| **Priority** | Medium |

**Problem** — Unhandled exceptions in background tasks can crash the master server process entirely.

**Required fix** — Wrap all background task entry points in top-level try/catch. Log the exception and restart the affected subsystem rather than letting it propagate to the process boundary.

**Expected result** — Master server remains online through non-critical errors.

---

### 5.8 🟡 Quantization of Rotation/Position

| | |
|---|---|
| **Files** | `LmpCommon/Message/Data/Vessel/VesselPositionMsgData.cs` |
| **Priority** | Medium — complements 5.5 |

**Problem** — Rotations are sent as 4 × `float` (16 bytes). Smallest-3 quaternion compression can represent the same rotation in 6 bytes. Lat/lon sent as `double` (16 bytes) but 32-bit fixed-point at 1-cm resolution requires only 8 bytes.

**Required fix** — Implement smallest-3 encoding for `SrfRelRotation`. Encode `LatLonAlt` as fixed-point `int32` triplets. Add a protocol version field so old clients degrade gracefully.

**Expected result** — ~30% reduction in position message size on top of delta compression.

---

### 5.9 🔵 Reflection-based Message Instantiation

| | |
|---|---|
| **File** | `LmpCommon/Message/Base/MessageStore.cs` |
| **Priority** | Low |

**Problem** — `MessageStore` uses `Activator.CreateInstance` (reflection) to instantiate message data objects. This is measurably slower than a compiled factory and generates extra allocations.

**Required fix** — Replace with `Expression`-compiled factories cached per type. Pattern: `Expression.Lambda<Func<T>>(Expression.New(typeof(T).GetConstructor(...))).Compile()`.

**Expected result** — Marginal (< 1 ms) improvement per message creation under high message rates.

---

### 5.10 🔵 Adaptive Tick Rates

| | |
|---|---|
| **Files** | `Server/Settings/Definition/IntervalSettingsDefinition.cs`, `Server/Message/VesselMsgReader.cs` |
| **Priority** | Low — saves CPU/bandwidth at low player density |

**Problem** — Update intervals are global constants. A vessel sitting stationary on a launchpad sends position at the same 30 ms rate as a vessel in a dogfight.

**Required fix** — Add a velocity/acceleration threshold: if a vessel's position delta between two consecutive updates is below a configurable epsilon, increase its update interval up to `SecondaryVesselUpdatesMsInterval`. Reset to the primary interval as soon as motion is detected.

**Expected result** — Stationary vessels consume near-zero bandwidth; no change to vessels in flight.

---

### 5.11 🔵 Binary Schema Optimisation

| | |
|---|---|
| **Files** | `LmpCommon/Message/Data/` — all `MsgData` classes |
| **Priority** | Low |

**Problem** — Variable-length string fields (`BodyName`, `PlayerName`) are serialised with Lidgren's `WriteString()` which prefixes a 2-byte length. For short strings that appear in every position message, a fixed-width 16-byte field (null-padded) eliminates the length prefix and allows zero-copy reads.

**Required fix** — Profile which string fields appear in high-frequency messages. Convert those to fixed-width byte arrays where the max length is known and bounded (body names are at most ~16 chars in stock KSP).

**Expected result** — Minor deserialisation speedup on the hot path; consistent message sizes simplify future delta-compression implementation.

---

## Summary Table

| # | File(s) | Status | Category |
|---|---|---|---|
| 1.1 | `uhttpsharp/HttpClient.cs` | ✅ Done | Bug fix |
| 1.2 | `uhttpsharp/Handlers/FileHandler.cs` | ✅ Done | Bug fix |
| 1.3 | `Server/Web/Structures/TelemetryData.cs` | ✅ Done | Bug fix |
| 1.4 | `Server/Message/VesselMsgReader.cs` + `TelemetryData.cs` | ✅ Done | Feature wire-up |
| 1.5 | `Server/Web/Structures/TelemetryData.cs` | ✅ Done | Bug fix |
| 1.6 | `Server/Web/Structures/TelemetryData.cs` | ✅ Done | Memory / correctness |
| 1.7 | `Server/Web/Handlers/ChatRestController.cs` | ✅ Done | Feature |
| 2.1 | `LmpServerGUI/Program.cs` | ✅ Done | UX |
| 2.2 | `LmpServerGUI/Program.cs` | ✅ Done | UX |
| 2.3 | `LmpServerGUI/Program.cs` | ✅ Done | UX |
| 2.4 | `LmpServerGUI/Program.cs` | ✅ Done | Bug fix / UX |
| 3.0 | `Server/Web/Pages/map.html` | ✅ Done | UX |
| 4.1 | `Server/Settings/Definition/IntervalSettingsDefinition.cs` | ✅ Done | Performance |
| 4.2 | `Server/Settings/Definition/ConnectionSettingsDefinition.cs` | ✅ Done | Performance |
| 4.3 | `Server/Client/ClientMainThread.cs` | ✅ Done | Observability |
| 5.1 | `LmpMasterServer/Lidgren/MasterServer.cs` | ⏳ Pending | Security — Critical |
| 5.2 | `Server/Server/MessageReceiver.cs` | ⏳ Pending | Stability — Critical |
| 5.3 | `Server/System/SpatialGrid.cs` | ⏳ Pending | Scale — Critical |
| 5.4 | `LmpClient/Systems/VesselPositionSys/` | ⏳ Pending | Smoothness — Critical |
| 5.5 | `LmpCommon/Message/Data/Vessel/` | ⏳ Pending | Bandwidth — Medium |
| 5.6 | `LmpMasterServer/Lidgren/MasterServer.cs` | ⏳ Pending | Performance — Medium |
| 5.7 | `LmpMasterServer/Lidgren/MasterServer.cs` | ⏳ Pending | Stability — Medium |
| 5.8 | `LmpCommon/Message/Data/Vessel/` | ⏳ Pending | Bandwidth — Medium |
| 5.9 | `LmpCommon/Message/Base/MessageStore.cs` | ⏳ Pending | Performance — Low |
| 5.10 | `Server/Settings/Definition/IntervalSettingsDefinition.cs` | ⏳ Pending | Efficiency — Low |
| 5.11 | `LmpCommon/Message/Data/` | ⏳ Pending | Efficiency — Low |
