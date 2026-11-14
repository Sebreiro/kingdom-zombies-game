# Pitfalls Research

**Domain:** Godot/C# side-view survival and camp management; solo and host-authoritative four-player LAN
**Researched:** 2026-11-13
**Confidence:** MEDIUM

## Evidence and scope

This report preserves the full 0.1 scope and the approved phase direction. Phases 1–5 remain rules/documentation, playable foundation, four-player LAN, asset production, then CI/repeatable testing. Phases 6–10 are survivors/inventory/combat, camp economy/development, zombie campaign/outcomes, persistence/player experience, and release validation/balancing. A later completion phase does not defer the identities and state contracts its features depend on.

**Provenance facts:** The project definition reports a starter project without verified gameplay. The asset plan reports sampled visuals/metadata and catalog matches, not a produced pack. Direct inspection of LangProj's style rules, rule matrix, and build properties confirms strict nullable/analyzer rules, generated-code exemptions scoped to its own outputs, and package-specific `ConfigureAwait(false)` rules. These are reference inputs, not KingdomZombies implementation facts. Original recovered source/resources and specification files were not modified.

**Confidence method:** `query research-plan` selected Context7 for Godot documentation and websearch for test infrastructure. Neither Context7 MCP nor its `ctx7` CLI was available, so primary Godot, Microsoft, and GitHub documentation was retrieved through web search/open. `query classify-confidence --provider websearch --verified` returned MEDIUM; the research digests were cached at that tier. All technical findings and design recommendations below use MEDIUM confidence. Local specification statements are requirements/provenance, not experimentally verified behavior. Recommendations are explicitly engineering inferences from those requirements and the cited API constraints.

**Contradictions/limits:** A-08 says survivors do not retreat after victory, while §§2.3/9.4 describe remaining zombies ceasing dawn retreat after the last portal. Treat A-08's actor wording as a typo: preserve the explicit zombie rule and ordinary survivor behavior, and record this interpretation in its test traceability. Do not edit the source specification. Current online .NET documentation spans newer SDK/test-runner behavior; exact test flags must be verified against the toolchain chosen in Phase 1. No engine compatibility, mixed-OS session, export, decoded asset pack, rendering, or performance benchmark was executed for this research.

## Critical Pitfalls

### Pitfall 1: Strict C# rules that reject correct Godot code or silently exempt everything

**What goes wrong:** The first gameplay scene fails to instantiate despite a clean domain build, or developers disable analyzers across all game code to accommodate generated bindings. Null-forgiving initializers hide absent scene references; stale native objects survive behind non-null C# references.

**Why it happens:** LangProj is a backend reference. Its constructor, visibility, naming, generated-file, dependency, and async conventions are not proof of Godot compatibility. Godot requires partial `GodotObject` types for generated engine bindings. Scene lifecycle also differs from constructor injection: children become ready before parents; reentering the tree does not automatically repeat `_Ready`. [Godot GD0001](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/diagnostics/GD0001.html), [Node lifecycle](https://docs.godotengine.org/en/stable/classes/class_node.html).

**How to avoid:** In Phase 1, map every inherited rule to retained/adapted/inapplicable with exact scope and positive/negative proof. Cover generated partial output, engine overrides, exported properties, scene/resource construction, nullable initialization, serialization, public engine entry points, and naming. Keep handwritten adapters analyzed; do not label them generated. Prefer nullable backing fields plus validated lifecycle access where appropriate; do not initialize fake nodes merely to satisfy nullability. Prove a real scene loads, resolves references, emits a signal, unloads, and reloads under the chosen compiler/analyzers. Treat missing required references as explicit errors.

**Warning signs:** A pure C# constructor test substitutes for scene instantiation; root-wide suppressions; `null!`; reflection rules select zero game types; tests require only build success; copied HTTP/EF/test-host requirements with no gameplay purpose.

**Phase to address:** Phase 1; real scene regressions in Phases 2 and 5.

### Pitfall 2: Background saving or inherited async rules touching live nodes

**What goes wrong:** Saving, asset work, or delayed callbacks mutate freed nodes or race scene changes. Rare failures appear only during disconnect, death, or returning to the menu.

**Why it happens:** A blanket package-level `ConfigureAwait(false)` policy or `Task.Run` around the game state moves scene access off its owning thread. Godot documents that the active scene tree is not thread-safe. C# signal automatic-disconnection has exceptions, including captured lambdas and custom signals. [Thread-safe APIs](https://docs.godotengine.org/en/stable/tutorials/performance/thread_safe_apis.html), [C# signals](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html).

**How to avoid:** Establish an explicit main-thread boundary and session lifetime. Capture immutable/coherent domain snapshots before background serialization; hand results back through a deliberate main-thread mechanism. Cancel obsolete work and reject callbacks belonging to the previous session generation. Track and disconnect subscriptions where automatic lifetime handling does not apply. Keep pure library async rules separate from node-facing continuations; prove the boundary instead of assuming every `await` resumes safely.

**Warning signs:** Serialization walks the scene tree from a worker; tasks survive menu exit; event subscriptions grow after repeated joins; exceptions disappear when waits or logging are added.

**Phase to address:** Phase 1 boundary; Phase 3 lifecycle; Phase 9 persistence implementation.

### Pitfall 3: A green pipeline with no required runtime tests

**What goes wrong:** CI reports success after building only, running zero tests, skipping the game suite when Godot is missing, or uploading a stale result. An AI agent mistakes infrastructure absence for acceptance.

**Why it happens:** Framework exit status is mistaken for coverage of required scenarios. VSTest and MTP differ on zero-test handling, and MTP behavior/options depend on SDK and runner version. Artifact upload defaults can warn rather than fail when nothing matches. [Microsoft migration guidance](https://learn.microsoft.com/en-us/dotnet/core/testing/migrating-vstest-microsoft-testing-platform), [MTP command behavior](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test-mtp), [GitHub upload-artifact](https://github.com/actions/upload-artifact).

**How to avoid:** Store a case manifest linking foundation cases and A-01–A-47 to phase, environment, executable selector, expected evidence, and status. Distinguish `planned`, `passed`, `failed`, `skipped`, `blocked`, and `not run`; only actual successful execution satisfies required checks. At Phase 5, future cases remain planned under their owning phases, not skipped tests falsely credited as complete. Reconcile the expected required IDs with unique executed, non-skipped IDs and fresh run-specific reports. Check every required process and artifact, not just an aggregate count. Prove the verifier fails for missing engine, missing adapter, zero discovery, wrong filter, skipped required case, absent report, forced child crash, timeout, and missing screenshot. Preserve exit failures through shell wrappers and artifact steps.

**Warning signs:** `if engine missing then success`; permissive report glob; one passed unit test stands in for the engine suite; no expected count/IDs; `continue-on-error`; ignored exit codes; elapsed runtime is the only evidence.

**Phase to address:** Rule proof checks in Phase 1; smoke evidence in Phases 2–3; full gate in Phase 5, extended in Phases 6–10.

### Pitfall 4: Headless tests credited as rendered, keyboard-driven play

**What goes wrong:** Simulations pass while the menu cannot start a game, focus blocks movement, UI clicks attack through panels, sprites are missing, or muted warnings become invisible.

**Why it happens:** Test commands directly set velocity or call domain handlers. Godot's headless option selects a headless display and dummy audio; `Input.ActionPress` does not invoke input callbacks. Global input polling is unaffected by GUI handled-event state. [Command line](https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html), [Input API](https://docs.godotengine.org/en/stable/classes/class_input.html).

**How to avoid:** Keep separate domain, engine/headless, rendered input, and real-device evidence lanes. Engine-injected key/mouse events must enter the production InputMap and routing path; action injection alone cannot prove rebinding or GUI interception. Add actual OS keyboard/mouse smoke cases for window focus and visible menu-to-scene movement. Consume GUI events and explicitly gate polled gameplay actions by interface/focus state. Capture labeled images at meaningful checkpoints and inspect them. Rendered CI evidence still does not establish creator-PC performance or audibility; retain audition checks. [Input propagation](https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html).

**Warning signs:** Screenshots come from headless runs; the actor moves through a test-only setter; tests call a button handler directly; a click both drops an item and swings a weapon; remapped controls have no test.

**Phase to address:** Phase 2 input contract; Phase 5 layered harness; Phases 6/9 UI cases; Phase 10 real hardware.

### Pitfall 5: Four clients exist, but independent authoritative play was never proven

**What goes wrong:** All peers control one actor, host input is applied twice, guest actions execute locally without host acceptance, or the harness passes after one guest failed to join.

**Why it happens:** A single-process mock or a count of sockets substitutes for host-plus-three-client execution. RPC configuration, identity, scene path, and authority are conflated. Godot requires matching RPC node paths and signatures; local execution and transfer behavior must be selected deliberately. [High-level multiplayer](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html).

**How to avoid:** Run four distinct production game processes with separate user-data directories, stable player identities, deterministic fixture/configuration, isolated run IDs and ports, and per-process logs. Wait for explicit readiness and completed world synchronization, not fixed sleeps. Feed different input sequences through each process's production input adapter; assert that only its controlled character responds and every observer converges. Include one real host input path, late join, guest disconnect/reconnect, simultaneous requests, and host loss. Use bounded waits, heartbeats, diagnostic snapshots, and cleanup limited to recorded child PIDs. Authenticate the command's actor from the sender/session mapping; never trust a supplied player ID. Keep test controls out of release behavior.

**Warning signs:** Four windows share preferences/player identity; only the host log is checked; guest failure leaves a passing host; random ports collide; retries silently restart until success; movement is tested only by direct domain calls.

**Phase to address:** Phase 3 foundation smoke; Phase 5 reliable process harness; Phase 9 complete sessions; Phase 10 mixed-OS LAN.

### Pitfall 6: Deterministic setup becomes an accidental physics-lockstep requirement

**What goes wrong:** Tests compare frame-perfect positions across platforms and become flaky, or gameplay assumes every peer computes identical collisions, damage, and random outcomes.

**Why it happens:** Seeded randomness and fixed physics ticks are mistaken for proof of bit-identical cross-platform simulation. Godot documents fixed-rate physics callbacks; that is not a determinism guarantee. No cross-platform lockstep guarantee was established by this research. [Node processing](https://docs.godotengine.org/en/stable/classes/class_node.html).

**How to avoid:** Use deterministic domain fixtures, injected domain clock/random source, ordered command scenarios, and recorded initial state. Let the host decide shared outcomes. Assert exact item/transaction/event identities and quantities, but bounded eventual position convergence and observable movement outcomes. Keep visual interpolation outside authoritative state comparisons. Record seeds and command traces for replay of domain decisions; do not claim exact whole-engine replay without evidence.

**Warning signs:** CI tolerances grow without diagnosis; every peer rolls loot; frame number alone identifies a command; tests require exactly equal floating-point positions on Windows and Linux.

**Phase to address:** Phases 3/5; domain determinism in Phases 6–8.

### Pitfall 7: Nested inventories implemented as UI arrays instead of transactions

**What goes wrong:** Two actors take one bow; selling food races eating; a filled bag leaves its contents behind; a failed purchase still charges; a quick slot becomes a second supply of food.

**Why it happens:** Visual drag state is used as ownership, reservations remove items early, and operations validate their separate steps against different world revisions.

**How to avoid:** Maintain stable item IDs and exactly one owner/location per item, with an acyclic containment graph. A host command validates sender access, current version, fit/rotation, stack compatibility, slot rules, depth, all affected destinations, and payment before one atomic commit. A failed command leaves ownership and currency unchanged. Move the container subtree intact; quick slots reference actual stack IDs. Reservations are claims on future work, not ownership. Check global conservation after competing operations, including splitting, equipping storage, NPC upgrades, death, selling, and disconnect. Bound traversal against malicious/corrupt cycles while permitting valid legacy layouts. These are design recommendations derived from spec §§5–7 and A-22–A-29.

**Warning signs:** Inventory code contains `remove` followed by a fallible `add`; drag removes the item immediately; NPC and player transfers use separate rules; stack identity is an array index; stale panels can commit.

**Phase to address:** Phase 6 authoritative item model; Phase 7 NPC/economy operations; Phase 9 load compatibility.

### Pitfall 8: Death generates loot instead of preserving actual possessions

**What goes wrong:** Spent arrows reappear on corpses, broken items become new equipment, hauling duplicates cargo, or aged corpses delete belongings. Tower archers accidentally create free bows and survivors.

**Why it happens:** Enemy appearance, combat statistics, and reward tables are independent representations. Corpse lifetime is attached to a death animation or particle timeout.

**How to avoid:** Generate legal actual inventories at zombie creation; combat consumes that state. Death transfers the same remaining equipment/nested contents to a persistent corpse. A carrier's picked-up cargo belongs to its new corpse if it dies. Keep loot data/interaction separate from cosmetic stains and presentation lifetime. Expire only completely empty containers after the specified countdown; adding any belonging cancels it. Structural tower guards remain the explicit no-inventory/no-corpse exception. Test zero arrows, damaged gear, empty bags, nested cargo, looting appearance updates, and death during pickup. Spec §§5.5/9, A-14/A-21/A-26/A-27/A-31/A-36 define the oracle.

**Warning signs:** A generic `DropLoot` rerolls on death; display weapons do not reference equipped items; global corpse caps; acid fade frees the loot node; loot is recreated from an archetype definition.

**Phase to address:** Phase 6 death/ammunition; Phase 7 hauling; Phase 8 zombie/tower integration; Phase 10 endurance.

### Pitfall 9: Save and late join reconstruct a plausible but inconsistent world

**What goes wrong:** A snapshot contains the item before a move and its destination after the move; a late join overwrites newer changes; death/payment sounds replay; a new guest receives another starting kit.

**Why it happens:** Serialization enumerates changing objects, node instance IDs are persisted, and per-property replication is treated as a transaction/snapshot protocol. `MultiplayerSynchronizer` excludes Object/Resource properties and peer-specific instance IDs/RIDs. [MultiplayerSynchronizer](https://docs.godotengine.org/en/stable/classes/class_multiplayersynchronizer.html).

**How to avoid:** Design stable domain/content/player IDs and versioned serializable state as features arrive. Capture a coherent authoritative revision, then serialize/send its immutable representation while play continues. Buffer or otherwise sequence later changes, apply a bounded catch-up protocol, and grant control only after the joining world is ready. Reconstruct current state separately from historical one-shot presentation events. Persist full nested ownership, health/wear, jobs/cargo/work, exploration, corpses, timers, commitments and reconnect associations. Use checked temporary writes and an OS-tested replacement/recovery strategy preserving the last valid save. Verify interrupted writes and failed replacements on both platforms.

**Warning signs:** Save DTOs are invented in Phase 9 for earlier systems; snapshots enumerate live dictionaries from a worker; IDs come from engine instance IDs; snapshot completion means only file receipt; initial-spawn logic also runs on reconnect.

**Phase to address:** IDs/boundaries in Phases 1/3; state design in Phases 6–8; complete protocol and recovery in Phase 9.

### Pitfall 10: Time, waves, and paid work repeat after retries or reload

**What goes wrong:** Dawn refills plants twice, regular and retaliation queues merge incorrectly, portal destruction races spawn release, saving rerolls zombie equipment, or a retried upgrade request charges twice.

**Why it happens:** Only current positions and totals are saved. Timers, event identity, generated inventory, queue progress and accepted commitments are reconstructed from current time/configuration. Reliable delivery is mistaken for application-level idempotency.

**How to avoid:** Give commands/events stable identities scoped to the run/session; record accepted results and reject or return the prior result for duplicate requests. Define authoritative ordering for simultaneous results and phase transitions. Persist generated commitments, warning/spawn progress, randomness state where needed, completion flags and remaining work. Preserve regular-queue dawn expiry versus persistent defensive/retaliation queues and cancellation when their source is destroyed. A cap limits future spawns, never deletes existing actors. Test before/at/after dawn, final-portal destruction, cap saturation, interrupted work, retries and load with changed durations/prices/caps. Spec §§3/8–11 and A-03/A-09/A-30/A-34–A-44 govern the results.

**Warning signs:** Loading calls the dawn handler; rejoining replays all events; queue is just a count; active zombies are removed to admit new ones; changing a price recomputes an accepted payment.

**Phase to address:** Phase 6 command convention; Phase 7 daily/work state; Phase 8 waves/outcomes; Phase 9 save/load integration.

### Pitfall 11: Configuration updates erase valid old belongings

**What goes wrong:** Shrinking a grid or nesting limit makes a save unloadable, automatically moves contents to stash, silently truncates stacks, or hides formerly valid equipment.

**Why it happens:** Current placement validation is applied as a destructive migration to all saved locations.

**How to avoid:** Preserve old display geometry, position, orientation, containment, and illegal legacy occupancy as visible/removable state. Apply current rules to new destinations and new actions; do not let unusable combinations attack or protect. Preserve legacy state in subsequent saves. Missing definitions/corruption produce actionable errors and retain originals. Test extracting a filled legacy bag, oversized stack splitting, incompatible two-hand equipment, failed destination fit, and resaving unresolved legacy contents. Spec §5.6/11.2 and A-42–A-44 specify this policy.

**Warning signs:** Load runs generic auto-sort; a failed validation logs a warning and drops an item; only default configuration is tested; configuration is live-reloaded during play.

**Phase to address:** Phase 6 item representation; Phase 9 compatibility fixtures and error UX.

### Pitfall 12: Asset count substitutes for usable fitted content

**What goes wrong:** Every requirement has a PNG, but backpacks detach during death, bows appear in the wrong hand, seven zombies are unreadable in crowds, tower guards have the wrong count, or donor scripts restore excluded mechanics.

**Why it happens:** V/M/C evidence labels are collapsed into “verified”; a catalog ID is treated as an extracted file; Unity animations/materials/palettes are assumed portable; generated front-facing artwork is accepted without movement/equipment variants.

**How to avoid:** Preserve source file/type/object ID and hashes, transformations, dependencies and stable output ID. Keep archive stream companions during extraction and preserve originals. Validate one complete neutral survivor and zombie with legal equipment before multiplying silhouettes. Build fixtures for required body/gear/action/direction combinations, both hands, mixed dual weapons, bows/ammunition, crown/headgear, corpse gear removal, all tiers/damage/ruins, muted warnings and expanded UI text. Check alpha, pixel scale, pivots, occlusion, frame timing, attachment anchors and palette in the actual renderer; audition audio. Count 180 resource obligations as coverage, not 180 independent files. Strip recovered event callbacks and compare reused algorithms against this game's rules. Asset-plan §§1–3/12 define the evidence and fit requirements.

**Warning signs:** “File exists” is the acceptance check; only idle right-facing gear is inspected; sound streams were never played; fantasy Greed is final zombie art; portal regeneration or external recruitment reappears from donor logic.

**Phase to address:** Phase 4 production fixtures; integrate and reverify during Phases 6–9; Phase 10 crowd checks.

### Pitfall 13: CI and shipped builds depend on the creator's donor directory

**What goes wrong:** Exports work locally but fresh CI, Linux, or a player's computer cannot find configuration, textures, fonts, or adapted algorithms.

**Why it happens:** Absolute `D:/Work/Games/KindomCrown/recovered` references, symlinks, local extraction caches, and case-insensitive file names become production dependencies. A successful export is mistaken for launch validation.

**How to avoid:** Make selected exported resources and adapted code self-contained project/build inputs. For any externally stored selected pack, use an explicit versioned acquisition step with integrity checks and documented access; runtime never reads the donor path. Validate all content-ID references and export inclusion, including non-scene JSON/catalog/audio resources. Rebuild/import from a clean checkout without the donor drive, then launch packaged Windows/Linux builds without SDK/editor installations. Record exact case for asset paths and the root `AGENTS.md`; avoid case-colliding files. Donor provenance remains a manifest, not a runtime address.

**Warning signs:** Developer cache masks missing files; assets load only with editor import state present; paths use Windows separators/casing; release archive contains source references but not data.

**Phase to address:** Phase 4 resource packaging; Phase 5 clean CI acquisition; Phase 10 clean-machine launch.

### Pitfall 14: Performance passes by removing required behavior

**What goes wrong:** A headless server meets tick rate while rendered clients fail, or benchmarks delete persistent loot and disable equipment/AI to reach 30 FPS.

**Why it happens:** Small demos hide layered rendering, per-frame inventory scans, crowd queries, network payload growth and save/join spikes. CI hardware and localhost networking are substituted for the creator's machine and a real mixed-OS LAN.

**How to avoid:** Record hardware, OS, renderer, resolution, content/configuration and actual displayed FPS. Use the spec's four-player, approximately 100-survivor/300-active-zombie dense scenario with required jobs, combat, gear, nested inventories, stash use and corpses enabled. Measure the ten-minute sampling gate, save/join freezes and three-hour saved/resumed endurance separately. Optimize measured hotspots through bounded queries, caching with invalidation, UI pagination, scheduled decisions, batched presentation and compact state deltas while preserving required behavior and belongings. Local four-process tests and CI provide regressions; real discovery/manual joining on Windows/Linux machines and creator-PC rendering retain separate acceptance records. Spec §14.2/A-38/A-47 defines these gates.

**Warning signs:** Report has average FPS only; headless frame rate; one controlled actor; population generator omits inventories; corpse pruning; a graphics option hides required equipment; accelerated soak replaces the required real-duration endurance run.

**Phase to address:** Phase 4 crowd fixture; Phase 5 measurement support; Phases 6–9 incremental budgets; Phase 10 release proof.

## Technical Debt Patterns

| Shortcut | Immediate Benefit | Long-term Cost | When Acceptable |
|----------|-------------------|----------------|-----------------|
| Temporary base artwork | Unblocks movement/LAN | Forgotten placeholder or wrong attachment contract | Phases 2–3 only, tracked into Phase 4; no claim of final asset completion |
| Headless simulation harness | Fast reliable domain/engine checks | Uncovered rendering, focus, input, audio | Always as one evidence lane, never sole E2E acceptance |
| Direct commands for fixture setup | Reproducible large states | Tests bypass production interactions | Setup and targeted domain tests; retain production input E2E and unaided full campaigns |
| Inventory stored inside presentation nodes | Easy inspector access | Save/network lifetime and ownership rewrites | Never as the sole authoritative model |
| Copy donor gameplay wholesale | Rapid prototype | Excluded mechanics and Unity dependencies | Reference/prototype only; production adaptations need spec comparison |
| Broad analyzer exceptions | Quick green build | Rules lose enforcement credibility | Never; exact scoped, proven compatibility decisions only |
| Defer save field design until Phase 9 | Fewer early DTOs | Missing identities, queues and legacy geometry | Never for authoritative state; full save UX still belongs to Phase 9 |

## Integration Gotchas

| Integration | Common Mistake | Correct Approach |
|-------------|----------------|------------------|
| Godot RPC | Correct method but mismatched node path; host applies local and remote paths twice | Stable RPC endpoints, deliberate authority/call-local semantics, host included in process tests |
| Replicated scene state | Send a Resource or native object instance ID | Portable stable domain/content IDs and explicit schema |
| LAN service | Discovery equated with transport; loopback proves the network | Test broadcast/session list and manual address separately across actual Windows/Linux machines; show actionable connection errors |
| Test runner | Assume current documentation flags work in selected SDK | Pin runner and prove zero/skip/filter/error behavior with deliberate failing fixtures |
| CI artifacts | One found file proves all four process logs exist | Validate exact manifest, freshness, run ID and per-process completeness before upload |
| Asset imports/exports | Editor cache or absolute donor path supplies missing data | Clean import/acquisition, selected packaged resources, export launch tests |

## Performance Traps

Thresholds below describe project stress scenarios, not measured engine limits.

| Trap | Symptoms | Prevention | When It Breaks |
|------|----------|------------|----------------|
| Every actor scans every actor every tick | CPU spikes at a battlefront | Side/range indexes, bounded sensing and measured scheduling preserving behavior | Approximately 400 active survivors/zombies |
| Every frame rebuilds nested inventory/weight and gear layers | Stutter during stash/corpse access | Revision-based cached aggregates and appearance changes with tested invalidation | Nested bags, four users and automatic NPC gear/hauling |
| Full world broadcast per frame | Reliable queue delay affects control/transactions | Separate bounded state updates from reliable commits and snapshot traffic | Four peers plus late join under battle load |
| One active physics/script node per inactive loot item | Increasing CPU/memory despite quiet world | Persist complete data; activate presentation/interaction as needed without deleting belongings | Three-hour run with at least 200 nonempty bodies/piles over its course |
| Synchronous snapshot serialization/compression | Save/join freezes | Coherent capture, immutable background encoding, measured capture/catch-up budgets | Representative maximum-state save/late join; compare against 0.5-second freeze limit |
| Hundreds of equipment layers ignored in early benchmarks | GPU/draw overhead absent from test evidence | Real fully dressed crowd fixture at 1920×1080 | Dense 100-survivor/300-zombie scene |

## Security Mistakes

These are LAN authority and data-integrity concerns, not a proposal for accounts or online services.

| Mistake | Risk | Prevention |
|---------|------|------------|
| Trust client-supplied owner, damage, price, position or outcome | High: arbitrary ownership and camp mutations | Resolve sender to controlled actor; validate intent against current host state |
| Accept commands before snapshot readiness or after session replacement | High: stale actions mutate wrong world | Session/generation IDs, readiness gate, sequence/idempotency checks |
| Recursively decode unbounded bag/snapshot graphs | High: cycles, hangs, memory exhaustion | Bounded decoding, reference validation, cycle detection and explicit incompatibility errors |
| Expose fixture/debug commands in production sessions | High: unintended item creation or control | Isolate test transport/control and verify release configuration |
| Treat a display name or transient peer ID as persistent identity | Medium: reconnect steals another survivor | Stable local identity association plus current host ownership checks; document trust limits of account-free LAN |

## UX Pitfalls

| Pitfall | User Impact | Better Approach |
|---------|-------------|-----------------|
| An inventory panel implies pause | Character dies without understandable feedback | Keep combat/critical warnings visible; describe active world behavior |
| Dragging holds the authoritative item | Closing/access loss deletes belongings | Preview only until host accepts the destination |
| Silent conflict correction | Player cannot tell why purchase/loot failed | Reason plus updated authoritative contents, no charge on failure |
| Successor camera/crown updates before ownership settles | Two players appear to control one survivor | Commit succession once, then update each player's camera and marker |
| Colors alone identify four players | Crowded cooperative play becomes ambiguous | Number/name markers as required by asset plan |
| Audio-only attack or connection warning | Muted players miss critical events | Persistent visible feedback plus sound |
| Legacy inventory silently rearranges | Players lose access/trust after configuration change | Clearly marked old layout, readable contents and deliberate extraction |

## "Looks Done But Isn't" Checklist

- [ ] **Rules:** Real Godot-generated and handwritten C# both build under scoped rules; positive and negative fixtures prove enforcement; scene instantiation is tested.
- [ ] **Menu/movement:** Visible menu starts the intended scene; actual remapped keyboard input moves the correct actor and GUI focus does not leak gameplay actions.
- [ ] **Four players:** Host plus three real clients complete readiness; each independently acts; observations converge; every process contributes fresh evidence.
- [ ] **Runtime testing:** Expected required case IDs all executed successfully; none silently skipped, undiscovered or replaced by domain-only cases; fault-injection proves the gate fails.
- [ ] **Assets:** Exported/imported usable variants satisfy resource obligations; gear fits moving/dying bodies; audio was auditioned; source evidence labels remain accurate.
- [ ] **Belongings:** No duplicate owner/location; nested bags, physical money, quick references, corpse transfers and NPC cargo survive races and failure.
- [ ] **Combat:** Character ammunition is genuinely consumed; actual worn/broken gear affects outcomes; tower guards retain their explicit structural exception.
- [ ] **Queues/time:** Dawn, warning, wave, retaliation and paid-work state survive save/retry exactly once; destroyed portals never spawn again.
- [ ] **Late join/load:** No new kit, reroll, healing, old event replay or world rollback; readiness gates control; interrupted save preserves prior valid data.
- [ ] **Legacy state:** Changed grids/stacks/slots/depth remain visible/removable and persist until resolved; missing definitions fail clearly.
- [ ] **Delivery:** Fresh packaged Windows/Linux builds run without editor, SDK, donor folder or developer import caches.
- [ ] **Release:** Full unaided solo and four-player campaigns, real mixed-OS LAN, creator-PC rendering gate and required three-hour endurance each have their own evidence; all required features remain enabled.

## Recovery Strategies

| Pitfall | Recovery Cost | Recovery Steps |
|---------|---------------|----------------|
| Analyzer conflict discovered after broad suppression | MEDIUM | Restore scope inventory; add minimal real-scene reproduction; adapt one rule at a time with positive/negative proofs |
| False-green CI | MEDIUM | Invalidate affected acceptance claims; introduce manifest reconciliation and deliberate failure fixtures; rerun required suites |
| Item duplication or mixed snapshots | HIGH | Retain failing save/traces; audit stable identities and ownership graph; centralize atomic commits; reproduce races before trusting further saves |
| Lost queue/paid-work state | HIGH | Add versioned commitment/event fields and transition tests; preserve source saves; explicitly reject unsupported/corrupt state instead of guessing |
| Asset fit discovered late | HIGH | Return to one fully fitted body/action matrix; repair shared anchors/layers before regenerating all variants |
| Donor-path dependency | MEDIUM | Inventory actual consumed inputs; export/package selected dependencies; verify clean build and launch without donor access |
| Crowd performance failure | MEDIUM–HIGH | Profile full scenario; optimize measured hot paths; rerun same feature-complete benchmark and conservation checks; never delete required state |

## Pitfall-to-Phase Mapping

| Pitfall | Prevention Phase | Verification |
|---------|------------------|--------------|
| 1. Godot-incompatible C# rules | 1 | Real-scene compile/load/unload; generated binding and nullable fixtures; compliant/noncompliant rule proofs |
| 2. Thread/lifetime races | 1, 3, 9 | Disconnect/menu exit during async work; no stale callback or off-thread scene mutation |
| 3. False-green testing | 1–3 smoke; 5 full harness | Missing engine/zero/skipped/crash/timeout/report fault injection; expected-ID reconciliation |
| 4. Headless/input blind spots | 2, 5, 9 | Visible menu/input/rebinding; GUI event interception; A-24/A-46 |
| 5. Unproven host plus three clients | 3, 5, 9 | Independent input/identity/readiness; A-38–A-40; real LAN in 10 |
| 6. Physics lockstep assumption | 3, 5 | Exact domain invariants and bounded movement convergence across OS/processes |
| 7. Nontransactional inventories | 6, 7 | A-22/A-25/A-28/A-29 with concurrent players and NPCs |
| 8. Invented/deleted death loot | 6–8 | A-14/A-21/A-26/A-27/A-31/A-36; endurance in 10 |
| 9. Incoherent save/late join | 3, 6–9 | A-38/A-39/A-41/A-44/A-45 with mutations during capture/catch-up |
| 10. Repeated time/queue/work events | 7–9 | A-03/A-09/A-30/A-34/A-35/A-41/A-42 boundary/retry tests |
| 11. Destructive configuration migration | 6, 9 | A-42–A-44 legacy extraction/resave/error fixtures |
| 12. Unusable asset variants | 4, 6–9 | Resource-ID crosswalk and fitted animation/corpse/tier/audio inspection fixtures |
| 13. Nonportable donor dependency | 4, 5, 10 | Clean acquisition/import/export and installed-player launch on both OSes |
| 14. Misleading performance pass | 4–9 incremental; 10 gate | A-47 specified samples, spikes, full feature configuration and endurance record |

Deep phase research is most valuable for Phase 1 engine/analyzer compatibility, Phase 5 process/input/result harness, Phase 6 transactional containment, and Phase 9 snapshot/commitment/legacy compatibility. Performance planning begins early, but its final verdict requires the actual content and reference hardware. The unmeasured maximum snapshot size, target-PC hardware, chosen renderer/runner behavior and full equipment-fit workload remain empirical gaps, not reasons to reduce scope.

## Sources

### Local requirements and inspected provenance

- [PROJECT.md](../PROJECT.md): scope, fixed phases, verification and architectural constraints.
- [0.1 specification](../../specs/0.1/initial-spec.md): §§2–14 and A-01–A-47; authoritative behavior, proposed balance and release gates.
- [Asset/resource plan](../../specs/0.1/assets-and-resources.md): evidence labels, source locations, modular-fit coverage and production checks. Donor metadata statements here are attributed to that inspection, not independently revalidated exports.
- [LangProj code style](D:/Work/SaaS/LangProj_monorepo/src/backend/docs/CodeRules/code-style.md), [rule matrix](D:/Work/SaaS/LangProj_monorepo/src/backend/docs/rule-matrix.md), [build properties](D:/Work/SaaS/LangProj_monorepo/src/backend/lang-proj-core/Directory.Build.props): inspected inheritance constraints; backend assumptions require explicit adaptation.

### Primary technical documentation

All accessed 2026-11-13; MEDIUM from the verified-websearch confidence seam. Godot `stable` pages are moving documentation, so implementation must check the pinned engine's corresponding reference. Microsoft runner guidance is version-sensitive; no unsupported CLI option is prescribed by this report.

- [Godot GD0001](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/diagnostics/GD0001.html): source-generated partial bindings.
- [Godot Node](https://docs.godotengine.org/en/stable/classes/class_node.html): lifecycle, processing and node removal.
- [Godot thread-safe APIs](https://docs.godotengine.org/en/stable/tutorials/performance/thread_safe_apis.html): active scene-tree thread boundary.
- [Godot C# signals](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html): subscription lifetime exceptions.
- [Godot high-level multiplayer](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html): RPC paths, authority, local execution and transport channels.
- [Godot MultiplayerSynchronizer](https://docs.godotengine.org/en/stable/classes/class_multiplayersynchronizer.html): unsupported object/resource and peer-specific property identities.
- [Godot command line](https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html): headless behavior, export and silently ignored unknown flags.
- [Godot Input](https://docs.godotengine.org/en/stable/classes/class_input.html), [InputEvent routing](https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html): action injection versus event propagation and GUI handling.
- [Microsoft VSTest/MTP migration](https://learn.microsoft.com/en-us/dotnet/core/testing/migrating-vstest-microsoft-testing-platform), [MTP CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test-mtp): distinct zero/skipped-test behavior and version-sensitive result policy.
- [GitHub upload-artifact](https://github.com/actions/upload-artifact): missing-artifact policy; exact evidence completeness remains the project's responsibility.

---
*Pitfalls research for: KingdomZombies complete 0.1*
*Researched: 2026-11-13*
