# OpenRS Architecture

## Purpose

This document describes the verified OpenRS architecture after the initial compatibility-preserving refactoring programme. It records current ownership and dependency rules, not speculative future structure.

## Runtime Flow

1. `Program` loads application configuration and initialises logging.
2. `GameWindow` owns the MonoGame lifecycle and delegates screens to NuciXNA `ScreenManager`.
3. `GameplayScreen` coordinates the legacy `GameClient` facade.
4. Input handlers interpret keyboard and mouse state and invoke existing client operations.
5. `PacketConstruction` and `StreamClass` preserve the version 3 wire protocol.
6. Specialised packet handlers mutate client session state.
7. Loaders construct world, model, texture, and media state.
8. Renderers query client state and issue the existing drawing and audio operations.

The order of lifecycle calls, state mutation, packet production, and drawing is a compatibility contract.

## Architectural Areas

### Host And Lifecycle

Paths:
- `OpenRS/Program.cs`
- `OpenRS/GameWindow.cs`
- `OpenRS/Gui/Screens`

Responsibilities:
- process startup and shutdown;
- MonoGame lifecycle ordering;
- NuciXNA screen registration and transitions;
- final composition of runtime services.

The host may coordinate presentation and application services. It must not encode packets, parse repositories, or own domain policy.

### Presentation

Paths:
- `OpenRS/Gui`
- `OpenRS/Net/Client/Rendering`
- presentation-specific classes under `OpenRS/Net/Client/Game`

Responsibilities:
- screen and control composition;
- rendering and visual calculations;
- presentation text and audio decisions;
- immutable or read-only consumption of game state.

`GameImage` remains the public compatibility facade for legacy drawing methods. Raw operations are divided among:
- `GameImagePixelRasteriser` for boxes, lines, individual pixels, and pixel-grid copies, with
	shared rectangle clipping for opaque and alpha-filled boxes;
- `GameImageViewportController` for drawing-area state and `GameImageScreenBufferProcessor` for
	clearing and fading;
- `GameImageShapeRasteriser` for circles, alpha rectangles, and gradients, with shared alpha
	composition;
- `GameImageBlurProcessor` for area blur;
- `GameImagePictureManager` for picture lifecycle coordination, with focused collaborators for
	metadata decoding, pixel scan order, sleep-sprite decoding, indexed/direct palette conversion,
	and screen capture;
- `GameImageSpriteRenderer` for direct and indexed picture drawing and scaled entities, with
	allocation-free values for clipping and scaling state;
- `GameImageCharacterRenderer` for tinting, flipping, and sheared character drawing, with
	dedicated colour-path and flipped-sprite renderers plus allocation-free layout state;
- `GameImageScaledSpriteBlitter` for specialised direct and indexed colour paths, with shared
	allocation-free tint classification and horizontal scanline clipping;
- `GameImageMinimapRenderer` for minimap draw coordination and error propagation, with focused
	collaborators for projection, rotation tables, counters, edge scan conversion, and scanline
	rasterisation;
- `GameImageTextRenderer` for text layout and tags, with a font registry, colour resolver, and
	glyph rasteriser.

Presentation code must not become an alternative owner of network or domain state.

### Application And Session Orchestration

Paths:
- `OpenRS/Net/Client/GameClient.cs`
- `OpenRS/Net/Client/Input`
- `OpenRS/Net/Client/Handlers`
- `OpenRS/Net/Client/World`

Responsibilities:
- coordinate ordered user and server operations;
- preserve current `GameClient` public and protected contracts;
- route input, packet, and world-interaction operations to their current owners.

`GameClient` remains a compatibility facade. New focused collaborators must not depend on the complete facade when a narrower existing owner is available.

### Domain

Paths:
- `OpenRS/Models`
- `OpenRS/GameLogic/GameManagers`

Responsibilities:
- domain state and values;
- inventory, bank, combat, quest, and entity operations;
- path and world-state concepts.

`InventoryManager` owns inventory and bank slot mutation. Slot allocation, value copying, server-bank mutation, visible-bank synchronisation, and inventory augmentation are separate private operations within that owner.

### Protocol

Paths:
- `OpenRS/Net`
- `OpenRS/Net/Client/Net`
- packet-facing code under `OpenRS/Net/Client/Handlers`

Responsibilities:
- packet identifiers and login codes;
- byte order, framing, buffering, and packet counters;
- version 3 login and transport sequencing;
- incoming command decoding.

`PacketConstruction` preserves its public compatibility state and methods. `PacketFraming` owns compact and extended length encoding and decoding. Numeric packet identifiers and serialised bytes are immutable contracts.

### Data Access And Loading

Paths:
- `OpenRS/DataAccess`
- `OpenRS/GameLogic/Mapping`
- `OpenRS/Net/Client/Loading`
- loading-specific classes under `OpenRS/Net/Client/Game`

Responsibilities:
- JSON and binary data acquisition;
- entity-to-domain mapping;
- content, model, map, sector, and world construction;
- current error, fallback, and event order.

Repositories must not depend on active presentation or client-session state.

### Infrastructure

Paths:
- `OpenRS/Audio`
- `OpenRS/Localisation`
- `OpenRS/Logging`
- `OpenRS/Settings`

Responsibilities:
- external devices and libraries;
- localisation and configuration;
- diagnostics and file locations.

Infrastructure may implement contracts required by inner areas, but it must not own game policy.

## Focused Compatibility Facades

The following public types deliberately retain legacy entry points while delegating focused responsibilities:
- `GameImage` delegates raw pixel, shape, blur, picture, sprite, character, text, and minimap work.
- `GameObject` delegates array allocation, geometry mutation, shade decoding, composition,
	area splitting, polygon copying, transformation, bounds, normals, lighting, and projection work.
- `ChatMessage` delegates nibble encoding and decoding while retaining the public `LastChat` buffer.
- `PacketConstruction` delegates stateless packet-length framing while retaining public transport state.
- `GameClientUtilities` delegates pure item-count formatting while retaining its static method.

These facades contain delegation or compatibility shape conversion. New domain policy must be placed in the component that owns it.

## Dependency Rules

- Domain models do not depend on GUI, rendering, sockets, repositories, or configuration implementations.
- Renderers do not construct packets or own gameplay state.
- Packet framing does not depend on presentation or data access.
- Repositories do not depend on active client state.
- Extracted internal components receive one owning facade or the narrow values they require.
- No new service locator, mutable singleton, global collection, or dependency cycle is permitted.

## Verification

`OpenRS.UnitTests` is the only automated test project. Refactoring acceptance requires:
- `dotnet test OpenRS.slnx` passing from a clean checkout;
- `OpenRS/Tools/verify-compatibility.sh` reporting equal API, data/content, and output manifests;
- no new compiler or editor diagnostics;
- unchanged public protocol enumerations and packet bytes;
- unchanged data and content files;
- manual runtime inspection at major lifecycle, server, visual, and audio boundaries.

No integration, end-to-end, snapshot, golden-master, architecture, mutation, performance, server-dependent, graphics-device-dependent, or desktop-automation test suite may be introduced.