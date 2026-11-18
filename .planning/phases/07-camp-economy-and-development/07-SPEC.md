# Phase 7: Camp economy and development — Specification

**Created:** 2026-11-18 (Europe/Paris)
**Interview:** 2026-11-17–18
**Ambiguity score:** 0.026 (gate: ≤ 0.20)
**Requirements:** 25 locked
**Owned project requirements:** 22; original IDs, phase ownership and Pending status retained

## Goal

**Implementation-decision reconciliation, 2026-11-18 (Europe/Paris):** The later [Phase 7 context](07-CONTEXT.md#implementation-decisions) supplies D-01–D-18 within the locked requirements below. All 25 requirement statements, 22 global requirement owners/statuses, 48 acceptance criteria and 168 explicit edge resolutions remain unchanged. Apply these decisions to R24, AC-40/AC-41 and their related transaction/feedback checks:

- D-01–D-05: One almost-full-screen, semi-transparent camp window has Stash, People, Food sales and Main building tabs and always opens on Stash. Only Stash shows the Phase 6 equipment/carried/shared-storage workspace. All tabs capture movement, attack, quick-food and world-click input while simulation/warnings continue; Escape handles the active panel first. Leaving Stash closes bag windows, remembers session positions and cancels unfinished interactions under ordinary inventory rules; returning leaves bags closed. Nonlethal damage keeps the window open with visible cues; death or main-building access loss closes it. This supersedes the earlier separate Open stash navigation sample.
- D-06–D-09: People uses a scrollable roster grouped by current job with headings/counts and current activity/equipment waiting. Selected status, role choices, visible prices and gear requirements remain alongside the roster. Away or controlled targets have disabled choices with specific live reasons. A fixed header keeps Buy recruit, carried-money price and both population limits visible with the blocking reason.
- D-10–D-14: Food quantity supports exact whole-number entry, +/− and Max. Show current stock, selection, unit price, proceeds and resulting stash money before Sell. Sell submits that visible quote directly, with no confirmation dialog; all other paid actions likewise use their explicitly priced buttons. Before a click, clamp the selection to current stock and refresh the quote; zero stock disables sale. After a clicked quote becomes stale, the host rejects without changing food or money, displays current values and a reason, and requires a new click. Never silently execute a revised quantity or price. These choices supersede the inspection confirmation and noneditable quantity examples.
- D-15–D-18: Nearby paid sites show compact work/waiting feedback; the site panel retains full progress and specific waiting reasons. Mark paid repair explicitly and display current health/full health, which may go down with further damage and up with worker restoration. A nearby civilian briefly signals danger/nightfall interruption before retreating, while People retains the activity/reason. This supersedes interpreting repair feedback as a permanently increasing completed-work bar.

Solo and LAN players can sustain and expand one working camp through real carried-money payments, recruitment, autonomous NPC jobs, finite food and wildlife, physical hauling, safe territorial work and three-tier development with full-health completion and one-payment repairs to full health.

## Background

The codebase was scouted on 2026-11-17 and its planning/implementation state reconciled on 2026-11-18. Phases 1–3 provide enforced Godot/C# boundaries, a playable movement scene and a four-participant authoritative LAN foundation. [FoundationSession](../../../src/core/FoundationSession.cs) owns participant identities, held movement intent and elapsed movement; it does not implement production camp economy or autonomous jobs. Existing state/snapshot proofs supply patterns rather than game-feature acceptance.

Phase 4 remains the active implementation phase. Its produced/partial content includes item and time definitions, equipped characters, work/animal presentations, main-building tiers and inspection-only camp/people/sale/site layouts. Those resources and fixture checks do not establish production item ownership, camp AI, payment or building gameplay. Phase 6 has an approved survivor/inventory/combat specification and context; its implementation is a prerequisite for Phase 7, not an already delivered dependency.

The missing deliverable is the integrated working-camp increment: autonomous people and resources, physical transactions and cargo, paid site work and completed structural benefits under live borders, danger and day/night changes. The creator's final repair simplification replaces interim extra-charge ideas: one flat fee buys work to full health, including damage during the active order, and closes at full health. Building/rebuild/upgrade completion restores full health without extra money or time.

Scope authority: [game specification](../../../specs/0.1/initial-spec.md), [approved requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md#phase-7-camp-economy-and-development), [project definition](../../PROJECT.md), [current state](../../STATE.md), [Phase 6 specification](../06-survivors-inventory-and-combat/06-SPEC.md), [architecture](../../../docs/architecture/README.md) and [local testing strategy](../../../docs/workflow/local-testing.md). R01, R24 and R25 express delivery obligations; R02–R23 map the 22 owned project IDs without creating new global requirements or transferring later-phase acceptance.

## Requirements

1. **R01 — Authoritative day/night and daily resource events:** Authoritative day/night transitions apply each plant refill and wildlife batch once per day, preserving fractional phase progress and processed-event identities across snapshot restoration.
   - Source: Phase 7 delivery prerequisites; ECON-03, ECON-05 integration.
   - Current: The DT-02 time configuration lists day/night durations and dawn events, but FoundationSession only advances movement elapsed time; production day/night and resource-event execution are absent.
   - Target: Authoritative day/night transitions apply each plant refill and wildlife batch once per day, preserving fractional phase progress and processed-event identities across snapshot restoration.
   - Acceptance: AC-01, AC-02, AC-43, AC-45.

2. **R02 — Live camp borders and reachable territory:** Each side uses its outermost completed living wall as its camp border, falls back through inner walls to the 12 u main refuge, and respects current danger and active portal barriers.
   - Source: WORLD-05.
   - Current: Phase 6 specifies reusable safety/reachability fixtures; current production FoundationSession has bounded movement rather than living walls, camp borders or portal barriers.
   - Target: Each side uses its outermost completed living wall as its camp border, falls back through inner walls to the 12 u main refuge, and respects current danger and active portal barriers.
   - Acceptance: AC-03, AC-04, AC-07, AC-45.

3. **R03 — Physical payments:** Every accepted paid camp action spends the exact integer price from accessible money stacks carried by the acting player, with no charge or commitment for invalid or repeated requests.
   - Source: ECON-01.
   - Current: Physical money is defined in the partial item data; no production camp-payment transaction or accepted work commitment exists.
   - Target: Every accepted paid camp action spends the exact integer price from accessible money stacks carried by the acting player, with no charge or commitment for invalid or repeated requests.
   - Acceptance: AC-05, AC-07, AC-45.

4. **R04 — Food sales:** A main-building food sale removes only the selected actual shared-stash food quantity and deposits its exact physical-money proceeds into the stash in one authoritative transaction.
   - Source: ECON-02.
   - Current: There is no production food-sale interface or transaction; shared physical stash behavior is a Phase 6 dependency, not an accepted current feature.
   - Target: A main-building food sale removes only the selected actual shared-stash food quantity and deposits its exact physical-money proceeds into the stash in one authoritative transaction.
   - Acceptance: AC-06, AC-07, AC-45.

5. **R05 — Finite gathering:** Gatherers spend the configured work time to move finite plant stock into their real carried inventories and deliver it to the stash; each dawn refills rather than adds stock.
   - Source: ECON-03.
   - Current: There are gathering animations and item definitions, but no production finite plant stock, gatherer job or resource delivery.
   - Target: Gatherers spend the configured work time to move finite plant stock into their real carried inventories and deliver it to the stash; each dawn refills rather than adds stock.
   - Acceptance: AC-02, AC-08, AC-45.

6. **R06 — Hunting and existing meat:** Hunters use actual bow ammunition to hunt reachable rabbits and deer within their configured exterior range and recover only the meat already owned by each animal.
   - Source: ECON-04.
   - Current: Rabbit/deer presentation and bow content are being produced in Phase 4; they do not implement autonomous hunting, real ammunition use or gameplay meat ownership.
   - Target: Hunters use actual bow ammunition to hunt reachable rabbits and deer within their configured exterior range and recover only the meat already owned by each animal.
   - Acceptance: AC-04, AC-09, AC-18, AC-20, AC-26, AC-45.

7. **R07 — Persistent daily wildlife:** Each authored wildlife zone receives at most its configured daily batch within the living cap, retaining earlier animals and the processed identity of skipped or partially admitted batches.
   - Source: ECON-05.
   - Current: Wildlife presentation fixtures are separate from production animal identities, cap accounting and once-only daily spawning, which do not exist yet.
   - Target: Each authored wildlife zone receives at most its configured daily batch within the living cap, retaining earlier animals and the processed identity of skipped or partially admitted batches.
   - Acceptance: AC-02, AC-10, AC-45.

8. **R08 — Main-building recruits:** An eligible purchase at the main building creates exactly one ordinary unassigned recruit with only the basic outfit, without replenishing the starting stash or creating extra supplies.
   - Source: NPC-01.
   - Current: Foundation admission creates temporary movement actors per participant; it does not implement paid main-building recruitment into the ordinary survivor population.
   - Target: An eligible purchase at the main building creates exactly one ordinary unassigned recruit with only the basic outfit, without replenishing the starting stash or creating extra supplies.
   - Acceptance: AC-05, AC-07, AC-11, AC-45.

9. **R09 — Fireside and total population limits:** Recruitment and voluntary unassignment obey completed-tier fireside capacity and the total survivor cap while involuntary control transitions preserve overflowing survivors and belongings.
   - Source: NPC-02.
   - Current: Partial main-building presentation exposes 8/12/16 seats; production fireside counting, population limits and overflow transitions are absent.
   - Target: Recruitment and voluntary unassignment obey completed-tier fireside capacity and the total survivor cap while involuntary control transitions preserve overflowing survivors and belongings.
   - Acceptance: AC-07, AC-12, AC-13, AC-45.

10. **R10 — Roles and basic warrior defense:** Living uncontrolled survivors at the main building can take a paid role, with same-role requests free, missing gear visibly waiting, cargo deposited before new duties, and warriors guarding the nearest completed wall or refuge.
   - Source: NPC-03; interview rounds 1–3.
   - Current: No production role assignments, job fees, equipment waiting or warrior guard behavior exist; Phase 6 specifies survivor control and remembered NPC roles.
   - Target: Living uncontrolled survivors at the main building can take a paid role, with same-role requests free, missing gear visibly waiting, cargo deposited before new duties, and warriors guarding the nearest completed wall or refuge.
   - Acceptance: AC-14, AC-15, AC-16, AC-07, AC-45.

11. **R11 — Real equipment upgrades:** Safe NPC stash visits acquire and upgrade suitable real equipment according to configured role preferences while retaining replaced items, condition and every nested container descendant.
   - Source: NPC-04.
   - Current: Phase 4 provides equipment appearances and item data; no production stash-visiting NPC equipment upgrade decisions exist.
   - Target: Safe NPC stash visits acquire and upgrade suitable real equipment according to configured role preferences while retaining replaced items, condition and every nested container descendant.
   - Acceptance: AC-17, AC-18, AC-45.

12. **R12 — Food, ammunition and unusable gear:** NPCs consume and restock actual food and arrows within configured cooldowns, capacity and reserves, stowing or persistently dropping unusable equipment before combat fallback without deleting belongings.
   - Source: NPC-05.
   - Current: Phase 6 owns shared food/ammunition/item contracts; autonomous NPC reserves, eating, restocking and safe fallback are not implemented.
   - Target: NPCs consume and restock actual food and arrows within configured cooldowns, capacity and reserves, stowing or persistently dropping unusable equipment before combat fallback without deleting belongings.
   - Acceptance: AC-18, AC-19, AC-20, AC-45.

13. **R13 — Civilian safety and exterior work:** All civilian jobs may work outside camp in daylight only when routes and work locations are reachable and free of nearby zombies; danger triggers escape with cargo, blocked escape permits self-defense, and nightfall stops exterior work.
   - Source: NPC-06; interview round 1.
   - Current: Current production actors do not autonomously work, detect job danger, flee with cargo or suspend exterior work at night.
   - Target: All civilian jobs may work outside camp in daylight only when routes and work locations are reachable and free of nearby zombies; danger triggers escape with cargo, blocked escape permits self-defense, and nightfall stops exterior work.
   - Acceptance: AC-04, AC-21, AC-22, AC-45.

14. **R14 — Autonomous unassigned workers:** Only unassigned workers perform shared paid construction, repairs and clearing, choosing reachable tasks by configured priority with at most three contributors and preserving worker-second progress through interruptions.
   - Source: NPC-07; interview round 3.
   - Current: Work animations are presentation assets; there is no production shared task queue, unassigned-worker dispatch or worker-second contribution.
   - Target: Only unassigned workers perform shared paid construction, repairs and clearing, choosing reachable tasks by configured priority with at most three contributors and preserving worker-second progress through interruptions.
   - Acceptance: AC-23, AC-24, AC-45.

15. **R15 — Reserved physical hauling:** Eligible non-busy ordinary NPCs reserve, pick up and deliver only actual loot that fits their real capacity, retain their role and personal reserves, and leave collected cargo on their own corpse if killed.
   - Source: NPC-08.
   - Current: Corpse/cargo presentation fixtures do not supply production hauling reservations, pickups, delivery or carrier-death ownership.
   - Target: Eligible non-busy ordinary NPCs reserve, pick up and deliver only actual loot that fits their real capacity, retain their role and personal reserves, and leave collected cargo on their own corpse if killed.
   - Acceptance: AC-25, AC-26, AC-18, AC-45.

16. **R16 — Paid clearing and site access:** Paid clearing consumes configured worker-seconds to unlock the corresponding reachable authored building sites without producing wood or removing food plants.
   - Source: BUILD-01.
   - Current: No production paid clearing orders or resulting site-unlock state exists.
   - Target: Paid clearing consumes configured worker-seconds to unlock the corresponding reachable authored building sites without producing wood or removing food plants.
   - Acceptance: AC-27, AC-29, AC-45.

17. **R17 — Three-tier development:** Main buildings, walls and standalone towers implement all three configured tiers, with completed-main-tier prerequisites, completed-tier fireside limits and unlimited stash capacity.
   - Source: BUILD-02.
   - Current: Partial main-building art/presentation demonstrates three silhouettes and completed-tier benefits; production construction prerequisites and gameplay progression are absent.
   - Target: Main buildings, walls and standalone towers implement all three configured tiers, with completed-main-tier prerequisites, completed-tier fireside limits and unlimited stash capacity.
   - Acceptance: AC-12, AC-28, AC-31, AC-45.

18. **R18 — Committed orders and unfinished frames:** A site accepts one noncancelable paid order at a time, retains actual work through interruptions and snapshots, and grants no completed border, guard or capacity benefit while its new or rebuilt frame remains unfinished.
   - Source: BUILD-03.
   - Current: There is no production paid site-order state, interruptible progress or damageable unfinished construction frame.
   - Target: A site accepts one noncancelable paid order at a time, retains actual work through interruptions and snapshots, and grants no completed border, guard or capacity benefit while its new or rebuilt frame remains unfinished.
   - Acceptance: AC-24, AC-29, AC-30, AC-32, AC-45.

19. **R19 — Functional upgrades and full-health completion:** An upgrading structure keeps its old completed tier functional until construction, rebuilding or upgrade completion instantly restores full destination-tier health at the original price and work quantity; destruction forfeits the upgrade and retains the last completed defense tier.
   - Source: BUILD-04; creator amendment in interview round 4.
   - Current: The original source required damage-preserving completion; the creator now explicitly requires free full-health restoration at build/rebuild/upgrade completion. Production upgrades do not yet exist.
   - Target: An upgrading structure keeps its old completed tier functional until construction, rebuilding or upgrade completion instantly restores full destination-tier health at the original price and work quantity; destruction forfeits the upgrade and retains the last completed defense tier.
   - Acceptance: AC-31, AC-32, AC-45.

20. **R20 — One flat payment repairs to full health:** One accepted fixed repair fee of 50% of the current tier's full build valuation, independent of missing health, keeps workers repairing to full health including further damage during the active order, without extra charges; the order closes at full health and later damage requires a new purchase.
   - Source: BUILD-05; creator round-6 amendment superseding round-4 and round-5 billing rules.
   - Current: The original source described bounded repair work. The final creator decision instead requires one flat fee for repairing to full health; interim automatic extra-charge ideas are superseded, and no production repair system exists.
   - Target: One accepted fixed repair fee of 50% of the current tier's full build valuation, independent of missing health, keeps workers repairing to full health including further damage during the active order, without extra charges; the order closes at full health and later damage requires a new purchase.
   - Acceptance: AC-33, AC-34, AC-35, AC-45.

21. **R21 — Last-tier rebuilding:** Destroyed defenses rebuild directly at their last completed tier for the specified price and worker-seconds, preserving useful inner walls and granting completed benefits only after rebuilding finishes.
   - Source: BUILD-06.
   - Current: No production destroyed-defense/last-completed-tier rebuilding system exists.
   - Target: Destroyed defenses rebuild directly at their last completed tier for the specified price and worker-seconds, preserving useful inner walls and granting completed benefits only after rebuilding finishes.
   - Acceptance: AC-03, AC-30, AC-36, AC-45.

22. **R22 — Structural tower guards:** Completed standalone towers provide exactly one, two or three independent structural archers by tier using intrinsic arrows, without survivor population, inventory, transferable equipment, corpse or loot.
   - Source: BUILD-07.
   - Current: Bow and structural-guard presentation work is partial asset evidence; production tower targeting, damage and structural lifecycle are absent.
   - Target: Completed standalone towers provide exactly one, two or three independent structural archers by tier using intrinsic arrows, without survivor population, inventory, transferable equipment, corpse or loot.
   - Acceptance: AC-37, AC-38, AC-45.

23. **R23 — Main-building attachments:** The completed tier-three main building supplies exactly two independent single-archer level-one attack profiles sharing its health, without separate repair, destruction, rebuilding or ordinary-survivor belongings.
   - Source: BUILD-08.
   - Current: The main-building presenter proves the intended two-child visual composition, not integrated production attacks, shared health or destruction.
   - Target: The completed tier-three main building supplies exactly two independent single-archer level-one attack profiles sharing its health, without separate repair, destruction, rebuilding or ordinary-survivor belongings.
   - Acceptance: AC-39, AC-32, AC-45.

24. **R24 — Usable camp interfaces and feedback:** Production people, role, food-sale and building interfaces display current quantities, eligibility, prices, activity, waiting reasons and progress while capturing panel input and keeping gameplay and warnings active.
   - Source: Phase 7 feature UI obligation; game specification §13.1.
   - Current: The playable foundation has menu/HUD and LAN controls. Phase 4 now includes inspection-only camp/people/sale/site UI layouts; these are presentation fixtures rather than integrated production camp interfaces.
   - Target: Production people, role, food-sale and building interfaces display current quantities, eligibility, prices, activity, waiting reasons and progress while capturing panel input and keeping gameplay and warnings active.
   - Acceptance: AC-40, AC-41, AC-46, AC-45.

25. **R25 — Authority, snapshots, configuration and proof:** All new camp state uses stable identities, host-authoritative atomic commitments, validated configurable values and coherent snapshot restoration, with saved repeatable scenarios and honest solo/four-player evidence for implemented behavior.
   - Source: Roadmap feature-delivery obligations; source specification §§10–12; local testing strategy.
   - Current: Existing foundation identity/command validation and detached snapshot examples provide patterns, not the required camp-state implementation, configuration coverage or feature evidence.
   - Target: All new camp state uses stable identities, host-authoritative atomic commitments, validated configurable values and coherent snapshot restoration, with saved repeatable scenarios and honest solo/four-player evidence for implemented behavior.
   - Acceptance: AC-42, AC-43, AC-44, AC-45, AC-46, AC-47, AC-48.

## Boundaries

**In scope:**

- A playable working camp area on both sides, authored work/food/wildlife areas and representative active/destroyed portal barriers.
- Authoritative day/night transitions, finite resource stock, once-only daily resource identities and live camp-border/refuge fallback before dependent jobs.
- All 22 Phase 7 owned requirements: WORLD-05, ECON-01–ECON-05, NPC-01–NPC-08 and BUILD-01–BUILD-08.
- Physical payments and food sales; recruitment/limits; autonomous roles, equipment, food/ammunition, safety, hauling and shared paid work.
- Three-tier main/wall/tower development, damageable unfinished frames, full-health construction/rebuild/upgrade completion and one flat-fee repair order to full health.
- Basic nearest-wall/refuge warrior guarding and structural tower attacks using the shared production combat/item contracts.
- Usable people/role/sale/structure interfaces, produced art/audio feedback, feature configuration, coherent snapshots and repeatable scenario definitions.
- Production solo/host/guest concurrency and interruption evidence for the implemented camp behaviors.

**Out of scope:**

- Complete authored six-portal map, shared exploration/minimap, seven-archetype campaign AI, scheduled waves, defenders/retaliation, final portal destruction and complete victory/defeat flow — Phase 8 owns those integrations and requirements.
- Full warrior side allocation, rally assaults, recall and campaign return/rebalancing — Phase 8; Phase 7 supplies basic local guarding only.
- Per-NPC task/target commands — this phase uses roles and shared paid work orders with automatic selection.
- Full manual/dawn/exit save UI, real load/reconnect/late-join orchestration and complete changed-configuration compatibility — Phase 9; feature snapshots and commitments are required now.
- Stash/inventory filtering — Phase 12/INV-14; pages, access and transfers remain earlier obligations.
- Hunger, cooking, crafting, wood/material harvesting, equipment shops/repair, wages, ordinary-survivor tower guards and recoverable fired arrows — excluded by the approved game scope.
- Automatic extra repair charges, per-hit/cumulative repair billing, payment takeovers and permanent maintenance orders — explicitly superseded by the final one-payment model.
- New engine/framework/service architecture, runtime donor-folder dependencies, or playable-content acceptance inferred from asset previews — existing boundaries and separate evidence remain binding.
- Release balancing certification, physical mixed-OS LAN, reference-PC stress and real-duration endurance — retain their hosted/release owners; no Phase 7 evidence substitutes for them.

## Constraints

- One design distance unit (u) is approximately a standing survivor's body width. The 12 u civilian safety cutoff is a distance, not a timer: a zombie at or within it interrupts civilian work and triggers escape.
- Godot/C#, the enforced five-project boundaries, pure rules, one host writer, explicit elapsed time/randomness and stable ordinal identities remain required. No implementation architecture, library or UI layout is selected here.
- Phase 6 is an implementation prerequisite. Reuse its real inventory, combat, safety/access, control/Respawn and snapshot contracts rather than treating planned behavior as already delivered.
- Working numerical baselines remain configurable, not proven balance. Canonical game-spec §§3, 5–8 and 13 supply all roster, footprint, quantity, appearance, health, work, price, reserve and timing values not restated here.
- Repair full build valuations include the main's initial 100 valuation and prior tier investments: main 100/300/800, walls 20/80/220, towers 40/130/310. The flat fee is max(1, ceil(0.5 × valuation)); it does not multiply by missing-health fraction. Construction/rebuild/upgrade completion restores full destination health without added price or time.
- Money/counts/capacities are integral. Health, time, distance and work may retain fractions; invalid/nonfinite/overflowing input rejects before mutation. Tolerances cannot move an eligibility boundary or discard items.
- Main-building player interaction retains the configured 4 u baseline. Role changes additionally require actual target presence in the authored main-building/fireside return area; a remote request does not summon or teleport the NPC.
- A camp border is inclusive; a zombie at exactly 12 u is unsafe. Safe daytime exterior job permission is distinct from safe-camp natural health recovery. Equally eligible choices use the stated priority/distance followed by stable identity.
- Use the produced self-contained assets and content definitions; asset fixture results are not gameplay, final appearance, listening or campaign acceptance.
- Current worker rates and future purchase prices may change through the specified new-run/load configuration path. Accepted payments and construction work survive; an already-paid repair remains a full-health target without another fee. No live reload.
- Follow the phase branch/PR rules when implementation starts: branch from current origin/dev, draft PR to dev at the first implementation push, same PR ready only after required checks. This specification work does not begin implementation, switch the active phase branch, advance Phase 4, or authorize merge.
- Finish whole-phase implementation and code review/fixes before game windows. Routine rendered tests use the development OS at 1280×720 without a VM, with Critical/new/changed/affected selection, grouped repair checks and the final complete selected-set pass. Hosted Windows/Linux and release evidence remain distinct.

## Acceptance Criteria

These are required future pass/fail checks, not claims that gameplay has been implemented or tested.

- [ ] **AC-01:** Default authoritative time advances through 600 s daylight and 300 s night. Just before, at and after a boundary, the new phase and plant/wildlife events occur once in source order; menus do not pause time. Equivalent elapsed intervals preserve phase and fractional progress within a declared tolerance.

- [ ] **AC-02:** Each plant refill and wildlife-zone/day batch has a stable processed identity. Snapshot restore before, during or after processing neither replays completed events nor loses outstanding ones; skipped cap-limited additions stay processed, and loading alone is not dawn.

- [ ] **AC-03:** On each side the outermost completed wall with positive health defines the inclusive border. Destroying it falls back through surviving inner walls, then to the 12 u refuge. Towers, unfinished frames and destroyed walls contribute no border; co-located identities remain distinct.

- [ ] **AC-04:** Every civilian job may perform exterior work in daylight only with a reachable, portal-clear route and location having no zombie at or within 12 u. Active portals cannot be bypassed by jumping. Exterior permission does not grant outside-camp natural health recovery.

- [ ] **AC-05:** Paid actions combine only accessible money carried by the acting controlled survivor, spend exact integer prices and leave change in real stacks. Exact funds succeed; insufficient funds, invalid quantities, lost range/access, stale target state or duplicate commands cause no extra charge or commitment. NPC, corpse, other-player and stash money cannot cover a shortfall automatically.

- [ ] **AC-06:** At the main building, sale accepts an integer quantity from one through actual available stash stock, for berries/fruit/meat at baseline 1/2/4 money each. It removes only that food and deposits exact physical proceeds in the stash, adding pages as needed. Carried food and nonfood are ineligible; proceeds do not enter the seller's inventory.

- [ ] **AC-07:** Concurrent purchases, last-slot recruitment, role changes, food sales/withdrawals, equipment pickups and paid orders commit in one host-authoritative order. Only successful effects charge or transfer. Duplicate, stale and losing actions preserve state and receive corrected contents/reasons; all participants converge.

- [ ] **AC-08:** Each gatherer spends 3 seconds of its own work per actual collected item, consumes existing stock only on successful collection and respects real capacity. Berry/fruit plants hold at most 8/6 baseline items; dawn refills to those limits, never adds that amount to leftovers. Empty stock produces no food and interrupted work cannot duplicate a collection.

- [ ] **AC-09:** Hunters use usable bows, real hand/reserve arrows and common combat costs to hunt reachable wildlife at or within 60 u beyond the live border, without crossing an active portal. Rabbits/deer use baseline 15/40 health, 4/4.5 u/s flee speeds and 8 u character detection. Their existing 2/5 meat transfers to their corpses; neither death nor pickup rerolls meat.

- [ ] **AC-10:** Each authored zone admits at most 2 new rabbits and 1 deer per dawn, subject to the total living cap of 24. At the cap, additions are skipped without deleting older animals; partial batches respect remaining places, and skipped additions are not retried later that day. Wildlife cannot be recruited, controlled or used for succession.

- [ ] **AC-11:** A successful local main-building purchase spends 12 baseline money and creates one distinct unassigned survivor with shirt, trousers and boots only. No money, food, weapon, arrows, armor, backpack or starting-stash refill is created. Eligible queued Respawn requests may take the recruit once in request order through the Phase 6 control path.

- [ ] **AC-12:** Completed main tiers provide fireside limits 8/12/16. Count all unassigned NPCs, including working builders/haulers, but exclude assigned waiting NPCs and controlled survivors. The total cap of 100 includes controlled survivors, excludes structural guards, and blocks further purchases at the limit without charging. Incomplete upgrades unlock no extra places.

- [ ] **AC-13:** Involuntary control return may temporarily exceed fireside capacity without deleting survivors/belongings or forcing roles. Purchases and voluntary unassignment remain blocked until legal; assigning an eligible NPC a different role may free a place. Lower configured caps preserve existing survivors and block new admissions as appropriate.

- [ ] **AC-14:** Role changes require the player at the main building and the living uncontrolled target actually returned to its authored main-building/fireside area. Remote or controlled targets reject without charge. Default role prices are unassigned 5, gatherer 8 and hunter/melee-warrior/warrior-archer 15; selecting the current role has no effect or fee.

- [ ] **AC-15:** After a legal role change, collected cargo and obsolete role equipment reach the stash before new duties, preserving personal equipment, applicable reserves, condition and nested contents. Missing essentials leave the new role assigned with visible Waiting for equipment; no substitute equipment is created.

- [ ] **AC-16:** Phase 7 warriors automatically guard the nearest completed living wall, or the main refuge if none is available, engage local eligible threats with real equipment and return to guard duty. Destruction updates their destination without teleportation. Full side allocation, rally assaults, recall and portal targeting are not delivered here.

- [ ] **AC-17:** NPC equipment changes require a safe physical stash visit; upgrade decisions occur no more often than once per 20 s per NPC. Use the configured melee ranking metal bat/machete/club/knife, bows for ranged roles, better protection/storage below severe load and better condition for equivalent items. Manual choices create no equipment lock.

- [ ] **AC-18:** NPC withdrawals/upgrades obey the Phase 6 single-ownership, fit, rotation, stacking, nesting and load rules. Replaced/broken gear returns to the stash or remains in a persistent legal location; moving storage preserves every descendant. A full inventory never deletes gear or creates invisible capacity.

- [ ] **AC-19:** NPCs keep up to 3 actual food items within capacity, eat when health is at or below 60% or stamina at or below 30%, and share a 6 s personal NPC eating cooldown. Recovery uses the common food values and clamps to maxima. Restocking is safe/local, consumption removes real items and lack of food causes no starvation.

- [ ] **AC-20:** Hunters/warrior archers carry up to 30/60 real arrows including the hand stack, within capacity. Automatic refill transfers only carried reserves. Empty ammunition/broken gear uses legal carried fallback or fists, stowing gear or dropping it into persistent loot before freeing a hand. Restocking waits for 5 s without a hostile target or enemy within 12 u, then requires a stash visit.

- [ ] **AC-21:** Civilian danger interrupts work and releases unused reservations while retaining collected cargo and completed progress. Civilians flee through reachable routes, fight only while escape is blocked and resume escape when it opens. They remain damageable and obey load; retreat never teleports, deletes cargo or grants invulnerability.

- [ ] **AC-22:** At nightfall exterior civilian work stops and NPCs retreat toward safe interior positions; safe interior tasks may continue. Breached interior territory remains unsafe when a zombie is at or within 12 u. Warriors retain combat duties, including when interrupted during optional hauling.

- [ ] **AC-23:** Only unassigned NPCs perform paid construction/repair/clearing. Priority is main repair, outer-wall repair, other repair, construction/upgrade, clearing, corpse hauling, idle; choose nearest reachable eligible tasks within priority. Safety, self-defense, necessary food and essential restocking override work. At most 3 workers contribute to one task; absent workers contribute zero.

- [ ] **AC-24:** Interruption, worker death, reassignment or replacement preserves the same paid order and actual progress. Another eligible builder resumes without repurchase or duplication. Construction/clearing progress uses actual configured worker-seconds; repair restores actual missing health at 10 health/s per worker. Snapshot/view updates do not advance work.

- [ ] **AC-25:** Idle eligible ordinary NPCs of any role may haul after primary duties, retaining that role; structural guards never haul. Reserve before pickup without transferring ownership. Respect actual grid/load and return when selected loot no longer fits or load reaches 80% of the soft threshold (16 kg at default 20 kg). Leave unfit loot; all loot categories remain eventually eligible.

- [ ] **AC-26:** Hauling priority is money, ammunition, usable combat gear, food, containers, clothing, broken gear. Deposit cargo while keeping personal equipment/reserves. A committed pickup removes items from the original container exactly once; carrier death puts those same items on its corpse. Interruption cannot reverse pickup or clone cargo, and nonempty loot never ages out.

- [ ] **AC-27:** The three clearing groups cost 15/25/35 money and 30/45/60 worker-seconds per side. Completion unlocks the matching authored site pair only when intervening portals are passable. No wood is produced and food plants remain. Later-group behavior is proven in representative authored portal-state scenarios without claiming the full campaign map.

- [ ] **AC-28:** All three main/wall/tower tiers use source §8.1 appearances, health, incremental prices, work and prerequisites. Wall/tower tier cannot exceed completed main tier. Pending upgrades retain old capacity, border/guard benefits and unlimited stash pages; no completed benefit is granted by payment alone.

- [ ] **AC-29:** A structure/site accepts one noncancelable paid order at a time; a second construction/upgrade/repair request rejects without charge. Destruction does not refund the order. Progress survives interruption and ordered state round trips; completion/effect publication cannot commit twice.

- [ ] **AC-30:** New and rebuilt frames start with 25% of destination-tier maximum health, including fractional health, and can be destroyed. While unfinished they grant no completed border, guard or capacity benefit. A valid construction/rebuild completion sets health to full destination maximum without extra payment or repair time.

- [ ] **AC-31:** During an upgrade the completed old tier remains functional at its actual current health. Successful completion switches tier/benefits once and sets health to full new-tier maximum at the original construction price/work quantity, including all damage taken before completion. This full-health rule explicitly replaces absolute-damage carry-forward.

- [ ] **AC-32:** Destroying an upgrading defense forfeits that upgrade and retains only its last completed tier for paid rebuilding. A structure whose destruction committed first cannot complete and resurrect. Main destruction stops its work/attacks and cannot be repaired/rebuilt; full campaign defeat UI/precedence remains Phase 8.

- [ ] **AC-33:** A repair on a living damaged structure charges max(1, ceil(0.5 × full build valuation)) once, independent of missing health. Full valuation includes prior tier investments and the main's initial 100 valuation: main 100/300/800, walls 20/80/220, towers 40/130/310; default flat fees are main 50/150/400, walls 10/40/110 and towers 20/65/155. Full-health or destroyed targets reject without charge.

- [ ] **AC-34:** The paid repair order restores to the structure's current maximum health, covering additional damage during that active order for the same fee and actual worker time. No top-up, cumulative bill, charge authorization, maintenance subscription or further carried-money requirement exists after acceptance. Builder replacement and buyer death/disconnection do not invalidate the paid job.

- [ ] **AC-35:** A repair closes once full health is reached. Damage committed after closure requires a new explicit purchase. Empty worker sets, danger and interruptions pause progress without losing payment; a destroyed target ends the order without refund or revival. Concurrent damage/completion follows the same committed host order on every participant.

- [ ] **AC-36:** A destroyed wall/tower rebuilds directly at its last completed tier using source §8.1 costs/work: walls 20/80/220 money and 20/35/50 worker-seconds, towers 40/130/310 and 30/45/60. Useful inner walls remain. Rebuilt completion follows the full-health rule and new frames grant no completed benefits.

- [ ] **AC-37:** Standalone tower tiers field exactly 1/2/3 independent structural archers, ranges 20/24/28 u, damage 14/20/26 per shot and 1.4 s firing intervals. Intrinsic arrows/bows need no recruit or stash item. Target nearest eligible zombie; equal-distance preference is an attacker of the defended structure, then stable identity. Do not target wildlife or portals.

- [ ] **AC-38:** Tower arrows obey the common friendly pass-through and solid-terrain rules, spend no ordinary inventory arrow and generate no recoverable ammunition. Tower destruction removes its guards/bows without population change, ordinary inventory, corpse or transferable loot. Guards cannot haul, change roles or become successors.

- [ ] **AC-39:** Completed main tier 3 owns exactly two independent one-archer tier-1 profiles (20 u, 14 damage, 1.4 s), sharing main health. They have no separate disable, repair, destroy or rebuild action and create no ordinary items/population. Earlier/pending main tiers grant no future attachments, and main destruction stops both.

- [ ] **AC-40:** Production people/role/sale/structure interfaces show current costs, counts, quantities, activity, equipment waiting, prerequisites, health/tier, paid progress and safety/waiting reasons. Repair quotes explicitly show the fixed fee and full-health goal, with no extra-charge or permanent-maintenance controls. Empty lists/stocks have usable explanations, and no stash/inventory filtering is introduced.

- [ ] **AC-41:** Production panel input captures its clicks; Escape closes the current panel before the run menu. Open panels do not pause time, work or threats, and damage/night warnings remain visible. Spectators cannot purchase or manipulate belongings/roles. Required work/payment/recruitment/sale sounds and visual feedback use produced content; repeated publications do not replay one-shot effects.

- [ ] **AC-42:** All new definitions and tunables have stable IDs, documented fields, defaults and validated ranges/relationships: roles/equipment/reserves, plants/wildlife, sites/trees, structures/repairs/towers and early time. Use the existing Godot/C# project/service boundaries and packaged content; no runtime dependency on recovered-source folders is introduced.

- [ ] **AC-43:** Coherent snapshot round trips preserve current phase/fraction, processed event IDs, plant/animal identities, health, cleared sites, tiers, paid flags/progress/full-health repair targets, roles, waiting, reservations and real cargo. Restoring never heals characters, refills items, creates money, charges again, rerolls animals or completes work; validated full construction completion remains its explicit structure-health exception.

- [ ] **AC-44:** Configuration changes preserve accepted payments and completed work. Fixed-quantity construction/clearing retains remaining quantity; a paid repair retains its paid-to-full-health goal with no additional fee. Current worker rates and future purchase prices apply normally. No live reload occurs; full changed-configuration load/session acceptance remains Phase 9.

- [ ] **AC-45:** Counts, prices, stack quantities and capacities are integers; health, distance, time and work may retain fractions. Reject invalid, nonfinite or overflowing values before mutation. Document tolerances without moving thresholds. Tied otherwise-equal tasks, targets, withdrawals and zone admission use stable identity order; equal definitions never collapse distinct instances. Empty/zero-work states remain valid, and duplicate event IDs do not replay effects.

- [ ] **AC-46:** Display labels preserve their configured English/Unicode text without corrupting identity comparison. Stable IDs use exact ordinal equality, independent of display normalization or enumeration order. Unknown required definitions, malformed snapshots and duplicate identities reject atomically with actionable diagnostics rather than substituting/deleting belongings. Full language/settings UX remains Phase 9.

- [ ] **AC-47:** Solo, host-local and guest actions share production authority. Stored one-to-four-participant scenarios exercise last-item/food/fireside/order conflicts, NPC/player competition, job/death/dawn interruptions, paid-state restore and each source requirement. Four local processes supply network proof only; they do not claim physical mixed-OS LAN or full save/late-join acceptance.

- [ ] **AC-48:** Save reusable AI-readable scenarios with requirement/A-ID mapping, importance/rationale, setup, production actions, expected results, evidence and ordered run groups. Finish implementation and code review/fixes before rendered tests. Use development OS at 1280×720 without a VM; select every active Critical, new/changed and affected case, retain failures/omissions, batch repairs and run the complete selected set after repairs. Hosted/release evidence remains distinct.

## Source Acceptance Boundaries

| Source scenario | Phase 7 obligation and evidence boundary |
| --- | --- |
| A-06, A-10, A-11 | Recruitment, limits, actual outfit and queued Phase 6 Respawn integration; no additional survivor or supply creation. |
| A-12–A-16 | Jobs, equipment, real reserves, fleeing, safe work and basic guarding now; assault/allocation/recall portions retain Phase 8 ownership. |
| A-17–A-21 | Paid clearing, three-tier structures, new full-health completion, flat-fee repair, rebuilding and structural guards now. |
| A-25, A-26 | Real NPC deliveries, food-sale proceeds, persistent loot and carrier-death cargo integrate with Phase 6 ownership/pages. Complete zombie roster coverage remains Phase 8. |
| A-28, A-29 | Actual player/NPC competition, physical carried payments, stash-only sales and consistent outcomes now. |
| A-30 | Finite food/meat, once-only resource identities and snapshot restoration now. Actual dawn save/load/late-join orchestration remains Phase 9. |
| A-31 | Structural tower attacks integrate common arrow/friendly-pass rules now; full campaign/archetype integration remains Phase 8. |
| A-41, A-42 | Paid order, cargo, role and daily-resource snapshot/configuration commitments begin here; complete player-facing save/load/session/configuration acceptance remains Phase 9. |

These boundaries grant no test pass or complete credit for portions owned by later phases.

## Edge Coverage

**Coverage:** 168/168 applicable edges resolved · 168 explicit · 0 backstop · 0 dismissed · 0 unresolved.

The compiled canonical edge probe was run over all 25 exact final requirement statements with authored numeric, collection, text, stateful and I/O shapes. It proposed 164 rows and zero unclassified requirements; a second canonical invocation validated all 164 explicit resolutions. Manual review added four cases concerning empty/ordered daily events, paid repairs without workers/buyer and panel input ordering. The compiled shared coverage validator checked the full 168-row union without orphan, duplicate or invalid resolutions.

The creator approved the money/belongings and work/interface groups, then reviewed the limits/timing examples and confirmed the 12 u civilian retreat cutoff. Existing approved numerical defaults, inclusive fits, once-only events, repeatable ties and empty-state rules remain binding. Every proposed pair is retained below; no implementation/test execution is implied by a resolved specification edge.

| Edge | Category | Requirement | Status | Verification | Resolution |
| --- | --- | --- | --- | --- | --- |
| E001 | boundary | R01 | resolved | explicit | AC-01 AC-02 AC-43 AC-45. 600/300 s transitions and processed resource-event IDs. |
| E002 | precision | R01 | resolved | explicit | AC-01 AC-02 AC-43 AC-45. 600/300 s transitions and processed resource-event IDs. |
| E003 | idempotency | R01 | resolved | explicit | AC-01 AC-02 AC-43 AC-45. 600/300 s transitions and processed resource-event IDs. |
| E004 | concurrency | R01 | resolved | explicit | AC-01 AC-02 AC-43 AC-45. 600/300 s transitions and processed resource-event IDs. |
| E005 | boundary | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E006 | adjacency | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E007 | empty | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E008 | ordering | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E009 | precision | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E010 | idempotency | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E011 | concurrency | R02 | resolved | explicit | AC-03 AC-04 AC-07 AC-45. Inclusive wall border, empty-wall refuge and live fallback. |
| E012 | boundary | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E013 | adjacency | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E014 | empty | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E015 | ordering | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E016 | precision | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E017 | idempotency | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E018 | concurrency | R03 | resolved | explicit | AC-05 AC-07 AC-45. Exact accessible integer money, distinct stacks and no duplicate charge. |
| E019 | boundary | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E020 | adjacency | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E021 | empty | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E022 | ordering | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E023 | precision | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E024 | idempotency | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E025 | concurrency | R04 | resolved | explicit | AC-06 AC-07 AC-45. Actual integer stash food, exact proceeds and competing withdrawals. |
| E026 | boundary | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E027 | adjacency | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E028 | empty | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E029 | ordering | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E030 | precision | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E031 | idempotency | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E032 | concurrency | R05 | resolved | explicit | AC-02 AC-08 AC-45. 3 s collection, zero stock and refill-to-8/6 limits. |
| E033 | boundary | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E034 | adjacency | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E035 | empty | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E036 | ordering | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E037 | precision | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E038 | idempotency | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E039 | concurrency | R06 | resolved | explicit | AC-04 AC-09 AC-18 AC-20 AC-26 AC-45. Inclusive 60 u range, actual arrows and once-only 2/5 meat. |
| E040 | boundary | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E041 | adjacency | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E042 | empty | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E043 | ordering | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E044 | precision | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E045 | idempotency | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E046 | concurrency | R07 | resolved | explicit | AC-02 AC-10 AC-45. 24 living cap, partial/skipped batches and preserved animal identities. |
| E047 | boundary | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E048 | adjacency | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E049 | empty | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E050 | ordering | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E051 | precision | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E052 | idempotency | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E053 | concurrency | R08 | resolved | explicit | AC-05 AC-07 AC-11 AC-45. One basic recruit, no extra supplies and competing last-place purchases. |
| E054 | boundary | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E055 | adjacency | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E056 | empty | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E057 | ordering | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E058 | precision | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E059 | idempotency | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E060 | concurrency | R09 | resolved | explicit | AC-07 AC-12 AC-13 AC-45. 8/12/16 and 100 limits, count categories and preserved overflow. |
| E061 | boundary | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E062 | adjacency | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E063 | empty | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E064 | ordering | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E065 | precision | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E066 | idempotency | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E067 | concurrency | R10 | resolved | explicit | AC-14 AC-15 AC-16 AC-07 AC-45. Local NPC presence, free same-role selection, cargo-first duties and nearest guard. |
| E068 | boundary | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E069 | adjacency | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E070 | empty | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E071 | ordering | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E072 | precision | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E073 | idempotency | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E074 | concurrency | R11 | resolved | explicit | AC-17 AC-18 AC-45. 20 s safety/locality gate, real gear ranking and preserved filled containers. |
| E075 | boundary | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E076 | adjacency | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E077 | empty | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E078 | ordering | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E079 | precision | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E080 | idempotency | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E081 | concurrency | R12 | resolved | explicit | AC-18 AC-19 AC-20 AC-45. 6 s cooldown, 60%/30% thresholds, real reserves and safe fallback. |
| E082 | boundary | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E083 | adjacency | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E084 | empty | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E085 | ordering | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E086 | precision | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E087 | idempotency | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E088 | concurrency | R13 | resolved | explicit | AC-04 AC-21 AC-22 AC-45. 12 u danger, nightfall, blocked/open escape and retained cargo. |
| E089 | boundary | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E090 | adjacency | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E091 | empty | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E092 | ordering | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E093 | precision | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E094 | idempotency | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E095 | concurrency | R14 | resolved | explicit | AC-23 AC-24 AC-45. Zero to three contributors, priority/distance ties and retained actual work. |
| E096 | boundary | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E097 | adjacency | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E098 | empty | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E099 | ordering | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E100 | precision | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E101 | idempotency | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E102 | concurrency | R15 | resolved | explicit | AC-25 AC-26 AC-18 AC-45. 16 kg default return threshold, real fit, reservations and cargo death. |
| E103 | boundary | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E104 | adjacency | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E105 | empty | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E106 | ordering | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E107 | precision | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E108 | idempotency | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E109 | concurrency | R16 | resolved | explicit | AC-27 AC-29 AC-45. Exact clearing prices/work, existing completion and portal-gated sites. |
| E110 | boundary | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E111 | adjacency | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E112 | empty | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E113 | ordering | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E114 | precision | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E115 | idempotency | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E116 | concurrency | R17 | resolved | explicit | AC-12 AC-28 AC-31 AC-45. Completed tiers 1–3, prerequisites and old-tier benefits. |
| E117 | boundary | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E118 | adjacency | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E119 | empty | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E120 | ordering | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E121 | precision | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E122 | idempotency | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E123 | concurrency | R18 | resolved | explicit | AC-24 AC-29 AC-30 AC-32 AC-45. One order, 25% frame health, interruption and destruction before completion. |
| E124 | boundary | R19 | resolved | explicit | AC-31 AC-32 AC-45. Old tier stays functional, valid completion fully heals, destruction cannot revive. |
| E125 | precision | R19 | resolved | explicit | AC-31 AC-32 AC-45. Old tier stays functional, valid completion fully heals, destruction cannot revive. |
| E126 | idempotency | R19 | resolved | explicit | AC-31 AC-32 AC-45. Old tier stays functional, valid completion fully heals, destruction cannot revive. |
| E127 | concurrency | R19 | resolved | explicit | AC-31 AC-32 AC-45. Old tier stays functional, valid completion fully heals, destruction cannot revive. |
| E128 | boundary | R20 | resolved | explicit | AC-33 AC-34 AC-35 AC-45. Flat fee once, no additional charges, no workers and closure at full health. |
| E129 | precision | R20 | resolved | explicit | AC-33 AC-34 AC-35 AC-45. Flat fee once, no additional charges, no workers and closure at full health. |
| E130 | idempotency | R20 | resolved | explicit | AC-33 AC-34 AC-35 AC-45. Flat fee once, no additional charges, no workers and closure at full health. |
| E131 | concurrency | R20 | resolved | explicit | AC-33 AC-34 AC-35 AC-45. Flat fee once, no additional charges, no workers and closure at full health. |
| E132 | boundary | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E133 | adjacency | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E134 | empty | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E135 | ordering | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E136 | precision | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E137 | idempotency | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E138 | concurrency | R21 | resolved | explicit | AC-03 AC-30 AC-36 AC-45. Last completed tier, preserved inner walls and full-health rebuild. |
| E139 | boundary | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E140 | adjacency | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E141 | empty | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E142 | ordering | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E143 | precision | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E144 | idempotency | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E145 | concurrency | R22 | resolved | explicit | AC-37 AC-38 AC-45. 1/2/3 guards, independent shots, target ties and no ordinary loot. |
| E146 | boundary | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E147 | adjacency | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E148 | empty | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E149 | ordering | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E150 | precision | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E151 | idempotency | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E152 | concurrency | R23 | resolved | explicit | AC-39 AC-32 AC-45. Two tier-one profiles, shared health and no duplicate attachments. |
| E153 | empty | R24 | resolved | explicit | AC-40 AC-41 AC-46 AC-45. Empty panels, faithful text, captured input and continuous world. |
| E154 | encoding | R24 | resolved | explicit | AC-40 AC-41 AC-46 AC-45. Empty panels, faithful text, captured input and continuous world. |
| E155 | idempotency | R24 | resolved | explicit | AC-40 AC-41 AC-46 AC-45. Empty panels, faithful text, captured input and continuous world. |
| E156 | concurrency | R24 | resolved | explicit | AC-40 AC-41 AC-46 AC-45. Empty panels, faithful text, captured input and continuous world. |
| E157 | boundary | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E158 | adjacency | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E159 | empty | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E160 | encoding | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E161 | ordering | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E162 | precision | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E163 | idempotency | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E164 | concurrency | R25 | resolved | explicit | AC-42 AC-43 AC-44 AC-45 AC-46 AC-47 AC-48. Coherent ownership/paid-state restore, exact IDs, numeric validation and evidence. |
| E165 | empty | R01 | resolved | explicit | AC-02 AC-45. A dawn with zero plants or eligible wildlife additions still records processed event identities and creates nothing. (Manual addition.) |
| E166 | ordering | R01 | resolved | explicit | AC-01 AC-02 AC-43. Transition events use source order and retained per-event identities; restored completed events cannot replay. (Manual addition.) |
| E167 | empty | R20 | resolved | explicit | AC-34 AC-35. With no eligible workers the paid repair waits; a buyer's death/disconnection does not require more money or delete the order. (Manual addition.) |
| E168 | ordering | R24 | resolved | explicit | AC-40 AC-41. Escape closes the current panel first and consumed panel clicks cannot reach a background purchase or attack. (Manual addition.) |

## Prohibitions (must-NOT)

**Coverage:** 0/0 additional applicable prohibitions resolved · 0 unresolved.

The two-stage recall/precision probe was performed over R01–R25 after edge resolution. Candidates involving unauthorized fees, lost/deleted belongings, invented supplies, automatic work completion and misleading execution evidence are already explicit functional acceptance checks, scope exclusions or ordinary engineering correctness. No additional bespoke values/safety/ethics prohibition survived the precision filter. No canon security/compliance constraint was duplicated here. Existing functional negative checks, including no repair top-ups and no revival after destruction, remain mandatory.

| Prohibition | Requirement | Status | Verification / Reason |
| --- | --- | --- | --- |

No prohibition/test descriptor is fabricated for a zero-item result.

## Ambiguity Report

| Dimension | Score | Minimum | Status | Notes |
| --- | --- | --- | --- | --- |
| Goal clarity | 0.98 | 0.75 | ✓ | Working solo/LAN camp and concrete transaction/job/building outcomes. |
| Boundary clarity | 0.98 | 0.70 | ✓ | Camp area, autonomous shared orders and later map/campaign/save/filtering owners explicit. |
| Constraint clarity | 0.96 | 0.65 | ✓ | Local role changes, exterior safety, fixed fee and full-health lifecycle resolved. |
| Acceptance criteria | 0.97 | 0.70 | ✓ | 48 pass/fail checks and all 168 applicable edges explicit. |
| **Ambiguity** | **0.026** | **≤ 0.20** | **✓** | 1 − (0.35×0.98 + 0.25×0.98 + 0.20×0.96 + 0.20×0.97). |

| Assessment | Goal | Boundaries | Constraints | Acceptance | Ambiguity |
| --- | --- | --- | --- | --- | --- |
| Initial, roadmap/requirements only | 0.88 | 0.70 | 0.68 | 0.72 | 0.2370 |
| After round 1 | 0.92 | 0.83 | 0.78 | 0.79 | 0.1565 |
| After round 2 | 0.94 | 0.92 | 0.88 | 0.89 | 0.0870 |
| After round 3 | 0.94 | 0.97 | 0.90 | 0.92 | 0.0645 |
| After round 4 | 0.96 | 0.97 | 0.90 | 0.94 | 0.0535 |
| After round 5 | 0.97 | 0.98 | 0.96 | 0.96 | 0.0315 |
| After round 6, flat fee not yet confirmed | 0.98 | 0.98 | 0.85 | 0.94 | 0.0540 |
| Final fee confirmation and edge review | 0.98 | 0.98 | 0.96 | 0.97 | 0.0260 |

## Interview Log

| Round | Perspective | Question summary | Decision locked |
| --- | --- | --- | --- |
| 1 | Researcher | Exterior civilian work and role-change distance | All civilian jobs may work outside camp in safe reachable daylight territory; NPC must actually return to the main building before changing role. |
| 2 | Researcher / Simplifier | Basic warriors and carried cargo during a job change | Guard the nearest completed wall/refuge; deposit collected cargo and obsolete role gear before new duties, keeping personal equipment and applicable reserves. |
| 3 | Boundary Keeper | Playable world scope and direct NPC commands | Working camp area on both sides with representative portals; complete six-portal map stays Phase 8. Roles/shared paid orders drive automatic tasks; no per-NPC work/target commands here. |
| 4 | Failure Analyst | Damage during construction and repairs | Full-health building/rebuild/upgrade completion is included in original money/time. Interim continuing-repair extra charges were later superseded in round 6. |
| 5 | Seed Closer | Worker replacement, payer loss and repair rounding | Replacement builders preserve the same accepted order/payment/progress. Interim payer-takeover and cumulative extra-billing ideas were later superseded in round 6. |
| 6 | Seed Closer | Repair closure and final simplification | One constant upfront fee independent of missing HP repairs to full health, including additional damage while active; no extra charges. Close at full health; later damage requires another purchase. |
| Final confirmation | Pricing / gate | Fixed fee amount and permission to finish | Use 50% of current-tier full build valuation, rounded up/minimum one; finish the checks and specification. |
| Edge review | Limits / conflicts / interruptions | Three grouped review sets | Money/belongings and work/interfaces specified. Limits/timing explained; creator clarified the meaning of 12 u and explicitly kept that civilian retreat distance. |

**Superseded discussion is history, not current scope:** automatic repair top-ups, cumulative per-order extra billing, pause-charge controls and payer takeover are not implementation requirements. The final flat fee removes those mechanisms. Canonical BUILD-04/BUILD-05 and source completion/repair rules were amended to agree; IDs, owners and Pending status remain unchanged.

---

*Phase: 07-camp-economy-and-development*
*Spec created: 2026-11-18*
*Planning complete: [24 executable plans and their coverage](07-COVERAGE.md), independently [reviewed](07-PLAN-CHECK.md). Implementation requires actual Phase 5/6 delivery and the [07-01 prerequisite audit](07-01-PLAN.md); no gameplay acceptance is granted by planning.*

