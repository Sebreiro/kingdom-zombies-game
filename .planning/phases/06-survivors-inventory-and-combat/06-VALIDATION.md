---
phase: "06"
slug: survivors-inventory-and-combat
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-24"
---

# Phase 06 Validation Strategy

Planning baseline is the Phase 5 merge `b9aaa2b`. The baseline shared PR smoke passed 4/4 at invocation `87cbb73a-e93f-412c-b053-fa41b043f8df`, recorded by the orchestrator. The counts below describe planned coverage. Phase 6 implementation, runtime and visual outcomes remain pending.

## Test Infrastructure and Feedback

The pinned .NET/xUnit Tier 1 test project, existing shared game-tests catalog, headless production runner, scenario adapter and bounded native process/input helpers supply the test infrastructure. Prepare the current assembly with `node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build`. After each task run its focused check in the PLAN. After each plan run its shared PR or named headless check on the current source, retaining evidence identity. After integration run `node src/godot-game/tools/game-tests/edge-check.cjs 6`. The five-second prepared Tier 1 and two-minute Critical render budgets from Phase 5 are measurement targets. Native launches and the complete phase-end selection have their declared timeouts.

Before the first real Windows game window, complete all implementation, whole-phase review and fixes. Bind local review JSON to the full current clean revision and use 1280×720. Use `node src/godot-game/tools/game-tests/check.cjs --phase-end --phase 6 --review .local/temp/phase06-review.json`. Keep the existing fixed Critical, new/changed and affected selectors. Retain all omissions and detect unclassified impacts. Preserve original failed/blocked receipts, re-review the repair, run failed/blocked and newly affected checks, then rerun the entire selected set once on the repaired build. Record exact commands, role/launch counts, invocation, current-source hash, images, statuses and cleanup before workspace cleanup. Host basic Windows and Ubuntu receipts retain separate provenance.

## Scope and dependency audit

The [roadmap](../../ROADMAP.md#phase-6-survivors-inventory-and-combat) goal and its five success checks are carried by 01–02/05/10/12, 02–03/10–11, 01/04/09/11, 05–06/10/12–13, and 07–10/12–13 respectively. Plans 14–15 close their evidence. The table below covers [approved requirements](../../REQUIREMENTS.md), all locked statements in [06-SPEC](06-SPEC.md), and all 132 resolved edges. [06-CONTEXT](06-CONTEXT.md) and [06-UI-SPEC](06-UI-SPEC.md) coverage follows below. [06-RESEARCH](06-RESEARCH.md) decisions and constraints reconcile as follows.

| Research item | Plan response |
| --- | --- |
| Single authoritative host elapsed and real geometry before consumers | 01 then 04–07; one post-boundary receipt and exact semantic tests |
| Strict authored typed content and normal eight/one startup, including over-one-page kit | 02–04; actual packaged-read proof in 14 |
| Persistent run and binding vs control on readiness, join, death, release | 02, 07, 09; current role/session tests in 13–15 |
| Candidate graph, deterministic page/rotation/contact and exact quantities | 03–08 |
| Public vs private scoped 32768-byte frames, stale revocation and corrected outcomes | 04, 09; proof on actual captured bytes, over-frame gameplay and four roles |
| Complete detached restore, saved geometry, separate durable/view clocks and RNG | 08–09 |
| Exact R-34 policy and reviewed cross-project public contracts | 01–02, 09, 14; use actual JSON and pinned scanner |
| Authored living/corpse, UI state/input priority and allowed camera | 10–12 |
| Reuse current Phase 05 scenario adapter, native/physical evidence and exact owner activation | 13–15 |
| Preserve aggregate/later owner scope, strict review and source/package closure | 13–15 and the scope table below |

| Plan | Wave | Dependency | Capability and owned handoff |
| --- | ---: | --- | --- |
| 06-01 | 1 | Phase 5 | Native host-tick/safety/access tracer; first implementation commit opens draft |
| 06-02 | 2 | 01 | Configured gameplay definitions, run-owned roster/kit and binding leases |
| 06-03 | 3 | 02 | Equipment, storage graph, stack and carried-reference rules |
| 06-04 | 4 | 03 | Local access, bounded consequences, deterministic stash and corrections |
| 06-05 | 4 | 03 | Locomotion, load, vitals, safety and real food actions |
| 06-06 | 5 | 04, 05 | Weapons, projectiles, armor and damage |
| 06-07 | 6 | 06 | Death, corpse/pile ownership, cleanup and exclusive Respawn |
| 06-08 | 7 | 07 | Strict full snapshot and saved layout preservation |
| 06-09 | 8 | 08 | Stable transport binding, command dispatcher and scoped publication |
| 06-10 | 9 | 09 | Native world actions, body, equipment and corpse projection |
| 06-11 | 10 | 10 | Inventory workspace, stash/loot, previews and corrected interaction |
| 06-12 | 11 | 11 | Live HUD, feedback, formal spectator controls and camera |
| 06-13 | 12 | 12 | Fixture, exact edge census and shared native scenario drivers |
| 06-14 | 13 | 13 | Packaged subset, asset handoff activation and full code review |
| 06-15 | 14 | 14 | Current grouped rendered acceptance, proof reconciliation and PR readiness |

Plan file names, private type decomposition and numeric tolerances are implementation choices. For a needed amendment, preserve the semantic contract, record the reason and evidence in the producing SUMMARY, and update dependent plan ownership/interfaces before executing that wave. Locked behaviors and UI decisions retain their definitions. Wave 4 plans write different files. Keep a single committed simulation root and treat each extra consumer update as a projection of the same revision.

## Requirement and unit-edge ownership

| Locked statement | Behavior | Approved source IDs | Plan IDs | Exact Tier 1 edge identities |
| --- | --- | --- | --- | --- |
| R01 | World time, safety and access | Phase goal | 01, 05, 07 | E001–E006 |
| R02 | Persistent population and one configured kit | CHAR-01 | 02 | E007–E013 |
| R03 | Movement and stamina | CHAR-02, CHAR-03 | 05, 10 | E014–E017 |
| R04 | Recovery and carried food | CHAR-04, CHAR-05 | 05, 12 | E018–E021 |
| R05 | Nested load | CHAR-06 | 05, 12 | E022–E028 |
| R06 | Persistent item identities and equipment | INV-01, INV-02 | 02, 03, 11 | E029–E035 |
| R07 | Placement, rotation and manual replacement | INV-03 | 03, 11 | E036–E042 |
| R08 | Exact stacks | INV-04 | 03, 11 | E043–E049 |
| R09 | Nested and filled storage | INV-05, INV-06 | 03, 11 | E050–E056 |
| R10 | Quick food references | INV-07 | 03, 05, 11, 12 | E057–E063 |
| R11 | Permitted access and safe dragging | INV-08, INV-09 | 04, 09, 11 | E064–E069 |
| R12 | Unlimited paged stash | INV-10 | 04, 09, 11 | E070–E073, E075–E077 |
| R13 | Authoritative outcome | INV-11 | 04, 09 | E078–E079 |
| R14 | Facing-directed melee | COMBAT-01, COMBAT-02 | 06 | E080–E086 |
| R15 | Mouse bow, projectile and defense | COMBAT-03, COMBAT-06 | 06, 09, 10 | E087–E093 |
| R16 | Armor and retained broken items | COMBAT-05 | 06 | E094–E098 |
| R17 | Equipped appearances | COMBAT-08 | 10, 12, 14 | E099–E103 |
| R18 | Corpse, ground loot and cleanup | INV-12, INV-13 | 07, 11 | E104–E110 |
| R19 | Control, succession and spectating | RUN-01, RUN-02, RUN-03, RUN-04 | 02, 07, 09, 12 | E111–E117 |
| R20 | Complete snapshot and saved layout | Phase goal | 08, 09 | E118–E124 |
| R21 | Playable current interfaces | Phase goal | 10, 11, 12 | E125–E128 |
| R22 | Combat fixture and repeatable production proof | Phase goal | 13, 14, 15 | E129–E133 |

All ranges are inclusive. Each E-ID has a uniquely named `Phase06_E###_Description` Tier 1 test method with meaningful production assertions. E074 is declared deferred to Phase 12 INV-14 and gets no Phase 6 method or pass credit. The actual shared inventory check rejects omissions, duplicates, unknown IDs and the deferred ID. Wave 0 allocation is present here. Test implementation occurs in the owning behavior tasks using existing infrastructure rather than empty scaffold methods.

## Per-Task Verification Map

Each task's automated command, explicit failure direction and planned files are in its linked PLAN. Where a class/driver is declared by a task it is required output and no current existence/result is implied. Platform/package/preparation and review requirements are defined in the respective PLAN. Tests are measured against the exact command and selected identity, with skip, zero and incorrect-role outcomes counted as non-success.

| Task ID | Wave | Source IDs | Threat ref | Expected behavior / verification target | Status |
| --- | ---: | --- | --- | --- | --- |
| 06-01-1 | 1 | CHAR-04, INV-08, INV-09, INV-10, INV-11 | T-06-01, T-06-02 | Advance the real session and resolve local refuge and access | Pending |
| 06-01-2 | 1 | CHAR-04, INV-08, INV-09, INV-10, INV-11 | T-06-01, T-06-02 | Exercise defense loss, access loss and exact tick validation | Pending |
| 06-01-3 | 1 | CHAR-04, INV-08, INV-09, INV-10, INV-11 | T-06-01, T-06-02 | Prove native focus, shutdown and fresh seeded observations | Pending |
| 06-02-1 | 2 | CHAR-01, INV-01, INV-02, RUN-01 | T-06-03, T-06-04 | Start a real run with the configured population and kit | Pending |
| 06-02-2 | 2 | CHAR-01, INV-01, INV-02, RUN-01 | T-06-03, T-06-04 | Bind and release current recruits without recreating state | Pending |
| 06-03-1 | 3 | INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07 | T-06-05 | Commit real slot and grid placement with exact split and merge | Pending |
| 06-03-2 | 3 | INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07 | T-06-05 | Move complete container trees and keep exact carried references | Pending |
| 06-04-1 | 4 | INV-08, INV-09, INV-10, INV-11 | T-06-06, T-06-07 | Open and transfer only among currently permitted local targets | Pending |
| 06-04-2 | 4 | INV-08, INV-09, INV-10, INV-11 | T-06-06, T-06-07 | Deposit, withdraw and browse the real paged stash | Pending |
| 06-05-1 | 4 | CHAR-02, CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07 | T-06-08 | Move with configured load and pay real stamina | Pending |
| 06-05-2 | 4 | CHAR-02, CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07 | T-06-08 | Recover from elapsed safe rest or consume one actual carried unit | Pending |
| 06-06-1 | 5 | COMBAT-01, COMBAT-02, COMBAT-03, COMBAT-05, COMBAT-06 | T-06-09, T-06-10 | Perform configured fists and single or simultaneous dual melee | Pending |
| 06-06-2 | 5 | COMBAT-01, COMBAT-02, COMBAT-03, COMBAT-05, COMBAT-06 | T-06-09, T-06-10 | Launch and resolve real bow arrows and obstruction | Pending |
| 06-07-1 | 6 | INV-11, INV-12, INV-13, RUN-01, RUN-02, RUN-03, RUN-04 | T-06-11, T-06-12 | Leave actual belongings on death and clean only an empty target | Pending |
| 06-07-2 | 6 | INV-11, INV-12, INV-13, RUN-01, RUN-02, RUN-03, RUN-04 | T-06-11, T-06-12 | Request Respawn explicitly and lease an actual successor | Pending |
| 06-08-1 | 7 | CHAR-01, INV-05, INV-06, INV-11, INV-12, INV-13, RUN-02, RUN-04 | T-06-13 | Restore clean run continuation with the same belongings and timers | Pending |
| 06-08-2 | 7 | CHAR-01, INV-05, INV-06, INV-11, INV-12, INV-13, RUN-02, RUN-04 | T-06-13 | Reject inconsistent restores and capture one coherent revision | Pending |
| 06-09-1 | 8 | INV-08, INV-09, INV-10, INV-11, RUN-01, RUN-02, RUN-03, RUN-04, COMBAT-03 | T-06-14, T-06-15, T-06-16 | Bind one owned transfer to stable ingress before changing the wire | Pending |
| 06-09-2 | 8 | INV-08, INV-09, INV-10, INV-11, RUN-01, RUN-02, RUN-03, RUN-04, COMBAT-03 | T-06-14, T-06-15, T-06-16 | Version and bound public and private frames with explicit revocation | Pending |
| 06-09-3 | 8 | INV-08, INV-09, INV-10, INV-11, RUN-01, RUN-02, RUN-03, RUN-04, COMBAT-03 | T-06-14, T-06-15, T-06-16 | Prove actual recipient bytes and same-path authoritative correction | Pending |
| 06-10-1 | 9 | CHAR-02, COMBAT-03, COMBAT-08, INV-12, RUN-01 | T-06-17, T-06-18 | Render actual living or corpse equipment and player crowns | Pending |
| 06-10-2 | 9 | CHAR-02, COMBAT-03, COMBAT-08, INV-12, RUN-01 | T-06-17, T-06-18 | Send fresh movement, interaction and mouse-aimed combat through the normal scene | Pending |
| 06-11-1 | 10 | INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, INV-08, INV-09, INV-10, INV-11, CHAR-05 | T-06-19, T-06-20 | Browse and edit current carried items and nested containers | Pending |
| 06-11-2 | 10 | INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, INV-08, INV-09, INV-10, INV-11, CHAR-05 | T-06-19, T-06-20 | Browse and safely transfer near stash, NPCs, corpses and piles | Pending |
| 06-12-1 | 11 | CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07, COMBAT-03, COMBAT-08, RUN-01, RUN-02, RUN-03, RUN-04 | T-06-21 | Read live vitals, ready gear, feedback and inventory priority | Pending |
| 06-12-2 | 11 | CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-07, COMBAT-03, COMBAT-08, RUN-01, RUN-02, RUN-03, RUN-04 | T-06-21 | Spectate living teammates and request actual control handover | Pending |
| 06-13-1 | 12 | All 29 Phase 6 source IDs | T-06-22, T-06-23 | Play one independent seeded combat fixture with actual targets and gear | Pending |
| 06-13-2 | 12 | All 29 Phase 6 source IDs | T-06-22, T-06-23 | Save and execute headless normal, fixture and local LAN rows | Pending |
| 06-13-3 | 12 | All 29 Phase 6 source IDs | T-06-22, T-06-23 | Declare ordered rendered solo and four-peer production cases | Pending |
| 06-14-1 | 13 | INV-01, INV-08, INV-10, INV-11, COMBAT-08, RUN-01, RUN-04 | T-06-24, T-06-25 | Activate exact original inspection and packaged gameplay cases | Pending |
| 06-14-2 | 13 | INV-01, INV-08, INV-10, INV-11, COMBAT-08, RUN-01, RUN-04 | T-06-24, T-06-25 | Review the whole phase and clear first real game launch | Pending |
| 06-15-1 | 14 | All 29 Phase 6 source IDs | T-06-26 | Run fresh reviewed 1280×720 solo, four-peer LAN and asset groups | Pending |
| 06-15-2 | 14 | All 29 Phase 6 source IDs | T-06-26 | Reconcile Phase 6 acceptance, hosted basic jobs and same-draft readiness | Pending |

## Locked acceptance crosswalk

| Locked AC | Producing plan IDs | Headless or executable proof group | Rendered companion | Required subject |
| --- | --- | --- | --- | --- |
| AC-01 | 01 | `phase06.headless.world` | `phase06.solo.world` | Current camp borders, reach and danger through real Step |
| AC-02 | 01, 05, 07 | `phase06.headless.world`, `phase06.headless.lifecycle` | `phase06.solo.life` | Authority and exact elapsed deadlines |
| AC-03 | 02 | `phase06.headless.start` | `phase06.lan.population` | Eight distinct outfits and one configured kit, all participant counts |
| AC-04 | 02, 03 | `phase06.headless.start`, `phase06.headless.inventory` | `phase06.solo.inventory` | 24 definitions and eleven real equip slots |
| AC-05 | 05, 10 | `phase06.headless.world` | `phase06.solo.movement` | Walk, sprint, jump, air control and input boundaries |
| AC-06 | 05, 06 | `phase06.headless.combat` | `phase06.solo.combat` | Stamina cost, regeneration, and no queued attacks |
| AC-07 | 01, 05 | `phase06.headless.world` | `phase06.solo.movement` | Continuous safe time and exact health recovery |
| AC-08 | 03, 05, 12 | `phase06.headless.inventory` | `phase06.solo.movement` | Carried food, full vitals and one personal cooldown |
| AC-09 | 05, 12 | `phase06.headless.inventory` | `phase06.solo.movement` | Nested load thresholds and exact modifiers |
| AC-10 | 03, 11 | `phase06.headless.inventory` | `phase06.solo.inventory` | Exact fit/rotation and manual occupied-slot replacement |
| AC-11 | 03, 11 | `phase06.headless.inventory` | `phase06.solo.inventory` | Integral split and partial merge identity |
| AC-12 | 03 | `phase06.headless.inventory` | `phase06.solo.inventory` | Real nesting, deepest descendant and cycle rules |
| AC-13 | 03, 04 | `phase06.headless.inventory` | `phase06.solo.inventory` | Filled moves/drop and carried reference lifetime |
| AC-14 | 03, 05 | `phase06.headless.inventory` | `phase06.solo.inventory` | Four exact carried reference identities |
| AC-15 | 04, 09, 11 | `phase06.headless.lan-privacy` | `phase06.solo.stash` | Current target permission, range, reach and selection |
| AC-16 | 03, 04, 11 | `phase06.headless.inventory` | `phase06.solo.inventory` | Preview/cancel and explicit Drop only |
| AC-17 | 04, 09 | `phase06.headless.inventory` | `phase06.solo.stash` | Unlimited pages and checked legal capacity |
| AC-18 | 04, 11 | `phase06.headless.inventory` | `phase06.solo.stash` | Deterministic stacks first, rotated cells, then new page |
| AC-19 | 04, 09 | `phase06.headless.lan-privacy` | `phase06.lan.conflicts` | Same authoritative current outcome for repeated or contested actions |
| AC-20 | 06 | `phase06.headless.combat` | `phase06.solo.combat` | Real fists, single/dual cost, target order and wear |
| AC-21 | 06, 10, 12 | `phase06.headless.combat` | `phase06.solo.combat` | Actual hand ammo, mouse aim and quiet empty bow |
| AC-22 | 06 | `phase06.headless.combat` | `phase06.solo.combat` | Ally passthrough, hostile defense, terrain and stable first hit |
| AC-23 | 06 | `phase06.headless.combat` | `phase06.solo.combat` | Exact raw-damage fractional armor wear |
| AC-24 | 06, 10, 11 | `phase06.headless.combat` | `phase06.solo.combat` | Retained broken items and one usable weapon |
| AC-25 | 10, 12, 14 | `phase06.headless.combat` | `phase06.solo.combat` | Committed living/corpse gear and crown layering |
| AC-26 | 07, 10 | `phase06.headless.lifecycle` | `phase06.solo.life` | Exact actual remaining body/loot graph |
| AC-27 | 07 | `phase06.headless.lifecycle` | `phase06.solo.life` | Continuous empty cleanup and repopulation reset |
| AC-28 | 07, 09, 12 | `phase06.headless.lifecycle`, `phase06.headless.lan-privacy` | `phase06.lan.succession` | Explicit gate, random real successor, exclusivity and camera |
| AC-29 | 02, 07, 09, 12 | `phase06.headless.lifecycle`, `phase06.headless.lan-privacy` | `phase06.lan.succession` | Waiting priority, preserved guest survivor and explicit follow |
| AC-30 | 08 | `phase06.headless.lifecycle` | `phase06.solo.life` | Exact detached full-state continuation |
| AC-31 | 08 | `phase06.headless.inventory` | `phase06.solo.inventory` | Saved geometry and legacy representation |
| AC-32 | 08, 09 | `phase06.headless.lan-privacy` | `phase06.lan.conflicts` | Atomic rejection, coherent capture and replay handling |
| AC-33 | 10, 11, 12 | `phase06.headless.inventory`, `phase06.headless.lifecycle` | `phase06.solo.inventory`, `phase06.lan.succession` | Usable real HUD, inventory, stash and spectator at 1280×720 |
| AC-34 | 10, 11, 12 | `phase06.headless.lan-privacy` | `phase06.lan.conflicts` | Panels and focus, real time and visible corrections |
| AC-35 | 06, 07, 13 | `phase06.headless.world`, `phase06.headless.combat`, `phase06.headless.lifecycle` | `phase06.solo.combat` | Stationary proof fixture through production rules |
| AC-36 | 13, 14 | Phase 6 headless groups | Selected phase case set | Tracked executable scenarios and ownership-aware selection |
| AC-37 | 14, 15 | Phase 6 headless groups | Selected phase case set | Review first and fresh grouped run, repairs and evidence |
| AC-38 | 01–13 | Phase 6 headless groups | Selected phase case set | Zero/one/many, stable identity and enumeration order |
| AC-39 | 01–09, 13 | Phase 6 headless groups | Selected phase case set | Boundary/precision and illegal input preservation |
| AC-40 | 06, 07, 10, 13 | `phase06.headless.combat`, `phase06.headless.lifecycle` | `phase06.solo.combat`, `phase06.lan.succession` | P01 clean and deliberate violation recognition |
| AC-41 | 04, 09, 11, 13 | `phase06.headless.lan-privacy` | `phase06.lan.conflicts`, `phase06.solo.stash` | P02 clean and deliberate violation recognition |

Headless and visual companions do not replace the exact unit, codec, wire or control checks. Pure-only items such as precise saved layout continuation and malformed restore use the named tests as their authoritative outcome. Visual judgments may establish readability, state distinction, camera behavior, identity and layering only from current images and real actions.

## Locked UI decisions

| Decisions | Delivery | Evidence companion |
| --- | --- | --- |
| D-01, D-02, D-03, D-04, D-05, D-06, D-07 | 11 inventory/workspace | phase06.solo.inventory |
| D-08, D-14, D-37, D-38, D-40 | 10–12 focus, Escape, input and damage behavior | phase06.solo.inventory and phase06.lan.conflicts |
| D-09, D-10, D-11, D-12, D-13, D-15, D-16 | 11 tooltip, icon, and bag/slot behavior | phase06.solo.inventory |
| D-17, D-18, D-19, D-20, D-21, D-22, D-23, D-24 | 04/09/11 fixed external panes, current scope and stash | phase06.solo.stash and phase06.lan.conflicts |
| D-25, D-26, D-27, D-28 | 12 current vitals, damage and action cues | phase06.solo.combat and phase06.lan.conflicts |
| D-29, D-30, D-31, D-32, D-33, D-34, D-35, D-36 | 07/09/12 request, wait, follow, camera and current transfer | phase06.solo.life and phase06.lan.succession |
| D-39 | 05/11/12 carried Consume | phase06.solo.inventory |
| D-41, D-42, D-43, D-44 | 06/10/12 cursor, real arrows and quiet unloaded state | phase06.solo.combat |
| D-45, D-46, D-47, D-48 | 05/12 safety, load, stamina and reason | phase06.solo.movement |

Use approved UI-SPEC wording, 1280×720 native sizes, real equipped icons, local scroll and focus, explicit empty/partially filled/error/loading/overflow/zero-one-many/long-text branches, and distinct rendered input/evidence. D-18 retains unfiltered stash paging while INV-14 remains outside the Phase 6 implementation. No deferred workflow is activated.

## Scenario groups and active row contract

| Group / order | Cases | Actual subject and setup/reset |
| --- | --- | --- |
| phase06.headless 1–6 | world, start, inventory, combat, lifecycle, lan-privacy | Use one compatible headless production session and separate four-role case as needed. Start from normal menu, explicit debug fixture setup only for its rows, role-specific current views and owned shutdown. |
| phase06.solo 1–6 | world, movement, inventory, stash, combat, life | Normal production menu and one compatible real Windows viewport. Reset through ordinary controls to a submitted fresh fixture or exact normal start. Physical key/pointer evidence and decoded images for each row. |
| phase06.lan 1–3 | population, conflicts, succession | Actual host plus three guest processes using production menu/address/admission, distinct profiles and isolated ports. Reuse the compatible process set, reset/rejoin deliberately, capture current native evidence per role. |
| phase06.packaged | phase06.packaged | Pinned export runtime in clean isolated headless profile with real normal setup and exact required resource reads. |
| phase06.inspection | The 18 full shared IDs below | Original inspection controls and case-specific recipes. One compatible owned session with verified fixture resets, separate launch only if original precondition requires it. |

All listed group suffixes expand to their stated group plus a period and suffix. The durable 06-SCENARIOS definition produced by 13–14 gives importance/rationale, setup/seed, physical actions, exact outcome, order, roles, evidence and reset for each. Use Phase 5 scenario exports `open(context)`, `cases`, `reset(state, context)`, `close(state, context)`. An empty selection, unknown row, scope mismatch, invalid image, missing peer, wrong artifact provenance or stale revision has no pass credit. Record per-row inputs and actual current state, and keep headless synthetic input out of physical input claims.

### Source acceptance boundaries

| Game-spec cases | Phase 6 executable proof | Continuing scope |
| --- | --- | --- |
| A-01 | phase06.headless.start, phase06.lan.population | Full day-1 world and portals remain campaign work |
| A-04, A-05 | phase06.solo.life, phase06.lan.succession | Current death/explicit Respawn, preserved successor and exclusivity |
| A-06 | phase06.lan.succession | Purchased recruitment and population limit stay Phase 7 |
| A-14 | phase06.solo.combat | Shared actual ammo/condition here, autonomous refill/fallback Phase 8 |
| A-22, A-23 | phase06.solo.inventory and phase06.solo.movement | Actual inventory, broken gear and nested load |
| A-24 | phase06.solo.stash and phase06.lan.conflicts | Reachable permitted real transfer and world continuing |
| A-25 | phase06.solo.stash | Autonomous delivery and selling Phase 7 |
| A-26 | phase06.solo.life, phase06.solo.combat | Phase 7 carrier delivery and Phase 8 full enemy roster |
| A-27 | phase06.headless.lifecycle and phase06.solo.life | Nonempty persistence and resettable cleanup |
| A-28 | phase06.lan.conflicts and phase06.lan.succession | Later purchase, limit, upgrade and sale cases retain owners |
| A-31 | phase06.solo.combat | Built-in tower integration stays with structure/campaign work |
| A-43 | Tier 1 Phase06SnapshotTests and phase06.headless.inventory | Phase 9 owns interactive legacy extraction and save compatibility |

### Mechanical prohibition controls

| ID | check_kind | check_target | check_rule | check_violation_fixture | check_clean_fixture |
| --- | --- | --- | --- | --- | --- |
| P01 / AC-40 | Tier 1 comparator on actual Core combat, inventory and current-control receipts | `Phase06MeleeTests` and `Phase06SuccessionTests` | Allied health unchanged under a real ally contact. Crown never changes equivalent damage/equipment/loot counts. | `P01_ViolationControl_DetectsAllyDamageOrCrownBenefit` uses same comparator with deliberately inconsistent receipt | `P01_CleanControl_PreservesAlliesAndCosmeticCrown` observes the live production joint path |
| P02 / AC-41 | Tier 1 actual encoded and decoded recipient/correction proof | `Phase06PublicationTests` | An unrelated private item/stack/tree never appears in public or another recipient bytes and remote/blocked stash cannot be inspected or mutated. | `P02_ViolationControl_DetectsPrivateOrRemoteStashResult` uses a controlled inconsistent observation with the same comparator | `P02_CleanControl_PreservesPrivateAndLocalAccess` submits and inspects actual production public/private receipts |

06-06/07 and 06-09 supply the named methods and concrete outcomes, 06-13 supplies linked case descriptors, and 06-15 records actual execution. A clean control must exercise the production path, and its violation control must trip the same assertion without introducing forbidden production behaviors. Neither control receives evidence credit before execution.

### Exact Phase 4 handoff cases

Membership is from the current relocated [handoff register](../../../src/godot-game/tools/asset-pipeline/handoff.json). Original procedures and stimuli are from [04-SCENARIOS](../04-asset-production/04-SCENARIOS.md). Activate these exact shared IDs, preserving original native and source evidence, without duplicating rows or treating an unrelated gameplay screen as the original fixture.

| Declared asset ID | Exact shared catalog ID | Original native group/order | Current shared group |
| --- | --- | --- | --- |
| `heldout.H14` | `asset-pipeline.heldout.H14` | G1-hub / 1013 | `phase06.inspection` |
| `heldout.H16` | `asset-pipeline.heldout.H16` | G1-hub / 1015 | `phase06.inspection` |
| `heldout.H17` | `asset-pipeline.heldout.H17` | G1-hub / 1016 | `phase06.inspection` |
| `heldout.H18` | `asset-pipeline.heldout.H18` | G1-hub / 1017 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.empty` | `asset-pipeline.UIH.CHARACTER-PREVIEW.empty` | G1-hub / 1040 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.loading` | `asset-pipeline.UIH.CHARACTER-PREVIEW.loading` | G1-hub / 1041 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.error` | `asset-pipeline.UIH.CHARACTER-PREVIEW.error` | G1-hub / 1042 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.partial` | `asset-pipeline.UIH.CHARACTER-PREVIEW.partial` | G1-hub / 1044 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.overflow` | `asset-pipeline.UIH.CHARACTER-PREVIEW.overflow` | G1-hub / 1045 | `phase06.inspection` |
| `UIH.CHARACTER-PREVIEW.long-text` | `asset-pipeline.UIH.CHARACTER-PREVIEW.long-text` | G1-hub / 1046 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.empty` | `asset-pipeline.UIH.UI-COLLECTIONS.empty` | G1-hub / 1053 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.loading` | `asset-pipeline.UIH.UI-COLLECTIONS.loading` | G1-hub / 1054 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.error` | `asset-pipeline.UIH.UI-COLLECTIONS.error` | G1-hub / 1055 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.populated` | `asset-pipeline.UIH.UI-COLLECTIONS.populated` | G1-hub / 1056 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.partial` | `asset-pipeline.UIH.UI-COLLECTIONS.partial` | G1-hub / 1057 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.overflow` | `asset-pipeline.UIH.UI-COLLECTIONS.overflow` | G1-hub / 1058 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.zero-one-many` | `asset-pipeline.UIH.UI-COLLECTIONS.zero-one-many` | G1-hub / 1059 | `phase06.inspection` |
| `UIH.UI-COLLECTIONS.long-text` | `asset-pipeline.UIH.UI-COLLECTIONS.long-text` | G1-hub / 1060 | `phase06.inspection` |

## Review and validation sign-off

No additional package or runner scaffold is needed. Phase-end reviews require actual 1280×720 visual inspection of the solo, LAN and handoff subjects. Keep required hold/follow/camera and correction behavior linked to current case captures, and keep all later-phase and separate release environments with their owners. Current planning only. Leave Nyquist compliance and wave 0 completion flags unset until the named evidence is actually checked. No Phase 6 runtime, rendered or hosted pass has been declared in this artifact.
