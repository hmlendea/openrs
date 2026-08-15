# State Ownership

## Ownership Rules

Each mutable state category has one authoritative owner. Other components may query it or request an operation, but they must not become alternative mutators.

| State Category | Current Authoritative Owner | Permitted Mutation Paths | Principal Consumers |
| --- | --- | --- | --- |
| Process and window lifecycle | `GameWindow` | MonoGame lifecycle methods | Screens and host services |
| Screen transitions and control lifecycle | NuciXNA `ScreenManager` and registered screens | Screen and GUI manager APIs | Host and presentation |
| Connection and authentication | `GameAppletMiddleMan` | Login, reconnect, logout, and transport flow | Login presentation and `GameClient` |
| Packet framing and buffering | `PacketConstruction`, `PacketConstructionReader`, `PacketConstructionWriter`, and `StreamClass` | Protocol facade methods only | Connection and action handlers |
| Incoming packet dispatch | `PacketHandler` and specialised handlers | `HandlePacket` entry points | Client session state |
| Public client compatibility state | `GameClient` | Existing handlers, input operations, and lifecycle methods | Renderers, GUI, and public callers |
| Inventory and bank slots | `InventoryManager` | Inventory and bank manager methods | Combat, menus, and presentation |
| Combat rune requirements | `CombatManager` | Combat manager operations | Input and combat presentation |
| Entity repositories | `EntityManager` | Manager loading and query operations | Gameplay and loading services |
| Mob movement, prayer, and combat flags | `MobInstance` | Mob methods and `PathHandler` | Gameplay and rendering |
| Legacy path progression | `PathHandler` | `SetPath`, `ResetPath`, and `UpdateLocation` | `MobInstance` |
| World sectors and tiles | `EngineHandle` and its focused world collaborators | Loading and world mutation operations | Interaction and rendering |
| Camera scene objects | `CameraSceneObjectTracker` | Camera scene facade methods | Camera rendering |
| Camera mouse-hit frame state | `CameraHitTracker` through `CameraSceneObjectTracker` | Camera scene facade methods | Camera rendering and input |
| Camera model cache and projection state | `Camera` | Existing camera facade methods | Rendering |
| Raw image pixel state | `GameImage` | Focused viewport, screen-buffer, primitive, shape, blur, character, picture, text, and minimap collaborators | GUI and renderers |
| Game-object geometry arrays | `GameObject` | `GameObjectArrayInitialiser` and `GameObjectGeometryBuilder` | Loading, camera, and rendering |
| Chat codec buffers | `ChatMessage` and `ChatMessageDecoder` | `ChatMessageCodec` through public facade methods | Chat input and packet handling |
| Runtime data entities | Repositories under `DataAccess/Repositories` | Repository operations | Managers and mappings |
| Localised text | `LocalisationManager` | Localisation loading APIs | Host, GUI, and gameplay messages |
| Audio playback | `AudioManager` | Audio manager methods | Presentation and packet handlers |
| Configuration | Types under `Settings` | Startup and settings APIs | Host and infrastructure |

## Transitional State

`GameClient` remains a large compatibility surface because numerous public fields and ordered side effects are observable. Extracted code must not accept `GameClient` merely for convenience. Further state extraction requires direct unit coverage for every affected reader, writer, alias, event, and null state.

`PathHandler` collision access remains intentionally incomplete and unchanged. Movement calculation beyond the currently unit-tested state transitions must not be refactored until isolated unit coverage exists.