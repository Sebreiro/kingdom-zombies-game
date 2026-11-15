---
phase: 02-playable-foundation
plan: "03"
subsystem: foundation-verification
status: complete
requirements-completed: [BASE-03]
key-files:
  created: [tools/phase2/evidence.cjs, tools/phase2/runner-fixture.cjs, tests/phase1/FoundationRunnerTests.cs, docs/workflow/foundation-checks.md, .planning/phases/02-playable-foundation/02-SCENARIOS.md]
  modified: [tools/phase2/check.cjs, tools/phase2/rendered.cjs, tools/phase2/required-cases.json, tools/phase2/windows-input.ps1]
completed: "2026-11-15"
---

# Plan 02-03: Repeatable foundation evidence

All three tasks are complete under the creator's amended policy. The stored catalog has 16 independent active cases; [02-SCENARIOS.md](02-SCENARIOS.md) records their importance, sources, steps, outcomes, selection and persistent run groups. Pure checks launch no game. Nine rendered cases share one 1280×720 process, use ordinary menu resets and test Quit last. Errors are collected, recoverable cases continue, and fatal/unusable state blocks remaining cases explicitly. Focused repair reports preserve original passing results and identities.

The runner rejects empty/missing/duplicate/stale results, bad paths/images, process failures/timeouts and concurrent desktop use. It records nonce/input/tool/revision/platform identities, actual input/state/capture evidence, immutable logs and appearance-review hashes. Review fixes isolate diagnostics by scenario, track owned mouse holds and retain scenario outcomes even when cleanup fails. No new project, package or game-control shortcut was added.

## Final evidence

| Selection | Result | Immutable invocation |
| --- | --- | --- |
| Core baseline | 3 pass, 1 fail | `269d8969-90cf-481d-a3fd-a88eb500e49c` |
| Runner controls | 3/3 pass | `41e1747f-29ad-4426-ae34-d42faff92a47` |
| Failed Core permission repair | 1/1 pass | `6bec74eb-439b-48bc-ad3e-6e503bd27252` |
| Initial rendered selection | 9/9 pass, 0 blocked | `d4f78d36-22b3-4296-9d69-c544afc2486d` |

The combined selection has **16/16 passing cases, zero missing**, and 50 required image-review bindings. The original Core failure remains failed; only its new focused repair supplies that case's acceptance. The initial rendered selection itself passed without repairs, so no final duplicate game run was needed under either the current chat instruction or the subsequent local strategy.

The final rendered process was PID `17240`, one launch, Windows 1280×720, 253.862 seconds, Space-activated Quit last and exit code zero. Cleanup confirmed no game/runner process or desktop lock. The actual reviewer inspected 22 distinct required PNG hashes and four supplementary focus/hover/pressed captures; no appearance defects were found. See [02-VISUAL-REVIEW.md](02-VISUAL-REVIEW.md).

`node tools/phase2/check.cjs --verify-completion .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` passed. Raw reports/images remain under `.phase2/runs/`; [the checked completion record](02-PLATFORM-EVIDENCE.json) preserves their paths/hashes and reviewer bindings. This is explicit scenario accounting, not Phase 5 dependency-aware reuse certification or other-environment release acceptance.

Implementation/review commits: `c3cb2d8`, `96c9380`, `f8dc717`, `e29f425`, `a710572`, `d087f15`, `1d0266f`. Detailed original findings/resolutions are in [02-REVIEW.md](02-REVIEW.md) and [02-REVIEW-FIX.md](02-REVIEW-FIX.md).
