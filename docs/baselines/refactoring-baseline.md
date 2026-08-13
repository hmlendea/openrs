# Refactoring Compatibility Baseline

## Environment

Recorded on 13 August 2026 on the approved Linux development workstation.

- .NET SDK: `10.0.104`
- .NET runtime: `10.0.4`
- target framework: `net10.0`
- server protocol version: `3`
- server port: `43594`
- build configuration for release checks: `Release`

## Automated Unit Baseline

Command:

```bash
dotnet test OpenRS.slnx
```

Result:
- discovered: 1,095;
- passed: 1,095;
- failed: 0;
- skipped: 0.

The warning produced while adding a null camera model is expected diagnostic output from a direct unit test.

## Public API Manifest

The baseline and current Release assemblies were reflected with identical sorting and member selection.

- manifest records: 3,402;
- baseline SHA-256: `c1f380c6ce9aa784dcd8e94bf36fcb597cee8206e020f404c5dc15dcac6ae8db`;
- current SHA-256: `c1f380c6ce9aa784dcd8e94bf36fcb597cee8206e020f404c5dc15dcac6ae8db`;
- comparison result: identical.

The manifest includes public and nested-public types, attributes, base types, interfaces, fields, literal values, constructors, methods, properties, and events.

## Data And Content Manifest

The normalised aggregate contains:
- data files: 1,278;
- content source files: 517;
- baseline SHA-256: `67fda600086e81c99fb557115c89c44c503eb0b89b33617bed8b8402552f8f11`;
- current SHA-256: `67fda600086e81c99fb557115c89c44c503eb0b89b33617bed8b8402552f8f11`;
- comparison result: identical.

Generated `Content/bin` and `Content/obj` files are excluded from the source-content aggregate.

## Release Output

- baseline files: 36;
- current files: 36;
- filename comparison result: identical;
- Release build warnings: 0;
- Release build errors: 0.

The current Release assembly SHA-256 is implementation-specific and is not required to match the pre-refactoring binary because internal IL has changed.

## Dependency Versions

Direct package versions remain unchanged:
- `Microsoft.Extensions.Configuration.Json` 10.0.9;
- `MonoGame.Content.Builder.Task` 3.8.4;
- `MonoGame.Framework.DesktopGL` 3.8.4;
- `NuciDAL` 3.1.1;
- `NuciLog` 1.2.1;
- `NuciLog.Core` 3.0.0;
- `NuciText.Censorship` 1.0.0;
- `NuciText.Censorship.English` 1.0.0;
- `NuciXNA.DataAccess` 4.0.3;
- `NuciXNA.Graphics` 2.2.5;
- `NuciXNA.Gui` 3.4.2;
- `NuciXNA.Input` 2.1.1;
- `NuciXNA.Primitives` 2.1.7;
- `System.Drawing.Common` 9.0.0.

## Build Performance Observation

A warmed Release build without restore produced:
- real time: 1.213 seconds;
- user time: 1.153 seconds;
- system time: 0.113 seconds.

This is an observation, not a performance test or cross-machine acceptance threshold.

## Runtime Checklist

Verified:
- canonical server compiles and starts;
- server loads 39 packet handlers and listens on port 43594;
- Release client starts;
- client loads 987 animation frames;
- the version 3 login handshake succeeds with server response code `0`;
- the pre-existing missing `runiteruck1` GLB warning remains unchanged.

Pending explicit manual observation:
- complete initial world-state inspection;
- movement and camera input;
- inventory and bank;
- shop, trade, and duel;
- combat, prayers, and spells;
- chat and social state;
- visual and audio equivalence;
- logout and reconnect.

Forced terminal termination after login logged a `NullReferenceException` from `MovementRegionPacketHandler.SnapshotLastPlayers` after the application had started closing. That handler is unchanged by this refactoring. Normal shutdown equivalence remains part of the pending manual checklist; this observation is not treated as a successful compatibility check or repaired as unrelated scope.

No automated integration or non-unit test is used for this checklist.

## Reproducible Compatibility Command

The non-test manifest comparisons are implemented in:

```bash
OpenRS/Tools/verify-compatibility.sh
```

The tool compares current code with `HEAD` for public API, source data/content, and Release output filenames. It does not launch a server, client, graphics device, or non-unit test.