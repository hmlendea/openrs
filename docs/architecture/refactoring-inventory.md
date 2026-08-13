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
| `Models/Appearance.cs` | Named fixed sprite protocol values and centralised colour-range validation | `AppearanceTests` |
| `Models/MobInstance.cs` | Shared immutable movement-sprite table | `MobInstanceTests` |
| `Net/Client/Game/ClientMob.cs` | Named protocol capacities and unset combat-level state | `ClientMobTests` |
| `Net/Client/Game/Cameras/Camera.cs` | Centralised projection-bound expansion | `CameraStateTests` |
| `Net/Client/Game/Cameras/CameraDepthSorter.cs` | Separated depth partitioning, render-state initialisation, and dependency resolution coordination | `CameraDepthSorterTests` |
| `Net/Client/Game/Cameras/CameraSceneObjectTracker.cs` | Centralised per-scene-object vertex ownership and separated metadata from geometry creation | `CameraSceneObjectTrackerTests`, `CameraStateTests` |
| `Net/Client/Game/ChatMessage.cs` | Reduced to compatibility facade | `ChatMessageTests` |
| `Net/Client/Game/EngineHandle.cs` | Centralised grid and sector allocation, grid guards, sector indexing, and seam policy | `EngineHandleTileTests`, `EngineHandlePathfindingTests` |
| `Net/Client/Game/GameImage.cs` | Reduced to bounded drawing facade with delegated storage and colour packing | `GameImageDrawingTests`, `GameImageColourTests` |
| `Net/Client/Game/GameImageCharacterRenderer.cs` | Centralised shear-aware layout and reused scaled-entity clipping across character paths | `GameImageCharacterTests` |
| `Net/Client/Game/GameImagePictureManager.cs` | Reduced to picture lifecycle coordination with delegated decoding, palette conversion, and capture | `GameImagePictureTests` |
| `Net/Client/Game/GameImageScaledSpriteBlitter.cs` | Centralised primary/secondary tint classification and horizontal scanline clipping | `GameImageCharacterTests` |
| `Net/Client/Game/GameImageSpriteBlitter.cs` | Centralised identical direct, indexed, and flipped sprite blend arithmetic | `GameImageSpriteTests`, `GameImageCharacterTests` |
| `Net/Client/Game/GameImageSpriteRenderer.cs` | Centralised picture clipping and extracted scaled-entity clipping state | `GameImageSpriteTests` |
| `Net/Client/Game/GameObject.cs` | Delegated allocation, geometry mutation, and shade decoding | `GameObjectTests` |
| `Net/Client/Game/GameObjectDataLoader.cs` | Removed misplaced optional-array allocation | `GameObjectTests` |
| `Net/Client/Game/ObjectModel.cs` | Centralised copied vertex and face collection semantics | `ObjectModelTests` |
| `Net/Client/Game/PathFinder.cs` | Centralised cardinal and diagonal neighbour expansion rules | `EngineHandlePathfindingTests` |
| `Net/Client/Game/RscSector.cs` | Centralised coordinate indexing and serialised tile sizing | `RscSectorTests`, `SectorTileTests` |
| `Net/Client/Game/SectorTile.cs` | Owns its serialised byte-count invariant | `SectorTileTests`, `RscSectorTests` |
| `Net/Client/Game/SectorCoordinates.cs` | Coordinates now own local tile indexing | `EngineHandleTileTests` |
| `Net/Client/Link.cs` | Uses stream position as the remaining-byte conversion cursor | `LinkTests` |
| `Net/Client/Menu.cs` | Decomposed keyboard editing, hit-testing, and component construction; replaced private numeric variants with `MenuComponentType` | `MenuTests` |
| `Net/Client/Net/PacketConstruction.cs` | Decomposed framing, read state, metrics, and deferred errors | Packet construction unit suites |
| `Net/Client/Utilities/GameClientUtilities.cs` | Delegated pure item-count formatting | `GameClientUtilitiesTests` |

## Focused Collaborators Added

| File | Responsibility |
| --- | --- |
| `Net/Client/Game/ChatMessageCodec.cs` | Chat nibble encoding, decoding, and normalisation |
| `Net/Client/Game/GameImageBlurProcessor.cs` | Area blur |
| `Net/Client/Game/GameImageCharacterClip.cs` | Allocation-free flipped and sheared character layout state |
| `Net/Client/Game/GameImageColourTint.cs` | Allocation-free primary and secondary character tint classification |
| `Net/Client/Game/GameImageColourPacker.cs` | Pure RGB/RGBA packing and channel clamping |
| `Net/Client/Game/GameImageEntityClip.cs` | Allocation-free scaled-entity clipping and source sampling state |
| `Net/Client/Game/GameImagePaletteConverter.cs` | Direct and indexed picture palette conversion |
| `Net/Client/Game/GameImagePictureCapture.cs` | Row-major and column-major screen capture |
| `Net/Client/Game/GameImagePictureDataDecoder.cs` | Picture metadata, palette, and pixel-index decoding |
| `Net/Client/Game/GameImagePixelRasteriser.cs` | Basic pixel rasterisation and drawing-area state |
| `Net/Client/Game/GameImageShapeRasteriser.cs` | Circle, alpha-rectangle, and gradient rasterisation |
| `Net/Client/Game/GameImageScaledScanline.cs` | Allocation-free horizontal clipping for scaled sprite scanlines |
| `Net/Client/Game/GameImageSleepSpriteDecoder.cs` | Legacy sleep-sprite run-length decoding |
| `Net/Client/Game/GameImageSpriteClip.cs` | Allocation-free picture clipping and interlace state |
| `Net/Client/Game/GameImageStorageInitialiser.cs` | Pixel and picture storage allocation |
| `Net/Client/Game/GameObjectArrayInitialiser.cs` | Required and optional geometry-array allocation |
| `Net/Client/Game/GameObjectGeometryBuilder.cs` | Vertex and face mutation |
| `Net/Client/Game/GameObjectShadeDecoder.cs` | Stateful legacy shade decoding |
| `Net/Client/Game/ObjectModelElementCollection.cs` | Copied, ordered model element collections |
| `Net/Client/MenuComponentType.cs` | Named legacy menu component variants |
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
- discovered unit tests: 1,368;
- failures: 0;
- skipped: 0.

The warning emitted by the null-camera-model test is expected diagnostic output from an explicitly tested branch.