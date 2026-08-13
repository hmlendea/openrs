# 0001: Unit-Test-Only Automated Verification

## Status

Accepted on 13 August 2026.

## Context

OpenRS must remain visually, behaviourally, and protocol compatible with the existing version 3 server. The repository contains one NUnit project with broad deterministic coverage of core models, managers, geometry, rasterisation, camera state, menus, chat, and protocol primitives.

The project owner requires that no integration or other automated test category be created.

## Decision

`OpenRS.UnitTests` is the only automated test project. Every automated test must isolate one production unit and must not require:
- a live server or network connection;
- a graphics or audio device;
- a window manager or desktop automation;
- an external account or secret;
- machine-specific files or state.

Public API manifests, source/data/content hashes, static analysis, and profiling are verification artefacts rather than test suites. Runtime server, visual, audio, and lifecycle checks are manual activities at major phase boundaries.

## Consequences

- Production slices without direct unit coverage are retained until unit coverage can be added without exposing internals solely for tests.
- Live packet timing, graphics-driver results, audio-device output, and complete workflows cannot be certified by automated tests in this programme.
- Every accepted production edit runs its focused unit suite immediately and the complete unit suite after the cohesive slice.
- Compilation alone is never sufficient evidence.