# Requirements: KingdomZombies

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. Historical evidence limits remain explicit; current routine local verification follows the development-OS-only strategy below.

**Verification policy, latest 2026-11-15 amendment:** Follow the [local testing strategy](../docs/workflow/local-testing.md) immediately: implement, review/fix, then grouped 1280×720 development-OS game tests without a VM. Select all active Critical cases plus new/changed cases and affected regressions, regardless of importance. Collect failures, batch repairs, check failures, then rerun the entire selected set on the repaired build. Every phase saves reusable scenarios with importance and ordered run groups. Phase 5 implements the framework and actual hosted Windows/Linux compatibility. TEST-11 adds Phase 11 nightly full testing; original 133 IDs/owners and all eight Phase 5 certification IDs are preserved. Basic hosted PR checks remain build/repository-only with a ten-minute-per-OS target.

**Defined:** 2026-11-13
**Release:** Complete version 0.1
**Status:** Original 133 version 0.1 requirements approved on 2026-11-13; TEST-11 and Phase 11 added on 2026-11-15. All 134 requirements have one primary phase. RULE-01–RULE-07 are complete under the Phase 1 exception; 127 requirements remain Pending.
**Core Value:** Players can build, equip, and defend a shared survivor camp through a complete solo or four-player LAN campaign, with consistent ownership of their survivors, items, and progress.

## Authority and Interpretation

The [game specification](../specs/0.1/initial-spec.md), [asset/resource plan](../specs/0.1/assets-and-resources.md), and [approved project definition](PROJECT.md) are the source of scope. [Research synthesis](research/SUMMARY.md) informs implementation and verification; it does not remove or add gameplay scope.

All required details in the source sections remain binding. The concise requirement statements below do not replace numerical tables, edge cases, configuration rules, or exclusions in the specification. Proposed values/supporting rules remain the working implementation baseline to validate through playtesting, not proven balance. No required game feature is deferred to a later release.

The GSD template heading `v1 Requirements` below means this project's first release, **version 0.1**, not a separate version 1.0. Each ID has exactly one primary roadmap phase; later integration and release reruns can exercise it again. All requirements are unimplemented/unverified at initialization.

## v1 Requirements

### Development rules and documentation

Source: Creator direction; PROJECT.md; LangProj rule and documentation references.

- [x] **RULE-01**: Developers can build with a pinned Godot/.NET/compiler/analyzer combination proven by a representative engine integration check.
- [x] **RULE-02**: Developers inherit applicable LangProj C# conventions through a retained/adapted/inapplicable rule register with reasons for every Godot adaptation.
- [x] **RULE-03**: Developers receive build or check failures for rule violations, with positive and deliberately failing proofs and only documented, narrowly scoped exemptions.
- [x] **RULE-04**: Developers can navigate a short root AGENTS.md and focused documentation indexes with valid links and consistent Windows/Linux filename casing.
- [x] **RULE-05**: Developers have documented, enforced dependency boundaries separating gameplay rules, Godot integration, and multiplayer services.
- [x] **RULE-06**: Developers can use normal Godot construction, callbacks, partial/generated code, exports, signals, nullability, and teardown under the rules without blanket suppression.
- [x] **RULE-07**: Developers have explicit rules for main-thread engine access, testable time/randomness, stable identities, and saveable authoritative state.

**Acceptance timing amended by the creator on 2026-11-14:** Phase 1 retains all applicable coding and architecture rules, existing working checks, narrow exemptions, focused documentation, and actual clean Windows/Linux baseline checks. Measured coverage (including the unchanged 90% line / 80% branch targets), exhaustive two-platform certification, and unfinished aggregate runner/report certification move to Phase 5 under TEST-02, TEST-07, TEST-08 and TEST-09. RULE-01–RULE-07 keep their Phase 1 primary ownership; the later certification work has explicit Phase 5 acceptance ownership in the [handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). Deferred work is pending, never passed or waived.

### Playable foundation

Source: Creator Phase 2 direction.

- [ ] **BASE-01**: Players can launch the game into a visible main menu and start a small playable scene.
- [ ] **BASE-02**: Players can see a few representative base assets and move one character left and right through normal gameplay input.
- [ ] **BASE-03**: Developers can repeat launch, menu, movement, and rendered-scene smoke checks for the playable foundation.

### LAN movement foundation

Source: Creator Phase 3 direction; spec §10.

- [ ] **LAN-01**: A host and up to three guests can connect to the same foundation scene using separate game processes.
- [ ] **LAN-02**: Each participant can move only their own character while every participant observes consistent movement.
- [ ] **LAN-03**: Participants use the same authoritative action path for solo, host-local, and guest input without duplicate execution.
- [ ] **LAN-04**: Developers can replace discovery, identity, and connection services without changing gameplay rules or using a LAN address as persistent player identity.

### Asset and resource production

Source: Asset plan §§3–12, all CH/AN/IT/BL/EN/FX/UI/AU/DT/TXT obligations.

- [ ] **ASSET-01**: Developers can trace selected recovered assets and reused logic to preserved originals, transformations, stable output IDs, versions, and evidence.
- [ ] **ASSET-02**: Players receive content covering all 180 resource obligations, reusing suitable donors before adapting or generating missing outputs.
- [ ] **ASSET-03**: Developers can inspect required body, equipment, hand, direction, action, corpse, building, and portal combinations in content fixtures.
- [ ] **ASSET-04**: Developers can inspect the required UI/text/data resources and audition every required sound/ambience category with its event bindings.
- [ ] **ASSET-05**: Players can run the packaged content without access to the creator's recovered-material folder or other absolute development paths.

### Local, CI, and AI-operated verification

Source: Creator Phase 5 direction; spec §14 and A-01–A-47.

- [ ] **TEST-01**: Developers and AI agents can find repository-stored readable cases linked to requirements, A-01–A-47, executable checks, owning phases, environments, and evidence.
- [ ] **TEST-02**: Developers and AI agents can run the same documented verification entry points locally and in GitHub Actions on Windows and Linux.
- [ ] **TEST-03**: AI agents can follow a documented select, run, inspect, reproduce, fix, and rerun procedure after changes.
- [ ] **TEST-04**: AI agents can execute gameplay-rule checks with controlled fixtures and repeatable time/randomness where applicable.
- [ ] **TEST-05**: AI agents can exercise actual menu, keyboard/mouse, focus, and scene behavior through production input paths.
- [ ] **TEST-06**: AI agents can run a real host-plus-three-client scenario with independent profiles, bounded waits, isolated logs, and reliable cleanup.
- [ ] **TEST-07**: AI agents can inspect current-run results, logs, state evidence, and required rendered images to diagnose failures.
- [ ] **TEST-08**: Verification fails for missing dependencies, zero discovery, skipped required cases, crashes, timeouts, missing peer results, or missing required evidence.
- [ ] **TEST-09**: Developers can prove the verification runner rejects deliberate failures and correctly distinguishes future planned cases from currently required cases.
- [ ] **TEST-10**: Developers can activate new cases with each implemented feature and retain separate rendered, real mixed-OS LAN, reference-PC, and endurance evidence.
- [ ] **TEST-11**: Developers receive scheduled nightly full-regression results across every active importance level, using saved scenarios and grouped game runs with inspectable failures/artifacts and manual replay. Phase 11 owns implementation; reduced phase-end selection begins immediately before this exists.

Phase 5 also owns the [Phase 1 certification handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md): real analyzer/production coverage measurement and 90%/80% enforcement, exhaustive clean Windows/Linux suites and evidence reconciliation, and remaining aggregate runner/report checks. Existing Phase 1 tests and enforcement remain required regressions. This changes delivery timing without adding or deleting requirement IDs.

### Survivors, movement, and recovery

Source: Spec §§2, 4; A-01, A-23.

- [ ] **CHAR-01**: Players start with eight total recruited survivors and one configured starting kit, independent of the number of connected players.
- [ ] **CHAR-02**: Players can walk, sprint, jump, and control airborne movement under configured movement and stamina rules.
- [ ] **CHAR-03**: Players can observe action stamina costs and natural recovery, with insufficient stamina preventing the relevant action.
- [ ] **CHAR-04**: Players can recover health naturally only under configured safe-camp conditions.
- [ ] **CHAR-05**: Players can consume carried food for bounded health/stamina recovery under its cooldown.
- [ ] **CHAR-06**: Players experience carried-load penalties based on all equipped and nested items counted once; severe load blocks sprint/jump while walking remains possible.

### Inventory, equipment, and belongings

Source: Spec §5; A-22–A-29.

- [ ] **INV-01**: Players can use all 24 configured item definitions with persistent instance identity and exactly one current location per item.
- [ ] **INV-02**: Players can equip the eleven specified slots and store items only in actual item-provided pockets and containers.
- [ ] **INV-03**: Players can rotate fitting items and receive a safe rejection for an invalid placement.
- [ ] **INV-04**: Players can split and combine compatible stacks while preserving exact quantities and item state.
- [ ] **INV-05**: Players can nest containers within configured limits while cycles and excessive nesting are rejected.
- [ ] **INV-06**: Players can move, unequip, or drop filled containers and clothing without losing their nested contents.
- [ ] **INV-07**: Players can assign four quick slots to actual carried food stacks, with references invalidated when those stacks leave or are exhausted.
- [ ] **INV-08**: Players can immediately inspect and transfer permitted nearby NPC, corpse, and loot contents; other controlled-player inventories remain private.
- [ ] **INV-09**: Players can cancel dragging, close a panel, or lose access without discarding or duplicating the item being previewed.
- [ ] **INV-10**: Players and NPCs can deposit any valid cargo into an unlimited paged shared stash, with local access, filtering, and valid automatic placement.
- [ ] **INV-11**: Concurrent transfers and ownership changes commit only valid successful actions, with losing actors receiving corrected state and clear failure feedback.
- [ ] **INV-12**: Players can recover exactly a dead ordinary character's remaining belongings; death creates no fresh loot or replacement ammunition.
- [ ] **INV-13**: Players can recover nonempty corpses and piles indefinitely; only empty containers expire, and adding an item cancels cleanup.

### Combat and equipment condition

Source: Spec §§4.2–4.3, 7.2, 9.1; A-14, A-23, A-26, A-31, A-32.

- [ ] **COMBAT-01**: Players can attack in their facing direction with fists or a usable one-handed melee weapon.
- [ ] **COMBAT-02**: Players can swing two one-handed melee weapons together with the specified combined stamina and attack timing.
- [ ] **COMBAT-03**: Players can aim a bow with the mouse and fire only with real arrows in the opposite hand, manually refilling that hand.
- [ ] **COMBAT-04**: NPCs and zombies consume actual arrows, refill from real carried reserves, and fall back to usable carried gear or fists when necessary.
- [ ] **COMBAT-05**: Characters receive the configured armor reduction and equipment wear; broken weapons/armor remain items but provide no usable attack/protection.
- [ ] **COMBAT-06**: Attacks obey faction and obstruction rules, including no friendly damage, friendly arrow passthrough, and enemy arrows hitting blocking defenses.
- [ ] **COMBAT-07**: Spit stops at its first eligible hit, and bloater acid remains entirely cosmetic.
- [ ] **COMBAT-08**: Required equipped items and their changes are visible on living characters and lootable corpses.

### Control, succession, and outcomes

Source: Spec §2; A-04–A-09.

- [ ] **RUN-01**: Each participant controls one unique ordinary survivor identified by a cosmetic crown that occupies no slot and creates no loot.
- [ ] **RUN-02**: A dead player's control transfers to a random eligible survivor without changing that survivor's health, gear, location, or belongings.
- [ ] **RUN-03**: Simultaneous deaths and availability changes cannot assign one survivor to multiple participants.
- [ ] **RUN-04**: Players without a successor can spectate and regain control in the specified waiting order when an eligible survivor becomes available.
- [ ] **RUN-05**: Main-building destruction and loss of every living controlled/eligible survivor each independently end the camp in defeat.
- [ ] **RUN-06**: Destroying the final portal immediately awards victory and allows continued play with surviving zombies and possessions intact.
- [ ] **RUN-07**: Simultaneous new victory/defeat follows defeat precedence, while a previously awarded victory remains recorded after a continuation defeat.

### World, time, and exploration

Source: Spec §3; A-01–A-03, A-15–A-17, A-30, A-33, A-45.

- [ ] **WORLD-01**: Players can traverse one handcrafted map with six portals and the specified baseline sites, clearing groups, plants, and wildlife zones.
- [ ] **WORLD-02**: Active portals block survivors even when jumping, permit zombies through, and become permanently passable after destruction.
- [ ] **WORLD-03**: Players experience configured day/night cycles, opening scheduled-wave grace, and three-day escalation boundaries.
- [ ] **WORLD-04**: Dawn applies each daily event once, including across saving/loading and late joining.
- [ ] **WORLD-05**: Completed living outer walls define camp borders, with inner-wall/refuge fallback and danger-aware safe-work conditions.
- [ ] **WORLD-06**: Players have independent cameras and a shared explored minimap that preserves known terrain, portals, defenses, and teammate identity without revealing unknown threats.
- [ ] **WORLD-07**: Active gameplay continues during solo/multiplayer inventories, menus, saving, and the victory overlay.

### Physical economy and food

Source: Spec §6; A-28–A-30.

- [ ] **ECON-01**: Players pay for actions using accessible physical money in their own carried inventory, with current-state validation and no charge on failed or duplicate actions.
- [ ] **ECON-02**: Players can sell only actual shared-stash food, receiving physical proceeds in that stash rather than an automatic carried wallet.
- [ ] **ECON-03**: Gatherers collect finite plant food into real inventories, with each plant refilled to its configured limit once per dawn.
- [ ] **ECON-04**: Hunters consume real ammunition to hunt rabbits and deer and collect their existing meat inventory.
- [ ] **ECON-05**: Wildlife batches respect daily identities and living caps while preserving animals left from earlier days.

### Recruitment, roles, and logistics

Source: Spec §§6.4, 7.1–7.3; A-06, A-10–A-16, A-26, A-28.

- [ ] **NPC-01**: Players can buy recruits only at the main building, receiving the basic outfit without replenishing money, food, weapons, or starting supplies.
- [ ] **NPC-02**: Recruitment obeys separate fireside and total-population limits, completed-tier capacity, and non-destructive temporary-control overflow rules.
- [ ] **NPC-03**: Players can assign gatherer, hunter, melee-warrior, and warrior-archer roles, with the specified costs, same-role behavior, and visible missing-equipment waiting.
- [ ] **NPC-04**: NPCs safely acquire and upgrade real equipment from the stash while preserving replaced gear and container contents.
- [ ] **NPC-05**: NPCs restock actual food/ammunition and safely stow or drop unusable gear without creating or deleting items.
- [ ] **NPC-06**: Civilians abandon unsafe work and flee with cargo, fight only when escape is blocked, and retreat from exterior work at night.
- [ ] **NPC-07**: Unassigned workers alone perform paid construction, repair, and clearing under configured work priorities and interruption rules.
- [ ] **NPC-08**: Eligible non-busy ordinary NPCs can reserve, collect, and deliver loot within capacity while retaining their role; carrier death leaves cargo on the carrier's corpse.

### Structures and paid work

Source: Spec §8; A-17–A-21.

- [ ] **BUILD-01**: Players can pay workers to clear required trees and unlock reachable building sites without producing a wood resource.
- [ ] **BUILD-02**: Players can build and upgrade the main building, walls, and towers through three tiers with completed-main-tier prerequisites.
- [ ] **BUILD-03**: Accepted work orders commit once, preserve progress on interruption, and supply no completed benefits while a new frame remains unfinished.
- [ ] **BUILD-04**: An old tier remains functional during an upgrade, with specified damage-preserving completion and destruction behavior.
- [ ] **BUILD-05**: Players can purchase a bounded amount of repair work; subsequent damage receives no free extra repair.
- [ ] **BUILD-06**: Players can rebuild a destroyed defense at its last completed tier while retaining useful inner walls.
- [ ] **BUILD-07**: Standalone tower tiers provide one, two, and three structural archers with no survivor inventory, population use, corpse, or transferable loot.
- [ ] **BUILD-08**: The tier-three main building provides exactly two single-archer level-one attack profiles sharing the main building's health.

### Zombies, waves, and portal pressure

Source: Spec §9; A-02, A-03, A-08, A-32–A-36.

- [ ] **ZOMB-01**: Players encounter all seven specified zombie archetypes with their distinct behavior and actual equipped-item effects.
- [ ] **ZOMB-02**: Zombies receive legal money, clothing, archetype guarantees, and optional loot before death; equipment and arrows remain real possessions.
- [ ] **ZOMB-03**: Designers can edit zombie progression independently to change future counts, mix, and quality without player-count scaling or retroactively inflating living zombies.
- [ ] **ZOMB-04**: Every surviving portal releases its scheduled eligible nightly wave; opening grace suppresses only scheduled waves.
- [ ] **ZOMB-05**: Damaging a portal can trigger warned defenders under its configured cooldown, stage, and cap rules during day, night, or grace.
- [ ] **ZOMB-06**: Each portal destruction schedules distinct additional retaliation at every surviving source without replacing regular waves.
- [ ] **ZOMB-07**: Wave queues obey source priority, living caps, dawn expiration, and source-destruction cancellation without deleting living enemies or their possessions.
- [ ] **ZOMB-08**: Eligible zombies retreat at dawn with their belongings, leave no loot when exiting, and stop retreating permanently after the final portal is destroyed.

### Warrior defense and assaults

Source: Spec §7.4; A-16, A-37.

- [ ] **ARMY-01**: Warriors allocate to camp sides and fall back with defensive borders while retaining combat priorities.
- [ ] **ARMY-02**: Players can pay a side's rally point to send a valid ready squad against its next surviving portal, with shortages/duplicate orders rejected without charge.
- [ ] **ARMY-03**: Players can recall squads for free; recall, success, and obsolete targets return survivors without teleporting, refunds, or automatically attacking another portal.

### Complete multiplayer sessions

Source: Spec §10; A-28, A-38–A-40, A-45.

- [ ] **SESSION-01**: Windows and Linux players can discover LAN sessions or join a manual address with clear metadata, capacity, and compatibility feedback.
- [ ] **SESSION-02**: A host can start alone and accept late joiners without creating new survivors or supplies.
- [ ] **SESSION-03**: Late joiners receive coherent current world state before gaining control while existing participants continue playing.
- [ ] **SESSION-04**: Players have equal camp permissions; only the host controls session-wide save, load, closing, and effective balance.
- [ ] **SESSION-05**: A departing guest's survivor returns to NPC control with belongings and remembered role; reconnection restores it only if alive and available.
- [ ] **SESSION-06**: Host departure or loss ends the session for all players with clear messaging and the specified last-successful-save recovery behavior.

### Saving and configuration compatibility

Source: Spec §§5.6, 11; A-30, A-40–A-44.

- [ ] **SAVE-01**: The host can save manually, at dawn, and on normal exit while active gameplay continues.
- [ ] **SAVE-02**: Players can select and resume multiple named runs with the specified manual/autosave retention.
- [ ] **SAVE-03**: A save captures coherent character, inventory, corpse, work, world, wave, random, outcome, exploration, and player-association state without healing, rerolls, or repeated events.
- [ ] **SAVE-04**: Failed, interrupted, corrupt, or unknown-content save operations give actionable errors and preserve the last valid/original data.
- [ ] **SAVE-05**: The host can retry, return to play, or explicitly leave without saving when an exit save fails.
- [ ] **SAVE-06**: Current host configuration applies only on new-run start/load while preserving old payments, work/event commitments, fractional time, possessions, and existing populations.
- [ ] **SAVE-07**: Players can inspect and extract belongings from saved legacy layouts without forced stash movement, deletion, or whole-save rejection solely for layout changes.
- [ ] **SAVE-08**: Legacy transfers use current destination rules, preserve unresolved saved layouts, and reject illegal actions without losing the source item.

### Designer configuration

Source: Spec §12; asset domains DT-01–DT-18; A-42–A-44.

- [ ] **DATA-01**: Designers can edit all specified balance/content domains in separate files with a complete shipped baseline and stable identifiers.
- [ ] **DATA-02**: Designers can find each field's purpose, unit, default, allowed values, and relationships in maintained documentation.
- [ ] **DATA-03**: Invalid configuration is rejected before start/load with file, field, value, and reason, including missing content and presentation references.
- [ ] **DATA-04**: Each player's display, audio, and binding preferences remain independent of shared host gameplay settings.

### Interface, audio, and translation readiness

Source: Spec §13; A-24, A-45, A-46.

- [ ] **UX-01**: Players can complete every specified menu, HUD, people, sale, inventory, stash, loot, building, rally, map, spectator, save, and outcome interaction with readable state/error feedback.
- [ ] **UX-02**: Panels consume their input, close in the specified order, handle overlapping targets, and prevent background attacks or purchases.
- [ ] **UX-03**: Spectators can follow teammates and inspect permitted shared status without mutating the camp or revealing unexplored territory.
- [ ] **UX-04**: Players can remap all essential actions, restore defaults, and retain their bindings between sessions.
- [ ] **UX-05**: Players can use the required window/resolution/UI-scale settings with readable layouts at 1280x720 and 1920x1080.
- [ ] **UX-06**: Players hear the required integrated effects/ambience and receive visible critical warnings even when audio is muted.
- [ ] **UX-07**: All required text ships in an English catalog with layouts tested using longer replacement strings and without gameplay text embedded solely in artwork.

### Release validation

Source: Spec §14; A-47 and all release gates.

- [ ] **REL-01**: Players can finish fresh solo and four-player campaigns without development commands, injected supplies, or unfinished mandatory steps.
- [ ] **REL-02**: Players can run packaged Windows and Linux builds without a development environment, with tested systems and mixed-OS LAN evidence recorded.
- [ ] **REL-03**: Developers can show passing acceptance evidence with no known reproducible crash, corruption, duplication, persistent disagreement, or progression blocker.
- [ ] **REL-04**: The feature-complete reference-PC stress test meets the specified 30-FPS sampling and save/join responsiveness limits at four players, about 100 survivors, and 300 zombies.
- [ ] **REL-05**: A three-hour saved/resumed endurance run preserves accessible belongings with at least 200 nonempty corpses/piles represented over its course.
- [ ] **REL-06**: Recorded solo/four-player playthroughs and tuning support the intended approximately 2–3-hour, 8–12-day default pacing.

## v2 Requirements

Future platform work acknowledged by the specification; no delivery date or next-release commitment is implied. The replaceable service boundary itself is already required by LAN-04.

- **PLAT-01**: Players can use a Steam service implementation for supported discovery/invitations, identity, and connections while retaining the same gameplay rules.
- **PLAT-02**: Players can use an Epic service implementation for supported discovery/invitations, identity, and connections while retaining the same gameplay rules.

## Out of Scope

| Feature | Reason |
| --- | --- |
| Extra maps/islands, travel, procedural terrain, rival camps | The release contains one authored map and shared camp. |
| External recruitment camps and homeless encounters | Recruitment occurs only at the main building. |
| Platform accounts/integration, online matchmaking, host migration | LAN and a replaceable service boundary are the current contract. |
| Split-screen, controllers, PvP, friendly damage | Keyboard/mouse cooperative play on separate computers is specified. |
| Firearms, crossbows, shields, parries, rolls, mounts | The specified melee/bow combat scope excludes these. |
| Classes, experience levels, skill trees | Progression is camp, equipment, and wave progression. |
| Hunger, thirst, infection, detailed injuries, aging, reproduction | The specified survival model excludes these systems. |
| Crafting, raw materials, rags, cooking, equipment shops/repair | The economy uses physical money, food sales, paid work, and real loot. |
| Recoverable fired arrows and gameplay acid | Arrows are spent; acid effects are cosmetic. |
| Tutorial, music, voice acting, additional shipped locales | Required labels, feedback, controls reference, effects/ambience, and English translation-ready text remain in scope. |
| In-game balance editor and live reload | Designers edit documented files applied on start/load. |
| Pausing active play, automatic treasury payments, player-count difficulty scaling | These conflict with explicit shared-world, ownership, and difficulty rules. |
| Destructive overflow migration, nonempty loot expiration, death loot rerolls | Earned belongings and existing state must be preserved. |
| A direct Unity runtime port or copied LangProj web/database architecture | Reuse must satisfy this game's Godot and specification boundaries. |

## Acceptance Criteria

The full source scenarios remain at [specification §14.3](../specs/0.1/initial-spec.md#143-observable-acceptance-scenarios). This crosswalk preserves their IDs and maps them to the requirements they exercise. Each umbrella scenario needs its own normal, boundary, failure, and concurrency checks where applicable; a token appearing in this table is not execution evidence.

| Source case | Related requirement IDs |
| --- | --- |
| A-01 | CHAR-01, RUN-01, WORLD-01, WORLD-03, BUILD-02 |
| A-02 | WORLD-03, ZOMB-04, ZOMB-05, ZOMB-06 |
| A-03 | WORLD-03, ZOMB-03, DATA-01 |
| A-04 | INV-12, RUN-01, RUN-02, WORLD-06, COMBAT-08 |
| A-05 | RUN-02, RUN-03, RUN-04 |
| A-06 | RUN-03, RUN-04, NPC-01, NPC-02 |
| A-07 | RUN-05 |
| A-08 | RUN-06, ZOMB-07, ZOMB-08 |
| A-09 | RUN-05, RUN-06, RUN-07 |
| A-10 | NPC-02, NPC-03, BUILD-02 |
| A-11 | NPC-01 |
| A-12 | NPC-03, NPC-07 |
| A-13 | NPC-04, INV-06, COMBAT-08 |
| A-14 | COMBAT-04, COMBAT-05, NPC-05 |
| A-15 | NPC-06, NPC-08, WORLD-05 |
| A-16 | NPC-06, NPC-07, ARMY-03, WORLD-03 |
| A-17 | BUILD-01, BUILD-02, BUILD-06, WORLD-01, WORLD-02, WORLD-05 |
| A-18 | BUILD-02, BUILD-03, BUILD-04, BUILD-07, NPC-02, ASSET-03 |
| A-19 | BUILD-08, ASSET-03 |
| A-20 | BUILD-03, BUILD-04, BUILD-05, BUILD-06, ECON-01 |
| A-21 | BUILD-07 |
| A-22 | INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, CHAR-06, COMBAT-02, COMBAT-03 |
| A-23 | CHAR-02, CHAR-03, CHAR-06, COMBAT-05 |
| A-24 | INV-08, INV-09, WORLD-07, UX-01, UX-02 |
| A-25 | INV-10, NPC-08, ECON-02 |
| A-26 | INV-01, INV-06, INV-11, INV-12, NPC-08, COMBAT-03, COMBAT-04 |
| A-27 | INV-13 |
| A-28 | INV-11, ECON-01, ECON-02, NPC-01, NPC-02, BUILD-03, SESSION-04 |
| A-29 | ECON-01, ECON-02, INV-10, NPC-05 |
| A-30 | WORLD-04, ECON-03, ECON-04, ECON-05, SAVE-01, SAVE-03 |
| A-31 | COMBAT-03, COMBAT-04, COMBAT-06, BUILD-07, BUILD-08 |
| A-32 | ZOMB-01, ZOMB-02, COMBAT-04, COMBAT-05, COMBAT-07, COMBAT-08, INV-12 |
| A-33 | WORLD-02 |
| A-34 | ZOMB-05, ZOMB-06, UX-06 |
| A-35 | ZOMB-04, ZOMB-05, ZOMB-06, ZOMB-07, ZOMB-08, WORLD-04 |
| A-36 | ZOMB-08, INV-13 |
| A-37 | ARMY-01, ARMY-02, ARMY-03, ECON-01 |
| A-38 | LAN-01, LAN-02, LAN-03, LAN-04, SESSION-01, SESSION-02, SESSION-03, DATA-03, REL-02 |
| A-39 | SESSION-05, RUN-02, RUN-03, RUN-04, NPC-02 |
| A-40 | SESSION-06, SAVE-01, SAVE-04, SAVE-05 |
| A-41 | SAVE-03, COMBAT-08, INV-01, INV-06, INV-12, INV-13, BUILD-03, ZOMB-07 |
| A-42 | SESSION-04, SAVE-06, DATA-01, DATA-02 |
| A-43 | SAVE-07, SAVE-08, INV-01, INV-04, INV-05, INV-06 |
| A-44 | SAVE-04, SAVE-05, DATA-03 |
| A-45 | WORLD-06, RUN-02, RUN-04, SESSION-03, SAVE-03, UX-03 |
| A-46 | DATA-04, UX-01, UX-02, UX-03, UX-04, UX-05, UX-06, UX-07 |
| A-47 | REL-01, REL-02, REL-03, REL-04, REL-05, REL-06, ASSET-01, ASSET-02, ASSET-03, ASSET-04, ASSET-05, TEST-01, TEST-02, TEST-03, TEST-04, TEST-05, TEST-06, TEST-07, TEST-08, TEST-09, TEST-10 |

Additional cases must cover RULE-01–RULE-07, BASE-01–BASE-03, LAN-01–LAN-04, ASSET-01–ASSET-05, TEST-01–TEST-10, and source behavior not isolated by one umbrella case, including safe health/food recovery (CHAR-04/CHAR-05), single melee attacks (COMBAT-01), and save selection/retention (SAVE-02). These are foundation/content/regression cases alongside A-01–A-47, not replacements or renumbering.

**A-08 interpretation:** Its phrase “survivors do not retreat” is inconsistent with the explicit zombie rule in §§2.3 and 9.4. RUN-06/ZOMB-08 preserve that unambiguous mechanic: remaining zombies stop dawn retreat after the final portal is destroyed. Ordinary civilian retreat rules are unchanged. The source specification has not been edited.

### Test-case lifecycle

- Phase 1 proves the rules; Phases 2 and 3 have their own smoke checks. Every phase saves AI-readable scenarios with importance, reproducible setup/actions/expected outcomes, dependencies and ordered game-run groups.
- Phase 5 registers the full catalog, implements the framework, and requires tests for already delivered foundations. Future feature cases have an explicit planned state and owning phase; they do not count as passed.
- Ordinary PRs use the basic ten-minute-target selection above. Expensive behavior, rendered-input, LAN, checker-defect and coverage work remains available through focused/full verification; a basic PR pass does not certify that broader scope.
- Phase 5 implements folder/dependency-aware selection and evidence reuse. Completion may combine fresh affected-group results with immutable matching prior results after validating relevant source/assets, dependencies, test/case definitions, tools and required environments. Identify reuse explicitly, rerun missing/invalidated groups and dependents, and block unknown impact. Every selected obligation needs valid evidence; omissions remain explicit without pass credit. The immediate phase-end minimum is all active Critical cases plus every new/changed case and affected regression. Repairs require focused checks followed by a fresh entire-selected-set rendered pass. Broad shared changes can select the full scope again.
- Feature implementation activates the corresponding required cases and supplies executable checks or an explicit repeatable evidence procedure as appropriate. Phase 11 adds scheduled full active-catalog execution across all importance levels; manual full execution remains available.
- Required checks cannot be silently skipped. Missing tools, incompatible runtimes, unavailable required environments, absent reports/images, and zero required-case execution are failures or reported blockers, not successful verification.
- Headless state tests, rendered input/UI evidence, four local processes, actual mixed-OS LAN, and rendered reference-PC measurements establish different facts. A passing result in one does not substitute for the others.

## Definition of Done

A requirement is complete only when implemented, verified against the full source behavior, and committed with its evidence and relevant cases. Assets must work in the appropriate fixtures and, for gameplay acceptance, in the integrated game. Saveable state, atomic authority, stable identities, per-feature configuration, and regression cases are developed with each feature rather than deferred wholesale to Phase 9.

Release additionally requires every gate in [specification §14](../specs/0.1/initial-spec.md#14-definition-of-a-finished-01-release): unaided solo/four-player victories, required content and outcomes, full sessions and persistence, documented configuration, usable presentation, runnable desktop delivery, recorded pacing, and no known release-blocking defect.

The reference-PC stress procedure uses 1920x1080, four participants, approximately 100 survivors and 300 active zombies, full required behavior and equipment visuals, ten minutes of representative activity, at least 30 FPS in 95% of one-second samples, no sub-30 stretch longer than five seconds, and no active-play save/join freeze longer than 0.5 seconds. Record hardware, OS, configuration, and raw results. A three-hour saved/resumed endurance run must retain accessible belongings with at least 200 nonempty corpses/piles represented over its course. Accelerated fixtures and hosted CI do not replace these specified empirical checks.

## Traceability

All 134 version 0.1 requirements have exactly one primary owning phase in [ROADMAP.md](ROADMAP.md). The creator approved these requirements, working-increment phase planning, and the detailed roadmap on 2026-11-13. The fixed first-five-phase order and accepted ten-phase direction are recorded in [PROJECT.md](PROJECT.md#agreed-delivery-direction). Earlier prerequisites and later integration/regression checks do not duplicate ownership: the assigned phase must deliver complete requirement acceptance. All implementation statuses remain Pending.

| Requirement | Phase | Status |
| --- | --- | --- |
| RULE-01 | Phase 1 | Complete |
| RULE-02 | Phase 1 | Complete |
| RULE-03 | Phase 1 | Complete |
| RULE-04 | Phase 1 | Complete |
| RULE-05 | Phase 1 | Complete |
| RULE-06 | Phase 1 | Complete |
| RULE-07 | Phase 1 | Complete |
| BASE-01 | Phase 2 | Pending |
| BASE-02 | Phase 2 | Pending |
| BASE-03 | Phase 2 | Pending |
| LAN-01 | Phase 3 | Pending |
| LAN-02 | Phase 3 | Pending |
| LAN-03 | Phase 3 | Pending |
| LAN-04 | Phase 3 | Pending |
| ASSET-01 | Phase 4 | Pending |
| ASSET-02 | Phase 4 | Pending |
| ASSET-03 | Phase 4 | Pending |
| ASSET-04 | Phase 4 | Pending |
| ASSET-05 | Phase 4 | Pending |
| TEST-01 | Phase 5 | Pending |
| TEST-02 | Phase 5 | Pending |
| TEST-03 | Phase 5 | Pending |
| TEST-04 | Phase 5 | Pending |
| TEST-05 | Phase 5 | Pending |
| TEST-06 | Phase 5 | Pending |
| TEST-07 | Phase 5 | Pending |
| TEST-08 | Phase 5 | Pending |
| TEST-09 | Phase 5 | Pending |
| TEST-10 | Phase 5 | Pending |
| CHAR-01 | Phase 6 | Pending |
| CHAR-02 | Phase 6 | Pending |
| CHAR-03 | Phase 6 | Pending |
| CHAR-04 | Phase 6 | Pending |
| CHAR-05 | Phase 6 | Pending |
| CHAR-06 | Phase 6 | Pending |
| INV-01 | Phase 6 | Pending |
| INV-02 | Phase 6 | Pending |
| INV-03 | Phase 6 | Pending |
| INV-04 | Phase 6 | Pending |
| INV-05 | Phase 6 | Pending |
| INV-06 | Phase 6 | Pending |
| INV-07 | Phase 6 | Pending |
| INV-08 | Phase 6 | Pending |
| INV-09 | Phase 6 | Pending |
| INV-10 | Phase 6 | Pending |
| INV-11 | Phase 6 | Pending |
| INV-12 | Phase 6 | Pending |
| INV-13 | Phase 6 | Pending |
| COMBAT-01 | Phase 6 | Pending |
| COMBAT-02 | Phase 6 | Pending |
| COMBAT-03 | Phase 6 | Pending |
| COMBAT-04 | Phase 8 | Pending |
| COMBAT-05 | Phase 6 | Pending |
| COMBAT-06 | Phase 6 | Pending |
| COMBAT-07 | Phase 8 | Pending |
| COMBAT-08 | Phase 6 | Pending |
| RUN-01 | Phase 6 | Pending |
| RUN-02 | Phase 6 | Pending |
| RUN-03 | Phase 6 | Pending |
| RUN-04 | Phase 6 | Pending |
| RUN-05 | Phase 8 | Pending |
| RUN-06 | Phase 8 | Pending |
| RUN-07 | Phase 8 | Pending |
| WORLD-01 | Phase 8 | Pending |
| WORLD-02 | Phase 8 | Pending |
| WORLD-03 | Phase 8 | Pending |
| WORLD-04 | Phase 9 | Pending |
| WORLD-05 | Phase 7 | Pending |
| WORLD-06 | Phase 9 | Pending |
| WORLD-07 | Phase 9 | Pending |
| ECON-01 | Phase 7 | Pending |
| ECON-02 | Phase 7 | Pending |
| ECON-03 | Phase 7 | Pending |
| ECON-04 | Phase 7 | Pending |
| ECON-05 | Phase 7 | Pending |
| NPC-01 | Phase 7 | Pending |
| NPC-02 | Phase 7 | Pending |
| NPC-03 | Phase 7 | Pending |
| NPC-04 | Phase 7 | Pending |
| NPC-05 | Phase 7 | Pending |
| NPC-06 | Phase 7 | Pending |
| NPC-07 | Phase 7 | Pending |
| NPC-08 | Phase 7 | Pending |
| BUILD-01 | Phase 7 | Pending |
| BUILD-02 | Phase 7 | Pending |
| BUILD-03 | Phase 7 | Pending |
| BUILD-04 | Phase 7 | Pending |
| BUILD-05 | Phase 7 | Pending |
| BUILD-06 | Phase 7 | Pending |
| BUILD-07 | Phase 7 | Pending |
| BUILD-08 | Phase 7 | Pending |
| ZOMB-01 | Phase 8 | Pending |
| ZOMB-02 | Phase 8 | Pending |
| ZOMB-03 | Phase 8 | Pending |
| ZOMB-04 | Phase 8 | Pending |
| ZOMB-05 | Phase 8 | Pending |
| ZOMB-06 | Phase 8 | Pending |
| ZOMB-07 | Phase 8 | Pending |
| ZOMB-08 | Phase 8 | Pending |
| ARMY-01 | Phase 8 | Pending |
| ARMY-02 | Phase 8 | Pending |
| ARMY-03 | Phase 8 | Pending |
| SESSION-01 | Phase 9 | Pending |
| SESSION-02 | Phase 9 | Pending |
| SESSION-03 | Phase 9 | Pending |
| SESSION-04 | Phase 9 | Pending |
| SESSION-05 | Phase 9 | Pending |
| SESSION-06 | Phase 9 | Pending |
| SAVE-01 | Phase 9 | Pending |
| SAVE-02 | Phase 9 | Pending |
| SAVE-03 | Phase 9 | Pending |
| SAVE-04 | Phase 9 | Pending |
| SAVE-05 | Phase 9 | Pending |
| SAVE-06 | Phase 9 | Pending |
| SAVE-07 | Phase 9 | Pending |
| SAVE-08 | Phase 9 | Pending |
| DATA-01 | Phase 9 | Pending |
| DATA-02 | Phase 9 | Pending |
| DATA-03 | Phase 9 | Pending |
| DATA-04 | Phase 9 | Pending |
| UX-01 | Phase 9 | Pending |
| UX-02 | Phase 9 | Pending |
| UX-03 | Phase 9 | Pending |
| UX-04 | Phase 9 | Pending |
| UX-05 | Phase 9 | Pending |
| UX-06 | Phase 9 | Pending |
| UX-07 | Phase 9 | Pending |
| REL-01 | Phase 10 | Pending |
| REL-02 | Phase 10 | Pending |
| REL-03 | Phase 10 | Pending |
| REL-04 | Phase 10 | Pending |
| REL-05 | Phase 10 | Pending |
| REL-06 | Phase 10 | Pending |
| TEST-11 | Phase 11 | Pending |

**Coverage:**
- Version 0.1 requirements: 134 total
- Mapped to phases: 134 (exactly one primary owner each)
- Unmapped: 0
- Duplicate primary owners: 0
- Unknown version 0.1 IDs in roadmap: 0
- Implementation statuses: 127 Pending; 7 complete
- Source acceptance scenarios linked: 47 of 47
- Future platform requirements: 2 (outside the current roadmap)

---
*Requirements defined: 2026-11-13*
*Last updated: 2026-11-15 after the local testing discussion and Phase 11/TEST-11 addition*
