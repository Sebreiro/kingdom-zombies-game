# 04-37 Task 2 — bounded node repair

Status: checkpoint — R2 repair budget exhausted; source WIP preserved. Recorded after checkpoint `6883fdd` under `$gsd-autonomous --only 4`.

## Decision and retained state

`DECOMPOSE`: Task 2 has coupled restoration, completeness and producer-lifetime concerns. Apply the configured node-repair procedure with two repair attempts per subtask. This is an in-memory decomposition of existing Task 2; do not modify PLAN.md or add a phase/plan/task to the graph. Keep 37 plans / 74 tasks / 31 waves, six completed plans and 04-37 at 1/2 completed tasks until all required gates pass.

Task 1 commits `daf2512` and `66c1980` are preserved. The five pending Task 2 files and all immutable failed/passing records in [the checkpoint](04-37-CHECKPOINT.md) are retained. The unrelated concurrent Phase 6 specification commit `affc858` is preserved and is not this task's work. Never reset, revert, rebase, stash or discard these inputs.

The selected suite completed 17/18. `rules.godot-callsite-boundary` failed before its collision batch with stale injected `AdjacentCallSite` output and CS0103. The final restored native and solution builds pass, and the frozen 26 Core controls pass. None of those positives completes the failed collision or newly identified completeness/lifetime obligations.

## Subtasks and bounded ownership

| Subtask | Required outcome | Implementation scope | Initial repair budget |
| --- | --- | --- | ---: |
| R1 — current producer restoration | Regenerate actual current output before semantic cohort validation, execute compiler-valid intended collisions, and finish with a fresh restored positive. Preserve `RequireValid` and every original/current negative. | `tests/phase1/GodotCallSiteAdapterTests.cs`; an already planned helper consumer only if necessary. Maximum five paths. | 2 |
| R2 — complete durable producer proof | Enforce both observation-to-permission and permission-to-observation coverage; reject extra rows and missing observations; admit later independently observed cohorts without applying frozen 177-specific expectations to all cohorts; reproduce historical producer inputs without forever rehashing nonproducer test/hub/scene subjects against the live checkout. | `tests/phase1/EngineGeneratedTests.cs`, `tests/phase1/ExemptionValidator.cs`, `tests/phase1/GeneratedCallSiteAdapterTests.cs`, `docs/code/engine-generated-spans.json`, `docs/code/generated-callsite-adaptations.json`. Maximum five paths; edit only those actually needed. | 2 |
| R3 — current complete verification | After focused failed/affected/new controls pass, execute all 18 selected cases freshly, followed by restored native/solution builds, all 26 Core controls and exact frozen-input checks. Preserve every earlier failure. | Evidence and supported metadata only; a new code failure routes back to its specific repair subtask with that budget decremented. | 2 |

This inline repair may reopen consumers already owned by Task 1 and therefore exceed Task 2's original five-path union. Each repair subtask stays within five implementation paths and the entire plan remains within its original nine. This is the exact bounded coordinator amendment before editing those reopened paths; it grants no new implementation file or broader diagnostic permission.

## Invariants

- Preserve all five frozen 04-03 source files byte-for-byte throughout 37 and this repair. Its execution freeze is a checkpoint precondition, distinct from persistent Godot generator input identity.
- Identify actual generator inputs from the real producer/compilation, preserve their authenticated bytes/recipe when historical reproduction requires them, and retain observation-subject identities separately. Test/hub/scene hashes must not become permanent live generator-input gates merely because they were frozen for 37.
- Retain immutable original 220 syntax / 18 callsite / 16 Godot / 26 permission baselines and the frozen 177-observation descriptor. Later cohorts require their own independently authenticated observations, source/package/input identities and exact complete coverage. No registry-derived expected inventory, arbitrary count changes, broad suppression, wildcard producer or absent-member approval.
- Reject compiler-invalid fixtures before accepting any intended collision/negative. Regenerate current producer state after injected cases; stale outputs cannot supply success or rejection credit.
- Keep the existing 66 syntax spans and eight member permissions only where supported by actual observations. Required new negative fixtures must execute and fail for the intended reason before the fixes are accepted.
- All source/headless implementation and independent whole-phase review/fixes precede real game windows. No window, listening or judgment credit; all ASSET requirements remain pending.

## Retry accounting and closeout

- R1 attempt 1 passed: restored-current build now precedes semantic inventory validation after the adjacent-source injection. Actual `rules.godot-callsite-boundary` invocation `5a7d412b-a228-4036-aabd-ee8eebdd752f` passes all original/current collisions and the final restored positive. Budget remaining: 2 (no failed repair attempt). Earlier sandbox invocation `4fee326b-c55d-4a92-819c-bef9a0732662` blocked on restore; the authorized native rerun succeeded. No collision or semantic validity check was weakened.
- R2 new controls genuinely failed before validation fixes: `aa2bea89-590f-4834-afcc-eb5fa1420143` accepts an extra unobserved row, and `9add11ec-98bd-442b-9fa3-424d4d5e6336` accepts missing observations. Preparation run `af16d6c1-5346-492c-a4f8-43f6a9c5d050` found a test line-format error and supplied no RED assertion credit.
- R2 attempt 1 compilation failed on three MA0006 ordinal string comparisons and one IDE0055 formatting issue in the new helper. Budget remaining: 1. Preserve these failures; the next correction must satisfy all R2 gates or checkpoint.
- R2 attempt 2 compilation failed: `EngineGeneratedTests.cs(618,20)` MA0002 (dictionary removal overload), `(247,35)` IDE0055, `(504,25)` SCR0006 (66-line mutation method). Actual strict test-project command returned exit 1, zero warnings and three errors at approximately 2026-11-16T18:44Z. These are current repair-source defects, not baseline failures and not assertion RED credit. **R2 budget remaining: 0.** Stop source corrections and return to the coordinator. R1 remains passing with budget 2; R3 remains unstarted with budget 2.
- R3: not yet executed; initial budget 2.

The two independent pre-fix rejection assertions executed as actual compiled tests. The earlier malformed-test preparation and both implementation compiler failures remain disclosed separately. No second full selected-suite attempt, post-repair restored native/solution/Core pass, Task 2 implementation commit, SUMMARY or progress update occurred.

Current source checkpoint: `.phase4/checkpoints/04-37-repair-r2-exhausted-711c7010-9239-42c5-b741-419cbd9dd509/manifest.json`. It preserves six uncommitted implementation files; the original five-file backup remains untouched. R1 adds only `GodotCallSiteAdapterTests.cs`; R2 edits only `EngineGeneratedTests.cs` and the two existing JSONs. No extra implementation path was introduced.

R2 WIP introduces bidirectional exact observation coverage, mandatory observations for nonbaseline inventories, frozen177 checks scoped to the named frozen cohort, original-subject hash matching for overlapping real inputs, and tracked authenticated compiler snapshots plus bounded isolated replay. The persistent input recipe comprises all 43 authored Godot C# compilation files excluding the two suppression adapters, plus the actual Godot csproj (44 snapshots / 201,112 raw bytes); the baseline recipe excludes the separately authenticated InspectionRoot. Current suppression adapters remain the existing proof mechanism. Test/hub/scene are retained immutable observation subjects, not permanent live generator-input gates. Snapshot hashes are preserved rather than relabeled; no ignored historical folder is required by the new replay path. The new nonproducer-change and historical-reproduction controls are written but **not executed**, because the test project currently fails compilation. Extra member rejection is likewise written but unexecuted; the two original new row/missing-observation REDs do not certify those controls. Future-cohort admission and replay require further assessment and execution before acceptance.

All five frozen03 source hashes were checked again after preserving this WIP and remain exact. The original 66 syntax spans/eight attributes, generator package, original observations and four retained raw outputs were not edited by this repair. No new window, source-policy/visibility, asset or requirement acceptance is supplied.

The prior three direct corrections are preserved history. This is the first structured node repair; budgets above must not be silently reset. Record each concrete failed attempt and remaining budget here. If a subtask exhausts its budget, return an honest checkpoint with the attempted changes, exact remaining blocker and source/evidence identities.

Do not create a completed SUMMARY, commit Task 2 as complete or advance progress until the entire required gate passes. The eventual SUMMARY must disclose `[Node Repair - DECOMPOSE] Task 2` and its subtask outcomes, the existing bounded fifth-file amendment and all retained failures. Preserve concurrent edits and stage only this plan's owned changes. Parent owns draft PR #5 and the continuation document.
