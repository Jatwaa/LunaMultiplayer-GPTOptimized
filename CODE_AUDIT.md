# Code Audit & Scale-Up Roadmap - LunaMultiplayer

Following a detailed review of the codebase and an analysis of the network architecture, this document outlines critical fixes and a roadmap for scaling the mod to support "MMO-level" co-op play.

## 🔴 High Impact / Critical Fixes
*Immediate priority to fix stability, security, and memory leaks.*

| Issue | File/Location | Impact | Expected Result |
| :--- | :--- | :--- | :--- |
| **UDP Amplification** | `LmpMasterServer/Lidgren/MasterServer.cs` | High | Prevents the Master Server from being used in DDoS attacks; reduces network overhead. |
| **Memory Leak in Pooling** | `Server/Server/MessageReceiver.cs` | High | Eliminates inconsistent GC spikes and memory growth, stabilizing server FPS. |
| **Area of Interest (AoI)** | New Architecture | High | **MMO Scale:** Drastically reduces bandwidth by only syncing vessels near the player. |
| **Predicted Movement** | Client/Server Logic | High | **MMO Scale:** Removes visual "stutter" and input lag, making co-op feel local. |

---

## 🟡 Medium Impact / Performance Improvements
*Significant gains in efficiency and reliability.*

| Issue | File/Location | Impact | Expected Result |
| :--- | :--- | :--- | :--- |
| **Delta Compression** | `LmpCommon` messages | Medium | **MMO Scale:** Reduces packet size by only sending changed data; supports more simultaneous vessels. |
| **Server List Allocations** | `LmpMasterServer/Lidgren/MasterServer.cs` | Medium | Reduces short-lived object allocations, lowering overall GC pressure on the Master Server. |
| **Process Termination** | `LmpMasterServer/Lidgren/MasterServer.cs` | Medium | Stops the server from crashing due to non-critical background task failures. |
| **Quantization** | Network Serialization | Medium | **MMO Scale:** Lowers bandwidth by compressing precision of rotation/position data. |

---

## 🔵 Low Impact / Optimization & Quality
*Code hygiene and incremental performance gains.*

| Issue | File/Location | Impact | Expected Result |
| :--- | :--- | :--- | :--- |
| **Reflection-based Instantiation**| `LmpCommon/Message/Base/MessageStore.cs`| Low | Slightly faster message creation via compiled expression factories. |
| **String Interpolation** | Project-wide | Low | Better readability and marginal performance gain in logging. |
| **Adaptive Tick Rates** | Network Loop | Low | **MMO Scale:** Saves CPU/Bandwidth by updating stationary vessels less frequently. |
| **Binary Schema Optimization**| `LmpCommon` | Low | Reduces serialization overhead by using fixed-length binary formats. |
