# Phase 6: Survivors, inventory, and combat — Specification

**Created:** 2026-11-16 (Europe/Paris)
**Ambiguity score:** 0.036 (gate: ≤ 0.20; unrounded 0.0355)
**Requirements:** 22 locked statements covering all 29 Phase 6 source requirement IDs
**Status:** Requirements locked; implementation and gameplay verification pending.
**Dependency:** Phase 5 complete before Phase 6 implementation; this specification does not advance current execution.

## Goal

Solo players and a host with up to three LAN guests can control a shared population of eight ordinary survivors, manage the same real belongings, use configured movement/recovery/equipment in combat, and pass through death, looting, succession and spectating without duplicating or silently losing survivors, items or quantities.

## Background

The scout found implemented Phase 2/3 menu, horizontal movement, independent player input, LAN discovery/manual joining, shared movement authority and coherent roster publication. The retained Phase 3 verification records 21/21 must-haves and a local Windows selected execution of 481/481 cases; this historical foundation evidence is not Phase 6 acceptance.

`src/core/FoundationActorState.cs` contains movement identity, horizontal position, facing and elapsed time. `FoundationSession.cs` currently creates a temporary actor on participant admission and removes it on departure. `FoundationSnapshot.cs` publishes that participant roster; `SnapshotDto.cs` illustrates detached example-state serialization. These are integration foundations, not a persistent survivor population or an item/combat system.

The scout found no production survivor health/stamina, vertical movement, recovery, item instances, equipment/grid/container graph, stash, combat, corpse cleanup, succession or spectator system. Phase 4 asset production is still executing, with partial content/inspection work and final asset acceptance pending. Phase 5's unified verification framework is planned and remains a prerequisite. Content manifests, example snapshots and earlier tests do not establish these missing behaviors.

The creator selected stationary hostile targets with explicitly triggered attacks for the Phase 6 playable combat slice. Enemy approach/pursuit, autonomous camp activity and the full campaign are later increments. Complete saved-layout representation begins here; changed-configuration legacy viewing/extraction remains Phase 9.

Scope authority: [approved requirements](../../REQUIREMENTS.md), [Phase 6 roadmap entry](../../ROADMAP.md#phase-6-survivors-inventory-and-combat), [project definition](../../PROJECT.md), [current state](../../STATE.md), [game specification](../../../specs/0.1/initial-spec.md), [asset plan](../../../specs/0.1/assets-and-resources.md), [architecture index](../../../docs/architecture/README.md) and [local testing strategy](../../../docs/workflow/local-testing.md). Canonical numerical tables and exclusions remain binding working baselines; this document does not turn proposed balance into proven playtest results.

## Requirements

1. **R01 — World time, safety and access:** Authoritative elapsed time, camp safety and reachable local access govern recovery, cooldowns, corpse cleanup and interactions in solo and LAN play.
   - Source: Roadmap Phase 6 delivery boundary; prerequisite/integration obligation, without transferring later-phase requirement ownership.
   - Current: Core has explicit elapsed movement and identity examples; the foundation has no production camp safety, attack danger, recovery or reachable inventory-access rules.
   - Target: One authoritative elapsed-time/safety/access contract governs this phase and is proven with living/destroyed-wall, refuge, enemy-distance and blocked-access fixtures before dependent features are integrated.
   - Acceptance: AC-01, AC-02, AC-19, AC-39.

2. **R02 — One survivor population and starting kit:** A fresh run creates eight ordinary survivors with basic outfits and exactly one configured starting stash for one through four participants.
   - Source: `CHAR-01`
   - Current: FoundationSession creates one temporary actor per participant and removes a departing participant's actor; it has no persistent eight-survivor roster, outfits or shared kit.
   - Target: Normal solo and LAN starts share exactly eight ordinary survivors and the single configured starting kit in game-spec §2.1, with no extra kit on admission or succession.
   - Acceptance: AC-03, AC-04, AC-19, AC-29.

3. **R03 — Movement and stamina:** Survivors walk, sprint, jump and steer in the air using configured speeds, stamina costs, recovery delays and action eligibility.
   - Source: `CHAR-02`, `CHAR-03`
   - Current: Foundation movement walks horizontally at 3 u/s; actor state has no health/stamina, vertical motion, sprint or combat action eligibility.
   - Target: Survivors walk, sprint, jump and steer in air using configured baseline costs/delays, with host-authoritative eligibility and no queued failed attack presses.
   - Acceptance: AC-02, AC-05, AC-06, AC-39.

4. **R04 — Recovery and physical food:** Survivors recover health only during configured safe rest and consume actual carried food for capped immediate recovery under a personal cooldown, including at full health and stamina.
   - Source: `CHAR-04`, `CHAR-05`
   - Current: There are no health, safe-rest, carried food or eating-cooldown systems.
   - Target: Safe health recovery and personal-cooldown food use work during live play; food is an actual carried item and is consumed even when both recovery attributes are full.
   - Acceptance: AC-02, AC-07, AC-08, AC-19.

5. **R05 — Nested carried load:** Every equipped and nested item contributes its actual quantity and weight exactly once to configured load penalties while walking remains possible at severe load.
   - Source: `CHAR-06`
   - Current: The movement foundation has no item graph or carried-weight computation.
   - Target: Actual equipped/nested weight is counted once and drives the configurable load curve; severe load prevents sprint/jump while preserving walking.
   - Acceptance: AC-09, AC-38, AC-39.

6. **R06 — Item identities and equipment:** All 24 configured item definitions use persistent instance identities, one current location and the eleven compatible equipment slots with item-provided storage.
   - Source: `INV-01`, `INV-02`
   - Current: No gameplay item instances, human equipment slots or item-provided storage exist in Core; the partial asset pack is not inventory implementation.
   - Target: All 24 definitions and eleven equipment slots use persistent item identities and a single valid owner/location; capacity comes from the actual worn/stored items.
   - Acceptance: AC-04, AC-12, AC-38, AC-39.

7. **R07 — Placement, rotation and manual replacement:** Grid placement and 90-degree rotation accept only legal fits; occupied equipment slots require manual unequipping before replacement.
   - Source: `INV-03`
   - Current: No grid placement, rotation or equipment-change transaction exists.
   - Target: Legal fits/90-degree rotations commit atomically; occupied equipment slots reject until manually unequipped, with no automatic swap.
   - Acceptance: AC-10, AC-16, AC-19, AC-39.

8. **R08 — Exact stacks:** Compatible stack splits and merges conserve exact quantities and state; a merge fills only available capacity and leaves the remainder in its source.
   - Source: `INV-04`
   - Current: No stack identity, split, merge or quantity-preservation behavior exists.
   - Target: Splits preserve the original remainder identity and create an extracted identity; compatible merges preserve destination identity, fill available capacity and retain the remainder at source.
   - Acceptance: AC-11, AC-14, AC-19, AC-39.

9. **R09 — Nested and filled storage:** Nested containers obey configured depth and acyclic containment, and moving, unequipping or dropping a filled storage item preserves all its descendants.
   - Source: `INV-05`, `INV-06`
   - Current: No containment graph, nesting-depth validation or movement of item-provided pockets exists.
   - Target: All descendants move with a filled storage item; final containment is acyclic, singly owned and within configured depth, including the deepest descendant after a move.
   - Acceptance: AC-12, AC-13, AC-19, AC-38.

10. **R10 — Quick food references:** Four quick slots reference actual carried food stacks and become invalid when the referenced stack leaves the survivor or is exhausted without substituting another stack.
   - Source: `INV-07`
   - Current: The foundation has no food quick slots or item-reference lifetime rules.
   - Target: Four references follow actual carried food identities, can share a stack without duplicating it, and invalidate when that exact stack leaves or disappears.
   - Acceptance: AC-08, AC-13, AC-14, AC-19.

11. **R11 — Permitted access and safe dragging:** Only permitted nearby NPC, corpse, loot and local-stash interactions commit; drag cancellation, panel closure and lost access preserve source ownership and controlled-player inventory privacy.
   - Source: `INV-08`, `INV-09`
   - Current: The foundation has no personal/NPC/corpse/stash panels or inventory permissions.
   - Target: Immediate nearby access observes current reachability/range/ownership and controlled-player privacy; drag previews commit only at valid destinations and survive cancellation/access loss without loss or duplication.
   - Acceptance: AC-01, AC-15, AC-16, AC-34, AC-41.

12. **R12 — Unlimited paged shared stash:** The shared stash provides unlimited aggregate capacity through configured pages, name/type filtering, deliberate placement and stack-first automatic placement with legal rotation.
   - Source: `INV-10`
   - Current: No stash pages, filters, placement or physical deposits/withdrawals exist.
   - Target: The local shared stash has unlimited aggregate page capacity, deterministic stack-first auto-placement, deliberate placement, quick transfer and normalized case-insensitive name/type filtering.
   - Acceptance: AC-17, AC-18, AC-19, AC-39, AC-41.

13. **R13 — One authoritative transaction outcome:** Solo, host-local and guest commands resolve once against current authoritative ownership and eligibility; competing or stale actions receive corrected state and failure feedback.
   - Source: `INV-11`
   - Current: Phase 3 has shared movement authority, readiness, sequence validation and coherent roster publication, but no item/combat/control transactions.
   - Target: Solo, host-local and guests validate commands against current gameplay state and commit each successful effect once; stale/conflicting actors receive corrected state and failure feedback.
   - Acceptance: AC-19, AC-32, AC-34.

14. **R14 — Facing-directed melee:** Fresh attack inputs perform facing-directed fists, single-weapon or simultaneous dual melee attacks with legal stamina, reach, timing and one target per weapon swing; one broken weapon does not disable a usable held melee weapon.
   - Source: `COMBAT-01`, `COMBAT-02`
   - Current: No unarmed/melee targeting, action interval, stamina payment or strike wear exists.
   - Target: Fists, one usable one-handed weapon and two usable one-handed weapons obey configured hits, combined cost/timing and one-target-per-weapon limits. A single usable weapon still works alongside a broken held weapon.
   - Acceptance: AC-06, AC-20, AC-24, AC-39, AC-40.

15. **R15 — Mouse-aimed bow and projectile rules:** Mouse-aimed bows consume real opposite-hand arrows and bow durability per shot, require manual player refilling and obey faction, defense and solid-terrain projectile rules.
   - Source: `COMBAT-03`, `COMBAT-06`
   - Current: The foundation has no bow/arrow-hand equipment, projectiles, factions or defense collision behavior.
   - Target: Accepted bow shots spend real opposite-hand arrows and bow condition, require manual player refilling and obey allied passthrough/hostile-defense/terrain rules through representative fixtures.
   - Acceptance: AC-21, AC-22, AC-39, AC-40.

16. **R16 — Armor and retained broken equipment:** Armor reduces each hit multiplicatively under the configured cap and wears from raw damage; a hit that breaks usable armor receives full protection, and broken equipment remains present without attack or protection benefits.
   - Source: `COMBAT-05`
   - Current: No armor computation, fractional wear, condition or broken-equipment rule exists.
   - Target: Configured multiplicative armor and raw-hit wear apply per authoritative hit. Armor protects fully on the hit that breaks it, while later hits get no protection. Broken items remain belongings and cannot be repaired.
   - Acceptance: AC-20, AC-23, AC-24, AC-39.

17. **R17 — Equipment in world appearances:** Equipped wearables and held items update living and corpse appearances while cosmetic crowns coexist with headwear and stored containers do not appear worn.
   - Source: `COMBAT-08`
   - Current: The foundation displays a simple direction-facing actor; Phase 4 is producing assets/inspection fixtures, not complete gameplay equipment changes.
   - Target: Applicable worn/held items follow current living/corpse equipment and layering, with visible crowns alongside headwear and no extra worn layer for nested bags.
   - Acceptance: AC-25, AC-38, AC-40.

18. **R18 — Corpses, ground loot and cleanup:** Death transfers exactly the remaining belongings to the corpse; nonempty corpses and loot piles persist indefinitely, and only continuous complete emptiness permits timed cleanup.
   - Source: `INV-12`, `INV-13`
   - Current: No character death inventory, corpse/pile ownership or empty cleanup timer exists.
   - Target: Death leaves only remaining actual possessions; nonempty corpses/piles persist indefinitely and only continuous complete emptiness permits delayed removal.
   - Acceptance: AC-02, AC-26, AC-27, AC-38, AC-39.

19. **R19 — Unique control, succession and spectating:** Each participant controls one unique crowned ordinary survivor; death triggers delayed random eligible succession preserving survivor state, and unavailable players spectate in waiting order until an eligible survivor becomes available.
   - Source: `RUN-01`, `RUN-02`, `RUN-03`, `RUN-04`
   - Current: Players control temporary foundation actors; no crowned survivor pool, death transfer, waiting queue or spectator control exists.
   - Target: Living ordinary recruits receive unique control/crowns; delayed random succession preserves their state, concurrent assignments stay exclusive and waiting spectators regain available survivors in the specified order.
   - Acceptance: AC-02, AC-28, AC-29, AC-38, AC-40.

20. **R20 — Coherent snapshots and saved layout representation:** Coherent snapshot round trips preserve survivor, item, ownership, nested-layout, condition, cleanup, control and random state including saved dimensions, footprints, orientations and legacy-layout metadata.
   - Source: Roadmap Phase 6 delivery boundary; prerequisite/integration obligation, without transferring later-phase requirement ownership.
   - Current: Example snapshot infrastructure and the Phase 3 movement roster exist; no full survivor/item graph, condition/timer/control snapshot or legacy-layout data exists.
   - Target: Round trips retain coherent gameplay state and old layout dimensions/placements/footprints without mutation, healing or event replay. Representation begins here; changed-configuration legacy viewing/extraction is delivered in Phase 9.
   - Acceptance: AC-30, AC-31, AC-32, AC-38, AC-39.

21. **R21 — Usable phase interfaces:** Production HUD, inventory, stash, loot and spectator interfaces expose the phase actions and outcomes while panel input and live-world feedback remain usable in solo and four-player LAN play.
   - Source: Roadmap Phase 6 delivery boundary; prerequisite/integration obligation, without transferring later-phase requirement ownership.
   - Current: The foundation offers start/join, movement HUD and session feedback; it has no health/stamina, item grids, stash, loot or spectator surfaces.
   - Target: The phase's actual HUD/inventory/stash/loot/equipment/quick-slot/succession interfaces expose their production actions at 1280×720, preserve panel priority and keep the world running.
   - Acceptance: AC-15, AC-16, AC-33, AC-34, AC-40, AC-41.

22. **R22 — Stationary combat slice and repeatable proof:** A repeatable playable fixture uses stationary hostile targets with triggered attacks and representative camp/defense cases, and stored scenarios verify the phase through production actions and declared evidence.
   - Source: Roadmap Phase 6 delivery boundary; prerequisite/integration obligation, without transferring later-phase requirement ownership.
   - Current: Phase 2/3 have stored scenarios and real input/LAN evidence. Phase 4 remains in progress and Phase 5's unified framework remains a prerequisite; no Phase 6 scenario is implemented or accepted.
   - Target: A stationary-target fixture and normal solo/four-player flows prove the phase through production behavior, saved scenarios and evidence under the current local testing policy, without requiring enemy pursuit or the campaign.
   - Acceptance: AC-35, AC-36, AC-37, AC-38.

## Source Requirement Coverage

| Source IDs | Locked statements |
| --- | --- |
| CHAR-01 | R02 |
| CHAR-02, CHAR-03 | R03 |
| CHAR-04, CHAR-05 | R04 |
| CHAR-06 | R05 |
| INV-01, INV-02 | R06 |
| INV-03 | R07 |
| INV-04 | R08 |
| INV-05, INV-06 | R09 |
| INV-07 | R10 |
| INV-08, INV-09 | R11 |
| INV-10 | R12 |
| INV-11 | R13 |
| INV-12, INV-13 | R18 |
| COMBAT-01, COMBAT-02 | R14 |
| COMBAT-03, COMBAT-06 | R15 |
| COMBAT-05 | R16 |
| COMBAT-08 | R17 |
| RUN-01, RUN-02, RUN-03, RUN-04 | R19 |

R01 and R20–R22 make the roadmap delivery boundary and usable proof explicit. They do not claim complete WORLD, NPC, SESSION, SAVE, DATA, UX or release acceptance assigned to later phases.

## Boundaries

**In scope:**

- The complete CHAR/INV/COMBAT/RUN source set above, in normal solo and four-player LAN gameplay.
- Real elapsed-time, safety/danger and reachable local-access rules, proven through representative camp/defense/enemy fixtures before the rest of the phase is integrated.
- Exactly eight persistent ordinary survivors and one configured fresh-run kit; existing survivors are used for joining/control, and guest departure does not delete a survivor or possessions.
- All 24 item definitions, eleven slots, actual pockets, filled/nested containers, stacks, rotation, load, condition and ground loot.
- Immediate permitted inventory interactions, private controlled inventories, local unlimited paged stash, filtering, quick transfer, four food references and corrected conflict feedback.
- Fists, usable single/dual melee, manually refilled mouse-aimed bows, faction/obstruction rules, armor/wear and retained broken equipment.
- Living/corpse equipped appearances and crowns using the required production content.
- The actual health/stamina/equipment/arrow/load/money HUD, inventory/stash/loot panels, quick-slot interaction and spectator/succession surfaces, including empty/error/access-loss states.
- Coherent snapshot round trips of the new state and complete saved-layout dimensions/placements/footprints/metadata.
- Stationary hostile targets with triggered real attacks, representative defenses/terrain and repeatable death/succession/loot scenarios; fixture-only supplies remain separate from normal startup.
- Stored scenarios, phase-owned acceptance activation, pure/build checks, review and grouped post-review solo/four-process evidence under the current testing policy.

**Out of scope:**

- Autonomous recruitment, job/equipment/restocking AI, food sale, hunting/gathering, hauling, paid construction/repair/upgrades and the full safe-work economy — Phase 7 owns those player flows. Phase 6 supplies their shared item/safety/combat foundations.
- Seven-archetype zombie behavior, enemy pursuit, waves, portals, progression, retaliation, army orders and complete victory/defeat — Phase 8. Stationary targets suffice here; zero successor candidates can remain a fixture spectator/waiting state without claiming campaign defeat acceptance.
- Complete cross-archetype automatic arrow refill/fallback, natural spit and cosmetic bloater acid acceptance — Phase 8 owns COMBAT-04/07. Shared equipment/ammo rules are established here without requiring those autonomous consumers.
- Full handcrafted-map/day-night/minimap integration — later world/campaign phases. Representative camp borders, danger, local reachability and elapsed time are required now rather than constant test substitutes.
- Manual/dawn/exit saves, multi-run selection, atomic file recovery, complete configuration-change application and interactive legacy-layout viewing/extraction — Phase 9. Saved-layout preservation is required here; legacy extraction is not claimed.
- Complete session recovery/reconnection/host-loss saves and the remaining session metadata/permissions experience — Phase 9. Basic joins/leaves must already conserve the Phase 6 survivor/item state.
- Complete preferences/remapping/UI-scale/translation/audio polish across the full game — Phase 9. This phase's input, feedback and equipment presentation must already work; later UX ownership does not permit missing phase interfaces.
- Physical mixed-OS LAN certification, other supported display configurations, reference-PC performance and real-duration endurance/release validation — their declared release procedures/Phase 10; routine local checks use development Windows at 1280×720 without a VM.
- Scheduled nightly full regression execution — Phase 11. Critical/new/changed/affected local selection and saved scenario metadata apply immediately.
- Additional combat/progression systems, friendly damage, equipment repair and other v0.1 exclusions — the canonical game scope remains unchanged.

## Constraints

- Godot/C# and the established engine-independent rules, stable identity, authoritative host, explicit time/randomness and service boundaries remain required. This specification locks behavior, not new interfaces, file formats, transport libraries or UI layouts.
- Implement against the current source's configured working baseline in §§2, 4, 5 and relevant food/access/UI sections. Survivor defaults include 100 health/stamina; 3/5 u/s walk/sprint; 1.5 u jump and 0.8 s level-ground airborne interval; 75% air control; 10/s sprint and 12 jump stamina; 14/s stamina recovery after 0.5 s; 2/s safe health recovery after 5 s; 2 s player food cooldown; 20/35 kg load thresholds; 3/4 u inventory/stash access; three container levels; four quick references; 10×10 stash pages; 180 s empty cleanup; and 1 s succession.
- All roster footprints, stack caps, weights, pocket/container grids, compatibility, weapon/armor values and starting-kit quantities follow the canonical tables. Relevant tunables and valid ranges/relationships are documented configurable data, not hidden gameplay constants. Complete cross-domain configuration acceptance retains Phase 9 ownership.
- Health/stamina/time/weight/condition may be fractional; counts, quantities and grid coordinates are integral. Document numeric storage/measurement tolerances without rounding away possessions, thresholds or armor wear. Reject invalid or overflowing values before mutation.
- Balance defaults remain proposals to implement and evaluate. The linear default load interpolation, full-stat food consumption, manual occupied-slot replacement, partial merges, broken-held-weapon fallback, armor-breaking-hit protection and no queued failed attacks are locked interview decisions for this phase.
- One authoritative committed order resolves simultaneous actions. Stable identities survive moves, death, snapshots and control changes; client/display ordering cannot establish ownership or replay an effect.
- The asset pack is a prerequisite, not evidence of gameplay integration. Production content must be packaged with stable references and no runtime dependency on the recovered-material directory.
- Follow the [phase branch and PR rules](../../../docs/workflow/ai-collaboration.md#phase-branches-and-pull-requests) when implementation starts: its feature branch originates from current `origin/dev`, draft PR targets `dev` at the first implementation push, and the same PR becomes ready only after required verification. This preparatory specification does not start implementation, switch the active branch or authorize a merge.
- Finish whole-phase implementation and code review/fixes before real game windows. Ordinary hosted Windows/Linux PR checks remain build/repository-only; fresh local rendered acceptance, hosted compatibility and release environments retain distinct evidence.

## Acceptance Criteria

- [ ] **AC-01:** The production elapsed-time, safety and access rules are exercised before dependent features: completed living outer walls define each camp side, destroying one falls back to the next wall or the main-building refuge of 12 u, and a tower alone does not expand camp. Camp-border points count as inside; an enemy at exactly 12 u prevents safe rest. A nearby but unreachable interaction fails, and moving to a reachable position enables it.

- [ ] **AC-02:** Timers use authoritative elapsed gameplay time rather than window, frame or client clocks. Just before, exactly at and just after 0.5 s stamina delay, 2 s food cooldown, 5 s safe-rest qualification, 1 s succession and 180 s empty cleanup produce the specified eligibility transitions. Duplicate commands do not advance time; equivalent elapsed intervals preserve recovery and timer results within a declared numeric tolerance.

- [ ] **AC-03:** Fresh starts with one, two, three and four participants each create exactly eight distinct living recruited survivors. Every survivor wears a basic shirt, trousers and boots with empty pockets and no personal money, food, weapon, arrows, armor or backpack. The one shared stash contains 200 money, four full-durability clubs, four full-durability bows, 240 arrows in eight stacks of 30, four empty small backpacks, four empty belts, 20 berries, ten fruit and ten meat in legal stacks. Identical outfits have distinct instance identities. Repeated start/admission messages and participant ordering cannot multiply the roster or supplies.

- [ ] **AC-04:** Exactly the 24 definitions in game-spec §5.3 are usable with their configured footprint, rotation, quantity, unit weight, storage, compatibility, durability and appearance. The eleven slots are headwear, face, torso clothing, legs, body armor, gloves, boots, backpack, belt and left/right hands. Equipped/stored items have one location each; empty slots are valid, identical definitions retain distinct instance identities, pair wearables count as one item, and no invisible personal grid adds capacity.

- [ ] **AC-05:** At the unloaded default baseline, level-ground walking/sprinting measure 3/5 u per second, a jump reaches 1.5 u over a nominal 0.8 s airborne interval, and airborne steering is 75% of ground walk control, within documented measurement tolerance. Walking costs no stamina. Ground/air transitions, opposing holds and focus loss do not create extra jumps, stuck movement or duplicated movement steps.

- [ ] **AC-06:** Sprint costs 10 stamina/s, jumping costs 12, and each attack pays its configured cost only when accepted. Exactly sufficient stamina permits the action; insufficient stamina blocks it. Natural stamina regeneration is 14/s after 0.5 s without a spending action, including outside camp and in combat. Attack presses during cooldown or without sufficient stamina are discarded and do not queue a later attack. Fractional spending/recovery is retained and clamped to legal bounds.

- [ ] **AC-07:** Natural health recovery is 2/s only after five continuous seconds inside camp, with no enemy at or within 12 u, and without sprinting, jumping or attacking. Ordinary walking is allowed. Losing a safe-rest condition resets qualification, and returning requires the full interval again. Recovery never exceeds maximum health and an unsafe location cannot recover health merely because its UI panel is open.

- [ ] **AC-08:** Using carried berries, fruit or meat consumes one actual unit and immediately restores respectively 8/15, 15/25 or 30/40 health/stamina, capped at the configured maxima. Food is consumed and starts the two-second personal cooldown even when both attributes are already full. Before cooldown expiry, with no valid carried food, or after another command consumed/transferred the stack, use fails without further consumption. Simultaneous quick-slot inputs share the same survivor cooldown.

- [ ] **AC-09:** Load equals the sum of unit weight times quantity for every equipped and nested descendant exactly once, independent of traversal order. Empty storage adds only its own weight. At 20 kg there is no penalty; between 20 and 35 kg the default curve is linear toward 55% movement speed and 150% action stamina cost; at or above 35 kg sprint/jump are blocked while walking remains possible at the minimum speed. Fractional weights are retained; invalid cyclic graphs are rejected rather than double-counted.

- [ ] **AC-10:** Integer-cell placements fit when their complete footprint is inside the grid, including an exact edge fit. Two items may touch edges without sharing any cell. Overlap, a one-cell overflow, invalid rotation/location and an occupied equipment slot reject without changing either item. Rotation is 90 degrees and is committed only with a valid placement; returning through a complete rotation cycle does not create a new item. Replacement requires manual unequipping; no automatic equipment swap is performed.

- [ ] **AC-11:** A split accepts an integer quantity from one through source quantity minus one, leaves the original identity on the remainder and assigns a fresh identity to the extracted stack. Zero, negative, fractional, excessive or overflowing quantities reject without mutation. Only matching definition/state stacks combine. Merging preserves destination identity, transfers min(source quantity, remaining destination capacity), leaves any remainder at source and retires the source only when exhausted. A full destination rejects unchanged; equipment and containers do not stack.

- [ ] **AC-12:** A worn storage container or container directly on a stash page is nesting level 1; all descendants must remain within the default three container levels. Empty/single containers are valid. Self-containment, indirect cycles, duplicate ownership and a filled-subtree move that would push any descendant past the limit reject atomically. Containment validation and resulting ownership do not depend on child enumeration order.

- [ ] **AC-13:** Moving, unequipping or explicitly dropping filled clothing, belts, pouches or backpacks moves the same outer item and every descendant with exact identities, placements, quantities and condition. Contents never remain as phantom pockets on the previous owner. Moving within the same survivor preserves carried references; moving the subtree out invalidates that survivor's references to its food. Invalid destination capacity/access leaves the entire subtree at source.

- [ ] **AC-14:** Quick slots 1–4 reference carried food identities without storing extra units. Moving a stack between carried pockets or partially splitting it preserves references to the original stack; a new split stack is not substituted. Exhausting, fully merging away, dropping, transferring away or losing the referenced stack on death invalidates its references. Several slots may reference the same stack but still consume the one real supply under the one personal cooldown. Empty slots and invalid indexes produce no use.

- [ ] **AC-15:** An ordinary nearby NPC, corpse or loot pile is accessible at an inclusive 3 u, and the shared stash at an inclusive 4 u from the main building, subject to current reachability and permission. Just-outside distances reject. Another controlled survivor's inventory is private. If an NPC becomes controlled, dies, moves away or otherwise loses access, stale actions are revalidated and rejected or resolved against the still-valid target without transferring from the wrong owner. Zero targets has explicit feedback; overlapping targets use a visible selector.

- [ ] **AC-16:** A drag is a preview with source ownership unchanged until a valid destination commits. Cancel, Escape/panel closure, release outside a grid, death, lost range, invalidated targets or a competing transfer do not implicitly drop, delete or duplicate the item. World dropping requires an explicit drop action/target. Repeated cancellation is harmless; failed/stale transfers show an explanation and corrected contents.

- [ ] **AC-17:** The default shared stash uses 10×10 pages and has no aggregate gameplay capacity cap. Filling existing pages and depositing additional valid items creates enough pages to accept the cargo. Empty stash/pages remain usable. Definitions that cannot fit any supported page orientation fail configuration validation. Page creation and a competing deposit cannot copy an item or publish an invalid partial transfer.

- [ ] **AC-18:** Stash auto-placement first merges matching stacks across existing pages, then uses the earliest fitting page/cells with legal rotation, then creates a page. Equal choices resolve consistently for the same authoritative state. Empty name/type filters show all matching contents; name matching uses Unicode-normalized, case-insensitive substring matching with consistent Windows/Linux results. Equivalent normalized strings match; an unmatched filter shows an explicit empty result. Filtering/navigation do not move items or change page ownership.

- [ ] **AC-19:** Solo, host-local and guests use the same authoritative action rules. Replaying a command commits no second effect, wear, stamina cost, consumption, timer reset or state creation. Concurrent actors taking one item or using the same food stack produce only valid successful effects in the host's committed order. Losers receive corrected state and clear feedback. Death, control changes and access loss are checked at commit; stale messages cannot regain control, restore spent items or overwrite newer state.

- [ ] **AC-20:** Fists use 8 damage, 0.7 s interval, 1.0 u reach and 3 stamina. Each usable melee weapon follows game-spec §4.3. One attack input with two usable one-handed weapons swings both, costs the sum and uses the slower interval; each weapon hits at most one eligible facing target and both may hit the same target. Reach boundary cases and zero/one/multiple targets are exercised. Target ties resolve consistently. A miss cannot wear a weapon, and each successful weapon strike wears only that weapon by one.

- [ ] **AC-21:** A bow follows mouse aim, occupies one hand and accepts only an arrow stack opposite it. An accepted shot spends one real arrow, 6 stamina and one bow durability, with default 22 raw damage, 1.2 s interval, 25 u/s projectile speed and 2 s lifetime. Empty arrow hand, incompatible hands, zero durability, insufficient stamina and cooldown reject a shot. Player reserves do not automatically refill the hand. Missed, expired, blocked and fired-before-death arrows remain spent and never become recoverable loot.

- [ ] **AC-22:** Friendly melee/projectiles cause no allied damage; friendly arrows pass allies and camp defenses. Hostile arrows damage the defense blocking their path. Solid terrain stops arrows. A projectile strikes only its first eligible collision; equal-contact cases resolve consistently, without piercing an extra target or applying damage twice. The representative fixture exercises ignored allies, adjacent/overlapping colliders, zero eligible hits, hostile survivors and blocking defenses through the real combat path.

- [ ] **AC-23:** Usable armor reductions combine multiplicatively, capped at 60%; body armor wears by 20% and the helmet by 10% of the same incoming raw hit. With plated armor plus helmet, raw damage 32 produces 17.28 health damage, 6.4 body wear and 3.2 helmet wear, subject only to declared storage tolerance. An item above zero durability at the start of a hit gives full configured protection for that hit even if it breaks. Each later hit uses the updated condition in authoritative order. Fractional condition is preserved, wear cannot increase raw damage, and zero damage cannot cause positive wear.

- [ ] **AC-24:** Zero-durability weapons cannot produce weapon attacks and zero-durability armor supplies no reduction. Broken items remain visible, transferable belongings and cannot be repaired. With one usable and one broken held melee weapon, an attack uses only the usable weapon's cost/interval; the broken item remains equipped. With only broken held weapons, the player must free a hand before punching. Ordinary clothes, bags and belts do not wear out or lose storage through durability.

- [ ] **AC-25:** All applicable equipped wearables and held items update on living survivors, representative hostile characters and lootable corpses after equip, unequip, transfer, wear/break and death. Headwear coexists with the cosmetic crown. Normal layering may cover underclothes; a bag nested in storage is not displayed as worn. Empty slots remove their equipment layer, and repeated/out-of-order updates cannot duplicate layers or restore older gear over a newer committed state.

- [ ] **AC-26:** Killing a loaded ordinary survivor or representative zombie leaves exactly its actual remaining equipped/stored possessions, nested contents, money, food, partially spent arrows and broken equipment with their identities and condition. Death creates no fresh loot, replacement ammo or copied successor inventory. Repeated death handling creates one corpse. Items removed earlier stay removed; empty and single-item corpses remain valid loot containers.

- [ ] **AC-27:** Any remaining item, including a broken item or empty bag, prevents age-based corpse/pile cleanup indefinitely. Completely empty containers expire only after 180 continuous seconds of authoritative emptiness. Adding an item cancels the countdown; becoming empty again starts a fresh full interval. At the expiry boundary, host action order determines whether a transfer still has a live valid target, and no committed nonempty container is removed. Cosmetic fading never deletes belongings.

- [ ] **AC-28:** Every connected controller owns at most one distinct living ordinary recruited survivor, identified by one cosmetic crown. After a controlled death, transfer occurs at the one-second baseline to a random living recruited survivor not controlled by another player. Candidate health, stamina, position, role memory and all belongings are unchanged. Dead characters, wildlife, corpses and structural guards are ineligible. Two simultaneous deaths with one candidate assign it once; the other player waits.

- [ ] **AC-29:** Waiting spectators receive newly eligible survivors in waiting order; equal waiting moments use player-slot order, with random eligible selection for each assignment. Zero candidates yields an explicit waiting state, permits following/cycling living teammates and does not create a recruit. Spectators cannot mutate inventories or attack. A guest departure releases its survivor to ordinary NPC ownership without deleting it, its gear or vulnerability; joins use the existing eight rather than creating a new kit. Full recruitment/reconnection/outcome flows retain later owners.

- [ ] **AC-30:** Snapshot encode/decode round trips preserve exact identity/location graphs, all quantities and fractional condition, living/dead state, position, motion/action/recovery state, projectiles as applicable, elapsed/cooldown/cleanup time, participant eligibility, role memory, waiting order and random state needed for future outcomes. Empty collections and a single descendant round-trip. Repeated restore does not replay an attack, spawn, food use, death, succession or cleanup event; reordered unordered records do not change the represented state.

- [ ] **AC-31:** Round trips retain saved grid dimensions, display footprints, placements, orientations, saved stack/layout information and legacy markers even when current definitions would describe a different layout. No automatic overflow transfer, truncation or inventory deletion is introduced. This phase proves representation/preservation; interactive changed-configuration legacy viewing/extraction and the complete save/load UI remain Phase 9 acceptance.

- [ ] **AC-32:** Malformed snapshots, missing definitions, duplicate identities/locations, cycles and invalid scalar data report a clear failure without replacing the prior valid state or modifying the source snapshot. Equivalent concurrent capture yields one coherent committed state rather than a mixture of pre/post-transfer ownership. Restoring cannot heal, repair, refill, change quantities or reroll succession randomness.

- [ ] **AC-33:** At 1280×720 the production HUD shows health, stamina, carried money, weight state, hands/equipment, arrows and four quick slots. Inventory exposes eleven slots, actual pockets/nested grids, rotation, splitting, condition and weight; stash exposes pages/filter/quick transfer; NPC/corpse/pile panels show the target and current permitted contents; spectator UI shows follow/cycle and waiting/control handover. Empty, invalid and blocked states are distinguishable and actions are reachable through normal keyboard/mouse input.

- [ ] **AC-34:** Inventory/panel clicks are captured before world attack or interaction. Escape closes the active panel before the enclosing menu action. Multiple/overlapping panels have consistent input priority and cannot trigger a second world action. Opening/closing panels does not pause combat, recovery, movement of other actors or cleanup. Damage, blocked actions, lost access and disconnection retain visible feedback, and repeated UI actions preserve the current authoritative state.

- [ ] **AC-35:** The playable combat fixture uses stationary hostile targets with explicitly triggered real melee/bow attacks, real inventory/ammo/durability and representative camp/defense/terrain cases. It supports damage, food, death, looting and succession through the same production rules used by normal play. Fixture-only supplies/setup are identified separately and do not alter the exact normal starting kit. Fresh resets are independent and cannot replay earlier fixture state. Enemy pursuit, waves, portals and the complete roster are not required here.

- [ ] **AC-36:** Stored AI-readable scenarios map all 29 source requirement IDs, this specification's acceptance criteria and relevant A-01, A-04, A-05, A-06, A-22–A-28 and A-31 coverage to executable checks and evidence, retaining explicit later-phase portions. Include importance/rationale, setup/seed, production actions, expected results, process roles, evidence, run-group/order, resets and owned cleanup. All active Critical, new/changed and affected regression cases are selected regardless of importance; omissions and uncertain impact are explicit.

- [ ] **AC-37:** Complete implementation and code review/fixes precede rendered game tests. Pure/build/config/snapshot/failure checks run without a real game window. Post-review tests include solo and an actual host-plus-three-guests process set on development Windows at 1280×720, grouped where compatible. Failures are retained, repairs are batched, failed/blocked/newly affected cases rerun, then the entire selected set passes on the repaired build. Zero selected required cases, missing peers/results/images, crashes and timeouts cannot pass. Hosted Windows/Linux build checks and distinct physical LAN/display/hardware/endurance release evidence remain separately identified.

- [ ] **AC-38:** Across item, participant, appearance, snapshot and fixture collections, empty and single-element cases remain usable; equal-looking values never collapse distinct stable identities, and repeated enumeration or presentation ordering cannot change ownership, total quantities, total weight or the current committed result. Stable tie decisions use authoritative state rather than arrival of stale presentation messages.

- [ ] **AC-39:** Counts, grid coordinates and stack quantities remain exact integers; health, stamina, weight, time, damage and durability retain necessary fractional precision. Minimum, maximum, exact threshold and nearest representable values on either side are exercised for applicable rules. Negative, non-finite, unsupported or overflowing inputs reject with feedback and no partial mutation; none wrap into a valid action or silently truncate belongings.

- [ ] **AC-40:** MUST NOT damage allies or grant crowns combat bonuses, an equipment slot or loot value; controlling an ordinary survivor does not create a stronger character class.

- [ ] **AC-41:** MUST NOT expose another controlled survivor's private inventory for inspection or manipulation by a different participant, or grant remote access to the shared stash.

## Source Acceptance Boundaries

| Source scenario | Phase 6 proof / retained later portion |
| --- | --- |
| A-01 | Eight survivors/outfits and one kit now; full day-1 world/main-building/portal setup keeps its later campaign owner. |
| A-04, A-05 | Actual death possessions, state-preserving random succession and exclusive simultaneous assignment now. |
| A-06 | Waiting order/new eligibility now; the actual recruit purchase/population-cap flow remains Phase 7. |
| A-14 | Shared real ammo/condition contract begins here; complete autonomous NPC/zombie refill/fallback remains Phase 8. |
| A-22, A-23 | Inventory manipulation and load/broken-equipment rules now through the production phase interfaces. |
| A-24 | Immediate NPC/corpse/stash access while live attacks continue, with panel input capture, now. |
| A-25 | Player/fixture deposits prove unlimited pages now; autonomous delivery and sale-proceeds integrations remain Phase 7. |
| A-26 | Loaded ordinary survivor/representative enemy death now; real mid-delivery carrier cargo integrates in Phase 7 and complete enemy population in Phase 8. |
| A-27 | Nonempty persistence and resettable empty cleanup now. |
| A-28 | Item/food/control conflicts now; purchases, population limits, upgrades and selling integrate with their owning phases. |
| A-31 | Character arrow spending, allied passthrough and blocking-defense behavior now; built-in tower integration remains with the structure/campaign phases. |
| A-43 | Saved layout representation/round trips now; changed-configuration interactive extraction and complete save compatibility remain Phase 9. |

These rows are evidence boundaries, not execution results or complete credit for later-phase scenario portions.

## Edge Coverage

**Coverage:** 133/133 applicable edges resolved · 133 explicit · 0 backstop · 0 dismissed · 0 unresolved.

The compiled canonical `edge-probe.cjs` was invoked over the exact 22 requirement statements with authored numeric/collection/text/stateful/I/O shapes. It proposed 118 rows and zero unclassified requirements; all 118 were resolved through the accepted timing/combat, inventory and shared-state/lifecycle sets. A second canonical invocation accepted the resolutions with 118 resolved, zero unresolved. Manual review added the 15 marked rows below to cover requirement interactions the shape pass did not propose. The engine's floor was preserved in full. Every row points to concrete pass/fail criteria above; no held-out-test substitution or silent dismissal is used.

| Edge | Category | Requirement | Status | Verification | Resolution / reason |
| --- | --- | --- | --- | --- | --- |
| E001 | boundary | R01 | resolved | explicit | AC-01 AC-02. |
| E002 | concurrency | R01 | resolved | explicit | AC-01 AC-19. |
| E003 | empty | R01 | resolved | explicit | AC-01 — No completed wall on a side uses the 12 u refuge rather than an undefined border. (manual addition) |
| E004 | idempotency | R01 | resolved | explicit | AC-02 AC-19. |
| E005 | ordering | R01 | resolved | explicit | AC-01 AC-19 — Removing an outer wall selects the next living wall from current committed state. (manual addition) |
| E006 | precision | R01 | resolved | explicit | AC-02 AC-39. |
| E007 | adjacency | R02 | resolved | explicit | AC-03 AC-38. |
| E008 | boundary | R02 | resolved | explicit | AC-03 AC-29. |
| E009 | concurrency | R02 | resolved | explicit | AC-03 AC-29. |
| E010 | empty | R02 | resolved | explicit | AC-03. |
| E011 | idempotency | R02 | resolved | explicit | AC-03 AC-19. |
| E012 | ordering | R02 | resolved | explicit | AC-03 AC-38. |
| E013 | precision | R02 | resolved | explicit | AC-03 AC-39. |
| E014 | boundary | R03 | resolved | explicit | AC-05 AC-06. |
| E015 | concurrency | R03 | resolved | explicit | AC-05 AC-06 AC-19. |
| E016 | idempotency | R03 | resolved | explicit | AC-05 AC-06 AC-19. |
| E017 | precision | R03 | resolved | explicit | AC-05 AC-06 AC-39. |
| E018 | boundary | R04 | resolved | explicit | AC-02 AC-07 AC-08. |
| E019 | concurrency | R04 | resolved | explicit | AC-07 AC-08 AC-19. |
| E020 | idempotency | R04 | resolved | explicit | AC-08 AC-19. |
| E021 | precision | R04 | resolved | explicit | AC-07 AC-08 AC-39. |
| E022 | adjacency | R05 | resolved | explicit | AC-09 AC-38. |
| E023 | boundary | R05 | resolved | explicit | AC-09. |
| E024 | concurrency | R05 | resolved | explicit | AC-09 AC-19. |
| E025 | empty | R05 | resolved | explicit | AC-09. |
| E026 | idempotency | R05 | resolved | explicit | AC-09 AC-19. |
| E027 | ordering | R05 | resolved | explicit | AC-09 AC-38. |
| E028 | precision | R05 | resolved | explicit | AC-09 AC-39. |
| E029 | adjacency | R06 | resolved | explicit | AC-04 AC-38. |
| E030 | boundary | R06 | resolved | explicit | AC-04 AC-39. |
| E031 | concurrency | R06 | resolved | explicit | AC-04 AC-19. |
| E032 | empty | R06 | resolved | explicit | AC-04. |
| E033 | idempotency | R06 | resolved | explicit | AC-04 AC-19. |
| E034 | ordering | R06 | resolved | explicit | AC-04 AC-38. |
| E035 | precision | R06 | resolved | explicit | AC-04 AC-39. |
| E036 | adjacency | R07 | resolved | explicit | AC-10. |
| E037 | boundary | R07 | resolved | explicit | AC-10. |
| E038 | concurrency | R07 | resolved | explicit | AC-10 AC-16 AC-19. |
| E039 | empty | R07 | resolved | explicit | AC-04 AC-10. |
| E040 | idempotency | R07 | resolved | explicit | AC-10 AC-19. |
| E041 | ordering | R07 | resolved | explicit | AC-10 AC-38. |
| E042 | precision | R07 | resolved | explicit | AC-10 AC-39. |
| E043 | adjacency | R08 | resolved | explicit | AC-11 AC-38. |
| E044 | boundary | R08 | resolved | explicit | AC-11. |
| E045 | concurrency | R08 | resolved | explicit | AC-11 AC-19. |
| E046 | empty | R08 | resolved | explicit | AC-11. |
| E047 | idempotency | R08 | resolved | explicit | AC-11 AC-19. |
| E048 | ordering | R08 | resolved | explicit | AC-11 AC-38. |
| E049 | precision | R08 | resolved | explicit | AC-11 AC-39. |
| E050 | adjacency | R09 | resolved | explicit | AC-12 AC-13. |
| E051 | boundary | R09 | resolved | explicit | AC-12. |
| E052 | concurrency | R09 | resolved | explicit | AC-12 AC-13 AC-19. |
| E053 | empty | R09 | resolved | explicit | AC-12 AC-13. |
| E054 | idempotency | R09 | resolved | explicit | AC-12 AC-13 AC-19. |
| E055 | ordering | R09 | resolved | explicit | AC-12 AC-38. |
| E056 | precision | R09 | resolved | explicit | AC-12 AC-39. |
| E057 | adjacency | R10 | resolved | explicit | AC-14. |
| E058 | boundary | R10 | resolved | explicit | AC-14. |
| E059 | concurrency | R10 | resolved | explicit | AC-08 AC-14 AC-19. |
| E060 | empty | R10 | resolved | explicit | AC-14. |
| E061 | idempotency | R10 | resolved | explicit | AC-08 AC-14 AC-19. |
| E062 | ordering | R10 | resolved | explicit | AC-14 AC-38. |
| E063 | precision | R10 | resolved | explicit | AC-14 AC-39. |
| E064 | boundary | R11 | resolved | explicit | AC-15. |
| E065 | concurrency | R11 | resolved | explicit | AC-15 AC-16 AC-19. |
| E066 | empty | R11 | resolved | explicit | AC-15 — Zero permitted targets gives feedback and no transaction. (manual addition) |
| E067 | idempotency | R11 | resolved | explicit | AC-16 AC-19. |
| E068 | ordering | R11 | resolved | explicit | AC-15 AC-34 — Overlapping targets use a selector; panel input wins before world interaction. (manual addition) |
| E069 | precision | R11 | resolved | explicit | AC-15 AC-39. |
| E070 | adjacency | R12 | resolved | explicit | AC-10 AC-18. |
| E071 | boundary | R12 | resolved | explicit | AC-17. |
| E072 | concurrency | R12 | resolved | explicit | AC-17 AC-18 AC-19. |
| E073 | empty | R12 | resolved | explicit | AC-17 AC-18. |
| E074 | encoding | R12 | resolved | explicit | AC-18. |
| E075 | idempotency | R12 | resolved | explicit | AC-18 AC-19. |
| E076 | ordering | R12 | resolved | explicit | AC-18. |
| E077 | precision | R12 | resolved | explicit | AC-17 AC-39. |
| E078 | concurrency | R13 | resolved | explicit | AC-19 AC-32. |
| E079 | idempotency | R13 | resolved | explicit | AC-19. |
| E080 | adjacency | R14 | resolved | explicit | AC-20 — Two swings can touch the same target but each weapon hits at most one target. (manual addition) |
| E081 | boundary | R14 | resolved | explicit | AC-06 AC-20 AC-24. |
| E082 | concurrency | R14 | resolved | explicit | AC-19 AC-20. |
| E083 | empty | R14 | resolved | explicit | AC-20 AC-24 — Zero targets is a miss; empty hands can punch; all-broken held gear requires freeing a hand. (manual addition) |
| E084 | idempotency | R14 | resolved | explicit | AC-06 AC-19 AC-20. |
| E085 | ordering | R14 | resolved | explicit | AC-19 AC-20 — Equal melee target choices resolve consistently and dual swings pay one combined accepted action. (manual addition) |
| E086 | precision | R14 | resolved | explicit | AC-20 AC-39. |
| E087 | adjacency | R15 | resolved | explicit | AC-22 — Adjacent or overlapping projectile colliders still produce only the first eligible hit. (manual addition) |
| E088 | boundary | R15 | resolved | explicit | AC-21. |
| E089 | concurrency | R15 | resolved | explicit | AC-19 AC-21 AC-22. |
| E090 | empty | R15 | resolved | explicit | AC-21 AC-22 — Empty arrow hand cannot fire, and a projectile with no eligible hit expires without loot. (manual addition) |
| E091 | idempotency | R15 | resolved | explicit | AC-19 AC-21. |
| E092 | ordering | R15 | resolved | explicit | AC-22 — Path order selects the first eligible hit; equal contacts resolve consistently. (manual addition) |
| E093 | precision | R15 | resolved | explicit | AC-21 AC-39. |
| E094 | boundary | R16 | resolved | explicit | AC-23 AC-24. |
| E095 | concurrency | R16 | resolved | explicit | AC-19 AC-23. |
| E096 | idempotency | R16 | resolved | explicit | AC-19 AC-23. |
| E097 | ordering | R16 | resolved | explicit | AC-19 AC-23 — Each ordered hit reads current armor condition, so a later hit sees a previous break. (manual addition) |
| E098 | precision | R16 | resolved | explicit | AC-23 AC-39. |
| E099 | adjacency | R17 | resolved | explicit | AC-25 AC-38. |
| E100 | concurrency | R17 | resolved | explicit | AC-25. |
| E101 | empty | R17 | resolved | explicit | AC-25. |
| E102 | idempotency | R17 | resolved | explicit | AC-25. |
| E103 | ordering | R17 | resolved | explicit | AC-25 AC-38. |
| E104 | adjacency | R18 | resolved | explicit | AC-26 AC-38. |
| E105 | boundary | R18 | resolved | explicit | AC-02 AC-27. |
| E106 | concurrency | R18 | resolved | explicit | AC-19 AC-26 AC-27. |
| E107 | empty | R18 | resolved | explicit | AC-26 AC-27. |
| E108 | idempotency | R18 | resolved | explicit | AC-19 AC-26. |
| E109 | ordering | R18 | resolved | explicit | AC-26 AC-27 AC-38. |
| E110 | precision | R18 | resolved | explicit | AC-26 AC-27 AC-39. |
| E111 | adjacency | R19 | resolved | explicit | AC-28 AC-38. |
| E112 | boundary | R19 | resolved | explicit | AC-02 AC-28. |
| E113 | concurrency | R19 | resolved | explicit | AC-28 AC-29. |
| E114 | empty | R19 | resolved | explicit | AC-29. |
| E115 | idempotency | R19 | resolved | explicit | AC-19 AC-28. |
| E116 | ordering | R19 | resolved | explicit | AC-29. |
| E117 | precision | R19 | resolved | explicit | AC-28 AC-29 AC-39. |
| E118 | adjacency | R20 | resolved | explicit | AC-30 AC-31 AC-38. |
| E119 | boundary | R20 | resolved | explicit | AC-30 AC-31 AC-39 — Round trips preserve legal extrema and saved layout extents without truncation. (manual addition) |
| E120 | concurrency | R20 | resolved | explicit | AC-30 AC-32. |
| E121 | empty | R20 | resolved | explicit | AC-30. |
| E122 | idempotency | R20 | resolved | explicit | AC-30 AC-32. |
| E123 | ordering | R20 | resolved | explicit | AC-30 AC-38. |
| E124 | precision | R20 | resolved | explicit | AC-30 AC-39 — Fractional quantities such as condition/time survive; integral item counts remain exact. (manual addition) |
| E125 | concurrency | R21 | resolved | explicit | AC-15 AC-33 AC-34. |
| E126 | empty | R21 | resolved | explicit | AC-33 — Empty slots, pages, filtered results and spectator candidates have usable explicit states. (manual addition) |
| E127 | idempotency | R21 | resolved | explicit | AC-16 AC-19 AC-34. |
| E128 | ordering | R21 | resolved | explicit | AC-34 — Panel priority prevents one click from also attacking or activating a world target. (manual addition) |
| E129 | adjacency | R22 | resolved | explicit | AC-35 AC-38. |
| E130 | concurrency | R22 | resolved | explicit | AC-35 AC-37. |
| E131 | empty | R22 | resolved | explicit | AC-36 AC-37. |
| E132 | idempotency | R22 | resolved | explicit | AC-35 AC-37. |
| E133 | ordering | R22 | resolved | explicit | AC-35 AC-36. |

## Prohibitions (must-NOT)

**Coverage:** 2/2 applicable prohibitions resolved · 2 test · 0 judgment · 0 dismissed · 0 unresolved.

The adversarial recall/precision pass reviewed all 22 requirements for unwanted product changes. Ordinary correctness candidates about lost items, invalid data, duplicated commands, cleanup and panel races stay in the edge/acceptance criteria above; they are not duplicated as bespoke prohibitions. Two cooperative-play/privacy limits were retained and explicitly accepted by the creator.

| ID | Prohibition | Requirements | Status | Verification / acceptance |
| --- | --- | --- | --- | --- | --- |
| P01 | MUST NOT damage allies or grant crowns combat bonuses, an equipment slot or loot value. | R14, R15, R17, R19, R21 | resolved | test; AC-22, AC-28, AC-40 |
| P02 | MUST NOT expose another controlled survivor's private inventory for inspection/manipulation by another participant or grant remote shared-stash access. | R11, R12, R21 | resolved | test; AC-15, AC-41 |

No Phase 6 wired negative check or known-bad/known-clean fixture exists yet, so `check_kind`, `check_target`, `check_rule`, `check_violation_fixture` and `check_clean_fixture` are deliberately uncaptured. Plan-phase must wire the mechanical checks and record actual descriptors/control fixtures; downstream verification must keep missing/unprovable checks flagged rather than infer a pass. Requirement resolution here is not a claim that a test has run.

## Ambiguity Report

| Dimension | Score | Minimum | Status | Notes |
| --- | --- | --- | --- | --- |
| Goal Clarity | 0.96 | 0.75 | met | Ordinary survivors, real possessions/combat, death and control are observable. |
| Boundary Clarity | 0.97 | 0.70 | met | Stationary targets now; autonomous campaign and interactive legacy extraction retain later owners. |
| Constraint Clarity | 0.96 | 0.65 | met | Baselines, authority, precision, access and timing rules are explicit. |
| Acceptance Criteria | 0.97 | 0.70 | met | 41 pass/fail criteria; 133 explicit edge resolutions; two accepted negative criteria. |
| **Ambiguity** | **0.036** | **≤ 0.20** | **passed** | Unrounded: 1 − (0.35×0.96 + 0.25×0.97 + 0.20×0.96 + 0.20×0.97) = 0.0355. |

All dimension minimums are met. No unresolved edge/prohibition or below-minimum planner assumption is carried forward. Implementation choices remain for discuss-phase; future implementation/verification is still pending.

## Interview Log

| Round / step | Perspective | Question / decision | Recorded result |
| --- | --- | --- | --- |
| Initial | Scout | Read approved sources, movement/LAN code and prior verification before asking questions. | Goal 0.90; boundary 0.82; constraint 0.78; acceptance 0.84; recomputed ambiguity 0.156. |
| 1 | Researcher | Minimum playable hostile behavior? | Stationary hostile targets with triggered attacks; no pursuit requirement. |
| 1 | Researcher | Legacy-layout delivery boundary? | Complete representation/round trips now; changed-configuration viewing/extraction in Phase 9. |
| 1 | Assessment / gate | Goal 0.94; boundary 0.94; constraint 0.86; acceptance 0.88; ambiguity 0.088. | Gate met; creator requested another round. |
| 2 | Researcher / Simplifier | Food at full health and stamina? | Consume one real food unit and start cooldown; clamp both attributes. |
| 2 | Researcher / Simplifier | One usable and one broken held melee weapon? | Attack with the usable weapon's own cost/timing and keep broken gear equipped. |
| 2 | Assessment / gate | Goal 0.96; boundary 0.95; constraint 0.92; acceptance 0.94; ambiguity 0.055. | Gate met; creator requested another round. |
| 3 | Boundary Keeper | Automatic replacement of occupied equipment slots? | Excluded; manually unequip first. |
| 3 | Boundary Keeper | Merge exceeds destination stack capacity? | Transfer only what fits; leave the remainder in its source. |
| 3 | Assessment / gate | Goal 0.96; boundary 0.97; constraint 0.92; acceptance 0.96; ambiguity 0.046. | Gate met; creator requested another round. |
| 4 | Failure Analyst | Hit breaks otherwise usable armor? | Full protection for that hit; later hits see broken condition. |
| 4 | Failure Analyst | Attack input during cooldown/insufficient stamina? | Discard; require a fresh ready input, without a pending queue. |
| 4 | Assessment / gate | Goal 0.96; boundary 0.97; constraint 0.96; acceptance 0.97; ambiguity 0.036. | Creator selected proceed and write after completeness checks. |
| Edge probe | Completeness | Timing/combat set, including exact thresholds, fractional wear and all-broken held gear. | Accepted as explicit pass/fail criteria. |
| Edge probe | Completeness | Inventory set, including identity after splits/merges, nesting, quick references, placement and Unicode/case-insensitive filtering. | Accepted as explicit pass/fail criteria. |
| Edge probe | Completeness | Shared-state/lifecycle set, including host order, waiting ties, emptiness, cleanup, snapshots, UI and fixture/evidence integrity. | Accepted as explicit pass/fail criteria. |
| Prohibition probe | Product limits | Cooperative combat/crown limits and private/local inventory access. | Both retained as explicit must-NOT criteria. |

Scores above are recalculated from the recorded dimensions. Initial 0.156 and round-1 0.088 correct the earlier narration's arithmetic of 0.155 and 0.089 respectively; gate outcomes are unchanged. Later displayed scores round the weighted formula to three decimals.

---

*Phase: 06-survivors-inventory-and-combat*
*Spec created: 2026-11-16*
*Next step: $gsd-discuss-phase 6 — implementation decisions for the behavior locked above.*
