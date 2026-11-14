# Phase 3: Four-player LAN foundation — Specification

**Created:** 2026-11-14 (Europe/Paris)
**Ambiguity score:** 0.080 (gate: ≤ 0.20)
**Requirements:** 4 locked — LAN-01, LAN-02, LAN-03, LAN-04
**Status:** Requirements locked; implementation and verification pending.

## Goal

A host and up to three guests can find or manually join the running Phase 2 foundation scene, independently walk with synchronized authoritative positions and their own cameras, and leave or restart without duplicated actors, retained slots or stale control.

## Background

At the 2026-11-14 scout, the repository had Godot/C# projects, enforced-rule tooling, pure state/identity/snapshot examples, native engine lifetime proofs and focused architecture documentation. Phase 1 was still executing. These are foundation proofs, not implemented gameplay or networking.

The [Phase 2 specification](../02-playable-foundation/02-SPEC.md) and [context](../02-playable-foundation/02-CONTEXT.md) lock a Start/Quit menu, colored background and ground, a small decorative main building, one static direction-facing placeholder, walking and direct Escape return. No playable menu, player movement, production camera or LAN implementation was found. Phase 3 depends on that increment's implementation and extends the same game and scene.

The creator selected both a basic LAN list and manual joining in this phase, immediate hosting, joining while existing participants keep moving, and removal of departing guests' temporary placeholders. This brings basic discovery into Phase 3 while complete campaign session metadata, survivor recovery and save integration retain their Phase 9 ownership.

Scope authority: [requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md), [project definition](../../PROJECT.md), and [game specification §10](../../../specs/0.1/initial-spec.md#10-multiplayer-and-session-behavior). The [architecture index](../../../docs/architecture/README.md) and [state/engine decision](../../../docs/architecture/decisions/0003-state-and-engine.md) govern prerequisite boundaries and lifetime handling. Research recommendations guide later implementation choices; they do not prove a network path exists.

## Requirements

1. **LAN-01 — Hosting, discovery and admission:** Players can host the Phase 2 foundation scene and join its running session through a basic LAN session list or manual address, with one host and up to three guests, one distinct placeholder per ready participant, and removal of a departing guest's placeholder.
   - Current: The Phase 2 menu/scene is specified but not implemented. The repository contains engine proof fixtures, with no production LAN host, session browser, join flow or participant lifecycle.
   - Target: Keep solo Start and extend the existing game with Host, a basic LAN session list and manual-address joining. Hosting enters the same foundation scene immediately. Guests join while others continue playing, receive current foundation state before control, and occupy one of four total slots. Guest departure removes this fixture's placeholder and releases its slot.
   - Acceptance: AC-01–AC-05 and AC-12–AC-13, using the production menu and real separate game processes.

2. **LAN-02 — Independent synchronized walking:** Each ready participant controls only their own character through normal walking input and an independent camera; in the ordinary four-process smoke check, remote movement changes appear within 250 ms and stopped client positions agree with the host within 0.05 u within 0.5 seconds.
   - Current: There is no playable character, movement/input implementation, production camera or network movement path. Phase 2 locks the basic walking/facing/bounds behavior to extend.
   - Target: Every ready participant has a distinct controlled placeholder and their own camera. Preserve Phase 2 input, walking, facing, focus and bounds behavior. Meet the agreed inclusive 250 ms remote-change limit and 0.05 u position agreement within 0.5 seconds after stopping in the ordinary local four-process check.
   - Acceptance: AC-06–AC-08 and AC-16–AC-17, with timed state observations and rendered production-input evidence.

3. **LAN-03 — One authoritative action path:** Solo, host-local, and guest movement commands use one host-authoritative action path that accepts an action at most once and rejects unready, stale-session, stale-sequence, and wrong-owner input without changing current state.
   - Current: Core state, stable identity and strict snapshot/lifetime proof examples exist; there is no production movement-command authority, sender-to-actor mapping, baseline readiness or revision protocol.
   - Target: Route solo, host-local and guest intent through the same host-owned validation and action processing. Repeated delivery has no additional effect. Reject unready or wrong-owner input and obsolete commands, baselines and callbacks; a current stop or session cannot be overwritten by old work.
   - Acceptance: AC-05 and AC-09–AC-13, including deliberate duplicate/reordered/unauthorized production-path delivery checks.

4. **LAN-04 — Replaceable services and stable local profiles:** Discovery, stable local player identity, and connection services can each be replaced without editing gameplay rules; player identity remains separate from display name, LAN address, and transient peer number.
   - Current: Multiplayer.Abstractions contains IProofService as a composition proof, and Core contains StableIdentity as a state proof. No actual discovery/identity/connection adapters or persisted local player profiles exist.
   - Target: Keep discovery, profile identity, endpoint/peer mapping and connection lifetime outside gameplay rules. Persist one opaque player identity per local profile, independent of display name or network endpoint. Demonstrate separate service substitutions without gameplay-rule edits and retain actual LAN-process checks as independent evidence.
   - Acceptance: AC-14–AC-18, including profile restarts, duplicate identity rejection and substitution of each service boundary.

## Boundaries

**In scope:**

- Extend the existing Godot launch target and Phase 2 fixture; retain solo Start, Quit and direct Escape return.
- Host controls, a basic refreshing LAN session list, manual host-address entry, join/cancel/back controls and visible connection/error feedback.
- The minimum list information needed to choose a host: session name, host label or endpoint, participant count out of four, and current availability/join status.
- Host-only through four-player sessions in separate processes, with distinct profiles, placeholders and local cameras.
- Joining a running foundation: current actor identities, positions, facing, participant ownership and a coherent state/revision baseline before control.
- Authoritative movement, ownership/readiness validation, duplicate/stale delivery handling and measured basic synchronization.
- Guest placeholder removal, admission-slot release, host-end messaging/menu return, and fresh-session teardown/rehosting.
- Stable local profile identities and replaceable discovery, identity and connection services with demonstrated boundary substitutions.
- Small repository-owned smoke cases, current per-peer results, rendered evidence, explicit failing controls and actual execution on Windows and Linux.

**Out of scope:**

- Full campaign snapshots/catch-up, inventories, jobs, waves, exploration and other campaign state during late join — those systems and Phase 9 own their complete acceptance.
- NPC takeover, remembered survivor roles, same-survivor reconnection, succession and spectating — they require later gameplay/session features. Placeholder removal here applies to the movement fixture.
- Saving/loading, normal-exit saving and last-successful-save recovery after host loss — Phase 9 owns persistence and complete host-loss behavior; this foundation ends the session and returns players to the menu.
- A waiting/ready lobby — the host enters the scene immediately and guests join it in progress.
- Finished session-browser presentation, comprehensive campaign metadata, invitations, online matchmaking, accounts, Steam/Epic implementation, host migration and Internet traversal — later session polish or explicit version 0.1 exclusions; the replaceable service boundary is included.
- Simulated poor-network acceptance, prediction/reconciliation requirements, final latency tuning and release performance/endurance benchmarks — only the agreed ordinary local movement targets are locked here. Concrete movement/network techniques remain implementation decisions.
- Production art, animation, equipment, combat, NPCs, zombies, economy, buildings or campaign expansion — retain Phase 2's basic fixture; Phases 4 and 6–8 own the relevant features.
- A comprehensive test framework, GitHub Actions and the full A-01–A-47 catalog — Phase 5 owns them; the delivered LAN increment still needs its own repeatable checks.
- Packaged release exports and actual mixed-computer/mixed-OS LAN certification — Phase 10 requires separate evidence. Local four-process results have their own stated environment.

## Constraints

- Use Godot and C# with the proven Phase 1 baseline, enforced code rules and existing gameplay/Godot/service dependency directions. Keep the currently approved project inventory; any future change follows its owning architecture decision.
- Extend the Phase 2 production scene and command route. Preserve its 3 u/s working walk baseline, inclusive 2.85–3.15 u unobstructed one-second authoritative displacement check, direction-facing placeholder, scene limits and input/focus behavior.
- Use keyboard/mouse. The relevant menu, browser and connection feedback must be usable at 1280×720 and 1920×1080. Routine layout, names, colors, identifiers' concrete representation, transport and camera technique belong to discussion/planning.
- At most four participants including the host can occupy admitted or pending slots. Capacity and slot ownership are host-owned exact integer state. Rejected/cancelled/failed admissions cannot retain a slot.
- Default session values are 30 seconds for an in-progress join and 10 seconds without a response for loss detection, taken from the source's proposed baseline and accepted in the edge review. Keep them explicit, with controlled-time checks at their thresholds; concrete configuration placement remains downstream work.
- The agreed synchronization limits are inclusive: remote movement changes by 250 ms after ordinary input, and stopped positions within 0.05 u of the host by 0.5 seconds after input release. Do not round an over-limit observation into a pass.
- The timing acceptance environment is the ordinary same-machine four-process smoke run on each OS, without injected delay or loss. Use the production network/input/render paths, an explicit measurement clock and fresh records. Arrange monitored actors within the observing camera for rendered timing checks; independent cameras need not show every actor at all times.
- Session/run, stable player/profile, actor and transient connection identities have different lifetimes. Changing a LAN address or peer number cannot change the selected profile's player identity. Restarting the application preserves it; starting a fresh hosted scene creates a new session identity and excludes previous-session work.
- LAN hosting, discovery and joining work without an external account or Internet service. Runtime/check dependencies are self-contained or reproducibly acquired declared inputs; the creator's donor folders are not runtime dependencies.
- Windows and Linux execution are both required for phase completion. Missing environments or pending prerequisite phases are blockers, never inferred passes. This document locks future acceptance; it supplies no gameplay or LAN execution evidence.
- A-38 and A-45 are related release scenarios. Phase 3's foundation results support their LAN/control prerequisites; their complete mixed-OS, campaign-state and persistence acceptance retains its later owner.

## Acceptance Criteria

- [ ] **AC-01 — Production entry and transition lifetime (LAN-01):** Launch retains solo Start and Quit and offers Host, the basic LAN list and manual joining through normal mouse/keyboard controls. Hosting enters exactly one instance of the Phase 2 fixture immediately. Escape leaves the local game for the menu. Repeated/held activation or rapid Host/Join/Cancel/Back cannot create duplicate live sessions, actors or input listeners. Menu input cannot also move a gameplay actor; Quit closes cleanly within a documented bounded wait.

- [ ] **AC-02 — Basic LAN browser and addressing (LAN-01):** A guest can discover a running host, select it and join through the list, or join through a valid manually entered address. The list shows the minimum information in Boundaries. Zero discovered sessions produces an empty state with manual joining still usable. Repeated advertisements give one row per session; different sessions with identical labels remain separate. Refresh/removal/reordering preserves a selected session by its identity or visibly clears it if gone. Unicode labels round-trip without becoming identifiers, and any display shortening preserves the underlying identity. A vanished/full host is revalidated on joining rather than treated as ready from a stale advertisement.

- [ ] **AC-03 — Capacity and racing admissions (LAN-01):** Host-only and total counts of two, three and four participants all work. Four means the host plus at most three guests. An attempted fifth participant is refused with a clear full-session reason and creates no placeholder. With one slot remaining, simultaneous join attempts reserve/admit exactly one; count never exceeds four, and the rejected attempt changes no existing player's actor or control. Pending admissions consume capacity until accepted or released.

- [ ] **AC-04 — Failed, repeated and cancelled joins (LAN-01):** Blank/malformed addresses, an unreachable/closed host, full capacity and an incompatible foundation protocol/content identity produce a clear failure before control. Repeated attempts cannot create multiple reservations or placeholders. Cancelled, failed and timed-out joins release their resources/slot and leave a usable menu/join flow. A still-pending join is cancelled on the first session update at or after its configured timeout, default 30 seconds; controlled-time checks immediately below and at the threshold establish the difference. No unrelated scene or existing participant is reset.

- [ ] **AC-05 — Joining a moving foundation (LAN-01, LAN-03):** Join while existing participants move. Existing players remain active. The newcomer receives one coherent current foundation baseline containing current participants, actor identities, positions, facing, ownership and its revision/session identity; it is not treated as ready and cannot act before that state is installed. On readiness it gains exactly one distinct placeholder. Existing actor identities/positions are preserved and subsequent state advances from the accepted baseline. Campaign catch-up is outside this fixture's acceptance.

- [ ] **AC-06 — Four independently controlled walkers and cameras (LAN-02):** Host plus three separate guest processes can walk simultaneously. A/D and left/right aliases control only the selected local participant's assigned actor, with the Phase 2 walking/facing/ground/building/bounds behavior. Aliases and keyboard repeat do not multiply speed; opposing directions cancel. Authoritative walking retains the Phase 2 baseline/tolerance. Each process's camera follows its own actor within the scene boundaries, and participant actors can be distinguished in the rendered fixture. Replicated positions remain finite and legal.

- [ ] **AC-07 — Measured basic synchronization (LAN-02):** In the stated ordinary local four-process environment, normal movement starts, reversals and stops become visible on the observing peers within 250 ms of the originating input. After input release, within 0.5 seconds every peer's replicated actor position is within 0.05 u of the stopped authoritative host position. Record the actual clock, input/change checkpoints, peer/actor identities and observed errors/times. Timed rendered checkpoints cover actors in the observing camera; state records also compare the correct actor/session/revision. Equality at the limits passes and an over-limit observation fails. Headless state agreement alone does not prove rendered responsiveness.

- [ ] **AC-08 — Independent focus and held input (LAN-02):** Focus loss clears that participant's held movement; peers see its stop within the accepted synchronization limit while other participants keep control. Regaining focus, returning from a menu, joining or rehosting does not resume a previous held direction until a fresh press. Focus changes in one game process cannot transfer input ownership to another actor.

- [ ] **AC-09 — Common authority and ownership (LAN-03):** Normal solo, host-local and guest actions enter the same production authority/validation path. Each accepted unique action has one authoritative effect. The host-local input has no second direct state-mutation route or validation exemption. The host binds an incoming command to its actual participant and currently assigned actor. An attempt to move another participant's actor, act before readiness or claim another actor changes no unauthorized state and is rejected/ignored with inspectable evidence.

- [ ] **AC-10 — Duplicate and reordered movement intent (LAN-03):** Deliberately deliver a command repeatedly and reorder movement intents through the actual production validation path. Repetition produces no additional movement effect or multiplied speed. An older move cannot override a newer accepted stop/reversal, and replay of host-local input cannot execute it twice. Ordinary held walking still advances once per authoritative simulation step. These controls exercise real command handling, with observation separate from mutation.

- [ ] **AC-11 — Baseline, revision and lifetime ordering (LAN-03, LAN-04):** Deliver an obsolete baseline/update after a newer accepted state and deliver delayed commands/callbacks from a closed session after rehosting. None rewinds or mutates the current world or assigns control. State publication and readiness identify the intended session/revision. Old connection or adapter callbacks cannot release a current occupant's slot or operate on its new actor. Session teardown closes acceptance before deferred work can reenter.

- [ ] **AC-12 — Guest cleanup and slot reuse (LAN-01, LAN-02, LAN-03):** Graceful guest leave removes exactly that fixture placeholder on host and peers and frees exactly its slot. An unresponsive guest is declared lost on the first session update at or after the configured no-response threshold, default 10 seconds, and receives the same cleanup; controlled-time checks distinguish immediately below from at the threshold. Remaining players keep their actors, positions and input ownership. A replacement guest can use the slot, receives a fresh placeholder, and old actor/connection work cannot control or delete it. Cleanup repeated after departure has no additional effect.

- [ ] **AC-13 — Host departure and rehosting (LAN-01, LAN-03):** Graceful host leave ends this foundation session for all participants; unexpected host loss does so once the configured loss condition is met. Guests receive a visible explanation and return to a usable menu. No host migration occurs. A subsequent Host/Join starts a fresh session without previous actors, retained reservations, input, messages or listeners. Saving and saved-run recovery are outside this phase.

- [ ] **AC-14 — Stable local profiles and exact identity (LAN-04):** A new local profile receives one stable opaque player identity, retained through repeated application restarts and endpoint/peer-number changes. Separate profiles, including four processes on one machine, retain distinct identities even with identical display names. Identifiers compare exactly without case folding, label substitution or truncation. Missing/invalid incoming identity is refused before ownership; a second active connection claiming an already-connected identity is rejected without displacing its owner. The profile's player identity and the fixture actor/session lifetimes remain distinct.

- [ ] **AC-15 — Replaceable service proofs (LAN-04):** Demonstrate separate substitutions for discovery, identity and connection adapters through the established service boundary while keeping gameplay-rule source unchanged and preserving the relevant join/control semantics. An absent/unavailable service reports a clear failure rather than silently choosing an external account/service. Adapter checks exercise the actual composition/authority boundary; substitute-service tests do not stand in for the real LAN-process smoke run. Game rules require no LAN addresses, transient peer numbers or discovery implementation knowledge.

- [ ] **AC-16 — Stored small checks and two-platform execution (LAN-01–LAN-04):** Store readable foundation cases and executable smoke entry points for the delivered behaviors, with exact case identities, commands, bounded waits/cleanup, result locations and rendered inspection steps. Execute the real host plus three guest processes on Windows and on Linux, using isolated profiles, session/port allocation and per-peer evidence. Cover both discovery/manual joining, simultaneous movement/cameras, current-state readiness, capacity/error cases, guest cleanup and rehosting; include the actual command/identity/substitution controls. Keep the ordinary Phase 2 solo flow as a regression check.

- [ ] **AC-17 — Fresh, complete and inspectable evidence (LAN-01–LAN-04):** Each run has fresh OS/toolchain/session/profile/actor identities, per-peer results/logs and required rendered images/checkpoints for browser/manual-join feedback, the four-participant scene, walking/facing/cameras and return to menu. Include timing/state records for AC-07. Success requires every required current result and image; deliberate zero-case execution, missing/duplicate-substitute results, missing images, stale evidence, crashes and timeouts fail or report blockers. Reordering valid case records does not change coverage. Concurrent runs isolate evidence/profiles/ports/cleanup or are explicitly refused; one run cannot reuse or delete another's evidence. Both OS runs are required before phase completion.

- [ ] **AC-18 — Local, self-contained increment and honest evidence scope (LAN-01, LAN-04):** The foundation's hosting/discovery/joining works without an external account or Internet service, and runtime/check resources resolve without the creator's external donor folders or absolute development paths. Report the tested environment accurately. Local four-process and adapter-substitution passes do not certify actual mixed-computer LAN, full A-38/A-45, packaged release, campaign recovery, final performance or endurance.

## Edge Coverage

**Coverage:** 20/20 applicable edges resolved · 0 unresolved

**Verification:** 20 explicit acceptance resolutions · 0 test backstops · 0 dismissals

The compiled edge probe ran over the four requirement statements above with authored shapes: LAN-01 numeric-range/collection/text/stateful/io; LAN-02 numeric-range/stateful/io; LAN-03 stateful/io; LAN-04 text/stateful/io. It returned 18 applicable rows and zero unclassified requirements. All engine-proposed rows are retained. Manual review added LAN-03 ordering and LAN-04 adjacency, both addressed by the creator's accepted authority/identity outcomes. The three grouped edge questions resolved every candidate explicitly; no fallback/default or held-out test obligation was substituted.

| Category | Requirement | Status | Resolution / Reason |
| --- | --- | --- | --- |
| boundary | LAN-01 | resolved; verification: explicit | AC-03–AC-04: host-only through four participants, fifth refused, last-slot race admits one, and default join timeout ends a pending admission. |
| adjacency | LAN-01 | resolved; verification: explicit | AC-02: duplicate announcements merge by session identity; equally named distinct sessions remain separate. |
| empty | LAN-01 | resolved; verification: explicit | AC-02–AC-04: no sessions is a usable empty list; blank/malformed addresses and failed/cancelled joins create no actor or retained reservation. |
| encoding | LAN-01 | resolved; verification: explicit | AC-02 and AC-14: Unicode labels are preserved; labels and manual addresses cannot replace or truncate stable identity. |
| ordering | LAN-01 | resolved; verification: explicit | AC-02: list refresh/reordering retains the selected session identity rather than a row index or name. |
| precision | LAN-01 | resolved; verification: explicit | AC-03–AC-04: capacity is an exact integer including pending admissions; default timeout is 30 seconds, with its threshold evaluated explicitly. |
| idempotency | LAN-01 | resolved; verification: explicit | AC-01, AC-04 and AC-12–AC-13: repeated transitions/admission/cleanup produce one session and at most one placeholder per ready participant. |
| concurrency | LAN-01 | resolved; verification: explicit | AC-03–AC-05 and AC-12: racing admissions, cancellation, ready-state publication and departure cannot leak slots or duplicate actors. |
| boundary | LAN-02 | resolved; verification: explicit | AC-06–AC-07: inherited input/scene bounds hold; timing and position maxima are inclusive and over-limit observations fail. |
| precision | LAN-02 | resolved; verification: explicit | AC-06–AC-07: authoritative 3 u/s walking retains the Phase 2 tolerance; remote timing and finite positions use documented measurements without rounding an over-limit result into a pass. |
| idempotency | LAN-02 | resolved; verification: explicit | AC-06, AC-08 and AC-10: aliases/repeat/duplicate delivery do not multiply speed; focus recovery requires a fresh press. |
| concurrency | LAN-02 | resolved; verification: explicit | AC-05–AC-08 and AC-12: four simultaneous movers, a joining peer, focus changes and guest removal preserve each remaining actor's control and camera. |
| idempotency | LAN-03 | resolved; verification: explicit | AC-09–AC-10: a unique accepted action has one authoritative effect across solo, host-local and guest input; repeated delivery adds no effect. |
| concurrency | LAN-03 | resolved; verification: explicit | AC-09–AC-11: sender ownership, readiness and session/revision ordering are checked on the actual authority path; obsolete work cannot mutate current state. |
| empty | LAN-04 | resolved; verification: explicit | AC-14–AC-15: a new local profile receives one identity; invalid incoming identity or unavailable service fails explicitly without an online fallback. |
| encoding | LAN-04 | resolved; verification: explicit | AC-14: opaque identifiers compare exactly, without case folding, display-name substitution or truncation. |
| idempotency | LAN-04 | resolved; verification: explicit | AC-14: the selected local profile retains its identity through repeated application launches and address/peer-number changes. |
| concurrency | LAN-04 | resolved; verification: explicit | AC-11–AC-15: duplicate identity connections cannot steal control; closed-session or old-adapter callbacks cannot affect the next session. |
| ordering | LAN-03 | resolved; verification: explicit | AC-10–AC-11: an older move cannot undo a newer stop, and an older baseline/revision cannot rewind newer state. Added by manual review; accepted in the authority/lifecycle group. |
| adjacency | LAN-04 | resolved; verification: explicit | AC-14: equal display names do not merge distinct profiles; the same connected identity cannot create a second owner. Added by manual review; accepted in the identity group. |

## Prohibitions (must-NOT)

**Coverage:** 0/0 additional applicable prohibitions · 0 unresolved

The adversarial recall and precision pass covered LAN-01 through LAN-04 after the edge review. Duplicate admissions, foreign/stale control, identity collisions, hidden slot leaks, lost stop commands, stale snapshots, teardown races and false evidence are engineering concerns with explicit acceptance criteria above. Online-only discovery, forced accounts or unrequested platform coupling are already excluded by the binding local-LAN scope and AC-18.

Generic injection and privacy/security controls are canon security-review concerns referred to $gsd-secure-phase; no duplicate bespoke prohibition is minted here. No additional values, safety, privacy, fairness or transparency prohibition remained after precision filtering. No fictional negative-test path, fixture or wired-check descriptor is introduced.

## Ambiguity Report

| Dimension | Score | Min | Status | Notes |
| --- | --- | --- | --- | --- |
| Goal Clarity | 0.95 | 0.75 | Met | One host and three independent synchronized walkers in the same basic scene. |
| Boundary Clarity | 0.95 | 0.70 | Met | Basic discovery and running-scene join included; complete campaign/session/persistence work retains later ownership. |
| Constraint Clarity | 0.85 | 0.65 | Met | Two OS runs, explicit movement/timing limits, session defaults, identity/service and lifetime constraints. Concrete mechanisms remain downstream work. |
| Acceptance Criteria | 0.90 | 0.70 | Met | Eighteen pass/fail criteria with all twenty raised edges explicitly resolved. |
| **Ambiguity** | **0.080** | **≤ 0.20** | **Passed** | 1 − (0.35 × 0.95 + 0.25 × 0.95 + 0.20 × 0.85 + 0.20 × 0.90) = 0.080. |

Assessment history:

- Initial, from roadmap/requirements: goal 0.90, boundaries 0.80, constraints 0.60, acceptance 0.65; ambiguity 0.235. Constraints and acceptance were below minimum.
- After round 1: goal 0.95, boundaries 0.90, constraints 0.60, acceptance 0.65; ambiguity 0.1925 (displayed 0.193). Overall ambiguity met the threshold, but constraints and acceptance remained below minimum.
- After round 2: goal 0.95, boundaries 0.95, constraints 0.85, acceptance 0.90; ambiguity 0.080. All minimums met.
- The creator explicitly approved proceeding to final probes and specification generation. All edge outcomes were then specified explicitly; no unresolved assumption is carried forward. Scores describe requirements clarity, not implemented or measured game quality.

## Interview Log

| Round | Perspective | Question summary | Decision locked |
| --- | --- | --- | --- |
| 1 | Researcher | Which joining experience belongs in Phase 3? | Include a basic LAN session list as well as manual joining. |
| 1 | Researcher | What happens to a departing guest's movement placeholder? | Remove the placeholder and free the slot; full survivor/NPC recovery remains later work. |
| 2 | Researcher / Simplifier | Are basic measured local movement targets sufficient? | Use inclusive 250 ms remote changes and 0.05 u agreement within 0.5 seconds after stopping; no simulated poor-network gate. |
| 2 | Simplifier | Running-scene joining or a waiting lobby? | Host enters immediately; guests receive current foundation state before control while existing players continue. |
| Gate | Clarity check | Proceed at ambiguity 0.080? | Creator selected Yes — finish the edge checks and write SPEC.md. |
| Edge 1 | Boundary / failure review | Capacity, discovery equality/order/Unicode, invalid input, repetition, cancellation and timeout? | Specify the complete proposed session/join group, including exact capacity and 30-second join default. |
| Edge 2 | Failure / lifetime review | Ready baseline, stale/duplicate/wrong-owner work, focus, disconnect, host loss and numeric thresholds? | Specify the complete proposed authority/lifecycle group, including 10-second loss default, session fencing and inclusive movement limits. |
| Edge 3 | Identity / verification review | Stable profiles, exact identity, duplicate ownership, adapter substitution and evidence environments? | Specify the complete proposed identity/verification group, including both actual OS runs and fresh rendered/per-peer evidence. |

---

*Phase: 03-four-player-lan-foundation*
*Spec created: 2026-11-14 (Europe/Paris)*
*Next step: $gsd-discuss-phase 3 — implementation decisions for the locked requirements.*
