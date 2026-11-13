# Phase 2: Playable foundation - Context

**Gathered:** 2026-11-14 (Europe/Paris)
**Status:** Ready for planning; implementation depends on completing Phase 1.

<domain>
## Phase Boundary

Deliver the basic menu-to-scene walking loop defined in the Phase 2 specification: Start/Quit, one small decorative main building, colored background and ground, one direction-facing player walking left/right, direct Escape return, and repeatable Windows/Linux smoke evidence. Extend the existing Godot project so this same scene can become the Phase 3 LAN foundation.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**3 requirements are locked: BASE-01, BASE-02, BASE-03.** See [02-SPEC.md](02-SPEC.md) for full requirements, boundaries, constraints, and AC-01 through AC-13.

Downstream agents MUST read `.planning/phases/02-playable-foundation/02-SPEC.md` before planning or implementing. Requirements are not duplicated as implementation decisions below.

**In scope (from SPEC.md):**

- A visible main menu with Start and Quit, and direct Escape return from play.
- One small reusable scene with colored background, ground, one small decorative main building, and one controllable static player; simple or placeholder visuals are sufficient.
- Left/right walking, visible facing, bounded ground movement, and a camera that keeps the locally controlled player visible.
- Input/focus routing, one canonical local movement-command path, active player/actor identity, and scene teardown/reentry checks needed for the Phase 3 extension.
- Repository-owned minimal scene resources and their content identities; provenance for any selected donor resource.
- A small stored and documented smoke-check set, current results/images, explicit failure checks, and actual Windows/Linux execution.

**Out of scope (from SPEC.md):**

- Production survivor art, idle/walk animation, modular equipment presentation, detailed scenery, forest assets, walls, towers, and the full asset pack — the creator requested basic visuals; Phase 4 owns complete content production.
- Main-building interaction, interiors, construction, recruitment, health, or lane-blocking collision — the building is scenery in this increment; later feature phases own its behavior.
- Sprinting, jumping, stamina, combat, inventories, NPCs, zombies, portals, time/day-night, economy, and campaign outcomes — Phases 6–8 own the relevant systems.
- LAN hosting/joining, multiple controlled players, discovery, reconnection, and multiplayer session recovery — Phases 3 and 9 own the relevant behavior; this phase preserves the shared foundation only.
- An in-scene Resume/menu overlay — Escape returns directly to the main menu as chosen by the creator.
- Player-facing saves/loads, saved-run management, full settings/remapping/UI-scale/audio/localization interfaces, and finished presentation — Phase 9 completes these requirements. Basic usable labels, input routing, and supported desktop layouts are required here.
- GitHub Actions integration, a comprehensive game-test framework, and the full A-01–A-47 case catalog — Phase 5 owns these; Phase 2 still supplies its own small executable smoke checks and rendered evidence.
- Release exports, complete mixed-computer LAN, reference-PC performance/endurance benchmarks, and balancing — Phase 10 requires separate evidence. Foundation checks do not certify a finished campaign.
- Replacing, bypassing, or completing Phase 1's rules/toolchain work in this phase — implementation depends on its proven baseline.

</spec_lock>

<decisions>
## Implementation Decisions

### No additional design interview

- **D-01:** The creator declined the offered discussion areas: “All of it is obvious. don't want to discuss”. Proceed to planning from the locked specification without further questions about routine presentation choices.
- **D-02:** No particular layout, palette, placeholder design, spawn position, camera implementation, scene dimensions, or control-hint placement was selected during this discussion. Do not turn proposed alternatives or agent defaults into creator-approved requirements.

### Communication

- **D-03:** Use English for user-facing communication. The creator explicitly corrected the initial French response. Preserve the existing interactive-question preference for future questions that actually need a decision; this instruction does not invite another Phase 2 design interview.

### Agent's Discretion

Research and planning may resolve the routine scene, placeholder, camera, menu, and hint choices within `02-SPEC.md`, using the smallest implementation that satisfies its acceptance criteria. They also choose concrete identity representations, command routing, world-unit/pixel mapping, resource layout, and smoke tooling consistent with Phase 1's completed rules and boundaries.

The source specification's applicable working baselines remain in force. Any implementation choice must preserve the explicit Phase 2 outcomes rather than adding features, requiring production art, weakening verification, or reopening already-settled requirements.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read the relevant references before planning or implementing.** All paths below are relative to the repository root.

### Phase and project authority

- `.planning/phases/02-playable-foundation/02-SPEC.md` — locked requirements; MUST read before planning. Contains exact input, lifecycle, movement, visibility, evidence, and two-platform acceptance obligations.
- `.planning/REQUIREMENTS.md` — BASE-01 through BASE-03, phase ownership, and verification lifecycle.
- `.planning/ROADMAP.md` — Phase 2 goal, dependency on Phase 1, and the boundary with Phase 3 LAN work.
- `.planning/PROJECT.md` — creator-approved delivery direction, Godot/C# constraint, reuse policy, and agent-operated verification expectations.
- `specs/0.1/initial-spec.md` — particularly §§3.1, 4.1, and 13.2 for design units, walking/input, and camera/presentation baselines. The narrower Phase 2 scope is explicit in `02-SPEC.md`.
- `specs/0.1/assets-and-resources.md` — particularly §§1 and 4 for provenance, generated resources, and the distinction between logical design units and source pixels. Full content production remains Phase 4.

### Prerequisite baseline and collaboration

- `.planning/phases/01-code-rules-and-documentation/01-SPEC.md` — required development baseline and compatibility/state/lifecycle proofs; its obligations are not completed by Phase 2 planning.
- `.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md` — established preferences for executable rules, focused documentation, C# game code/tests, focused checks followed by full code verification, and concise reports.
- `.planning/STATE.md` — current execution position and unresolved prerequisite proofs; Phase 1 is still in progress at capture time.
- `tools/phase1/toolchain.json` — current pinned toolchain inputs; use completed Phase 1 compatibility evidence rather than inferring support from starter project feature tags.
- `Agents.md` — currently tracked root guide and focused documentation entry point; preserve Phase 1's responsibility for canonical filename casing.
- `docs/workflow/ai-collaboration.md` — interactive question controls when a question is needed; the creator declined further design questions for this discussion.
- `docs/workflow/gsd.md` — phase workflow and planning-artifact navigation.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- `src/godot-game/project.godot` is the existing launch target. It has project, assembly, rendering, and canvas stretch settings, but no player-facing main scene or production input actions at scout time.
- `src/godot-game/proofs/Phase1.cs` and `src/godot-game/proofs/Phase1.tscn` provide the current engine construction/ready/teardown bootstrap proof. They are proof fixtures, not the playable foundation.
- `src/core/KingdomZombies.Core.csproj` and `src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj` establish existing project boundaries. Their inspected `ProofValue` and `IProofService` types are minimal boundary proofs, not gameplay identity or movement implementations.
- `tools/phase1/` and `tests/phase1/` provide existing toolchain and evidence-checking infrastructure whose applicable conventions can inform the small foundation checks. Phase 2 does not own finishing or replacing Phase 1 proofs.

### Established Patterns

- C# projects, strict build configuration, inherited analyzers, and proof tooling now exist; the earlier empty-project description in the specification is historical scout context.
- No playable menu, player movement, production camera, foundation command route, or Phase 2 smoke entry point was found during this scout. These are implementation work, not assumed reusable features.
- Documentation uses focused indexes. No codebase maps, packaged spike/sketch findings, or matching pending todos were found.
- Current project state records Windows toolchain/enforcement progress and outstanding Phase 1 obligations, including Linux proofs. A configured toolchain or prepared Linux environment is not executed two-platform acceptance evidence.

### Integration Points

- Add the player-facing menu and scene under the existing Godot launch target, keeping Phase 1's proof fixture available independently.
- Connect production input to one local command path and explicit active player/actor state within the proven gameplay/Godot/service boundaries, ready for Phase 3 extension.
- Attach the basic resources, camera, and scene lifecycle to that same foundation. Keep selected runtime and check inputs self-contained.
- Add the small Phase 2 check set and rendered evidence procedure without implementing Phase 5's comprehensive framework.

</code_context>

<specifics>
## Specific Ideas

- The creator's current instruction is to skip further design discussion because the proposed choices are obvious.
- The specification already records the desired simplicity: “Just small main building. and player (even without a proper asset) moving both sides. just colored background and ground. just basic project.” This remains the presentation direction; no additional visual preference was selected here.

</specifics>

<deferred>
## Deferred Ideas

None. The creator proposed no new capability. Existing later-phase work remains assigned as specified in the roadmap.

</deferred>

---

*Phase: 02-playable-foundation*
*Context gathered: 2026-11-14 (Europe/Paris)*
