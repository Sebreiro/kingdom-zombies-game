# Project Research Summary

**Project:** KingdomZombies
**Domain:** Godot/C# pixel-art survival and camp management; solo and four-player cooperative LAN on Windows/Linux
**Researched:** 2026-11-13
**Confidence:** MEDIUM for technical recommendations; explicit local specification controls scope

## Executive Summary

KingdomZombies must deliver the complete specified v0.1: a shared camp, real belongings, survivors and jobs, seven zombie archetypes, six permanently destructible portals, complete saves and LAN sessions, and verified campaigns on both desktop platforms. The accepted ten-phase direction is appropriate; the first five phases and their order are fixed creator requirements. Menu/movement and network demonstrations are intermediate foundations. No mandatory feature becomes optional because it is expensive or called a differentiator in a research template.

Build a compact modular game with an engine-independent C# core and Godot adapters. One host owns simulation and atomic commands; solo and host-local input use that same path. Establish stable identities, a single item ownership graph, coherent snapshot contracts and durable event/work commitments as each feature arrives. Phase 9 completes persistence and recovery rather than introducing their underlying state model. Reuse the applicable LangProj enforcement approach and recovered content/algorithms, with explicit Godot compatibility proofs and specification comparisons. Selected sources must become self-contained project inputs.

The principal risks are incompatible inherited rules, false-green runtime testing, duplicated or lost belongings, inconsistent load/join state, and art/performance claims unsupported by actual integration. Pin the candidate toolchain only after Phase 1 proves it; require positive and negative rule tests, production input, four real game processes, and a required-case evidence manifest. Keep headless simulation, rendered input, real mixed-OS LAN, and reference-PC measurements as distinct evidence. Research found useful designs and source references; it did not build the game, validate the asset pack, prove exports, or measure performance.

## Key Findings

### Recommended Stack

[STACK.md](STACK.md) recommends the following **conditional integration candidate**, not an established working baseline:

| Technology | Recommendation and reason |
|---|---|
| Godot .NET / Godot.NET.Sdk / .NET export templates | Matching **4.7.2** versions; scene/input/render/audio/export integration. Prove the exact distribution on Windows and Linux. |
| .NET / C# | SDK **10.0.404**, explicit **C# 14.0**, proposed **net10.0** target. Installed SDK and published package metadata do not prove native hosting, bindings, reload or exported runtime compatibility. |
| Renderer | **Compatibility** candidate for 2D; compare real required effects and equipped crowds before replacing the starter's Forward Plus setting. |
| Multiplayer | Built-in **ENetMultiplayerPeer/MultiplayerAPI**, with **PacketPeerUDP** discovery and required manual address join. Application authority and catch-up remain project code. |
| Data | **System.Text.Json**, explicit versioned DTOs and file storage; stable IDs and coherent capture are mandatory regardless of format. |
| Pure tests | **xunit.v3 4.0.2** candidate with inherited analyzer/architecture proofs; retain useful donor assertions and architecture packages only after restore/proof. |
| Engine tests | **gdUnit4.api 5.1.0-rc7** is a prerelease candidate, paired with adapter **3.2.0**, analyzers **1.1.0**, and Test SDK **18.0.1** candidate. Prove discovery, runtime execution and failure behavior. |
| Delivery tooling | Pinned local/CI toolchain, GitHub Actions on Windows/Linux, repository-owned four-process orchestrator, clean export/launch verification. UnityPy **1.25.4** is optional offline extraction tooling, never a runtime dependency. |

Keep VSTest for the Godot adapter; run xUnit's MTP executable directly or through an explicitly scoped working directory. Do not copy LangProj's root MTP selection into a mixed runner solution. If the gdUnit prerelease fails integration, use a bounded repository-owned Godot scene harness with the same acceptance obligations. Do not silently skip required cases or change the game baseline to accommodate a runner.

Phase 1 must classify inherited rules as retained/adapted/inapplicable and prove both compliant and violating cases. Cover handwritten versus generated partials, engine callbacks, scene/resource construction, exports, nullability, signals, teardown and main-thread access. No blanket suppression, fake generated-code labels, or imported ASP.NET/EF architecture.

### Expected Features

[FEATURES.md](FEATURES.md) maps complete source scope to phase and acceptance ownership. Its LOW classifier result concerns an unsupported local/curated provider, not uncertainty about the creator's explicit specification.

**Must have:**

- Rules/docs, visible menu-to-movement, independent host-plus-three-guest movement, complete resource production, and repeatable agent-operated local/CI verification.
- One authored map and six portals; eight starting survivors; movement/recovery/load; melee/bow/armor/durability and actual arrow consumption; shared exploration and independent cameras.
- All 24 baseline items, nested grid inventories, equipment, physical cash, unlimited paged stash, immediate unpaused interactions, persistent actual death loot, and safe legacy layout extraction.
- Main-building recruitment, four assigned roles and unassigned workers, two wildlife types, gathering/hunting/food sales, hauling, paid clearing/work/repairs/rebuilding, and three main/wall/tower tiers.
- Seven zombie archetypes with real legal inventories; day/night escalation, defense/regular/retaliation queues, dawn retreat, rally assaults/recall, succession/spectating, victory and both defeat conditions.
- Complete LAN discovery/manual join/late join/reconnect/host-loss behavior; coherent manual/dawn/exit saves; multiple runs, error recovery and changed-config compatibility.
- Separate documented configuration domains and independent zombie progression; required menus/HUD, remapping, UI scaling, gear appearances, effects, auditioned sound/ambience, English catalog and translation-ready layouts.
- Complete solo/four-player campaigns, A-01–A-47, runnable Windows/Linux packages, actual LAN, reference-PC stress, saved/resumed endurance and pacing evaluation.

**Distinctive and still mandatory:** physical belongings and money across every system; ordinary-survivor succession without healing or teleportation; permanent portals with additive retaliation; non-destructive legacy inventories; one unpaused authoritative world. Structural tower archers are an explicit exception: no ordinary inventory/population/loot, standalone counts 1/2/3, main attachments exactly 1+1 sharing main health.

**Deferred:** Steam/Epic service implementations only; their replaceable discovery/identity/connection boundary is required now. Explicit exclusions such as extra maps, crafting, online matchmaking, host migration, controllers, music and equipment repair are not promises for v2. Preserve proposed balance as a configurable, unproven playtest baseline.

### Architecture Approach

[ARCHITECTURE.md](ARCHITECTURE.md) recommends two initial production assemblies: pure C# core and Godot game. Add assemblies or frameworks only for demonstrated boundaries. Commands flow from production input through the session gateway to one host writer, then committed state/deltas feed views and immutable snapshots.

1. **Core and command processor:** stable run/player/entity/item/content IDs, explicit tick ordering, permission/current-state checks, atomic commits and duplicate-command handling. The host's own actions obey the same rules as guests.
2. **Ownership and simulation systems:** one acyclic item graph; real ammo, payments and corpse transfers; jobs/cargo/reservations and paid work; persistent day/wave/portal/outcome commitments. Preserve exactly one location per item and no mutation on rejected transactions.
3. **Godot adapters and presentation:** input/focus/remapping, authoritative host collision, rebuildable views, gear and audio. Main-thread engine access and explicit session lifetime prevent stale callbacks; one-shot effects are distinct from persistent facts.
4. **Session and replication:** replaceable discovery/identity/connection ports; bounded baseline-plus-delta catch-up with readiness gating. Stable player identity differs from transient peer identity. Physics is host-authoritative, not presumed cross-platform lockstep.
5. **Persistence/configuration:** coherent revision capture, detached serialization, checked replacement preserving prior saves, versioned compatibility and legacy placement metadata. Current settings govern new actions; accepted work, belongings and event commitments survive loading.
6. **Verification and content tooling:** production-input drivers, read-only observation, isolated per-process profiles/logs, bounded cleanup, case/evidence manifests and provenance. Test-only authority controls must not ship.

### Critical Pitfalls

[PITFALLS.md](PITFALLS.md) expands these into fourteen concrete risks and phase checks.

1. **Rules that break Godot or exempt it entirely:** prove real scene instantiation/load/unload with inherited analyzers, precise exceptions and deliberate bad-code fixtures; separate pure async rules from engine-thread continuations.
2. **False-green tests and substitute evidence:** reconcile required case IDs against fresh, uniquely executed non-skipped results and every expected peer/artifact. Inject missing-engine/adapter, zero-discovery, wrong-filter, skip, crash, timeout and missing-report/image failures. Headless state setters do not prove rendered production input.
3. **Ownership loss or duplication:** transact moves/payments/death/hauling against one graph, retain contents and identities, consume actual arrows, and test competing requests. Never reroll death loot or prune nonempty corpses for performance.
4. **Incoherent saving, joining or event recovery:** design snapshots/legacy metadata early; capture one committed revision; sequence catch-up and gate control; persist work/randomness/queue progress and once-only markers. Load must not replay dawn, purchases, destruction or initial supplies.
5. **Asset presence and lightweight benchmarks mistaken for completion:** prove fitted body/gear/action/direction/corpse combinations, legal guard counts, imported audio and clean self-contained packaging. Measure the feature-complete rendered crowd on actual reference hardware; preserve separate LAN and endurance records.

## Implications for Roadmap

Retain the accepted ten-phase direction below. Phases 1–5 and their order are fixed; Phases 6–10 preserve the accepted working direction for detailed requirements mapping.

### Phase 1: Code rules and documentation

**Rationale:** Engine-compatible enforcement and boundaries must precede gameplay.
**Delivers:** Pinned/proven toolchain candidate, scoped rule matrix and positive/negative proofs, architecture decisions, concise root AGENTS.md and focused indexes.
**Addresses:** Developer rules, reproducibility and early identity/authority/persistence contracts.
**Avoids:** Blanket suppressions, generated-code blind spots, incompatible constructors/threading, and MTP/VSTest confusion.

### Phase 2: Playable foundation

**Rationale:** Prove the production input and engine lifecycle path before networking it.
**Delivers:** Visible main menu, small scene, base assets and one left/right moving character, with launch/input/render smoke checks.
**Addresses:** Menu-to-play, input, canonical actor identity/state and local command path.
**Avoids:** Direct test-only movement, focus leakage and headless-only visual claims.

### Phase 3: Four-player LAN foundation

**Rationale:** Establish shared authority before adding ownership-sensitive gameplay.
**Delivers:** Actual host plus three guest processes independently controlling synchronized actors; replaceable service ports, stable identities, baseline/revision and lifecycle smoke proofs.
**Addresses:** Multiplayer foundation on the same scene; groundwork for complete session recovery.
**Avoids:** Host double execution, shared profiles, sockets mistaken for readiness, physics lockstep and snapshot rollback.

### Phase 4: Asset production

**Rationale:** Stable content and appearance contracts must precede bulk gameplay integration.
**Delivers:** Recovered/adapted/generated required pack, source hashes/identities/recipes, stable output IDs, self-contained imports and inspectable fixtures covering all 180 resource obligations.
**Addresses:** Character/equipment/animation, buildings/map/enemies, effects/UI/audio/data/text resources; shared outputs may satisfy multiple obligations.
**Avoids:** Catalog evidence credited as finished production, detached equipment, wrong guard counts and donor-drive dependencies. Later phases still integrate and verify content in play.

### Phase 5: CI and repeatable game testing

**Rationale:** Full gameplay needs an agent-operable regression system; earlier phases already retain their smaller proofs.
**Delivers:** Matching local/GitHub Actions entry points, full A-01–A-47 catalog plus foundation/regression cases, required-case manifest, actual input/four-process orchestration, rendered/headless evidence and failure diagnostics.
**Addresses:** Traceability from atomic requirement to owning phase, executable case, environment and evidence; agent selection/run/inspect/reproduce/fix/rerun procedure.
**Avoids:** False-green discovery/skips/artifacts and orphaned processes. Future-feature cases remain explicitly planned with owners, not passed; activate them as features arrive.

### Phase 6: Survivors, inventory, and combat

**Rationale:** Camp and enemies depend on trustworthy possessions and actors.
**Delivers:** Survivor control/recovery/load, real nested items/equipment/stash, atomic transfers/payments, combat/ammunition/wear, corpses and succession/spectating foundations; snapshot round trips and legacy representation.
**Addresses:** Real belongings, immediate inventory interactions, combat and unchanged successor state.
**Avoids:** Partial operations, duplicate quick-slot supplies, invented death loot and deferred persistence design. Supply minimum authoritative time/reachability/safety contracts needed by recovery.

### Phase 7: Camp economy and development

**Rationale:** Build the operational camp on shared item, money and actor rules.
**Delivers:** Recruitment, roles, actual gear selection, food/wildlife, safe jobs/hauling, paid clearing/work/repairs/upgrades/rebuilding and territory/defenses.
**Addresses:** Finite physical economy, durable cargo/reservations, three tiers and structural guards.
**Avoids:** Double payment, free repair, carrier-death duplication, repeated dawn resources and nonpersistent accepted work. Establish minimum world/time services before full campaign integration.

### Phase 8: Zombie campaign and outcomes

**Rationale:** Campaign pressure depends on functional camp, combat and actual enemy belongings.
**Delivers:** Authored map/campaign integration, seven zombies, progression, all wave/source queues, permanent portals, retaliation, dawn retreat, assaults/recall, victory and defeat ordering.
**Addresses:** Complete six-portal loop with durable event identities, generated inventories and source commitments.
**Avoids:** Rerolls, duplicated retaliation, deleted living actors at caps, regenerated portals and incorrect simultaneous outcomes. A-08's noun typo is resolved by explicit §§2.3/9.4: remaining zombies stop dawn retreat after the last portal; ordinary survivor rules remain intact.

### Phase 9: Persistence and complete player experience

**Rationale:** Integrate existing state contracts into complete recovery and usability.
**Delivers:** Manual/dawn/exit saves, multiple runs, safe failure recovery, config reconciliation and legacy extraction; full discovery/manual join/late join/reconnect/host loss; remaining interfaces, settings, audio and translation readiness.
**Addresses:** Complete save/session/player experience, with feature presentation already integrated throughout earlier phases.
**Avoids:** Destructive migration, catch-up before readiness, new supplies/healing on load/join, replayed effects and unvalidated save replacement.

### Phase 10: Release validation and balancing

**Rationale:** Only the integrated game can establish release behavior and measured viability.
**Delivers:** Fresh unaided solo/four-player victories, all required acceptance evidence, clean Windows/Linux launch, real mixed-OS LAN, tuned baseline pacing and reference-PC performance/endurance records.
**Addresses:** Approximately 2–3 hours/8–12 days pacing; four-player 100-survivor/300-zombie crowd at 1920×1080; at least 30 FPS in 95% of one-second samples over ten minutes, no sub-30 stretch over five seconds, no save/join freeze over 0.5 seconds; three-hour saved/resumed endurance with at least 200 nonempty corpses/piles represented over its course.
**Avoids:** Accelerated fixtures credited as real-duration campaigns/endurance, CI throughput credited as rendered reference-PC speed, or required behavior disabled to pass.

### Phase Ordering Rationale

- Foundations establish enforcement, real input and authority; assets then establish content contracts; comprehensive automation precedes the main feature build.
- Ownership and stable state precede economy/jobs, which precede enemy campaign integration. Snapshot round trips, configuration validation and case activation accompany every feature.
- Persistent loot, layered gear and join/save costs need incremental measurements from asset/gameplay fixtures onward. Final hardware acceptance remains Phase 10.
- Phase 4 production is not final integration; Phase 5 planned cases are not passed cases; Phase 9 persistence completion is not permission to defer serializable state.

### Research Flags

Use targeted `$gsd-plan-phase --research-phase <N>` work for **1** (engine/compiler/analyzers and runner pairing), **3** (packet/revision/readiness and measured movement needs), **4** (equipment fitting/extraction/import pipeline), **5** (Windows/Linux rendered input, process isolation and false-green controls), **6** (nested ownership/legacy transaction invariants), and **9** (coherent capture/catch-up, platform file replacement and compatibility).

**Standard patterns after those proofs:** Phase 2 menu/view composition; Phase 7 explicit job/work state machines; Phase 8 durable scheduling/queue policies. Use focused follow-up if a recovered algorithm or measurement exposes a gap. Phase 10 primarily needs empirical execution on real environments, not another broad literature survey. Ordinary patterns do not remove the need for the project's specific concurrency and acceptance cases.

## Confidence Assessment

| Area | Confidence | Notes |
|---|---|---|
| Stack | MEDIUM | Report cites official releases/packages/docs and inspected donor pins; exact integrated restore/build/scene/export/runner matrix is unproven. |
| Features | HIGH scope authority; LOW provider classification | Explicit creator-approved local requirements control scope. LOW came from unsupported curated-provider classification, not a scope ambiguity. Cost, balance and implementation remain unverified. |
| Architecture | MEDIUM | Official engine constraints and inspected sources support the design; transactions, catch-up and performance have no runtime prototype proof. |
| Pitfalls | MEDIUM | Documented API/runner constraints and requirement-derived failure modes; prevention strategies still require deliberate failing tests and runtime evidence. |

**Overall confidence:** MEDIUM. This summary synthesizes the four reports; it does not independently revalidate their external source claims or certify implementation.

### Gaps to Address

- **Version/source drift:** STACK cites versioned 4.7 docs/packages, while architecture/pitfalls cite moving `stable` aliases. The C# prerequisite paragraph reportedly still references Godot 4.5; gdUnit setup examples reportedly lag current metadata, including Test SDK 18.0.0 versus the proposed 18.0.1. Record these discrepancies, freeze matching docs/package manifests, and let Phase 1 integration prove compatibility. Do not treat an old minimum-target policy or NuGet compatibility as an export proof.
- **Runner integration:** gdUnit is prerelease and VSTest-based; inherited xUnit uses MTP. Verify exact selected CLI behavior, working-directory SDK resolution, failure propagation and required-ID reconciliation before relying on reports.
- **Unmeasured runtime costs:** Reference CPU/GPU/RAM are unspecified; snapshot size/capture, catch-up buffers, tick/update rates, layered rendering and target selection need representative measurements. Prediction and deeper optimization remain evidence-driven choices.
- **Unproduced content:** The 180 obligations and V/M/C source evidence do not establish exported, fitted, audible assets. Complete provenance, production checks and subsequent gameplay integration.
- **Persistence details:** File promotion and interruption recovery must be tested on both platforms; legacy graph rules and accepted commitments need regression fixtures before final save UX.
- **Balance and acceptance wording:** Preserve source-marked proposed numbers until playtesting. Record the A-08 zombie-retreat interpretation in case traceability without changing source scope or requesting a redundant scope decision.

## Sources

### Primary requirements and inspected local evidence

- [PROJECT.md](../PROJECT.md), [game specification](../../specs/0.1/initial-spec.md), [asset/resource plan](../../specs/0.1/assets-and-resources.md), and [config.json](../config.json): binding scope, accepted sequence, A-01–A-47 and evidence requirements.
- [STACK.md](STACK.md), [FEATURES.md](FEATURES.md), [ARCHITECTURE.md](ARCHITECTURE.md), [PITFALLS.md](PITFALLS.md): detailed source registers and provenance for this synthesis.
- LangProj source rules/build settings/analyzer tests under `D:/Work/SaaS/LangProj_monorepo`; recovered algorithms under `D:/Work/Games/KindomCrown/recovered/source/Assembly-CSharp`: inspected source candidates as reported by researchers, not validated ports or production dependencies.

### Primary technical documentation cited by researchers — MEDIUM research confidence

- [Godot archive](https://godotengine.org/download/archive/), [Godot.NET.Sdk 4.7.2](https://www.nuget.org/packages/Godot.NET.Sdk/4.7.2), [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [.NET version selection](https://learn.microsoft.com/en-us/dotnet/core/versions/selection): candidate pins and SDK/runtime distinction.
- [Godot C# targeting policy](https://godotengine.org/article/godotsharp-packages-net8/), [4.7 C# basics](https://docs.godotengine.org/en/4.7/tutorials/scripting/c_sharp/c_sharp_basics.html), [renderer comparison](https://docs.godotengine.org/en/4.7/tutorials/rendering/renderers.html), [exports](https://docs.godotengine.org/en/4.7/tutorials/export/exporting_projects.html): compatibility candidates and documented caveats.
- [gdUnit API prerelease](https://www.nuget.org/packages/gdUnit4.api/5.1.0-rc7), [adapter](https://feed.nuget.org/packages/gdUnit4.test.adapter/3.2.0), [compatibility table](https://github.com/godot-gdunit-labs/gdUnit4Net), [setup guide](https://godot-gdunit-labs.github.io/gdUnit4/latest/csharp_project_setup/csharp-setup/), [Test SDK](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/18.0.1): version pairing and stale-example discrepancies.
- [xUnit release](https://xunit.net/releases/v3/4.0.2), [xUnit MTP](https://xunit.net/docs/getting-started/v3/microsoft-testing-platform), [runner modes](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test), [VSTest/MTP migration](https://learn.microsoft.com/en-us/dotnet/core/testing/migrating-vstest-microsoft-testing-platform): runner boundary and result semantics.
- [Godot multiplayer](https://docs.godotengine.org/en/4.7/tutorials/networking/high_level_multiplayer.html), [MultiplayerSynchronizer](https://docs.godotengine.org/en/stable/classes/class_multiplayersynchronizer.html), [Node](https://docs.godotengine.org/en/stable/classes/class_node.html), [GD0001](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/diagnostics/GD0001.html), [thread-safe APIs](https://docs.godotengine.org/en/stable/tutorials/performance/thread_safe_apis.html): authority, identities, generation and lifecycle boundaries.
- [Input](https://docs.godotengine.org/en/stable/classes/class_input.html), [input event routing](https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html), [CLI](https://docs.godotengine.org/en/4.7/tutorials/editor/command_line_tutorial.html), [artifact upload](https://github.com/actions/upload-artifact): production input and evidence limitations.
- [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview), [UnityPy](https://github.com/K0lb3/UnityPy): serialization and offline extraction candidates.

No secondary market claims or tertiary opinion sources are needed to redefine the explicit scope. Technical source access dates in the reports are 2026-11-13; moving pages require version matching during implementation.

---
*Research completed: 2026-11-13*
*Ready for roadmap: yes — complete v0.1, preserving the accepted ten-phase direction; runtime proofs remain outstanding.*
