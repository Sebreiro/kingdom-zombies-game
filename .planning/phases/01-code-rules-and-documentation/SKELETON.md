# Walking Skeleton — KingdomZombies

**Phase:** 1 · **Generated:** 2026-11-13 · **Status:** planned; no implementation/platform pass claimed.

## Capability Proven End-to-End

A developer launches the existing Godot project through pinned local commands, observes real C# lifecycle/state assertions, and receives a trustworthy result that accepts compliant inputs and rejects intended violations.

## Architectural Decisions

| Decision | Choice | Rationale |
| --- | --- | --- |
| Framework | Existing `src/godot-game/project.godot`, Godot .NET/C# | Locked D-07–D-09; one launch target and minimal proof scene. |
| Toolchain | First probe stable Godot4.7.2, SDK10.0.404, explicit C#14/net10; lock only after native-host compatibility proof | D-14–D-16; real evidence controls stable fallback, not metadata. |
| Core | Framework-only C# state/time/random/identity examples | Independent of engine objects and concrete providers. |
| Services | Independent Multiplayer.Abstractions port assembly | Future services replaceable; no LAN implementation in Phase1. |
| Data | Strict detached internal example snapshot plus atomic immutable in-memory root replacement | AC-18–22; no database or production save system. |
| Auth | No account/session surface | Explicit phase boundary. |
| Development target | Actual Windows and actual Linux native engine execution | AC-02; instructions or cross-builds do not substitute. |
| Rule authority | Executable definitions indexed by85-row local register | D-01–D-04;84 heading relations and24 source exemptions remain traceable. |
| Verification | One C# xUnit proof project; dependency-free Node coordinator | D-09; independent required IDs, process self-failures, fresh evidence, exact coverage. |
| Directory layout | Core, service ports, existing game, analyzer, Phase1.Tests | Exactly five owned projects; no web topology. |

## Stack Touched in Phase 1

- [ ] Pinned workspace-local bootstrap, restore/build, strict analyzers and trustworthy proof runner.
- [ ] Real engine-created scene/resources, exports, generated/handwritten partials, callbacks, signals, two unload cycles and thread/lifetime guards.
- [ ] Real controlled state advance and snapshot read/write/atomic restore.
- [ ] Exact graph, source rule inventory and portable documentation navigation.
- [ ] Actual clean same-source full proofs on Windows and Linux with measured90%line/80%branch gates.

Generic web skeleton expectations are adapted to the approved game tooling phase: scene entry replaces a web route; detached snapshot read/write replaces a database operation; developer command/result interaction replaces player UI; native local execution replaces a web deployment. No HTTP, database, account, menu, HUD or gameplay work is introduced.

## Out of Scope

Playable/menu/input foundation belongs to Phase2, LAN foundation Phase3, asset production Phase4, comprehensive test framework/GitHub Actions Phase5, gameplay/state/save UX later phases, release exports/performance/endurance Phase10. This skeleton proves development contracts only.

## Subsequent Slice Plan

Phases2–10 retain their approved roadmap order and ownership. They consume the enforced five-project boundaries, controlled inputs, exact identity/state contracts and proof commands; none is claimed delivered here.

## Implementation Order

- Wave1: 01-01 — Prove the pinned native-host toolchain.
- Wave2: 01-02 — Make Phase 1 results trustworthy before using them.
- Wave3: 01-03 — Capture the complete donor baseline and executable register.
- Wave4: 01-04 — Build the five owned C# boundaries under evaluated configuration.
- Wave5: 01-18 — Prove permitted composition and compiler-test configuration, including all five generated dependency locks.
- Wave6: 01-05 — Prove inherited constructor, directive and call-site rules.
- Wave7: 01-06 — Prove null, member-size and source-layout rules.
- Wave8: 01-07 — Prove continuation, import and local-declaration rules.
- Wave9: 01-08 — Reject real build violations and bounded-exemption escapes.
- Wave10: 01-09 — Reject evaluated dependency and compiled-type leaks.
- Wave11: 01-10 — Prove controlled time, randomness and stable identities.
- Wave12: 01-11 — Validate strict snapshots and replace state atomically.
- Wave13: 01-12 — Prove actual Godot lifetimes, exports, signals and thread access.
- Wave14: 01-13 — Validate focused documentation with portable exact navigation.
- Wave15: 01-14 — Bind every project and inherited rule to its real proof.
- Wave16: 01-15 — Measure real analyzer and native-hosted managed coverage.
- Wave17: 01-16 — Prove clean Windows checkout and prepare genuine Linux execution.
- Wave18: 01-17 — Execute Linux and reconcile complete two-platform proof.

Shared rule/register/proof project outputs and engine build/coverage artifacts force ordered execution. This keeps evidence generation isolated and avoids concurrent edits to common configuration. Each plan has2–3 bounded proof increments.

