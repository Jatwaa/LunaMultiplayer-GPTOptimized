# Luna Multiplayer Mod (LMP)

## What is it?
Luna Multiplayer (LMP) is a comprehensive multiplayer mod for **Kerbal Space Program (KSP)**. It allows players to interact in a shared universe, synchronizing vessel positions, movements, and game states in real-time.

## How it Works

### Network Architecture
- **UDP-Based Communication**: Uses the `Lidgren` library for reliable UDP message handling, ensuring a balance between speed and reliability.
- **Master Server System**: Employs a master server architecture for server discovery, allowing players to find and connect to available game servers without knowing the IP addresses manually.
- **NAT Punchthrough & UPnP**: Includes features to bypass router restrictions, making it easier for users to host servers without complex port forwarding.

### Core Synchronisation Mechanisms
- **Time Synchronization (NTP)**: Uses the Network Time Protocol to ensure all clients and the server are synchronized to the same clock, which is critical for physics consistency.
- **Interpolation**: Implements entity interpolation to smooth out vessel movements, preventing "jumping" or stuttering caused by network latency or packet loss.
- **Multi-threaded Design**: Utilizes a task-based approach (rather than raw threads) to handle network processing and game state updates efficiently.

### Game Integration
- **Career & Science Mode Support**: Synchronizes funds, science points, and strategies across all players.
- **Data Compression**: Uses `QuickLZ` for fast, garbage-free compression of network messages to reduce bandwidth usage.
- **Message Caching**: Caches network messages to minimize Garbage Collector (GC) spikes, maintaining a smoother frame rate during gameplay.

## Component Breakdown
- **LmpClient**: The mod integrated into the KSP game client.
- **Server**: The standalone server application that manages the game state.
- **LmpMasterServer**: The server that tracks and lists active game servers.
- **LmpCommon**: Shared logic, data structures, and network message definitions used by both client and server.
