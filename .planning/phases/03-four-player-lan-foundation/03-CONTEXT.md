# Phase 3: Four-player LAN foundation - Context

**Gathered:** 2026-11-14 (Europe/Paris)
**Status:** Ready for planning; implementation depends on completion of Phases 1 and 2.

<domain>
## Phase Boundary

Extend the Phase 2 foundation so one host and up to three guests can discover or manually join the running scene, independently walk with synchronized authoritative positions and their own cameras, and leave or restart without duplicated actors, retained slots, or stale control. This is the same basic game and scene, with the LAN increment and its repeatable checks.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**4 requirements are locked: LAN-01 through LAN-04.** See [03-SPEC.md](03-SPEC.md) for full requirements, constraints, and acceptance criteria.

Downstream agents MUST read `.planning/phases/03-four-player-lan-foundation/03-SPEC.md` before planning or implementing. Requirements are not duplicated in the implementation decisions below. Declining further design discussion does not remove any specified behavior or verification obligation.

**In scope (from SPEC.md):**

- Extend the existing Godot launch target and Phase 2 fixture; retain solo Start, Quit and direct Escape return.
- Host controls, a basic refreshing LAN session list, manual host-address entry, join/cancel/back controls and visible connection/error feedback.
- The minimum list information needed to choose a host: session name, host label or endpoint, participant count out of four, and current availability/join status.
- Host-only through four-player sessions in separate processes, with distinct profiles, placeholders and local cameras.
- Joining a running foundation: current actor identities, positions, facing, participant ownership and a coherent state/revision baseline before control.
- Authoritative movement, ownership/readiness validation, duplicate/stale delivery handling and measured basic synchronization.
- Guest placeholder removal, admission-slot release, host-end messaging/menu return, and fresh-session teardown/rehosting.
- Stable local profile identities and replaceable discovery, identity and connection services with demonstrated boundary substitutions.
- Small repository-owned smoke cases, current per-peer results, rendered evidence, explicit failing controls and actual execution on Windows and Linux.

**Out of scope (from SPEC.md):**

- Full campaign snapshots/catch-up, inventories, jobs, waves, exploration and other campaign state during late join — those systems and Phase 9 own their complete acceptance.
- NPC takeover, remembered survivor roles, same-survivor reconnection, succession and spectating — they require later gameplay/session features. Placeholder removal here applies to the movement fixture.
- Saving/loading, normal-exit saving and last-successful-save recovery after host loss — Phase 9 owns persistence and complete host-loss behavior; this foundation ends the session and returns players to the menu.
- A waiting/ready lobby — the host enters the scene immediately and guests join it in progress.
- Finished session-browser presentation, comprehensive campaign metadata, invitations, online matchmaking, accounts, Steam/Epic implementation, host migration and Internet traversal — later session polish or explicit version 0.1 exclusions; the replaceable service boundary is included.
- Simulated poor-network acceptance, prediction/reconciliation requirements, final latency tuning and release performance/endurance benchmarks — only the agreed ordinary local movement targets are locked here. Concrete movement/network techniques remain implementation decisions.
- Production art, animation, equipment, combat, NPCs, zombies, economy, buildings or campaign expansion — retain Phase 2's basic fixture; Phases 4 and 6–8 own the relevant features.
- A comprehensive test framework, GitHub Actions and the full A-01–A-47 catalog — Phase 5 owns them; the delivered LAN increment still needs its own repeatable checks.
- Packaged release exports and actual mixed-computer/mixed-OS LAN certification — Phase 10 requires separate evidence. Local four-process results have their own stated environment.

</spec_lock>

<decisions>
## Implementation Decisions

### No further design discussion

- **D-01:** The creator declined all offered discussion areas: “Nothing. all is obvious”. Proceed to planning from the locked specification without further questions about these routine choices.
- **D-02:** No particular host/join layout, session-naming convention, local-profile interface, display-name default, placeholder color, name-label style, local-player marker, progress presentation, or error/retry layout was selected. Do not turn offered discussion topics or agent defaults into creator-approved requirements.

### Carried-forward communication and presentation

- **D-03:** Continue using English for user-facing communication and available interactive question controls when a future question actually needs a decision, as recorded in Phase 2 context and the indexed collaboration instructions. This does not invite another Phase 3 design interview.
- **D-04:** Carry forward Phase 2's basic presentation direction. Its context delegates routine scene/menu/camera choices within its specification; this discussion adds no new visual preference or asset requirement.

### Agent's Discretion

Research and planning may resolve the routine host/join flow, session names, local-profile selection and display names, character distinction, and connection/error feedback within `03-SPEC.md`. Use the smallest usable implementation that meets the specified behavior, keyboard/mouse handling, and readability at both required resolutions. Concrete choices remain implementation decisions rather than additional creator-approved requirements.

Transport/discovery mechanisms, identifiers' representations, profile persistence details, authority and replication techniques, revision/readiness handling, camera implementation, configuration placement, and smoke-check tooling remain research/planning work under the locked specification and the completed prerequisite rules and boundaries. Preserve all specified capacity, identity, timing, ownership, teardown, substitution, and two-platform evidence obligations. No new protocol, library, numeric target, or architecture exemption was selected in this discussion.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read the relevant references before planning or implementing.** All paths below are relative to the repository root.

### Phase and project authority

- `.planning/phases/03-four-player-lan-foundation/03-SPEC.md` — locked requirements; MUST read before planning. Contains exact LAN admission, identity, authority, synchronization, lifetime, and evidence obligations.
- `.planning/phases/02-playable-foundation/02-SPEC.md` — prerequisite production scene, movement, input/focus, camera, and transition contracts to extend.
- `.planning/phases/02-playable-foundation/02-CONTEXT.md` — basic presentation direction, routine implementation discretion, and explicit English communication preference.
- `.planning/REQUIREMENTS.md` — LAN-01 through LAN-04, complete-session requirements, phase ownership, and verification lifecycle.
- `.planning/ROADMAP.md` — Phase 3 goal and dependency; boundaries with Phase 5 verification infrastructure, Phase 9 complete sessions/persistence, and Phase 10 release evidence.
- `.planning/PROJECT.md` — complete delivery direction, Godot/C# constraint, replaceable multiplayer services, self-contained inputs, and agent-operated verification expectations.
- `.planning/research/SUMMARY.md` — research candidates and unresolved proofs; recommendations do not establish an implemented or verified network path.
- `specs/0.1/initial-spec.md` — particularly §10 for multiplayer/session behavior and service separation, plus §§4.1 and 13.2 for inherited movement/input and camera/presentation baselines. Apply the narrower Phase 3 boundary in `03-SPEC.md`.

### Prerequisite baseline and architecture

- `.planning/phases/01-code-rules-and-documentation/01-SPEC.md` — required enforced development baseline and actual compatibility/state/lifecycle proofs.
- `.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md` — executable rules, focused documentation, C# game code/tests, focused documentation checks, and complete verification for code/contract changes.
- `.planning/STATE.md` — current execution position and unresolved prerequisites. Phase 1 is still executing at capture time; Phase 2 production gameplay and Phase 3 networking are not implemented.
- `tools/phase1/toolchain.json` — pinned baseline inputs; actual prerequisite compatibility evidence remains required.
- `tools/phase1/projects.json` — current approved project inventory and dependency categories; this discussion does not approve changing them.
- `docs/architecture/README.md` — indexed project, type, engine, state, and service boundaries.
- `docs/architecture/decisions/0002-boundaries.md` — gameplay/engine/service dependency rationale.
- `docs/architecture/decisions/0003-state-and-engine.md` — controlled state, strict snapshots, atomic publication, and closing engine-lifetime acceptance before deferred work returns.

### Indexed collaboration and verification instructions

- `AGENTS.md` — canonical root guide and focused documentation entry point.
- `docs/workflow/ai-collaboration.md` — interactive question preference when a question is needed; the creator declined further questions for this discussion.
- `docs/workflow/verification.md` — focused documentation checks, complete code verification, fresh evidence, and honest platform/result scope.
- `docs/workflow/gsd.md` — phase workflow and planning-artifact navigation.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- `src/godot-game/project.godot` remains the existing launch target. Its current settings include rendering and canvas stretch configuration; no production main scene or movement input actions were found during the scout.
- `src/core/StableIdentity.cs` demonstrates exact ordinal identity comparison, and `src/core/ExampleStateStore.cs` demonstrates detached validation followed by atomic immutable-root publication. These are proof examples, not production player profiles or multiplayer snapshots.
- `src/multiplayer-abstractions/IProofService.cs` demonstrates an independent service boundary. Actual discovery, identity, and connection services still need implementation.
- `src/godot-game/proofs/Phase1.cs`, `src/godot-game/proofs/Phase1.tscn`, and `src/godot-game/proofs/LifetimeToken.cs` provide native lifecycle proofs and the close-before-cancellation pattern. They are not a second playable game or a network implementation.
- `tools/phase1/` and `tests/phase1/` supply existing proof/evidence conventions that may inform this increment's small checks without expanding into Phase 5's comprehensive framework.

### Established Patterns

- The current C# projects, enforced rules, controlled state/snapshot examples, and engine lifetime proofs establish prerequisite patterns. Phase 3 must extend the actual completed Phase 2 production scene and command path when they exist.
- No production menu, walking character, camera, LAN browser, connection lifecycle, or network movement implementation was found during the scout. Their future delivery cannot be inferred from proof fixtures or project declarations.
- Keep gameplay independent of discovery details, LAN addresses, and transient peers under the established dependency policy. The specification distinguishes stable player/profile, actor, session/run, and connection lifetimes.
- Focused documentation indexes already exist. No codebase maps, packaged spike/sketch findings, or matching pending todos were found for this discussion.
- Phase 1 implementation is still in progress. Preparation of a Linux environment and Windows proof results do not supply Phase 3's required actual Windows and Linux execution evidence.

### Integration Points

- Extend the existing game's Phase 2 menu with the specified hosting, basic discovery, and manual joining, keeping the solo flow as a regression case.
- Extend the Phase 2 canonical action route into one host-owned validation and movement path shared by solo, host-local, and guest input.
- Compose replaceable discovery, local identity, and connection adapters at the established service/engine boundary, with profiles distinct from actor and session ownership.
- Connect participant readiness and current-state installation to actor creation, local control, per-player cameras, and rendered character distinction.
- Add bounded joining, cleanup, session replacement, and small per-peer smoke/evidence paths, while leaving complete campaign recovery and release certification with their roadmap owners.

</code_context>

<specifics>
## Specific Ideas

- The creator's current instruction is: “Nothing. all is obvious”. End the optional interview and use the locked specification for planning.
- All four offered topics were declined. No detailed product alternative was selected, and no follow-up design questions were asked.
- Phase 2's simple fixture remains the presentation direction. No additional appearance or naming reference was supplied here.

</specifics>

<deferred>
## Deferred Ideas

None. The creator proposed no new capability or roadmap change. Existing later-phase work retains its specified ownership.

</deferred>

---

*Phase: 03-four-player-lan-foundation*
*Context gathered: 2026-11-14 (Europe/Paris)*
