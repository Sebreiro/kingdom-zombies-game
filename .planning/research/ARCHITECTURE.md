# Architecture Research

**Domain:** Godot/C# side-view survival and camp management; solo and four-player cooperative LAN
**Project:** KingdomZombies, complete version 0.1
**Researched:** 2026-11-13
**Confidence:** MEDIUM

## Standard Architecture

### System Overview

Use a compact modular game with one authoritative host, an engine-independent C# core, and Godot adapters for input, collision, presentation, networking, and storage. Solo uses the same host command path with an in-process connection. Clients submit intentions and display replicated state; they never commit belongings, damage, recruitment, payments, succession, or campaign outcomes.

This is a recommendation grounded in the project's ownership requirements and official engine constraints, not evidence of a working implementation. The repository currently has starter settings, not gameplay. Its `4.7` feature tag is not a validated engine/toolchain pin. Phase 1 must select and prove a compatible Godot .NET editor, SDK, compiler, analyzers, and export templates before treating the architecture as executable.

```text
                  Participating host / solo process
┌───────────────────────────────────────────────────────────────┐
│ Godot application root: boot, menu, session, explicit wiring    │
│ Local input ──┐                    ┌── UI, animation, audio     │
│ LAN adapter ─┼─> validated commands│   and equipment views     │
│ NPC intents ─┘                    ↑                           │
├───────────────────────────────────┼───────────────────────────┤
│ C# GameCore: single writer        │ read-only state/deltas     │
│ World clock + scheduler → command/transaction processing      │
│ Actors / inventories / camp / jobs / campaign / ownership      │
│ Stable IDs + state revisions + explicit saved commitments     │
├───────────────────────────────────┼───────────────────────────┤
│ Godot collision adapter ↔ host tick coordinator               │
│ Config/content validation → immutable effective catalog       │
│ Snapshot capture → file writer / network snapshot sender      │
└───────────────────────────────────────────────────────────────┘
                      ↕ ENet session transport
┌───────────────────────────────────────────────────────────────┐
│ Up to three guests: input → commands; snapshots → local views │
│ Local cameras/settings; replicated world; no gameplay writer  │
└───────────────────────────────────────────────────────────────┘
```

Godot's multiplayer API provides ENet peers, authority declarations, transfer modes, and channels. Those are transport facilities; application transaction and join consistency remain our responsibility. Keep RPC endpoints in a small session adapter instead of scattering them through gameplay actors. [Godot high-level multiplayer](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html)

### Component Responsibilities

| Component | Responsibility | Typical implementation |
|-----------|----------------|------------------------|
| Application/session root | Boot validation, menu transitions, host/client/solo lifetime, dependency composition and teardown | A small Godot root scene and explicit C# construction/binding |
| GameCore world | Stable actor/item/building/portal IDs, authoritative mutable state and invariant checks | Plain C# classes; no Godot, sockets, file paths, UI, or platform account types |
| Command processor | Sender permission, control ownership, current-state validation, deduplication, ordering and transaction commit | One host queue drained at explicit tick boundaries; local host goes through it too |
| Inventory/economy | Item parentage, grids, equipment, split/merge, consumption, payments, transfer and legacy removal | One transaction service over the shared ownership graph |
| Actor/combat systems | Health/stamina, movement intent, damage, wear, ammunition, death, successor reservation | Tick-driven state changes; host collision results enter as data |
| Camp/work systems | Recruitment capacity, roles, work reservations, paid commitments, cargo, safety, buildings and defenses | Explicit job/order records and interruptible finite states |
| Campaign scheduler | Time/day, once-only daily actions, waves/queues, portal defense/retaliation, retreat and outcomes | Persistent scheduled records with stable IDs and due ticks/progress |
| Godot world adapter | Collision bodies, terrain query, actor view lifetime, animation, equipment and audio | Scenes project state; only host physics participates in authoritative outcomes |
| Session services | Discovery, endpoint selection, connection lifetime and local identity mapping | LAN implementation behind small replaceable ports |
| Replication | Protocol validation, initial baseline, revisions/deltas, transient effects and resync | Explicit DTO codec; separate reliable transactions and frequent movement updates |
| Persistence/configuration | Versioned save envelopes, safe writes, validation, migrations and changed-settings reconciliation | Files and explicit mappers outside the core; no database requirement |
| Verification tooling | Case selection, process/input driving, bounded observation, logs/images/results | External runner plus non-shipping test adapter; production rules are unchanged |

## Recommended Project Structure

Start with two production assemblies, not LangProj's fifteen-project server layout. Keep Godot-attached scripts in its game project to simplify editor discovery and imports. Add a separate assembly only when a demonstrated dependency or tooling boundary needs one.

```text
AGENTS.md                         # short project description and task-based links
docs/
├── CodeRules/                    # inherited/adapted style and architecture rules
├── rule-matrix.md                # mechanisms, scopes, exemptions, positive/negative proofs
├── decisions/                   # toolchain, graph, authority, persistence, testing decisions
├── testing/                     # case catalog and agent operating procedure
└── content/                     # designer schema and provenance documentation
src/
├── KingdomZombies.Core/
│   ├── Model/                   # identities, state, definitions and invariants
│   ├── Simulation/              # tick systems, jobs, combat and campaign
│   ├── Commands/                # validated operations and atomic transactions
│   ├── Contracts/               # explicit state/command/result/snapshot data
│   └── Ports/                   # only real external seams
└── godot-game/
    ├── project.godot
    ├── Bootstrap/               # composition and lifetime
    ├── Input/                   # remapping, focus contexts, intent generation
    ├── World/                   # collision, actor views, authored-map adapter
    ├── Presentation/            # UI, animation, equipment, audio, cameras
    ├── Sessions/                # RPC gateway, LAN discovery/identity/transport
    ├── Persistence/             # snapshot codecs and file lifecycle
    ├── Configuration/           # parsing and reference validation
    ├── scenes/                  # boot/menu/session/world/reusable views
    ├── assets/                  # shipped normalized art/audio/fonts
    └── content/                 # map, config domains, animation data, manifests
tests/
├── Architecture/                # Phase 1 rules and real-build proofs
├── Core/                        # invariants, replayable domain sequences
├── Engine/                      # scene/lifecycle/input integration
├── Multiplayer/                 # real host plus three guest processes
├── Cases/                       # A-01..A-47 and foundation/regression catalog
└── Fixtures/                    # versioned setup inputs and baseline saves
tools/verification/              # local/CI launcher and evidence collection
packages/                       # selected analyzer sources/packages and their tests
```

### Structure Rationale

- **Core → System only.** Keep one physical assembly initially, with namespace/module rules and a small public command/snapshot API. No scene nodes, `Variant`, transport peers, renderer resources, file I/O, or dependency resolution in its rules.
- **Godot game → Core.** Composition can construct concrete core types directly. Within the game assembly, enforce namespace edges: UI/input call session-facing commands and read views; they do not call mutation internals or file/network APIs. Infrastructure implements ports, never controls a hidden second world.
- **Tests → appropriate production assemblies.** Architecture tests inspect evaluated references, source dependency violations, generated-code boundaries, documentation links, and shipping manifests. Test runners and fixtures must not become runtime dependencies of the release export.
- **Ports at changing boundaries.** Session discovery, local identity, connection transport, snapshot storage, geometry observations, and controlled clock/randomness are meaningful seams. Do not create an interface and `.Abstractions` project for every class or reproduce ASP.NET registration patterns.
- **Self-contained inputs.** Bring selected analyzers and recovered adaptations into this repository with provenance. CI must not depend on `D:/Work/SaaS` or `D:/Work/Games/KindomCrown` being present.

## Architectural Patterns

### Pattern 1: Engine lifecycle outside domain construction

**What:** Ordinary C# domain objects have explicit validated construction. Engine-created Nodes/Resources follow Godot construction and scene lifecycle; their exported values describe references/presentation, not unvalidated authoritative rules.

**When:** Every attached C# script, scene spawn, exported reference, or async resource operation.

**Trade-offs:** A little explicit binding avoids global service lookup and makes teardown and tests reliable.

Godot-derived C# types require `partial` for source generation. Exports accept Variant-compatible values and may be private, so a rule requiring public mutable gameplay properties is unnecessary. Use nullable exported references followed by explicit scene validation; do not scatter null-forgiving operators to silence initialization warnings. [GD0001](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/diagnostics/GD0001.html), [C# exports](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html)

Create/bind dynamic scenes before adding them to the active tree, or keep processing disabled until binding completes. Do not assume a parent `_Ready` initialized children: child `_Ready` runs first. Resolve scene references in the appropriate ready stage, start the session explicitly after validation, and disconnect subscriptions/cancel pending work on teardown. Reentering a tree does not automatically repeat `_Ready`. [Node lifecycle](https://docs.godotengine.org/en/stable/classes/class_node.html)

Keep active scene-tree and rendering mutations on the main thread. Background work may serialize an immutable snapshot, compute on detached data, or perform file I/O; marshal completed results through a queue and discard results for a closed session generation. `Task.Run` is not permission to access Nodes. [Godot thread-safe APIs](https://docs.godotengine.org/en/stable/tutorials/performance/thread_safe_apis.html)

### Pattern 2: One writer, explicit tick order, deterministic rules where useful

**What:** A host tick coordinator owns simulation advancement and mutation order. Commands, NPC intentions, movement/collision feedback, combat, world transitions, and snapshot publication have explicit stages.

**When:** Movement from Phase 2 onward; shared ownership from Phase 3 onward; all later gameplay.

**Recommended sequence:** drain validated commands → resolve ownership/orders and movement intentions → advance host collision bodies → record collision/position facts → resolve combat/death → advance scheduled work/campaign transitions → resolve outcomes → publish a committed revision and any requested snapshot. Specify same-tick tie rules, including the spec's simultaneous victory/defeat behavior. Do not let Godot sibling processing order choose economic winners or outcomes accidentally.

The Godot collision adapter is authoritative only on the host. At a commit boundary the core holds canonical positions and combat state; scene body transforms are engine working data between boundaries. Clients interpolate host results. Start with server authority and interpolation; add client movement prediction/reconciliation only if Phase 3 measurements justify it, without allowing predicted motion to award damage or loot.

Use fixed simulation steps, stable iteration/tie ordering, integer quantities/currency, explicit duration units, and an owned PRNG with versioned saved state or precomputed event commitments. Separate cosmetic randomness. Unit/domain scenarios can use supplied ticks and seeds. Engine physics/network timing are not assumed bit-identical across Windows/Linux; this is not deterministic lockstep. Compare semantic state at an agreed revision and use appropriate movement tolerances.

**Trade-offs:** Single-threaded authority makes simultaneous actions tractable at four players. If profiling later requires parallel work, workers return proposals over immutable input; only the host writer commits them after revalidation.

### Pattern 3: Atomic inventory and paid-work transactions

**What:** All belongings use one ownership graph, not independent copies in the stash, gear renderer, NPC cargo, corpse, or client UI. An item instance has exactly one location; parent/container relationships are acyclic.

**When:** Moves, rotation, stacking, splitting, equip/unequip, food use/sales, ammo expenditure, recruitment/job payments, corpse creation, hauling and construction.

A command carries a session epoch, command ID, client sequence, target IDs and relevant expected revisions. The host derives the player from the connection, verifies the sender's controlled actor/permissions, validates the entire proposed operation against current state, then commits all touched locations/counters and a result together. Failed operations change nothing and return a reason plus current relevant state. A duplicate command returns its prior result within the session's bounded deduplication window; obsolete session commands are rejected.

```text
Take last bow:
  read current source + destination + access constraints
  validate actor, item ownership, grid fit, weight and nesting
  prepare source removal + destination insertion as one change
  commit both and increment revisions
  publish accepted result, inventory delta and appearance change
  competing stale take receives conflict; no second bow exists
```

Reserve recruitment capacity and its payment in the same transaction. Accepted building orders store the purchased tier/work quantity or authorized repair amount, amount paid, remaining work and target identity. Workers contribute saved progress; price changes, rejoin, interruption, and load cannot purchase the same order again. Item death transfer changes parentage into the corpse atomically; cosmetic death animation cannot create loot. Structural tower guards never own a normal survivor inventory or spawn corpse loot.

**Trade-offs:** Explicit transaction preparation adds code, but prevents multi-step rollback defects. A database, distributed transaction library, or full event-sourced world is unnecessary. Keep a useful diagnostic command/event journal; snapshots remain the recovery source.

### Pattern 4: Versioned state plus transient effects

**What:** Persistent facts and one-shot presentation notifications have distinct contracts.

**When:** Late join, loading, reconnecting, scene recreation, pooling and animation.

State contains actor life/death, gear, portal/building state, work progress, warnings and their remaining time, and exploration. It reconstructs appearances and ongoing loops. Transient effects carry an event ID and source revision for a sound, hit flash, particle burst, or purchase acknowledgment. Rebuilding a snapshot must not replay historic death, destruction, payment, or warning-start effects. Views are rebuildable from state; deleting a view cannot delete a nonempty corpse's inventory.

`MultiplayerSynchronizer` cannot synchronize arbitrary `Object`/`Resource` properties or peer-specific instance IDs. Use stable application IDs and explicit DTOs for inventory/campaign state. If synchronizers/spawners are used for primitive movement/view state, assign one replication owner per field and test ordering with the snapshot path. [MultiplayerSynchronizer](https://docs.godotengine.org/en/stable/classes/class_multiplayersynchronizer.html)

### Pattern 5: Snapshots established early, full save experience later

**What:** Capture a coherent immutable world at a committed tick. Map it to a versioned save envelope or join baseline without walking mutable Nodes on a worker thread.

**When:** Identity and small state contracts begin in Phases 2–3. Every domain feature in Phases 6–8 adds snapshot round-trip coverage. Phase 9 completes manual/dawn/exit saving, compatibility and recovery UX.

The save envelope records schema version, run ID, content/map version, effective-config fingerprint, revision/tick, and all required state. Include nested item placements, legacy layout metadata, current/remembered roles, player associations, paid commitments, task/cargo state, world phase progress, daily-event completion, wave generation/spawn progress, pending warnings, random state, portal destruction and corpse cleanup timers. Capture no Node reference, engine instance ID, callback, coroutine, or live task.

Serialize off-thread after capture; write a new temporary file, flush and validate it, then promote it using a platform-tested same-directory replacement strategy. Retain the last valid save and only apply retention after successful promotion. Capture time and write time are different; a save confirmation refers to its captured revision. Do not promise power-loss durability without testing the actual filesystem strategy.

For host exit, finish the coherent save and report failure before closing; offer retry/return to play/explicit exit without latest progress as specified. Hosts alone request save/load. Queued save requests need bounded/coalesced handling so repeated clicks cannot exhaust memory while play continues.

### Pattern 6: Legacy layouts are first-class saved data

**What:** Separate an item's current behavior definition from the saved representation needed to preserve its placement.

**When:** Loading after dimensions, stack capacity, nesting or inventory grids change.

Retain saved positions, rotation, display footprint, old grid boundaries, and containment where required. Load and show legacy locations; permit removal and legal-sized stack extraction, not new invalid insertion. Permit extracting a filled container as a unit only when its new outer placement is legal and it does not worsen invalid nesting. Once emptied, a legacy container adopts current rules. Persist unresolved legacy metadata again on subsequent saves. Missing required definitions or corruption produce an explicit error and preserve the original file.

Configuration reconciliation is an explicit transformation after decoding, before activation: lower maxima clamp current values; higher maxima do not heal; saved day/phase fraction survives duration changes; future generation uses current progression; already paid/generated/queued commitments survive; over-cap populations remain and block new additions. Load is not a dawn transition.

## Data Flow

### Request Flow

```text
Keyboard/mouse → Godot InputMap/focus → local intent/command
    → session gateway → host current-state validation
    → atomic core commit → state revision + result + effects
    → local/remote read models → world views and UI
```

An inventory panel has local selection/drag state and an authoritative backing view. It may show a pending operation immediately but may not count it as committed ownership. Replicated acceptance/rejection updates every open view, including NPC changes. UI input focus must suppress the relevant gameplay intents explicitly: merely accepting a GUI event does not disable global `Input` state polling. [Godot Input](https://docs.godotengine.org/en/stable/classes/class_input.html)

### State Management

```text
Validated content + seed → new world
Stored snapshot → decode → schema migration → config reconciliation → world
World → committed read model → UI/rendering
World → immutable captured snapshot → file or joining-client baseline
```

Use stable run/player/actor/item/container/building/portal IDs independent of scene paths, display names, LAN addresses and transport peer IDs. A reconnect credential or equivalent session identity proof binds the stable local player identity to its connection; a client must not gain another survivor by claiming a display name. Platform identity can later map onto this boundary without becoming the saved owner ID.

### Key Data Flows

1. **Late join:** check protocol/content compatibility and slot availability, including spectators; obtain host effective settings; capture baseline revision R; transfer bounded chunks while buffering later deltas; instantiate client views; apply only contiguous deltas after R; acknowledge readiness at revision C; then atomically assign an eligible survivor or spectator status and permit actions. Existing players continue. Overflow/gap/timeouts trigger bounded restart or failure, never world rollback. The current baseline and catch-up protocol must include corpses, jobs, queues and exploration, not just spawned actors.
2. **Network channels:** use reliable ordered commands/results and committed inventory/campaign deltas; sequenced frequent movement input/state can use unreliable delivery with stale-sequence rejection; snapshot transfer has its own bounded stream/channel so it does not delay normal actions. Define explicit baseline/delta revisions because ordering is not global across channels. Validate payload sizes/counts/enum values and sender ownership before accepting them. No arbitrary object deserialization.
3. **Guest disconnect:** release control once, remove crown, preserve inventory and remembered job, return survivor to NPC control. Reconnect restores the previous survivor only if alive and free; otherwise use normal succession/spectating. Simultaneous succession uses the same reservation writer. Host loss closes the session; no migration path is needed.
4. **Day transition:** persist the day/phase and completion markers with the refill, wildlife, retreat, stage update and autosave request. Resuming or joining reads current state rather than invoking the transition again. Persist wave commitments and spawn counts so queue resumption does not reroll loot or double spawns.
5. **Content:** recovered/generated inputs → normalization and provenance manifest → stable content IDs → validated Godot imports and appearance bindings → read-only runtime catalog. Separate collider/interaction geometry from decorative sprites. Equipment appearance derives from actual equipped instance/definition state, including corpse changes. Definitions own gameplay values; scenes own presentation references; do not maintain competing balance numbers in both.
6. **Configuration:** parse separate documented domains at new-run/load boundaries, including independent zombie progression; validate ranges, cross-references and content versions before activation. Errors identify file/field/value/reason. Local controls/audio/video preferences stay local; guest balance files never override the host's effective catalog. Include non-resource JSON/config files explicitly in export verification.

## Scaling Considerations

The meaningful scale is four players and a dense persistent world, not millions of accounts.

| Scale | Architecture adjustments and evidence |
|-------|---------------------------------------|
| Phase 2: one actor | Establish input→command→host simulation→view path, stable identity, simple snapshot shape; basic visible launch/movement smoke |
| Phase 3: host + three guests | Real processes, independently controlled actors, bounded input queues, sequence/revision protocol, disconnect cleanup; verify actual network path |
| Approximately 100 survivors + 300 active zombies | Spatial/side-based candidate indexes, stagger slow work/equipment decisions, bounded spawn queues, batched dirty deltas, avoid full inventory replication per frame |
| Three-hour saved/resumed session with 200+ nonempty corpses/piles represented | Persist ownership as compact records; virtualize stash UI and nonvisible presentation; monitor serialized size, join catch-up, allocations and open-panel refresh; never delete belongings to meet a budget |

### Scaling Priorities

1. **First likely bottleneck: NPC targeting/work scans and layered rendering.** Profile candidate searches, physics bodies, animation/equipment draw cost and allocations. Use side-view spatial queries and changed-state updates before considering an ECS rewrite. Logical off-screen activity must continue where it affects the campaign.
2. **Second likely bottleneck: snapshot capture/replication as persistent loot grows.** Measure capture separately from serialization and transfer. Keep captures coherent and brief; if copying exceeds the budget, introduce measured immutable pages/copy-on-write at domain boundaries rather than inconsistent incremental reads. Bound joining-client buffers and backpressure.
3. **Reference-PC acceptance is a dedicated rendered test.** Record CPU/GPU/RAM/OS/build/config/settings. At 1920×1080 and the specified four-player crowd, the proposed gate is at least 30 displayed FPS in 95% of one-second samples over ten minutes, no sub-30 period longer than five seconds, and no save/join active-play freeze longer than 0.5 seconds. Record spikes separately. Do not disable required AI, gear, inventories or combat. Hosted CI throughput is not this measurement.
4. **Network acceptance includes real machines.** Loopback processes validate serialization and logic; they do not prove Windows/Linux LAN discovery, firewall behavior, actual latency or rendering under the target setup. Keep a repeatable mixed-OS LAN procedure and evidence record.

## Automation Architecture

### Phase ownership and operating model

Phase 1 implements analyzer/architecture proof tests. Phases 2 and 3 include basic launch/input/connection smoke checks. Phase 4 makes the complete content inspectable with fixtures. **Phase 5 builds the comprehensive runner, CI, stored catalog and evidence workflow**; do not move that framework earlier or defer it until release.

At Phase 5 register all A-01 through A-47 with requirement, owning phase, setup, production actions, assertions, evidence and environment. Foundation cases are active; future cases are explicitly planned, not passing or silently skipped. Each later feature activates its cases. Release aggregation rejects missing required evidence, undiscovered tests, unexpected skips, unavailable engines, timeouts or environment failures.

### Verification layers

| Layer | Executes | Evidence and limitations |
|-------|----------|--------------------------|
| Rule proofs | Positive/negative analyzer and architecture fixtures; targeted real builds | Proves enforcement actually fails bad code, including narrow Godot exceptions |
| Domain | Real command processor with seeded state and explicit ticks | Ownership conservation, atomic failure, command duplicates, event ordering, save round trips; does not prove physics/UI |
| Engine/scene | Actual Godot scene lifecycle, imports, physics and input callbacks | Scene load, missing reference, input gating and host geometry assertions |
| Process multiplayer | Host plus three distinct guest processes, same production protocol | Concurrent operations, independent movement, join/reconnect/host-loss and per-peer revision evidence |
| Rendered input | Real menus, keys/mouse, remapped actions, dragging, focus, camera and screen sizes | Screenshots/video plus assertions; validates visuals and input paths headless runs cannot |
| Release environments | Exported Windows/Linux builds, actual LAN and creator-PC rendering | Full fresh campaigns, pacing, performance and three-hour saved/resumed endurance |

### Agent-operated production input

The external runner launches an isolated session, drives keyboard/mouse events through production controls, observes expected predicates with deadlines, and collects results. For repeatable in-engine tests, a non-shipping adapter may call `Input.ParseInputEvent` with key/mouse events so they traverse normal mapping, GUI routing and gameplay code. It must not call inventory handlers directly and label that end-to-end. Godot documents that parsed events do not control OS cursor/focus, so exported-build cases also need real OS input/focus checks. [Input event injection](https://docs.godotengine.org/en/stable/classes/class_input.html#class-input-method-parse-input-event)

Keep test observation read-only: semantic actor/item/revision data, diagnostics and frame captures. Exclude the driver endpoint and fixture loaders from shipping exports and prove the exclusion. No release RPC can teleport actors, award items, force a victory, bypass transactions or replace the authoritative world. Test code may prepare unit fixtures; it must never masquerade as the production command path.

Version fixtures by case, schema/content/config hashes and seed. For focused integration tests, load a valid fixture through the production save/new-run loader, then exercise the target behavior through production input. Record fixture provenance and what setup it shortcuts. Fresh-campaign release gates start from the actual menu/new run and reach victory without development commands, save editing or injected items. Stress setup fixtures are labeled performance fixtures, not evidence of completing a normal campaign. Seeds control gameplay generation; clocks can accelerate domain tests, but pacing/render-performance/endurance checks use their specified real-time conditions.

### Reliable orchestration and evidence

- Give each run a unique directory, identity set, save/config root, bounded port allocation, seed and session ID. Never share a mutable `user://` profile among four processes.
- Readiness predicates identify the actual role/session/revision; a process spawning is not proof it connected. Use bounded waits for menu readiness, connection, world baseline, acknowledgment and cleanup instead of unbounded sleeps.
- Capture stdout/stderr and structured events per peer with tick/revision/command ID, exit status, configuration hashes, screenshots at named checkpoints and on failure, and the seed/setup/actions needed to reproduce. Preserve last-known host/client state summaries on timeout.
- Compare authoritative semantic inventory/ownership at a common acknowledged revision. A screenshot or equal entity count alone is not consistency proof. Check unique item locations, amounts spent, rejected conflicts and absence of replayed effects.
- Cleanup in `finally`: release pressed inputs, request normal process shutdown, wait with a deadline, then terminate only child processes owned by this run and release its ports/files. Retain failure artifacts. CI must report missing prerequisites and cleanup failures explicitly.
- The agent procedure is select impacted cases → run the documented local/CI entry point → inspect machine assertions and rendered evidence → reproduce with recorded seed/actions → fix → rerun relevant cases and required gates. Image capture without inspection is not visual acceptance.

Godot supports `--headless` with editor or exported binaries. Use it for appropriate integration/network assertions, while retaining rendered cases. A dedicated-server export is an optional testing/deployment artifact, not a new version 0.1 product requirement. [Headless execution/export](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_dedicated_servers.html)

## LangProj Inheritance and Godot Compatibility

The inspected LangProj resources demonstrate enforceable rules, not a game architecture to copy. Preserve the rule-matrix method: every retained/adapted rule names scope, mechanism and both good/bad proofs; changes update documentation, enforcement and proofs together. Use a short root `AGENTS.md` with focused linked documents and normalize the existing filename casing without creating Windows/Linux collisions.

| Reference rule family | KingdomZombies recommendation | Phase 1 proof |
|-----------------------|-----------------------------|---------------|
| Warnings as errors, nullable, format, named arguments, short members, docs and custom SCR rules | Retain applicable rules; pin compatible compiler/analyzers rather than inheriting `net10.0`/`latest` blindly | Good and violating compilations; actual build failure for representative diagnostics |
| Generated-code and suppression controls | Scope exclusions to genuine generated output; handwritten partial scripts remain checked | Real Godot generator build passes; handwritten violation in a partial script fails; false generated markers fail |
| Domain purity and dependency graph | Core remains engine-free; explicit evaluated reference table and namespace rules | Synthetic Godot/transport leak fails; direct/transitive reference leak proofs |
| Entity constructors/private setters | Retain validated plain C# state mutation; omit EF-only private obsolete parameterless constructors | Invalid domain state fails; scene/resource instantiate/load works with intended construction |
| Godot exports and nullable references | Explicit nullable exports and required-reference validation; Variant-compatible adapter properties | Valid scene succeeds; missing/wrong reference produces clear startup failure; no blanket `!` exemption |
| Public types and engine callbacks | Scope public/partial/override requirements to engine-attached types and callbacks | Actual attached script builds, instantiates and receives callbacks under all analyzers |
| Interface-only sibling projects/per-layer DI registration | Adapt to small boundary ports and explicit composition; no blanket abstraction projects or service locator | Forbidden dependency/service-resolution use fails; replacement session service fixture works |
| HTTP DTO names, EF enums, PostgreSQL, health endpoints | Inapplicable transport/database rules are recorded as such; retain explicit stable serialized contracts and validation | Save/wire contract compatibility and invalid-input proofs, not HTTP-host tests |
| Wall clock/units | Separate UTC save metadata and monotonic timeout time from simulated ticks; explicit units | Simulated day/duration changes and timeout behavior are independently testable |

Inspect source generator diagnostics alongside inherited analyzers, not after gameplay exists. The inspected `NullForgivingAnalyzer` already opts out of generated-code analysis; verify equivalent behavior for every inherited rule. Engine editor examples are not automatically compatible with this project's strict style. Test script filename/class discovery, resource export/import, callback signatures, missing nodes, teardown/reentry, and main-thread boundaries on the selected pinned toolchain.

## Recovered Logic: Reuse Candidates and Boundaries

These are source-inspection findings, not validated ports. Preserve original files; selected adaptations get source path/hash, purpose, dependency removal notes, spec differences and tests. Start from the smallest useful algorithm.

| Inspected source under `recovered/source/Assembly-CSharp` | Useful candidate | Required adaptation / warning |
|------------------------------------------------------------------------|------------------|-------------------------------|
| `JobAssigner.cs` | Cost-matrix assignment with row reduction, starred/primed zeros and augmentation | Remove Unity debug dependency and static mutable `pathLength`; encapsulate buffers; test empty/rectangular/tied costs, invalid work eligibility and deterministic ties; benchmark before running frequently |
| `Actor.cs` | Availability, assignment and cost responsibilities | Retain responsibility split as pure actor/job policy; do not port `MonoBehaviour` inheritance |
| `StateMachine.cs` | Explicit states, transition priorities and interruption | Reexpress persisted task state/timers; do not copy coroutine reset via reflection over compiler field `<>1__state`, Unity `Time.time`, or static current-machine state |
| `KingdomTimer.cs` | Persisted duration/progress and action identity | Replace singleton manager/global-save mutation with scheduler-owned records and once-only event IDs |
| `WorkableBuildingData.cs` | Separate persisted construction progress | Expand into paid work commitments and worker-seconds; preserve new game's repair and interruption rules |
| `Wave.cs` and `Portal.cs` | Data-defined spawn composition, spawn queues, authority checks and visual state separation | Use seven-zombie progression, permanent destruction, cap/queue commitments and damage-triggered defense; original proximity spawns/rebuilding/campaign rules conflict with this spec |
| `WaveController.cs` | Water-wave presentation only | Name is misleading for campaign research; it is not the enemy-wave scheduler |

Avoid copying `Managers`/`NetworkBigBoss`/`NetworkPostbox` coupling, Unity persistence components, scene-wide searches or platform code into the core. Do not infer four-player support or physical nested inventories from original-game networking. A candidate that needs a large web of managers is usually a behavioral reference, not a direct port.

## Anti-Patterns

### Anti-Pattern 1: Scene properties are the authoritative inventory

**What people do:** Replicate every actor's scene fields and save the scene tree.
**Why wrong here:** Multi-container operations become partial, instance IDs are unstable, and corpses/items disappear when presentation unloads.
**Do this instead:** One ownership graph, atomic command commits, explicit stable snapshots, disposable views.

### Anti-Pattern 2: Host-local actions bypass shared validation

**What people do:** Host UI mutates directly while guests use RPC validation.
**Why wrong here:** Last-item and last-slot races have different semantics, and solo tests miss production networking rules.
**Do this instead:** One command processor, with local delivery for the host/solo and a transport gateway for guests.

### Anti-Pattern 3: Persistent work is a coroutine or animation callback

**What people do:** Waiting animations complete purchases, spawn loot or schedule retaliation.
**Why wrong here:** Save/load, cancellation and scene reload duplicate or lose commitments.
**Do this instead:** Save explicit order/state/progress/deadline records. Animation reacts to committed simulation state.

### Anti-Pattern 4: Every engine inconvenience becomes a suppression

**What people do:** Disable analyzers across all Godot scripts or force EF constructors/service registration into Nodes.
**Why wrong here:** Either strict rules stop protecting the game or the editor lifecycle breaks.
**Do this instead:** Retain useful rules, scope proven compatibility adaptations, and keep positive/negative Godot build fixtures.

### Anti-Pattern 5: An AI test injects the expected result

**What people do:** A test endpoint grants items/teleports actors, then asserts state and calls it end-to-end.
**Why wrong here:** Production inputs, permissions, focus, collision and economic prerequisites are untested.
**Do this instead:** Clearly separated setup fixtures, real production input, independent observation, and fresh unmodified campaign gates.

### Anti-Pattern 6: Engine threading or lockstep by assumption

**What people do:** Run live Nodes from tasks, share mutable resources, or compare floating-point physics exactly across peers.
**Why wrong here:** Race conditions and false determinism undermine saves, tests and multiplayer.
**Do this instead:** Main-thread engine adapters, one mutation writer, detached worker data, authoritative replication and domain-level reproducibility.

## Integration Points

### External Services

| Service | Integration pattern | Notes |
|---------|---------------------|-------|
| LAN discovery and connections | Discovery advertisements/manual addresses → session descriptor → ENet adapter | Discovery is separate from game commands; game must remain playable by manual address if broadcast discovery fails |
| Local filesystem | Snapshot/config/profile ports | Separate local preferences/identity from host run saves; test Windows/Linux paths and save replacement |
| Future Steam/Epic services | New discovery/identity/connection adapters mapping to existing game IDs | Boundary only in 0.1; no implementation, accounts or matchmaking scope |
| CI/build tooling | Same versioned entry points as local verification | Engine/templates/SDK/content are explicit inputs; external reference folders are not dependencies |

### Internal Boundaries

| Boundary | Communication | Notes |
|----------|---------------|-------|
| UI/input → session | Intent/command DTOs and read models | No direct world mutations or host-only save calls from guests |
| Session → core | Validated identity plus commands | Core has no peer IDs, IP addresses or Godot RPC attributes |
| Core ↔ physics | Host motion intentions and geometry observations | Avoid two independently writable position/health models |
| Core → persistence/replication | Immutable revisioned capture/deltas | Shared state vocabulary; distinct file schema and network envelopes can evolve independently |
| Core → presentation | State projections plus transient event IDs | Save/join reconstruct facts without replaying historical effects |
| Content → core/presentation | Validated stable definition IDs and mapped view resources | Gameplay balance does not depend on sprite filenames or original recovered object IDs |
| Test driver → game | Production input; read-only diagnostics | Adapter and setup tooling excluded from release; no authority override |

## Roadmap Implications

| Fixed/accepted phase | Architecture work enabled or required |
|----------------------|---------------------------------------|
| 1 — Rules/docs | Decide/pin the graph and toolchain, map inherited rules, establish Godot compatibility and positive/negative proof tests; no full E2E framework |
| 2 — Menu/movement | Establish explicit boot/session lifetime, production input, host-local command path, canonical actor ID/state and render/physics separation; visible smoke |
| 3 — LAN/movement | Prove host plus three guests, independent control, small baseline/revision protocol and lifecycle; preserve replaceable discovery/identity/transport boundary; connection smoke |
| 4 — Assets | Stable IDs, manifests, complete imports, modular equipment/corpse view contract and inspectable content fixtures; later feature phases integrate behavior |
| 5 — Comprehensive CI/E2E | Activate foundation tests, full planned acceptance catalog, input/process orchestration, rendered/headless evidence, bounded cleanup and agent procedure |
| 6 — Survivors/inventory/combat | Single ownership graph, atomic payments/transfers, authority, death/succession and initial snapshot/legacy contracts; foundation NPC state expands in Phase 7 |
| 7 — Camp economy/development | Persisted roles, reservations/cargo, recruitment, work commitments, buildings and safe territory; consider characterized JobAssigner adaptation |
| 8 — Zombie campaign/outcomes | Explicit daily/wave/portal commitments, actual loot generation, assaults and outcome ordering; integrate authored map/campaign content |
| 9 — Persistence/player experience | Complete all save triggers/retention/recovery, full late-join/reconnect/config reconciliation/legacy UI and remaining presentation; state contracts already exist |
| 10 — Release validation/balance | Fresh solo/four-player victories, real mixed-OS LAN, creator-PC performance, saved/resumed endurance and full acceptance evidence |

Deeper phase research is warranted for Phase 1 toolchain/analyzer compatibility, Phase 3 packet/latency/join design, Phase 4 equipment attachment pipelines, Phase 5 robust rendered process input on Windows/Linux, Phase 6 nested legacy inventory transactions, and Phase 9 file promotion/schema compatibility. Ordinary menu composition and read-only view binding can use standard engine patterns once the pinned toolchain is proven.

## Confidence and Open Risks

**Confidence method:** `gsd-tools query research-plan` selected Context7 for the two documentation questions. Context7 MCP and `ctx7` CLI were unavailable, so research used web search and direct official stable documentation, cross-checking related API pages. `gsd-tools query classify-confidence --provider websearch --verified` returned **MEDIUM**; the cached digests and this report use that tier. Local specification/source observations are identified separately from architectural recommendations. No prototype, benchmark or recovered port was executed.

| Finding/risk | Confidence | Remaining validation |
|--------------|------------|----------------------|
| Godot partial/export/lifecycle/thread constraints | MEDIUM | Official docs checked; actual selected engine/compiler/analyzer integration still needs Phase 1 build/scene proofs |
| Host writer, commands, ownership graph and snapshot design | MEDIUM | Fits binding requirements; confirm with real four-process conflicts and load/join races |
| Recovered assignment/state/timer candidates | MEDIUM | Source inspected; correctness, suitability and performance are unproven |
| Export-safe automation separation | MEDIUM | Build packaging and actual OS input support must be demonstrated in Phase 5 |
| Reference hardware performance and save capture cost | MEDIUM design confidence; no measured result | Hardware unknown; crowd/render/save/join/endurance gates remain outstanding |

The biggest uncertainty is operational cost: large persistent inventories plus equipped crowds may make snapshot capture, layered drawing or target selection the first bottleneck. Measure each separately; do not redesign the whole game or drop persistent loot on speculation. Exact tick/snapshot rates, prediction needs, serialization format, runner library and platform save promotion primitives are implementation decisions requiring the phase-specific proofs above.

## Sources

All web sources accessed 2026-11-13. `stable` is a moving documentation alias; freeze links to the selected engine minor version during Phase 1. No unstable-only API is required by these recommendations.

- [Project scope and fixed delivery order](../PROJECT.md); [game specification](../../specs/0.1/initial-spec.md), especially §§5, 10–12 and 14; [asset/resource plan](../../specs/0.1/assets-and-resources.md), especially DT resources and production validation.
- [Godot high-level multiplayer](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html): ENet, RPC/authority and channels.
- [Godot MultiplayerSynchronizer](https://docs.godotengine.org/en/stable/classes/class_multiplayersynchronizer.html): property replication and object/identity limitations.
- [Godot GD0001](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/diagnostics/GD0001.html): required partial classes/source generators.
- [Godot C# exported properties](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html): export types/access/defaults.
- [Godot Node](https://docs.godotengine.org/en/stable/classes/class_node.html): lifecycle and processing order.
- [Godot thread-safe APIs](https://docs.godotengine.org/en/stable/tutorials/performance/thread_safe_apis.html): scene-tree/resource threading restrictions.
- [Godot Input](https://docs.godotengine.org/en/stable/classes/class_input.html): action state, GUI event handling and injected input limits.
- [Godot dedicated-server/headless export](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_dedicated_servers.html): headless binaries and resource stripping.
- Local read-only reference: `D:/Work/SaaS/LangProj_monorepo/AGENTS.md`, `src/backend/AGENTS.md`, `src/backend/docs/CodeRules/code-style.md`, `code-architecture.md`, `src/backend/docs/rule-matrix.md`, `src/backend/docs/decisions/0003-enforcement-approach.md`, `src/backend/lang-proj-core/Directory.Build.props`, `Directory.Build.targets`, and `packages/Sebreiro.CodeRules.Analyzers/{Sebreiro.CodeRules.Analyzers.csproj,NullForgivingAnalyzer.cs}`.
- Local read-only recovered sources: `D:/Work/Games/KindomCrown/recovered/source/Assembly-CSharp/{Actor.cs,JobAssigner.cs,StateMachine.cs,KingdomTimer.cs,WorkableBuildingData.cs,Wave.cs,Portal.cs,WaveController.cs}`. Observed source behavior only; no runtime validation claimed.

---
*Architecture research for: KingdomZombies version 0.1*
*Researched: 2026-11-13*
