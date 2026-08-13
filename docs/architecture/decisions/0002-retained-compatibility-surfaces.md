# 0002: Retained Compatibility Surfaces

## Status

Accepted on 13 August 2026.

## Context

Several legacy classes remain above the preferred 300-line threshold. Their remaining code includes public mutable state, graphics-device behaviour, network sequencing, content loading, packet mutation, or incomplete legacy operations that are not directly isolated by the existing unit suite.

Splitting those paths without discriminating unit coverage would distribute risk rather than improve maintainability.

## Decision

The following categories remain intentionally above the preferred threshold until direct unit coverage exists for the responsibility being moved:
- `GameClient`, `GameApplet`, and `GameAppletMiddleMan` compatibility and lifecycle state;
- packet handlers and input handlers with ordered mutations and protocol side effects;
- content, media, map, model, and world loaders;
- renderer classes requiring graphics-device or complete state fixtures;
- camera projection and polygon rendering paths;
- `Menu` rendering paths not covered by its input and component-state tests;
- `EngineHandle` scene-construction paths beyond covered tile and pathfinding units;
- `GameObject` public compatibility state and projection operations;
- incomplete collision and path operations.

Focused, directly covered responsibilities have been extracted. Remaining classes may exceed the threshold, but no changed method increases their maximum complexity.

## Consequences

- A line-count target cannot justify unverified movement of observable behaviour.
- New logic must not be added to retained compatibility classes when a focused owner exists.
- Future extraction begins with unit coverage for the precise responsibility, not a broad rewrite.
- Each retained class is listed by the set-based classification in `refactoring-inventory.md`.