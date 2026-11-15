---
phase: 02-playable-foundation
fixed_at: 2026-11-15T12:05:14Z
review_path: .planning/phases/02-playable-foundation/02-REVIEW.md
iteration: 1
findings_in_scope: 4
fixed: 4
skipped: 0
status: all_fixed
---

# Phase 2: Code Review Fix Report

**Fixed at:** 2026-11-15T12:05:14Z
**Source review:** .planning/phases/02-playable-foundation/02-REVIEW.md
**Iteration:** 1

**Summary:** Four findings in scope, four fixes committed, none skipped. Logic changes retain `fixed: requires human verification`; this report does not replace the independent code re-review or rendered acceptance.

## Fixed Issues

### CR-01: Linked production source violates the suppression scanner's sole-consumer assumption

**Files modified:** `tests/phase1/ConsumerCompilation.cs`, `tests/phase1/ExemptionValidator.cs`, `tests/phase1/SuppressionTests.cs`
**Commit:** 96c9380
**Status:** fixed: requires human verification
**Applied fix:** Declare the exact game/test ownership pair for `FoundationInput.cs`, retain each evaluated project identity, and scan shared bytes in every actual owner context. Reject zero, missing, duplicate and undeclared owner sets. A real game/test compiler-symbol control uses different conditional suppression attributes to prove both branches are checked.
**Verification:** Strict proof-project build passed. All three `SuppressionTests` passed, including the actual repository and undeclared-entry-point checks. Final exact exemption acceptance also passed.

### CR-02: Isolated test-consumer fixtures omit their new linked compiler input

**Files modified:** `tests/phase1/BuildFixture.cs`
**Commit:** f8dc717
**Status:** fixed: requires human verification
**Applied fix:** Copy exactly `src/godot-game/foundation/FoundationInput.cs`, creating its parent directory, for test-consumer fixtures. Reuse the same file-copy operation in tree copying; no game project or engine dependency was added to the test fixture.
**Verification:** Strict proof-project build passed. The four exact test-consumer policy controls passed: compliant custom-rule input, positional argument rejection, permitted Shouldly assertion and banned Xunit assertion rejection. Their existing clean setup gates remain mandatory. Final exact two-path xUnit producer relocation and early-record restore mutation proofs also passed.

### CR-03: One scenario's native diagnostic fails every later healthy scenario

**Files modified:** `tools/phase2/rendered.cjs`, `tools/phase2/runner-fixture.cjs`
**Commit:** e29f425
**Status:** fixed: requires human verification
**Applied fix:** Assess only the scenario's native-log slice, including the resources branch. Attribute startup output to the first scenario and record each scenario's log range. Keep the complete session log intact. Extract the actual scenario loop for a pure production-tracer control: an initial diagnostic fails the first scenario, recovery runs once, and the next healthy scenario passes while the original log remains unchanged.
**Verification:** Both Node syntax checks passed; `scenario-diagnostics` and integrated `repeatability` controls passed. Final C# `RepairedEvidenceRetainsOriginalIdentity` passed with the new control included.

### CR-04: A failed pressed-button observation leaves owned mouse input held

**Files modified:** `tools/phase2/rendered.cjs`, `tools/phase2/runner-fixture.cjs`
**Commits:** a710572, d087f15
**Status:** fixed: requires human verification
**Applied fix:** Record possible left-button holds before `mouse-down`/`click` dispatch; clear them only after acknowledged `mouse-up`/`click`. Reset and final cleanup share a release operation that attempts every owned keyboard and mouse release even after a failure, retains failed ownership for retry, and preserves the existing process/window/focus checks. Final cleanup still terminates the owned process and writes logs if releasing input fails. The re-review identified that a thrown cleanup error could replace the completed scenario array; d087f15 retains that array and attaches explicit cleanup failure and nested causes to the last executed scenario, preserving its original scenario status, observations, earlier passes and later blocked records.
**Verification:** Both Node syntax checks passed. Pure controls passed for failed mouse-down and click acknowledgements, failed observation cleanup, release-all despite an earlier key-release failure, failed-release retry and a successful click leaving no pending hold. Additional production-cleanup controls passed for a crashed game with owned held input and failed release acknowledgements, proving prior outcomes and partial evidence survive. Integrated repeatability passed directly and through the final focused C# proof; the exact C# repeatability method passed again after d087f15 in 3.883 seconds.

## Verification

All gates ran from the **main checkout** on development Windows; `workflow.use_worktrees=false`. No game window, native scenario/tracer process or VM was started. In-memory controls call the production tracer and scenario loop without native input. Parent-owned reports and unrelated files were preserved.

| Evidence | Result |
| --- | --- |
| `.phase2/review-fixes/cr01-suppression-final.xml` | 3 passed, 0 failed/skipped; 12.000 seconds CLI duration |
| `.phase2/review-fixes/cr02-consumers-exact.xml` | 4 passed, 0 failed/skipped; 72.506 seconds CLI duration |
| `.phase2/review-fixes/strict-build.log` | Whole solution strict build, 0 warnings/errors; 11.60 seconds |
| `.phase2/review-fixes/final-exact-regressions.xml` | 4 exact affected regressions passed, 0 failed/skipped; 55.281 seconds test duration |
| `.phase2/review-fixes/cr04-cleanup-repeatability.xml` | Exact C# repeatability regression after d087f15 passed, 0 failed/skipped; 3.883 seconds CLI duration |
| `.phase2/controls/51abc3a1-d0e2-49d5-92dd-51ad2d8cb179` | Direct integrated pure repeatability, diagnostic isolation, owned-input and cleanup-failure controls passed |

Initial diagnostic attempts are retained separately. A first test invocation omitted `PHASE1_ROOT` and failed setup; corrected root-aware sandbox invocations exposed MSBuild/process sandbox limitations. The accepted real consumer checks used the declared root and escalated process execution. An initial C# style failure was rolled back with Git and corrected before any fix commit. An initial conditional-pragma control was replaced with conditional attributes because Roslyn exposes both pragma directives as trivia; the final control proves distinct consumer contexts. An invalid exploratory wildcard filter selected no tests and supplies no evidence. None of these attempts is represented as a pass.

The existing broad graph report was supplied by the parent and was not rerun. Independent re-review and the declared grouped 1280×720 rendered run remain with the parent. No acceptance or finding-resolution claim is made beyond the recorded fix/check scope.

---

_Fixed: 2026-11-15T12:05:14Z_
_Fixer: the agent (gsd-code-fixer)_
_Iteration: 1_
