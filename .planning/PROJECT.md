# KingdomZombies

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. The complete-other-OS-VM rule continues for later phases.

**Current platform policy (creator instruction, 2026-11-14):** Agents use the development OS for implementation checks. Before phase completion, its complete required scope must also pass in a local VM running the other supported OS. Use hardware acceleration when available. This supersedes earlier representative-only Phase 1 Linux timing; coverage and the eight explicit Phase 5 declarations retain their ownership. See [AI collaboration](../docs/workflow/ai-collaboration.md).

## What This Is

KingdomZombies is a side-view, pixel-art survival and camp-management game inspired by Kingdom Two Crowns, built with Godot and C#. It is for solo players and groups of up to four cooperative LAN players on Windows and Linux: recruit and equip survivors, develop one shared camp, survive zombie attacks, and destroy six portals. Version 0.1 is the complete playable, saveable game defined by the existing specifications, delivered through a sequence of working foundations and game features.

## Core Value

Players can build, equip, and defend a shared survivor camp through a complete solo or four-player LAN campaign, with consistent ownership of their survivors, items, and progress.

## Scope Authority

- [Version 0.1 game specification](../specs/0.1/initial-spec.md) defines mandatory behavior, exclusions, proposed balance, release gates, and acceptance scenarios A-01 through A-47.
- [Asset and resource production plan](../specs/0.1/assets-and-resources.md) defines required content, reusable donors, evidence levels, and production checks. It supplements the game specification; it does not override it.
- The creator confirmed on 2026-11-13 that the project must deliver the full specified version 0.1, use Godot and C#, and follow the first five phases recorded below. The creator then approved the proposed ten-phase direction and creation of this project definition.
- The creator subsequently approved all 133 version 0.1 requirements, working-increment phase planning, and the detailed ten-phase roadmap on 2026-11-13. Each phase ends with a verifiable result; initialization is complete and Phase 1 is ready for discussion and planning.
- Confirmed behavior and exclusions are binding. Values and supporting rules labeled proposed in the specifications remain the working baseline to implement and evaluate; approval of this project does not turn untested balance into proven balance.
- GSD's initial-release requirements will describe version 0.1. A template heading such as `v1 Requirements` must not be interpreted as a separate version 1.0 release or a reduced game scope.

## Requirements

### Validated

(None yet — the repository contains specifications, references, and a starter Godot project, but no implemented gameplay has been verified.)

### Active

- [ ] Establish strict, enforced C# and architecture rules adapted from LangProj, with a rule matrix, scoped Godot compatibility decisions, and positive and negative proof tests.
- [ ] Create a documentation tree with a short root `AGENTS.md` containing the project description and links to focused documents; agents load only documentation relevant to their task.
- [ ] Deliver an initial menu-to-game flow with a small scene, a few base assets, and one character moving left and right.
- [ ] Extend that same foundation to a host and up to three LAN guests, each independently controlling a character and observing consistent movement.
- [ ] Produce the required asset and resource pack by reusing suitable recovered Kingdom Two Crowns content first, adapting it where necessary, and generating the missing content.
- [ ] Establish GitHub Actions and matching local verification commands, including a stored test-case catalog and repeatable end-to-end testing that an AI agent can run and inspect after changes.
- [ ] Deliver one handcrafted map with six portals, exploration, shared minimap knowledge, day/night transitions, camp borders, and the specified daily events.
- [ ] Implement survivors, movement, health, stamina, recovery, load penalties, melee and bow combat, armor, durability, and actual ammunition consumption.
- [ ] Implement the shared item model: 24 baseline item definitions, grid inventories, equipment slots, stacking, rotation, nested containers, physical money, unlimited paged shared stash, and immediate inventory interactions.
- [ ] Keep actual belongings on corpses and loot piles, preserve nonempty loot indefinitely, and prevent item duplication or loss across transfers, death, hauling, multiplayer, and saving.
- [ ] Implement eight starting survivors, recruitment at the main building, four assigned NPC roles plus unassigned workers, equipment selection, safe work, hauling, food gathering, hunting, and the food-sale economy.
- [ ] Implement paid tree clearing, construction, upgrades, repairs, rebuilding, three main-building/wall/tower tiers, standalone tower guards, and the main building's two single-archer attachments.
- [ ] Implement all seven zombie archetypes with real inventories, legal generated equipment, independently configurable progression, scheduled attacks, portal defenses, retaliation, spawn queues, and dawn retreat.
- [ ] Implement warrior defense allocation, paid rally assaults, recall, and return behavior, without losing or teleporting survivors and equipment.
- [ ] Complete succession, spectating, both defeat conditions, immediate victory after the last portal, and continued play with remaining enemies.
- [ ] Complete Windows/Linux LAN discovery, manual joining, late join, reconnection, simultaneous-action resolution, and host-loss behavior under one authoritative host.
- [ ] Keep multiplayer service concerns separate from gameplay so later Steam/Epic discovery, identity, and connection services can be added without rewriting game rules.
- [ ] Implement coherent manual/dawn/exit saves, multiple saved runs, safe failure recovery, host configuration changes on start/load, and visible/removable legacy inventory layouts without silently deleting belongings.
- [ ] Ship separate, documented, validated configuration domains, including an independently editable zombie-progression file and complete stable content identifiers.
- [ ] Complete the specified interface, controls/remapping, equipment appearances, effects, sound and ambience, readable UI scaling, and English translation catalog.
- [ ] Verify complete solo and four-player campaigns, Windows/Linux runnable exports, default pacing, reference-PC performance, endurance, and all release acceptance scenarios.

### Out of Scope

- Additional maps/islands, map travel, procedural terrain, competing camps, external recruitment camps, and homeless recruitment encounters — version 0.1 has one authored camp/map and main-building recruitment.
- Online matchmaking, Steam/Epic integration, platform accounts, host migration, split-screen, controller support, PvP, and friendly damage — version 0.1 is keyboard/mouse cooperative LAN play, one computer per player; only the future service boundary is included.
- Firearms, crossbows, shields, parrying, combat rolls, mounts, character classes, experience levels, and skill trees — excluded by the specified combat and progression scope.
- Hunger, thirst, infection, detailed injuries, aging, reproduction, crafting, raw-material harvesting, clothing-to-rag conversion, cooking, equipment shops, and equipment repair — excluded by the specified economy and survivor rules.
- Recoverable fired arrows and gameplay acid hazards — arrows are spent; bloater acid is cosmetic.
- Tutorial sequences, music, voice acting, and additional shipped languages — readable labels, feedback, controls reference, effects/ambience, and translation readiness remain required.
- An in-game balance editor and live configuration reload — designers edit documented files, applied on new-run start or load.
- Copying LangProj's web-service/database architecture wholesale or retaining Unity-specific runtime dependencies — references must be adapted to Godot and this game's needs.
- Reducing version 0.1 to a movement demo or treating an isolated feature demonstration as a finished release — the early foundations are intermediate deliverables only.

## Context

### Current repository

- Workspace: `D:/Work/Games/KingdomZombies`.
- Game project: `src/godot-game/project.godot`; inspected starter settings declare `KingdomZombies`, the `4.7`/`Forward Plus` feature tags, and a .NET assembly name. Those settings do not prove an installed or validated engine/compiler combination.
- The inspected game source consists of the Godot project settings and icon, with no C# gameplay, scenes, solution, or test runner yet.
- Visual examples are under `examples/`, including three base references and zombie artwork. References guide style; written specifications control silhouettes and behavior.
- The root `Agents.md` now provides a concise bootstrap description and documentation index. Phase 1 must establish the full focused rule documentation and consistent filename casing without creating case-colliding files on Windows/Linux.
- GSD initialization now includes approved context, workflow settings, research, 133 requirements, a ten-phase roadmap, and current state. No completed gameplay capability is being migrated. The asset register describes production work still to do; it is not evidence that the pack has already been exported or integrated.

### LangProj inheritance

Reference repository: `D:/Work/SaaS/LangProj_monorepo`.

Inspected entry points and implementation resources:

- `AGENTS.md` and `src/backend/AGENTS.md`: short descriptions, focused indexes, lazy document loading, and separation between repository rules and tool instructions.
- `src/backend/docs/CodeRules/code-style.md` and `code-architecture.md`: style conventions, dependency discipline, type rules, documentation, and test expectations.
- `src/backend/docs/rule-matrix.md` and `docs/decisions/0003-enforcement-approach.md`: explicit enforcement, scoped exemptions, and tests proving both compliant and noncompliant cases.
- `src/backend/lang-proj-core/.editorconfig`, `Directory.Build.props`, `Directory.Build.targets`, and `BannedSymbols.txt`: build-time settings and analyzer integration.
- `packages/Sebreiro.CodeRules.Analyzers` and its tests: reusable Roslyn rules for named arguments, body comments, primary constructors, regions, null-forgiving operators, body/line length, member ordering, continuation indentation, using ordering, `this.` qualification, and local `new` declarations.
- `LangProject.Core.ArchitectureTests`: project/dependency, documentation/index, format, suppression, rule-matrix, and real-build proof patterns.

Inherit the applicable C# rules and the enforcement philosophy. Phase 1 must record which source rules are retained, adapted, or inapplicable, with reasons and proof checks. It must check compatibility with Godot-generated partial code, engine callbacks, scene/resource construction and serialization, node lifetime, main-thread access, nullable initialization, and the chosen SDK/compiler/analyzer versions. Do not import HTTP/EF-specific projects, constructors, test hosts, or registration rules simply because they exist in LangProj. Do not solve incompatibilities with blanket warning suppression or hidden exceptions.

### Recovered game content and logic

Reference root: `D:/Work/Games/KindomCrown/recovered`.

The creator expressly permits reuse of the supplied assets and game logic. Reuse suitable assets and algorithms as much as practical. The source includes Unity-based code and original-game rules, so any reused logic needs a Godot adaptation and comparison with this specification, including four-player support, physical inventories, recruitment, structural tower guards, permanently destroyed portals, and outcomes.

Preserve original recovered files. Track source identity, transformations, generated gaps, stable output identifiers, and validation evidence. The asset document distinguishes visual samples, decoded metadata, and catalog-only candidates; those evidence levels must not be represented as finished production validation. Its 180 resource obligations may share outputs and are not a count of individual image files.

### Agreed delivery direction

The first five phases and their order are creator requirements. The creator approved five further phases, the detailed ten-phase roadmap, and complete requirement mapping on 2026-11-13. See [ROADMAP.md](ROADMAP.md) for completion criteria and [REQUIREMENTS.md](REQUIREMENTS.md#traceability) for primary ownership.

| Phase | Working title | Intended outcome |
| --- | --- | --- |
| 1 | Code rules and documentation | Enforced Godot-compatible rules, proof tests, rule matrix, architecture decisions, and concise documentation indexes. |
| 2 | Playable foundation | Main menu starts a small scene with a few base assets and a character that moves left/right. |
| 3 | Four-player LAN foundation | Host and guests connect to that same scene and control independent synchronized characters. |
| 4 | Asset production | Required content is recovered/adapted/generated, identified, documented, imported, and inspectable in fixtures. |
| 5 | CI and repeatable game testing | GitHub Actions and local runners execute stored cases and provide results and diagnostic evidence an AI agent can use. |
| 6 | Survivors, inventory, and combat | Players manage real belongings, fight, die, and succeed to another survivor; ownership and persisted state are designed in. |
| 7 | Camp economy and development | Recruitment, jobs, food, wildlife, hauling, building, repair, upgrades, and safe territory form a working camp. |
| 8 | Zombie campaign and outcomes | Zombie roster, waves, progression, portals, retaliation, assaults, victory, and defeat complete the campaign loop. |
| 9 | Persistence and complete player experience | Full saves, configuration compatibility, session recovery, remaining interface, audio, and translation readiness work together. |
| 10 | Release validation and balancing | Full campaigns, cross-platform delivery, performance/endurance evidence, and pacing satisfy the specified release gates. |

Phase 1 includes tests that prove the code rules. Phases 2 and 3 include basic launch, movement, and connection smoke checks; the full game-test infrastructure arrives in Phase 5. Network consistency, stable identities, serializable state, configuration, and verification are considered as each feature is built. Phase 9 completes the save/session experience rather than introducing persistence assumptions for the first time. Phase 4 produces the content and fixtures; later gameplay phases still integrate and verify that content in actual play.

### Agent-operated verification

Repeatable testing after changes is a primary development requirement, not an optional finishing task.

- Store readable scenarios and executable tests in the repository. Map the spec's A-01 through A-47 IDs to requirements, implementation phases, tests, and evidence; add foundation and regression cases without renumbering the source acceptance scenarios.
- Define a documented agent procedure to select relevant checks, execute them, inspect results/logs/images, reproduce failures, fix issues, and rerun required verification.
- Use testable gameplay rules, controlled setup/randomness/time where appropriate, scene/input tests, and real host-plus-three-client process tests. End-to-end cases must exercise production behavior, not merely assert test-only substitutes.
- Include visible menu/input/rendering checks alongside simulation and network assertions. A headless pass alone cannot establish correct artwork, input focus, readability, or rendering performance.
- Use consistent local and CI entry points, bounded waits, reliable cleanup, diagnostic artifacts, and clear process exit status. A missing engine, undiscovered test, skipped required case, timeout, or unavailable environment must not appear as a passing check.
- At Phase 5, register the full release case catalog and automate the foundations already implemented. Mark future feature cases as planned with an owning phase; activate them as features arrive. Before release, all required cases need their specified evidence.
- Hosted CI and four local processes provide useful checks but do not alone prove a real mixed Windows/Linux LAN session or the creator-PC rendering target. Retain repeatable procedures and evidence for those environments, including AI-operated visual checks where appropriate.
- Update cases, automation, and results alongside behavior changes; do not treat a one-time manual playthrough or code coverage alone as acceptance coverage.

## Constraints

- **Scope:** Complete version 0.1 from the supplied specifications; no silently dropped mandatory systems. Changes to confirmed behavior/exclusions require an explicit recorded scope decision.
- **Technology:** Godot with C#. Pin and prove a compatible engine, .NET SDK, compiler, analyzers, testing tools, and export templates; inherited .NET 10/latest-language settings are reference inputs until compatibility is verified.
- **Platforms/input:** Windows and Linux, keyboard/mouse, one computer per player, solo or up to four cooperative LAN participants including the host/spectators. No account service is required.
- **Architecture:** Host-authoritative outcomes, clear gameplay/engine/service boundaries, stable item/player/content identity, and replaceable multiplayer services. Exact project layout and interfaces remain Phase 1 design work.
- **Ownership/persistence:** No item/money duplication, silent loss, double payment, duplicated daily/wave events, or resurrection through joining/loading. Nonempty corpses persist; incompatible saved layouts remain visible and removable.
- **Content:** One handcrafted map, six portals, seven zombie archetypes, 24 baseline items, two wildlife types, four assigned NPC roles, three main/wall/tower tiers, and all required presentation/resources.
- **Default run:** Eight starting survivors, 600-second days, 300-second nights, one scheduled-wave-free opening night, escalation every three days, and no player-count difficulty scaling. Unconfirmed baseline quantities remain configurable and subject to playtesting.
- **Performance:** Target approximately 100 survivors and 300 active zombies in four-player play at 1920x1080 on the creator's reference PC, with the specification's 30-FPS sampling and save/join responsiveness checks. Record actual hardware and measurements; no benchmark has been completed.
- **Endurance/pacing:** Evaluate approximately 2–3 hours and 8–12 days per successful default campaign. Run the specified three-hour saved/resumed endurance session with at least 200 nonempty corpses/loot piles represented over its course.
- **Presentation:** Side-view pixel art, visible equipped items, English catalog with translation-ready layouts, remappable controls, readable 1280x720 and 1920x1080 UI, sound effects and ambience, and visible critical warnings when muted.
- **Configuration:** Separate documented domains with a complete baseline; zombie progression in its own file. Validate invalid values/references explicitly; apply host balance only on new-run start/load and preserve saved commitments.
- **Quality:** Strict rules must be executable and proven while permitting correct Godot development. GitHub Actions and local tests must support reliable AI-driven regression checking.
- **Dependencies:** External reference folders are source inputs, not permanent runtime or CI dependencies. Selected resources and reusable code must become reproducible, self-contained project inputs with documented origins.
- **Schedule/budget:** No deadline, budget, monetization plan, or exact reference-PC specification has been supplied; none is assumed by this initialization.

## Key Decisions

| Decision | Rationale | Outcome |
| --- | --- | --- |
| Deliver the full specified version 0.1 | The creator explicitly selected the complete game; early movement/network scenes are foundations. | — Pending implementation |
| Use Godot and C# | The chosen project and requested inheritance of C# rules establish the technical direction. | — Pending toolchain validation |
| Preserve Phases 1–5 in the requested order | Rules, a visible foundation, LAN, assets, then comprehensive CI/testing precede the main gameplay build. | — Approved and mapped; pending delivery |
| Use five further phases, for ten total | The accepted direction separates survivors/combat, camp, campaign, persistence/experience, and release proof. | — Detailed roadmap approved; pending delivery |
| Plan phases as working increments | The creator selected a verifiable result per phase; the GSD mode is `mvp`, without reducing the complete version 0.1 release scope. | — Approved direction; pending delivery |
| Inherit applicable LangProj rules with documented Godot adaptations | Strong consistency and enforcement must coexist with the engine lifecycle and generated code. | — Pending compatibility proofs |
| Keep root `AGENTS.md` short and use a documentation index tree | Agents and developers should find focused rules without loading every document. | — Pending Phase 1 |
| Reuse recovered assets and suitable logic first | The creator authorizes reuse; the asset plan identifies concrete donors and known gaps. | — Pending export/adaptation verification |
| Store acceptance cases and automate repeated verification | An AI agent must be able to test changes, reproduce failures, and inspect evidence. | — Pending Phase 5 framework |
| Start basic tests in the foundation phases | The rules and early playable/network foundations must be demonstrably usable before the full CI phase. | — Pending implementation |
| Simplify Phase 1 completion; move certification to Phase 5 | Preserve the working rules/tests and representative clean Windows/Linux checks; deliver measured 90%/80% coverage, exhaustive platform suites and unfinished aggregate runner certification with CI/testing. | — Creator approved 2026-11-14; [handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md); implementation remains pending |
| Design authority, identity, and saveable state throughout | Later persistence and LAN correctness cannot rely on retrofitting incompatible gameplay state. | — Pending architecture and tests |
| Preserve spec precedence over reference-game behavior | Unity assumptions, two-player behaviors, and excluded mechanics must not leak into this game. | — Pending reuse reviews |
| Keep proposed balance explicitly unproven | The supplied numbers are a reproducible baseline; playthroughs must establish pacing and viability. | — Pending playtesting |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition:**
1. Move invalidated requirements to Out of Scope with a reason.
2. Move verified requirements to Validated with the phase and evidence reference.
3. Add newly agreed requirements to Active.
4. Record decisions and update their outcomes.
5. Update What This Is and current context if the project has changed.
6. Keep the specification/requirement/case mappings and the testing procedure consistent with implemented behavior.

**After each milestone:**
1. Review every section against the delivered game and its evidence.
2. Check whether the Core Value still expresses the highest priority.
3. Review Out of Scope and its reasons.
4. Update current repository, platform, content, test, and release context.
5. Record balance changes and explicitly distinguish confirmed scope changes from tuning.

---
*Last updated: 2026-11-13 after final roadmap approval and completed project initialization*
