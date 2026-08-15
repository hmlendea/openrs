# Compatibility Facades

## Purpose

Compatibility facades preserve established public entry points while internal responsibilities move to focused owners. Their public signatures, state, exceptions, allocation semantics where observable, and ordered side effects remain unchanged.

## Game Image

Facade: `OpenRS.Net.Client.Game.GameImage`

Delegates:
- `GameImageAlphaBlender`;
- `GameImagePixelRasteriser`;
- `GameImageRectangleClip`;
- `GameImageScreenBufferProcessor`;
- `GameImageViewportController`;
- `GameImageShapeRasteriser`;
- `GameImageBlurProcessor`;
- `GameImagePictureManager`;
- `GameImagePictureDataDecoder`;
- `GameImagePicturePixelDecoder`;
- `GameImageSleepSpriteDecoder`;
- `GameImagePaletteConverter`;
- `GameImageIndexedPaletteBuilder`;
- `GameImageDirectColourResolver`;
- `GameImagePictureCapture`;
- `GameImageSpriteRenderer`;
- `GameImageSpriteClip`;
- `GameImageEntityClip`;
- `GameImageSpriteBlitter`;
- `GameImageCharacterRenderer`;
- `GameImageCharacterColourRenderer`;
- `GameImageFlippedSpriteRenderer`;
- `GameImageCharacterClip`;
- `GameImageScaledSpriteBlitter`;
- `GameImageColourTint`;
- `GameImageScaledScanline`;
- `GameImageTextRenderer`;
- `GameImageFontRegistry`;
- `GameImageTextColourResolver`;
- `GameImageGlyphRasteriser`;
- `GameImageMinimapRenderer`;
- `GameImageMinimapProjection`;
- `GameImageMinimapRotationTable`;
- `GameImageMinimapDrawCounter`;
- `GameImageMinimapRasteriser`;
- `GameImageMinimapScanlineBuilder`.

Preserved contracts:
- all public drawing methods and properties;
- pixel coordinate order and clipping;
- integer blend and interpolation arithmetic;
- interlaced row selection;
- exception and zero-size behaviour.

## Game Object

Facade: `OpenRS.Net.Client.Game.GameObject`

Delegates:
- `GameObjectAreaSplitter`;
- `GameObjectArrayInitialiser`;
- `GameObjectBoundsCalculator`;
- `GameObjectGeometryBuilder`;
- `GameObjectNormalCalculator`;
- `GameObjectPolygonCopier`;
- `GameObjectProjector`;
- `GameObjectShadeDecoder`;
- existing loader, composer, transformer, transform controller, and lighting calculator.

Preserved contracts:
- public fields, constructors, and methods;
- required and optional array identity;
- capacity rejection and index results;
- stateful shade-buffer cursor;
- transform and projection state.

## Chat Message

Facade: `OpenRS.Net.Client.Game.ChatMessage`

Delegates:
- `ChatMessageCodec`;
- `ChatMessageEncoder`;
- `ChatMessageDecoder`;
- `ChatMessageNibbleAlphabet`.

Preserved contracts:
- public mutable `LastChat` buffer;
- nibble encoding and trailing padding;
- sentence capitalisation and tag replacement;
- 80-character truncation;
- `"."` decode fallback.

## Packet Construction

Facade: `OpenRS.Net.Client.Net.PacketConstruction`

Delegates:
- `PacketFraming`;
- `PacketConstructionReader`;
- `PacketConstructionWriter`.

Preserved contracts:
- all public fields and virtual methods;
- compact packet byte swapping;
- extended packet length encoding;
- counters, timeouts, deferred errors, and flush thresholds;
- primitive byte order and UTF-8 bytes.

## Login Encryptor

Facade: `OpenRS.Net.Client.Net.LoginEncryptor`.

Delegates:
- `LoginPacketBuffer`;
- `LoginPacketRsaEncryptor`.

Preserved contracts:
- public mutable packet and offset fields;
- big-endian primitive and UTF-8 encoding;
- RSA modular exponentiation and length prefixing;
- partial cursor mutation when an operation throws;
- the legacy no-op `Encrypt` method.

## Game Client Utilities

Facade: `OpenRS.Net.Client.Utilities.GameClientUtilities`.

Delegate: `ItemCountFormatter` for the pure static formatting operation.

Preserved contracts:
- static call sites;
- culture-sensitive integer conversion currently used by the client;
- thousands separators and colour-tag thresholds;
- negative count formatting.

## Removal Policy

No compatibility facade or member may be removed during this programme. A future removal requires explicit approval, public API analysis, and an independently versioned compatibility decision.