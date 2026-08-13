# Refactoring Inventory

## Classification Rule

Every tracked production C# file under `OpenRS` is classified by this document:
- files listed under **Refactored** contain a verified structural change in the current programme;
- files listed under **Focused Collaborators Added** were introduced to own an extracted responsibility;
- every other tracked production C# file is **Intentionally Retained** because it is already cohesive, preserves a public compatibility surface, contains incomplete legacy behaviour, or lacks direct unit coverage for a safe structural change;
- files under `bin` and `obj` are generated and excluded from source classification.

This set-based rule classifies the complete production tree without relying on a stale numeric file count.

## Refactored

| File | Result | Verification |
| --- | --- | --- |
| `GameLogic/GameManagers/InventoryManager.cs` | Centralised slot creation and value copying; separated server-bank mutation and visible-bank coordination | `InventoryManagerTests` |
| `Models/Appearance.cs` | Named fixed sprite protocol values | `AppearanceTests` |
| `Models/MobInstance.cs` | Shared immutable movement-sprite table | `MobInstanceTests` |
| `Net/Client/Game/Cameras/Camera.cs` | Centralised projection-bound expansion | `CameraStateTests` |
| `Net/Client/Game/Cameras/CameraDepthSorter.cs` | Separated depth partitioning from recursion | `CameraDepthSorterTests` |
| `Net/Client/Game/ChatMessage.cs` | Reduced to compatibility facade | `ChatMessageTests` |
| `Net/Client/Game/EngineHandle.cs` | Centralised grid guards, sector indexing, and seam policy | `EngineHandleTileTests` |
| `Net/Client/Game/GameImage.cs` | Reduced to bounded drawing facade | `GameImageDrawingTests`, `GameImageColourTests` |
| `Net/Client/Game/GameObject.cs` | Delegated allocation, geometry mutation, and shade decoding | `GameObjectTests` |
| `Net/Client/Game/GameObjectDataLoader.cs` | Removed misplaced optional-array allocation | `GameObjectTests` |
| `Net/Client/Game/SectorCoordinates.cs` | Coordinates now own local tile indexing | `EngineHandleTileTests` |
| `Net/Client/Menu.cs` | Decomposed keyboard editing and centralised hit-testing | `MenuTests` |
| `Net/Client/Net/PacketConstruction.cs` | Decomposed framing, read state, metrics, and deferred errors | Packet construction unit suites |
| `Net/Client/Utilities/GameClientUtilities.cs` | Delegated pure item-count formatting | `GameClientUtilitiesTests` |

## Focused Collaborators Added

| File | Responsibility |
| --- | --- |
| `Net/Client/Game/ChatMessageCodec.cs` | Chat nibble encoding, decoding, and normalisation |
| `Net/Client/Game/GameImageBlurProcessor.cs` | Area blur |
| `Net/Client/Game/GameImagePixelRasteriser.cs` | Basic pixel rasterisation and drawing-area state |
| `Net/Client/Game/GameImageShapeRasteriser.cs` | Circle, alpha-rectangle, and gradient rasterisation |
| `Net/Client/Game/GameObjectArrayInitialiser.cs` | Required and optional geometry-array allocation |
| `Net/Client/Game/GameObjectGeometryBuilder.cs` | Vertex and face mutation |
| `Net/Client/Game/GameObjectShadeDecoder.cs` | Stateful legacy shade decoding |
| `Net/Client/Net/PacketFraming.cs` | Compact and extended packet-length framing |
| `Net/Client/Utilities/ItemCountFormatter.cs` | Pure inventory quantity display formatting |

## Intentionally Retained Categories

- Packet and model enumerations: immutable numeric compatibility contracts.
- `LoginEncryptor`: restored unchanged after its complete focused suite confirmed the current RSA contract.
- `PathHandler`: collision lookup is intentionally incomplete and movement calculation lacks direct isolation.
- Incoming packet handlers: numeric identifiers are covered, but state mutation branches lack direct unit coverage.
- Input handlers beyond `Menu`: frame ordering and packet side effects lack direct unit coverage.
- Loaders beyond tested sector and model primitives: content and graphics dependencies lack direct unit coverage.
- Renderers beyond unit-tested image and camera primitives: visual workflows require manual verification.
- `GameClient` and lifecycle classes: broad public mutable state and ordered side effects remain compatibility surfaces.
- Data repositories and mappings: retained until isolated unit coverage proves serialisation, ordering, defaults, and errors.

## Verification Baseline

The accepted automated baseline is:
- command: `dotnet test OpenRS.slnx`;
- discovered unit tests: 1,095;
- failures: 0;
- skipped: 0.

The warning emitted by the null-camera-model test is expected diagnostic output from an explicitly tested branch.