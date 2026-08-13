# OpenRS Refactoring Plan

## Objective

Refactor the entire OpenRS project without changing any observable result. The game must present identical visuals, audio, input responses, timing semantics, data interpretation, and network traffic. It must remain fully compatible with the existing server and existing game data.

This programme permits internal structural changes only. A phase is complete only when its compatibility evidence is equal to the approved baseline. Passing compilation alone is insufficient.

The secondary objective is to improve execution performance where profiling demonstrates a material constraint and an optimisation can retain every compatibility contract. Performance work must reduce measured cost rather than rely on intuition, and it must never exchange correctness, maintainability, determinism, or server compatibility for velocity.

## Compatibility Contract

The following contracts are immutable throughout the programme:
- Every client packet identifier, field order, field width, signedness, text encoding, length prefix, compact-packet transformation, flush boundary, and transmission sequence.
- Every server command identifier, packet boundary, bit offset, field interpretation, state mutation, and malformed-packet result.
- The login sequence, client version, session hash, RSA operation, reconnect seqtransliteration-apiuence, timeout semantics, server address defaults, and authentication result handling.
- Every public type, member, constructor, enumeration name and value, event, default value, side effect, exception, and null result.
- Every JSON property name, value type, identifier, localisation key, file name, relative path, case, default, ordering-sensitive operation, and save format.
- Every content asset identifier, content-pipeline name, source asset, texture region, font, colour, scale, coordinate, draw order, blend mode, clipping result, and missing-content result.
- Every screen transition, control registration order, focus rule, mouse region, keyboard mapping, menu order, menu action, camera response, movement rule, and game-state transition.
- Every sound identifier, trigger condition, volume calculation, repetition rule, and playback order.
- The executable name, target framework, startup flow, configuration precedence, output content, and default window dimensions and title.

Compatibility means exact equality for deterministic results. Performance measurements cannot justify an observable change. Execution cost may improve, but game-time progression, update order, input semantics, packet cadence, and other contractually observable timing must remain unchanged.

## Non-Goals

The refactoring programme must not:
- repair legacy defects, complete incomplete functions, rebalance gameplay, alter malformed-input handling, or modernise visible behaviour;
- change server configuration, protocol security, protocol constants, or authentication semantics;
- rename or eliminate public members, packet values, serialised fields, assets, localisation keys, or data files;
- alter graphics, layouts, fonts, colours, animations, audio, input latency rules, or frame-update order;
- update dependencies, the target framework, nullable settings, build properties, or content-pipeline versions during structural phases;
- create integration, end-to-end, snapshot, golden-master, architecture, mutation, performance, or any other non-unit-test project or suite;
- combine structural refactoring with formatting, asset conversion, data migration, performance optimisation, or feature development in the same change.

Potential defect, security, and modernisation changes must be recorded separately and implemented only as explicitly approved compatibility changes after this programme.

## Current Baseline

The baseline recorded on 13 August 2026 is:
- `dotnet build OpenRS/OpenRS.csproj` succeeds for `net10.0`.
- The solution contains one executable project and one NUnit unit-test project.
- `OpenRS.UnitTests` contains 49 unit-test source files covering managers, models, cameras, pathfinding, protocol framing, login encryption, menus, and protocol identifiers.
- Six `TODO` or `NotImplementedException` markers exist and are outside the refactoring scope.
- Startup proceeds through `Program`, `GameWindow`, NuciXNA `ScreenManager`, and `SplashScreen`.
- `GameClient` is the principal mutable client state and orchestration surface.
- Incoming packets pass through `PacketHandler` and seven specialised packet-handler groups.
- Outgoing packets pass through `PacketConstruction` and `StreamClass`.
- Rendering is coordinated by `GameRenderer` and specialised renderers.
- Runtime data and localisation are loaded from `OpenRS/Data`; compiled content is defined by `OpenRS/Content/Content.mgcb`.

Tag or otherwise retain the approved source revision, dependency lock state, binaries, content production, data files, configuration, and unit-test results used to produce the baseline. Never revise a unit-test expectation merely to accept an unintended refactoring difference.

## Target Architecture

The refactored codebase must converge on explicit ownership and inward dependency direction while retaining compatibility adapters at current public entry points. The architecture is a target for incremental extraction, not permission for a wholesale rewrite.

### Architectural Areas

| Area | Responsibility | May Depend On | Must Not Depend On |
| --- | --- | --- | --- |
| Host and lifecycle | Process startup, MonoGame lifecycle, screen composition, and final disposal | Presentation, application orchestration, infrastructure composition | Protocol details, repository implementations, mutable domain internals |
| Presentation | Screens, controls, rendering, visual read models, and audio presentation commands | Application queries, immutable presentation models, MonoGame and NuciXNA | Network transport, repositories, packet buffers, direct domain mutation |
| Application orchestration | Translate input and server messages into ordered domain operations; coordinate use cases | Domain, protocol contracts, repository and infrastructure interfaces | MonoGame drawing types, concrete sockets, concrete repositories |
| Domain | Authoritative game state, rules already present in the client, state transitions, and domain results | Domain types only | MonoGame, NuciXNA, sockets, JSON, files, logging implementations |
| Protocol | Packet identifiers, framing, encoding, decoding, login exchange, and transport sequencing | Protocol types and narrow application command interfaces | Presentation, repositories, rendering, broad `GameClient` mutation |
| Data access | Runtime data acquisition, persistence, data objects, and domain mappings | Domain models, file and JSON infrastructure | Presentation, input, networking, mutable game-session state |
| Infrastructure | Configuration, logging, localisation, audio devices, clocks, randomness, file access, and adapters | External libraries and interfaces owned by inner areas | Domain-policy decisions or game-state ownership |

Permitted dependencies point from outer mechanisms towards narrow contracts owned by inner areas. Transitional compatibility adapters may temporarily violate this direction only when the violation is recorded, has a removal phase, and does not spread to new callers.

### Runtime Flow

The intended runtime flow is:
1. The host composes the existing runtime implementations and starts the unchanged MonoGame lifecycle.
2. Input interpretation produces an application command rather than directly mutating unrelated state.
3. Incoming packet decoding produces a typed application command or message after preserving exact packet semantics.
4. One application handler invokes the authoritative domain owner for that operation.
5. Domain mutation produces an explicit result and, where currently observable, ordered events and outgoing protocol requests.
6. Presentation queries immutable or read-only view data and emits the unchanged ordered draw and audio commands.
7. Outgoing protocol requests are encoded and transmitted by the protocol area with unchanged bytes and timing.

This flow must not introduce asynchronous queues, deferred events, additional frames of latency, or reordered operations unless the baseline demonstrates that they previously existed.

### State Ownership

Before extracting a field from `GameClient`, record it in a state-ownership catalogue containing its current declaration, all readers, all writers, lifetime, reset point, aliases, events, packet dependencies, rendering dependencies, and intended owner.

The intended owners are:

| State Category | Authoritative Owner | Permitted Mutators | Permitted Consumers |
| --- | --- | --- | --- |
| Connection and authentication | Connection session | Login and connection application handlers | Host, login presentation, protocol diagnostics |
| Player identity, skills, fatigue, prayers, and spells | Player state | Player-state packet and local-action handlers | Presentation and application queries |
| Players and non-player characters | Mob state | Movement, region, and mob-update handlers | World interaction, camera queries, rendering |
| Terrain, sectors, walls, objects, and ground items | World state | Loading, region, and object-state handlers | Collision and interaction queries, rendering |
| Inventory, equipment, and bank | Item-container state owners | Inventory, bank, and item-action handlers | Menus, presentation, interaction queries |
| Shop, trade, and duel | Separate session state owners | Their corresponding packet and local-action handlers | Their corresponding presentation and action queries |
| Combat | Combat state | Combat packet and combat-style action handlers | Presentation, menus, audio decisions |
| Quests and social state | Dedicated domain state owners | Corresponding packet handlers | Presentation and interaction queries |
| Camera and viewport | Camera state | Camera input and lifecycle handlers | Projection and rendering calculations |
| Interface selection and menus | Interface state | Input and menu application handlers | Presentation and packet-request creation |
| Loading progress and errors | Loading session | Loading orchestration | Loading presentation and host lifecycle |

Exactly one component owns mutation for each state category. Packet handlers, renderers, controls, and repositories must not become alternative state owners. Cross-domain operations must pass through an application handler whose ordered effects are covered by unit tests where the operation can be isolated and by a documented manual runtime check otherwise.

### Commands, Queries, And Events

- Commands represent an existing user or server-initiated operation and have one handler responsible for its ordered effects.
- Queries return immutable snapshots or read-only views and cannot mutate state, transmit packets, load content, or emit events.
- Events represent effects that are already observable in the baseline; refactoring must not invent additional event semantics merely to decouple classes.
- Commands and events use domain-specific names. Generic `Execute`, `Process`, `HandleData`, or `UpdateState` names require a specific enclosing type that makes the operation unambiguous.
- A handler cannot accept `GameClient` merely for convenient access. It receives the narrow owners and services required by its responsibility.
- Compatibility members on `GameClient` delegate to the authoritative owner and contain no duplicated policy.

### Compatibility Facade

`GameClient` remains the public compatibility facade wherever its existing surface is observable. Its final responsibilities are restricted to:
- preserving existing public and protected members, events, fields where field identity is required, and construction entry points;
- coordinating lifecycle calls that are contractually associated with `GameClient`;
- delegating to explicit state owners and application handlers;
- adapting current public data shapes without duplicating the underlying policy.

The facade must not remain the default dependency for extracted code. Every retained member must be classified as a compatibility member, lifecycle member, or genuinely owned state. A compatibility comment is warranted only where removing apparently redundant code would violate a verified contract.

## Maintainability Standards

Maintainability is an acceptance condition, not a subjective aspiration. Measure the baseline before Phase 2 and report the result after every production phase.

### Responsibility And Cohesion

- Every production type has one describable responsibility and one principal reason to change.
- Every state mutation is located in the authoritative owner or its application handler.
- Types named `Manager`, `Helper`, `Utility`, `Service`, or `Handler` require a precise responsibility; they cannot accumulate unrelated functions.
- Extracted classes must own behaviour, invariants, or a boundary. Data-only wrappers created solely to reduce line counts are prohibited.
- New abstractions require at least two genuine implementations, a verified architectural boundary, or a material reduction in coupling or duplication.
- Compatibility adapters remain thin and contain delegation or shape conversion only.

### Comprehensibility

- Names describe game concepts and intent rather than implementation mechanics.
- Public compatibility names remain unchanged; new internal names use current repository conventions and British English.
- Methods present one abstraction level and one result. Boolean parameters that select distinct operations are replaced internally by explicit operations while preserving public wrappers.
- Protocol constants, packet offsets, bit widths, asset measurements, and sentinel values use domain-specific named constants without changing values.
- Non-obvious protocol, rendering, and lifecycle invariants receive concise comments explaining the constraint, not narrating the code.
- Each architectural area contains a brief document defining its responsibility, dependencies, state ownership, entry points, and principal invariants.

### Complexity Limits

For new or substantially refactored non-generated code:
- methods should remain within 30 logical lines, cyclomatic complexity 10, and nesting depth 3;
- types should remain within 300 logical lines, excluding unavoidable public compatibility declarations;
- constructors should receive no more than seven dependencies;
- a type should expose no more than one principal public responsibility;
- changed code should contain no duplicated block of six or more logical lines when a coherent shared operation exists;
- no new mutable public field, global mutable collection, service locator, or singleton may be introduced;
- no dependency cycle may be introduced between namespaces or architectural areas.

An exceeded threshold requires either further decomposition or a concise architecture decision record explaining why decomposition would reduce clarity or violate compatibility. Existing code above a threshold cannot become more complex during an intermediate phase and must show a monotonic reduction within its scheduled extraction phase.

### Coupling And Dependencies

- Each extracted type receives only the dependencies it uses.
- Domain types remain independent of MonoGame, NuciXNA, networking, repositories, configuration, and logging implementations.
- Renderers and controls cannot access packet construction or mutate authoritative domain state.
- Packet decoders cannot draw, load content, access controls, or select audio devices.
- Repositories cannot reference active client-session state or presentation types.
- Cross-area access occurs through narrow interfaces, commands, queries, or immutable values owned by the inner consumer.
- Static dependency analysis and code review must enforce these rules by namespace and assembly dependency inspection.

### Test Quality

- `OpenRS.UnitTests` is the only permitted automated test project, and every automated test must be a deterministic unit test.
- Do not create integration, end-to-end, snapshot, golden-master, architecture, mutation, performance, server-dependent, graphics-device-dependent, or desktop-automation tests.
- Unit tests may preserve legacy behaviour or define an internal contract, but they must isolate one production unit and replace external collaborators with test doubles.
- Every changed branch has a direct unit test, including boundaries, errors, nulls, empty collections, maximum capacities, and malformed data where currently reachable.
- Unit tests use controlled clocks, randomness, transport, and file access where those influence the isolated unit.
- Unit tests cannot require a live server, graphics device, audio device, window manager, network connection, external account, or machine-specific file.
- Unit tests assert complete relevant results and observable order, not private implementation details.
- Shared fixture builders use domain terminology and expose meaningful defaults; opaque numeric packet arrays include decoded descriptions.
- Flaky unit tests block the phase; retries cannot convert a failure into acceptance.

### Documentation And Decisions

- Maintain an architecture overview containing the area map, dependency direction, runtime flow, and state-ownership catalogue.
- Record consequential decisions concerning state boundaries, compatibility adapters, protocol invariants, rendering invariants, and accepted complexity exceptions as short architecture decision records.
- Document each compatibility shim with the contract it preserves and the phase or condition for its possible elimination.
- Update documentation in the same change that alters an architectural boundary.
- Documentation must describe verified behaviour and current structure, not intended future work presented as complete.

### Reviewability And Source Control

- A change modifies one responsibility and remains independently reversible.
- Characterisation is committed before or with the first structural change it protects.
- Mechanical movement is separated from behavioural or structural editing whenever practical.
- Renames are separated from logic extraction so history remains reviewable.
- Each change report identifies the moved responsibility, old and new owners, compatibility evidence, complexity delta, dependency delta, and residual compatibility shim.
- Review rejects changes that relocate a large method without improving ownership, cohesion, dependencies, or testability.

## Architectural Acceptance Gates

In addition to exact compatibility, every production phase must demonstrate:
- a current state-ownership catalogue for the touched domain;
- no new forbidden dependency or dependency cycle;
- no new unbounded access to `GameClient`;
- no increase in public API surface unless required to preserve an existing binary contract;
- no increase in the maximum complexity of touched production methods or types;
- a reduction in at least one measured defect in the targeted area: complexity, coupling, duplication, mutable-state exposure, responsibility count, or untested branches;
- direct unit tests for every changed branch and static dependency analysis for every changed boundary;
- current documentation for changed ownership and dependencies;
- a recorded rationale for every retained threshold violation and compatibility adapter;
- an independently reviewable diff with no unrelated modifications.

A phase fails when the code remains equally difficult to comprehend but has merely been distributed among more files. Passing compatibility tests cannot override an architectural acceptance failure.

## Automation And Reporting

The canonical continuous integration workflow must execute:
1. Restore with the repository's locked dependency versions.
2. Compile the solution.
3. Execute the `OpenRS.UnitTests` unit-test suite, with no other test project or test category.

Before merge, execute these non-test verification activities locally:
1. Formatting and static analysis configured to avoid behavioural rewrites.
2. Public API manifest comparison.
3. Data, localisation, content, and produced-file hash comparison.
4. Complexity, duplication, dependency-cycle, and changed-branch coverage reporting where tooling is available.

Each phase report must include baseline and current values for source files, production lines, unit-test lines, maximum and mean method complexity, methods above thresholds, types above thresholds, dependency cycles, forbidden dependencies, duplicated blocks, public API changes, and changed-code branch coverage. Performance profiling remains a separate non-test activity in Phase 13. Metrics inform review and do not reward fragmentation or deletion of useful unit tests.

## Performance Standards

Performance is evaluated as execution cost under identical inputs and semantics. The programme must improve verified constraints where compatible improvements exist and must not introduce a statistically significant regression elsewhere.

### Performance Metrics

Measure at minimum:
- process startup to the first presented frame;
- splash, data, content, model, map, sector, and world-loading durations;
- login packet construction and response-processing cost, excluding external network latency;
- update and draw CPU duration at median, 95th percentile, 99th percentile, and maximum;
- frame-time distribution, missed-frame incidence, and sustained frame pacing;
- managed allocations per update, draw, packet, loaded entity, and representative gameplay minute;
- generation 0, generation 1, generation 2, and large-object-heap collection counts and pause durations;
- managed heap, native memory, graphics memory where measurable, and peak working set;
- incoming packet decoding and dispatch throughput and latency by command family;
- outgoing packet construction and buffering throughput and latency by packet size;
- menu construction, inventory rendering, world rendering, model loading, entity updates, and region transitions;
- performance as player, non-player-character, ground-item, wall, object, inventory, bank, chat, and menu populations approach supported maxima.

Retain raw samples and environment metadata so medians, percentiles, variance, confidence intervals, and outliers can be reviewed rather than reduced to one mean value.

### Benchmark Environment

Maintain at least one controlled reference system and one representative reduced-performance system. Record:
- processor, graphics processor, memory, storage, display mode, operating system, kernel, graphics driver, power profile, and thermal state;
- .NET SDK, runtime, garbage collector mode, MonoGame version, native libraries, and package lock state;
- build configuration, compiler options, working directory, configuration, content hashes, server revision, and network topology;
- deterministic inputs, random seeds, clock source, world state, account state, camera path, warm-up procedure, and sample count;
- background-process policy and the method used to detect thermal throttling or external interference.

Use Release production for performance acceptance. Debug measurements may assist diagnosis but cannot approve or reject an optimisation. Compare identical binaries and fixtures repeatedly, randomise benchmark order where practical, and separate cold-start from warmed execution.

### Performance Gates

- Each structural phase must retain comparable profiling measurements before acceptance when it touches a performance-sensitive path.
- A result is a regression when the controlled benchmark demonstrates a statistically significant increase in cost at 95% confidence and the difference exceeds the benchmark's predeclared noise floor.
- Frame-time, loading, allocation, collection, memory, and packet-processing regressions are evaluated independently; an improvement in one metric cannot conceal a regression in another.
- Any accepted regression requires explicit user approval, an architecture decision record, quantified user impact, and evidence that compatibility cannot be preserved by a different implementation.
- A performance optimisation must demonstrate a repeatable improvement in its declared primary metric on at least the controlled reference system and must pass all compatibility, maintainability, and other performance gates.
- Benchmark thresholds, fixtures, sample counts, and noise floors must be declared before measuring a candidate optimisation and cannot be revised merely to accept it.
- Hardware-dependent improvements require manual runtime confirmation that work has not been omitted.

### Optimisation Rules

- Profile first and optimise only a measured constraint represented by a reproducible benchmark.
- Prefer removing redundant work, redundant allocation, repeated parsing, repeated lookup, and unnecessary conversion before introducing caches, pooling, parallelism, or unsafe code.
- Preserve numeric types, operation order, overflow semantics, collection order, object identity, event order, packet order, draw order, and resource lifetime wherever observable.
- A cache must define ownership, key equality, capacity, invalidation, lifetime, memory cost, and deterministic eviction. Unbounded caches are prohibited.
- Pooling requires tests for reset completeness, aliasing, retained references, exceptional returns, and disposal. Pooling cannot change public object identity or lifetime.
- Parallelism and asynchronous processing are prohibited unless deterministic ordering, thread affinity, exception propagation, cancellation, shutdown, and all exact compatibility results are proven.
- Unsafe code, native interoperation, and manual memory management require evidence that safer alternatives cannot meet the measured objective and require dedicated correctness and lifetime tests.
- Do not reduce validation, logging required by the baseline, content quality, draw precision, protocol checks, or test coverage to improve a benchmark.
- Keep each optimisation separate from structural refactoring and identify the profile evidence, hypothesis, benchmark, compatibility evidence, maintenance cost, and rollback condition.
- Retain an optimisation only when its measured benefit justifies its additional complexity. Reject clever code that provides no material repeatable benefit.

### Performance Budgets

Phase 0 must convert the measured baseline into explicit non-regression budgets for each deterministic scenario. Budgets must cover startup, loading, update, draw, frame pacing, memory, allocations, garbage collection, packet processing, and stress populations.

Initial budgets equal the approved baseline distribution plus only the predeclared measurement noise floor. Phase 13 may tighten budgets after accepted improvements. Never relax a budget without explicit user approval and a recorded rationale.

## Required Verification Harness

### Build Baseline

Capture and retain:
- the SDK and runtime versions;
- restored package versions and hashes;
- build command, warnings, and produced file inventory;
- hashes for copied data, configuration, native libraries, and compiled content;
- the initial working-directory and environment assumptions.

### Performance Observation Baseline

Record deterministic profiling scenarios before performance optimisation:
- cold and warm startup;
- content and data loading with fixed input hashes;
- idle gameplay, movement, populated world, dense interface, combat, inventory, bank, shop, trade, duel, and chat;
- packet decode and dispatch microbenchmarks for every command family;
- packet construction benchmarks at compact, extended, and maximum supported lengths;
- sustained gameplay replay and maximum supported entity and interface populations;
- repeated load, login, logout, reconnect, region transition, and shutdown cycles to reveal retention and resource leaks.

Capture raw timing, allocation, collection, memory, frame pacing, and throughput samples without creating a benchmark test project. Execute sufficient independent process runs to establish stable distributions and a noise floor.

### Protocol Unit Coverage

Unit-test protocol primitives and isolated handlers through test doubles. Unit-test fixtures must include:
- every `ClientPacket` value emitted by reachable code;
- compact payload lengths at 0, 1, 158, and 159 bytes;
- extended payload lengths at 160, 161, 255, 256, and the maximum supported length;
- signed and unsigned boundary values for bytes, shorts, integers, and longs;
- empty, ASCII, and multibyte UTF-8 strings using current encoding semantics;
- consecutive packets, delayed flushes, timeouts, socket closure, and I/O failures where these can be isolated without a live server;
- every `ServerCommand` handled by each specialised handler;
- bit-packed movement and entity updates at byte boundaries and cross-byte boundaries;
- unknown commands and malformed packets, preserving current logs and state effects.

For outgoing traffic, unit tests compare exact byte arrays and packet boundaries. For incoming traffic, unit tests compare ordered state changes, events, logs, and exceptions produced by the isolated handler. Live-server traffic is outside the automated test suite and may only be inspected manually.

### Manual Behaviour Verification

Manually inspect these scenarios at major phase boundaries without creating automated test artefacts:
- startup, splash completion, login success, every login failure, logout, reconnect, and shutdown;
- player and non-player-character movement, region changes, object updates, inventory, bank, shop, trade, duel, combat, prayer, spell, quest, chat, fatigue, and system updates;
- mouse movement, mouse buttons, keyboard input, menu construction, menu selection, camera movement, and window focus changes;
- loading success, absent content, invalid content, memory failure, and network failure.

Document the environment, inputs, server revision, observed screen transitions, and result for each performed check. Unit tests remain the only automated tests.

### Manual Visual Verification

Manually inspect fixed representative states on the approved graphics environment:
- splash, login, world loading, normal gameplay, every tab and overlay, menus, chat, inventory, bank, shop, trade, duel, combat, appearance, death, system update, and error states;
- representative terrain, walls, objects, items, players, non-player characters, animations, camera angles, clipping, and imported models;
- hover, pressed, selected, focused, disabled, empty, full, minimum, and maximum control states.

Do not create screenshot, framebuffer, draw-trace, graphics-device, or desktop-automation tests. Any observable difference blocks the associated change.

### Manual Audio Verification

Manually inspect asset selection, volume, loop state, and timing for representative interactions. Do not create audio-device or PCM-comparison tests.

### Data And Content Verification

For every repository, mapping extension, JSON file, localisation file, binary data file, and content asset:
- hash input files before and after each phase;
- unit-test deserialised object graphs and writable repository round trips where these remain isolated and platform-safe;
- validate identifier uniqueness, references, defaults, casing, and missing-value results;
- compare the compiled content manifest and asset dimensions;
- preserve GLB accessor offsets, strides, texture indices, winding, shading sentinels, and coordinate conversions.

### Public Surface Manifest

Generate an assembly API manifest containing all public and protected types and members, signatures, generic constraints, inheritance, interfaces, attributes, enumeration values, constants, and default parameter values. Compare the manifest after every phase as a build artefact, not a test project. Existing public fields remain fields unless an explicitly compatible forwarding mechanism is proven by the manifest and unit tests.

## Execution Rules

Each production change must:
1. Address one responsibility in one reviewable commit.
2. Add or retain characterisation coverage before moving logic.
3. Preserve the old entry point and public surface while delegating internally.
4. Use a branch-by-branch substitution, with the old implementation available for differential tests until equivalence is demonstrated.
5. Compare the old and new implementations with identical inputs where practical.
6. Pass all prior gates plus the phase-specific gate.
7. Contain no unrelated formatting, naming, dependency, asset, or data changes.
8. Identify the old and new owner of every moved responsibility and state mutation.
9. Report complexity, coupling, dependency, duplication, and changed-branch coverage deltas.
10. Pass the architectural acceptance gates as well as compatibility verification.

After each production change, execute the build, the complete `OpenRS.UnitTests` suite, static analysis, public API comparison, and data and content hash comparison. Perform the documented manual runtime checks at major phase boundaries. Any difference blocks the phase until resolved or explicitly removed from scope.

## Phase 0: Freeze The Reference

Deliverables:
- archive the approved executable production and runtime file manifest;
- record the public API, data, localisation, content, and produced-file manifests;
- record the passing `OpenRS.UnitTests` result;
- document the approved manual runtime environment and checklist without retaining secrets;
- establish repeatable commands for the build, unit tests, static analysis, manifests, and hashes.

Exit gate:
- the clean build and complete unit-test suite pass;
- repeated manifest and hash generation produces equal results from unchanged source.

## Phase 1: Confirm Unit-Test Coverage

Use only the existing `OpenRS.UnitTests` project without changing production behaviour solely to expose internals:
- retain and extend unit tests for pure data readers, mappings, repositories, settings, protocol framing, state transitions, and deterministic calculations;
- use test doubles at existing virtual or external boundaries only;
- do not create another test project or any non-unit-test category;
- do not add a live server, graphics device, audio device, window manager, network, or desktop dependency to unit tests.

Prioritise `BinaryDataReader`, `PacketConstruction`, `LoginEncryptor`, mapping extensions, repositories, `PacketHandler`, and every packet enumeration.

Exit gate:
- the complete unit-test suite passes from a clean checkout;
- every production slice selected for refactoring has direct unit coverage for its existing public and observable branches.

## Phase 2: Mechanical Hygiene

Apply behaviour-neutral corrections in isolated changes:
- eliminate unreachable commented legacy blocks only after confirming they do not participate in generated output or documentation;
- organise using directives and formatting one file at a time;
- replace duplicated literal protocol or layout values only when the replacement is an identical compile-time constant;
- improve private names only where reflection, serialisation, logs, and diagnostics prove independence from the name;
- retain all public names and all existing exception, logging, and null semantics.

Do not combine hygiene with extraction. This phase may be omitted where churn provides no material maintenance value.

Exit gate:
- the build, unit tests, manifests, hashes, and applicable manual checks remain equal.

## Phase 3: Data Access And Mapping

Refactor from the lowest-dependency layer upward:
- characterise each type in `DataAccess/DataObjects` and each repository in `DataAccess/Repositories`;
- isolate repeated repository construction and file-location logic behind internal collaborators;
- retain repository public types, save timing, ordering, default values, exceptions, and exact serialised production;
- simplify each mapping extension independently after proving property-by-property equivalence;
- add referential validation as test-only diagnostics; do not change runtime acceptance or correction behaviour;
- preserve all `Data` files and content copy rules unchanged.

Exit gate:
- all unit-tested object graphs and round-trip bytes are equal;
- all source data, localisation, content, and output hashes are unchanged.

## Phase 4: Protocol Primitives

Refactor protocol code without altering its external or wire contract:
- characterise every method and field in `BinaryDataReader`, `PacketConstruction`, `StreamClass`, `LoginEncryptor`, and related utility types;
- extract internal cursor or framing components only behind the current methods;
- preserve compact-packet byte swapping, extended lengths, buffering, flushes, counters, timeout progression, errors, and UTF-8 production exactly;
- preserve current malformed-input results, including current exceptions and partial state;
- retain `ClientPacket`, `ServerCommand`, `LoginCode`, `MenuAction`, and `MessageType` names and numeric values.

Exit gate:
- all protocol unit-test fixtures remain byte-for-byte equal;
- the public API manifest is equal.

## Phase 5: Incoming Packet Handling

Refactor one specialised handler at a time:
- characterise every command branch in movement and region, object state, mob update, trade and shop, duel and question, player state, and system handlers;
- extract command-specific private or internal handlers while retaining `PacketHandler.HandlePacket` as the entry point;
- preserve dispatch precedence because more than one handler accepting a command would currently select the first;
- retain ordered state mutations, event emission, logging, exception containment, and unknown-command behaviour;
- introduce a dispatch table only after tests prove unique ownership and identical precedence for every numeric command.

Exit gate:
- every isolated server-command unit fixture produces identical state, log, event, and exception results;
- the documented manual server check reveals no protocol or state difference.

## Phase 6: Outgoing Actions And Input

Refactor input and world interaction by user action:
- characterise keyboard, mouse, menu, camera, movement, combat, trade, duel, prayer, spell, inventory, and chat actions;
- separate input interpretation from action execution behind current entry points;
- preserve update order, repeat cadence, focus behaviour, hit regions, menu order, selected indices, path requests, and packet timing;
- retain all outgoing packet construction at its current semantic boundary until action equivalence is complete.

Exit gate:
- each isolated input unit test produces identical state, menu, and packet results;
- the documented manual input check reveals no visible or audio difference.

## Phase 7: Game State Extraction

Reduce `GameClient` incrementally without replacing it:
- inventory all fields by ownership domain: connection, player, mobs, world, inventory, bank, shop, trade, duel, combat, quests, prayers, spells, chat, camera, loading, and interface state;
- create internal state components one domain at a time;
- keep existing `GameClient` fields, properties, methods, and events as authoritative forwarding surfaces until compatibility is proven;
- move mutation logic only after all readers and writers for the domain are characterised;
- preserve array identity, allocation timing, aliasing, mutability, event order, and null behaviour where callers can observe them;
- do not replace fields with properties or collections with abstractions unless complete API and behavioural equivalence is proven.

Exit gate:
- API manifests are identical;
- unit tests preserve state values and reference identity where observable.

## Phase 8: Loading And World Construction

Refactor loading in dependency order:
- characterise data, media, texture, animation, model, map, sector, terrain, wall, object, and entity loading;
- extract narrowly focused loaders behind `GameLoader` and current public methods;
- preserve progress events, event order, cache keys, load order, error propagation, fallbacks, memory-error behaviour, and intermediate state;
- preserve GLB buffer-view offsets, accessor offsets, strides, indices, normals, winding, material mapping, coordinate conversion, and visible-face texture rules;
- leave incomplete loaders and pathfinding semantics unchanged.

Exit gate:
- unit-tested loaded object graphs, geometry buffers, texture selections, events, and logs are identical;
- manual loading and rendering checks reveal no difference.

## Phase 9: Rendering

Refactor rendering from leaf renderers towards `GameRenderer`:
- characterise each renderer's ordered draw commands and all state read from `GameClient`;
- extract pure calculations for projection, clipping, sorting, animation frame selection, colours, text, and coordinates without changing numeric types or operation order;
- retain the MonoGame and NuciXNA lifecycle, `SpriteBatch.Begin` and `End` placement, screen and control registration, blend state, sampler state, rasteriser state, and draw order;
- continue to use `GuiImage` and `GuiText` for NuciXNA controls and preserve dynamic layout calculations;
- refactor one visual subsystem per change: world, characters, items, interface, menus, inventory, bank, shop, trade, duel, chat, and overlays.

Exit gate:
- renderer unit tests and content manifests remain equal;
- manual visual and audio checks reveal no difference.

## Phase 10: Screens, Controls, And Lifecycle

Refactor `GameWindow`, screens, and controls only after rendering equivalence:
- preserve `Initialize`, `LoadContent`, `UnloadContent`, `Update`, and `Draw` ordering;
- preserve NuciXNA control registration and manager-owned control lifecycle;
- isolate screen composition from game-state orchestration behind existing screen types;
- preserve starting screen, transitions, focus, visibility, enabled state, z-order, update order, and disposal timing;
- preserve fixed timestep, vertical synchronisation, clearing, sprite-batch boundaries, window title, dimensions, and mouse visibility.

Exit gate:
- lifecycle and input unit tests remain equal;
- manual screen, visual, and audio checks reveal no difference.

## Phase 11: Managers, Models, And Utilities

Refactor remaining domains in small dependency-directed groups:
- game managers: entity, inventory, combat, quest, and related orchestration;
- models: construction, derived values, collections, path and sector state;
- utilities: text, geometry, camera, entity creation, and repeated calculations;
- audio, localisation, settings, and logging adapters.

For each group, first map all callers and all observable effects. Preserve singleton timing, static state, cache lifetime, collection order, numeric overflow, culture, casing, logs, and exceptions.

Exit gate:
- the build, complete unit-test suite, manifests, hashes, and applicable manual checks remain equal after every group.

## Phase 12: Dependency Direction And Composition

Only after the preceding extractions are stable:
- centralise internal construction where this does not change public constructors or initialisation order;
- introduce internal interfaces only at demonstrated test or ownership boundaries;
- eliminate cyclic dependencies one edge at a time using forwarding adapters;
- retain all runtime implementations, defaults, singleton identities, and initialisation timing;
- document the resulting dependency direction and ownership model.

Do not introduce a dependency-injection container unless a separate proposal proves that object identity, creation order, disposal, performance, and all compatibility traces remain identical.

Exit gate:
- all automated non-runtime checks and unit tests are equal;
- a clean checkout can restore, compile, and execute the unit-test suite with one documented command.

## Phase 13: Profile-Guided Performance Optimisation

Commence this phase only after structural refactoring and all architectural gates are complete. Optimise one measured constraint per change:
- profile representative deterministic scenarios and rank constraints by user impact and measured cost;
- establish a focused benchmark and declare its primary metric, secondary metrics, noise floor, and compatibility fixtures before editing production code;
- prefer algorithmic and allocation improvements that preserve operation order and ownership boundaries;
- verify retained caches, buffers, pooled resources, graphics resources, and event subscriptions across repeated lifecycle scenarios;
- execute the complete unit-test suite and repeat the declared profiling measurements after each optimisation;
- document rejected candidates where the benefit is insignificant, unstable, architecture-damaging, or incompatible.

Candidate investigation areas, subject to profiling evidence, include:
- repeated packet parsing, dispatch lookup, and temporary packet allocations;
- repeated data mapping, string conversion, localisation lookup, and asset lookup;
- per-frame collection allocation, enumeration, sorting, projection, clipping, and menu construction;
- world, entity, inventory, chat, model, texture, and geometry work repeated without a state change;
- loading I/O patterns and duplicate decoding that can be consolidated without changing order or errors;
- resource retention, event subscriptions, and buffers that increase long-session memory or collection pressure.

Exit gate:
- every accepted optimisation demonstrates a repeatable material improvement in its declared primary metric;
- no other performance budget regresses beyond its declared noise floor;
- every compatibility and architectural comparison remains equal;
- added complexity is documented and proportionate to the measured benefit;
- performance budgets are tightened to the accepted results where the improvement is stable.

## Phase 14: Final Consolidation

Perform no functional refactoring in this phase. Instead:
- execute the complete unit-test suite repeatedly on the approved environment;
- manually execute a reference-server session covering login, movement, interaction, combat, inventory, bank, shop, trade, duel, chat, logout, and reconnect;
- compare API manifests, data and content hashes, logs, produced files, and documented manual observations;
- audit every changed public symbol, protocol constant, serialised field, asset reference, and lifecycle method;
- update architecture documentation to describe only the verified implementation.

Exit gate:
- every automated non-runtime comparison and unit test is equal;
- the manual runtime checklist records no observable difference;
- no compatibility waiver remains;
- the complete project compiles without new warnings;
- all unit-test and non-runtime verification artefacts are reproducible from a clean checkout.

## Recommended Change Order

Within each phase, use this order:
1. Pure readers and calculations.
2. Data mappings and repositories.
3. Protocol framing and parsing.
4. Specialised packet handlers.
5. Input actions and packet emission.
6. Domain state extraction from `GameClient`.
7. Loading and world construction.
8. Leaf renderers, then rendering orchestration.
9. GUI controls, screens, and game lifecycle.
10. Composition and dependency direction.
11. Profile-guided performance optimisation.

Never execute two adjacent items concurrently when they share mutable `GameClient` state or protocol traffic.

## Risk Register

| Risk | Consequence | Mandatory Control |
| --- | --- | --- |
| Incomplete unit coverage | Silent regressions | Select only production slices with direct unit coverage and extend unit tests first where necessary |
| Mutable public fields | API or aliasing incompatibility | Preserve fields and compare API plus reference identity |
| Compact packet framing | Server disconnect or corrupt commands | Byte-level unit tests at every length boundary |
| Packet dispatch precedence | Incorrect state mutation | Characterise command ownership before table dispatch |
| Bit-packed updates | Position or entity corruption | Cross-byte fixtures and recorded packet replays |
| Global singleton state | Changed initialisation or lifetime | Lifecycle traces and identity assertions |
| Rendering operation order | Pixel or layering differences | Renderer unit tests plus documented manual visual inspection |
| Floating-point reordering | Camera or geometry differences | Preserve numeric type and operation order |
| Content identifiers and casing | Missing runtime assets | Manifest and case-sensitive path verification |
| Repository ordering or defaults | Changed game data | Object-graph and exact round-trip comparisons |
| Asynchronous networking | Changed packet timing or races | Isolated transport unit tests plus documented manual server verification |
| Legacy incomplete code | Accidental behaviour expansion | Exclude repairs and retain current results |
| Dependency upgrades | Unrelated runtime differences | Freeze packages and framework during refactoring |
| Unmeasured optimisation | Complexity without material benefit | Require profile evidence and a reproducible benchmark |
| Benchmark noise | False improvement or regression | Use repeated runs, declared noise floors, and confidence intervals |
| Cache or pool lifetime | Retained memory, stale state, or aliasing | Bound ownership and test invalidation, reset, disposal, and repeated lifecycles |
| Parallel execution | Reordered state, packets, draws, or exceptions | Prohibit unless deterministic equivalence and thread affinity are proven |

## Completion Criteria

The programme is complete only when:
- all production source files have been classified as refactored, intentionally retained, generated, or excluded with a reason;
- every changed reachable client and server packet path has direct unit coverage;
- every public and protected assembly member appears unchanged in the API manifest;
- all runtime data, localisation, content, and output-file hashes match the baseline unless a file contains test-only additions;
- all unit-tested deterministic behaviour and lifecycle scenarios remain equal;
- the final documented manual runtime, visual, audio, and reference-server checklist records no observable difference;
- every mutable state category has one documented authoritative owner;
- the implemented dependencies conform to the target architecture and static dependency analysis detects violations;
- no renderer, control, repository, or packet decoder owns unrelated game-state mutation;
- `GameClient` is a documented compatibility facade and lifecycle coordinator rather than the default dependency for extracted code;
- no unexplained dependency cycle, duplicated policy, service locator, new singleton, or new mutable global state remains;
- all substantially refactored code satisfies the complexity limits or has an approved architecture decision record;
- every touched area demonstrates reduced complexity, coupling, duplication, mutable-state exposure, or untested branching without increasing another measure without rationale;
- changed production branches have direct deterministic unit tests;
- the architecture overview, state-ownership catalogue, compatibility-shim catalogue, and architecture decision records describe the final verified structure;
- controlled benchmarks demonstrate no unexplained performance regression versus the approved baseline;
- every accepted optimisation has reproducible profile evidence, a material improvement, exact compatibility evidence, and proportionate maintenance cost;
- startup, loading, frame pacing, update, draw, allocation, garbage collection, memory, packet processing, and stress-scenario observations are documented without creating performance tests;
- the clean build succeeds and the complete compatibility suite passes;
- no phase relies on compilation alone or an undocumented compatibility exception.

The strict rule is simple: when equivalence cannot be demonstrated, retain the existing implementation.