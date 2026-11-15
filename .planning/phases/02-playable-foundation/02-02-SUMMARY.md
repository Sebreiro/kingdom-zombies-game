---
phase: 02-playable-foundation
plan: "02"
subsystem: gameplay-foundation
status: complete
requirements-completed: [BASE-01, BASE-02]
key-files:
  created: [tests/phase1/FoundationLifecycleTests.cs]
  modified: [src/core/FoundationSession.cs, src/godot-game/foundation/FoundationRoot.cs, src/godot-game/foundation/FoundationInput.cs, src/godot-game/foundation/FoundationView.cs, tests/phase1/GraphFixture.cs]
completed: "2026-11-15"
---

# Plan 02-02: Complete controls and scene lifetime

Both tasks are complete under the creator's amended testing order. A/D and arrows normalize to one command, opposites cancel, repeat/release and fresh-press barriers work, movement stays identified at 3u/s, extreme elapsed time saturates safely, and camera smoothing/bounds keep the actor visible. Start/Escape handle held/repeated activation, actual world-node teardown and fresh sessions. Closed authority rejects stale commands. Menu focus, error/retry and clean keyboard Quit are delivered.

Source commits include `68e39da`, `5450f0e` and graph-fixture repair `7cd3b2b`. Review fixes `96c9380` and `f8dc717` preserve shared-source suppression checking and isolated test dependencies. The complete phase review is clean; its original findings remain in [02-REVIEW.md](02-REVIEW.md).

## Verification

- Strict solution builds passed with zero warnings/errors; the last compiler-proof adjustment build took 6.56 seconds. `1d0266f` aligns the generated-origin proof with strict build flags without changing its 30-second deadline.
- Original final Core run `269d8969-90cf-481d-a3fd-a88eb500e49c` passed authority/input/lifecycle and failed the compiler-permission timeout. Focused repair `6bec74eb-439b-48bc-ad3e-6e503bd27252` passed that single failed case. Both reports remain immutable.
- Actual graph suite: 50/50 passed, zero failed/skipped, 998.362 seconds; `.phase2/implementation-only-20261115/graph-all.json`. Focused forbidden-edge and compiled type-leak checks passed too.
- Shared rendered run `d4f78d36-22b3-4296-9d69-c544afc2486d` passed all nine scenarios in one Windows 1280×720 process in 253.862 seconds, using real input. It tested both direction families, aliases/opposites/repeats, signed one-second distances, scenery traversal/bounds, repeated restarts, focus transfer, resource closure and Quit last.
- Actual appearance review passed. [02-PLATFORM-EVIDENCE.json](02-PLATFORM-EVIDENCE.json) binds original reports and 50 required image-review references; the completion validator passed with no missing cases.

Earlier exploratory failures, including focus refusal, observation timeout and fixture coupling, remain historical reports. They are not overwritten or presented as passes. No VM or additional resolution was executed after the strategy change. Routine and release evidence follow the current creator policy; this phase supplies no new other-OS/release certification.
