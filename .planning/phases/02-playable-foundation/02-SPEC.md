# Phase 2: Playable foundation — Specification

**Created:** 2026-11-13 (Europe/Paris)
**Ambiguity score:** 0.063 (gate: ≤ 0.20; exact score 0.0625)
**Requirements:** 3 locked — BASE-01, BASE-02, BASE-03
**Status:** Requirements locked; implementation and verification pending.

## Goal

A player can launch a Start/Quit menu, enter a basic scene containing a small decorative main building, colored background, ground, and one player, walk left and right, and return to the menu with Escape, with repeatable passing smoke checks on Windows and Linux.

## Background

At the codebase scout, `src/godot-game/` contained `project.godot`, its icon/import metadata, and a UTF-8 `.editorconfig`. There was no C# project, playable scene, gameplay script, input/camera implementation, or foundation smoke runner. The starter engine/rendering feature tags are declarations, not compatibility evidence. Phase 1 owns the proven toolchain, code rules, architecture boundaries, and minimal engine proof fixtures; its planning artifacts do not establish completed implementation.

The creator explicitly narrowed this increment to a basic project: a small main building and one player against colored background and ground. Proper production assets are unnecessary. A static player with a visible facing direction is sufficient, including a simple direction marker when using placeholder art. The building is scenery that the player can walk past. This same playable scene is the foundation for Phase 3 LAN movement; it is an intermediate increment toward the full game.

Scope authority: [requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md), and [project definition](../../PROJECT.md). The [game specification](../../../specs/0.1/initial-spec.md), particularly §§4.1 and 13, supplies the working walking/input baseline. The [asset plan](../../../specs/0.1/assets-and-resources.md) governs later content production; reference images and donor samples are not required production assets for this increment.

## Requirements

1. **BASE-01 — Menu and scene lifecycle:** Players can launch into a main menu with Start and Quit, start exactly one instance of the basic scene, and return directly to the main menu with Escape.
   - Current: No main scene, menu, Start/Quit actions, or playable-scene lifecycle exists.
   - Target: Start enters one fresh scene with one player at its starting position. Escape returns directly to the main menu. Repeated or held activation causes one transition per press. Previous scene controls are removed, held movement does not carry into the next scene, and Quit closes the game cleanly.
   - Acceptance: AC-01 and AC-07–AC-09 pass through the ordinary player-facing controls on both operating systems.

2. **BASE-02 — Basic scene and walking:** Players can move one visible, direction-facing static player left and right through a small scene containing a small decorative main building, colored background, and ground; placeholder visuals are acceptable.
   - Current: There is no player, ground, building representation, movement/input path, or production camera.
   - Target: A/D and left/right arrows control the same local player through the normal gameplay command path. Walking uses the working baseline of 3 u/s. Opposing directions cancel, aliases do not double speed, release stops movement, and focus loss clears held input until a fresh press. The player remains grounded and visible, stops at either scene end, can walk past the building, and visibly faces the direction of travel. The active local player/actor retains its identity while moving and turning. Camera, input routing, and lifecycle behavior support extending this scene in Phase 3.
   - Acceptance: AC-02–AC-08 pass, including a one-second unobstructed walk within 5% of the baseline distance and movement checks through production input.

3. **BASE-03 — Repeatable foundation checks:** Developers can repeat repository-stored launch, menu, movement, and rendered-scene smoke checks on Windows and Linux, with current-run results and required images and a failing verdict when required behavior or evidence is missing.
   - Current: There are no executable foundation smoke checks, rendered evidence procedures, or current-run foundation results.
   - Target: A small documented check set exercises the delivered menu, basic scene, walking/facing, bounds/focus, return/restart, and Quit behavior. Every required case has a distinct identity, current result, and required evidence. Missing or duplicate-substitute results, empty execution, missing tools, crashes, timeouts, and missing required images cannot produce a pass. Reruns use fresh evidence; concurrent runs isolate evidence or are explicitly refused. Actual execution on both Windows and Linux is required for completion.
   - Acceptance: AC-10–AC-13 pass, with executed evidence for AC-01–AC-09 from both operating systems. The comprehensive framework and full release-case catalog remain Phase 5 work.

## Boundaries

**In scope:**

- A visible main menu with Start and Quit, and direct Escape return from play.
- One small reusable scene with colored background, ground, one small decorative main building, and one controllable static player; simple or placeholder visuals are sufficient.
- Left/right walking, visible facing, bounded ground movement, and a camera that keeps the locally controlled player visible.
- Input/focus routing, one canonical local movement-command path, active player/actor identity, and scene teardown/reentry checks needed for the Phase 3 extension.
- Repository-owned minimal scene resources and their content identities; provenance for any selected donor resource.
- A small stored and documented smoke-check set, current results/images, explicit failure checks, and actual Windows/Linux execution.

**Out of scope:**

- Production survivor art, idle/walk animation, modular equipment presentation, detailed scenery, forest assets, walls, towers, and the full asset pack — the creator requested basic visuals; Phase 4 owns complete content production.
- Main-building interaction, interiors, construction, recruitment, health, or lane-blocking collision — the building is scenery in this increment; later feature phases own its behavior.
- Sprinting, jumping, stamina, combat, inventories, NPCs, zombies, portals, time/day-night, economy, and campaign outcomes — Phases 6–8 own the relevant systems.
- LAN hosting/joining, multiple controlled players, discovery, reconnection, and multiplayer session recovery — Phases 3 and 9 own the relevant behavior; this phase preserves the shared foundation only.
- An in-scene Resume/menu overlay — Escape returns directly to the main menu as chosen by the creator.
- Player-facing saves/loads, saved-run management, full settings/remapping/UI-scale/audio/localization interfaces, and finished presentation — Phase 9 completes these requirements. Basic usable labels, input routing, and supported desktop layouts are required here.
- GitHub Actions integration, a comprehensive game-test framework, and the full A-01–A-47 case catalog — Phase 5 owns these; Phase 2 still supplies its own small executable smoke checks and rendered evidence.
- Release exports, complete mixed-computer LAN, reference-PC performance/endurance benchmarks, and balancing — Phase 10 requires separate evidence. Foundation checks do not certify a finished campaign.
- Replacing, bypassing, or completing Phase 1's rules/toolchain work in this phase — implementation depends on its proven baseline.

## Constraints

- Use Godot and C# for the game and game tests, under Phase 1's selected and proven toolchain, enforced rules, and gameplay/Godot/service boundaries. Development tooling may use other languages. Do not choose a new toolchain merely from the starter feature tags.
- Keep `src/godot-game/project.godot` as the game launch target and extend the same foundation for Phase 3; do not create a separate demonstration game.
- Use keyboard/mouse and the source defaults A/D or left/right arrows for walking. Full rebinding and settings persistence remain later work.
- The working walk baseline is 3 u/s. The one-second movement acceptance interval is inclusive: 2.85–3.15 u away from scene boundaries. This is a smoke-check tolerance, not evidence of final balance or a release performance benchmark. The scene's world-unit scale and bounds must be explicit so this distance is verifiable.
- The basic menu and scene must remain visible and usable at 1280×720 and 1920×1080. Full UI scaling, configurable camera tuning, and final visual polish remain later work.
- Establish explicit identities for the active local session/player/actor and selected content, consistent with Phase 1. Player/actor identity remains unchanged during movement and facing changes; a new Start creates a fresh scene/session. Identifier representation and concrete command/camera implementations belong to discuss-phase and planning.
- Selected runtime/check inputs must be self-contained in the repository or reproducibly obtained as declared dependencies. Preserve any donor originals; no runtime dependency may point to the creator's external reference folders.
- Actual Windows and Linux execution is required for phase completion. An unavailable required environment is a reported blocker, never a pass or an implied result from the other OS. This document records future acceptance obligations; no game check has passed merely because the specification exists.

## Acceptance Criteria

- [ ] **AC-01 — Main menu (BASE-01):** Normal launch displays a readable main menu with Start and Quit. Both mouse activation and normal keyboard focus/activation can select the actions. Start enters the basic scene through the production flow; no development command is required.
- [ ] **AC-02 — Basic rendered fixture (BASE-02):** The scene visibly contains a colored background, ground, one small decorative main-building representation, and exactly one player. A static sprite or placeholder with a direction marker satisfies player presentation. The menu and fixture are visible and usable at 1280×720 and 1920×1080; missing production art or animation is not a failure when the agreed basic representations are present.
- [ ] **AC-03 — Direction and input combinations (BASE-02):** A/left arrow moves left and D/right arrow moves right through production input. The player's visible facing matches the latest nonzero movement direction and remains when stopped. Holding both directional aliases does not exceed ordinary speed; simultaneously holding any left and any right input produces zero horizontal movement. Releasing all movement input stops movement by the next movement update, and keyboard repeat does not accumulate speed.
- [ ] **AC-04 — Walking distance (BASE-02):** From an unobstructed position away from either end, one second of held directional input produces an absolute horizontal displacement in the inclusive interval 2.85–3.15 u. Check both directions. The result follows elapsed game time rather than growing with the number of input-repeat events.
- [ ] **AC-05 — Ground, building, and bounds (BASE-02):** Walking keeps the player grounded. The player can traverse the building's position without opening an interface or being blocked by it. At each legal horizontal limit, further outward input leaves the player within that limit; inward input resumes ordinary movement. The production camera keeps the player visible and respects the scene boundaries, including at either end.
- [ ] **AC-06 — One identified local actor (BASE-02):** While walking and turning through normal input, the same active player/actor identity is retained and only that actor moves. Normal control enters the canonical local movement-command path without a second direct mutation path. The scene started from the main menu is the identified foundation to extend in Phase 3; direct test-only position changes do not count as control evidence.
- [ ] **AC-07 — Repetition, restart, and teardown (BASE-01, BASE-02):** Held/repeated Start or Escape activation causes one transition per press. During transitions, rapid activation cannot leave duplicate live scenes or players: the main menu has zero active gameplay players and play has exactly one. Escape returns directly to the main menu and removes previous scene input/callbacks. Starting again creates one fresh scene at its initial spawn, without retained movement, duplicate listeners, or changed walk speed from the previous scene.
- [ ] **AC-08 — Focus and input handoff (BASE-01, BASE-02):** Losing window focus or returning to the menu clears held movement and stops the former gameplay input from affecting a scene. Regaining focus or starting again does not resume that held movement until a fresh press. Menu mouse/keyboard actions cannot also issue gameplay movement commands.
- [ ] **AC-09 — Quit (BASE-01):** Activating Quit in the main menu ends the game process cleanly within the smoke check's documented bounded wait, with no crash or hanging game process.
- [ ] **AC-10 — Self-contained foundation (BASE-02, BASE-03):** The game and its checks resolve the minimal scene resources without access to external donor/reference folders or absolute development paths. Any reused resource has a recorded source and transformation; original donors remain unchanged. No full asset-production fixture is required for these minimal resources.
- [ ] **AC-11 — Stored cases and two-platform evidence (BASE-03):** Repository-stored readable cases and executable smoke entry points cover AC-01–AC-09, with documented commands, required-case identities, bounded waits/cleanup, result locations, and rendered inspection steps. Actual Windows and Linux runs record OS/toolchain identity and current results. Each run provides required rendered images for the launch menu, initial scene, left-facing movement, right-facing movement, and returned menu, plus movement/state and process results. The evidence establishes production input and visible behavior; headless results alone do not satisfy it. Future A-01–A-47 catalog/framework work is not required here.
- [ ] **AC-12 — Missing, empty, and duplicate evidence (BASE-03):** Success requires a valid current result for every required case and every required image. Duplicate or extra records cannot stand in for a missing required case. Deliberate zero-case execution, a missing required result/image, and duplicate-substitution trials fail visibly. Missing tools, an unmet expected behavior, crashes, and timeouts fail or report a blocking condition rather than passing. Both required OS runs must be present before phase completion.
- [ ] **AC-13 — Repeatability, order, and concurrent runs (BASE-03):** Rerunning unchanged checks produces the same behavioral verdict using newly produced evidence; an earlier pass or image cannot satisfy a later failed/incomplete run. Reordering otherwise valid case results does not change the verdict. Simultaneous runs either keep their results, images, and cleanup separate or explicitly refuse the conflict; interrupted or partial output cannot be mixed into a passing run.

## Edge Coverage

**Coverage:** 12/12 applicable edges resolved · 0 unresolved

**Verification:** 12 explicit acceptance resolutions · 0 test backstops · 0 dismissals

The compiled edge probe proposed 11 rows from authored shapes: BASE-01 `stateful/io`, BASE-02 `numeric-range/stateful/io`, and BASE-03 `collection/stateful/io`. The shape choices describe lifecycle, movement limits/rate, and required-case/result sets; visual assets are not treated as a sorting/text-processing feature. All 11 engine rows are retained below. Manual review additionally raised BASE-01's active scene/player count boundary. The creator accepted the concrete outcomes in three grouped edge questions; none was silently defaulted.

| Category | Requirement | Status | Resolution / Reason |
| --- | --- | --- | --- |
| boundary | BASE-01 | resolved; verification: explicit | AC-07: zero gameplay players in the menu, exactly one in play, including rapid transitions. Additional manually raised edge. |
| idempotency | BASE-01 | resolved; verification: explicit | AC-07: one transition per press; new Start restores a fresh initial scene without duplicate controls. |
| concurrency | BASE-01 | resolved; verification: explicit | AC-07–AC-08: rapid transition input cannot duplicate scenes; previous-scene input/callbacks are removed and held input is cleared. |
| boundary | BASE-02 | resolved; verification: explicit | AC-03 and AC-05: opposing inputs cancel; both scene ends block outward motion and allow inward motion; ground and building traversal remain valid. |
| precision | BASE-02 | resolved; verification: explicit | AC-04: 3 u/s baseline, inclusive 2.85–3.15 u over one unobstructed second in either direction. |
| idempotency | BASE-02 | resolved; verification: explicit | AC-03 and AC-07: input aliases/repeat do not multiply speed; scene reentry does not duplicate movement listeners. |
| concurrency | BASE-02 | resolved; verification: explicit | AC-03 and AC-08: simultaneous opposing inputs cancel, and focus/menu transitions clear held movement until a fresh press. |
| adjacency | BASE-03 | resolved; verification: explicit | AC-12: equal/duplicate case identities cannot merge into substitutes for missing required results or images. |
| empty | BASE-03 | resolved; verification: explicit | AC-12: zero execution and incomplete required result/image sets cannot pass. |
| ordering | BASE-03 | resolved; verification: explicit | AC-13: result order does not change the verdict; identities determine required-case coverage. |
| idempotency | BASE-03 | resolved; verification: explicit | AC-13: reruns reproduce the verdict using fresh evidence rather than stale passes. |
| concurrency | BASE-03 | resolved; verification: explicit | AC-13: simultaneous runs isolate evidence/cleanup or explicitly refuse the conflict; partial/interrupted output cannot supply a pass. |

## Prohibitions (must-NOT)

**Coverage:** 0/0 additional applicable prohibitions · 0 unresolved

The adversarial recall and precision review covered BASE-01 through BASE-03 after resolving the edges. No additional bespoke values, safety, privacy, or ethics prohibition was retained for this basic local foundation. Duplicate scenes, input leaks, off-screen movement, stale/false passes, and missing evidence are engineering concerns with explicit acceptance criteria above. Production-art demands, unrequested interfaces/services, and gameplay expansion are controlled by the agreed boundaries. No extra prohibition or fictional wired-check descriptor is introduced.

## Ambiguity Report

| Dimension | Score | Min | Status | Notes |
| --- | --- | --- | --- | --- |
| Goal Clarity | 0.95 | 0.75 | Met | One basic menu-to-scene walking loop is the measurable deliverable. |
| Boundary Clarity | 0.98 | 0.70 | Met | Placeholder visuals and a decorative building are sufficient; feature/art/framework expansion is explicitly excluded. |
| Constraint Clarity | 0.90 | 0.65 | Met | Both OS runs are required; source controls, walk baseline/tolerance, scene limits, and focus behavior are explicit. Concrete implementation remains downstream work. |
| Acceptance Criteria | 0.90 | 0.70 | Met | Concrete visible/input/lifecycle checks and evidence-failure outcomes; all raised edges are resolved. |
| **Ambiguity** | **0.063** | **≤ 0.20** | **Passed** | Exact: 1 − (0.35 × 0.95 + 0.25 × 0.98 + 0.20 × 0.90 + 0.20 × 0.90) = 0.0625. |

Assessment history:

- Initial: goal 0.90, boundaries 0.80, constraints 0.60, acceptance 0.60; ambiguity 0.245. Constraints and acceptance were below minimum.
- After round 1: goal 0.95, boundaries 0.95, constraints 0.60, acceptance 0.70; ambiguity 0.170. The weighted threshold passed, but constraints remained below minimum, so interviewing continued.
- After round 2: goal 0.95, boundaries 0.98, constraints 0.90, acceptance 0.90; exact ambiguity 0.0625, reported as 0.063. All minimums passed. The creator approved proceeding through the final edge review to writing and committing the specification.
- After edge review: all 12 applicable edges have explicit acceptance resolutions. The score remains 0.063; there are no unresolved requirement assumptions or retained prohibitions. These scores describe specification clarity, not completed game verification.

## Interview Log

| Round | Perspective | Question summary | Answer / decision locked |
| --- | --- | --- | --- |
| 1 | Researcher | Minimum scene content? | Creator: “Just small main building. and player (even without a proper asset) moving both sides. just colored background and ground. just basic project.” Production art, forest scenery, walls, and towers are unnecessary for this phase. |
| 1 | Researcher | Required player presentation? | Static player facing the movement direction; animation can wait. Read together with the explicit placeholder allowance, a simple direction marker is sufficient. |
| 1 | Researcher | Required menu flow? | Start and Quit in the main menu; Escape in play returns directly to the main menu. |
| 2 | Researcher / Simplifier | Does the main building have gameplay behavior? | Scenery only, with no interaction, interiors, construction, or lane-blocking collision. |
| 2 | Researcher / Simplifier | Which platforms need executed smoke evidence now? | Both Windows and Linux must run repeatable menu, movement, and visible-scene checks before phase completion. |
| Gate | Requirement review | Proceed after passing the clarity gate? | Creator approved final edge review, then writing and committing SPEC.md. |
| Edge | Failure analysis | Repeated Start/Escape, fresh start, held input, and teardown? | Creator chose “Specify these outcomes”: one transition per press, one fresh scene/player, controls removed on return, no held input carried over, clean Quit. |
| Edge | Failure analysis | Direction combinations, focus loss, ground/ends, facing, and speed? | Creator chose “Specify these outcomes”: A/D and arrows, cancellation/no doubled speed, stop/clear input, grounded bounded visible movement past the building, placeholder direction marker, and 3 u/s with a 5% one-second tolerance. |
| Edge | Failure analysis | Missing/duplicate/empty/reordered/stale/concurrent results? | Creator chose “Specify these outcomes”: distinct current case results/images, no duplicate substitutes or empty passes, visible dependency/crash/timeout/evidence failures, fresh reruns, and isolated or refused concurrent runs. Keep the check set small. |

---

*Phase: 02-playable-foundation*
*Spec created: 2026-11-13 (Europe/Paris)*
*Next step: $gsd-discuss-phase 2 — implementation decisions within these locked requirements.*
