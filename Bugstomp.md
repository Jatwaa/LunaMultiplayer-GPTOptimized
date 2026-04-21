# Craft Ghosting Bug Report

**Date:** 2026-04-16
**Project:** LunaMultiPlayer (KSP Multiplayer Mod)
**Issue:** Players see duplicate/ghostly versions of other players' crafts

---

## Root Cause Summary

Ghosting is caused by **position interpolation not using dead reckoning predictions**, combined with **queue cleanup failures** and **race conditions** in the vessel lifecycle.

---

## Critical Bugs

### Bug #1: Dead Reckoning PredictedPosition Never Used

**File:** `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs`
**Lines:** 100-119

```csharp
// PredictedPosition is calculated but never read
Vector3 velocity = new Vector3((float)update.Velocity.x, ...);
update.PredictedPosition += velocity * dt;

// But VesselPositioner.SetVesselPosition() uses simple lerp:
var lerpedPos = Vector3d.Lerp(currentPos, targetPos, percentage); // Line 37
```

**Impact:** At 30ms update rate, vessels visually "step" instead of moving smoothly. This is the **primary cause** of craft ghosting.

**Fix:** Wire `PredictedPosition` as the primary position source in `VesselPositioner.SetVesselPosition()`.

---

### Bug #2: Position Update Queue Not Cleaned on Vessel Remove

**File:** `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs`
**Lines:** 226-230

```csharp
public void RemoveVessel(Guid vesselId)
{
    CurrentVesselUpdate.TryRemove(vesselId, out _);
    TargetVesselUpdateQueue.TryRemove(vesselId, out _);  // Queue may not be empty!
}
```

**Issue:** `AdjustExtraInterpolationTimes()` clears `Target` but **never dequeues old entries** from `TargetVesselUpdateQueue`. Stale future positions can cause interpolation to use wrong data.

**Fix:** Fully drain the queue on remove, not just TryRemove the dictionary entry.

---

### Bug #3: Race Between Vessel Kill and Position Queue

**File:** `LmpClient/Systems/VesselPositionSys/VesselPositionMessageHandler.cs`
**Lines:** 14-32

```csharp
public void HandleMessage(IServerMessageBase msg)
{
    if (!VesselCommon.DoVesselChecks(vesselId))  // Checks kill list
        return;
    // Race: vessel could be killed RIGHT after this check
    // Position update for dead vessel still gets queued
}
```

**Impact:** Position updates can re-queue a vessel right after it's killed, causing a brief ghost to reappear.

**Fix:** Add explicit vessel existence check (`FlightGlobals.FindVessel()`) before queueing.

---

### Bug #4: KSC Ghost Purge Only Runs on Player Join

**File:** `Server/Message/VesselMsgReader.cs`
**Lines:** 279-311

```csharp
private static void PurgeStaleKscVessels()
{
    // Called only during HandleVesselsSync (when a new player joins)
    // Does NOT run periodically or when vessels are abandoned
}
```

**Impact:** When a player disconnects while their vessel is at KSC, the ghost persists until the **next** player joins.

**Fix:** Run ghost purge on a timer in addition to on-join.

---

### Bug #5: Vessel Load Timing Race

**File:** `LmpClient/Systems/VesselProtoSys/VesselProtoSystem.cs`
**Lines:** 147-163

```csharp
var existingVessel = FlightGlobals.FindVessel(vesselProto.VesselId);
if (existingVessel == null)
{
    VesselLoader.LoadVessel(protoVessel, forceReload);
}
```

**Impact:** Position updates may arrive before the vessel is fully loaded, causing them to be dropped or interpolated incorrectly.

**Fix:** Queue position updates until vessel load is confirmed, or drop early updates gracefully.

---

### Bug #6: Decouple/Undock Lock Acquisition Without Cleanup

**File:** `LmpClient/Systems/VesselDecoupleSys/VesselDecoupleEvents.cs`
**Lines:** 24-25

```csharp
LockSystem.Singleton.AcquireUnloadedUpdateLock(part.vessel.id, true, true);
LockSystem.Singleton.AcquireUpdateLock(part.vessel.id, true, true);
```

**Impact:** If the decouple message is lost, the new vessel never receives proper position updates and can appear as a ghost at the wrong location.

**Fix:** Implement retry logic or cleanup path for failed decouple/undock events.

---

### Bug #7: Vessel Unload/Rail State Desync

**File:** `LmpClient/Systems/VesselPositionSys/VesselPositionUpdate.cs`
**Lines:** 139-181

```csharp
if (InterpolationFinished && VesselPositionSystem.TargetVesselUpdateQueue.TryGetValue(VesselId, out var queue) && queue.TryDequeue(out var targetUpdate))
{
    Vessel.SetVesselPosition(this, Target, LerpPercentage);
}

// ALWAYS set position even without queue data
Vessel.SetVesselPosition(this, Target, LerpPercentage);
```

**Impact:** When a vessel goes off-rails (`RailEvent.onVesselGoneOffRails`), the packed state may not match server expectations. `FlightIntegrator_FixedUpdate.cs` skips physics for immortal vessels, causing visual desync.

---

## Priority Fix Order

| Priority | Bug | Effort | Impact |
|----------|-----|--------|--------|
| 1 | #1 Dead Reckoning | Medium | High - Primary cause |
| 2 | #2 Queue Cleanup | Low | High - Stale ghosts |
| 3 | #3 Kill Race | Low | Medium - Brief reappearing ghosts |
| 4 | #4 KSC Purge | Low | Low - Only on rejoin |
| 5 | #5 Load Race | Medium | Medium |
| 6 | #6 Lock Cleanup | Medium | Low |
| 7 | #7 Rail Desync | High | Low |

---

## Files to Examine

| File | Key Lines | Status |
|------|-----------|--------|
| `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs` | 37, 100-119, 226-230, 244-248 | Needs fixes |
| `LmpClient/Systems/VesselPositionSys/VesselPositionMessageHandler.cs` | 14-32 | Needs fix |
| `Server/Message/VesselMsgReader.cs` | 279-311 | Needs timer |
| `LmpClient/Systems/VesselProtoSys/VesselProtoSystem.cs` | 147-163 | Needs sync |
| `LmpClient/Systems/VesselDecoupleSys/VesselDecoupleEvents.cs` | 24-25 | Needs retry |
| `LmpClient/Systems/VesselRemoveSys/VesselRemoveSystem.cs` | 96-108 | Verify ordering |

---

## Lock System Bugs

### Lock Bug #1: Kerbal Lock Type Check Against Wrong Dictionary

**File:** `LmpCommon/Locks/LockQuery.cs`
**Line:** 171

```csharp
case LockType.Kerbal:
    return LockStore.SpectatorLocks.ContainsKey(lockDefinition.KerbalName);  // BUG: Should be KerbalLocks!
```

**Impact:** `LockExists()` for Kerbal locks **always returns false** (checks empty SpectatorLocks). This causes lock acquisition logic to fail or behave incorrectly.

**Fix:** Change `SpectatorLocks` to `KerbalLocks`.

---

### Lock Bug #2: Direct LockStore Manipulation Bypasses Server

**File:** `LmpClient/Systems/VesselLockSys/VesselLockEvents.cs`
**Lines:** 117-118, 124-125, 137-138

```csharp
// Lines 117-118: Direct manipulation
LockSystem.LockStore.RemoveLock(LockSystem.LockQuery.GetUpdateLock(lockDefinition.VesselId));
LockSystem.LockStore.AddOrUpdateLock(new LockDefinition(LockType.UnloadedUpdate, lockDefinition.PlayerName, lockDefinition.VesselId));

// Lines 124-125: Same issue
LockSystem.LockStore.RemoveLock(LockSystem.LockQuery.GetUnloadedUpdateLock(lockDefinition.VesselId));
LockSystem.LockStore.AddOrUpdateLock(new LockDefinition(LockType.UnloadedUpdate, lockDefinition.PlayerName, lockDefinition.VesselId));
```

**Issue:** When another player steals a lock, the current owner **directly modifies their LockStore** without sending a release message to the server. This creates client-server desync.

**Impact:** Ghost crafts may appear because the previous owner's client still thinks it has the lock and continues sending updates.

**Fix:** Send proper release/acquire messages through `LockMessageSender` instead of direct manipulation.

---

### Lock Bug #3: Client Removes Lock Before Server Confirmation

**File:** `LmpClient/Systems/Lock/LockSystem.cs`
**Lines:** 149-158

```csharp
private void ReleaseLock(LockDefinition lockDefinition)
{
    var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<LockReleaseMsgData>();
    msgData.Lock = lockDefinition;

    LockEvent.onLockRelease.Fire(lockDefinition);
    LockStore.RemoveLock(lockDefinition);  // Removed IMMEDIATELY

    MessageSender.SendMessage(msgData);  // Server may reject!
}
```

**Issue:** Client removes the lock from local `LockStore` **before** the server confirms the release. If the message is lost or server rejects it, the lock is permanently lost locally but still held on server.

**Impact:** Client desyncs from server state. Could lead to duplicate craft updates (client thinks no lock, server knows client has lock).

---

### Lock Bug #4: LockQuery.GetLock Returns Uninitialized Variable

**File:** `LmpCommon/Locks/LockQuery.cs`
**Lines:** 182-210

```csharp
public LockDefinition GetLock(LockType lockType, string playerName, Guid vesselId, string kerbalName)
{
    LockDefinition existingLock;  // Never assigned for Spectator!
    switch (lockType)
    {
        // ...
        case LockType.Spectator:
            LockStore.SpectatorLocks.TryGetValue(playerName, out existingLock);
            break;
        // ...
    }
    return existingLock;  // Could return uninitialized variable!
}
```

**Issue:** `existingLock` is declared but only assigned in some branches. For Spectator case it IS assigned, but the pattern is error-prone.

**Impact:** Low - currently all branches do assign, but fragile pattern.

---

### Lock Bug #5: Force Locks Bypass Normal Acquire Flow

**File:** `LmpClient/Systems/Lock/LockSystem.cs`
**Lines:** 53-56

```csharp
if (force && immediate)
{
    LockStore.AddOrUpdateLock(lockDefinition);  // Added locally WITHOUT server confirmation
}
```

**Issue:** When `force=true` and `immediate=true`, lock is added to local store without waiting for server approval. If server rejects the force, client has a lock it shouldn't have.

**Impact:** Can cause ghosting if client believes it has a lock it doesn't actually have on server.

---

## Lock System Priority

| Priority | Bug | Effort | Impact |
|----------|-----|--------|--------|
| 1 | #1 Kerbal Lock Check | Trivial | High - Kerbal locks broken |
| 2 | #2 Direct Store Manipulation | Medium | High - Server desync |
| 3 | #3 Premature Lock Removal | Medium | Medium - Client desync |
| 4 | #4 Uninitialized Variable | Low | Low - Fragile code |
| 5 | #5 Force Lock Bypass | Low | Medium - Ghosting |

---

## Lock Files to Examine

| File | Key Lines | Issue |
|------|-----------|-------|
| `LmpCommon/Locks/LockQuery.cs` | 171 | Wrong dictionary |
| `LmpClient/Systems/VesselLockSys/VesselLockEvents.cs` | 117-118, 124-125, 137-138 | Direct Store access |
| `LmpClient/Systems/Lock/LockSystem.cs` | 149-158, 53-56 | Premature removal, force bypass |

---

## Additional Critical Bugs

### Bug A: Missing GameEvents.onKerbalLevelUp.Remove()

**File:** `LmpClient/Systems/KerbalSys/KerbalSystem.cs`
**Lines:** 68 registered, 86-91 not removed

```csharp
// Line 68 - Registered:
GameEvents.onKerbalLevelUp.Add(KerbalEvents.KerbalLevelUp);

// Lines 86-91 - OnDisabled:
// NOTE: onKerbalLevelUp is NOT removed!
GameEvents.onKerbalStatusChange.Remove(KerbalEvents.StatusChange);
GameEvents.onKerbalTypeChange.Remove(KerbalEvents.TypeChange);
// onKerbalLevelUp missing!
```

**Severity:** High
**Impact:** If system is re-enabled, handler is added again causing duplicate invocations and memory leaks.

---

### Bug B: PlayerColorSystem - Missing GameEvents.onVesselCreate.Remove()

**File:** `LmpClient/Systems/PlayerColorSys/PlayerColorSystem.cs`
**Lines:** 32, 42

**Severity:** Medium
**Impact:** Event handler leak when system is disabled.

---

### Bug C: ShareProgressBaseSystem.QueueAction - No Null Check

**File:** `LmpClient/Systems/ShareProgress/ShareProgressBaseSystem.cs`
**Lines:** 86-90

```csharp
public void QueueAction(Action action)
{
    _actionQueue.Enqueue(action);
    RunQueue();  // No null check - NullReferenceException if action is null
}
```

**Severity:** Medium
**Impact:** Null action causes crash.

---

### Bug D: ShareCareerSystem - Queue Not Initialized When Disabled

**File:** `LmpClient/Systems/ShareCareer/ShareCareerSystem.cs`
**Lines:** 27-36, 60

```csharp
protected override void OnEnabled()
{
    if (SettingsSystem.ServerSettings.GameMode != GameMode.Career) return; // Early exit!
    base.OnEnabled();
    _actionQueue = new Queue<Action>();  // Never runs if not Career
}

private void RunQueue()
{
    while (_actionQueue.Count > 0 ...)  // NullReferenceException if _actionQueue is null!
```

**Severity:** Medium
**Impact:** Non-career mode causes NullReferenceException.

---

### Bug E: WarpSystem.PlayerIsInPastSubspace - KeyNotFoundException

**File:** `LmpClient/Systems/Warp/WarpSystem.cs`
**Lines:** 213-221

```csharp
return playerSubspace != CurrentSubspace && Subspaces[playerSubspace] < Subspaces[CurrentSubspace];
//                                  ^^^^^^^^^^^^^^^^ KeyNotFoundException if not in Subspaces!
```

**Severity:** Medium
**Impact:** Race condition can cause exception.

---

### Bug F: MessageBatcher.Batch - List Not Thread-Safe

**File:** `Server/System/MessageBatcher.cs`
**Lines:** 30-40, 73-113

```csharp
private class Batch
{
    public List<IServerMessageBase> Messages { get; } = new List<IServerMessageBase>(); // NOT thread-safe!
}

public void Enqueue(ClientStructure client, IServerMessageBase message)
{
    var batch = _clientBatches.GetOrAdd(client.Connection, _ => new Batch());
    lock (batch)
    {
        batch.Messages.Add(message);  // Multiple threads access same Batch
    }
}
```

**Severity:** High
**Impact:** Concurrent modification of List can cause corruption or exceptions.

---

### Bug G: VesselLoader - Null vesselRef.parts After Load Exception

**File:** `LmpClient/VesselUtilities/VesselLoader.cs`
**Lines:** 69-90

```csharp
catch (Exception loadEx)
{
    if (vesselProto.vesselRef != null)
    {
        foreach (var part in vesselProto.vesselRef.parts)  // parts could be null!
            Object.Destroy(part.gameObject);
    }
}
```

**Severity:** High
**Impact:** After Load exception, parts may be null causing NullReferenceException.

---

### Bug H: VesselRemoveSystem.KillVessel - Null Parts During Destruction

**File:** `LmpClient/Systems/VesselRemoveSys/VesselRemoveSystem.cs`
**Lines:** 134-141

```csharp
if (killVessel.parts != null)
{
    foreach (var part in killVessel.parts)
    {
        if (part != null) Object.Destroy(part.gameObject);  // part can become null during iteration!
    }
}
```

**Severity:** Medium
**Impact:** Unity may null out parts during destruction causing exceptions.

---

### Bug I: VesselStoreSystem.RemoveVessel - Unbounded Fire-and-Forget Task

**File:** `Server/System/VesselStoreSystem.cs`
**Lines:** 28-39

```csharp
public static void RemoveVessel(Guid vesselId)
{
    _ = Task.Run(() =>  // No cancellation mechanism
    {
        FileHandler.FileDelete(...);
    });
}
```

**Severity:** Low
**Impact:** No way to track or cancel the task if server shuts down.

---

### Bug J: WarpSystem.RemoveSubspace - Not Thread-Safe

**File:** `Server/System/WarpSystem.cs`
**Lines:** 34-51

**Severity:** Medium
**Impact:** Multiple message handlers could call concurrently causing race on Subspaces dictionary.

---

### Bug K: VesselCouple/Decouple/Undock Systems - Concurrent Modification

**Files:**
- `LmpClient/Systems/VesselCoupleSys/VesselCoupleSystem.cs`
- `LmpClient/Systems/VesselDecoupleSys/VesselDecoupleSystem.cs`
- `LmpClient/Systems/VesselUndockSys/VesselUndockSystem.cs`
- `LmpClient/Systems/VesselUpdateSys/VesselUpdateSystem.cs`

**Severity:** Low
**Impact:** Nested while loops modifying inner queues during foreach could theoretically cause issues.

---

### Bug L: ScenarioSystem - Null ScenarioModule Not Checked

**File:** `LmpClient/Systems/Scenario/ScenarioSystem.cs`
**Lines:** 167-180

**Severity:** Medium
**Impact:** Null ScenarioModule used in logging without null check.

---

### Bug M: TimingManager Handlers Not Tracked for Disposal

**File:** `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs`
**Lines:** 67, 70, 84-85

```csharp
protected override void OnEnabled()
{
    TimingManager.FixedUpdateAdd(HandlePositionsStage, HandleVesselUpdates);  // No check if already added
    TimingManager.LateUpdateAdd(SendPositionsStage, SendVesselPositionUpdates);
}

protected override void OnDisabled()
{
    TimingManager.FixedUpdateRemove(HandlePositionsStage, HandleVesselUpdates);  // Remove once
```

**Severity:** Medium
**Impact:** If system is enabled/disabled multiple times, handlers could be added multiple times.

---

### Bug N: ServerContext.Clients Access in MessageBatcher.FlushAll

**File:** `Server/System/MessageBatcher.cs`
**Lines:** 58-71

```csharp
public void FlushAll()
{
    foreach (var kvp in _clientBatches)
    {
        if (batch.Messages.Count > 0 && ServerContext.Clients.TryGetValue(kvp.Key.RemoteEndPoint, out var client))
        //                    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ Could fail if client disconnects
```

**Severity:** Medium
**Impact:** Client disconnect during enumeration could cause issues.

---

### Bug O: VesselLoader.LoadVesselIntoGame - NaN Orbit After Load

**File:** `LmpClient/VesselUtilities/VesselLoader.cs`
**Lines:** 134-138

```csharp
if (double.IsNaN(vesselProto.vesselRef.orbitDriver.pos.x))
{
    return false;  // But vessel already added to FlightGlobals!
}
```

**Severity:** Low
**Impact:** Invalid vessel state left in FlightGlobals.

---

## Additional Bugs Priority

| Priority | Bug | Severity | Impact |
|----------|-----|----------|--------|
| 1 | F - MessageBatcher thread safety | High | Server crashes/corruption |
| 2 | A - KerbalLevelUp event leak | High | Memory leak, duplicate events |
| 3 | G - VesselLoader null parts | High | Crash on load failure |
| 4 | C - ShareProgress null action | Medium | Crash |
| 5 | D - ShareCareer null queue | Medium | Crash in non-career |
| 6 | E - WarpSystem KeyNotFound | Medium | Crash |
| 7 | H - KillVessel null parts | Medium | Crash during destroy |
| 8 | J - WarpSystem RemoveSubspace | Medium | Race condition |
| 9 | L - ScenarioSystem null module | Medium | Crash |
| 10 | M - TimingManager duplicate handlers | Medium | Memory leak |
| 11 | N - Clients access race | Medium | Crash |
| 12 | B - PlayerColor event leak | Medium | Memory leak |
| 13 | K - Concurrent modification | Low | Theoretical issue |
| 14 | I - Fire-and-forget task | Low | No cleanup |
| 15 | O - NaN orbit after load | Low | Invalid state |

---

## Explosion Handling & Server Lag

### Current Issues

1. **No explosion-specific throttling** - When a vessel explodes, it generates many part/damage events that all get sent to clients
2. **No per-vessel update rate limiting** - Large vessels with many parts can overwhelm clients
3. **No message queue limits** - MessageBatcher has no max queue size, only batch size
4. **VesselFlightState written on every update** - Disk I/O during explosions causes lag

### Suggested Fixes

#### 1. Explosion Detection & Response

```csharp
// In VesselMsgReader or dedicated explosion detector
public static class ExplosionDetector
{
    private static readonly HashSet<Guid> _recentlyExploded = new HashSet<Guid>();
    private static readonly TimeSpan ExplosionCooldown = TimeSpan.FromSeconds(5);

    public static void OnVesselExplode(Guid vesselId)
    {
        _recentlyExploded.Add(vesselId);
        // Throttle updates from this vessel
    }

    public static bool IsExplosionCoolingDown(Guid vesselId)
    {
        return _recentlyExploded.Contains(vesselId);
    }
}
```

#### 2. Per-Vessel Update Throttling

Add to `ServerSettings`:
```csharp
public int MaxUpdatesPerVesselPerSecond { get; set; } = 30;
public int MaxPositionUpdatesPerSecond { get; set; } = 60;
public int MaxFlightStateUpdatesPerSecond { get; set; } = 10;
```

#### 3. MessageBatcher Queue Limits

```csharp
private const int MaxQueueSize = 500; // Add this

public void Enqueue(...)
{
    var batch = _clientBatches.GetOrAdd(client.Connection, _ => new Batch());
    lock (batch)
    {
        if (batch.Messages.Count >= MaxQueueSize)
            return; // Drop message if queue full
        // ...
    }
}
```

#### 4. Throttle FlightState Writes During Explosions

```csharp
// In VesselFlightStateDataUpdater
private static readonly Dictionary<Guid, DateTime> _lastWriteTime = new Dictionary<Guid, DateTime>();
private static readonly TimeSpan MinWriteInterval = TimeSpan.FromMilliseconds(100);

public static void ThrottledWriteFlightstateData(...)
{
    if (_lastWriteTime.TryGetValue(vesselId, out var last) && 
        DateTime.UtcNow - last < MinWriteInterval)
        return;

    _lastWriteTime[vesselId] = DateTime.UtcNow;
    WriteFlightstateDataToFile(...);
}
```

#### 5. Debounce Position Updates During High-Frequency Events

```csharp
// In VesselMsgReader position handler
private static readonly Dictionary<Guid, DateTime> _lastPositionTime = new Dictionary<Guid, DateTime>();

if (posMessage)
{
    var vesselId = posData.VesselId;
    if (_lastPositionTime.TryGetValue(vesselId, out var last) &&
        DateTime.UtcNow - last < TimeSpan.FromMilliseconds(10)) // 100fps max
        return; // Skip if too soon

    _lastPositionTime[vesselId] = DateTime.UtcNow;
    // Process normally
}
```

#### 6. Disable Spatial Grid Updates During Explosions

```csharp
// When vessel enters explosion state, pause its spatial grid updates
// until explosion animation completes
```

---

## Client-Side Data Shaping Helpers

These optimizations can be implemented on the client to reduce server lag and improve network performance without changing the wire protocol.

### 1. Distance-Based Update Throttling

**Concept:** Send updates less frequently for distant vessels.

```csharp
// In VesselPositionSystem - adaptive intervals based on distance
private int GetAdaptiveUpdateInterval(Vessel vessel)
{
    var distance = VesselCommon.GetDistanceToNearestPlayer(vessel);
    if (distance > 10000) return 500;  // Far - very slow
    if (distance > 1000) return 200;   // Medium
    if (distance > 100) return 100;    // Close
    return SettingsSystem.ServerSettings.VesselUpdatesMsInterval; // Nearby - full rate
}
```

**Files to modify:** `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs`

---

### 2. Vessel Importance Scoring

**Concept:** Prioritize updates for "important" vessels.

```csharp
public enum VesselImportance
{
    Irrelevant = 0,    // Don't send updates
    Low = 1,          // Send every 5s
    Medium = 2,       // Send every 1s  
    High = 3,         // Send at normal rate
    Critical = 4       // Always send, highest priority
}

public static VesselImportance CalculateImportance(Vessel vessel)
{
    if (vessel == FlightGlobals.ActiveVessel) return VesselImportance.Critical;
    if (vessel.vesselType == VesselType.Eva) return VesselImportance.High;
    if (LockSystem.LockQuery.UpdateLockBelongsToPlayer(vessel.id, SettingsSystem.CurrentSettings.PlayerName))
        return VesselImportance.High;
    if (VesselCommon.GetDistanceToNearestPlayer(vessel) < 500) return VesselImportance.Medium;
    return VesselImportance.Low;
}
```

---

### 3. Explosion State Detection & Throttling

**Concept:** Detect when a vessel is exploding and throttle updates during/after.

```csharp
public static class ExplosionStateDetector
{
    private static readonly Dictionary<Guid, ExplosionState> _vesselStates = new Dictionary<Guid, ExplosionState>();
    
    public class ExplosionState
    {
        public bool IsExploding { get; set; }
        public DateTime ExplosionStartTime { get; set; }
        public int PartCountBeforeExplosion { get; set; }
        public float DamageLevel { get; set; }
    }
    
    public static void OnVesselDamageReceived(Guid vesselId, float damage)
    {
        if (!_vesselStates.TryGetValue(vesselId, out var state))
        {
            state = new ExplosionState();
            _vesselStates[vesselId] = state;
        }
        
        state.DamageLevel += damage;
        
        if (state.DamageLevel > 50 && !state.IsExploding)
        {
            state.IsExploding = true;
            state.ExplosionStartTime = DateTime.UtcNow;
            state.PartCountBeforeExplosion = FlightGlobals.FindVessel(vesselId)?.parts?.Count ?? 0;
        }
    }
    
    public static bool ShouldThrottle(Guid vesselId)
    {
        if (!_vesselStates.TryGetValue(vesselId, out var state))
            return false;
            
        if (state.IsExploding && (DateTime.UtcNow - state.ExplosionStartTime).TotalSeconds < 10)
            return true;
            
        if (state.IsExploding && (DateTime.UtcNow - state.ExplosionStartTime).TotalSeconds > 10)
        {
            state.IsExploding = false;
            state.DamageLevel = 0;
        }
        
        return false;
    }
}
```

**Files to modify:** New file in `LmpClient/Systems/VesselPositionSys/`

---

### 4. Part Change Delta Compression

**Concept:** Only send part changes instead of full part states.

```csharp
public class PartStateTracker
{
    private readonly Dictionary<Guid, PartState> _lastKnownState = new Dictionary<Guid, PartState>();
    
    public class PartState
    {
        public float Temperature { get; set; }
        public float Resources { get; set; }
        public bool IsActive { get; set; }
        public bool IsDead { get; set; }
    }
    
    public List<PartStateDelta> GetChangedParts(Vessel vessel, float tolerance = 0.01f)
    {
        var changes = new List<PartStateDelta>();
        
        foreach (var part in vessel.parts)
        {
            if (!_lastKnownState.TryGetValue(part.flightID, out var last))
            {
                changes.Add(new PartStateDelta { PartId = part.flightID, IsNew = true });
                continue;
            }
            
            var current = CaptureState(part);
            
            if (Math.Abs(current.Temperature - last.Temperature) > tolerance * last.Temperature ||
                Math.Abs(current.Resources - last.Resources) > tolerance * last.Resources ||
                current.IsActive != last.IsActive ||
                current.IsDead != last.IsDead)
            {
                changes.Add(new PartStateDelta { PartId = part.flightID, Changes = ComputeDelta(last, current) });
            }
        }
        
        return changes;
    }
}
```

---

### 5. Resource Update Batching

**Concept:** Batch resource updates instead of sending per-part.

```csharp
public class ResourceUpdateBatcher
{
    private readonly Dictionary<Guid, Dictionary<string, float>> _pendingResources = new Dictionary<Guid, Dictionary<string, float>>();
    private DateTime _lastFlush = DateTime.UtcNow;
    private readonly TimeSpan _batchWindow = TimeSpan.FromMilliseconds(100);
    
    public void QueueResourceUpdate(Guid vesselId, string resourceName, float amount)
    {
        if (!_pendingResources.TryGetValue(vesselId, out var resources))
        {
            resources = new Dictionary<string, float>();
            _pendingResources[vesselId] = resources;
        }
        
        resources[resourceName] = amount;
        
        if (DateTime.UtcNow - _lastFlush > _batchWindow)
            Flush();
    }
    
    public void Flush()
    {
        foreach (var kvp in _pendingResources)
        {
            // Send batched resource update for vessel kvp.Key with resources kvp.Value
        }
        _pendingResources.Clear();
        _lastFlush = DateTime.UtcNow;
    }
}
```

---

### 6. Position Update Coalescing

**Concept:** Combine multiple rapid position updates into one.

```csharp
public class PositionUpdateCoalescer
{
    private static readonly Dictionary<Guid, CoalescedUpdate> _pendingUpdates = new Dictionary<Guid, CoalescedUpdate>();
    private static readonly TimeSpan _coalesceWindow = TimeSpan.FromMilliseconds(5);
    
    public class CoalescedUpdate
    {
        public VesselPositionMsgData Latest { get; set; }
        public List<VesselPositionMsgData> Buffer { get; } = new List<VesselPositionMsgData>();
        public DateTime FirstReceived { get; set; }
    }
    
    public static void QueuePositionUpdate(Vessel vessel, VesselPositionMsgData update)
    {
        if (!_pendingUpdates.TryGetValue(vessel.id, out var coalesced))
        {
            coalesced = new CoalescedUpdate { Latest = update, FirstReceived = DateTime.UtcNow };
            _pendingUpdates[vessel.id] = coalesced;
        }
        else
        {
            coalesced.Latest = update;
            coalesced.Buffer.Add(update);
        }
        
        if (DateTime.UtcNow - coalesced.FirstReceived > _coalesceWindow)
            SendCoalescedUpdate(vessel.id);
    }
}
```

---

### 7. Vessel State Change Filter

**Concept:** Only send vessel updates when state actually changes.

```csharp
public class VesselStateFilter
{
    private readonly Dictionary<Guid, CachedState> _lastSent = new Dictionary<Guid, CachedState>();
    
    public class CachedState
    {
        public string Situation { get; set; }
        public bool Landed { get; set; }
        public bool Splashed { get; set; }
        public int Stage { get; set; }
        public string LandedAt { get; set; }
    }
    
    public static bool ShouldSendVesselUpdate(Vessel vessel)
    {
        var current = new CachedState
        {
            Situation = vessel.situation.ToString(),
            Landed = vessel.Landed,
            Splashed = vessel.Splashed,
            Stage = vessel.currentStage,
            LandedAt = vessel.landedAt
        };
        
        if (!_lastSent.TryGetValue(vessel.id, out var last))
        {
            _lastSent[vessel.id] = current;
            return true;
        }
        
        if (current.Situation != last.Situation ||
            current.Landed != last.Landed ||
            current.Splashed != last.Splashed ||
            current.Stage != last.Stage ||
            current.LandedAt != last.LandedAt)
        {
            _lastSent[vessel.id] = current;
            return true;
        }
        
        return false;
    }
}
```

---

### 8. Priority-Based Message Queue

**Concept:** Prioritize messages on client before sending.

```csharp
public enum MessagePriority
{
    Critical = 0,   // Position updates for active vessel
    High = 1,       // Flight state updates
    Normal = 2,     // Vessel updates
    Low = 3         // Secondary vessel updates
}

public class PriorityMessageQueue
{
    private readonly Dictionary<MessagePriority, Queue<IMessageData>> _queues = new Dictionary<MessagePriority, Queue<IMessageData>>();
    private readonly int _maxQueueSize = 100;
    
    public void Enqueue(IMessageData message, MessagePriority priority)
    {
        var queue = GetOrCreateQueue(priority);
        
        if (_queues[MessagePriority.Low].Count > _maxQueueSize)
            _queues[MessagePriority.Low].Dequeue();
            
        queue.Enqueue(message);
    }
}
```

---

### 9. Bandwidth Adaptive Quality

**Concept:** Automatically adjust quality based on network conditions.

```csharp
public class BandwidthAdaptiveSystem
{
    private readonly MovingAverage _bandwidthHistory = new MovingAverage(20);
    private readonly MovingAverage _latencyHistory = new MovingAverage(10);
    
    public enum QualityLevel
    {
        Ultra = 0,    // 30ms updates, full data
        High = 1,     // 50ms updates
        Medium = 2,   // 100ms updates
        Low = 3,      // 200ms updates
        Minimal = 4   // 500ms updates
    }
    
    public QualityLevel CurrentQuality { get; private set; } = QualityLevel.Ultra;
    
    public void UpdateBandwidthStats(float bytesPerSecond, float latencyMs)
    {
        _bandwidthHistory.Add(bytesPerSecond);
        _latencyHistory.Add(latencyMs);
        
        var avgBandwidth = _bandwidthHistory.Average;
        var avgLatency = _latencyHistory.Average;
        
        if (avgLatency > 500 || avgBandwidth < 10000)
            CurrentQuality = QualityLevel.Minimal;
        else if (avgLatency > 200 || avgBandwidth < 50000)
            CurrentQuality = QualityLevel.Low;
        else if (avgLatency > 100 || avgBandwidth < 100000)
            CurrentQuality = QualityLevel.Medium;
        else if (avgLatency > 50 || avgBandwidth < 200000)
            CurrentQuality = QualityLevel.High;
        else
            CurrentQuality = QualityLevel.Ultra;
    }
    
    public int GetAdaptiveUpdateInterval() => CurrentQuality switch
    {
        QualityLevel.Ultra => 30,
        QualityLevel.High => 50,
        QualityLevel.Medium => 100,
        QualityLevel.Low => 200,
        QualityLevel.Minimal => 500,
        _ => 30
    };
}
```

---

### 10. Server Settings to Add

**Files to modify:** `Server/Settings/Definition/IntervalSettingsDefinition.cs`

```csharp
[XmlComment(Value = "Maximum vessels to send position updates for simultaneously (0 = unlimited)")]
public int MaxSimultaneousPositionUpdates { get; set; } = 10;

[XmlComment(Value = "Minimum time between flight state updates in ms")]
public int MinFlightStateUpdateIntervalMs { get; set; } = 50;

[XmlComment(Value = "Enable adaptive quality based on network conditions")]
public bool EnableAdaptiveQuality { get; set; } = true;

[XmlComment(Value = "Maximum message queue size per client in messages")]
public int MaxClientMessageQueueSize { get; set; } = 500;
```

---

### Implementation Priority

| Optimization | Impact | Effort |
|--------------|--------|--------|
| Explosion detection & throttling | **High** - explosions cause most lag | Medium |
| Distance-based update intervals | **High** - reduces unnecessary updates | Low |
| Vessel importance scoring | **High** - prioritize active vessels | Medium |
| Part change delta compression | Medium - reduces part update size | High |
| Resource update batching | Medium - reduces resource message count | Medium |
| Position coalescing | Medium - reduces position spam | Low |
| Vessel state change filter | Low - saves few bytes | Low |
| Bandwidth-adaptive quality | Medium - automatic adjustment | Medium |

---

## Complete File Reference

| File | Key Lines | Status |
|------|-----------|--------|
| `Server/System/MessageBatcher.cs` | 19-21, 30-40, 58-71 | HIGH - Add queue limits, fix thread safety |
| `Server/System/VesselStoreSystem.cs` | 28-39 | Add cancellation token |
| `LmpClient/Systems/KerbalSys/KerbalSystem.cs` | 68, 86-91 | Register/unregister match |
| `LmpClient/VesselUtilities/VesselLoader.cs` | 69-90, 134-138 | Null checks |
| `LmpClient/Systems/VesselRemoveSys/VesselRemoveSystem.cs` | 134-141 | Safe iteration |
| `LmpClient/Systems/ShareProgress/ShareProgressBaseSystem.cs` | 86-90 | Null check |
| `LmpClient/Systems/ShareCareer/ShareCareerSystem.cs` | 27-36, 60 | Initialize queue properly |
| `LmpClient/Systems/Warp/WarpSystem.cs` | 213-221, 34-51 | Thread safety |
| `LmpClient/Systems/PlayerColorSys/PlayerColorSystem.cs` | 32, 42 | Add remove call |
| `LmpClient/Systems/Scenario/ScenarioSystem.cs` | 167-180 | Null check |
| `LmpClient/Systems/VesselPositionSys/VesselPositionSystem.cs` | 67, 70, 84-85 | Track handler state |
