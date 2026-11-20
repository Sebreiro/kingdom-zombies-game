# Phase 8: Zombie campaign and outcomes — Specification

**Created:** 2026-11-18 (Europe/Paris)
**Ambiguity score:** 0.028 (gate: ≤ 0.20)
**Requirements:** 23 locked, covering 19 owned project requirements and four delivery or handoff obligations
**Acceptance:** 57 future checks. All project requirement IDs, primary owners and Pending statuses are retained.

## Goal

Solo and four-player LAN players can explore and defend the authored camp, fight a complete seven-archetype zombie campaign, destroy six permanent portals directly or through paid warrior assaults, and reach victory, continued play or defeat with consistent shared state.

## Background

[FoundationSession](../../../src/core/FoundationSession.cs) provides authoritative movement and participant state. Phase 4 supplies authored content and inspection fixtures, including the [map](../../../src/godot-game/content/world/map.json), [time](../../../src/godot-game/content/config/time.json), [zombie progression](../../../src/godot-game/content/config/zombie-progression.json), [portals and assaults](../../../src/godot-game/content/config/portals-assaults.json) and [map and warning definitions](../../../src/godot-game/content/ui/map-spectator-warnings.json). Those definitions describe historical authoring defaults and preview surfaces. The Phase 6 survivor and Phase 7 camp contracts supply gameplay prerequisites. Phase 8 integrates the actual campaign, its presentation and the shared outcomes in normal gameplay.

Scope references are the [0.1 game specification](../../../specs/0.1/initial-spec.md), [project definition](../../PROJECT.md), [approved requirement IDs](../../REQUIREMENTS.md), [Phase 8 roadmap](../../ROADMAP.md#phase-8-zombie-campaign-and-outcomes), [Phase 6 specification](../06-survivors-inventory-and-combat/06-SPEC.md), and [Phase 7 specification](../07-camp-economy-and-development/07-SPEC.md). The decisions below take precedence over earlier wave, cap, warning and interface wording in those sources. D-08 through D-12 record the decisions approved in the [Phase 8 discussion](08-CONTEXT.md) on 2026-11-21.

## Creator decisions

| Decision | Rule |
| --- | --- |
| D-01 — Regular and retaliation sources | For each new regular or retaliation event select the closest surviving portal to the main building on each side. There are zero, one or two sources. Nightfall uses the portals alive at that nightfall. Retaliation uses the portals alive after all destruction in that resolved gameplay moment. Each destruction remains a distinct event. |
| D-02 — No living-zombie spawn caps | Remove the shared 300-living-zombie limit and the separate living-defender limits. A ready wave can release at its configured pace regardless of living counts. Approximately 300 active zombies remains a reference-PC stress workload under REL-04. |
| D-03 — Defender lead time | A qualifying hit can provoke defenders at the actual damaged portal, even when another portal is closer to camp. Keep the two-second baseline delay. Give no dedicated cue, sound or player alert for the pending defender wave. |
| D-04 — Accepted releases | A portal finishes its current release before starting another. For the next release, choose among ready entries using the source priority of defense, retaliation and regular by default. Capture a wave's effective stage and generation inputs at trigger time. At dawn expire unspawned regular work. A destroyed source cancels its own pending spawns without transferring them or changing already living enemies. |
| D-05 — Assault readiness and roster | Prefer ready armed warriors and the existing melee/archer mix. Fill vacancies with other available warriors who can fight, including injured fists-only warriors who meet the same fitness rule. The roster is fixed at launch. Death or player takeover removes a member from the order without a replacement. |
| D-06 — Assault return | Recall, success or an obsolete target starts an ordinary return to the original side. The side's assault slot stays occupied until its remaining squad members are back and can rejoin allocation. An order with no remaining members closes. |
| D-07 — Dawn and dusk | At dawn eligible zombies retreat with belongings. A zombie arriving at a still-living portal in the same moment as dusk exits first. A zombie that has not exited resumes attacking at dusk and may retreat at the next dawn. If an exit coincides with destruction of the final portal, destruction wins and the zombie stays with its belongings. |
| D-08 — Immediate retaliation | Each distinct portal destruction makes its surviving sources' retaliation entries ready as soon as the destruction moment is resolved. Use the current-release and ready-entry rules in D-04. Retaliation has no dedicated warning, countdown, cue, sound or lead time. |
| D-09 — Portal destruction feedback | Show the destruction effect and permanent ruins in the world. Update the marker for a portal already known to the camp. Destruction produces no separate local or camp-wide notice and reveals no unknown site. |
| D-10 — Map and clock | Use a fixed full-world compact minimap. M toggles the expanded full-world map and Escape closes it. The expanded map owns gameplay input while simulation continues. The HUD shows day, daylight/night and stage. In the game background, the sun crosses left to right from dawn to dusk and the moon from dusk to dawn. Use the sky motion in place of a countdown or progress bar. |
| D-11 — Contextual rally control | At each physical rally, show one nearby action for that side. Show stationed and ready counts, planned composition, the next known portal and carried-money price. One press sends the squad and one press recalls it for free while the order is active. Show the unavailable reason on approach. During return, show a disabled Returning label and the count still on the way. |
| D-12 — Outcome controls | Show a victory card over the live world and any open panel while preserving the current panel and its input ownership. The card consumes its own clicks. Solo Continue closes it locally and the LAN host's Continue closes it for all participants. Defeat ends active play and shows the day, the applicable defeat causes and any earlier victory. |

D-01 through D-04 and D-08 through D-09 govern wave sources, release capacity, readiness and player feedback. Reconcile WORLD-03 and ZOMB-04 through ZOMB-07, the corresponding roadmap success criteria, specification §§3.2 and 9.4–9.6 and A-02/A-34/A-35, and their gameplay data, catalog and checks when implementing Phase 8. In DT-15, the original two-second `warningSeconds` value for defenders represents their delay. The original ten-second retaliation warning value and retaliation warning samples are historical authoring inputs. Use D-10 through D-12 for the corresponding map, rally, outcome and input provisions of the game specification and its acceptance scenarios.

## Requirements

1. **R01 — Authored world:** Play the full authored map with six fixed portal sites and all baseline terrain, sites, clearings, plants and wildlife zones.
   - Source: WORLD-01.
   - Current: DT-01, DT-09 and the world pack describe a full map. Foundation movement uses a small bounded world. Phase 7 plans representative camp sites and barriers.
   - Target: Use the authored map and normal run start in solo and LAN. Preserve all site IDs, positions, terrain continuity, unlock prerequisites, and Phase 6/7 starting-state rules.
   - Acceptance: AC-01, AC-02, AC-03.

2. **R02 — Portal passage:** Keep living portals impassable to survivors at all jump heights and traversable by zombies, then permanently open each destroyed site.
   - Source: WORLD-02.
   - Current: DT-15 and the world pack describe barriers. Phase 7 plans representative reachability and portal states.
   - Target: Use the same live barrier state for walking, jumping, navigation, and site reachability. Destruction opens the route exactly once.
   - Acceptance: AC-04, AC-05.

3. **R03 — Campaign clock:** Advance authoritative day, night, grace, stage and once-only dawn events across exact configured boundaries.
   - Source: WORLD-03.
   - Current: DT-02 specifies time defaults. Phase 7 plans elapsed time and dawn activity for camp jobs.
   - Target: Use one host clock for day, night, stage and events. Publish the current day and phase, and settle all crossed transitions in order.
   - Acceptance: AC-06, AC-07, AC-08.

4. **R04 — Zombie roster:** Run all seven archetypes with their distinct movement, attacks and real equipment, using shared combat and targeting rules.
   - Source: ZOMB-01.
   - Current: DT-12 specifies the archetypes. Phases 6 and 7 plan shared combat, representative stationary enemies, roles and equipment.
   - Target: Implement shambler, runner, brute, armored, spitter, zombie archer and acid bloater using their configured stats and actual weapons, armor, ammunition, durability and weight.
   - Acceptance: AC-09, AC-10.

5. **R05 — Generated belongings:** Generate each zombie's mandatory money, legal clothing and archetype guarantees before optional real equipment and loot at creation.
   - Source: ZOMB-02.
   - Current: DT-13 and the item pack describe physical loot and quality tiers. Phase 6 plans item identity, nested inventories and corpse transfer.
   - Target: Roll legal inventories before placing a zombie in combat. Reserve legal capacity for mandatory money, wearable clothing and archetype guarantees before optional items.
   - Acceptance: AC-11, AC-12.

6. **R06 — Physical ammo and fallback:** Spend real arrows, refill from legal carried reserves and use a usable fallback or fists as required for NPCs and zombies.
   - Source: COMBAT-04.
   - Current: Phases 6 and 7 specify shared equipment and NPC carried reserve semantics. The complete campaign roster is a later consumer.
   - Target: Spend an arrow and apply bow wear per shot through the shared inventory contract. Reserve transfers are legal item movements. NPCs restock only when safe. Zombies use their own belongings.
   - Acceptance: AC-13, AC-14.

7. **R07 — Spit and cosmetic acid:** Stop spit at the first eligible hit or solid terrain and render bloater acid as a purely cosmetic death effect.
   - Source: COMBAT-07.
   - Current: Combat and effects definitions describe the attacks. Phase 4 previews their resources and Phase 6 plans common first-hit combat.
   - Target: Resolve spit in travel order and keep the acid splash and stains cosmetic at any display or sound setting.
   - Acceptance: AC-15, AC-16.

8. **R08 — Independent progression:** Use the independent editable progression table for future wave counts, variant weights and quality without living-stat inflation or player-count scaling.
   - Source: ZOMB-03.
   - Current: DT-14 contains an independent stage table. Campaign runtime has no production wave generator yet.
   - Target: Use validated host progression data to generate future counts, weights and quality. Keep the stage and effective generation inputs fixed for each accepted wave. Preserve living enemies without player-count scaling.
   - Acceptance: AC-17, AC-18, AC-19.

9. **R09 — Regular portal waves:** Schedule one regular wave at the closest surviving portal to camp on each side at eligible nightfall, with opening grace applying only to regular waves.
   - Source: ZOMB-04.
   - Current: DT-02 and DT-14/15 describe grace, eligible nights and release pacing.
   - Target: At each eligible nightfall, select the closest surviving portal to the main building on each side and give each selected portal one regular wave. A side with no live portal supplies none. Lock the sources when that nightfall is resolved.
   - Acceptance: AC-20, AC-21, AC-22.

10. **R10 — Provoked defenders:** Trigger defenders from the damaged living portal using the configured delay, cooldown and stage, without a defender warning or a living count limit.
   - Source: ZOMB-05.
   - Current: DT-15 records the original defender trigger, warning timing, release window, cooldown, health, minimum stages and source caps. Phase 8 uses its lead time and replaces its defender presentation and cap rules.
   - Target: Only accepted damage to a living portal can trigger a defender event. Defenders come from that exact portal and use a two-second baseline delay, a 10 s normal release window and a 30 s cooldown, including during opening grace. The delay has no dedicated world, sound or UI warning. Ordinary damage feedback remains applicable.
   - Acceptance: AC-23, AC-24.

11. **R11 — Distinct retaliation:** Make one extra retaliation wave immediately ready at the closest surviving portal to camp on each side for each permanent portal destruction.
   - Source: ZOMB-06.
   - Current: DT-15 and Phase 4 fixtures record the earlier authoring baseline, including a ten-second warning lead. D-08 and D-09 supersede that baseline.
   - Target: For every permanent portal destruction, queue a distinct, immediately ready retaliation at the closest surviving portal to camp on each side. Select from the resolved post-destruction set. Keep the 60 s normal release window and source arbitration. A side with no surviving portal supplies none. Present destruction through world effects, ruins and known map markers.
   - Acceptance: AC-25, AC-26.

12. **R12 — Queue arbitration:** Arbitrate releases at each source by readiness and queue priority without living count limits, and preserve live enemies and their belongings through cancellations.
   - Source: ZOMB-07.
   - Current: DT-14 previously described the two layers of living count limits alongside source queue priority. The creator removed the limits for Phase 8.
   - Target: Finish a current release before choosing another at that portal, subject to expiry or source destruction. Choose the next ready release by the source's configured priority, with defense ahead of retaliation and regular by default. Preserve same-priority event ordering. Population never gates a ready release.
   - Acceptance: AC-27, AC-28, AC-29.

13. **R13 — Dawn retreat and exit:** Move eligible zombies toward surviving portals at dawn and exit with belongings, with retreat ending permanently after final-portal destruction.
   - Source: ZOMB-08.
   - Current: DT-14 records the retreat policy. Phase 7 plans independent civilian retreat.
   - Target: At dawn send eligible living zombies by ordinary movement toward the nearest living portal on the same side, otherwise the nearest living portal overall. Resolve destroyed destinations again and stop all retreat at final destruction.
   - Acceptance: AC-30, AC-31, AC-32, AC-33, AC-34.

14. **R14 — Warrior defense allocation:** Allocate warriors between live defensive sides, station at current border or refuge and fall back while preserving combat priorities.
   - Source: ARMY-01.
   - Current: Phase 7 plans nearest-wall/refuge guarding and current borders. DT-15 describes the Phase 8 allocation policy.
   - Target: Balance eligible warriors by current side counts and shorter travel, using the outermost living wall or same-side refuge. Preserve combat and assault priorities through rebalance and breach.
   - Acceptance: AC-35, AC-36.

15. **R15 — Rally purchase and target:** Validate and atomically pay for a ready side squad to assault its next surviving portal under one active order per side.
   - Source: ARMY-02.
   - Current: DT-15 and camp UI pack describe the rally. Phase 7 plans money, roles and local guarding.
   - Target: At the selected side's valid rally, atomically select eight ready warriors, target the next living portal and deduct 25 money from the acting player's carried inventory once. Prefer ready armed warriors under the existing melee/archer mix. If those warriors leave vacancies, an available warrior who can fight with fists, whether injured or at full health may fill a slot.
   - Acceptance: AC-37, AC-38, AC-39, AC-40, AC-41.

16. **R16 — Recall and return:** Return surviving assault warriors after free recall, success or obsolete target, with ordinary travel and no automatic next attack.
   - Source: ARMY-03.
   - Current: DT-15 describes recall and normal travel. There is no production campaign assault lifecycle yet.
   - Target: Recall for free at that side's current rally. After recall, success or obsolete target, surviving original squad members return to their side and resume allocation when eligible. Those who die or come under player control leave the order without replacements.
   - Acceptance: AC-42, AC-43.

17. **R17 — Defeat conditions:** Defeat the camp when main health reaches zero or the living controlled-or-eligible survivor set becomes empty.
   - Source: RUN-05.
   - Current: DT-01 describes outcome predicates. Phase 6 plans control/succession and Phase 7 plans main destruction.
   - Target: Defeat immediately on actual main destruction or loss of every living controlled or eligible recruited survivor. Apply a finished defeat state to the shared run.
   - Acceptance: AC-44, AC-45.

18. **R18 — Victory and continued play:** Immediately award final-portal victory and allow continuing play with surviving enemies, corpses and possessions.
   - Source: RUN-06.
   - Current: DT-01 and the outcome resource describe victory. No production six-portal clear exists yet.
   - Target: Record victory in the authoritative gameplay moment when the last portal is destroyed and keep the world active through victory and Continue.
   - Acceptance: AC-46, AC-47.

19. **R19 — Outcome precedence:** Resolve simultaneous first victory and defeat as defeat and preserve a previously awarded victory after a later loss.
   - Source: RUN-07.
   - Current: DT-01 and source §2.3 define simultaneous precedence and recorded victory. Foundation session has no production campaign result.
   - Target: Evaluate first results from one authoritative resolved gameplay moment and retain the first awarded victory across later continuation defeat.
   - Acceptance: AC-48, AC-49.

20. **R20 — Live shared exploration:** Reveal live camp exploration through ordinary survivors, with independent cameras, shared discovery and a minimap that respects unknown locations.
   - Source: Phase 8 handoff for WORLD-06.
   - Current: Foundation provides local and LAN movement views. Phase 4 has map/warning fixtures and Phase 7 defines borders.
   - Target: Use independent survivor cameras and shared authoritative discovery. Preserve established spectator following while filtering the minimap to information the camp knows.
   - Acceptance: AC-50, AC-51.

21. **R21 — Campaign presentation:** Present the live clock and sky, map, contextual rally and outcome actions using packaged assets and translatable feedback with correct input ownership.
   - Source: Phase 8 player surfaces.
   - Current: Phase 4 supplies English strings, preview surfaces and effects. Existing foundation foreground controls are a starting point.
   - Target: Use the produced map, actors, structures, effects, layouts, audio and catalog in ordinary campaign play. Apply D-09 through D-12 to portal feedback, the map, the rally and outcome interactions. Preserve the input ownership of open panels and the visibility of other required critical feedback.
   - Acceptance: AC-52, AC-53.

22. **R22 — Coherent campaign state:** Demonstrate coherent campaign snapshots across transitions and preserve accepted state needed by later persistence and late-join integration.
   - Source: Phase 9 handoff for WORLD-04 / SAVE-03 / SESSION-03.
   - Current: Phases 3, 6 and 7 supply session/state boundaries or plan coherent state contracts. Phase 9 owns actual save storage, load and late-join acceptance.
   - Target: Carry full campaign state through a coherent capture and restore proof for the later Phase 9 consumers. Include queued commitments and deterministic generation state needed for continuity.
   - Acceptance: AC-54, AC-55.

23. **R23 — Production campaign verification:** Demonstrate real solo and four-player scenarios through normal controls with recorded scope, identities and required grouped evidence.
   - Source: Phase 8 evidence.
   - Current: Prior phases supply reusable verification conventions. Existing asset inspections and future camp plans do not supply completed Phase 8 gameplay evidence.
   - Target: Save repeatable source and normal-input scenarios with stable links to project IDs and group the required rendered cases by compatible game run.
   - Acceptance: AC-56, AC-57.

## Boundaries

**In scope**

- One complete playable authored map with live portals, current site gates, normal survivor and zombie movement, live camp exploration, separate cameras and a shared minimap.
- The configurable campaign clock, all seven combat archetypes and original item ownership contracts. Spawned equipment, attack effects, corpses and exit behavior work in the same game as camp work and defenses.
- Scheduled waves, provoked defenders, distinct retaliation, queue and cancellation rules, dawn retreat and the return to attacks at dusk.
- Warrior allocation, side rally purchases, fixed squads, recall, return and live side fallback.
- Campaign HUD and sky, map, contextual rally, result screens and translatable feedback. Live solo and LAN results and a coherent state proof for future save and join consumers.

**Out of scope**

- Phase 9 owns durable save and load, automatic save execution, differing configuration on load, full reconnect and late-join acceptance, and completed settings and presentation requirements. Phase 8 carries the campaign state, identities and trigger needed by those consumers.
- The product retains its authored map and normal physical transactions. Procedural maps, new gameplay currencies, revival, equipment shops, crafting, additional factions and other platform networking remain outside version 0.1.
- Supported display and mixed-OS release acceptance, full reference-PC stress, saved/resumed endurance, and baseline pacing sign-off retain their release owners. Phase 8 plans and runs its scoped verification.

## Constraints

- One host resolves authoritative movement, accepted time boundaries, damage, spawning, spending and outcomes. Valid clients see the resulting shared state. Guest settings and display timing cannot change those rules.
- Keep actual item identity, contents, counts, weight and condition through generation, combat, departure, death and snapshots. All users share the same starting camp and effective progression regardless of player count. Reuse the Phase 6 and Phase 7 gameplay and asset contracts.
- Use current validated campaign data, exact authored IDs, stable event identities, elapsed-time semantics and integer counts/money. The closest portal is measured by distance from the main building within its authored side. Equal compatible ordering keys use a stable identity. Keep unknown locations hidden in map and campaign feedback.
- Progression uses existing configurable tables and data validation. Apply the approved changes to the earlier cap and warning data and documentation. Other numerical defaults apply until playtesting justifies an approved balance change.
- Carry enough state for Phase 9 to preserve accepted generation inputs, random progression, live inventories and corpses, portal damage/destruction and retaliation commitments, paid and returning squads, dawn receipts and recorded outcomes, including the victory card's shared dismissal. Capturing or restoring a state cannot synthesize or replay accepted actions.
- [Local testing strategy](../../../docs/workflow/local-testing.md) governs phase evidence. Implement and review the whole phase before rendered tests. Use 1280×720 on the development OS, no VM, and grouped scenario runs. Keep original and repaired evidence distinct.

## Acceptance Criteria

- [ ] **AC-01 (R01)** — Start fresh solo and four-player runs at day 1 dawn. Both have eight recruited survivors, separate controlled assignments, one shared kit, tier-1 main, unbuilt sites, and six active portals. Compare actual state and item counts, including late-arriving network messages.
- [ ] **AC-02 (R01)** — Traverse both sides of the authored map after opening routes. Verify edges at ±320 u, portals at ±100/190/280 u, walls at ±25/75/155/245 u, towers at ±21/71/151/241 u, clearing groups at ±50/125/215 u, berries at ±10/50 u, fruit at ±16 u, and wildlife zone centers at ±45/130/230 u with 15 u widths. Verify every identity exactly once and walkability for NPCs.
- [ ] **AC-03 (R01)** — Clear an outer site's trees while an intervening portal is alive, and destroy a portal while its associated trees remain uncleared. Either incomplete prerequisite leaves the paired site locked. Both together unlock that pair. Clearing leaves plants intact and yields no wood.
- [ ] **AC-04 (R02)** — Approach each active portal from both sides on foot and at supported jump/sprint limits. A survivor never reaches the far side through it or by crossing it in a single movement update. Zombies can cross while it lives.
- [ ] **AC-05 (R02)** — Destroy a portal and repeat movement, NPC navigation and site prerequisite checks. Its barrier remains open across dawn, repeated hits, and snapshot restoration. Other live portals retain their barriers.
- [ ] **AC-06 (R03)** — Observe configured daylight 600 s and night 300 s. Just before a boundary stay in the previous phase. At it, change once. First regular night is night 2. Stages change at dawn on days 4, 7, 10 and 13 by default and clamp to the last row. Show day, phase and stage in the HUD and use the host clock for the sun's daylight crossing and the moon's night crossing. The sky motion supplies the progress cue.
- [ ] **AC-07 (R03)** — Advance across multiple boundaries and replay an already observed update or restore mid-phase. Advance day/stage, refill plants, add the wildlife batch and order eligible zombie retreat once per dawn. Capture the stable autosave trigger for Phase 9. Fractional elapsed time survives coherent handoff and visual transitions.
- [ ] **AC-08 (R03)** — Use the inventory, map, camp and victory overlays and the contextual rally during an active run. Authoritative time, danger and simulation continue during these interactions. Finished defeat halts active play.
- [ ] **AC-09 (R04)** — Generate each variant through production wave generation in controlled scenarios. Compare speed, health, attacks, natural traits, real equipped-item effects and tier data to its definition. Demonstrate that a compatible non-archer bow user uses the same ammo contract.
- [ ] **AC-10 (R04)** — Observe melee priority at contact, usable ranged gear, spitter fallback and closure for melee. Show zombies attack obstructing defenses, can engage nearby threatening survivors or exposed towers within the baseline 12 u, and defenders prioritize nearby attackers. Verify the shared per-target melee limits of six per structure and three per survivor.
- [ ] **AC-11 (R05)** — Compare living inventories with corpse contents after spending arrows and wearing equipment. Verify real money and wearable clothing, usable body armor on armored types, usable bow and 24–48 physical arrows on zombie archers, and 15–30 baseline arrows on incidental bow users.
- [ ] **AC-12 (R05)** — Drive quality/weight bounds and optional capacity failure using reproducible seeds. Equip and place only legal items, including arrow-hand stacks for bow loadouts. An optional item that fails fit leaves mandatory items, stable identities and physical weight valid. Death transfers existing items without a new roll.
- [ ] **AC-13 (R06)** — Exhaust the hand stack with reserve arrows available, exhaust every reserve, then break the current weapon. Observe carried-only refill, fallback to a usable carried attack or fists, and accurate count and durability including after death.
- [ ] **AC-14 (R06)** — Compare player manual refill with NPC/zombie auto refill. Ordinary NPCs can seek the stash when safe. Zombies never use it. Tower equipment and ammo remain structural and fired arrows produce no recoverable item.
- [ ] **AC-15 (R07)** — Place eligible survivors and defenses at distinct distances, with allied zombies and solid terrain as controls. The first eligible survivor or defense takes a direct hit and the projectile stops, or earlier solid terrain stops it. An exact collision tie resolves stably and never damages two targets.
- [ ] **AC-16 (R07)** — Kill a bloater beside targets and a wall. Show the baseline 3 u splash and 20 s stain while comparing health, status, movement, durability and collision against a control with identical combat input. Acid adds no gameplay effect.
- [ ] **AC-17 (R08)** — Load the default five rows, exact seven weights summing to 100, stage references and Q0–Q4 bindings. Add a valid later stage and edit counts/weights/quality separately. Future generated outcomes obey changes and existing living state is identical.
- [ ] **AC-18 (R08)** — With the same accepted events, seed and simulation inputs, compare one and four connected players. Wave math and initial supplies match. Reject malformed rows, nonfinite/negative timings, invalid count/weight bounds or references with file, field, value and reason before play. No live reloading or retroactive reroll.
- [ ] **AC-19 (R08)** — Trigger a defender wave and retaliation before escalation dawn, then defer their release past it. Each uses the trigger-time stage and accepted mix/quality. A later event uses the new stage. Keep already generated items and random work intact. No waiting wave rerolls after restore.
- [ ] **AC-20 (R09)** — Run through night 1 and night 2 with six living portals. Night 1 has zero regular waves and still permits provocation and retaliation. Night 2 enqueues one wave at each of the two nearest portals and none at the four farther portals. At stage 0 each selected source commits six zombies, released at normal pacing over the configured 90 s window when uninterrupted.
- [ ] **AC-21 (R09)** — Destroy the nearest portal on one side before nightfall, then exhaust the living sources on that side in a separate case. Select the next living portal on that side when one exists. A side with no living portal contributes zero waves. Verify exact source identity and one wave per selected source, even when distances tie across sides or nightfall is repeated.
- [ ] **AC-22 (R09)** — Destroy a selected source mid-release. Cancel its unspawned regular remainder without transferring it. Keep the living spawns and belongings. The next eligible nightfall can select that side's next surviving portal.
- [ ] **AC-23 (R10)** — Compare proximity, zero/invalid damage, actual damage, a repeat just below and at 30 s from the previous trigger, and lethal damage before release. Only eligible damage can trigger a defender event, and a subsequently destroyed source releases no pending spawns. Observe the two-second minimum delay with no defender warning cue or alert and the 10 s normal release.
- [ ] **AC-24 (R10)** — Damage each nearest, middle and farthest portal in a controlled production scenario, including a farther portal while a nearer one is alive. Validate the actual damaged source, baseline health 2,500/5,000/8,000, defender count 6/10/14 and minimum stage 0/1/2. At trigger time lock the greater of current stage and the damaged portal's minimum. No global or local living count makes a ready defender wave wait.
- [ ] **AC-25 (R11)** — Destroy one portal during opening daylight grace, then destroy two portals in the same gameplay moment. For each destruction, only the nearest surviving portal of each nonempty side queues one immediately ready wave. If both sides have sources and two portals fall together, that is four commitments across the two selected portals. Compare a free and a busy source. A free source can start the release at once subject to ready-entry priority. A busy source finishes its current release first. Regular waves remain separate. Across solo and LAN, verify the absence of a retaliation banner, sound, countdown or lead time.
- [ ] **AC-26 (R11)** — Destroy a selected source while retaliation is queued behind another release, repeat a prior destruction update, and destroy the final portal. Cancel the destroyed source's unspawned work without moving it to another portal. Prior destruction updates create no new commitments. Subsequent destructions use the current nearest survivors and final destruction creates zero commitments. Verify destruction effects, permanent ruins, updated known map markers, undisclosed unknown locations and the absence of separate destruction notices.
- [ ] **AC-27 (R12)** — Start a regular release, then make retaliation and defenders ready. Observe one release at that portal at a time. Finish the current sequence, then select eligible defense before retaliation and regular. Simultaneously ready same-priority releases retain their distinct identities and stable trigger ordering. An item whose lead time has not elapsed cannot delay a ready item.
- [ ] **AC-28 (R12)** — Run equivalent ready releases below, at and above the retired 300 living-zombie threshold and above the retired 40/60/80 live defender limits for the corresponding sources. Each uses the same configured cadence regardless of counts or corpses. Different portals can release concurrently, while a single portal never interleaves active sequences. No enemies or possessions are removed to make room.
- [ ] **AC-29 (R12)** — At dawn expire only unspawned regular work. Pending defense and retaliation may continue to their releases. On source destruction cancel every unspawned entry from that source. Keep all already living enemies and exact receipts needed to avoid replay. Other sources continue independently.
- [ ] **AC-30 (R13)** — Track a same-side tie, an opposite-side fallback and a destination destroyed during travel. Observe stable target selection, legal routing, self-defense or attack on a blocking defense, and no teleportation. A defender or retaliator spawned after dawn gets its next applicable dawn order. At nightfall, a retreater that has not exited resumes attacking from its current position. It becomes eligible for a new retreat order at the next dawn if a portal still survives.
- [ ] **AC-31 (R13)** — Follow an eligible zombie through its still-living exit. It leaves with current belongings without creating a corpse or drop. Existing corpses stay, later waves use new identities, and after the final destruction pending retreat exits cease while enemies remain dangerous.
- [ ] **AC-32 (R13)** — Keep one retreating zombie alive and outside every portal just before dusk, then cross nightfall. It resumes hostile behavior in place with the same possessions. It retreats again at the next eligible dawn, while an already exited zombie never returns.
- [ ] **AC-33 (R13)** — At exact nightfall, resolve arrival at a still-living retreat destination before changing the zombie back to attack behavior. The arriving zombie exits once with its belongings, while a zombie that has not arrived switches to attacking in place.
- [ ] **AC-34 (R13)** — Resolve a retreat exit and destruction of the final portal in one authoritative moment. Destruction wins. The zombie stays at its current location with the same inventory and resumes hostile behavior. Neither a corpse nor an exit is recorded.
- [ ] **AC-35 (R14)** — Observe both sides at the 20 s baseline rebalance. Prefer fewer stationed then shorter route, with a stable final tie. Warriors in immediate combat, assault or assault return keep those commitments until eligible.
- [ ] **AC-36 (R14)** — Destroy the outer and then inner wall, and rebuild a wall while defenders are moving. Observe same-side fallback/reallocation by normal travel, current border/rally position, no tower-only expansion, and 15 u engagement / 16 u preferred archer distance defaults.
- [ ] **AC-37 (R15)** — Verify rally at 5 u inside the current outer wall or beside the refuge with no wall, and interaction within the baseline 3 u. Approach each physical rally and verify that one press sends its own squad toward its own side. Prefer 4 melee and 4 archers and fill composition shortages with other eligible warriors. One side can assault while the other has an independent order.
- [ ] **AC-38 (R15)** — Check the unavailable reason on approach with seven ready, insufficient carried cash despite funded stash, no target or a side slot occupied. Submit stale and invalid-access requests and verify that no invalid send is charged. Concurrent/repeated valid requests can accept one current order per side and charge the winning carried inventory once. Validate again at commit if cash, readiness, rally or target changed.
- [ ] **AC-39 (R15)** — Launch against the next living portal on that side after a nearer one is destroyed. Keep the mission through nightfall, engage nearby threats before targeting the portal, and permit direct player attacks without a rally purchase.
- [ ] **AC-40 (R15)** — With seven ready armed warriors and one available injured fists-only warrior who can still fight, accept one eight-member squad and charge once. With only seven total eligible fighters, reject it without charging. Prefer qualified armed warriors when enough are available.
- [ ] **AC-41 (R15)** — After launch kill a warrior or transfer one to player control under normal succession. Available original members continue using their existing gear. The removed slot stays vacant and an unselected warrior never receives a replacement order.
- [ ] **AC-42 (R16)** — Independently trigger recall, squad success, target destroyed by someone else, and an obsolete target on approach. Issue each return once, retain existing gear, preserve normal travel through current defenses, and apply no refund, teleport, replacement or automatic next target.
- [ ] **AC-43 (R16)** — Recall an outgoing squad in one press at its rally. While members remain on return, show a disabled Returning label and the current return count. Reject a new order on that side even with extra ready warriors. After the living returnees regain their side and can rejoin allocation, allow a valid new order. If no squad members survive, clear the order without a return barrier or refund.
- [ ] **AC-44 (R17)** — Destroy the main with survivors alive. Separately eliminate all living controlled or eligible recruits while the main stands. Purchase capacity, spectators, money, corpses, wildlife and structural archers never avert the result. Show defeat once with the day and actual cause, and end active play.
- [ ] **AC-45 (R17)** — Remove one leader with a viable controlled teammate or eligible recruited successor. The run stays active and the established Respawn flow applies. Repeated death or main-destruction messages never record an extra result or allow play to resume in a defeated run.
- [ ] **AC-46 (R18)** — Destroy the fifth portal, then the sixth while enemies are alive and an order or queue has pending work. Victory appears only on the sixth and the card warns that surviving zombies remain dangerous. Living enemies and possessions or corpses persist, and destroyed-source queues and zombie retreat end. Continue closes the card without resetting elapsed time, inventory or position.
- [ ] **AC-47 (R18)** — Observe a later dawn after victory with civilians and zombies present. Existing living zombies stay active, civilian retreat behavior follows its own rules, and a dismiss/reopen or repeated network update does not grant or replay victory again.
- [ ] **AC-48 (R19)** — Cause final destruction and each independent defeat reason in the same resolved moment, including reversed delivery order within that moment. Also trigger both defeat causes together and show both on the defeat screen. Report one defeat with no fleeting victory. Shifting the defeat to a later resolved moment yields an awarded victory first and a later defeat that shows the earlier victory.
- [ ] **AC-49 (R19)** — Compare host and guest outcomes, historical victory and continuation status. In LAN, press Continue as host and verify that all peers dismiss the card together while active play continues. Check that a guest cannot dismiss the shared card with Continue. Replay and restore either result at a coherent state boundary without changing its precedence or duplicating outcome prompts.
- [ ] **AC-50 (R20)** — Separate players and let a living NPC reveal new terrain within baseline 24 u. Verify a fixed full-world minimap frame with shared discovered terrain, position, known portals and defenses, and labeled or numbered teammates with color. Moving away or switching controlled survivor keeps discovered terrain.
- [ ] **AC-51 (R20)** — Test immediately below, at and beyond the reveal boundary plus overlapping reveal sets under duplicated/reordered updates. Undiscovered portals and live enemies in unexplored territory never appear. Known destruction updates remain correct, and spectator following cannot reveal new territory by itself.
- [ ] **AC-52 (R21)** — Check the HUD day, phase and stage, and view the sun and moon moving left to right in their respective phases with no countdown or progress bar. Toggle the expanded map with M, close it with M or Escape, and verify it blocks movement, attacks, quick-food keys and world interactions while open. Approach each rally and read stationed and ready counts, planned roles, next known portal, carried-money price and the Send, Recall or Returning state in the contextual prompt. Other required critical feedback remains visible while muted and during panels. Unknown locations stay undisclosed.
- [ ] **AC-53 (R21)** — At 1280×720 use normal keyboard and mouse with overlapping targets, access loss, longer text and muted sound. Verify readable campaign controls, protection against click-through and spectator transactions, and a visible reason for invalid actions. Verify movement and combat with only the victory card open, then show the card above an already-open panel and keep that panel and its existing input rules in place. Card clicks only act on the card. Guest Main menu exits only the guest and host Main menu uses the established session boundary. Defeat exposes the menu and new-run flow, plus earlier saves when Phase 9 supports the action.
- [ ] **AC-54 (R22)** — Capture immediately before and after dawn, defender activation, a retaliation commitment, paid assault, source destruction and final outcome. Restore and compare later observations with a no-restore control. Show the same queue, order and release progress, accepted generation inputs, event identities, random stream and previously generated inventory IDs and contents. Verify no second charge, retaliation commitment or dawn event.
- [ ] **AC-55 (R22)** — Include fractional elapsed time, portal and exploration facts, pending waves and their receipts, current and historical results, shared victory card dismissal and returning squad identities. Verify that restore preserves the current card state and messages cannot replay a result. Prove scoped in-memory state representation for the Phase 9 persistence and late-join consumers.
- [ ] **AC-56 (R23)** — Use reproducible scenarios to exercise all phase behaviors through actual domain code and normal solo/four-player live campaign actions, with recorded seeds and identities. Compare all clients with the host. Label accelerated/headless cases and retain actual timing and rendered input observations separately.
- [ ] **AC-57 (R23)** — After full implementation and review, run the declared selected set, including all active Critical and all new/changed/affected cases. Use development Windows 1280×720, no VM, explicit group order, reset and cleanup. If repairs are needed, retain failures and rerun the fresh entire selection on repaired inputs. Record broader release and Phase 9 proofs under their own owners.

## Source Acceptance Boundaries

| Source | Phase 8 requirements | Evidence responsibility |
| --- | --- | --- |
| A-01 | R01, R03 | Normal start, eight survivors, one kit, six living portals and day 1. Phase 6 owns the starting outfit/item contract. |
| A-02, A-03 | R03, R08–R11 | Opening grace and escalation through real production wave inputs. Phase 9 owns full config UI and load application. |
| A-07–A-09 | R17–R19 | Independent defeat conditions, final portal, continuing danger and deterministic precedence. A-08 uses the documented zombie-retreat interpretation. |
| A-14, A-26, A-31, A-32 | R04–R07 | Real gear, spends, fallback, projectiles and actual corpse ownership across all seven archetypes. Existing structural-archer and item contracts remain in force. |
| A-16, A-17, A-33 | R01–R03, R14–R16 | Live map barriers, clearing gates, camp boundaries, and uninterrupted assault combat at nightfall. |
| A-34–A-36 | R09–R13, R21 | Verify provocation with its silent delay and distinct immediate retaliation commitments without notice. Select only the nearest surviving source per side, exercise uncapped pacing and cancellation, and prove ordinary exits and possession preservation. |
| A-37 | R14–R16, R21 | Live allocation, paid assault, recall, success, obsolete-target return and exact order lifecycle. |
| A-30, A-41, A-42, A-45 | R03, R08, R20, R22 | Live day/state behavior and coherent campaign representation now. Phase 9 owns real disk persistence, changed settings on load and late join. |
| A-46, A-47 | R21, R23 | Relevant muted/visible and input checks now. Complete settings, supported platform, reference performance and release endurance retain their roadmap owners. |

Preserve the [project's A-08 interpretation](../../REQUIREMENTS.md#acceptance-criteria). After the final portal falls, living zombies cease dawn retreat and ordinary civilian retreat still follows its own rules.

## Edge Coverage

**Coverage:** 164/164 applicable edges resolved · 160 explicit · 4 held-out backstops · 0 dismissed · 0 unresolved.

The original compiled edge probe proposed 160 edges on the 23 requirement statements. Manual review added four applicable pairs for identity and exact outcome precedence. The shared coverage validator checked that set. The resolutions below reflect the subsequent Phase 8 discussion and remain verification obligations for implementation.

| Edge | Category | Requirement | Status | Verification | Resolution |
| --- | --- | --- | --- | --- | --- |
| E001 | boundary | R01 | resolved | explicit | AC-01, AC-02, AC-03. Exact site sets, coordinate signs and overlapping map zones |
| E002 | adjacency | R01 | resolved | explicit | AC-01, AC-02, AC-03. Exact site sets, coordinate signs and overlapping map zones |
| E003 | empty | R01 | resolved | explicit | AC-01, AC-02, AC-03. Exact site sets, coordinate signs and overlapping map zones |
| E004 | ordering | R01 | resolved | explicit | AC-01, AC-02, AC-03. Reorder map input, repeat run setup and compare one shared kit at both player counts. |
| E005 | precision | R01 | resolved | explicit | AC-01, AC-02, AC-03. Exact site sets, coordinate signs and overlapping map zones |
| E006 | idempotency | R01 | resolved | explicit | AC-01, AC-02, AC-03. Reorder map input, repeat run setup and compare one shared kit at both player counts. |
| E007 | concurrency | R01 | resolved | explicit | AC-01, AC-02, AC-03. Reorder map input, repeat run setup and compare one shared kit at both player counts. |
| E008 | encoding | R01 | resolved | explicit | AC-01, AC-02, AC-03. Compare stable source IDs in DT-09 and the live map. Mirrored prefixes and translated names preserve distinct identities. |
| E009 | boundary | R02 | resolved | explicit | AC-04, AC-05. Both approach sides and jump or long-update barrier reach |
| E010 | adjacency | R02 | resolved | explicit | AC-04, AC-05. Both approach sides and jump or long-update barrier reach |
| E011 | empty | R02 | resolved | explicit | AC-04, AC-05. Both approach sides and jump or long-update barrier reach |
| E012 | ordering | R02 | resolved | explicit | AC-04, AC-05. A repeated destruction or restore cannot reopen another portal or resurrect the destroyed one. |
| E013 | precision | R02 | resolved | explicit | AC-04, AC-05. Both approach sides and jump or long-update barrier reach |
| E014 | idempotency | R02 | resolved | explicit | AC-04, AC-05. A repeated destruction or restore cannot reopen another portal or resurrect the destroyed one. |
| E015 | concurrency | R02 | resolved | explicit | AC-04, AC-05. A repeated destruction or restore cannot reopen another portal or resurrect the destroyed one. |
| E016 | boundary | R03 | resolved | explicit | AC-06, AC-07, AC-08. A tick below, at and across dawn/dusk or a stage boundary |
| E017 | adjacency | R03 | resolved | explicit | AC-06, AC-07, AC-08. A tick below, at and across dawn/dusk or a stage boundary |
| E018 | empty | R03 | resolved | explicit | AC-06, AC-07, AC-08. A tick below, at and across dawn/dusk or a stage boundary |
| E019 | ordering | R03 | resolved | explicit | AC-06, AC-07, AC-08. Process each crossed transition once and retain elapsed fractions after restore. |
| E020 | precision | R03 | resolved | explicit | AC-06, AC-07, AC-08. A tick below, at and across dawn/dusk or a stage boundary |
| E021 | idempotency | R03 | resolved | explicit | AC-06, AC-07, AC-08. Process each crossed transition once and retain elapsed fractions after restore. |
| E022 | concurrency | R03 | resolved | explicit | AC-06, AC-07, AC-08. Process each crossed transition once and retain elapsed fractions after restore. |
| E023 | boundary | R04 | resolved | explicit | AC-09, AC-10. All seven variants, zero durability, eligible target ties and attack caps |
| E024 | adjacency | R04 | resolved | explicit | AC-09, AC-10. All seven variants, zero durability, eligible target ties and attack caps |
| E025 | empty | R04 | resolved | explicit | AC-09, AC-10. All seven variants, zero durability, eligible target ties and attack caps |
| E026 | ordering | R04 | resolved | backstop | AC-09, AC-10. Held-out test: permute the presentation order of equally eligible targets while retaining the same authoritative state and require stable targeting. |
| E027 | precision | R04 | resolved | explicit | AC-09, AC-10. All seven variants, zero durability, eligible target ties and attack caps |
| E028 | idempotency | R04 | resolved | explicit | AC-09, AC-10. Generate directed cases for normal and unusual legal loadouts and compare actual stats and attack ordering. |
| E029 | concurrency | R04 | resolved | explicit | AC-09, AC-10. Generate directed cases for normal and unusual legal loadouts and compare actual stats and attack ordering. |
| E030 | boundary | R05 | resolved | explicit | AC-11, AC-12. Minimum/maximum roll, empty optional capacity and mandatory equipment conflicts |
| E031 | adjacency | R05 | resolved | explicit | AC-11, AC-12. Minimum/maximum roll, empty optional capacity and mandatory equipment conflicts |
| E032 | empty | R05 | resolved | explicit | AC-11, AC-12. Minimum/maximum roll, empty optional capacity and mandatory equipment conflicts |
| E033 | ordering | R05 | resolved | explicit | AC-11, AC-12. Preserve generated state through duplicate death, snapshot restore and exact ownership transfer. |
| E034 | precision | R05 | resolved | explicit | AC-11, AC-12. Minimum/maximum roll, empty optional capacity and mandatory equipment conflicts |
| E035 | idempotency | R05 | resolved | explicit | AC-11, AC-12. Preserve generated state through duplicate death, snapshot restore and exact ownership transfer. |
| E036 | concurrency | R05 | resolved | explicit | AC-11, AC-12. Preserve generated state through duplicate death, snapshot restore and exact ownership transfer. |
| E037 | boundary | R06 | resolved | explicit | AC-13, AC-14. Last arrow, reserve refill, scarce safe restock and broken equipment ties |
| E038 | adjacency | R06 | resolved | explicit | AC-13, AC-14. Last arrow, reserve refill, scarce safe restock and broken equipment ties |
| E039 | empty | R06 | resolved | explicit | AC-13, AC-14. Last arrow, reserve refill, scarce safe restock and broken equipment ties |
| E040 | ordering | R06 | resolved | explicit | AC-13, AC-14. An interrupted or repeated attack cannot spend extra ammo, create ammo or reclaim a fired arrow. |
| E041 | precision | R06 | resolved | explicit | AC-13, AC-14. Last arrow, reserve refill, scarce safe restock and broken equipment ties |
| E042 | idempotency | R06 | resolved | explicit | AC-13, AC-14. An interrupted or repeated attack cannot spend extra ammo, create ammo or reclaim a fired arrow. |
| E043 | concurrency | R06 | resolved | explicit | AC-13, AC-14. An interrupted or repeated attack cannot spend extra ammo, create ammo or reclaim a fired arrow. |
| E044 | boundary | R07 | resolved | explicit | AC-15, AC-16. First eligible target, earlier terrain, overlaps and exact hit ties |
| E045 | adjacency | R07 | resolved | explicit | AC-15, AC-16. First eligible target, earlier terrain, overlaps and exact hit ties |
| E046 | empty | R07 | resolved | explicit | AC-15, AC-16. First eligible target, earlier terrain, overlaps and exact hit ties |
| E047 | ordering | R07 | resolved | explicit | AC-15, AC-16. Two eligible bodies or one acid splash never produce a second direct hit or incidental gameplay effect. |
| E048 | precision | R07 | resolved | explicit | AC-15, AC-16. First eligible target, earlier terrain, overlaps and exact hit ties |
| E049 | idempotency | R07 | resolved | explicit | AC-15, AC-16. Two eligible bodies or one acid splash never produce a second direct hit or incidental gameplay effect. |
| E050 | concurrency | R07 | resolved | explicit | AC-15, AC-16. Two eligible bodies or one acid splash never produce a second direct hit or incidental gameplay effect. |
| E051 | boundary | R08 | resolved | explicit | AC-17, AC-18, AC-19. First stage, exact step, last/extensible stage and malformed data |
| E052 | adjacency | R08 | resolved | explicit | AC-17, AC-18, AC-19. First stage, exact step, last/extensible stage and malformed data |
| E053 | empty | R08 | resolved | explicit | AC-17, AC-18, AC-19. First stage, exact step, last/extensible stage and malformed data |
| E054 | ordering | R08 | resolved | explicit | AC-17, AC-18, AC-19. Queues accepted before a boundary retain their stage and generation inputs while new events use the new rules. |
| E055 | precision | R08 | resolved | explicit | AC-17, AC-18, AC-19. First stage, exact step, last/extensible stage and malformed data |
| E056 | idempotency | R08 | resolved | explicit | AC-17, AC-18, AC-19. Queues accepted before a boundary retain their stage and generation inputs while new events use the new rules. |
| E057 | concurrency | R08 | resolved | backstop | AC-17, AC-18, AC-19. Held-out test: race stale guest progression data with a new accepted host event. Wave state comes from the effective host data only. |
| E058 | boundary | R09 | resolved | explicit | AC-20, AC-21, AC-22. Opening grace, exact eligible nightfall and no, one or two closest sources |
| E059 | adjacency | R09 | resolved | explicit | AC-20, AC-21, AC-22. Opening grace, exact eligible nightfall and no, one or two closest sources |
| E060 | empty | R09 | resolved | explicit | AC-20, AC-21, AC-22. Opening grace, exact eligible nightfall and no, one or two closest sources |
| E061 | ordering | R09 | resolved | explicit | AC-20, AC-21, AC-22. Replay nightfall or destroy one source during release. Check selected identities, promotion for later events and one regular wave per selected live source. |
| E062 | precision | R09 | resolved | explicit | AC-20, AC-21, AC-22. Opening grace, exact eligible nightfall and no, one or two closest sources |
| E063 | idempotency | R09 | resolved | explicit | AC-20, AC-21, AC-22. Replay nightfall or destroy one source during release. Check selected identities, promotion for later events and one regular wave per selected live source. |
| E064 | concurrency | R09 | resolved | explicit | AC-20, AC-21, AC-22. Replay nightfall or destroy one source during release. Check selected identities, promotion for later events and one regular wave per selected live source. |
| E065 | boundary | R10 | resolved | explicit | AC-23, AC-24. Damage, exact delay/cooldown boundary, any damaged source and its stage minimum |
| E066 | adjacency | R10 | resolved | explicit | AC-23, AC-24. Damage, exact delay/cooldown boundary, any damaged source and its stage minimum |
| E067 | empty | R10 | resolved | explicit | AC-23, AC-24. Damage, exact delay/cooldown boundary, any damaged source and its stage minimum |
| E068 | ordering | R10 | resolved | explicit | AC-23, AC-24. Retain one accepted hit event and no defender warning. Dead portals cancel unspawned work, while current living counts do not block a live source. |
| E069 | precision | R10 | resolved | explicit | AC-23, AC-24. Damage, exact delay/cooldown boundary, any damaged source and its stage minimum |
| E070 | idempotency | R10 | resolved | explicit | AC-23, AC-24. Retain one accepted hit event and no defender warning. Dead portals cancel unspawned work, while current living counts do not block a live source. |
| E071 | concurrency | R10 | resolved | explicit | AC-23, AC-24. Retain one accepted hit event and no defender warning. Dead portals cancel unspawned work, while current living counts do not block a live source. |
| E072 | boundary | R11 | resolved | explicit | AC-25, AC-26. Zero, one or two source sides and immediate readiness at the resolved destruction boundary. |
| E073 | adjacency | R11 | resolved | explicit | AC-25, AC-26. Compare overlapping destructions with consecutive resolved moments and free with busy sources. |
| E074 | empty | R11 | resolved | explicit | AC-25, AC-26. No living source means no commitment on that side. Final destruction makes the last site passable. |
| E075 | ordering | R11 | resolved | explicit | AC-25, AC-26. Select sources after settling destruction in that moment. Finish the current release before selecting a ready retaliation. |
| E076 | precision | R11 | resolved | explicit | AC-25, AC-26. Compare the exact destruction boundary with the prior moment and verify immediate readiness. |
| E077 | idempotency | R11 | resolved | explicit | AC-25, AC-26. Repeated destruction updates do not create a commitment or replay the destruction effect. |
| E078 | concurrency | R11 | resolved | explicit | AC-25, AC-26. Distinct same-moment destructions yield a separate commitment per event and surviving source. |
| E079 | encoding | R11 | resolved | explicit | AC-25, AC-26. Use stable portal and event identities to preserve same-moment commitments and update only known markers across peers. |
| E080 | boundary | R12 | resolved | explicit | AC-27, AC-28, AC-29. One active release per portal, exact readiness, same-priority ties and counts beyond former limits |
| E081 | adjacency | R12 | resolved | explicit | AC-27, AC-28, AC-29. One active release per portal, exact readiness, same-priority ties and counts beyond former limits |
| E082 | empty | R12 | resolved | explicit | AC-27, AC-28, AC-29. One active release per portal, exact readiness, same-priority ties and counts beyond former limits |
| E083 | ordering | R12 | resolved | explicit | AC-27, AC-28, AC-29. Apply normal pace, priority and expiry. Confirm independent sources progress and neither living enemies nor possessions disappear. |
| E084 | precision | R12 | resolved | explicit | AC-27, AC-28, AC-29. One active release per portal, exact readiness, same-priority ties and counts beyond former limits |
| E085 | idempotency | R12 | resolved | explicit | AC-27, AC-28, AC-29. Apply normal pace, priority and expiry. Confirm independent sources progress and neither living enemies nor possessions disappear. |
| E086 | concurrency | R12 | resolved | explicit | AC-27, AC-28, AC-29. Apply normal pace, priority and expiry. Confirm independent sources progress and neither living enemies nor possessions disappear. |
| E087 | boundary | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Same-side tie, opposite-side fallback, no target and coincident exit/dusk or destruction |
| E088 | adjacency | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Same-side tie, opposite-side fallback, no target and coincident exit/dusk or destruction |
| E089 | empty | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Same-side tie, opposite-side fallback, no target and coincident exit/dusk or destruction |
| E090 | ordering | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Only a living portal commits an exit. Nightfall resumes hostile behavior in place and final destruction permanently clears retreat. |
| E091 | precision | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Same-side tie, opposite-side fallback, no target and coincident exit/dusk or destruction |
| E092 | idempotency | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Only a living portal commits an exit. Nightfall resumes hostile behavior in place and final destruction permanently clears retreat. |
| E093 | concurrency | R13 | resolved | explicit | AC-30, AC-31, AC-32, AC-33, AC-34. Only a living portal commits an exit. Nightfall resumes hostile behavior in place and final destruction permanently clears retreat. |
| E094 | boundary | R14 | resolved | explicit | AC-35, AC-36. Equal side counts, travel ties, no wall and a live border change |
| E095 | adjacency | R14 | resolved | explicit | AC-35, AC-36. Equal side counts, travel ties, no wall and a live border change |
| E096 | empty | R14 | resolved | explicit | AC-35, AC-36. Equal side counts, travel ties, no wall and a live border change |
| E097 | ordering | R14 | resolved | explicit | AC-35, AC-36. Stable assignments honor in-combat/assault exclusions and move defenders under ordinary routing. |
| E098 | precision | R14 | resolved | explicit | AC-35, AC-36. Equal side counts, travel ties, no wall and a live border change |
| E099 | idempotency | R14 | resolved | backstop | AC-35, AC-36. Held-out test: repeat an already committed rebalance interval update and check for a second assignment or movement order. |
| E100 | concurrency | R14 | resolved | explicit | AC-35, AC-36. Stable assignments honor in-combat/assault exclusions and move defenders under ordinary routing. |
| E101 | boundary | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Seven vs eight, empty armed slots, overlap with other rally and no known target |
| E102 | adjacency | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Seven vs eight, empty armed slots, overlap with other rally and no known target |
| E103 | empty | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Seven vs eight, empty armed slots, overlap with other rally and no known target |
| E104 | ordering | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Exact carried cost and fixed initial roster commit once despite stale or competing requests. |
| E105 | precision | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Seven vs eight, empty armed slots, overlap with other rally and no known target |
| E106 | idempotency | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Exact carried cost and fixed initial roster commit once despite stale or competing requests. |
| E107 | concurrency | R15 | resolved | explicit | AC-37, AC-38, AC-39, AC-40, AC-41. Exact carried cost and fixed initial roster commit once despite stale or competing requests. |
| E108 | boundary | R16 | resolved | explicit | AC-42, AC-43. No returnees, still-traveling returnees, reached camp and concurrent recall/target destruction |
| E109 | adjacency | R16 | resolved | explicit | AC-42, AC-43. No returnees, still-traveling returnees, reached camp and concurrent recall/target destruction |
| E110 | empty | R16 | resolved | explicit | AC-42, AC-43. No returnees, still-traveling returnees, reached camp and concurrent recall/target destruction |
| E111 | ordering | R16 | resolved | explicit | AC-42, AC-43. Remove departed members without replacements. End the side slot once and keep actual gear and location. |
| E112 | precision | R16 | resolved | explicit | AC-42, AC-43. No returnees, still-traveling returnees, reached camp and concurrent recall/target destruction |
| E113 | idempotency | R16 | resolved | explicit | AC-42, AC-43. Remove departed members without replacements. End the side slot once and keep actual gear and location. |
| E114 | concurrency | R16 | resolved | explicit | AC-42, AC-43. Remove departed members without replacements. End the side slot once and keep actual gear and location. |
| E115 | boundary | R17 | resolved | explicit | AC-44, AC-45. Zero eligible vs one living recruit, main health remaining vs zero and ineligible bodies |
| E116 | adjacency | R17 | resolved | explicit | AC-44, AC-45. Zero eligible vs one living recruit, main health remaining vs zero and ineligible bodies |
| E117 | empty | R17 | resolved | explicit | AC-44, AC-45. Zero eligible vs one living recruit, main health remaining vs zero and ineligible bodies |
| E118 | ordering | R17 | resolved | explicit | AC-44, AC-45. Repeat death notifications and simultaneous state changes without creating a second defeat. |
| E119 | precision | R17 | resolved | explicit | AC-44, AC-45. Zero eligible vs one living recruit, main health remaining vs zero and ineligible bodies |
| E120 | idempotency | R17 | resolved | explicit | AC-44, AC-45. Repeat death notifications and simultaneous state changes without creating a second defeat. |
| E121 | concurrency | R17 | resolved | explicit | AC-44, AC-45. Repeat death notifications and simultaneous state changes without creating a second defeat. |
| E122 | boundary | R18 | resolved | explicit | AC-46, AC-47. Five vs six destroyed, zero vs several living zombies and next dawn |
| E123 | adjacency | R18 | resolved | explicit | AC-46, AC-47. Five vs six destroyed, zero vs several living zombies and next dawn |
| E124 | empty | R18 | resolved | explicit | AC-46, AC-47. Five vs six destroyed, zero vs several living zombies and next dawn |
| E125 | ordering | R18 | resolved | explicit | AC-46, AC-47. Award once and continue with existing enemies, belongings, positions and clock. |
| E126 | precision | R18 | resolved | explicit | AC-46, AC-47. Five vs six destroyed, zero vs several living zombies and next dawn |
| E127 | idempotency | R18 | resolved | explicit | AC-46, AC-47. Award once and continue with existing enemies, belongings, positions and clock. |
| E128 | concurrency | R18 | resolved | explicit | AC-46, AC-47. Award once and continue with existing enemies, belongings, positions and clock. |
| E129 | adjacency | R19 | resolved | explicit | AC-48, AC-49. Consecutive moments vs one resolved moment, reversed input order and retained prior victory |
| E130 | empty | R19 | resolved | explicit | AC-48, AC-49. Consecutive moments vs one resolved moment, reversed input order and retained prior victory |
| E131 | ordering | R19 | resolved | explicit | AC-48, AC-49. Only one new result takes effect per moment and earlier victory persists through later loss. |
| E132 | idempotency | R19 | resolved | explicit | AC-48, AC-49. Only one new result takes effect per moment and earlier victory persists through later loss. |
| E133 | concurrency | R19 | resolved | explicit | AC-48, AC-49. Only one new result takes effect per moment and earlier victory persists through later loss. |
| E134 | boundary | R19 | resolved | explicit | AC-48, AC-49. Exercise first victory and each defeat predicate in the same resolved moment, then one moment apart. |
| E135 | precision | R19 | resolved | explicit | AC-48, AC-49. Classify by the host's resolved gameplay moment, unaffected by presentation clock rounding or client arrival time. |
| E136 | boundary | R20 | resolved | explicit | AC-50, AC-51. Just inside/on/outside 24 u, overlapping discovery and no known regions |
| E137 | adjacency | R20 | resolved | explicit | AC-50, AC-51. Just inside/on/outside 24 u, overlapping discovery and no known regions |
| E138 | empty | R20 | resolved | explicit | AC-50, AC-51. Just inside/on/outside 24 u, overlapping discovery and no known regions |
| E139 | encoding | R20 | resolved | explicit | AC-50, AC-51. Just inside/on/outside 24 u, overlapping discovery and no known regions |
| E140 | ordering | R20 | resolved | explicit | AC-50, AC-51. Merge only authoritative sightings and keep identity, display names and markers distinct on all peers. |
| E141 | precision | R20 | resolved | explicit | AC-50, AC-51. Just inside/on/outside 24 u, overlapping discovery and no known regions |
| E142 | idempotency | R20 | resolved | explicit | AC-50, AC-51. Merge only authoritative sightings and keep identity, display names and markers distinct on all peers. |
| E143 | concurrency | R20 | resolved | explicit | AC-50, AC-51. Merge only authoritative sightings and keep identity, display names and markers distinct on all peers. |
| E144 | boundary | R21 | resolved | explicit | AC-52, AC-53. Empty/active panels, overlapping targets, muted warnings and expanded text |
| E145 | adjacency | R21 | resolved | explicit | AC-52, AC-53. Empty/active panels, overlapping targets, muted warnings and expanded text |
| E146 | empty | R21 | resolved | explicit | AC-52, AC-53. Empty/active panels, overlapping targets, muted warnings and expanded text |
| E147 | encoding | R21 | resolved | explicit | AC-52, AC-53. Empty/active panels, overlapping targets, muted warnings and expanded text |
| E148 | ordering | R21 | resolved | explicit | AC-52, AC-53. Keep foreground input ownership and outcome permissions, and prove rejection reasons remain visible. |
| E149 | precision | R21 | resolved | explicit | AC-52, AC-53. Empty/active panels, overlapping targets, muted warnings and expanded text |
| E150 | idempotency | R21 | resolved | explicit | AC-52, AC-53. Keep foreground input ownership and outcome permissions, and prove rejection reasons remain visible. |
| E151 | concurrency | R21 | resolved | explicit | AC-52, AC-53. Keep foreground input ownership and outcome permissions, and prove rejection reasons remain visible. |
| E152 | boundary | R22 | resolved | explicit | AC-54, AC-55. Before/after changes, elapsed fractions, empty and multiple queues and staged outcome history |
| E153 | adjacency | R22 | resolved | explicit | AC-54, AC-55. Before/after changes, elapsed fractions, empty and multiple queues and staged outcome history |
| E154 | empty | R22 | resolved | explicit | AC-54, AC-55. Before/after changes, elapsed fractions, empty and multiple queues and staged outcome history |
| E155 | ordering | R22 | resolved | explicit | AC-54, AC-55. Restore at coherent boundaries and compare live behavior with a no-restore control without replay. |
| E156 | precision | R22 | resolved | explicit | AC-54, AC-55. Before/after changes, elapsed fractions, empty and multiple queues and staged outcome history |
| E157 | idempotency | R22 | resolved | explicit | AC-54, AC-55. Restore at coherent boundaries and compare live behavior with a no-restore control without replay. |
| E158 | concurrency | R22 | resolved | explicit | AC-54, AC-55. Restore at coherent boundaries and compare live behavior with a no-restore control without replay. |
| E159 | adjacency | R23 | resolved | explicit | AC-56, AC-57. Full selection, empty evidence, prior failures and unjustified omissions |
| E160 | empty | R23 | resolved | explicit | AC-56, AC-57. Full selection, empty evidence, prior failures and unjustified omissions |
| E161 | encoding | R23 | resolved | backstop | AC-56, AC-57. Held-out test: non-ASCII labels and identical display names cannot alias independent scenario, actor or evidence IDs. |
| E162 | ordering | R23 | resolved | explicit | AC-56, AC-57. Actual invocation identity and continuous evidence distinguish performed gameplay and other proof tiers. |
| E163 | idempotency | R23 | resolved | explicit | AC-56, AC-57. Actual invocation identity and continuous evidence distinguish performed gameplay and other proof tiers. |
| E164 | concurrency | R23 | resolved | explicit | AC-56, AC-57. Actual invocation identity and continuous evidence distinguish performed gameplay and other proof tiers. |

## Prohibitions (must-NOT)

**Coverage:** 8 resolved · 0 unresolved. Each prohibition maps to a future negative acceptance check. Implementation planning supplies the executable descriptors and known-good and known-bad proof subjects.

| Prohibition | Requirement | Status | Verification / acceptance |
| --- | --- | --- | --- |
| Death must not generate replacement loot, restore spent arrows or reset gear condition. | R05 | resolved | test · AC-11, AC-12 |
| Bloater acid must not alter health, status, movement, item condition or collision. | R07 | resolved | test · AC-16 |
| Joining players must not raise zombie difficulty, and stage advancement must not inflate living enemies. | R08 | resolved | test · AC-17, AC-18 |
| Provoked defenders must not create a dedicated warning cue or player alert, including audio or HUD warnings. | R10 | resolved | test · AC-23 |
| Retaliation must not have a warning or advance delay. Portal destruction must not produce a separate local or camp-wide notice. | R11 | resolved | test · AC-25, AC-26 |
| Living population must not delay a ready release or delete already living enemies or their possessions. | R12 | resolved | test · AC-28 |
| An invalid rally order must not charge any player, and a valid purchase must not draw from the shared stash. | R15 | resolved | test · AC-38 |
| The minimap and campaign feedback must not expose unexplored portals or live enemy positions through unexplored terrain. | R20 | resolved | test · AC-26, AC-51 |

## Ambiguity Report

| Dimension | Score | Minimum | Status | Basis |
| --- | --- | --- | --- | --- |
| Goal clarity | 0.98 | 0.75 | Met | Full solo/LAN campaign, authored map, warrior orders and campaign results. |
| Boundary clarity | 0.98 | 0.70 | Met | Phase 8 gameplay, Phase 9 integration and release evidence are explicit. |
| Constraint clarity | 0.95 | 0.65 | Met | Creator chose source selection, uncapped releases, defender lead, retained stage, fixed roster and simultaneous ordering. |
| Acceptance criteria | 0.97 | 0.70 | Met | 57 pass/fail checks and 164 actionable edge outcomes. |
| **Ambiguity** | **0.028** | **≤ 0.20** | **Met** | 1 − (0.35×0.98 + 0.25×0.98 + 0.20×0.95 + 0.20×0.97). |

## Interview Log

| Round | Perspective | Question summary | Decision |
| --- | --- | --- | --- |
| 1 | Researcher | Wave overlap, warning recipients and squad return | Finish the current release before prioritizing defenders. Hold the assault slot until returns finish. No player was selected as a defender warning recipient. A dedicated world cue remains open. |
| 2 | Researcher / Simplifier | Combat readiness, cap pacing and retreat at nightfall | Allow a healthy fists-only warrior to fill an open slot. Resume capped waves at normal pace. Zombies still retreating at nightfall resume attacks and can retreat again at dawn. |
| 3 | Boundary Keeper | Accepted wave stage, roster attrition and simultaneous warnings | Use the trigger-time stage. Keep the original roster and carry on after death or takeover without replacements. Show a separate global warning per destroyed portal. |
| 4 | Failure Analyst | Unavailable defender release, injured unarmed roster and dusk/exit ordering | A locally capped, unstarted defense can yield to eligible ready work. Injured fists-only warriors qualify under the same fitness rule as armed warriors. Arrival at a living retreat portal at exactly dusk is resolved before normal nightfall reengagement. |
| 5 | Seed closer | Global/portal wave caps, closest source rule and final portal exit | Destruction of the last portal defeats a simultaneous retreat exit. Creator proposes eliminating the hard wave cap and restricting releases to closest portals. Prior cap-dependent decisions need reconciliation. Keep reviewing before authoring. |
| 6 | Source and cap reconciliation | Nearest sources, living limits and final defender presentation | Regular and retaliation use the closest surviving portal to camp on each side. Defenders use the damaged portal. Remove global and per-portal living count limits. Keep the two-second defender delay with no dedicated cue, audio or player alert. Author the specification. |

The round-six population decision superseded the earlier choices about a global spawn slot, resuming a capped wave and bypassing a defender wave at its local living cap. The later Phase 8 discussion superseded the round-three retaliation warnings and the ten-second lead time. Trigger-time stage and the release priority between ready entries continue to apply.

*Next step: `$gsd-plan-phase 8` with the approved [phase context](08-CONTEXT.md).*
