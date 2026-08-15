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
| `GameLogic/GameManagers/CombatManager.cs` | Delegated elemental-staff rune coverage while retaining inventory quantity orchestration | `CombatManagerTests` |
| `GameLogic/GameManagers/InventoryManager.cs` | Reduced to the public inventory facade over active-item and bank-state owners | `InventoryManagerTests`, `CombatManagerTests` |
| `Models/Appearance.cs` | Delegated sprite and colour-range validation while retaining public model metadata | `AppearanceTests` |
| `Models/MobInstance.cs` | Shared immutable movement-sprite table | `MobInstanceTests` |
| `Net/Client/Game/ClientMob.cs` | Named protocol capacities and unset combat-level state | `ClientMobTests` |
| `Net/Client/Game/Cameras/Camera.cs` | Centralised projection-bound expansion | `CameraStateTests` |
| `Net/Client/Game/Cameras/CameraDepthSorter.cs` | Reduced to the public depth-sort and dependency-resolution facade | `CameraDepthSorterTests`, `CameraPolygonIntersectionTests` |
| `Net/Client/Game/Cameras/CameraPolygonRasteriser.cs` | Centralised specialised polygon edge setup and delegated mouse-hit evaluation while retaining scanline ownership | `CameraRenderingTests`, `CameraSceneObjectTrackerTests` |
| `Net/Client/Game/Cameras/CameraSceneObjectTracker.cs` | Separated mouse-hit frame state from scene metadata and geometry | `CameraSceneObjectTrackerTests`, `CameraStateTests` |
| `Net/Client/Game/Cameras/CameraTextureManager.cs` | Separated buffer residency, palette rasterisation, lighting animation, and packed-colour conversion | `CameraTextureTests` |
| `Net/Client/Game/Cameras/PolygonIntersectionCalculator.cs` | Separated plane classification and projected-polygon construction from polygon sweeping | `CameraDepthSorterTests`, `CameraPolygonIntersectionTests` |
| `Net/Client/Game/ChatMessage.cs` | Reduced to compatibility facade | `ChatMessageTests` |
| `Net/Client/Game/ChatMessageCodec.cs` | Reduced to encoding and decoding coordination over a shared nibble alphabet | `ChatMessageTests` |
| `Net/Client/Game/EngineHandle.cs` | Centralised grid and sector allocation, grid guards, sector indexing, and seam policy | `EngineHandleTileTests`, `EngineHandlePathfindingTests` |
| `Net/Client/Game/GameImage.cs` | Reduced to bounded drawing facade with delegated storage and colour packing | `GameImageDrawingTests`, `GameImageColourTests` |
| `Net/Client/Game/GameImageCharacterRenderer.cs` | Reduced to clipping, default-colour policy, logging, and specialised colour/flipped renderer coordination | `GameImageCharacterTests` |
| `Net/Client/Game/GameImageMinimapRasteriser.cs` | Separated edge scan conversion from texture scanline rendering | `GameImageMinimapTests` |
| `Net/Client/Game/GameImageMinimapRenderer.cs` | Reduced to projection, counter, and rasterisation coordination with retained error propagation | `GameImageMinimapTests` |
| `Net/Client/Game/GameImagePaletteConverter.cs` | Reduced to picture-state mutation over indexed-palette and direct-colour algorithms | `GameImagePictureTests` |
| `Net/Client/Game/GameImagePictureManager.cs` | Reduced to picture lifecycle coordination with delegated decoding, palette conversion, and capture | `GameImagePictureTests` |
| `Net/Client/Game/GameImagePictureDataDecoder.cs` | Separated metadata and palette parsing from pixel scan-order decoding | `GameImagePictureTests` |
| `Net/Client/Game/GameImagePixelRasteriser.cs` | Reduced to primitive opaque pixel drawing with delegated viewport and screen-buffer processing | `GameImageDrawingTests`, `GameImageTextTests` |
| `Net/Client/Game/GameImageScaledSpriteBlitter.cs` | Centralised primary/secondary tint classification and horizontal scanline clipping | `GameImageCharacterTests` |
| `Net/Client/Game/GameImageShapeRasteriser.cs` | Reuses shared alpha composition and clipped rectangle scanline state | `GameImageDrawingTests` |
| `Net/Client/Game/GameImageSpriteBlitter.cs` | Centralised identical direct, indexed, and flipped sprite blend arithmetic | `GameImageSpriteTests`, `GameImageCharacterTests` |
| `Net/Client/Game/GameImageSpriteRenderer.cs` | Centralised picture clipping and extracted scaled-entity clipping state | `GameImageSpriteTests` |
| `Net/Client/Game/GameObject.cs` | Delegated allocation, geometry mutation, shade decoding, composition, area splitting, polygon copying, bounds, normals, lighting, and projection | `GameObjectTests` |
| `Net/Client/Game/GameObjectComposer.cs` | Reduced to child-object composite assembly and polygon-group mapping | `GameObjectTests` |
| `Net/Client/Game/GameObjectDataLoader.cs` | Removed misplaced optional-array allocation | `GameObjectTests` |
| `Net/Client/Game/GameObjectShaderCalculator.cs` | Reduced to lighting state, shade levels, and Gouraud-mode initialisation | `GameObjectTests` |
| `Net/Client/Game/GameObjectTransformer.cs` | Reduced to local-to-world transformation with delegated bounds, normals, and camera projection | `GameObjectTests` |
| `Net/Client/Game/ObjectModel.cs` | Centralised copied vertex and face collection semantics | `ObjectModelTests` |
| `Net/Client/Game/GameImageTextRenderer.cs` | Separated font storage, colour-tag resolution, and glyph rasterisation from text layout | `GameImageTextTests` |
| `Net/Client/Game/PathFinder.cs` | Reduced to breadth-first-search orchestration over destination, neighbour, and route collaborators | `EngineHandlePathfindingTests` |
| `Net/Client/Game/RscSector.cs` | Reduced to tile-grid state and coordinate indexing with delegated stream decoding | `RscSectorTests`, `SectorTileTests` |
| `Net/Client/Game/SectorTile.cs` | Retains its data contract and serialised size while delegating stream decoding | `SectorTileTests`, `RscSectorTests` |
| `Net/Client/Game/SectorCoordinates.cs` | Coordinates now own local tile indexing | `EngineHandleTileTests` |
| `Net/Client/GameApplet.cs` | Separated keyboard and mouse state mutation from the lifecycle facade | `GameAppletTests` |
| `Net/Client/GameAppletMiddleMan.cs` | Separated deterministic social packet handling from connection and authentication flow | `GameAppletMiddleManTests` |
| `Net/Client/Link.cs` | Delegated remaining-byte conversion and fixed in-memory file caching | `LinkTests`, `GameAppletTests` |
| `Net/Client/Menu.cs` | Reduced to component state and draw coordination over panel, text, keyboard, and scrollbar collaborators | `MenuTests`, `MenuRenderingTests` |
| `Net/Client/Net/LoginEncryptor.cs` | Reduced to the public mutable-buffer facade over byte-cursor and RSA packet operations | `LoginEncryptorTests` |
| `Net/Client/Net/PacketConstruction.cs` | Reduced to the public and virtual protocol facade over framing, reader, and writer state | Packet construction unit suites |
| `Net/Client/Utilities/GameClientUtilities.cs` | Delegated pure item-count formatting | `GameClientUtilitiesTests` |

## Focused Collaborators Added

| File | Responsibility |
| --- | --- |
| `GameLogic/GameManagers/ElementalRuneStaffCoverageChecker.cs` | Elemental rune coverage from equipped standard, battle, and mystic staves |
| `GameLogic/GameManagers/InventoryBank.cs` | Server-bank mutation and visible-bank synchronisation |
| `GameLogic/GameManagers/InventoryItemCollection.cs` | Active inventory slots, equipment lookup, removal, and quantity calculation |
| `Models/AppearanceValidator.cs` | Appearance sprite and colour-range policy |
| `Net/Client/Game/Cameras/CameraDepthQuickSorter.cs` | Descending camera-model depth sorting |
| `Net/Client/Game/Cameras/CameraHitTracker.cs` | Mouse-hit frame coordinates, candidates, objects, and face indices |
| `Net/Client/Game/Cameras/CameraModelDependencyResolver.cs` | Overlap dependency assignment and render-order reordering |
| `Net/Client/Game/Cameras/CameraModelPlaneClassifier.cs` | Projected model vertices versus a reference face plane |
| `Net/Client/Game/Cameras/CameraPolygonEdge.cs` | Shared fixed-point polygon edge clipping, position, and shade slopes |
| `Net/Client/Game/Cameras/CameraPolygonHitTester.cs` | Scanline-based polygon mouse-hit evaluation and recording |
| `Net/Client/Game/Cameras/CameraTextureBufferPool.cs` | Texture access clock, buffer allocation, and least-recently-used eviction |
| `Net/Client/Game/Cameras/CameraTextureColourCodec.cs` | Packed texture-colour quantisation and expansion |
| `Net/Client/Game/Cameras/CameraTextureRasteriser.cs` | Palette pixels, lighting variants, and 64-pixel texture animation |
| `Net/Client/Game/Cameras/ProjectedPolygonBuilder.cs` | Projected polygon arrays and widened two-vertex segments |
| `Net/Client/Game/ChatMessageDecoder.cs` | Chat nibble decoding and display normalisation |
| `Net/Client/Game/ChatMessageEncoder.cs` | Chat text truncation and nibble encoding |
| `Net/Client/Game/ChatMessageNibbleAlphabet.cs` | Shared chat character-code mapping |
| `Net/Client/Game/GameImageBlurProcessor.cs` | Area blur |
| `Net/Client/Game/GameImageAlphaBlender.cs` | Reusable integer RGB alpha composition |
| `Net/Client/Game/GameImageCharacterClip.cs` | Allocation-free flipped and sheared character layout state |
| `Net/Client/Game/GameImageCharacterColourRenderer.cs` | Direct and indexed one-/two-colour character rendering |
| `Net/Client/Game/GameImageColourTint.cs` | Allocation-free primary and secondary character tint classification |
| `Net/Client/Game/GameImageColourPacker.cs` | Pure RGB/RGBA packing and channel clamping |
| `Net/Client/Game/GameImageDirectColourResolver.cs` | Indexed-palette expansion with opaque-black and transparency sentinels |
| `Net/Client/Game/GameImageEntityClip.cs` | Allocation-free scaled-entity clipping and source sampling state |
| `Net/Client/Game/GameImageFlippedSpriteRenderer.cs` | Flipped blended and colour-shifted sprite rendering |
| `Net/Client/Game/GameImageFontRegistry.cs` | Process-global font arrays, shadow flags, and character offsets |
| `Net/Client/Game/GameImageGlyphRasteriser.cs` | Clipped opaque and alpha glyph pixels |
| `Net/Client/Game/GameImageIndexedPalette.cs` | Allocation-free indexed-palette result state |
| `Net/Client/Game/GameImageIndexedPaletteBuilder.cs` | Frequency-ranked palette creation and nearest-colour indexing |
| `Net/Client/Game/GameImageMinimapDrawCounter.cs` | Legacy minimap draw-counter and rotation-state policy |
| `Net/Client/Game/GameImageMinimapProjection.cs` | Allocation-free rotated minimap corner projection and vertical bounds |
| `Net/Client/Game/GameImageMinimapScanlineBuilder.cs` | Minimap edge rasterisation and scanline range storage |
| `Net/Client/Game/GameImageMinimapRotationTable.cs` | Lazy minimap trigonometry table initialisation |
| `Net/Client/Game/GameImagePictureCapture.cs` | Row-major and column-major screen capture |
| `Net/Client/Game/GameImagePicturePixelDecoder.cs` | Linear and column-major picture pixel scan decoding |
| `Net/Client/Game/GameImageRectangleClip.cs` | Shared clipped and interlaced rectangle scanline state |
| `Net/Client/Game/GameImageShapeRasteriser.cs` | Circle, alpha-rectangle, and gradient rasterisation |
| `Net/Client/Game/GameImageScreenBufferProcessor.cs` | Full-screen clearing and fading |
| `Net/Client/Game/GameImageScaledScanline.cs` | Allocation-free horizontal clipping for scaled sprite scanlines |
| `Net/Client/Game/GameImageSleepSpriteDecoder.cs` | Legacy sleep-sprite run-length decoding |
| `Net/Client/Game/GameImageSpriteClip.cs` | Allocation-free picture clipping and interlace state |
| `Net/Client/Game/GameImageStorageInitialiser.cs` | Pixel and picture storage allocation |
| `Net/Client/Game/GameImageTextColourResolver.cs` | Inline text colour-code resolution |
| `Net/Client/Game/GameImageViewportController.cs` | Drawing-area bounds mutation and reset |
| `Net/Client/Game/GameObjectAreaSplitter.cs` | Spatial chunk sizing, construction, face assignment, and final projection-array sizing |
| `Net/Client/Game/GameObjectArrayInitialiser.cs` | Required and optional geometry-array allocation |
| `Net/Client/Game/GameObjectBoundsCalculator.cs` | Face, collider, and global transformed bounds |
| `Net/Client/Game/GameObjectGeometryBuilder.cs` | Vertex and face mutation |
| `Net/Client/Game/GameObjectNormalCalculator.cs` | Polygon normals, flat shading, and Gouraud normal accumulation |
| `Net/Client/Game/GameObjectPolygonCopier.cs` | Vertex de-duplication and polygon geometry/render-metadata copying |
| `Net/Client/Game/GameObjectProjector.cs` | Camera projection context and per-vertex projected coordinates |
| `Net/Client/Game/GameObjectShadeDecoder.cs` | Stateful legacy shade decoding |
| `Net/Client/Game/PathDestinationMatcher.cs` | Destination containment and object-adjacency policy |
| `Net/Client/Game/PathNeighbourExpander.cs` | Cardinal and diagonal breadth-first-search expansion |
| `Net/Client/Game/PathRouteReconstructor.cs` | Direction-marker reconstruction into path turning points |
| `Net/Client/Game/RscSectorDecoder.cs` | Complete sector-buffer validation and tile assembly |
| `Net/Client/Game/SectorTileDecoder.cs` | Fixed-order sector-tile byte decoding |
| `Net/Client/Game/ObjectModelElementCollection.cs` | Copied, ordered model element collections |
| `Net/Client/GameAppletKeyboardInputHandler.cs` | Applet key flags, text buffers, and submission state |
| `Net/Client/GameAppletMouseInputHandler.cs` | Applet mouse coordinates, buttons, and callback ordering |
| `Net/Client/GameAppletSocialPacketHandler.cs` | Friend, ignore, privacy, and private-message packet state |
| `Net/Client/LinkFileCache.cs` | Fixed-capacity process-global in-memory file cache |
| `Net/Client/MenuPanelRenderer.cs` | Menu panels, borders, corners, and scrollbars |
| `Net/Client/MenuScrollbarState.cs` | Allocation-free calculated scrollbar state |
| `Net/Client/MenuScrollbarStateCalculator.cs` | Shared scrollbar arrows, dragging, clamping, and thumb arithmetic |
| `Net/Client/MenuTextInputEditor.cs` | Menu text mutation and submission rules |
| `Net/Client/MenuTextRenderer.cs` | Menu text alignment, input masking, hit-testing, and drawing |
| `Net/Client/MenuComponentType.cs` | Named legacy menu component variants |
| `Net/Client/Net/LoginPacketBuffer.cs` | Login packet byte cursor and primitive encoding/decoding |
| `Net/Client/Net/LoginPacketRsaEncryptor.cs` | RSA modular exponentiation and length-prefixed packet replacement |
| `Net/Client/Net/PacketConstructionReader.cs` | Compact/extended inbound frame state and deferred read errors |
| `Net/Client/Net/PacketConstructionWriter.cs` | Outbound packet buffers, primitive bytes, metrics, flushing, and deferred write errors |
| `Net/Client/Net/PacketFraming.cs` | Compact and extended packet-length framing |
| `Net/Client/SignedByteStreamReader.cs` | Remaining-stream conversion preserving signed byte patterns |
| `Net/Client/Utilities/ItemCountFormatter.cs` | Pure inventory quantity display formatting |

## Intentionally Retained Categories

- Packet and model enumerations: immutable numeric compatibility contracts.
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