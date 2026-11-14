# Feature Research

**Domain:** KingdomZombies — complete v0.1 solo/four-player LAN survival and camp management
**Researched:** 2026-11-13
**Confidence:** LOW under the provider classifier for local/curated input (unsupported provider); this is a limitation of that classifier, not a competing scope judgment. Feature scope below is directly traceable to the creator-approved local specification. Implementation cost, ordering recommendations, and balance remain unverified planning judgments.

## Scope and Evidence

The [game specification](../../specs/0.1/initial-spec.md) is authoritative; the [resource plan](../../specs/0.1/assets-and-resources.md) supplements it, and [PROJECT.md](../PROJECT.md) records creator-approved technology, development obligations, and delivery direction. All three were read in full. This dimension needs no competitor-driven feature additions or external library research. The local inputs define what to build; they do not establish that any capability is already implemented or that the proposed balance works.

Every gameplay category below is mandatory for **the complete v0.1 release**. “Differentiator” describes the game's distinctive behavior and does not mean optional. Numeric tables and supporting rules marked proposed in the source remain the implementation/playtest baseline; preserve that status in requirements. Do not replace the selected categories, turn the foundation demos into release scope, or classify mandatory features as post-MVP work.

## Feature Landscape

### Table Stakes (Mandatory Capabilities)

Complexity is a relative implementation estimate, not evidence of feasibility or elapsed time. Split each semicolon-separated capability into independently verifiable requirements; retain the source section and acceptance IDs rather than treating a whole row as one completed feature.

| Category | Why required | Complexity | Mandatory capabilities and source |
|---|---|---|---|
| Developer rules and documentation | Creator's required Phase 1 foundation | HIGH | Godot-compatible C# rule matrix inherited selectively from LangProj; retained/adapted/inapplicable decisions; analyzer/build/architecture enforcement; positive and negative proof tests; concise root `AGENTS.md` and focused documentation indexes with consistent Windows/Linux casing. Prove compatibility with generated partials, callbacks, scene/resource serialization, node lifetime, main-thread access, nullability, SDK/compiler/analyzer versions. No blanket suppression or imported web/database architecture. [PROJECT](../PROJECT.md) |
| Visible playable foundation | Required Phase 2 deliverable | MEDIUM | Main-menu start into a small scene, a few real base assets, one controllable left/right moving character; launch/input/render smoke evidence. This remains an intermediate foundation. [PROJECT](../PROJECT.md) |
| Four-player network foundation | Required Phase 3 deliverable | HIGH | One host plus three guests in that same scene, independent control and synchronized movement; participant/controller identity, host authority, service separation and connection smoke tests. [PROJECT](../PROJECT.md) |
| Resource production | Required Phase 4 complete production pack | HIGH | Reuse suitable recovered assets/logic first, adapt to Godot/spec, generate actual gaps; preserve originals; self-contained selected outputs with stable IDs, source identity, transformations/recipes, versions, hashes and evidence levels. Cover all 180 obligations, inspectable fixtures, and later gameplay integration. [Resource plan §§3–12](../../specs/0.1/assets-and-resources.md) |
| Repeatable AI-operated verification | Required Phase 5 and ongoing delivery capability | HIGH | Repository-stored readable cases **and executable tests**; local commands matching GitHub Actions; agent instructions for case selection, execution, evidence inspection, reproduction, correction and rerun; production scene/input/render checks and actual host-plus-three-client processes; controlled setups/time/randomness, bounded waits, cleanup, logs/images/results and reliable exit codes. Missing tools, undiscovered/skipped required cases, unavailable environments and timeouts cannot pass. Register all A-01–A-47 in Phase 5; mark unimplemented cases planned with an owning phase, then activate them. [PROJECT](../PROJECT.md) |
| Run initialization and outcomes | A campaign needs a beginning, consequences and completion | HIGH | Eight total survivors for solo or four players; unique control, basic outfits, one shared starting stash, tier-1 main, dawn/day 1 and six portals; no per-player supply scaling. Random eligible succession preserving location/health/gear; remembered jobs, waiting-order spectators and camera follow; both defeat conditions; immediate final-portal victory with continued danger; simultaneous defeat precedence and retained prior victory. [Spec §2](../../specs/0.1/initial-spec.md#2-run-lifecycle-leadership-and-outcomes) |
| Authored world and time | Expansion and defense depend on a consistent world | HIGH | One handcrafted walkable map, six portals, eight wall/eight tower sites and baseline plants/clearing/wildlife zones; active portals block survivors even when jumping, allow zombies through, become permanently passable when destroyed. Configured 600/300-second day/night, first-night scheduled-wave grace, three-day escalation; exactly-once dawn events; shared exploration, independent cameras and known-only minimap; wall-based borders, fallback refuge, safe-work conditions. Active play never pauses, including solo and menus. [Spec §3](../../specs/0.1/initial-spec.md#3-world-time-exploration-and-camp-boundaries) |
| Survivor movement and recovery | Players must survive with the same real state as NPCs | HIGH | Walk/sprint/jump/air movement; health/stamina and spending gates; natural stamina recovery, safe-camp-only health recovery, food cooldown/clamping; recursive carried weight counted once; configurable load penalties, severe-load sprint/jump lockout while walking remains possible. [Spec §4](../../specs/0.1/initial-spec.md#4-survivors-movement-recovery-and-combat) |
| Combat and equipment condition | Tactical combat must match possessions | HIGH | Facing melee/fists; simultaneous dual one-handed swings with slower interval/summed stamina; manually aimed bow with arrow-only opposite hand; player manual arrow refill versus NPC/zombie real-reserve refill and fallback; safe NPC restocking; per-hit armor and durability, broken gear retained but ineffective, no repair; faction-aware collision, no friendly damage, allied projectile passthrough, enemy arrows hit defenses; first-hit-only spit and purely cosmetic bloater acid. [Spec §4](../../specs/0.1/initial-spec.md#4-survivors-movement-recovery-and-combat) |
| Common inventory and storage | Ownership is the core game promise | HIGH | All 24 item definitions; persistent instance IDs and exactly one location; 11 equipment slots, finite item-provided pockets/grids, four food-stack references; rotation, compatible stacking/splitting, nesting and cycle/depth rejection; filled containers retain contents on move/unequip/drop; private controlled-player inventories, nearby NPC access, explicit world drops. Immediate open/transfer/equip, preview-only drag, safe cancellation/lost access; unlimited paged local-access stash with filters and automatic placement. [Spec §5](../../specs/0.1/initial-spec.md#5-inventory-equipment-items-and-corpses) |
| Death, corpses and hauling | Casualties preserve actual investment and salvage | HIGH | Actual belongings remain at death with consumed arrows absent; no fresh death loot. Any non-busy ordinary role may haul under duty/safety constraints, structural guards cannot; reserve before pickup, actual capacity/load and delivery, role retention; carrier death transfers collected cargo to its own corpse once. Every nonempty corpse/pile persists indefinitely, including empty bags/broken items; empty cleanup countdown cancels on repopulation; visual-effect fading cannot delete loot. [Spec §§5.5, 7](../../specs/0.1/initial-spec.md#55-death-hauling-and-cleanup) |
| Physical economy and recruitment | Camp growth uses finite, visibly owned resources | HIGH | Physical money from the acting player's accessible carried containers only; exact payment, quote/revalidation and no charge on failure/duplicate. Separate stash-food-only sales deposit physical proceeds to stash; no global wallet. Food recovery; finite plants, once-per-dawn refill, rabbits/deer with pre-existing meat, real hunting ammo, retained wildlife and caps. Main-only purchases, basic outfit only, unassigned versus total-cap accounting, completed-tier capacity, temporary involuntary overflow without deleting survivors. [Spec §6](../../specs/0.1/initial-spec.md#6-economy-food-and-recruitment) |
| NPC roles and work | The camp must operate without micromanaging every trip | HIGH | Four assigned roles: gatherer, hunter, melee warrior, warrior archer; unassigned workers exclusively execute paid building/repair/clearing. Paid role changes, same-role no-op, waiting for missing gear; actual stash visits and safe upgrades even over manual choices; equipment preferences, food/ammo reserves, safe fallback stow or persistent drop; civilian flee-first/trapped self-defense; duty/safety priorities, saved work and interruption, at most three workers per order. [Spec §7](../../specs/0.1/initial-spec.md#7-npc-roles-equipment-work-and-army-orders) |
| Defense and assault orders | Players can expand beyond holding walls | HIGH | Automatic side allocation and fallback positions; moving side rally points; ready-warrior selection with configured mix, paid attack on next live portal, one active assault per side; reject shortages/no target/duplicate without payment. Continue through night, fight threats, return after success/obsolete target, free recall without teleport/refund/automatic next target; safe resupply and restored defense allocation. [Spec §7.4](../../specs/0.1/initial-spec.md#74-defense-and-rally-assaults--proposed-baseline) |
| Building progression and paid work | Growth changes the real defense line | HIGH | Three main/wall/tower tiers, main-tier prerequisites; clearing costs money/work but yields no wood; one order per site; unfinished frames have damageable health and no completed benefits; functional old tier during upgrade, damage-preserving completion, lost upgrade on destruction; retained inner walls; paid quantified repair and previous-completed-tier rebuilding; no free repair/cancellation/refund. Structural standalone guards 1/2/3; main tier 3 has exactly two single-archer level-1 profiles sharing main health. [Spec §8](../../specs/0.1/initial-spec.md#8-main-building-walls-towers-and-paid-work) |
| Zombie roster and generated possessions | Enemies are combatants with real loot | HIGH | Shambler, runner, brute, armored, spitter, zombie archer, acid bloater; actual equipment/stat interactions, legal guaranteed money/clothing before optional rolls, guaranteed usable armored/archer gear; bow users need physical arrow-hand stacks; optional loot only when legal; stamina, wear, load and fallback without stash access. Loot exists **before death**. [Spec §§9.1–9.2](../../specs/0.1/initial-spec.md#9-zombies-loot-portals-and-escalating-attacks) |
| Waves, portals and escalation | Campaign pressure must remain consistent across time/load | HIGH | Independently editable progression changes future count/mix/quality, never player-count scaling or retroactive living-zombie stat inflation; regular waves per live portal; grace does not suppress defense/retaliation. Damage-triggered defenders, cooldown/minimum stage/caps; one distinct retaliation event per destruction at every surviving source, additive to regular waves. Prioritized source queues and global living cap; wait without deleting enemies; regular remainder expires at dawn, defensive/retaliation commitments persist unless source destroyed. Dawn retreat carries belongings out without loot, no retreat after final portal. [Spec §9](../../specs/0.1/initial-spec.md#9-zombies-loot-portals-and-escalating-attacks) |
| Complete LAN sessions | Cooperative progress must survive membership changes | HIGH | Windows/Linux discovery list and manual address join; session metadata/capacity/compatibility, start solo and late join, four participants including spectators; complete catch-up before accepting new-player actions while others continue. Equal camp permissions, host-only save/load/close/config; stable local player identity; guest departure returns real character to remembered NPC job, reconnect only restores available live survivor; host loss ends session with clear recovery limits. Replaceable discovery/identity/connection boundary. [Spec §10](../../specs/0.1/initial-spec.md#10-multiplayer-and-session-behavior) |
| Saves and changed configuration | Complete campaigns must resume without economic resets | HIGH | Coherent manual/dawn/normal-exit saves while gameplay continues; named multiple runs, retained autosaves and selection; complete state including corpse timers, nested inventories, work/cargo, queues/warnings/randomness/daily markers, outcomes and player associations. Safe failed/interrupted writes preserve prior save; exit-error retry/play/explicit unsaved exit. Start/load-only host configuration, maximum clamps without healing, fractional time, preserved payments/work/event commitments, no deletion on lowered caps, explicit missing-content errors. [Spec §11](../../specs/0.1/initial-spec.md#11-saving-loading-and-configuration-changes) |
| Legacy inventories | Balance edits must not destroy saved belongings | HIGH | Old footprints, grids, orientation, stacks, containment and incompatible equipment remain visible/removable; no forced stash move/rejection solely for layout changes. New destinations use current rules; failure retains original position; legal whole-container extraction preserves removable internal legacy state; emptied containers adopt current limits; excess stacks extract legally; invalid equipped combinations give no attack/protection; persist unresolved layouts across saves. [Spec §5.6](../../specs/0.1/initial-spec.md#56-loading-inventories-after-configuration-changes) |
| Designer configuration | All baseline content/balance must be editable | HIGH | Complete separate documented domains DT-01–DT-18, zombie progression in its own file; stable content IDs; each field's purpose/unit/default/range/relationships; validate numbers, rosters, references, mandatory loadouts, slots, stash fit, tier/guard/map rules and presentation assets before start/load; file/field/value/reason errors, no hidden fallback constants. Local display/audio/bindings do not alter shared balance. [Spec §12](../../specs/0.1/initial-spec.md#12-designer-configuration) |
| Complete player experience | Every required action and failure must be understandable | HIGH | All required menu/HUD/people/sale/inventory/stash/loot/site/rally/map/spectator/save/outcome/settings surfaces and empty/waiting/conflict/lost-access states; panel input capture, overlap selector, safe drag/close, spectator restrictions, visible warnings over panels; remapping/reset and saved local settings; readable 1280×720/1920×1080 UI, 80–150% scale and expanded strings; complete English catalog; all equipment/actions/body fits and corpse states; effects, auditioned sound events and clean ambience loops, muted-warning parity. [Spec §13](../../specs/0.1/initial-spec.md#13-interface-controls-art-sound-and-translation) |
| Runnable, measured release | An isolated demo does not satisfy v0.1 | HIGH | Unassisted fresh solo/four-player victories; all outcomes and A-01–A-47; packaged Windows/Linux exports without developer setup; real mixed-OS LAN evidence; no known reproducible corruption/duplication/disagreement/progression blocker. Reference-PC stress with required systems visible, 100 survivors/300 zombies, 30 FPS in 95% of one-second samples over ten minutes, no >5-second sub-30 run and no >0.5-second save/join freeze; three-hour saved/resumed endurance with ≥200 nonempty corpses/piles over its course; record hardware/OS/config and tune 2–3-hour/8–12-day pacing. [Spec §14](../../specs/0.1/initial-spec.md#14-definition-of-a-finished-01-release) |

### Differentiators (Mandatory Core Behaviors)

These are product characteristics, not unsupported market-superiority claims.

| Feature | Player value | Complexity | Required consequence |
|---|---|---|---|
| Real belongings across combat, death and labor | Loot reflects what an enemy actually carried and spent | HIGH | One model for ordinary survivors/zombies; finite ammo, wear and storage; cargo and filled bags remain recoverable; visible gear follows real state. |
| Physical cash and camp logistics | Preparation and cash withdrawal matter | HIGH | Food sales feed stash inventory; carried cash funds actions; money cannot teleport from treasury or teammate. |
| Ordinary-survivor succession | The camp's remaining people determine survival | HIGH | No healing/gear transfer/teleport on control change; simultaneous deaths cannot select one survivor twice; spectators wait for eligible recruits. |
| Structural tower defense | Paid infrastructure provides a clear exception to population logistics | MEDIUM | 1/2/3 independent standalone archers, unlimited structural ammo, no inventory/loot/population/successor; main attachments are 1+1 with shared health. |
| Permanent portal progress and additive retaliation | Expansion reduces attack sources while provoking immediate danger | HIGH | Destroyed barriers never regrow; concurrent destructions produce distinct queued events; last portal awards victory immediately without clearing enemies. |
| Safe loading after designer changes | Balance iteration preserves earned possessions | HIGH | Legacy layouts stay inspectable/extractable and accepted work/spawn commitments survive current-config loading. |
| One unpaused cooperative world | Shared actions have consistent consequences | HIGH | Menus do not stop risk; authority serializes conflicting actions and returns clear rejection/corrected state; solo uses the same rules. |

### Anti-Features (Explicit Exclusions)

| Excluded feature | Why it may be tempting | Why excluded | Required alternative |
|---|---|---|---|
| More maps/islands, travel, procedural world, rival camps | Broader campaign variety | Explicit one-map contract | Author the complete six-portal map and its progression. |
| External recruitment camps, homeless encounters | Reference-game familiarity | Recruitment is main-building-only | Purchases respect fireside and total limits. |
| Online/platform accounts, matchmaking, host migration, split-screen, controllers, PvP/friendly fire | Broader multiplayer coverage | Outside specified cooperative keyboard/mouse LAN release | Implement the replaceable service boundary and full current LAN behavior. |
| Firearms/crossbows/shields/parry/rolls/mounts, classes/XP/skill trees | More combat/progression | Explicit exclusions | Finish specified melee/bow, equipment and camp progression. |
| Hunger/thirst/infection/injuries/aging/reproduction | More survival simulation | Explicit exclusions | Food immediately restores health/stamina; survival follows combat/camp rules. |
| Crafting/raw materials/rags/cooking/equipment shops or repair | Extra economic sinks/sources | Contradicts selected economy | Cash-funded work, food sales, zombie equipment, persistent broken items. |
| Recoverable arrows, damaging acid | Visual effect suggests a mechanic | Explicitly prohibited | Fired arrows are spent; acid is cosmetic. |
| Tutorial, music, voice acting, extra shipped languages | More presentation scope | Not required for v0.1 | Labels, controls reference, complete effects/ambience, English catalog and translation-ready layout. |
| In-game editor/live reload, automatic treasury/overflow disposal | Convenience | Conflicts with data/ownership contracts | Documented files applied at start/load; carried cash; removable legacy layouts. |
| Pause, player-count difficulty scaling, loot rerolls, tower-guard loot | Familiar shortcuts | Contradicts core rules | Continuous simulation, same difficulty for 1–4, generated real inventories and structural guards. |

Exclusions are not promises for the next release. No competitor-derived optional systems are added to this roadmap.

## Feature Dependencies

```text
Enforced rules + documentation + compatible toolchain (P1)
  -> visible scene/input foundation (P2)
  -> authoritative four-player movement + service/identity boundary (P3)
  -> stable resource/content contracts + complete pack/fixtures (P4)
  -> stored case catalog + local/CI/agent execution (P5)
  -> inventory/character/combat/death/succession (P6)
  -> food/recruitment/jobs/work/borders/buildings (P7)
  -> zombies/time/waves/portals/assaults/outcomes (P8)
  -> complete save/session/config/experience flows (P9)
  -> full campaign/platform/performance/endurance/pacing evidence (P10)

Stable item/content/player IDs + authoritative transactions + serializable state
  -> EVERY gameplay feature; cannot wait for P9
World reachability + time/safety/border queries -> work, recovery, hunting, hauling
Inventory + money + reservations -> jobs, equipment, sales, payment, loot, recruitment
Equipment + combat + corpses -> legal zombie generation and loot economy
Event identities + queues + random commitments -> waves, daily events, save, late join
Persistent state + separate transient effects -> reconstruction without replay
```

### Dependency Notes and Early Contracts

The first five phase positions are fixed. The last five follow the accepted direction; dependencies below identify work needed early inside those phases, not permission to reorder or remove phases.

1. **P1–P3 authority and identity:** distinguish player identity, current controlled survivor, peer connection, entity, item instance and definition IDs. Commands must validate ownership, distance, eligibility and current version at commit; duplicate or stale commands do not pay twice. Late join must catch up without rolling back existing players or replaying events. A failed operation is atomic: rejected transfers/payments preserve both inventories and balances.
2. **P4 resource/data interfaces:** lock item/equipment slot identities, body anchors, guard counts, appearance/state keys, config references and manifest shape before bulk production. Produce all 180 obligations: CH 13, AN 18, IT 24, BL 16, EN 11, FX 12, UI 27, AU 30, DT 27 and TXT 2. Shared outputs are legal; counts are obligations, not files. Catalog/metadata evidence is not final visual/audio approval. Later features still need actual integration tests.
3. **P5 full acceptance inventory:** establish A-ID → atomic requirement → owning phase → executable/readable case → evidence links. Existing foundations run now; future scenarios stay visibly planned. Headless simulation cannot prove rendering, input focus, actual mixed-OS LAN or reference-PC speed.
4. **P6 persistence-ready ownership:** represent nested graphs, old grid metadata, partial stacks/condition, inventory transactions, corpse timers and control handover as saveable state. Include round-trip fixtures before P9. Quick slots reference existing stack IDs; moving or exhausting that stack invalidates the reference rather than choosing another.
5. **P6–P7 minimum world/safety services:** combat recovery and NPC work need authoritative time, location, reachability and border/danger contracts before the full P8 campaign. Start/load validation and documented per-feature config accompany implementation. P7 needs persistent paid work quantities, accepted prices, reservations and cargo before final save UI.
6. **P8 durable event semantics:** assign origin IDs to daily transitions, defender/retaliation/regular releases, warnings and outcomes. Preserve queue progress and generated randomness; distinguish regular dawn expiration from persistent defender/retaliation queues. Resolve concurrent deaths, recruitment and final-portal/main destruction at an explicit simulation boundary. Cancellation removes pending spawns, never living actors or their possessions.
7. **P9 completes user-facing recovery:** save consistency, atomic replacement/failure UI, compatibility and legacy extraction exercise contracts already present. Reconstruct persistent appearances/loops without replaying old purchase, creation, death or destruction effects. Do not postpone concurrency correctness, compatibility data or scenario automation to this phase.

## MVP Definition — Full v0.1 Delivery Boundary

### Launch With (v0.1)

All categories in the mandatory table ship. The complete map, seven zombies, 24 baseline items, four assigned roles, unassigned workers, two wildlife species, nine building/tier appearances, resource obligations and A-01–A-47 are required together. Confirmed defaults and proposed baseline values retain their original status. Early playable/network scenes validate foundations; they are not a smaller release.

### Add After Validation

Within this release, validation activates and integrates the already-required systems as their phases arrive, then tunes the proposed baseline. There is no “later” bucket for saving, LAN recovery, legacy inventories, presentation, configuration or release evidence. Source-listed optional cosmetic variants are not added as requirements.

### Future Consideration

Only the explicitly reserved **Steam/Epic discovery, invitation, identity and connection services** are deferred platform work. Platform matchmaking, account linking and publication have no implementation commitment in v0.1. Their replaceable gameplay-independent boundary must already be demonstrated in v0.1.

## Feature Prioritization Matrix

Priority denotes release obligation; ordering denotes dependency. All selected v0.1 categories are P1.

| Feature group | User/developer value | Implementation cost | Priority / delivery focus |
|---|---|---|---|
| Rules/documentation, playable and LAN foundations | HIGH | HIGH | P1 — fixed P1–P3, with immediate smoke/proof tests |
| Complete resource production and test infrastructure | HIGH | HIGH | P1 — fixed P4–P5, integration continues later |
| Survivors/items/combat/death/control | HIGH | HIGH | P1 — P6; identity/authority/save contracts earlier |
| Camp economy/jobs/work/defenses | HIGH | HIGH | P1 — P7; world/safety/data dependencies included |
| Zombie campaign/portals/squads/outcomes | HIGH | HIGH | P1 — P8; durable events and concurrency included |
| Full save/session recovery/config/presentation | HIGH | HIGH | P1 — P9 completion; foundations throughout |
| Campaigns/platforms/stress/endurance/pacing | HIGH | HIGH | P1 — P10 release gate; cases built throughout |
| Steam/Epic service implementations | Future | Unestimated | Deferred — boundary only in current scope |

## Acceptance Coverage Guidance

Each source A-ID is an umbrella scenario, not a complete atomic requirement. Add focused failure/concurrency/boundary cases underneath it without renumbering source IDs. The phase column proposes primary behavior ownership; P5 owns case infrastructure, P9 adds recovery integration, and P10 reruns full release evidence.

| Source scenarios | Behavior that must remain observable | Primary phase(s) |
|---|---|---|
| A-01 | Same eight survivors/outfits/stash/day/portals in solo and four-player starts; no multiplied supply | P6, P8 |
| A-02, A-03 | First-night scheduled grace versus provocation; night-2 waves; stage boundaries and separate progression | P8 |
| A-04, A-05, A-06 | Actual death loot and unchanged successor; two deaths/one successor; ordered waiting spectator and single recruit purchase | P6, P7 |
| A-07, A-08, A-09 | Both independent defeats; immediate final-portal victory and remaining dangerous zombies; simultaneous defeat precedence and recorded prior victory | P8 |
| A-10, A-11, A-12, A-13 | Fireside capacity/paid completed upgrades; basic recruit outfit; all roles/waiting/unassigned-only building; safe automatic gear upgrades preserving contents | P7 |
| A-14, A-15, A-16 | Actual-ammo/broken-weapon fallback; trapped civilian self-defense then escape with cargo; night retreat/interior work/assault continuity | P6–P8 |
| A-17, A-18, A-19, A-20, A-21 | Paid clearing and expansion; all tiers and functional old tier; structural guards 1/2/3 versus main 1+1; quantified paid repair/previous-tier rebuild; no tower guard loot or population | P7 |
| A-22, A-23, A-24, A-25 | Grid/stack/slot/nesting/quick-slot identity and exact weight; severe load/broken gear; immediate unpaused UI with capture; unlimited stash delivery | P6; P9 full UX |
| A-26, A-27 | Actual remaining belongings/cargo on correct corpse; nonempty persistence, empty countdown and repopulation cancellation | P6, P7 |
| A-28, A-29, A-30 | Conflicting items/food/orders/recruitment commit once; physical stash sale proceeds and carried-cash expenses; finite plant/prey and exactly-once dawn across load | P6–P9 |
| A-31, A-32 | Projectile faction collision and finite character arrows; all seven actual-gear zombies; first-target spit, cosmetic acid, retained loot | P6, P8 |
| A-33, A-34, A-35, A-36 | Unbypassable live portal/permanent opening; daytime/grace defenders and extra retaliation; caps/queue cancellations/dawn; retreat leaves with possessions, no fresh loot | P8 |
| A-37 | Ready squad payment/target/side, night behavior, recall, success and obsolete-target return without teleport/retarget | P8 |
| A-38, A-39, A-40 | Discovery/manual mixed-OS joins and complete current snapshots; reconnect availability/no stolen control; host normal/unexpected exit and save distinction | P3 foundation; P9 complete |
| A-41, A-42, A-43, A-44 | Mid-battle full-state resume; current host values with old commitments; legacy removal/current destinations; corrupt/unknown/interrupted-save safety | P9; state contracts P3–P8 |
| A-45, A-46 | Independent successor cameras/shared explored state across join/save; remap/mute/scale/expanded text preserve usability | P6–P9 |
| A-47 | Unassisted solo/four-player campaigns, packaging, actual hardware/LAN tests, pacing and persistent-loot endurance | P10 |

**Cross-cutting regressions beyond the umbrella scenarios:** duplicate command delivery, stale quote, target leaves range/dies during drag, disconnect during transfer, simultaneous successor/recruit allocation, queued wave source destroyed, snapshot overtaken by live updates, save during transaction/dawn/wave release, pickup followed by carrier death, item insertion during corpse cleanup, lower configured cap than saved population, and failed legal extraction destination. Prove conservation at each boundary, not just matching final UI numbers.

## Competitor / Reference Feature Analysis

No new market survey is used to revise established scope. The supplied [reference observations](../../specs/0.1/initial-spec.md#15-supplied-references-and-inspected-material) and [resource evidence](../../specs/0.1/assets-and-resources.md#33-findings-that-prevent-incorrect-reuse) are the relevant comparison; those documents describe static inspection, not a runtime benchmark of Kingdom Two Crowns.

| Reference pattern | KingdomZombies interpretation | Scope protection |
|---|---|---|
| Paid upgrades, outer wall borders, job priorities | Paid worker progress, safe border fallback, retained old walls | No free repairs or material harvesting imported |
| Guard slots and building assemblies | Structural 1/2/3 standalone guards and two single-guard main attachments | No recruited guard assignment, inventory, successor or guard corpse |
| Portal/daily/network systems | Permanent portals, distinct durable waves, once-per-day events, four players | No portal regeneration, two-player cap, or inherited scaling |
| Asset catalogs, clothed body frames and fantasy enemies | Export donors with provenance; generate modular equipment and seven zombie silhouettes | Catalog “inventory” is not gameplay inventory; one sample is not a finished pack |
| Recovered animation callbacks and fading | Approved visual/audio events and persistent equipped corpses | No excluded rewards, duplicated work/hits or loot deletion |

## Gaps and Interpretation Notes

- No scope conflict was found between PROJECT, game specification and resource plan. Template language calling differentiators optional or recommending MVP cuts is inapplicable to the creator's full-v0.1 instruction.
- A-08's result wording says “survivors do not retreat” after the last portal. Sections 2.3 and 9.4 clearly establish that **remaining zombies** cease dawn retreat; acceptance prose should clarify this noun without changing the mechanic.
- Actual balance, resource completeness, toolchain compatibility, reference hardware and performance are unproven. Keep their verification explicit; do not treat this feature map as implementation evidence.
- Provider confidence was obtained with `query classify-confidence --provider curated --verified`, which returns LOW because the classifier recognizes web/document providers rather than local specification authority. No unsupported external factual claim relies on that result.

## Sources

- [Creator-approved project definition and fixed phase direction](../PROJECT.md), updated 2026-11-13.
- [Complete v0.1 game specification](../../specs/0.1/initial-spec.md), updated 2026-11-13; §§1–16, including all A-01–A-47.
- [Asset and resource production plan](../../specs/0.1/assets-and-resources.md), dated 2026-11-13; 180 output obligations, resource evidence and completion crosswalk.
- [Project configuration](../config.json), including research workflow flags and empty agent-skill mapping.

---
*Feature research for: KingdomZombies complete v0.1*
*Researched: 2026-11-13*
