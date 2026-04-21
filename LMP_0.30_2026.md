# LunaMultiplayer 0.30.0 — 2026 Change Log

All changes applied to the `LunaMultiplayer-GPTOptimized` fork since branching from the
upstream `0.29.1` release. Changes are listed newest-first within each section.

---

## Version

| Component | Old | New |
|-----------|-----|-----|
| All assemblies | `0.29.1` | `0.30.0` |
| KSPAssembly attribute | `("LMP", 0, 30)` | `("LMP", 0, 30)` *(unchanged — stays compatible with KSP's loader)* |
| Cross-compatible peer | — | `0.30.x ↔ 0.29.x` (joining players on 0.29 can still connect) |

---

## Bug Fixes

### Kerbal trait `NullReferenceException` on login
**Files:** `LmpClient/Systems/KerbalSys/KerbalSystem.cs`

`KerbalRoster.SetExperienceTrait` crashed when a kerbal's ConfigNode was missing the
`trait =` field. The client now detects a missing or empty trait before constructing
`ProtoCrewMember` and defaults it to `"Pilot"`, logging a warning so the broken file
can be identified and repaired server-side.

---

### Server-side kerbal file repair
**Files:** `Server/System/KerbalSystem.cs`

Added `KerbalFileIsValid(byte[])` validation and `RepairAndOverwrite(string, string)`
repair logic. On `HandleKerbalsRequest` each kerbal file is checked for:
- Valid UTF-8 encoding and non-empty content
- Presence of `name =` and `trait =` fields

Stock kerbals (Jebediah, Bill, Bob, Valentina) are restored from embedded resource
templates. All others receive a generated pilot template. Fixes corrupt `Bob Kerman.txt`
and `Harbal Kerman.txt` that were causing all players to crash on login.

---

### Ghost vessel `#autoLOC_*` crash (FlightIntegrator / Part.Update)
**Files:** `Server/System/VesselStoreSystem.cs`, `Server/Message/VesselMsgReader.cs`

Vessels saved with an unresolved KSP localisation key as their name (e.g.
`#autoLOC_8005483`) cause `ArgumentOutOfRangeException` in
`FlightIntegrator.UpdateOcclusionSolar` and `NullReferenceException` in `Part.Update`
on every frame for every connected client.

- `VesselStoreSystem.LoadExistingVessels` now calls `VesselNameIsValid()` and skips any
  vessel file whose name is missing, empty, or starts with `#autoLOC_`.
- `VesselMsgReader.HandleVesselProto` rejects inbound proto messages with the same rule,
  preventing the bad vessel from being stored in the first place.
- The offending 375 KB ghost vessel was manually deleted from the Universe/Vessels folder.

---

### 5 FPS lag — vessel sync sending full protos every 10 seconds
**Files:** `Server/Message/VesselMsgReader.cs`

The `HandleVesselsSync` path was sending the full serialised proto of every stored vessel
to every client every 10 seconds regardless of whether the vessel had changed. With 11
vessels averaging ~15 KB each (~165 KB total) this caused multi-second KSP parse stalls.

Fix: added a monotonic per-vessel version counter (bumped only when the vessel owner
pushes a new proto) and a per-client received-version cache. `HandleVesselsSync` now
skips any vessel whose version the client has already received. The full proto is only
sent when the vessel structurally changes.

Added `VesselMsgReader.ClearClientSyncCache(playerName)` called from
`ClientConnectionHandler.DisconnectClient` so a reconnecting player always gets a fresh
full sync.

---

### Server list empty (master server fetch failing in KSP's Mono)
**Files:** `LmpCommon/RepoRetrievers/MasterServerRetriever.cs`

`MasterServerRetriever` fetches the list of master server IPs from GitHub over HTTPS
using `WebClient`. KSP's old Mono runtime frequently fails this silently (TLS/certificate
issues), leaving `MasterServersEndpoints` empty. With no endpoints, zero requests are
sent and the server list stays permanently blank.

Fix: after the GitHub fetch, if the endpoint set is still empty, the retriever now
populates it from a hardcoded fallback list:
- `ms.lmp.dasskelett.dev:8700`
- `lmp.nightshade.fun:8700`

---

## New Features

### In-game version display
**Files:** `LmpClient/Windows/Status/StatusWindow.cs`

The LMP status window title now shows the running version:
- Release builds: `LMP 0.30.0`
- Debug builds: `LMP 0.30.0 - PID: <pid>`

---

### Lag detection & reporting — server
**Files:** `Server/Log/LagReporter.cs`, `Server/Server/MessageReceiver.cs`,
`Server/Message/VesselMsgReader.cs`, `Server/MainServer.cs`

`LagReporter` is a static class that writes detailed diagnostic snapshots to
`logs/LagReport.log` (alongside the existing server log). It is triggered by:

| Trigger | Threshold |
|---------|-----------|
| Single message handler slow | > 500 ms |
| Vessel sync pass slow | > 300 ms |
| GC heap spike | > 200 MB growth in one 5-second monitor cycle |

Each report includes: timestamp, trigger description, server uptime, GC memory,
authenticated player count, per-player name/ping/subspace, rolling 200-message
timing statistics (avg / p95 / max), and the full stored vessel ID list.

Reports are throttled to one per 10 seconds to prevent log flooding. A background
monitor task (`LagReporter.RunMonitorAsync`) is started alongside the other server
tasks in `MainServer.cs`.

---

### Lag detection & reporting — client
**Files:** `LmpClient/Systems/LagReportSys/LagReportSystem.cs`

`LagReportSystem` is an always-enabled LMP system that monitors Unity frame times
every update and writes a snapshot to `LagReport.log` in the KSP root folder when:

| Trigger | Condition |
|---------|-----------|
| Instant severe spike | Single frame < 5 FPS |
| Sustained lag | Average < 20 FPS for > 1 second |

Each report includes: UTC timestamp, trigger description, scene, network/LMP state,
GC memory, rolling 120-frame statistics (avg frame time, worst-5%, min/max FPS),
and — in flight/tracking scenes — total vessel count and loaded-in-physics count.

Reports are throttled to one per 15 seconds. File writes happen on a background
thread so the reporter cannot itself contribute to the lag it is measuring.

---

## Diagnostics Added (temporary — can be removed once issues are confirmed resolved)

### Master server request logging
**File:** `LmpClient/Network/NetworkSender.cs`

Logs the master server endpoints being queried each time the server list is requested:
```
[LMP]: Sending master server request to 4 endpoint(s): 116.203.125.175:8700, ...
```

### Server list version filter logging
**File:** `LmpClient/Network/NetworkServerList.cs`

Logs each server entry that is dropped by the version compatibility filter:
```
[LMP]: Server list: skipping server 'Foo' — version 0.29.0 not compatible with local 0.30.0
```

---

## Infrastructure

### `commit.ps1` — interactive commit helper
**File:** `commit.ps1` (repo root)

PowerShell script with a colour console UI for staging, documenting, and pushing
changes. On each run it:
1. Shows `git status` and a diff summary.
2. Prompts for a short change description and optional detail.
3. Appends a timestamped entry to `LMP_0.30_2026.md`.
4. Stages all tracked changes, commits with a generated message, and pushes to `origin/master`.
5. Optionally launches `release.ps1` immediately after push.

Run with: `powershell -ExecutionPolicy Bypass -File commit.ps1`

---

### `release.ps1` — release packager & publisher
**File:** `release.ps1` (repo root)

PowerShell script that packages locally-built binaries into distributable ZIPs and
publishes a versioned GitHub release via the `gh` CLI. On each run it:

1. Reads the version from `LmpCommon/Properties/AssemblyInfo.cs`.
2. Verifies that `LmpClient/bin/Release/` and `Server/bin/Release/net8.0/` are populated.
3. Builds **`LMP-{version}-Client.zip`** with the correct KSP `GameData/` layout:
   - `GameData/LunaMultiplayer/` — `LmpClient.dll`, `LmpCommon.dll`, `LmpGlobal.dll`,
     `Lidgren.Network.dll`, `CachedQuickLz.dll`, `JsonFx.dll`, `System.Runtime.Serialization.dll`
   - `GameData/000_Harmony/` — `0Harmony.dll`, `HarmonyInstallChecker.dll`, `Harmony.version`
4. Builds **`LMP-{version}-Server.zip`** with the `LMPServer/` folder (excludes `.pdb` files
   and runtime-created dirs: `logs/`, `Universe/`, `Config/`).
5. Creates a GitHub release tagged `v{version}` and uploads both ZIPs as assets.
6. On tag conflict, uploads assets to the existing release instead.

**Prerequisites:** `gh` CLI installed (`https://cli.github.com`) and authenticated (`gh auth login`).

Run with: `powershell -ExecutionPolicy Bypass -File release.ps1`
Or triggered automatically at the end of `commit.ps1`.

---

### `.github/workflows/release.yml` — CI release verification
**File:** `.github/workflows/release.yml`

GitHub Actions workflow triggered when a `v*.*.*` tag is pushed. It:
1. Builds and tests the server project on Ubuntu to verify the tag is clean.
2. Marks the GitHub release as latest and updates its notes from `LMP_0.30_2026.md`.

---

*This document is updated automatically by `commit.ps1` on each push.*
